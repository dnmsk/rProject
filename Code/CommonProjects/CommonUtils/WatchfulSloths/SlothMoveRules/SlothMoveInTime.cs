using System;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public abstract class SlothMoveInTimeBase<T> : SlothMoveRule<T> {
        protected readonly IWatch _watch;

        private readonly Func<T> _move;

        /// <summary>
        /// Время последнего запуска.
        /// </summary>
        protected DateTime? LastMoveDate { get; private set; }

        protected SlothMoveInTimeBase(Func<T> move, IWatch watch = null) {
            _watch = watch ?? new Watch();
            _move = move;
        }

        /// <summary>
        /// Движение.
        /// </summary>
        public override void Move(int wakeUpInterval) {
            try {
                LastMoveDate = _watch.Now().AddMilliseconds(-wakeUpInterval/2);
                Result = _move();
            } catch (Exception e) {
                _logger.Error(e);
            }
        }
    }
}
