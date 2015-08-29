using System;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    /// <summary>
    /// Правило движений ленивца с сохраненным результатом.
    /// </summary>
    public abstract class SlothMoveRule<T> : ISlothMoveRule {
        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SlothMoveRule");

        protected SlothMoveRule() {}

        protected SlothMoveRule(T startValue) {
            _result = startValue;
        }

        /// <summary>
        /// Ключ, по которому будет сохранено правило леницем.
        /// </summary>
        public Type Key {
            get { return GetType(); }
        }

        private T _result;
        /// <summary>
        /// Результат движения.
        /// </summary>
        public virtual T Result {
            get { return _result; }
            protected set { _result = value; }
        }

        /// <summary>
        /// Нужно двигаться.
        /// </summary>
        public abstract bool IsNeedMove { get; }
        
        /// <summary>
        /// Движение.
        /// </summary>
        public abstract void Move();
    }
}