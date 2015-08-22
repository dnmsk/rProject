using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using CommonUtils.Core.Logger;

namespace CommonUtils.Code {
    public class BrowserInfo {
        /// <summary>
        /// Logger для текущего класса
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(BrowserInfo).FullName);

        public bool Mobile { get; private set; }
        public string UserAgent { get; private set; } = string.Empty;

        public string Os { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        private readonly int _majorVersion;
        private readonly double _minorVersion;

        private static readonly Regex _operaVersionRegex = new Regex(@"(?<=.{5,10}\s*(Version|Opera|OPR)/)(\d*)\.(\d*)", RegexOptions.IgnoreCase |
                                                                                                                         RegexOptions.Compiled |
                                                                                                                         RegexOptions.Singleline |
                                                                                                                         RegexOptions.Multiline |
                                                                                                                         RegexOptions.CultureInvariant);
        private static readonly Regex _ie11 = new Regex(@"Trident/7\.0.*?rv:\s?11\.(\d*)", RegexOptions.IgnoreCase |
                                                                                           RegexOptions.Compiled |
                                                                                           RegexOptions.Singleline |
                                                                                           RegexOptions.Multiline |
                                                                                           RegexOptions.CultureInvariant);

        public decimal CurrentVersion() {
            return _majorVersion + (decimal)_minorVersion;
        }

        public BrowserInfo(HttpBrowserCapabilitiesBase browser, string userAgent) {
            try {
                UserAgent = userAgent ?? string.Empty;
                Name = browser.Browser;
                Os = browser.Platform;
                _majorVersion = browser.MajorVersion;
                _minorVersion = browser.MinorVersion;
                float currentVervion = 0;
                // Набор костылей для корректного определения параметров
                // Этот для юзерагента дивного IE11
                var isIe = _ie11.Match(UserAgent);
                if (isIe.Groups.Count > 1) {
                    Name = "IE";
                    _majorVersion = 11;
                    _minorVersion = Convert.ToDouble(isIe.Groups[1].ToString());
                }
                // Иногда дотнет путает мобильные и десктопные браузеры
                if (browser.IsMobileDevice || UserAgent.ToLower().Contains("mobile") || UserAgent.ToLower().Contains("android")) {
                    Mobile = true;
                }
                // У оперы тоже свой особенный юзерагент
                if (UserAgent.IndexOf("OPR") > -1) {
                    Name = "Opera";
                }
                if (Name == "Opera") {
                    var match = _operaVersionRegex.Match(UserAgent);
                    float.TryParse(match.Value, NumberStyles.Float, new CultureInfo("en-US"), out currentVervion);
                }
                if (currentVervion != 0) {
                    _majorVersion = (int)currentVervion;
                    _minorVersion = currentVervion - (int)currentVervion;
                }
            } catch (Exception ex) {
                _logger.Error(ex);
            }
        }
    }
}
