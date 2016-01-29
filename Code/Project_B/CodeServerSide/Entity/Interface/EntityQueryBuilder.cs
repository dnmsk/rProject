using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        
        public static List<T> FilterByGender<T, K>(this DbDataSource<T, K> ds, GenderType gender, params Enum[] fields) where T : class, IGenderTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return QueryHelper.FilterByGender(ds.Sort(GetEntityInstance<T>().KeyFields[0], SortDirection.Asc),
                GetEntityInstance<T>().GenderTypeField,
                gender,
                fields
            );
        }
        
        public static DbDataSource<T, K> FilterByLanguage<T, K>(this DbDataSource<T, K> ds, LanguageType languageType) where T : class, ILanguageTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.WhereEquals(GetEntityInstance<T>().LanguageTypeField, (short)languageType);
        }
        
        public static DbDataSource<T, K> FilterByBroker<T, K>(this DbDataSource<T, K> ds, BrokerType brokerType) where T : class, IBrokerTyped, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.WhereEquals(GetEntityInstance<T>().BrokerField, (short)brokerType);
        }
        public static DbDataSource<T, K> FilterByName<T, K>(this DbDataSource<T, K> ds, string name, bool containsAll) where T : class, INamedEntity, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            var searchPhrase = name
                .Split(new[] { ' ', '.', ',', ')', '(', '/', '-', '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Union(containsAll && name.Contains("/") ? new[] {"/"} : new string[0])
                .ToArray();
            return searchPhrase.Any() 
                ? (containsAll ? 
                    ds.Where(QueryHelper.GetFilterByWordIgnoreCaseAnd(searchPhrase, GetEntityInstance<T>().NameField)) : 
                    ds.Where(QueryHelper.GetFilterByWordIgnoreCaseOr(searchPhrase, GetEntityInstance<T>().NameField, false)))
                : ds.WhereNull(GetEntityInstance<T>().NameField);
        }
        public static DbDataSource<T, K> FilterByNameCompetition<T, K>(this DbDataSource<T, K> ds, string[] name) where T : class, INamedEntity, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.Where(QueryHelper.GetIndexedFilterByWordIgnoreCase(CompetitionHelper.ListStringToName(name), GetEntityInstance<T>().NameField));
        }
        public static DbDataSource<T, K> FilterByNameCompetitor<T, K>(this DbDataSource<T, K> ds, string[] name) where T : class, INamedEntity, IKeyedAbstractEntity<K>, new() where K : struct, IComparable<K> {
            return ds.Where(QueryHelper.GetFilterByWordIgnoreCaseOr(name, GetEntityInstance<T>().NameField, true));
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