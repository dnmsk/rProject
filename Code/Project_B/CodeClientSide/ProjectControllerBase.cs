using MainLogic.WebFiles;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide {
    public abstract class ProjectControllerBase : ApplicationControllerBase {
        private LanguageType _currentLanguage = LanguageType.Default;
        protected LanguageType CurrentLanguage {
            get {
                if (_currentLanguage != LanguageType.Default) {
                    return _currentLanguage;
                }
                switch (((RouteData.Values["language"] as string) ?? string.Empty).ToLower()) {
                    case "ru":
                        _currentLanguage = LanguageType.Russian;
                        break;
                    case "en":
                    default:
                        _currentLanguage = LanguageType.English;
                        break;
                }
                return _currentLanguage;
            }
        }
    }
}