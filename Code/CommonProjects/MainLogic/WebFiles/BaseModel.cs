namespace MainLogic.WebFiles {
    public class BaseModel {
        public readonly MainLogicProvider MainLogicProvider;
        public SessionModule SessionModule { get; private set; }

        public bool IsAuthenticated() {
            return SessionModule.IsAuthenticated();
        }

        public string AccountEmail { get; private set; }

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
