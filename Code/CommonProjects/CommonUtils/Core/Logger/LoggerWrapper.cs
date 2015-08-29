using System;
using System.IO;
using CommonUtils.Core.Config;
using NLog;
using NLog.Config;

namespace CommonUtils.Core.Logger {
    public class LoggerWrapper {
        private readonly NLog.Logger _logger;

        static LoggerWrapper() {
            if (ConfigHelper.LocalConfigPath != string.Empty) {
                LogManager.Configuration =
                    new XmlLoggingConfiguration(Path.Combine(ConfigHelper.LocalConfigPath, "NLog.config"));
            }
        }

        public bool IsDebugEnabled {
            get { return _logger.IsDebugEnabled; }
        }

        public bool IsInfoEnabled {
            get { return _logger.IsInfoEnabled; }
        }

        public LoggerWrapper(string logName) {
            _logger = LogManager.GetLogger(logName);

        }

        public void Info(string formatString, params object[] args) {
            _logger.Info(formatString, args);
        }

        public void Trace(string formatString, params object[] args) {
            _logger.Trace(formatString, args);
        }

        public void Fatal(string message) {
            _logger.Fatal(message);
        }

        public void Fatal(string formatString, params object[] args) {
            _logger.Fatal(formatString, args);
        }

        public void Fatal(Exception e) {
            _logger.Fatal(e);
        }

        public void Error(string formatString, params object[] args) {
            _logger.Error(formatString, args);
        }

        public void Error(Exception e) {
            _logger.Error(e);
        }

        public void Debug(string message) {
            _logger.Debug(message);
        }

        public void Debug(string formatString, params object[] args) {
            _logger.Debug(formatString, args);
        }
        
        public void Warn(string formatString, params object[] args) {
            _logger.Warn(formatString, args);
        }
    }
}