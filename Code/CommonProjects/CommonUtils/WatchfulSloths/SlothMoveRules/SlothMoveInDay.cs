using System;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class SlothMoveInDay<T> : SlothMoveInTimeBase<T> {
        /// <summary>
        /// Время запуска задачи 
        /// </summary>
        protected DateTime _time;

        public SlothMoveInDay(Func<T> move, DateTime time, IWatch watch = null) : base(move, watch) {
            _time = time;
        }

        public override bool IsNeedMove {
            get {
                var now = _watch.Now();
                DateTime timeToExecute = new DateTime(now.Year, now.Month, now.Day, _time.Hour, _time.Minute, 0);

                if (LastMoveDate.HasValue && LastMoveDate.Value >= timeToExecute) {
                    return false; // Уже выполнили.
                }

                // Пришло время. Выполняем задачу. 
                return TimeComparer.TimeEquals(now, _time);
            }
        }
    }
}