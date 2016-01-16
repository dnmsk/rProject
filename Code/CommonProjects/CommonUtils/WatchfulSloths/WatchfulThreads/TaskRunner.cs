using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.WatchfulSloths.WatchfulThreads {
    public class TaskRunner : Singleton<TaskRunner>, IDisposable {
        private readonly object _lockObject = new object();

        private ManualResetEventSlim _runManagerEventSlim;
        private ManualResetEventSlim _threadManagerEventSlim;

        private readonly ConcurrentQueue<WatchfulThread> _watchfulThreadsInSleep;
        private readonly HashSet<WatchfulThread> _watchfulThreadsInProgress;
        private readonly HashSet<WatchfulThread> _watchfulThreadsInRunner;
        private readonly ConcurrentQueue<Action<WatchfulThread>> _actionsToExecute;
        private const int _minThreads = 4;
        private const int _maxThreads = 36;
        private static readonly long _updaMinThreadsTimeout = (long)TimeSpan.FromMinutes(5).TotalMilliseconds;
        private static readonly long _logTraceTimeout = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;

        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(TaskRunner).FullName);

        public TaskRunner() {
            _runManagerEventSlim = new ManualResetEventSlim(false);
            _threadManagerEventSlim = new ManualResetEventSlim(false);

            _watchfulThreadsInProgress = new HashSet<WatchfulThread>();
            _watchfulThreadsInRunner = new HashSet<WatchfulThread>();

            _watchfulThreadsInSleep = new ConcurrentQueue<WatchfulThread>();
            _actionsToExecute = new ConcurrentQueue<Action<WatchfulThread>>();
            
            new Thread(ThreadManager).Start();
            new Thread(RunManager).Start();
        }

        public void AddAction(Action action) {
            AddAction(thread => action());
        }

        private void AddAction(Action<WatchfulThread> action) {
            if (ConfigHelper.TestMode) {
                action(null);
            } else {
                lock (_lockObject) {
                    _actionsToExecute.Enqueue(action);
                }
                _runManagerEventSlim.Set();
            }
        }
        
        private void RunManager() {
            var launchedTask = 0;
            var holderLoops = 0;
            var measurement = Stopwatch.StartNew();
            while (_runManagerEventSlim != null) {
                Action<WatchfulThread> nextAction;
                while (_actionsToExecute.TryDequeue(out nextAction)) {
                    WatchfulThread thread;
                    if (_watchfulThreadsInSleep.TryDequeue(out thread)) {
                        lock (_lockObject) {
                            _watchfulThreadsInProgress.Add(thread);
                        }
                        thread.SetAction(nextAction);
                        launchedTask++;
                    } else {
                        _actionsToExecute.Enqueue(nextAction);
                    }
                }

                if (measurement.ElapsedMilliseconds > _logTraceTimeout) {
                    int freeSloths;
                    int inProgressSloths;
                    int waitTasks;
                    lock (_lockObject) {
                        freeSloths = _watchfulThreadsInSleep.Count;
                        inProgressSloths = _watchfulThreadsInProgress.Count;
                        waitTasks = _actionsToExecute.Count;
                    }
                    _logger.Info("Статистика хомяков: занято={0} отдыхают={1} задач_в_ожидании={2} задач_выполнено={3} запусков={4}", inProgressSloths, freeSloths, waitTasks, launchedTask, holderLoops);
                    launchedTask = 0;
                    holderLoops = 0;
                    measurement.Restart();
                }
                _runManagerEventSlim?.Wait();
                _runManagerEventSlim?.Reset();
                holderLoops++;
            }
        }

        private void ThreadManager() {
            var defaultMinThreads = _minThreads;
            var creationsCount = 0;
            var measurement = Stopwatch.StartNew();
            var infoDebug = new Action<string>(s => {
                // ReSharper disable once AccessToModifiedClosure
                _logger.Info("{0}, total:{1} min:{2} creations:{3}", s, _watchfulThreadsInRunner.Count, defaultMinThreads, creationsCount);
            });

            while (_threadManagerEventSlim != null) {
                var totalThreadsCount = _watchfulThreadsInRunner.Count;
                var threadsInSleep = _watchfulThreadsInSleep.Count;
                var actionsQueueCount = _actionsToExecute.Count;

                if (measurement.ElapsedMilliseconds > _updaMinThreadsTimeout) {
                    measurement.Restart();
                    if (creationsCount > _maxThreads/2) {
                        defaultMinThreads++;
                    } else if (creationsCount == default(int) && threadsInSleep > _minThreads && defaultMinThreads > _minThreads) {
                        defaultMinThreads--;
                    }
                    creationsCount = 0;
                }

                var needLaunchThread = totalThreadsCount < _minThreads ||
                                 actionsQueueCount > default(int) && threadsInSleep == default(int) && totalThreadsCount < _maxThreads;
                if (needLaunchThread) {
                    creationsCount++;
                    AddThread();
                    infoDebug("Add");
                    continue;
                }

                var needDisposeThread = threadsInSleep > defaultMinThreads;
                if (needDisposeThread) {
                    infoDebug("Dispose");
                    AddAction(thread => {
                        thread.Dispose();
                    });
                }

                _threadManagerEventSlim?.Wait(TimeSpan.FromMilliseconds(100));
            }
        }

        private void AddThread() {
            var watchfulThread = new WatchfulThread(OnWorkDoneThreadAction, OnDisposeThreadAction);
            _watchfulThreadsInRunner.Add(watchfulThread);
            _watchfulThreadsInSleep.Enqueue(watchfulThread);
        }

        private void OnWorkDoneThreadAction(WatchfulThread watchfullThread) {
            lock (_lockObject) {
                _watchfulThreadsInProgress.Remove(watchfullThread);
            }
            if (watchfullThread.CanWork()) {
                _watchfulThreadsInSleep.Enqueue(watchfullThread);
            }
            _runManagerEventSlim?.Set();
        }

        private void OnDisposeThreadAction(WatchfulThread watchfullThread) {
            _watchfulThreadsInRunner.Remove(watchfullThread);
        }

        public void Dispose() {
            var eventSlim = _threadManagerEventSlim;
            _threadManagerEventSlim = null;
            eventSlim.Set();

            eventSlim = _runManagerEventSlim;
            _runManagerEventSlim = null;
            eventSlim.Set();

            lock (_lockObject) {
                _watchfulThreadsInSleep.ToArray().Each(t => t.Dispose());
                _watchfulThreadsInProgress.Each(t => t.Dispose());
            }
        }
    }
}
