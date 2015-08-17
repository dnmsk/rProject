using System;

namespace CommonUtils.ExtendedTypes {
    public static class BitExtention {
        /// <summary>
        /// Выставляет биты
        /// </summary>
        /// <param name="src"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FlagSet<T>(this T src, T value) where T : struct, IConvertible {
            Type type;
#pragma warning disable 184 //Решарпер гонит! Климанов.
            if (value is Enum) {
#pragma warning restore 184
                type = Enum.GetUnderlyingType(typeof(T));
            } else {
                type = typeof(T);
            }

            if (type == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)src | (byte)(object)value);
            }
            if (type == typeof(short)) {
                return (T)(object)(short)((short)(object)src | (short)(object)value);
            }
            if (type == typeof(int)) {
                return (T)(object)((int)(object)src | (int)(object)value);
            }
            if (type == typeof(long)) {
                return (T)(object)((long)(object)src | (long)(object)value);
            }
            throw new NotImplementedException("Не известный тип " + type);
        }

        /// <summary>
        /// Сбрасывает биты
        /// </summary>
        /// <param name="src"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FlagDrop<T>(this T src, T value) where T : struct, IConvertible {
            Type type;
#pragma warning disable 184 //Решарпер гонит! Климанов.
            if (value is Enum) {
#pragma warning restore 184
                type = Enum.GetUnderlyingType(typeof(T));
            } else {
                type = typeof(T);
            }

            if (type == typeof(byte)) {
                return (T)(object)(byte)(((byte)(object)src & (byte.MaxValue ^ (byte)(object)value)));
            }
            if (type == typeof(short)) {
                return (T)(object)(short)(((short)(object)src & (short.MaxValue ^ (short)(object)value)));
            }
            if (type == typeof(int)) {
                return (T)(object)(int)(((int)(object)src & (int.MaxValue ^ (int)(object)value)));
            }
            if (type == typeof(long)) {
                return (T)(object)(long)(((long)(object)src & (long.MaxValue ^ (long)(object)value)));
            }
            throw new NotImplementedException("Не известный тип " + type);
        }
    }
}
