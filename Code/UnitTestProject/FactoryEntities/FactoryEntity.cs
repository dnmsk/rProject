using System;
using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;

namespace UnitTestProject.FactoryEntities {
    internal class FactoryEntity {
        private readonly Dictionary<Type, Func<object>> _mapCreators = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<Type, object> _mapBelongs = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _mapAfter = new Dictionary<Type, object>();
        
        public T BuildDao<T>(Action<T> option = null) where T : class, IAbstractEntity {
            Func<object> creator;
            if (!_mapCreators.TryGetValue(typeof(T), out creator)) {
                throw new ApplicationException(
                    string.Format("Для переданного типа {0} нет фабрики.", typeof(T).Name));
            }

            T entity = creator() as T;
            if (entity == null) {
                throw new ApplicationException(
                    string.Format("Не удалось создать сущность заданного типа {0}.", typeof(T).Name));
            }

            option.Do(f => f(entity));
            object belongs;
            if (_mapBelongs.TryGetValue(typeof(T), out belongs)) {
                ((Action<T>)belongs)(entity);
            }

            return entity;
        }
        
        public void AddCreatorDao<T>(Func<T> func, Action<T> belongs = null, Action<T> afters = null)
            where T : class, IAbstractEntity {
            _mapCreators.Add(typeof(T), func);
            if (belongs != null) {
                _mapBelongs.Add(typeof(T), belongs);
            }
            if (afters != null) {
                _mapAfter.Add(typeof(T), afters);
            }
        }

        public T PostProcess<T>(T entity) where T : class, IAbstractEntity {
            object afters;
            if (_mapAfter.TryGetValue(typeof(T), out afters)) {
                ((Action<T>)afters)(entity);
            }

            return entity;
        }
    }
}
