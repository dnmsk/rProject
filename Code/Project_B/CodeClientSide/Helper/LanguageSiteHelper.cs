using System;
using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using DbEntity;
using MainLogic;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public class LanguageSiteHelper : Singleton<LanguageSiteHelper> {
        private readonly MultipleKangooCache<LanguageType, Dictionary<short, string>> _siteTextCache;

        public LanguageSiteHelper() {
            _siteTextCache = new MultipleKangooCache<LanguageType, Dictionary<short, string>>(MainLogicProvider.WatchfulSloth, 
                languagesCache => {
                    foreach (var languageType in LanguageTypeHelper.Instance.GetLanguages()) {
                        var listEntityForType = LanguageSiteText.DataSource.WhereEquals(LanguageSiteText.Fields.Languagetype, (short)languageType).AsList();
                        var dict = new Dictionary<short, string>();
                        foreach (var languageSiteText in listEntityForType) {
                            dict[(short)languageSiteText.Sitetext] = languageSiteText.Text;
                        }
                        languagesCache[languageType] = dict;
                    }
                });
        }

        public string GetText(LanguageType languageType, SiteText siteText) {
            Dictionary<short, string> langCaches;
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