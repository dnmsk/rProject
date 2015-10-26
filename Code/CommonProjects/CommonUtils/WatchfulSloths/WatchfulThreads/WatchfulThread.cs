using System;
using System.Threading;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.WatchfulThreads {
    public class WatchfulThread {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WatchfulThread).FullName);

        private readonly Action<WatchfulThread> _onWorkDoneAction;
        private readonly Thread _thread;
        private Action _action = null;
        private bool _canWork = true;

        public WatchfulThread(Action<WatchfulThread> onWorkDoneAction) {
            _onWorkDoneAction = onWorkDoneAction;
            _thread = new Thread(Action);
            _thread.Start();
        }

        public bool IsFree() {
            return _action == null;
        }

        public bool SetAction(Action action) {
            if (IsFree()) {
                lock (_thread) {
                    if (IsFree()) {
                        _action = action;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Kill() {
            _canWork = false;
        }

        private void Action() {
            while (_canWork) {
                if (_action != null) {
                    try {
                        _action();
                    } catch (Exception ex) {
                        _logger.Error(ex);
                    }
                    lock (_thread) {
                        _action = null;
                    }
                    _onWorkDoneAction(this);
                    continue;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
