using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic.WebFiles.UserPolicy;
using MainLogic.WebFiles.UserPolicy.Enum;

namespace MainLogic.WebFiles {
    public class BaseModel {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BaseModel).FullName);
        private static readonly Dictionary<Enum, IUserPolicy> _userPolicyStore = new Dictionary<Enum, IUserPolicy>();
        
        static BaseModel() {
            ConfigurePolicyStore(UserPolicyGlobal.IsAuthenticated, userID => userID != default(int));
            ConfigurePolicyStore(UserPolicyGlobal.IsStatisticsDisabled, userID => {
                var configurationProperty = SiteConfiguration.GetConfigurationProperty<int[]>("AccountIDsDisabledStatistic");
                return configurationProperty == null || configurationProperty.Contains(userID);
            });
        }

        public static void ConfigurePolicyStore(IUserPolicy userPolicy) {
            if (_userPolicyStore.ContainsKey(userPolicy.PolicySection)) {
                _logger.Error("Policy store already contains key " + userPolicy.PolicySection);
            }
            _userPolicyStore[userPolicy.PolicySection] = userPolicy;
        }

        public static void ConfigurePolicyStore<T>(Enum policyName, Func<int, T> policyValueGetter) {
            ConfigurePolicyStore(new SimpleUserPolicy<T>(policyName, policyValueGetter));
        }

        public readonly MainLogicProvider MainLogicProvider;
        public SessionModule SessionModule { get; }

        private BaseModel() {
            _cachedPolicyData = new SimpleKangooCache<Enum, object>(null,
                key => {
                    IUserPolicy obj;
                    if (_userPolicyStore.TryGetValue(key, out obj) && obj != null) {
                        return obj.GetUserPolicyObj(SessionModule.AccountID);
                    } 
                    _logger.Error("Policy for {0} {1} not found in store", key.GetType(), key);
                    return null;
                }); 
        }

        private readonly SimpleKangooCache<Enum, object> _cachedPolicyData; 
        public T GetUserPolicyState<T>(Enum policyName) {
            try {
                var result = _cachedPolicyData[policyName];
                if (result == null) {
                    _logger.Error("Policy for {0} {1} return null", policyName.GetType(), policyName);
                }
                return (T) result;
            } catch (Exception ex) {
                _logger.Error(ex);
            }
            return default(T);
        }

        public bool IsAuthenticated() {
            return GetUserPolicyState<bool>(UserPolicyGlobal.IsAuthenticated);
        }

        public string AccountEmail { get; }

        public BaseModel(BaseModel baseModel) : this() {
            MainLogicProvider = baseModel.MainLogicProvider;
            SessionModule = baseModel.SessionModule;
            AccountEmail = baseModel.AccountEmail;
        }

        public BaseModel(SessionModule session, MainLogicProvider mainLogicProvider) : this() {
            MainLogicProvider = mainLogicProvider;
            SessionModule = session;
            if (SessionModule.IsAuthenticated()) {
                var accountDetails = MainLogicProvider.AccountProvider.GetAccountDescription(SessionModule.AccountID);
                AccountEmail = accountDetails.Email;
            }
        }
    }
}
