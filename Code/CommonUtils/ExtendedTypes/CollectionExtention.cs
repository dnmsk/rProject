using System;
using System.Collections.Generic;

namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Модуль для расширений на коллекции.
    /// </summary>
    public static class CollectionExtention {
        /// <summary>
        /// Разбивает коллекцию на части заданного размера, возвращает коллекцию частей.
        /// </summary>
        /// <typeparam name="T">Тип объектов в коллекции.</typeparam>
        /// <param name="list">Коллекция для разделения.</param>
        /// <param name="partSize">Размер пачки.</param>
        /// <remarks>
        /// В данном методе сознательно используются ICollection и IList, а не IEnumerable, 
        /// чтобы избежать потерь производительности и памяти, а так же добавить удобства 
        /// в работе с возвращаемым результатом.
        /// </remarks>
        /// <returns>Возвращает коллекцию частей, на которые разбили исходную коллекцию.</returns>
        public static IList<IList<T>> ToParts<T>(this ICollection<T> list, int partSize) {
            if (partSize <= 0) {
                throw new ArgumentException("Размер пачки должен быть больше 0.");
            }

            var partList = new List<IList<T>>();
            if (list.Count == 0) {
                return partList;
            }

            if (list.Count <= partSize) {
                partList.Add(new List<T>(list));
                return partList;
            }

            IList<T> part = new List<T>(partSize);
            foreach (T id in list) {
                if (part.Count >= partSize) {
                    partList.Add(part);
                    part = new List<T>(partSize);
                }

                part.Add(id);
            }

            partList.Add(part);
            return partList;
        }

        /// <summary>
        /// Заполнение списка с помощью функции генерации элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов списка.</typeparam>
        /// <param name="arr">Коллекция для заполнения.</param>
        /// <param name="elemCreator">Функция генеация элемента.</param>
        /// <returns>Возвращает заполненный список.</returns>
        public static IList<T> Fill<T>(this IList<T> arr, Func<T> elemCreator) {
            return arr.Do(a => a.Count.Steps(i => { a[i] = elemCreator(); }));
        }

        /// <summary>
        /// Заполнение списка с помощью функции генерации элементов, которая зависит от индекса.
        /// </summary>
        /// <typeparam name="T">Тип элементов списка.</typeparam>
        /// <param name="arr">Коллекция для заполнения.</param>
        /// <param name="elemCreator">Функция генеация элемента, котора зависит от индекса.</param>
        /// <returns>Возвращает заполненный список.</returns>
        public static IList<T> Fill<T>(this IList<T> arr, Func<int, T> elemCreator) {
            return arr.Do(a => a.Count.Steps(i => { a[i] = elemCreator(i); }));
        }

        /// <summary>
        /// Заполнение массива с помощью функции генерации элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов массива.</typeparam>
        /// <param name="arr">Коллекция для заполнения.</param>
        /// <param name="elemCreator">Функция генеация элемента.</param>
        /// <returns>Возвращает заполненный массив.</returns>
        public static T[] Fill<T>(this T[] arr, Func<T> elemCreator) {
            return arr.Do(a => a.Length.Steps(i => { a[i] = elemCreator(); }));
        }

        /// <summary>
        /// Заполнение массива с помощью функции генерации элементов, которая зависит от индекса.
        /// </summary>
        /// <typeparam name="T">Тип элементов массива.</typeparam>
        /// <param name="arr">Коллекция для заполнения.</param>
        /// <param name="elemCreator">Функция генеация элемента, котора зависит от индекса.</param>
        /// <returns>Возвращает заполненный массив.</returns>
        public static T[] Fill<T>(this T[] arr, Func<int, T> elemCreator) {
            return arr.Do(a => a.Length.Steps(i => { a[i] = elemCreator(i); }));
        }

        /// <summary>
        /// Рандомно перемешивает элементы в списке
        /// </summary>
        /// <typeparam name="T">Тип элементов массива.</typeparam>
        /// <param name="list">список для заполнения</param>
        public static IList<T> Shuffle<T>(this IList<T> list) {
            Random rng = new Random(DateTime.Now.Millisecond);

            var ret = new List<T>(list);
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = ret[k];
                ret[k] = ret[n];
                ret[n] = value;
            }

            return ret;
        }

        /// <summary>
        /// Рандомно перемешивает элементы в массиве
        /// </summary>
        /// <typeparam name="T">Тип элементов массива.</typeparam>
        /// <param name="list">список для заполнения</param>
        public static T[] Shuffle<T>(this T[] list) {
            Random rng = new Random(DateTime.Now.Millisecond);

            var ret = (T[]) list.Clone();
            int n = list.Length;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = ret[k];
                ret[k] = ret[n];
                ret[n] = value;
            }

            return ret;
        }
    }
}
