using CommonUtils.WatchfulSloths.SlothMoveRules;

namespace CommonUtils.WatchfulSloths {
    /// <summary>
    /// Интерфейс обеспечения доступа к данным, актуальность которых не кретична, однако кретично время доступа.
    /// </summary>
    public interface IWatchfulSloth {
        /// <summary>
        /// Установка правила, по которому будет производится контроль данных и их актуальности.
        /// </summary>
        /// <param name="moveRule">Правило.</param>
        /// <returns>Возвращает true, если правило успешно установлено, иначе false.</returns>
        bool SetMove(ISlothMoveRule moveRule);

        /// <summary>
        /// Возвращает данные, за которыми следит ленивец.
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="keyMove">Ключ, по которому храняться данные. Обычно это typeof(правило).</param>
        /// <param name="defValue">Значение по умолчанию.</param>
        /// <returns>Возвращает актуальные данные в случае успеха, либо значени по умолчанию.</returns>
//        T Get<T>(Type keyMove, T defValue);
    }
}