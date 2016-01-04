using System;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class SlothMoveByTimeSingle<T> : SlothMoveByTime<T> {
        private bool _isRunning;
        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SlothMoveByTimeSingle");

        private readonly object _lockObj = new object();

        public SlothMoveByTimeSingle(Func<T> move, TimeSpan moveInterval, T startValue, IWatch watch = null)
            : base(move, moveInterval, startValue, watch) {
            _isRunning = false;
        }

        public override bool IsNeedMove {
            get {
                return !_isRunning && base.IsNeedMove;
            }
        }

        public override void Move(int wakeUpInterval) {
            lock (_lockObj) {
                if (_isRunning) {
                    _logger.Info("Отменил таск, предыдущий не завершен. " + MoveName);
                    return;
                }
                _isRunning = true;
            }
            base.Move(wakeUpInterval);
            lock (_lockObj) {
                _isRunning = false;
            }
        }
    }
}
