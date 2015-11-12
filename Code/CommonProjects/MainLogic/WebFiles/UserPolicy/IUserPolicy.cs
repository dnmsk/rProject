namespace MainLogic.WebFiles.UserPolicy {
    public interface IUserPolicy {
        System.Enum PolicySection { get; }
        object GetUserPolicyObj(int userID);
    }

    public interface IUserPolicy<T> : IUserPolicy {
        T GetUserPolicy(int userID);
    }
}
