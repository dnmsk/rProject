using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Extension-методы для IEnumerable 
    /// </summary>
    public static class EnumerableExtensions {
        /// <summary>
        /// Записывает коллекцию в виде строки: 1,2,3
        /// </summary>
        /// <param name="list">Коллекция</param>
        /// <param name="separator">Разделитель элементов</param>
        /// <returns>Строка из элементов коллекции</returns>
        public static string StrJoin(this IEnumerable list, string separator) {
            var res = new StringBuilder();
            bool first = true;
            foreach (object item in list) {
                if (!first) {
                    res.Append(separator);
                } else {
                    first = false;
                }
                res.Append(item.ToString());
            }
            return res.ToString();
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum Double value 
        /// if the sequence is not empty; otherwise returns the specified default value. 
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The maximum value in the sequence or default value if sequence is empty.</returns>
        public static T MaxOrDefault<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> selector, T defaultValue) {
            return !source.Any() 
                ? defaultValue 
                : source.Select(selector).Max();
        }

        public static decimal SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue) {
            return !source.Any() ? defaultValue : source.Sum(selector);
        }

        /// <summary>
        /// Для каждого элемента коллекции выполняет <see cref="action"/>.
        /// </summary>
        /// <typeparam name="T">Тип объектов в коллекции.</typeparam>
        /// <param name="list">Коллекция для обхода.</param>
        /// <param name="action">Функция над елементами коллекции.</param>
        public static void Each<T>(this IEnumerable<T> list, Action<T> action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }

            foreach (var elem in list) {
                action(elem);
            }
        }

        /// <summary>
        /// Для каждого элемента коллекции выполняет <see cref="action"/> и предоставляет доступ к индексу/counter-у.
        /// </summary>
        /// <typeparam name="T">Тип объектов в коллекции.</typeparam>
        /// <param name="list">Коллекция для обхода.</param>
        /// <param name="action">Функция над елементами коллекции.</param>
        public static void Each<T>(this IEnumerable<T> list, Action<int, T> action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }

            int index = 0;
            foreach (var elem in list) {
                action(index++, elem);
            }
        }

        /// <summary>
        /// Расчитывает значение медианы для входной последовательности
        /// </summary>
        /// <param name="source"></param>
        /// <returns>значение медианы для входной последовательности</returns>
        public static int Median(this IEnumerable<int> source) {
            if (!source.Any()) {
                return 0;
            }

            var sortedList = from number in source orderby number select number;
            int itemIndex = sortedList.Count() / 2;

            if (sortedList.Count() % 2 == 0) {
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            }
            return sortedList.ElementAt(itemIndex);
        }

        public static List<T> DistinctBy<T, TKey>(this IList<T> source, Func<T, TKey> selector) {
            return source
                .GroupBy(selector)
                      .Select(g => g.First())
                      .ToList();            
        }

        public static void ForEachAsParallel<T>(this IEnumerable<T> source, Action<T> action, int degreeOfParallelism) {
            source
                .AsParallel()
                .WithDegreeOfParallelism(degreeOfParallelism)
                .ForAll(action);
        }

        public static IEnumerable<T> GetFlags<T>(this Enum input) where T : struct {
            return Enum.GetValues(input.GetType()).Cast<T>().Where(v => input.HasFlag((Enum)(object)v));
        }
    }
}
