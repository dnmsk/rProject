using System;
using System.Reflection;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;

namespace UnitTestProject.FactoryEntities {
    internal class Factory {
        private static readonly FactoryEntity _factory = new FactoryEntity();
        /// <summary>
        /// Добавление функции создания в список известных для генерации сущности. 
        /// </summary>
        public static void AddCreatorDao<T>(Func<T> func, Action<T> belongs = null, Action<T> afters = null)
            where T : class, IAbstractEntity {
            _factory.AddCreatorDao(func, belongs, afters);
        }

        /// <summary>
        /// Создание и сохранение сущности с необходимыми зависимостями.
        /// </summary>
        public static T CreateDao<T>(Action<T> option) where T : class, IAbstractEntity {
            var entity = _factory.BuildDao(option);
            var b = entity.IsNew
                ? entity.Save()
                : entity.Insert();
            entity = _factory.PostProcess(entity);
            return entity;
        }

        /// <summary>
        /// Создание и сохранение сущности с необходимыми зависимостями.
        /// </summary>
        /// <typeparam name="T">Тип сущности.</typeparam>
        /// <returns>Возвращает сущность после сохранения.</returns>
        public static T CreateDao<T>() where T : class, IAbstractEntity {
            return CreateDao<T>(null);
        }

        /// <summary>
        /// Инициализация фабрики для создания сущностей.
        /// </summary>
        public static void Init(Assembly entityAssembly) {
            entityAssembly.GetTypes()
                .Each(type => type.If(t => typeof(ICreator).IsAssignableFrom(t) && !t.IsInterface)
                                  .With(t => Activator.CreateInstance(type) as ICreator)
                                  .Do(c => c.Bind()));
        }
    }
}
