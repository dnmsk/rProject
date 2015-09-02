using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using AutoPublication.Models;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic.WebFiles;

namespace AutoPublication.Code {
    public class BuildPublishProvider : Singleton<BuildPublishProvider>, IDisposable {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BuildPublishProvider).FullName);

        private static readonly JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
        private readonly List<BuildPublishItem> _buildPublishItems;
        private const string _configurationProperty = "PublishItemSerialized";
        private const string _publishDesciptionFile = "publishinfo.txt";

        public BuildPublishProvider() : this(SiteConfiguration.GetConfigurationProperty(_configurationProperty)) {
        }

        public BuildPublishProvider(string publishItemSerialized) {
            _buildPublishItems = new JavaScriptSerializer().Deserialize<List<BuildPublishItem>>(publishItemSerialized) ?? new List<BuildPublishItem>();
            SlothMovePlodding.AddAction(() => {
                ReadFileDescriptions();
            });
        }

        private void ReadFileDescriptions() {
            foreach (var buildPublishItem in _buildPublishItems) {
                var path = Path.Combine(buildPublishItem.ProjectPath, _publishDesciptionFile);
                if (!File.Exists(path)) {
                    WriteToFileCMD(buildPublishItem, path);
                    continue;
                }
                try {
                    using (var reader = new StreamReader(path)) {
                        var fileContent = reader.ReadToEnd(); // CmdCommandExecutor.Run("cmd.exe", "/c type " + path);
                        var readedBuildPublishItem = _jsSerializer.Deserialize<BuildPublishItem>(fileContent);
                        buildPublishItem.ProjectPath = readedBuildPublishItem.ProjectPath;
                        buildPublishItem.ProjectName = readedBuildPublishItem.ProjectName;
                        buildPublishItem.BuildName = readedBuildPublishItem.BuildName;
                        buildPublishItem.ProjectPublishDate = readedBuildPublishItem.ProjectPublishDate;
                    }
                } catch (Exception ex) {
                        _logger.Error(ex);  
                }
            }
        }

        public void AddPublishPath(BuildPublishItem buildPublishItem) {
            _buildPublishItems.Add(buildPublishItem);
            UpdateConfigFile();
        }

        public List<BuildPublishItem> GetPublishItems() {
            return _buildPublishItems;
        }

        public void PublishBuild(BuildPublishItem buildPublishItem, string pathToBuild) {
            try {
                buildPublishItem.BuildName = PublicatorProvider.Publish(pathToBuild, buildPublishItem.ProjectName, buildPublishItem.ProjectPath);
                var path = Path.Combine(buildPublishItem.ProjectPath, _publishDesciptionFile);
                buildPublishItem.ProjectPublishDate = DateTime.Now;
                for(var i = 0; i < _buildPublishItems.Count; i++) {
                    var item = _buildPublishItems[i];
                    if(item.ProjectPath == buildPublishItem.ProjectPath &&
                        item.ProjectName == buildPublishItem.ProjectName) {
                        _buildPublishItems.RemoveAt(i);
                    }
                }
                _buildPublishItems.Add(buildPublishItem);
                if (File.Exists(path)) {
                    CmdCommandExecutor.Run("cmd.exe", string.Format("/c del \"{0}\"", path));
                }
                WriteToFileCMD(buildPublishItem, path);
            } catch (Exception ex) {
                _logger.Error(ex);
            }
        }

        private static void WriteToFileCMD(BuildPublishItem buildPublishItem, string path) {
            CmdCommandExecutor.Run("cmd.exe", string.Format("/c echo {0} >> \"{1}\"", _jsSerializer.Serialize(buildPublishItem), path));
        }

        public void Dispose() {
            UpdateConfigFile();
        }

        private void UpdateConfigFile() {
            var serializedItems = _jsSerializer.Serialize(_buildPublishItems);
            SiteConfiguration.ModifyConfigurationProperty(_configurationProperty, serializedItems);
        }

        public void RemovePublishPath(BuildPublishItem buildPublishItem) {
            for (var i = 0; i < _buildPublishItems.Count; i++) {
                var item = _buildPublishItems[i];
                if (item.ProjectPath == buildPublishItem.ProjectPath && item.ProjectName == buildPublishItem.ProjectName) {
                    _buildPublishItems.RemoveAt(i);
                }
            }
            UpdateConfigFile();
        }
    }
}