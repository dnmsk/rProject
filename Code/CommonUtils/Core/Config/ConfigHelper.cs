namespace CommonUtils.Core.Config {
    public class ConfigHelper {
        private static readonly PathSrcBase _pathSrc;

        static ConfigHelper() {
            _pathSrc = new ApplicationConfigSrc();
            if (string.IsNullOrEmpty(_pathSrc.GetLocalPathValue)) {
                _pathSrc = new EnvironmentVariableSrc();
            }
        }

        /// <summary>
        /// Путь к локальным конфигам (с "\" в конце)
        /// </summary>
        public static string LocalConfigPath {
            get {
                return _pathSrc.LocalConfigPath;
            }
        }

        /// <summary>
        /// Путь к глобальным конфигам (с "\" в конце)
        /// </summary>
        public static string GlobalConfigPath {
            get {
                return _pathSrc.GlobalConfigPath;
            }
        }

        public static bool TestMode {
            get { return _pathSrc.TestMode; }
            set { _pathSrc.TestMode = value; }
        }
    }
}