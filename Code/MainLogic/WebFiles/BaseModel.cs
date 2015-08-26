namespace MainLogic.WebFiles {
    public class BaseModel {
        public readonly MainLogicProvider MainLogicProvider;
        public SessionModule SessionModule { get; private set; }

        public bool IsAthenticated() {
            return SessionModule.IsAthenticated();
        }

        public string Email { get; private set; }

        public BaseModel(SessionModule session, MainLogicProvider mainLogicProvider) {
            MainLogicProvider = mainLogicProvider;
            SessionModule = session;
            if (SessionModule.IsAthenticated()) {
                var accountDetails = MainLogicProvider.AccountProvider.GetAccountDescription(SessionModule.AccountID);
                Email = accountDetails.Email;
            }
        }
    }
}
