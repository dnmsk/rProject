using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using MainLogic.Code;
using MainLogic.Entities;
using MainLogic.Transport;
using MainLogic.WebFiles;
using MainLogic.Wrapper;

namespace MainLogic.Providers {
    public interface IUserProvider {
    }

    public class UserProvider : SafeInvokerBase, IUserProvider {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(UserProvider).FullName);

        public UserProvider() : base(_logger) { }

        public int CreateNewGuid(string ip, string userAgent) {
            return InvokeSafe(() => {
                if (UserAgentValidationPolicy.IsBot(userAgent)) {
                    return UserAgentValidationPolicy.BOT_GUID;
                }
                var guid = new Guest {
                    Datecreated = DateTime.Now,
                    Ip = ip
                };
                guid.Save();
                return guid.ID;
            }, UserAgentValidationPolicy.BOT_GUID);
        }

        public void SaveReferrer(int guestid, string referrer, string target) {
            InvokeSafe(() => {
                if (referrer.Contains(SiteConfiguration.ProductionHostName, StringComparison.InvariantCultureIgnoreCase)) {
                    return;
                }
                new GuestReferrer {
                    Datecreated = DateTime.Now,
                    GuestID = guestid,
                    Urlreferrer = referrer,
                    Urltarget = target
                }.Save();
            });
        }
        public void SaveTechInfo(int guestid, GuestTechInfoTransport techInfo) {
            InvokeSafeSingleCall(() => {
                var isbot = guestid == UserAgentValidationPolicy.BOT_GUID;
                var lowerUserAgent = techInfo.UserAgent.ToLower();
                var guestExistsBrowser = GuestExistsBrowser.DataSource
                    .WhereEquals(GuestExistsBrowser.Fields.Useragent, lowerUserAgent)
                    .First(GuestExistsBrowser.Fields.ID);
                var now = DateTime.Now;
                if (guestExistsBrowser == null) {
                    guestExistsBrowser = new GuestExistsBrowser {
                        Datecreated = now,
                        Browsertype = techInfo.BrowserType,
                        Isbot = isbot,
                        Ismobile = techInfo.IsMobile,
                        Os = techInfo.Os,
                        Useragent = lowerUserAgent,
                        Version = techInfo.Version
                    };
                    guestExistsBrowser.Save();
                }
                new GuestTechInfo {
                    Datecreated = now,
                    GuestexistsbrowserID = guestExistsBrowser.ID,
                    GuestID = guestid
                }.Save();
                return (object)null;
            }, null);
        }
        public void SaveUtm(int guestid, UtmParamWrapper utmParams) {
            InvokeSafe(() => {
                if (utmParams.Any()) {
                    new UtmGuestReferrer {
                        Datecreated = DateTime.Now,
                        GuestID = guestid,
                        Campaign = utmParams.UtmCampaign,
                        Medium = utmParams.UtmMedium,
                        Source = utmParams.UtmSource
                    }.Save();
                }
            });
        }
        public void SaveGuestAction(int guestid, int subdomainRuleID, int action, int arg) {
            InvokeSafe(() => {
                new GuestActionLog {
                    Datecreated = DateTime.Now,
                    Action = action,
                    Arg = arg,
                    GuestID = guestid,
                    UtmsubdomainruleID = subdomainRuleID
                }.Save();
            });
        }

        public UtmSubdomainRuleTransport[] GetSubdomainRules() {
            return InvokeSafe(() => {
                return UtmSubdomainRule.DataSource
                    .AsList(
                        UtmSubdomainRule.Fields.ID,
                        UtmSubdomainRule.Fields.Subdomainname,
                        UtmSubdomainRule.Fields.Targetdomain
                    )
                    .Select(usr => new UtmSubdomainRuleTransport {
                        ID = usr.ID,
                        SubdomainName = usr.Subdomainname,
                        TargetDomain = usr.Targetdomain
                    })
                    .ToArray();
            }, null);
        }
    }
}
