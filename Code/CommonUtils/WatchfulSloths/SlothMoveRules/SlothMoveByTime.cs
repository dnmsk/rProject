using System;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    /// <summary>
    /// Правило движений ленивца по времени с сохраненным результатом.
    /// </summary>
    public class SlothMoveByTime<T> : SlothMoveRule<T> {
        private readonly Func<T> _move;
        
        protected readonly TimeSpan _moveInterval;

        protected DateTime? _prevMove;

        protected readonly IWatch _watch;

        public SlothMoveByTime(Func<T> move, TimeSpan moveInterval, T startValue, IWatch watch = null) : base(startValue) {
            _move = move;
            _moveInterval = moveInterval;
            _watch = watch ?? new Watch();
        }

        /// <summary>
        /// Нужно двигаться.
        /// </summary>
        public override bool IsNeedMove {
            get {
                return !_prevMove.HasValue 
                    || (_watch.Now() - _prevMove.Value) > _moveInterval;
            }
        }

        /// <summary>
        /// Движение.
        /// </summary>
        public override void Move() {
            try {
                Result = _move();
            } catch (Exception e) {
                _logger.Error(e);
            }
            _prevMove = _watch.Now();
        }
    }
}