using System;

namespace CommonUtils.Code {
    public static class MaybeExtentions {
        /// <summary>
        /// Безопасное использование объекта, если объект не равен null, то будет выполнен <see cref="action"/>,
        /// иначе будет прокинут null.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <typeparam name="TResult">Тип результата, который возвращает <see cref="action"/>.</typeparam>
        /// <param name="obj">Ссылка на объект.</param>
        /// <param name="action">Функция, которая выполняется над объектом.</param>
        /// <returns>Результат функции <see cref="action"/>.</returns>
        public static TResult With<T, TResult>(this T obj, Func<T, TResult> action)
            where T : class where TResult : class {
            return obj == null
                ? null
                : action(obj);
        }

        /// <summary>
        /// Безопасное использование объекта, если объект не равен null, то будет выполнен <see cref="action"/>,
        /// иначе будет прокинут null.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <typeparam name="TResult">Тип результата, который возвращает <see cref="action"/>.</typeparam>
        /// <param name="obj">Ссылка на объект.</param>
        /// <param name="action">Функция, которая выполняется над объектом.</param>
        /// <returns>Результат функции <see cref="action"/>.</returns>
        public static TResult? With<T, TResult>(this T obj, Func<T, TResult?> action)
            where T : class where TResult : struct {
            return obj == null
                ? null
                : action(obj);
        }

        /// <summary>
        /// Безопасная проверка состояния объекта <see cref="obj"/> методом <see cref="action"/>.
        /// </summary>
        /// <typeparam name="T">Тип проверяемого объекта</typeparam>
        /// <param name="obj">Проверемый объект.</param>
        /// <param name="action">Функция проверки.</param>
        /// <returns>Возвращает проверяемый объект, если объект не равен null и функция проверки вернула true, иначе null.</returns>
        public static T If<T>(this T obj, Predicate<T> action) where T : class {
            if (obj == null) {
                return null;
            }

            return action(obj)
                ? obj
                : null;
        }

        /// <summary>
        /// Безопасная проверка состояния объекта "Если НЕ" <see cref="obj"/> методом <see cref="action"/>.
        /// </summary>
        /// <typeparam name="T">Тип проверяемого объекта</typeparam>
        /// <param name="obj">Проверемый объект.</param>
        /// <param name="action">Функция проверки.</param>
        /// <returns>Возвращает проверяемый объект, если объект не равен null и функция проверки вернула false, иначе null.</returns>
        public static T Unless<T>(this T obj, Predicate<T> action) where T : class {
            if (obj == null) {
                return null;
            }

            return !action(obj)
                ? obj
                : null;
        }

        /// <summary>
        /// Выполнение функции в случае, если объект null и возвращает его после завершения <see cref="action"/>.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="obj">Объект.</param>
        /// <param name="action">Действие, которое нужно выполнить в случае, если объект null.</param>
        /// <returns>
        /// Возвращает объект.
        /// </returns>
        public static T IfNull<T>(this T obj, Action action) where T : class {
            if (obj == null) {
                action();
                return null;
            }

            return obj;
        }

        /// <summary>
        /// Выполнение функции в случае, если объект дефолтового значения и возвращает его после завершения <see cref="action"/>.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="obj">Объект.</param>
        /// <param name="action">Действие, которое нужно выполнить в случае, если объект default(T).</param>
        /// <returns>
        /// Возвращает объект.
        /// </returns>
        public static T IfNotSet<T>(this T obj, Action action) where T : struct, IComparable {
            if (obj.CompareTo(default(T)) == 0) {
                action();
            }

            return obj;
        }

        /// <summary>
        /// Выполнение функции в случае, если объект дефолтового значения и возвращает его после завершения <see cref="action"/>.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="obj">Объект.</param>
        /// <param name="action">Действие, которое нужно выполнить в случае, если объект default(T).</param>
        /// <returns>
        /// Возвращает объект.
        /// </returns>
        public static T? IfNotSet<T>(this T? obj, Action action) where T : struct, IComparable {
            if (obj == null) {
                action();
            } else if (obj.Value.CompareTo(default(T)) == 0) {
                action();
            }

            return obj;
        }

        /// <summary>
        /// Безопасное действие над объектом.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="obj">Объект.</param>
        /// <param name="action">Действие, которое нужно выполнить с объектом <see cref="obj"/>.</param>
        /// <returns>
        /// Если объект не равен null, то выполняется действие над объектом и возвращается объект, 
        /// иначе возвращается null.
        /// </returns>
        public static T Do<T>(this T obj, Action<T> action) where T : class {
            if (obj == null) {
                return null;
            }

            action(obj);
            return obj;
        }

        /// <summary>
        /// Безопасное возвращение результата функции <see cref="action"/> над объектом <see cref="obj"/>, 
        /// со значением по умолчанию.
        /// </summary>
        /// <typeparam name="T">Тип объекта <see cref="obj"/>.</typeparam>
        /// <typeparam name="TResult">Тип возвращаемого результата <see cref="action"/>.</typeparam>
        /// <param name="obj">Объект, над которым будет выполнена функция.</param>
        /// <param name="action">Функция, выполняемая над объектом.</param>
        /// <param name="failValue">Значение по умолчанию.</param>
        /// <returns>
        /// Если объект не равен null, то выполняется функция <see cref="action"/> и возвращается ее результат,
        /// иначе вернется <see cref="failValue"/>.
        /// </returns>
        public static TResult Return<T, TResult>(this T obj, Func<T, TResult> action, TResult failValue)
            where T : class {
            return obj == null
                ? failValue
                : action(obj);
        }

        /// <summary>
        /// Безопасное возвращение результата функции <see cref="action"/> над объектом <see cref="obj"/>, 
        /// со значением по умолчанию.
        /// </summary>
        /// <typeparam name="T">Тип объекта <see cref="obj"/>.</typeparam>
        /// <typeparam name="TResult">Тип возвращаемого результата <see cref="action"/>.</typeparam>
        /// <param name="obj">Объект, над которым будет выполнена функция.</param>
        /// <param name="action">Функция, выполняемая над объектом.</param>
        /// <param name="failValue">Значение по умолчанию.</param>
        /// <returns>
        /// Если объект не равен null, то выполняется функция <see cref="action"/> и возвращается ее результат,
        /// иначе вернется <see cref="failValue"/>.
        /// </returns>
        public static TResult Return<T, TResult>(this T? obj, Func<T?, TResult> action, TResult failValue)
            where T : struct {
            return obj == null
                ? failValue
                : action(obj);
        }

        /// <summary>
        /// Безопасное возвращение результата функции <see cref="action"/> над объектом <see cref="obj"/>, 
        /// со значением по умолчанию null.
        /// </summary>
        /// <typeparam name="T">Тип объекта <see cref="obj"/>.</typeparam>
        /// <typeparam name="TResult">Тип возвращаемого результата <see cref="action"/>.</typeparam>
        /// <param name="obj">Объект, над которым будет выполнена функция.</param>
        /// <param name="action">Функция, выполняемая над объектом.</param>
        /// <returns>
        /// Если объект не равен null, то выполняется функция <see cref="action"/> и возвращается ее результат,
        /// иначе вернется null.
        /// </returns>
        public static TResult Return<T, TResult>(this T obj, Func<T, TResult> action)
            where T : class where TResult : class {
            return obj == null
                ? null
                : action(obj);
        }

        /// <summary>
        /// Проверка значения на null. Используется в последовательностях с дгурими функциями <see cref="MaybeExtentions"/>.
        /// Выделена отдельно, т.к. очень часто используется true/false результат.
        /// </summary>
        /// <typeparam name="T">Тип объекта <see cref="obj"/>.</typeparam>
        /// <param name="obj">Объект, который проверяется на null.</param>
        /// <returns>
        /// Если объект не равен null, то возвращается true, иначе false.
        /// </returns>
        public static bool ReturnSuccess<T>(this T obj) where T : class {
            return obj != null;
        }
    }
}