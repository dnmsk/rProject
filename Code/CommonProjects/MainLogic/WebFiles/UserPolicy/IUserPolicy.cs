namespace MainLogic.WebFiles.UserPolicy {
    public interface IUserPolicy {
        System.Enum PolicySection { get; }
        object GetUserPolicyObj(SessionModule userID, MainLogicProvider mainLogicProvider);
    }

    public interface IUserPolicy<T> : IUserPolicy {
        T GetUserPolicy(SessionModule userID, MainLogicProvider mainLogicProvider);
    }
}
