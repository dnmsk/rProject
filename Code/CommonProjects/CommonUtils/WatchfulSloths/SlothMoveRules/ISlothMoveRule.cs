using System;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    /// <summary>
    /// Интерфейс правила движений ленивца.
    /// </summary>
    public interface ISlothMoveRule {
        /// <summary>
        /// Ключ, по которому будет сохранено правило леницем.
        /// </summary>
        Type Key { get; }

        /// <summary>
        /// Нужно двигаться.
        /// </summary>
        bool IsNeedMove { get; }

        /// <summary>
        /// Движение.
        /// </summary>
        void Move();
    }
}