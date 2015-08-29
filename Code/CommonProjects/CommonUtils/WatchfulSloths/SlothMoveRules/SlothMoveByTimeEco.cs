using System;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    /// <summary>
    /// Правило движений ленивца по времени с сохраненным результатом.
    /// Обновление данных приостанавливается, если нет запросов к результату.
    /// </summary>
    public class SlothMoveByTimeEco<T> : SlothMoveByTime<T> {
        private DateTime? _hitTime;

        public SlothMoveByTimeEco(Func<T> move, TimeSpan moveInterval, T startValue, IWatch watch = null) 
            : base(move, moveInterval, startValue, watch) {
        }

        /// <summary>
        /// Нужно двигаться. Не будем двигаться, если данные никто не спрашивал, даже если время пришло.
        /// </summary>
        public override bool IsNeedMove {
            get {
                return !_prevMove.HasValue
                    || (_watch.Now() - _prevMove.Value > _moveInterval
                        && (_hitTime.HasValue && _watch.Now() - _hitTime < _moveInterval));
            }
        }

        /// <summary>
        /// Результат движения, так же ставим хиттайм, чтобы сигнализировать о том, что данные кому-то нужны.
        /// </summary>
        public override T Result {
            get {
                _hitTime = _watch.Now();
                return base.Result;
            }
        }
    }
}