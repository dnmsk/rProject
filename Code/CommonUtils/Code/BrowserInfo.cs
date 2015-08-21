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
        public string UserAgent {
            get { return _userAgent; }
            private set { _userAgent = value; }
        }
        public string Os {
            get { return _os; }
            private set { _os = value; }
        }
        public string Name {
            get { return _name; }
            private set { _name = value; }
        }

        private readonly int _majorVersion;
        private readonly double _minorVersion;
        private string _os = string.Empty;
        private string _userAgent = string.Empty;
        private string _name = string.Empty;

        // Накостыленная регулярка для определения версии оперы.   .{5,10} убирать нельзя иначе некоторые оперы так и не узнают свою версию /Курсиков/
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
                _majorVersion = browser.MajorVersion;
                _minorVersion = browser.MinorVersion;
                Os = browser.Platform;
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
                Match match;
                if (UserAgent.IndexOf("OPR") > -1) {
                    Name = "Opera";
                }
                if (Name == "Opera") {
                    match = _operaVersionRegex.Match(UserAgent);
                    Single.TryParse(match.Value, NumberStyles.Float, new CultureInfo("en-US"), out currentVervion);
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
