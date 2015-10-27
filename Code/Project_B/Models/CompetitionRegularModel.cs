using System;
using System.Collections.Generic;
using MainLogic.WebFiles;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class CompetitionRegularModel : BaseModel {
        public LanguageType CurrentLanguage { get; private set; }
        public DateTime DateUtc { get; set; }
        public List<CompetitionItemBetShortModel> CompetitionModel { get; set; }
        public Dictionary<int, ResultModel> ResultMap { get; set; }
        public int LimitToDisplayInGroup = int.MaxValue;
        public CompetitionRegularModel(LanguageType currentLanguage, BaseModel baseModel) : base(baseModel) {
            CurrentLanguage = currentLanguage;
        }
    }
}