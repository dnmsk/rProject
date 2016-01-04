using System;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    /// <summary>
    /// Правило движений ленивца до первого успешного результата.
    /// </summary>
    public class SlothMoveByFirstSuccess<T> : SlothMoveRule<T> {
        private readonly Func<T> _move;
        private bool _needSuccess = true;
        public SlothMoveByFirstSuccess(Func<T> move, T startValue) : base(startValue) {
            _move = move;
        }

        /// <summary>
        /// Нужно двигаться.
        /// </summary>
        public override bool IsNeedMove {
            get { return _needSuccess; }
        }

        /// <summary>
        /// Движение.
        /// </summary>
        public override void Move(int wakeUpInterval) {
            try {
                Result = _move();
                _needSuccess = false;
            } catch (Exception e) {
                _logger.Error(e);
            }
        }
    }
}