using System;

namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Класс с методами расширения для int-типов.
    /// </summary>
    public static class IntExtentions {
        /// <summary>
        /// Выполняет блок <see cref="action"/> для всех элементов последовательности, начиная со <see cref="start"/> 
        /// с шагом <see cref="step"/> до <see cref="limit"/>.
        /// </summary>
        /// <param name="start">Начало последовательности.</param>
        /// <param name="limit">Придел/окончание последовательности.</param>
        /// <param name="step">Шаг итерации.</param>
        /// <param name="action">Действие.</param>
        public static void Step(this int start, int limit, int step, Action<int> action) {
            for (int i = start; i < limit; i += step) {
                action(i);
            }
        }
        
        /// <summary>
        /// Выполняет блок <see cref="action"/> для всех элементов последовательности, начиная со <see cref="start"/> 
        /// с шагом 1 до <see cref="limit"/>.
        /// </summary>
        /// <param name="start">Начало последовательности.</param>
        /// <param name="limit">Придел/окончание последовательности.</param>
        /// <param name="action">Действие.</param>
        public static void Step(this int start, int limit, Action<int> action) {
            start.Step(limit, 1, action);
        }

        /// <summary>
        /// Выполняет блок <see cref="action"/> для всех элементов последовательности, начиная с 0 
        /// с шагом 1 до <see cref="limit"/>.
        /// </summary>
        /// <param name="limit">Придел/окончание последовательности.</param>
        /// <param name="action">Действие.</param>
        public static void Steps(this int limit, Action<int> action) {
            0.Step(limit, 1, action);
        }

        /// <summary>
        /// Выполняет блок <see cref="action"/> для всех элементов последовательности, начиная с 0
        /// с шагом <see cref="step"/> до <see cref="limit"/>.
        /// </summary>
        /// <param name="limit">Придел/окончание последовательности.</param>
        /// <param name="step">Шаг итерации.</param>
        /// <param name="action">Действие.</param>
        public static void Steps(this int limit, int step, Action<int> action) {
            0.Step(limit, step, action);
        }
    }
}