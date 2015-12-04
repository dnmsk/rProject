using System;
using System.Collections.Generic;
using System.Threading;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.WatchfulSloths.WatchfulThreads {
    public class WatchfulHolder {
        private readonly string _holderName;
        private bool _canWork = true;
        private readonly object _lockObject = new object();
        private readonly Queue<WatchfulThread> _watchfulThreadsInSleep;
        private readonly HashSet<WatchfulThread> _watchfulThreadsInProgress;
        private readonly Queue<Action> _actionsToExecute;
        private readonly int _delayTimeout = 500;

        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WatchfulHolder).FullName);

        public WatchfulHolder(int minWatchfulCount, int delayTimeout, string holderName) {
            _holderName = holderName;
            _delayTimeout = delayTimeout;
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
            }));
        }

        public void Kill() {
            _canWork = false;
        }

        public void AddTask(Action action) {
            lock (_lockObject) {
                _actionsToExecute.Enqueue(action);
            }
        }

        private void RunHolder() {
            var cnt = 0;
            var launchedTask = 0;
            var traceValue = 15 * 60 * 1000 / _delayTimeout;

            while (_canWork) {
                cnt++;
                var haveTask = true;
                var haveThreads = true;

                lock (_lockObject) {
                    haveTask = _actionsToExecute.Count != 0;
                }
                if (haveTask) {
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

                if (cnt == traceValue) {
                    int freeSloths;
                    int inProgressSloths;
                    int waitTasks;
                    lock (_lockObject) {
                        freeSloths = _watchfulThreadsInSleep.Count;
                        inProgressSloths = _watchfulThreadsInProgress.Count;
                        waitTasks = _actionsToExecute.Count;
                    }

                    _logger.Info("Статистика хомяков ({0}): занято={1} отдыхают={2} задач_в_ожидании={3} задач_выполнено={4}", _holderName, inProgressSloths, freeSloths, waitTasks, launchedTask);
                    cnt = 0;
                    launchedTask = 0;
                }
//                if (haveTask && !haveThreads) {
//                    
//                }
                Thread.Sleep(_delayTimeout);
            }
            ShutdownAll();
        }

        private void ShutdownAll() {
            lock (_lockObject) {
                _watchfulThreadsInSleep.Each(t => t.Kill());
                _watchfulThreadsInProgress.Each(t => t.Kill());
            }
        }
    }
}
