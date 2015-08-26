using System;
using System.Configuration;
using System.IO;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Core.Config {
    public class ApplicationConfigSrc : PathSrcBase {
        private readonly string _path;
        const string _config = "Config";
        public ApplicationConfigSrc() {
            _path = ConfigurationManager.AppSettings["ConfigSource"];
            if (_path.IsNullOrWhiteSpace()) {
                _path = AppDomain.CurrentDomain.BaseDirectory;
                var combinedPath = Path.Combine(_path, _config);
                if (Directory.Exists(combinedPath)) {
                    _path = combinedPath;
                    return;
                }
                combinedPath = Path.Combine(_path, "bin", _config);
                if (Directory.Exists(combinedPath)) {
                    _path = combinedPath;
                    return;
                }
                throw new ArgumentException("Не найдена папка с конфигурационными файлами: " + _path);
            }
        }

        public override string GetLocalPathValue {
            get { return _path; }
        }
    }
}