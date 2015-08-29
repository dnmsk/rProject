using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Code {
    public static class Transliterator {
        private static readonly Dictionary<string, string> _isoRuToEn = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _isoEnToRu = new Dictionary<string, string>();

        static Transliterator() {
            _isoRuToEn.Add("А", "A");
            _isoRuToEn.Add("Б", "B");
            _isoRuToEn.Add("В", "V");
            _isoRuToEn.Add("Г", "G");
            _isoRuToEn.Add("Д", "D");
            _isoRuToEn.Add("Е", "E");
            _isoRuToEn.Add("Ё", "Yo");
            _isoRuToEn.Add("Ж", "Zh");
            _isoRuToEn.Add("З", "Z");
            _isoRuToEn.Add("И", "I");
            _isoRuToEn.Add("Й", "J");
            _isoRuToEn.Add("К", "K");
            _isoRuToEn.Add("Л", "L");
            _isoRuToEn.Add("М", "M");
            _isoRuToEn.Add("Н", "N");
            _isoRuToEn.Add("О", "O");
            _isoRuToEn.Add("П", "P");
            _isoRuToEn.Add("Р", "R");
            _isoRuToEn.Add("С", "S");
            _isoRuToEn.Add("Т", "T");
            _isoRuToEn.Add("У", "U");
            _isoRuToEn.Add("Ф", "F");
            _isoRuToEn.Add("Х", "H");
            _isoRuToEn.Add("Ц", "C");
            _isoRuToEn.Add("Ч", "Ch");
            _isoRuToEn.Add("Ш", "Sh");
            _isoRuToEn.Add("Щ", "Sch");
            _isoRuToEn.Add("Ъ", "");
            _isoRuToEn.Add("Ы", "Y");
            _isoRuToEn.Add("Ь", "'");
            _isoRuToEn.Add("Э", "E");
            _isoRuToEn.Add("Ю", "Yu");
            _isoRuToEn.Add("Я", "Ya");
            _isoRuToEn.Add("а", "a");
            _isoRuToEn.Add("б", "b");
            _isoRuToEn.Add("в", "v");
            _isoRuToEn.Add("г", "g");
            _isoRuToEn.Add("д", "d");
            _isoRuToEn.Add("е", "e");
            _isoRuToEn.Add("ё", "yo");
            _isoRuToEn.Add("ж", "zh");
            _isoRuToEn.Add("з", "z");
            _isoRuToEn.Add("и", "i");
            _isoRuToEn.Add("й", "j");
            _isoRuToEn.Add("к", "k");
            _isoRuToEn.Add("л", "l");
            _isoRuToEn.Add("м", "m");
            _isoRuToEn.Add("н", "n");
            _isoRuToEn.Add("о", "o");
            _isoRuToEn.Add("п", "p");
            _isoRuToEn.Add("р", "r");
            _isoRuToEn.Add("с", "s");
            _isoRuToEn.Add("т", "t");
            _isoRuToEn.Add("у", "u");
            _isoRuToEn.Add("ф", "f");
            _isoRuToEn.Add("х", "h");
            _isoRuToEn.Add("ц", "c");
            _isoRuToEn.Add("ч", "ch");
            _isoRuToEn.Add("ш", "sh");
            _isoRuToEn.Add("щ", "sch");
            _isoRuToEn.Add("ъ", "");
            _isoRuToEn.Add("ы", "y");
            _isoRuToEn.Add("ь", "");
            _isoRuToEn.Add("э", "e");
            _isoRuToEn.Add("ю", "yu");
            _isoRuToEn.Add("я", "ya");
            _isoRuToEn.Add(" ", " ");
            _isoRuToEn.Add("1", "1");
            _isoRuToEn.Add("2", "2");
            _isoRuToEn.Add("3", "3");
            _isoRuToEn.Add("4", "4");
            _isoRuToEn.Add("5", "5");
            _isoRuToEn.Add("6", "6");
            _isoRuToEn.Add("7", "7");
            _isoRuToEn.Add("8", "8");
            _isoRuToEn.Add("9", "9");
            _isoRuToEn.Add("0", "0");
            _isoRuToEn.Add("-", "-");

            _isoRuToEn
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .ToList()
                .ForEach(v => {
                    if (!_isoEnToRu.ContainsKey(v.Value)) {
                        _isoEnToRu.Add(v.Value, v.Key);
                    }
                });
        }

        private static List<string> GetRussianSimpleWord(string str) {
            if (string.IsNullOrEmpty(str)) {
                return new List<string> {string.Empty};
            }
            var variants = new List<string>();
            var text = string.Empty;
            for (var maxLookingpos = 0; maxLookingpos < str.Length; maxLookingpos++) {
                var symbol = str[maxLookingpos];
                string detected;
                if (_isoEnToRu.TryGetValue((text + symbol).ToLower(), out detected)) {
                    variants.AddRange(
                        GetRussianSimpleWord(str.Substring(maxLookingpos + 1)).Select(s => detected + s).ToList());
                }
                else if (string.IsNullOrEmpty(text)) {
                    variants.AddRange(
                        GetRussianSimpleWord(str.Substring(maxLookingpos + 1)).Select(s => symbol + s).ToList());
                }
                text += symbol;
            }
            return variants;
        }

        /// <summary>
        ///     Транслитерирует входную строку, используя минимальный вариант решения в случае коллизии.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetRussianMin(string str) {
            var result = new List<string>();

            result.AddRange(str
                .Split(' ')
                .Select(GetRussianSimpleWord)
                .Select(s => {
                    var res = s.FirstOrDefault();
                    if (string.IsNullOrEmpty(res)) {
                        return string.Empty;
                    }
                    foreach (var outRes in s) {
                        if (res.Length > outRes.Length) {
                            res = outRes;
                        }
                    }
                    return res;
                }));
            return result.StrJoin(" ");
        }

        public static string GetTranslit(string str, bool keepUnknownSymbols = true) {
            var res = string.Empty;
            foreach (var ch in str) {
                string s;
                var charAsString = ch.ToString(CultureInfo.InvariantCulture);
                if (!_isoRuToEn.TryGetValue(charAsString, out s)) {
                    s = (keepUnknownSymbols || _isoEnToRu.ContainsKey(charAsString))
                        ? charAsString
                        : string.Empty;
                }
                res += s;
            }
            return res;
        }
    }
}