using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using IDEV.Hydra.DAO;
using MainLogic;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public class LanguageTypeHelper : Singleton<LanguageTypeHelper> {
        private Dictionary<string, LanguageType> _isoToLanguageType = new Dictionary<string, LanguageType>();
        private List<LanguageType> _orderedLanguageTypes = new List<LanguageType>();

        private readonly MultipleKangooCache<LanguageType, Tuple<string, string>> _languageTypeToName;

        public static LanguageType DefaultLanguageTypeSetted => LanguageType.English;

        public LanguageTypeHelper() {
            _languageTypeToName = new MultipleKangooCache<LanguageType, Tuple<string, string>> (MainLogicProvider.WatchfulSloth,
                dictionary => {
                    var isoToLanguageType = new Dictionary<string, LanguageType>();
                    var orderedLanguageTypes = new List<LanguageType>();
                    foreach (var languageEntity in Language.DataSource
                                                           .Sort(Language.Fields.ID, SortDirection.Asc)                                       
                                                           .AsList()) {
                        var languageType = languageEntity.LanguageType;
                        var isoLower = languageEntity.IsoName.ToLowerInvariant();
                        dictionary[languageType] = new Tuple<string, string>(languageEntity.LocalName, isoLower);
                        isoToLanguageType[isoLower] = languageType;
                        orderedLanguageTypes.Add(languageType);
                    }
                    _isoToLanguageType = isoToLanguageType;
                    _orderedLanguageTypes = orderedLanguageTypes;
                });
        }

        public List<LanguageType> GetLanguages() {
            return _orderedLanguageTypes;
        }

        public string[] GetIsoNames() {
            return _isoToLanguageType.Keys.ToArray();
        }

        public LanguageType GetLanguageByIsoOrDefault(string isoName) {
            LanguageType lang;
            return _isoToLanguageType.TryGetValue((isoName ?? string.Empty).ToLower(), out lang) ? lang : DefaultLanguageTypeSetted;
        }

        public Tuple<string, string> GetLanguageName(LanguageType languageType) {
            Tuple<string, string> name;
            return _languageTypeToName.TryGetValue(languageType, out name) ? name : new Tuple<string, string>(languageType.ToString(), languageType.ToString());
        }
    }
}