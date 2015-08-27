using System.Collections.Generic;
using MainLogic.WebFiles;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Models {
    public class PhraseModel : BaseModel {
        public readonly List<PhraseEntityModel> PhraseEntities;

        public PhraseModel(BaseModel baseModel, List<PhraseEntityModel> phraseEntities) : base(baseModel) {
            PhraseEntities = phraseEntities;
        }
    }
}