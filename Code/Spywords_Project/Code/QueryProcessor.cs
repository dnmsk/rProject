using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using CommonUtils.Code;
using CommonUtils.Core.Config;
using CommonUtils.ExtendedTypes;
using Lemmatizer;
using LemmatizerNET.Enum;

namespace Spywords_Project.Code {
    public class QueryProcessor : Singleton<QueryProcessor> {
        private const int MIN_WORD_LENGTH = 2;

        private static readonly char[] _disabledChars = {
            '@'
        };

        private static readonly char[] _splitChars = {
            ' ', ',', '.', '?', '(', ')', '/', '\\', '+', '-', '–', '—', '―', '\r', '\n', (char) 8197, '%', '$', '&',
            '\'', '"', '!', '@', '_', ':', ';', '\0', '“', '”', '«', '»'
        };

        private static readonly char[] _splitNumbers = {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private static readonly HashSet<string> _prepositions = new HashSet<string> {
            "из",
            "со",
            "за",
            "под",
            "от",
            "на",
            "по",
            "под",
            "над",
            "до",
            "при"
        };

        private static readonly string[] _suffixToRemove = {
            "чик", "щик", "чка", "щка", "ок", "онк", "енк", "ск"
        };

        private static readonly Dictionary<string, string> _suffixToReplace = new Dictionary<string, string> {
            {"рн", "р"},
            {"нн", "н"}
        };

        private static readonly string[] _unGluedNouns = {
            "спа", "био", "оил", "3д", "арт", "противо", "микро", "термо"
        };

        private static readonly string[] _rightWords = {
            "тату"
        };

        private LemmatizerRussian _lemmatizer = null;

        public QueryProcessor() {
            ConfigHelper.RegisterConfigTarget("Application.xml", xml => {
                var xmlDoc = (XmlDocument)xml;
                var container = xmlDoc.GetElementsByTagName("LemmatizerDict");
                if (container.Count != 1) {
                    return;
                }
                _lemmatizer = new LemmatizerRussian(container[0].InnerText);
            });
            for (var i = 0; i < 1000 && _lemmatizer == null; i++) {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Возвращает запрос в формате пригодном для работы в коде.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="useDisabledChars"></param>
        /// <returns></returns>
        public QueryBaseForm ProcessQueryTest(string query, bool useDisabledChars = true) {
            return ProcessQuery(query, useDisabledChars, false);
        }

        /// <summary>
        ///     Возвращает запрос в формате пригодном для работы в коде.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="useDisabledChars"></param>
        /// <param name="returnOnTest">Возвратить результат на тестовом режиме, когда результат выполнения не важен.</param>
        /// <returns></returns>
        public QueryBaseForm ProcessQuery(string query, bool useDisabledChars = true, bool returnOnTest = true) {
            if (ConfigHelper.TestMode && returnOnTest) {
                var bf = new QueryBaseForm();
                if (!string.IsNullOrWhiteSpace(query)) {
                    foreach (var s in query.Split(' ')) {
                        if (!bf.BaseMeaningWords.Contains(s)) {
                            bf.BaseMeaningWords.Add(s);
                        }
                    }
                }
                return bf;
            }

            if (string.IsNullOrWhiteSpace(query) ||
                (useDisabledChars && query.Split(_disabledChars, StringSplitOptions.RemoveEmptyEntries).Length > 1)) {
                return new QueryBaseForm();
            }

            query = Transliterator.GetRussianMin(query.ToLower());

            var resultedWords = new QueryBaseForm();

            var partsOfQuery = query.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < partsOfQuery.Length; i++) {
                var word = partsOfQuery[i];
                ProcessWord(word, resultedWords);
            }
            return resultedWords;
        }

        /// <summary>
        ///     Метод обработки слов. Разделит слова содержащие цифру на несколько.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="resultedWords"></param>
        private void ProcessWord(string word, QueryBaseForm resultedWords) {
            //Если слово имеет несколько вариантов написания слитно,раздельно или через дефис - считаем его отдельным словом и отклеиваем.
            var ungluedPart = _unGluedNouns.FirstOrDefault(ugn => word.StartsWith(ugn));
            if (!string.IsNullOrWhiteSpace(ungluedPart)) {
                if (ungluedPart == word) {
                    resultedWords.BaseUngluedWords.Add(Transliterator.GetRussianMin(ungluedPart));
                    return;
                }
                resultedWords.BaseUngluedWords.Add(Transliterator.GetRussianMin(ungluedPart));
                word = word.Substring(ungluedPart.Length);
            }

            var trySplit = word.Split(_splitNumbers, StringSplitOptions.RemoveEmptyEntries);
            switch (trySplit.Length) {
                case 0:
                    break;
                case 1:
                    word = Transliterator.GetRussianMin(ProcessBaseWord(trySplit[0]));
                    if (!string.IsNullOrWhiteSpace(word)) {
                        resultedWords.BaseMeaningWords.Add(word);
                    }
                    break;
                default:
                    foreach (var w in trySplit) {
                        ProcessWord(w, resultedWords);
                    }
                    break;
            }
        }

        /// <summary>
        ///     Обработка 1 неделимого слова
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string ProcessBaseWord(string word) {
            if (word.Length < MIN_WORD_LENGTH) {
                return string.Empty;
            }
            if (_prepositions.Contains(word)) {
                return string.Empty;
            }

            //Проверяем, что это слово уже в базовой форме, то его не нужно обрабатывать
            if (_rightWords.Contains(word)) {
                return word.ToLower();
            }
            
            word = ProcessBaseForm(word);
            return word.ToLower();
        }

        /// <summary>
        ///     Получение базовой формы для неделимого слова. Осторожно, рекурсия.
        /// </summary>
        /// <param name="rightWord"></param>
        /// <param name="tries"></param>
        /// <returns></returns>
        private string ProcessBaseForm(string rightWord, int tries = 0) {
            if (tries > 3) {
                return rightWord;
            }
            if (rightWord.Length < MIN_WORD_LENGTH) {
                return string.Empty;
            }
            var baseForms = _lemmatizer.CreateParadigmCollectionFromForm(rightWord).FirstOrDefault();
            if (baseForms == null) {
                return string.Empty;
            }
            if (baseForms.WordDescription == null) {
                return rightWord;
            }
            var lemmatizerBaseForm = TryProcessSuffix(baseForms.BaseForm);
            switch (baseForms.WordDescription.PartOfSpeeches) {
                case RPartOfSpeeches.NOUN:
                    break;
                case RPartOfSpeeches.ADJ_FULL:
                    var wordWithoutEnds = lemmatizerBaseForm.Substring(0, lemmatizerBaseForm.Length - 2);
                    lemmatizerBaseForm =
                        ProcessBaseForm(TryProcessSuffix(wordWithoutEnds), ++tries);
                    break;
                default:
                    break;
            }
            return lemmatizerBaseForm ?? string.Empty;
        }

        private string TryProcessSuffix(string word) {
            foreach (var suffix in _suffixToRemove) {
                if (word.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase) &&
                    (word.Length - suffix.Length) > 3) {
                    return ProcessBaseForm(word.Substring(0, word.Length - suffix.Length));
                }
            }
            foreach (var kv in _suffixToReplace) {
                if (word.EndsWith(kv.Key, StringComparison.InvariantCultureIgnoreCase)) {
                    return ProcessBaseForm(word.Substring(0, word.Length - kv.Key.Length) + kv.Value);
                }
            }
            return word;
        }
    }
}