using System;
using System.Threading;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.WatchfulThreads {
    public class WatchfulThread : IDisposable {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WatchfulThread).FullName);

        private readonly Action<WatchfulThread> _onWorkDoneAction;
        private readonly Thread _thread;
        private Action _action;
        private ManualResetEventSlim _eventSlim;

        public WatchfulThread(Action<WatchfulThread> onWorkDoneAction) {
            _eventSlim = new ManualResetEventSlim(false);
            _onWorkDoneAction = act => {
                onWorkDoneAction(act);
                _action = null;
                _eventSlim.Reset();
            };
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
                        _eventSlim.Set();
                        return true;
                    }
                }
            }
            return false;
        }

        public void Dispose() {
            var eventSlim = _eventSlim;
            _eventSlim = null;
            eventSlim.Set();
        }

        private void Action() {
            while (_eventSlim != null) {
                if (_action != null) {
                    try {
                        _action();
                    } catch (Exception ex) {
                        _logger.Error(ex);
                    }
                    _onWorkDoneAction(this);
                }
                _eventSlim.Wait();
            }
        }
    }
}
