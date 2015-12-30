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
        private ManualResetEventSlim _eventSlim;
        private readonly ConcurrentQueue<WatchfulThread> _watchfulThreadsInSleep;
        private readonly HashSet<WatchfulThread> _watchfulThreadsInProgress;
        private readonly ConcurrentQueue<Action> _actionsToExecute;
        private const int _threads = 10;
        private static readonly int _logDelay = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;

        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(TaskRunner).FullName);

        public TaskRunner() {
            _eventSlim = new ManualResetEventSlim(false);
            _watchfulThreadsInSleep = new ConcurrentQueue<WatchfulThread>();
            _watchfulThreadsInProgress = new HashSet<WatchfulThread>();
            _actionsToExecute = new ConcurrentQueue<Action>();
            _threads.Steps(i => AddThread());

            new Thread(RunHolder).Start();
        }

        private void AddThread() {
            _watchfulThreadsInSleep.Enqueue(new WatchfulThread(watchfullThread => {
                lock (_lockObject) {
                    _watchfulThreadsInProgress.Remove(watchfullThread);
                }
                _watchfulThreadsInSleep.Enqueue(watchfullThread);
                _eventSlim.Set();
            }));
        }

        public void AddAction(Action action) {
            if (ConfigHelper.TestMode) {
                action();
            } else {
                lock (_lockObject) {
                    _actionsToExecute.Enqueue(action);
                }
                _eventSlim.Set();
            }
        }
        
        private void RunHolder() {
            var launchedTask = 0;
            var holderLoops = 0;
            var measurement = Stopwatch.StartNew();
            while (_eventSlim != null) {
                Action nextAction;
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

                if (measurement.ElapsedMilliseconds > _logDelay) {
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
                    measurement.Reset();
                }
                _eventSlim.Wait();
                _eventSlim.Reset();
                holderLoops++;
            }
        }

        public void Dispose() {
            var eventSlim = _eventSlim;
            _eventSlim = null;
            eventSlim.Set();
            lock (_lockObject) {
                _watchfulThreadsInSleep.ToArray().Each(t => t.Dispose());
                _watchfulThreadsInProgress.Each(t => t.Dispose());
            }
        }
    }
}
