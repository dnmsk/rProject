using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;

namespace Spywords_Project.Code {
    public class QueryBaseForm {
        public QueryBaseForm() {
            BaseMeaningWords = new HashSet<string>();
            BaseUngluedWords = new List<string>();
        }

        /// <summary>
        ///     Значащие слова в запросе
        /// </summary>
        public HashSet<string> BaseMeaningWords { get; }

        /// <summary>
        ///     Хитрожопые приставки, типа микро/био/etc. Учитываются при поиске, при подсказках.
        /// </summary>
        public List<string> BaseUngluedWords { get; }

        /// <summary>
        ///     Все слова подходящие для выполнения действия над ними.
        /// </summary>
        public HashSet<string> BaseWords {
            get { return new HashSet<string>(BaseMeaningWords.Union(BaseUngluedWords).Distinct()); }
        }

        /// <summary>
        ///     Все отсортированные слова, соединенные через пробел
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return BaseMeaningWords.Union(BaseUngluedWords).OrderBy(s => s).StrJoin(" ");
        }
    }
}