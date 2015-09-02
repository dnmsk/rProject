using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic.WebFiles;

namespace AutoPublication.Code {
    public class TeamcityProvider : Singleton<TeamcityProvider> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (TeamcityProvider).FullName);

        private readonly string _teamcityTarget;
        private readonly WebRequestHelper _requestHelper;
        private readonly Regex _ipExtractorRegex = new Regex("ip_(?<ip>\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})", RegexOptions.Compiled);

        private readonly string _pathToShare;
        private List<ZipBuildItem> _buildItems = new List<ZipBuildItem>();

        public TeamcityProvider(string teamcityTarget, string pathToShare) {
            _pathToShare = pathToShare;
            _teamcityTarget = teamcityTarget;
            _requestHelper = new WebRequestHelper();

            WatchfulSloth.Instance.SetMove(new SlothMoveByTimeSingle<object>(() => {
                UpdateBuildCache();
                return null;
            }, new TimeSpan(0, 30, 0), null));
        }

        public TeamcityProvider() : this(SiteConfiguration.GetConfigurationProperty("TeamcityTarget"), SiteConfiguration.GetConfigurationProperty("PathToShare")) { }

        private void Authenticate() {
            _requestHelper.GetContent(_teamcityTarget + "/guestLogin.html?guest=1");
        }

        private string[] GetBuildServers() {
            Authenticate();
            var buildAgentsPageContenet = _requestHelper.GetContent(_teamcityTarget + "/agents.html");
            var result = new List<string>();
            foreach (Match match in _ipExtractorRegex.Matches(buildAgentsPageContenet.Item2)) {
                result.Add(match.Groups["ip"].Value);
            }
            return result.Distinct().ToArray();
        }

        public void UpdateBuildCache() {
            var result = new List<ZipBuildItem>();
            foreach(var server in GetBuildServers()) {
                try {
                    var files = CmdCommandExecutor.Run("cmd.exe", string.Format(@"/c dir /b /s \\{0}\{1}\*.7z", server, _pathToShare))
                        .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    result.AddRange(files.Select(f => new ZipBuildItem(f)));
                }
                catch(Exception ex) {
                    _logger.Error(ex);
                }
            }
            _buildItems = result;
        }

        public List<ZipBuildItem> GetBuilds() {
            return _buildItems;
        }
    }
}