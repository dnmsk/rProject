using System.Linq;
using MainLogic.WebFiles;
using Project_B.CodeClientSide.Enums;

namespace Project_B {
    public static class BaseModelConfig {
        public static void ConfigureBaseModel() {
            BaseModel.ConfigurePolicyStore(UserPolicyLocal.IsPageEditor, sessionModule => {
                var configurationProperty = SiteConfiguration.GetConfigurationProperty<int[]>("PageEditorAccountIDs");
                return configurationProperty != null && configurationProperty.Contains(sessionModule.AccountID);
            });
        }
    }
}