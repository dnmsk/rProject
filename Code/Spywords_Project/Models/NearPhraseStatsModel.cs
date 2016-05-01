using System.Collections.Generic;
using MainLogic.WebFiles;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Models {
    public class NearPhraseStatsModel : BaseModel {
        public readonly List<PhraseEntityModel> Phrases;
        public readonly List<DomainStatsEntityModel> PhraseDomains;

        public NearPhraseStatsModel(BaseModel baseModel, List<DomainStatsEntityModel> phraseDomains, List<PhraseEntityModel> phrases) : base(baseModel) {
            Phrases = phrases;
            PhraseDomains = phraseDomains;
        }
    }
}