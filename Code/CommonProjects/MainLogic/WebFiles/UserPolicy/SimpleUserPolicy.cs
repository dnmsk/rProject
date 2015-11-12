using System;

namespace MainLogic.WebFiles.UserPolicy {
    internal class SimpleUserPolicy<T> : IUserPolicy<T> {
        private readonly Func<int, T> _policyValueGetter;
        public System.Enum PolicySection { get; }
        public object GetUserPolicyObj(int userID) {
            return GetUserPolicy(userID);
        }

        internal SimpleUserPolicy(System.Enum policyName, Func<int, T> policyValueGetter) {
            _policyValueGetter = policyValueGetter;
            PolicySection = policyName;
        }

        public T GetUserPolicy(int userID) {
            return _policyValueGetter(userID);
        }
    }
}
