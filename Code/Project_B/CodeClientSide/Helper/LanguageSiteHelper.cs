using System;
using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public class LanguageSiteHelper : Singleton<LanguageSiteHelper> {
        private readonly MultipleKangooCache<LanguageType, Dictionary<short, string>> _siteTextCache;

        public LanguageSiteHelper() {
            _siteTextCache = new MultipleKangooCache<LanguageType, Dictionary<short, string>>(MainLogicProvider.WatchfulSloth, 
                languagesCache => {
                    foreach (var languageType in LanguageTypeHelper.Instance.GetLanguages()) {
                        var listEntityForType = SiteText.DataSource.WhereEquals(SiteText.Fields.Languagetype, (short)languageType).AsList();
                        var dict = new Dictionary<short, string>();
                        foreach (var languageSiteText in listEntityForType) {
                            dict[(short)languageSiteText.Sitetext] = languageSiteText.Text;
                        }
                        languagesCache[languageType] = dict;
                    }
                });
        }

        public string GetText(LanguageType languageType, SiteTextType siteTextType) {
            Dictionary<short, string> langCaches;
            string text;
            return (_siteTextCache.TryGetValue(languageType, out langCaches) ||
                    _siteTextCache.TryGetValue(LanguageTypeHelper.DefaultLanguageTypeSetted, out langCaches)) 
                   && langCaches.TryGetValue((short) siteTextType, out text)
                ? text
                : string.Format("{{{0}}}", siteTextType);
        }

        public void SetText(LanguageType languageType, SiteTextType siteTextType, string text) {
            var textEntity = SiteText.DataSource
                                             .WhereEquals(SiteText.Fields.Languagetype, (short) languageType)
                                             .WhereEquals(SiteText.Fields.Sitetext, (short) siteTextType)
                                             .First() 
                            ?? new SiteText {
                                Datecreatedutc = DateTime.UtcNow,
                                Languagetype = languageType,
                                Sitetext = siteTextType
                            };
            textEntity.Text = text;
            textEntity.Save();
        }
    }
}