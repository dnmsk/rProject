using System;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class SlothMoveInWeekSingle<T> : SlothMoveInDay<T> {
        private readonly DayOfWeek _dayOfWeek;

        /// <summary>
        /// Коеструктор
        /// </summary>
        public SlothMoveInWeekSingle(Func<T> move, DateTime time, DayOfWeek dayOfWeek, IWatch watch = null) : base(move, time, watch) {
            _dayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// Запущена ли таска
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Объект для логирования ошибок и отладочной информации.
        /// </summary>
        private new static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SlothMoveInWeekSingle");

        private readonly object _lockObj = new object();

        public override bool IsNeedMove {
            get {
                if (_isRunning) {
                    _logger.Info("Отказался выполнять таск, предыдущий не завершен.");
                    return false;
                }
                var now = _watch.Now();

                if (_dayOfWeek != now.DayOfWeek) {
                    return false;
                }

                var timeToExecute = new DateTime(now.Year, now.Month, now.Day, _time.Hour, _time.Minute, 0);

                if (LastMoveDate.HasValue && LastMoveDate.Value >= timeToExecute) {
                    return false; // Уже выполнили.
                }

                // Пришло время. Выполняем задачу. 
                return TimeComparer.TimeEquals(now, _time);
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
