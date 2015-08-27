using System.Collections.Generic;
using MainLogic.WebFiles;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Models {
    public class DomainStatsModel : BaseModel {
        public readonly PhraseEntityModel Phrase;
        public readonly List<DomainStatsEntityModel> PhraseDomains;

        public DomainStatsModel(BaseModel baseModel, List<DomainStatsEntityModel> phraseDomains, PhraseEntityModel phrase) : base(baseModel) {
            Phrase = phrase;
            PhraseDomains = phraseDomains;
        }
    }
}