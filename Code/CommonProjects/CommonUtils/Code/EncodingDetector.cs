using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.Code {
    public class EncodingDetector {
        /// <summary>
        /// Дефолтная кодировка
        /// </summary>
        public static Encoding StatisticsDefaultEncofing = Encoding.UTF8;

        /// <summary>
        /// Набор пар байт, которые считаем плохими
        /// </summary>
        private static readonly HashSet<Pair<byte, byte>> _badBytePairs = new HashSet<Pair<byte, byte>>(new List<Pair<byte, byte>> {
            new Pair<byte, byte>(224, 250),
            new Pair<byte, byte>(224, 251),
            new Pair<byte, byte>(224, 252),
            new Pair<byte, byte>(225, 231),
            new Pair<byte, byte>(225, 233),
            new Pair<byte, byte>(225, 239),
            new Pair<byte, byte>(225, 244),
            new Pair<byte, byte>(225, 253),
            new Pair<byte, byte>(226, 230),
            new Pair<byte, byte>(226, 233),
            new Pair<byte, byte>(226, 244),
            new Pair<byte, byte>(226, 253),
            new Pair<byte, byte>(226, 254),
            new Pair<byte, byte>(227, 230),
            new Pair<byte, byte>(227, 231),
            new Pair<byte, byte>(227, 233),
            new Pair<byte, byte>(227, 239),
            new Pair<byte, byte>(227, 244),
            new Pair<byte, byte>(227, 245),
            new Pair<byte, byte>(227, 246),
            new Pair<byte, byte>(227, 249),
            new Pair<byte, byte>(227, 250),
            new Pair<byte, byte>(227, 251),
            new Pair<byte, byte>(227, 252),
            new Pair<byte, byte>(227, 253),
            new Pair<byte, byte>(227, 254),
            new Pair<byte, byte>(227, 255),
            new Pair<byte, byte>(228, 244),
            new Pair<byte, byte>(228, 249),
            new Pair<byte, byte>(228, 253),
            new Pair<byte, byte>(229, 250),
            new Pair<byte, byte>(229, 251),
            new Pair<byte, byte>(230, 226),
            new Pair<byte, byte>(230, 231),
            new Pair<byte, byte>(230, 233),
            new Pair<byte, byte>(230, 242),
            new Pair<byte, byte>(230, 244),
            new Pair<byte, byte>(230, 245),
            new Pair<byte, byte>(230, 246),
            new Pair<byte, byte>(230, 248),
            new Pair<byte, byte>(230, 249),
            new Pair<byte, byte>(230, 250),
            new Pair<byte, byte>(230, 251),
            new Pair<byte, byte>(230, 253),
            new Pair<byte, byte>(230, 254),
            new Pair<byte, byte>(230, 255),
            new Pair<byte, byte>(231, 233),
            new Pair<byte, byte>(231, 244),
            new Pair<byte, byte>(231, 245),
            new Pair<byte, byte>(231, 246),
            new Pair<byte, byte>(231, 249),
            new Pair<byte, byte>(232, 250),
            new Pair<byte, byte>(232, 251),
            new Pair<byte, byte>(232, 252),
            new Pair<byte, byte>(233, 224),
            new Pair<byte, byte>(233, 225),
            new Pair<byte, byte>(233, 226),
            new Pair<byte, byte>(233, 230),
            new Pair<byte, byte>(233, 232),
            new Pair<byte, byte>(233, 233),
            new Pair<byte, byte>(233, 240),
            new Pair<byte, byte>(233, 249),
            new Pair<byte, byte>(233, 250),
            new Pair<byte, byte>(233, 251),
            new Pair<byte, byte>(233, 252),
            new Pair<byte, byte>(233, 253),
            new Pair<byte, byte>(233, 254),
            new Pair<byte, byte>(234, 225),
            new Pair<byte, byte>(234, 227),
            new Pair<byte, byte>(234, 228),
            new Pair<byte, byte>(234, 233),
            new Pair<byte, byte>(234, 236),
            new Pair<byte, byte>(234, 239),
            new Pair<byte, byte>(234, 244),
            new Pair<byte, byte>(234, 245),
            new Pair<byte, byte>(234, 247),
            new Pair<byte, byte>(234, 249),
            new Pair<byte, byte>(234, 250),
            new Pair<byte, byte>(234, 251),
            new Pair<byte, byte>(234, 252),
            new Pair<byte, byte>(234, 253),
            new Pair<byte, byte>(234, 255),
            new Pair<byte, byte>(235, 240),
            new Pair<byte, byte>(235, 244),
            new Pair<byte, byte>(235, 246),
            new Pair<byte, byte>(235, 250),
            new Pair<byte, byte>(235, 253),
            new Pair<byte, byte>(236, 230),
            new Pair<byte, byte>(236, 231),
            new Pair<byte, byte>(236, 233),
            new Pair<byte, byte>(236, 242),
            new Pair<byte, byte>(236, 244),
            new Pair<byte, byte>(236, 249),
            new Pair<byte, byte>(236, 250),
            new Pair<byte, byte>(236, 254),
            new Pair<byte, byte>(237, 233),
            new Pair<byte, byte>(237, 235),
            new Pair<byte, byte>(237, 239),
            new Pair<byte, byte>(237, 245),
            new Pair<byte, byte>(237, 250),
            new Pair<byte, byte>(238, 250),
            new Pair<byte, byte>(238, 251),
            new Pair<byte, byte>(238, 252),
            new Pair<byte, byte>(239, 225),
            new Pair<byte, byte>(239, 226),
            new Pair<byte, byte>(239, 227),
            new Pair<byte, byte>(239, 228),
            new Pair<byte, byte>(239, 230),
            new Pair<byte, byte>(239, 231),
            new Pair<byte, byte>(239, 233),
            new Pair<byte, byte>(239, 236),
            new Pair<byte, byte>(239, 244),
            new Pair<byte, byte>(239, 245),
            new Pair<byte, byte>(239, 249),
            new Pair<byte, byte>(239, 250),
            new Pair<byte, byte>(240, 250),
            new Pair<byte, byte>(240, 253),
            new Pair<byte, byte>(241, 233),
            new Pair<byte, byte>(242, 230),
            new Pair<byte, byte>(242, 233),
            new Pair<byte, byte>(243, 243),
            new Pair<byte, byte>(243, 244),
            new Pair<byte, byte>(243, 250),
            new Pair<byte, byte>(243, 251),
            new Pair<byte, byte>(243, 252),
            new Pair<byte, byte>(244, 225),
            new Pair<byte, byte>(244, 226),
            new Pair<byte, byte>(244, 227),
            new Pair<byte, byte>(244, 228),
            new Pair<byte, byte>(244, 230),
            new Pair<byte, byte>(244, 231),
            new Pair<byte, byte>(244, 233),
            new Pair<byte, byte>(244, 234),
            new Pair<byte, byte>(244, 239),
            new Pair<byte, byte>(244, 244),
            new Pair<byte, byte>(244, 245),
            new Pair<byte, byte>(244, 246),
            new Pair<byte, byte>(244, 247),
            new Pair<byte, byte>(244, 248),
            new Pair<byte, byte>(244, 249),
            new Pair<byte, byte>(244, 250),
            new Pair<byte, byte>(244, 252),
            new Pair<byte, byte>(244, 253),
            new Pair<byte, byte>(244, 255),
            new Pair<byte, byte>(245, 225),
            new Pair<byte, byte>(245, 230),
            new Pair<byte, byte>(245, 231),
            new Pair<byte, byte>(245, 233),
            new Pair<byte, byte>(245, 244),
            new Pair<byte, byte>(245, 245),
            new Pair<byte, byte>(245, 246),
            new Pair<byte, byte>(245, 247),
            new Pair<byte, byte>(245, 249),
            new Pair<byte, byte>(245, 250),
            new Pair<byte, byte>(245, 251),
            new Pair<byte, byte>(245, 252),
            new Pair<byte, byte>(245, 254),
            new Pair<byte, byte>(245, 255),
            new Pair<byte, byte>(246, 225),
            new Pair<byte, byte>(246, 227),
            new Pair<byte, byte>(246, 228),
            new Pair<byte, byte>(246, 230),
            new Pair<byte, byte>(246, 231),
            new Pair<byte, byte>(246, 233),
            new Pair<byte, byte>(246, 237),
            new Pair<byte, byte>(246, 239),
            new Pair<byte, byte>(246, 240),
            new Pair<byte, byte>(246, 241),
            new Pair<byte, byte>(246, 242),
            new Pair<byte, byte>(246, 244),
            new Pair<byte, byte>(246, 245),
            new Pair<byte, byte>(246, 246),
            new Pair<byte, byte>(246, 247),
            new Pair<byte, byte>(246, 248),
            new Pair<byte, byte>(246, 249),
            new Pair<byte, byte>(246, 250),
            new Pair<byte, byte>(246, 252),
            new Pair<byte, byte>(246, 253),
            new Pair<byte, byte>(246, 254),
            new Pair<byte, byte>(246, 255),
            new Pair<byte, byte>(247, 225),
            new Pair<byte, byte>(247, 227),
            new Pair<byte, byte>(247, 228),
            new Pair<byte, byte>(247, 230),
            new Pair<byte, byte>(247, 231),
            new Pair<byte, byte>(247, 233),
            new Pair<byte, byte>(247, 239),
            new Pair<byte, byte>(247, 241),
            new Pair<byte, byte>(247, 245),
            new Pair<byte, byte>(247, 246),
            new Pair<byte, byte>(247, 247),
            new Pair<byte, byte>(247, 249),
            new Pair<byte, byte>(247, 250),
            new Pair<byte, byte>(247, 251),
            new Pair<byte, byte>(247, 253),
            new Pair<byte, byte>(247, 254),
            new Pair<byte, byte>(247, 255),
            new Pair<byte, byte>(248, 225),
            new Pair<byte, byte>(248, 227),
            new Pair<byte, byte>(248, 230),
            new Pair<byte, byte>(248, 231),
            new Pair<byte, byte>(248, 233),
            new Pair<byte, byte>(248, 244),
            new Pair<byte, byte>(248, 248),
            new Pair<byte, byte>(248, 249),
            new Pair<byte, byte>(248, 250),
            new Pair<byte, byte>(248, 251),
            new Pair<byte, byte>(248, 253),
            new Pair<byte, byte>(248, 255),
            new Pair<byte, byte>(249, 225),
            new Pair<byte, byte>(249, 226),
            new Pair<byte, byte>(249, 227),
            new Pair<byte, byte>(249, 228),
            new Pair<byte, byte>(249, 230),
            new Pair<byte, byte>(249, 231),
            new Pair<byte, byte>(249, 233),
            new Pair<byte, byte>(249, 234),
            new Pair<byte, byte>(249, 235),
            new Pair<byte, byte>(249, 236),
            new Pair<byte, byte>(249, 239),
            new Pair<byte, byte>(249, 241),
            new Pair<byte, byte>(249, 242),
            new Pair<byte, byte>(249, 244),
            new Pair<byte, byte>(249, 245),
            new Pair<byte, byte>(249, 246),
            new Pair<byte, byte>(249, 247),
            new Pair<byte, byte>(249, 248),
            new Pair<byte, byte>(249, 249),
            new Pair<byte, byte>(249, 250),
            new Pair<byte, byte>(249, 251),
            new Pair<byte, byte>(249, 253),
            new Pair<byte, byte>(249, 254),
            new Pair<byte, byte>(249, 255),
            new Pair<byte, byte>(250, 224),
            new Pair<byte, byte>(250, 225),
            new Pair<byte, byte>(250, 226),
            new Pair<byte, byte>(250, 227),
            new Pair<byte, byte>(250, 228),
            new Pair<byte, byte>(250, 230),
            new Pair<byte, byte>(250, 231),
            new Pair<byte, byte>(250, 232),
            new Pair<byte, byte>(250, 233),
            new Pair<byte, byte>(250, 234),
            new Pair<byte, byte>(250, 235),
            new Pair<byte, byte>(250, 236),
            new Pair<byte, byte>(250, 237),
            new Pair<byte, byte>(250, 238),
            new Pair<byte, byte>(250, 239),
            new Pair<byte, byte>(250, 240),
            new Pair<byte, byte>(250, 241),
            new Pair<byte, byte>(250, 242),
            new Pair<byte, byte>(250, 243),
            new Pair<byte, byte>(250, 244),
            new Pair<byte, byte>(250, 245),
            new Pair<byte, byte>(250, 246),
            new Pair<byte, byte>(250, 247),
            new Pair<byte, byte>(250, 248),
            new Pair<byte, byte>(250, 249),
            new Pair<byte, byte>(250, 250),
            new Pair<byte, byte>(250, 251),
            new Pair<byte, byte>(250, 252),
            new Pair<byte, byte>(250, 253),
            new Pair<byte, byte>(251, 224),
            new Pair<byte, byte>(251, 238),
            new Pair<byte, byte>(251, 244),
            new Pair<byte, byte>(251, 250),
            new Pair<byte, byte>(251, 251),
            new Pair<byte, byte>(251, 252),
            new Pair<byte, byte>(251, 253),
            new Pair<byte, byte>(251, 254),
            new Pair<byte, byte>(252, 224),
            new Pair<byte, byte>(252, 230),
            new Pair<byte, byte>(252, 233),
            new Pair<byte, byte>(252, 235),
            new Pair<byte, byte>(252, 239),
            new Pair<byte, byte>(252, 240),
            new Pair<byte, byte>(252, 243),
            new Pair<byte, byte>(252, 250),
            new Pair<byte, byte>(252, 251),
            new Pair<byte, byte>(252, 253),
            new Pair<byte, byte>(253, 224),
            new Pair<byte, byte>(253, 225),
            new Pair<byte, byte>(253, 226),
            new Pair<byte, byte>(253, 228),
            new Pair<byte, byte>(253, 230),
            new Pair<byte, byte>(253, 232),
            new Pair<byte, byte>(253, 233),
            new Pair<byte, byte>(253, 238),
            new Pair<byte, byte>(253, 246),
            new Pair<byte, byte>(253, 247),
            new Pair<byte, byte>(253, 248),
            new Pair<byte, byte>(253, 249),
            new Pair<byte, byte>(253, 250),
            new Pair<byte, byte>(253, 251),
            new Pair<byte, byte>(253, 252),
            new Pair<byte, byte>(253, 253),
            new Pair<byte, byte>(253, 254),
            new Pair<byte, byte>(253, 255),
            new Pair<byte, byte>(254, 224),
            new Pair<byte, byte>(254, 232),
            new Pair<byte, byte>(254, 238),
            new Pair<byte, byte>(254, 243),
            new Pair<byte, byte>(254, 244),
            new Pair<byte, byte>(254, 250),
            new Pair<byte, byte>(254, 251),
            new Pair<byte, byte>(254, 252),
            new Pair<byte, byte>(254, 253),
            new Pair<byte, byte>(254, 255),
            new Pair<byte, byte>(255, 224),
            new Pair<byte, byte>(255, 238),
            new Pair<byte, byte>(255, 244),
            new Pair<byte, byte>(255, 250),
            new Pair<byte, byte>(255, 251),
            new Pair<byte, byte>(255, 252),
            new Pair<byte, byte>(255, 253)
        });

        /// <summary>
        /// Кодировка, в которой текст находится по умолчанию (по крайней мере мы так думаем)
        /// </summary>
        private static readonly Encoding _trunkEncoding = Encoding.GetEncoding(1251);

        /// <summary>
        /// Кодировки для которых будем определять похожесть
        /// </summary>
        private static readonly List<Encoding> _encodings = new List<Encoding> {
            Encoding.GetEncoding(1251),
            Encoding.GetEncoding("koi8-r"),
            Encoding.UTF8
        };

        /// <summary>
        /// Получает 255 байт, начиная с первого не ascii символа
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static byte[] GetTextSnapshot(IEnumerable<byte> content) {
            // TODO: optimize
            IEnumerable<byte> skipWhile = content.SkipWhile(b => b <= 128).ToArray();

            int noAsciiCount = 0;
            int indexLastNoAscii = 0;
            int count = 0;
            foreach (byte b in skipWhile) {
                count++;
                if (b > 128) {
                    noAsciiCount++;
                    indexLastNoAscii = count;
                }
                if (noAsciiCount > 255) {
                    break;
                }
            }

            return skipWhile.Take(indexLastNoAscii).ToArray();
        }
        /// <summary>
        /// Преобразует последоватеьность байт в нижний регистра
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static void BytesToLowercase(byte[] bytes) {
            if (bytes == null)
                return;
            for (int i = 0; i < bytes.Length; i++) {
                byte b = bytes[i];
                if ((b >= 65 && b <= 90) || (b >= 192 && b <= 223)) {
                    bytes[i] += 32;
                }
            }
        }

        /// <summary>
        /// считаем количество плохих пар байт
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static int GetBadBytePairCount(byte[] bytes) {
            int count = 0;
            var pair = new Pair<byte, byte>(0, 0);
            for (int i = 0; i + 1 < bytes.Length; i++) {
                pair.Value1 = bytes[i];
                pair.Value2 = bytes[i + 1];
                if (_badBytePairs.Contains(pair)) {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// Находит наиболее вероятную кодировку в тексте на основе статистических данных
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Encoding DetectFromStatistic(IEnumerable<byte> content) {
            const int GOOD_BYTE_COUNT_MIN = 64;
            byte[] snapshot = GetTextSnapshot(content);

            int? goodByteCountMax = null;
            int? koeffMin = null;
            int koefMinIndex = 0;
            for (int i = 0; i < _encodings.Count; i++) {
                byte[] converteBytes = Encoding.Convert(_encodings[i], _trunkEncoding, snapshot);
                BytesToLowercase(converteBytes);

                var w = GetEncodingStatistic(converteBytes);
                if (!goodByteCountMax.HasValue || goodByteCountMax.Value < w.GoodByteCount) {
                    goodByteCountMax = w.GoodByteCount;
                }
                int koeffEqual = w.KoeffEqual;
                if (!koeffMin.HasValue || koeffMin.Value > koeffEqual) {
                    koeffMin = koeffEqual;
                    koefMinIndex = i;
                }
            }
            return (goodByteCountMax < GOOD_BYTE_COUNT_MIN) ? StatisticsDefaultEncofing : _encodings[koefMinIndex];
        }

        /// <summary>
        /// Определяем численные показатель соответствия кодировки
        /// </summary>
        /// <param name="snapshotLowercase"></param>
        /// <returns></returns>
        private static EncodingStatistic GetEncodingStatistic(byte[] snapshotLowercase) {
            int goodByteCount = 0;
            int badByteCount = 0;
            if (snapshotLowercase != null) {
                for (int i = 0; i < snapshotLowercase.Length; i++) {
                    byte b = snapshotLowercase[i];
                    if (b >= 192) {
                        goodByteCount++;
                    } else if ((b < 32 && b != 9 && b != 10 && b != 13)
                        || ((b >= 128 && b <= 191)
                            && b != 168
                            && b != 171
                            && b != 184
                            && b != 187
                            && b != 160)
                        || b == 63) {
                        badByteCount++;
                    }
                }
            }
            EncodingStatistic stat = new EncodingStatistic {
                GoodByteCount = goodByteCount,
                BadByteCount = badByteCount,
                BadBytePairCount = GetBadBytePairCount(snapshotLowercase)
            };

            return stat;
        }
        private class EncodingStatistic {
            /// <summary>
            /// Количество хороших байт
            /// </summary>
            public int GoodByteCount { get; set; }
            /// <summary>
            /// Количество плохих байт
            /// </summary>
            public int BadByteCount { get; set; }
            /// <summary>
            /// Количество плохих пар байт
            /// </summary>
            public int BadBytePairCount { get; set; }
            /// <summary>
            /// Цисленный показатель соответствия кодировке
            /// </summary>
            public int KoeffEqual {
                get {
                    return 64 * BadBytePairCount + 32 * BadByteCount - GoodByteCount;
                }
            }
        }
        private class Pair<T1, T2> {
            public T1 Value1;
            public T2 Value2;
            public Pair(T1 value1, T2 value2) {
                Value1 = value1;
                Value2 = value2;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }
                if (ReferenceEquals(this, obj)) {
                    return true;
                }
                if (obj.GetType() != typeof(Pair<T1, T2>)) {
                    return false;
                }
                return Equals((Pair<T1, T2>)obj);
            }

            public bool Equals(Pair<T1, T2> other) {
                if (ReferenceEquals(null, other)) {
                    return false;
                }
                if (ReferenceEquals(this, other)) {
                    return true;
                }
                return Equals(other.Value1, Value1) && Equals(other.Value2, Value2);
            }

            public override int GetHashCode() {
                unchecked {
                    return (Value1.GetHashCode() * 397) ^ Value2.GetHashCode();
                }
            }
        }
    }
}
