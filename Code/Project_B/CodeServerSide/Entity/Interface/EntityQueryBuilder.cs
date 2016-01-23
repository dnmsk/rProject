using System;
using System.Collections.Concurrent;
using System.Linq;
using IDEV.Hydra.DAO;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Interface {
    public static class EntityQueryBuilder {
        private static readonly ConcurrentDictionary<Type, object> _mapInstances = new ConcurrentDictionary<Type, object>();
        
        public static DbDataSource<T, K> FilterBySportType<T, K>(this DbDataSource<T, K> ds, SportType sportType) where T : class, ISportTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.WhereEquals(GetEntityInstance<T>().SportTypeField, (short)sportType);
        }
        
        public static DbDataSource<T, K> FilterByGender<T, K>(this DbDataSource<T, K> ds, GenderType gender) where T : class, IGenderTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.WhereEquals(GetEntityInstance<T>().GenderTypeField, (short)gender);
        }
        
        public static DbDataSource<T, K> FilterByLanguage<T, K>(this DbDataSource<T, K> ds, LanguageType languageType) where T : class, ILanguageTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.WhereEquals(GetEntityInstance<T>().LanguageTypeField, (short)languageType);
        }
        
        public static DbDataSource<T, K> FilterByBroker<T, K>(this DbDataSource<T, K> ds, BrokerType brokerType) where T : class, IBrokerTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.WhereEquals(GetEntityInstance<T>().BrokerField, (short)brokerType);
        }
        public static DbDataSource<T, K> FilterByName<T, K>(this DbDataSource<T, K> ds, string name, bool all) where T : class, INamedEntity, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            var searchPhrase = name
                .Split(new[] { ' ', '.', ',', ')', '(', '/', '-', '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => s.Length > 1)
                .ToArray();
            return searchPhrase.Any() 
                ? (all ? 
                    ds.Where(QueryHelper.GetFilterByWordIgnoreCaseAnd(searchPhrase, GetEntityInstance<T>().NameField)) : 
                    ds.Where(QueryHelper.GetFilterByWordIgnoreCaseOr(searchPhrase, GetEntityInstance<T>().NameField)))
                : ds.WhereNull(GetEntityInstance<T>().NameField);
        }
        /*
        public static DbDataSource<T, int> GetDsForRawType<T>(BrokerEntityType brokerEntity) where T : AbstractEntityTemplateKey<T, int> {
            switch (brokerEntity) {
                case BrokerEntityType.Competition:
                    return RawCompetition.DataSource;
            }
            return null;
        } 
        */
        private static T GetEntityInstance<T>() where T : new() {
            var type = typeof (T);
            object instance;
            if (!_mapInstances.TryGetValue(type, out instance)) {
                instance = new T();
                _mapInstances[type] = instance;
            }
            return (T) instance;
        }
    }
}