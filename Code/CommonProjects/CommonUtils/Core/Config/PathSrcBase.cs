using System;
using System.Configuration;
using System.IO;

namespace CommonUtils.Core.Config {
    public abstract class PathSrcBase {
        protected volatile bool _testMode;
        protected string _localConfigPath;

        public abstract string GetLocalPathValue { get; }

        /// <summary>
        /// Путь к локальным конфигам (с "\" в конце)
        /// </summary>
        public string LocalConfigPath {
            get {
                if (_localConfigPath == null || _testMode) {
                    if (_testMode) {
                        string path = GetLocalPathValue;
                        _localConfigPath = string.IsNullOrEmpty(path)
                            ? Path.Combine(Environment.CurrentDirectory, "Configs\\")
                            : NormalizePath(Path.Combine(path, "Test"));
                    } else {
                        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
                        if (Directory.Exists(".\\Configs\\")) {
                            _localConfigPath = ".\\Configs\\"; //работает для sandbox, но не работает для сервиса
                        } else if (Directory.Exists(path)) {
                            _localConfigPath = path; //а вот это для сервиса
                        } else {
                            _localConfigPath = GetLocalPathValue;
                        }
                    }/*
                    if (_localConfigPath.StartsWith(".")) {
                        _localConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _localConfigPath.Trim('.', '/', '\\'));
                    }*/
                }
                return _localConfigPath;
            }
        }
        
        public bool TestMode {
            get { return _testMode; }
            set {
                _testMode = value;
                _localConfigPath = null;
            }
        }

        protected static string NormalizePath(string configPath) {
            if (!configPath.EndsWith("\\")) {
                configPath += "\\";
            }
            return configPath;
        }
    }
}