using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.WatchfulSloths.WatchfulThreads {
    public class WatchfulHolder : IDisposable {
        private readonly string _holderName;
        private readonly object _lockObject = new object();
        private ManualResetEventSlim _eventSlim;
        private readonly Queue<WatchfulThread> _watchfulThreadsInSleep;
        private readonly HashSet<WatchfulThread> _watchfulThreadsInProgress;
        private readonly Queue<Action> _actionsToExecute;

        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WatchfulHolder).FullName);

        public WatchfulHolder(int minWatchfulCount, string holderName) {
            _eventSlim = new ManualResetEventSlim(false);
            _holderName = holderName;
            _watchfulThreadsInSleep = new Queue<WatchfulThread>(minWatchfulCount);
            _watchfulThreadsInProgress = new HashSet<WatchfulThread>();
            _actionsToExecute = new Queue<Action>();
            minWatchfulCount.Steps(i => AddThread());

            new Thread(RunHolder).Start();
        }

        private void AddThread() {
            _watchfulThreadsInSleep.Enqueue(new WatchfulThread(watchfullThread => {
                lock (_lockObject) {
                    _watchfulThreadsInProgress.Remove(watchfullThread);
                    _watchfulThreadsInSleep.Enqueue(watchfullThread);
                }
                _eventSlim.Set();
            }));
        }

        public void AddTask(Action action) {
            lock (_lockObject) {
                _actionsToExecute.Enqueue(action);
            }
            _eventSlim.Set();
        }

        private static readonly int _logDelay = (int) TimeSpan.FromMinutes(30).TotalMilliseconds;

        private void RunHolder() {
            var launchedTask = 0;
            var holderLoops = 0;
            var measurement = Stopwatch.StartNew();
            while (_eventSlim != null) {
                bool haveTask;
                lock (_lockObject) {
                    haveTask = _actionsToExecute.Count != 0;
                }
                if (haveTask) {
                    var haveThreads = true;
                    while (haveTask && haveThreads) {
                        lock (_lockObject) {
                            haveTask = _actionsToExecute.Count != 0;
                            haveThreads = _watchfulThreadsInSleep.Count != 0;
                            if (haveTask && haveThreads) {
                                var nextThread = _watchfulThreadsInSleep.Dequeue();
                                nextThread.SetAction(_actionsToExecute.Dequeue());
                                _watchfulThreadsInProgress.Add(nextThread);
                                launchedTask++;
                            }
                        }
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

                    _logger.Info("Статистика хомяков ({0}): занято={1} отдыхают={2} задач_в_ожидании={3} задач_выполнено={4} запусков={5}", _holderName, inProgressSloths, freeSloths, waitTasks, launchedTask, holderLoops);
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
                _watchfulThreadsInSleep.Each(t => t.Dispose());
                _watchfulThreadsInProgress.Each(t => t.Dispose());
            }
        }
    }
}
