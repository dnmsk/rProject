using System;
using System.Collections.Generic;
using MainLogic.WebFiles;

namespace Project_B.Models {
    public class CompetitionRegularModel : BaseModel {
        public DateTime DateUtc { get; set; }
        public List<CompetitionItemRegilarModel> CompetitionModel { get; set; }
        public CompetitionRegularModel(BaseModel baseModel) : base(baseModel) {
        }
    }
}