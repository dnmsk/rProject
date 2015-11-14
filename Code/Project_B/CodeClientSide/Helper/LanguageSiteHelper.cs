using System;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using DbEntity;
using MainLogic;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public class LanguageSiteHelper : Singleton<LanguageSiteHelper> {
        private readonly MultipleKangooCache<LanguageType, MultipleKangooCache<short, string>> _siteTextCache;

        public LanguageSiteHelper() {
            _siteTextCache = new MultipleKangooCache<LanguageType, MultipleKangooCache<short, string>>(MainLogicProvider.WatchfulSloth, 
                languagesCache => {
                    foreach (var languageType in LanguageTypeHelper.Instance.GetLanguages()) {
                        languagesCache[languageType] = new MultipleKangooCache<short, string>(null, languageCache => {
                            foreach (var textEntity in LanguageSiteText.DataSource.WhereEquals(LanguageSiteText.Fields.Languagetype, (short)languageType).AsList()) {
                                languageCache[(short) textEntity.Sitetext] = textEntity.Text;
                            }
                        });
                    }
                });
        }

        public string GetText(LanguageType languageType, SiteText siteText) {
            MultipleKangooCache<short, string> langCaches;
            string text;
            return (_siteTextCache.TryGetValue(languageType, out langCaches) ||
                    _siteTextCache.TryGetValue(LanguageTypeHelper.DefaultLanguageTypeSetted, out langCaches)) 
                   && langCaches.TryGetValue((short) siteText, out text)
                ? text
                : string.Format("{{{0}}}", siteText);
        }

        public void SetText(LanguageType languageType, SiteText siteText, string text) {
            var textEntity = LanguageSiteText.DataSource
                                             .WhereEquals(LanguageSiteText.Fields.Languagetype, (short) languageType)
                                             .WhereEquals(LanguageSiteText.Fields.Sitetext, (short) siteText)
                                             .First() 
                            ?? new LanguageSiteText {
                                Datecreatedutc = DateTime.UtcNow,
                                Languagetype = languageType,
                                Sitetext = siteText
                            };
            textEntity.Text = text;
            textEntity.Save();
        }
    }
}