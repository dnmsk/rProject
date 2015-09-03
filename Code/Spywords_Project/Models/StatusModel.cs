using MainLogic.WebFiles;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Models {
    public class StatusModel : BaseModel {
        public ProgressStatusSummary ServerProgress { get; set; }

        public StatusModel(BaseModel baseModel, ProgressStatusSummary serverProgress) : base(baseModel) {
            ServerProgress = serverProgress;
        }
    }
}