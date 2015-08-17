namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Расширение для определения четных и не четных чисел.
    /// </summary>
    public static class EvenExtentions {
        /// <summary>
        /// Проверка числа на нечетность, возвращает true, если число четное, иначе false.
        /// </summary>
        /// <param name="value">Число для проверки.</param>
        /// <returns>Возвращает true, если число нечетное, иначе false.</returns>
        public static bool Odd(this int value) {
            return !value.Even();
        }

        /// <summary>
        /// Проверка числа на нечетность, возвращает true, если число четное, иначе false.
        /// </summary>
        /// <param name="value">Число для проверки.</param>
        /// <returns>Возвращает true, если число нечетное, иначе false.</returns>
        public static bool Odd(this long value) {
            return !value.Even();
        }

        /// <summary>
        /// Проверка числа на нечетность, возвращает true, если число четное, иначе false.
        /// </summary>
        /// <param name="value">Число для проверки.</param>
        /// <returns>Возвращает true, если число нечетное, иначе false.</returns>
        public static bool UnEven(this int value) {
            return !value.Even();
        }

        /// <summary>
        /// Проверка числа на нечетность, возвращает true, если число четное, иначе false.
        /// </summary>
        /// <param name="value">Число для проверки.</param>
        /// <returns>Возвращает true, если число нечетное, иначе false.</returns>
        public static bool UnEven(this long value) {
            return !value.Even();
        }

        /// <summary>
        /// Проверка числа на четность, возвращает true, если число четное, иначе false.
        /// </summary>
        /// <param name="value">Число для проверки.</param>
        /// <returns>Возвращает true, если число четное, иначе false.</returns>
        public static bool Even(this int value) {
            return value % 2 == 0;
        }

        /// <summary>
        /// Проверка числа на четность, возвращает true, если число четное, иначе false.
        /// </summary>
        /// <param name="value">Число для проверки.</param>
        /// <returns>Возвращает true, если число четное, иначе false.</returns>
        public static bool Even(this long value) {
            return value % 2 == 0;
        }
    }
}
