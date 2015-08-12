namespace CommonUtils.Code {
    /// <summary>
    ///     Класс-singleton
    /// </summary>
    /// <typeparam name="T">Тип, который лежит в Singleton</typeparam>
    public class Singleton<T> where T : class, new() {
        private static T _instance;
        private static readonly object _lock = new object();

        protected Singleton() {
        }

        /// <summary>
        ///     Экземпляр объекта
        /// </summary>
        public static T Instance {
            get {
                var instance = _instance;
                if (instance == null) {
                    lock (_lock) {
                        instance = _instance ?? (_instance = new T());
                    }
                }
                return instance;
            }
        }

        public void Reset() {
            lock (_lock) {
                _instance = null;
            }
        }
    }
}