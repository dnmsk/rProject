using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Core.Config {
    public static class ConfigHelper {
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
        
        private static Dictionary<string, Tuple<FileMonitor, List<Action<object>>, ObjectWrapper<string>, bool>> _fileMonitors = 
            new Dictionary<string, Tuple<FileMonitor, List<Action<object>>, ObjectWrapper<string>, bool>>();

        public static void RegisterConfigTarget(string configFileName, Action<object> act, bool isXml = true) {
            Tuple<FileMonitor, List<Action<object>>, ObjectWrapper<string>, bool> targetMonitor;
            if (!_fileMonitors.TryGetValue(configFileName.ToLower(), out targetMonitor)) {
                var fileMonitor = new FileMonitor(LocalConfigPath, configFileName);
                var actions = new List<Action<object>> {act};
                targetMonitor = new Tuple<FileMonitor, List<Action<object>>, ObjectWrapper<string>, bool>(fileMonitor,
                    actions, new ObjectWrapper<string>(string.Empty), isXml);
                Action actionOnChange = () => {
                    var tries = 2;
                    do {
                        try {
                            var fileName = Path.Combine(LocalConfigPath, configFileName);
                            if (!File.Exists(fileName)) {
                                return;
                            }
                            var content = File.ReadAllText(fileName);
                            targetMonitor.Item3.Obj = content;

                            object actionArg = content;
                            if (targetMonitor.Item4) {
                                var configXml = new XmlDocument();
                                configXml.LoadXml(content);
                                actionArg = configXml;
                            }
                            foreach (var action in actions) {
                                action(actionArg);
                            }
                            break;
                        }
                        catch (Exception ex) {
                            System.Diagnostics.Debug.Write(ex.ToString());
                            Thread.Sleep(50);
                        }
                    } while (tries-- > 0);
                };
                fileMonitor.Changed += () => { actionOnChange(); };
                actionOnChange();
                fileMonitor.Start();
            }
            else {
                object actionArg = targetMonitor.Item3.Obj;
                if (targetMonitor.Item4) {
                    var configXml = new XmlDocument();
                    configXml.LoadXml(targetMonitor.Item3.Obj);
                    actionArg = configXml;
                }
                act(actionArg);
                targetMonitor.Item2.Add(act);
            }
        }

        public static bool TestMode {
            get { return _pathSrc.TestMode; }
            set { _pathSrc.TestMode = value; }
        }
    }
}