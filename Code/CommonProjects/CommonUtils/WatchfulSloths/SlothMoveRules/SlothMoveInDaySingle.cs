using System;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class SlothMoveInDaySingle<T> : SlothMoveInDay<T> {
        /// <summary>
        /// Коеструктор
        /// </summary>
        public SlothMoveInDaySingle(Func<T> move, DateTime time, IWatch watch = null) : base(move, time, watch) {}

        /// <summary>
        /// Запущена ли таска
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Объект для логирования ошибок и отладочной информации.
        /// </summary>
        private new static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SlothMoveInDaySingle");

        private readonly object _lockObj = new object();

        public override bool IsNeedMove {
            get {
                if (_isRunning) {
                    _logger.Info("Отказался выполнять таск, предыдущий не завершен.");
                }
                return !_isRunning && base.IsNeedMove;
            }
        }

        public override void Move(int wakeUpInterval) {
            lock (_lockObj) {
                if (_isRunning) {
                    _logger.Info("Отменил таск, предыдущий не завершен.");
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