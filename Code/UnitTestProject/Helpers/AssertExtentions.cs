using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils.ExtendedTypes;
using NUnit.Framework;

namespace UnitTestProject.Helpers {
    public static class AssertExtentions {
        /// <summary>
        /// Верификация коллекции данных, на основе ожидаемой коллекции.
        /// Все элементы будут сравнены друг с другом с помощью <see cref="comparers"/>.
        /// </summary>
        /// <typeparam name="T">Тип сравниваемых объектов.</typeparam>
        /// <param name="actualItems">Коллекция, которую проверям.</param>
        /// <param name="expectedItems">Коллекция с ожидаемыми значениями.</param>
        /// <param name="comparers">Методы проверки.</param>
        public static void Verify<T>(this ICollection<T> actualItems, ICollection<T> expectedItems,
                                     params Func<T, Object>[] comparers) {
            if (expectedItems == null) {
                Assert.IsNull(actualItems);
                return;
            }

            Assert.AreEqual(expectedItems.Count, actualItems.Count);
            var actualList = actualItems.ToList();
            Func<T, T, bool> comparer = (e, a) =>
                comparers.All(p => {
                    var resExpected = p(e);
                    var resActual = p(a);
                    if (resExpected == null) {
                        return resActual == null;
                    }

                    return resExpected.Equals(resActual);
                });

            expectedItems.Each((i, e) =>
                Assert.IsTrue(comparer(e, actualList[i]), "Ошибка при сравнении {0}-ого элемента.".Format(i)));
        }

        /// <summary>
        /// Верификация коллекции данных, на основе ожидаемой коллекции и перечисленных имен свойств, 
        /// которые будем проверять.
        /// </summary>
        /// <typeparam name="T">Тип сравниваемых объектов.</typeparam>
        /// <param name="actualItems">Коллекция, которую проверям.</param>
        /// <param name="expectedItems">Коллекция с ожидаемыми значениями.</param>
        /// <param name="needProps">Имена свойст, значения в которых будут проверяться.</param>
        public static void Verify<T>(this ICollection<T> actualItems, ICollection<T> expectedItems,
                                     params string[] needProps) {
            if (expectedItems == null) {
                Assert.IsNull(actualItems);
                return;
            }

            Assert.AreEqual(expectedItems.Count, actualItems.Count);
            var actualList = actualItems.ToList();
            Type t = typeof(T);
            var props = t.GetProperties().Where(e => needProps.Contains(e.Name));
            Func<T, T, bool> comparer = (e, a) =>
                props.All(p => p.GetValue(e, null).Equals(p.GetValue(a, null)));

            expectedItems.Each((i, e) =>
                Assert.IsTrue(comparer(e, actualList[i]), "Ошибка при сравнении {0}-ого элемента.".Format(i)));
        }

        /// <summary>
        /// Верификация коллекции данных, на основе ожидаемой коллекции, когда элементы.
        /// Все элементы будут сравнены друг с другом с помощью <see cref="comparer"/>.
        /// </summary>
        /// <typeparam name="TExpected">Тип ожидаемых элементов.</typeparam>
        /// <typeparam name="TActual">Тип проверяемых элементов.</typeparam>
        /// <param name="expectedItems">Список с ожидаемыми элементами.</param>
        /// <param name="actualItems">Список с проверяемыми элементами.</param>
        /// <param name="comparer">Функция сравнения элементов между собой.</param>
        public static void Verify<TExpected, TActual>(this ICollection<TExpected> expectedItems,
                                                      ICollection<TActual> actualItems,
                                                      Func<TExpected, TActual, bool> comparer) {
            if (expectedItems == null) {
                Assert.IsNull(actualItems);
                return;
            }

            Assert.AreEqual(expectedItems.Count, actualItems.Count);
            var actualList = actualItems.ToList();
            expectedItems.Each((i, e) =>
                Assert.IsTrue(comparer(e, actualList[i]), "Ошибка при сравнении {0}-ого элемента.".Format(i)));
        }

        /// <summary>
        /// Верификация разных типов.
        /// Элементы будут сравнены друг с другом с помощью <see cref="comparer"/>.
        /// </summary>
        /// <typeparam name="TExpected">Тип ожидаемого элемента.</typeparam>
        /// <typeparam name="TActual">Тип проверяемого элемента.</typeparam>
        /// <param name="expectedItem">Ожидаемый элемент.</param>
        /// <param name="actualItem">Проверяемый элемент.</param>
        /// <param name="comparer">Функция сравнения элементов между собой.</param>
        public static void Verify<TExpected, TActual>(this TExpected expectedItem,
                                                      TActual actualItem,
                                                      Func<TExpected, TActual, bool> comparer) where TExpected : class {
            if (expectedItem == null) {
                Assert.IsNull(actualItem);
                return;
            }
            Assert.IsTrue(comparer(expectedItem, actualItem), "Ошибка при сравнении элементoв.");
        }

        /*
        /// <summary>
        /// Верификация 2х элементов, на основе заданных методов проверки.
        /// </summary>
        /// <typeparam name="T">Тип проверяемых элементов.</typeparam>
        /// <param name="actualItem"></param>
        /// <param name="expectedItem"></param>
        /// <param name="comparers"></param>
        public static void Verify<T>(this T actualItem, T expectedItem, params Func<T, Object>[] comparers) {
            Assert.IsNotNull(expectedItem);
            comparers.Each(c => {
                var resExpected = c(expectedItem);
                var resActual = c(actualItem);
                Assert.AreEqual(resExpected, resActual);
            });
        }
         */
    }
}
