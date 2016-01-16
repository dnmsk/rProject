using System;
using System.Threading;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.WatchfulThreads {
    internal class WatchfulThread : IDisposable {
        public int ThreadID => _thread.ManagedThreadId;

        private readonly Action<WatchfulThread> _onDisposeThreadAction;

        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WatchfulThread).FullName);

        private readonly Action<WatchfulThread> _onWorkDoneAction;
        private readonly Thread _thread;
        private Action<WatchfulThread> _action;
        private ManualResetEventSlim _eventSlim;

        public WatchfulThread(Action<WatchfulThread> onWorkDoneAction, Action<WatchfulThread> onDisposeThreadAction) {
            _onDisposeThreadAction = onDisposeThreadAction;
            _eventSlim = new ManualResetEventSlim(false);
            _onWorkDoneAction = act => {
                _action = null;
                _eventSlim?.Reset();
                onWorkDoneAction(act);
            };
            _thread = new Thread(Action);
            _thread.Start();
        }

        internal bool IsFree() {
            return _action == null;
        }

        internal bool SetAction(Action<WatchfulThread> action) {
            if (IsFree()) {
                lock (_thread) {
                    if (IsFree()) {
                        _action = action;
                        _eventSlim.Set();
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool CanWork() {
            return _eventSlim != null;
        }

        public void Dispose() {
            _onDisposeThreadAction(this);
            var eventSlim = _eventSlim;
            _eventSlim = null;
            eventSlim.Set();
        }

        private void Action() {
            while (_eventSlim != null) {
                _eventSlim?.Wait();
                if (_action != null) {
                    try {
                        _action(this);
                    } catch (Exception ex) {
                        _logger.Error(ex);
                    }
                    _onWorkDoneAction(this);
                }
            }
        }

        public override int GetHashCode() {
            return ThreadID;
        }
    }
}
