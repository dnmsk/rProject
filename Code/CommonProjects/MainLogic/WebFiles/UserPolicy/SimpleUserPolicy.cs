using System;

namespace MainLogic.WebFiles.UserPolicy {
    internal class SimpleUserPolicy<T> : IUserPolicy<T> {
        private readonly Func<SessionModule, T> _policyValueGetter;
        public System.Enum PolicySection { get; }
        public object GetUserPolicyObj(SessionModule userID) {
            return GetUserPolicy(userID);
        }

        internal SimpleUserPolicy(System.Enum policyName, Func<SessionModule, T> policyValueGetter) {
            _policyValueGetter = policyValueGetter;
            PolicySection = policyName;
        }

        public T GetUserPolicy(SessionModule userID) {
            return _policyValueGetter(userID);
        }
    }
}
