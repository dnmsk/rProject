using System.Linq;

namespace MainLogic.WebFiles {
    public class BaseModel {
        private bool? _isStatisticDisabled;
        public bool IsStatisticDisabled {
            get {
                if (_isStatisticDisabled.HasValue) {
                    return _isStatisticDisabled.Value;
                }
                var configurationProperty = SiteConfiguration.GetConfigurationProperty<int[]>("AccountIDsDisabledStatistic");
                _isStatisticDisabled = configurationProperty == null || configurationProperty.Contains(SessionModule.AccountID);
                return _isStatisticDisabled.Value;
            }
        }

        public readonly MainLogicProvider MainLogicProvider;
        public SessionModule SessionModule { get; }

        public bool IsAuthenticated() {
            return SessionModule.IsAuthenticated();
        }

        public string AccountEmail { get; }

        public BaseModel(BaseModel baseModel) {
            MainLogicProvider = baseModel.MainLogicProvider;
            SessionModule = baseModel.SessionModule;
            AccountEmail = baseModel.AccountEmail;
        }

        public BaseModel(SessionModule session, MainLogicProvider mainLogicProvider) {
            MainLogicProvider = mainLogicProvider;
            SessionModule = session;
            if (SessionModule.IsAuthenticated()) {
                var accountDetails = MainLogicProvider.AccountProvider.GetAccountDescription(SessionModule.AccountID);
                AccountEmail = accountDetails.Email;
            }
        }
    }
}
