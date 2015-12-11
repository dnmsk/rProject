using System;

namespace MainLogic.WebFiles.UserPolicy {
    internal class SimpleUserPolicy<T> : IUserPolicy<T> {
        private readonly Func<SessionModule, MainLogicProvider, T> _policyValueGetter;
        public System.Enum PolicySection { get; }

        public object GetUserPolicyObj(SessionModule userID, MainLogicProvider mainLogicProvider) {
            return GetUserPolicy(userID, mainLogicProvider);
        }

        internal SimpleUserPolicy(System.Enum policyName, Func<SessionModule, MainLogicProvider, T> policyValueGetter) {
            _policyValueGetter = policyValueGetter;
            PolicySection = policyName;
        }

        public T GetUserPolicy(SessionModule userID, MainLogicProvider mainLogicProvider) {
            return _policyValueGetter(userID, mainLogicProvider);
        }
    }
}
