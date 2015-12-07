using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths;
using CommonUtils.WatchfulSloths.KangooCache;
using IDEV.Hydra.DAO;
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

        public int CreateNewGuest(string ip, string userAgent) {
            return InvokeSafe(() => {
                if (UserAgentValidationPolicy.IsBot(userAgent)) {
                    return UserAgentValidationPolicy.BOT_GUID;
                }
                var guid = new Guest {
                    Datecreated = DateTime.UtcNow,
                    Ip = ip
                };
                guid.Save();
                return guid.ID;
            }, UserAgentValidationPolicy.BOT_GUID);
        }
        
        public bool CheckGuest(int guid) {
            return InvokeSafe(() => Guest.DataSource
                                         .WhereEquals(Guest.Fields.ID, guid)
                                         .IsExists(), false);
        }

        public void SaveReferrer(int guestid, string referrer, string target) {
            InvokeSafe(() => {
                if (!SiteConfiguration.ProductionHostName.IsNullOrWhiteSpace() && (
                        (guestid == UserAgentValidationPolicy.BOT_GUID && referrer.IsNullOrWhiteSpace())
                        || referrer.Contains(SiteConfiguration.ProductionHostName, StringComparison.InvariantCultureIgnoreCase))) {
                    return;
                }
                new GuestReferrer {
                    Datecreated = DateTime.UtcNow,
                    GuestID = guestid,
                    Urlreferrer = referrer,
                    Urltarget = target
                }.Save();
            });
        }

        private readonly KangarooCache<int, int> _cacheBrowserInfosByGuest = new KangarooCache<int, int>(WatchfulSloth.Instance,
            guestID => {
                var techInfo = GuestTechInfo.DataSource
                    .WhereEquals(GuestTechInfo.Fields.GuestID, guestID)
                    .Sort(GuestTechInfo.Fields.Datecreated, SortDirection.Desc)
                    .First(GuestTechInfo.Fields.GuestexistsbrowserID);
                return techInfo != null ? techInfo.GuestexistsbrowserID : default(int);
            }, TimeSpan.FromMinutes(30));

        public void SaveTechInfo(int guestid, GuestTechInfoTransport techInfo) {
            InvokeSafeSingleCall(() => {
                var isbot = guestid == UserAgentValidationPolicy.BOT_GUID;
                var lowerUserAgent = techInfo.UserAgent.ToLower();
                var guestExistsBrowser = GuestExistsBrowser.DataSource
                    .WhereEquals(GuestExistsBrowser.Fields.Useragent, lowerUserAgent)
                    .First(GuestExistsBrowser.Fields.ID);
                var now = DateTime.UtcNow;
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
                if (guestid != UserAgentValidationPolicy.BOT_GUID && _cacheBrowserInfosByGuest[guestid] != guestExistsBrowser.ID) {
                    var newGuestTechInfo = new GuestTechInfo {
                        Datecreated = now,
                        GuestexistsbrowserID = guestExistsBrowser.ID,
                        GuestID = guestid
                    };
                    newGuestTechInfo.Save();
                    _cacheBrowserInfosByGuest[guestid] = newGuestTechInfo.ID;
                }
                return (object)null;
            }, null);
        }
        public void SaveUtm(int guestid, UtmParamWrapper utmParams) {
            InvokeSafe(() => {
                if (guestid != UserAgentValidationPolicy.BOT_GUID || utmParams.Any()) {
                    new UtmGuestReferrer {
                        Datecreated = DateTime.UtcNow,
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
                    Datecreated = DateTime.UtcNow,
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
