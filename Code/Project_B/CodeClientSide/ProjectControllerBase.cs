using System;
using MainLogic.WebFiles;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide {
    public abstract class ProjectControllerBase : ApplicationControllerBase {
        private LanguageType _currentLanguage = LanguageType.Default;
        public LanguageType CurrentLanguage {
            get {
                if (_currentLanguage != LanguageType.Default) {
                    return _currentLanguage;
                }
                return _currentLanguage = LanguageTypeHelper.Instance.GetLanguageByIsoOrDefault(RouteData.Values["language"] as string);
            }
        }

        public Tuple<string, string> GetPreviousControllerAction() {
            var referrer = Request.UrlReferrer;
            if (referrer != null && SiteConfiguration.ProductionHostName.IndexOf(referrer.Host, StringComparison.InvariantCultureIgnoreCase) >= 0 && referrer.Segments.Length > 1) {
                var segments = referrer.Segments;
                return new Tuple<string, string>(
                    segments.Length == 2 ? "Home" : segments[2].Trim('/'), 
                    segments.Length <= 3 ? "Index" : segments[3].Trim('/'));
            }
            return null;
        }

        public virtual SubNavigationType SubNavigationType {
            get { return SubNavigationType.None; }
        }
    }
}