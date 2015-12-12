using System;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using MainLogic;
using MainLogic.Code;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Entity;

namespace Project_B.CodeClientSide.Helper {
    public class RedirectLinkHelper : Singleton<RedirectLinkHelper> {
        private const int _shift = 100000;
        private const string _externalLinkBeginText = "{externalid:";
        private const string _internalLinkBeginText = "{internalid:";
        private const string _linkEndText = "}";
        private readonly MultipleKangooCache<short, string> _externalLinkCache = new MultipleKangooCache<short, string>(MainLogicProvider.WatchfulSloth,
            dictionary => {
                foreach (var externalLink in SiteExternalLink.DataSource.AsList(SiteExternalLink.Fields.Link)) {
                    dictionary[externalLink.ID] = externalLink.Link;
                }
            }, TimeSpan.FromHours(6));
        private readonly MultipleKangooCache<short, string> _internalLinkCache = new MultipleKangooCache<short, string>(MainLogicProvider.WatchfulSloth,
            dictionary => {
                foreach (var externalLink in SiteInternalLink.DataSource.AsList(SiteInternalLink.Fields.Link)) {
                    dictionary[externalLink.ID] = externalLink.Link;
                }
            }, TimeSpan.FromHours(6));

        public short GetRedirectID(int guestID, string linkID) {
            long intId;
            if (UserAgentValidationPolicy.BOT_GUID != guestID && long.TryParse(ApplicationControllerBase.CryptoManager.DecryptString(linkID), out intId) && intId != default(int)) {
                return (short)(intId % _shift);
            }
            return default(short);
        }

        public int GetRedirectDecryptedID(int guestID, string linkID) {
            long intId;
            if (UserAgentValidationPolicy.BOT_GUID != guestID && long.TryParse(ApplicationControllerBase.CryptoManager.DecryptString(linkID), out intId) && intId != default(int)) {
                return (int) intId;
            }
            return default(int);
        }

        public string GetExternalLink(short linkID) {
            string externalLink;
            if (_externalLinkCache.TryGetValue(linkID, out externalLink)) {
                SlothMovePlodding.AddAction(() => {
                    SiteExternalLink.DataSource
                        .WhereEquals(SiteExternalLink.Fields.ID, linkID)
                        .Update(SiteExternalLink.Fields.Followcount, new DbFnSimpleOp(SiteExternalLink.Fields.Followcount, FnMathOper.Add, 1));
                });
            }
            return externalLink;
        }
        
        public string GetInternalLink(short linkID) {
            string internalLink;
            if (_internalLinkCache.TryGetValue(linkID, out internalLink)) {
                SlothMovePlodding.AddAction(() => {
                    SiteInternalLink.DataSource
                        .WhereEquals(SiteInternalLink.Fields.ID, linkID)
                        .Update(SiteInternalLink.Fields.Followcount, new DbFnSimpleOp(SiteInternalLink.Fields.Followcount, FnMathOper.Add, 1));
                });
            }
            return internalLink;
        }
        
        public void SetExternalLink(short linkID, string linkTarget) {
            _externalLinkCache[linkID] = linkTarget;
        }
        
        public void SetInternalLink(short linkID, string linkTarget) {
            _internalLinkCache[linkID] = linkTarget;
        }

        public static string CreateHrefToExternalRedirect(UrlHelper urlHelper, int guestID, short linkID) {
            return urlHelper.Action("External", "Redirect", new { id = GetRedirectID(guestID, linkID) }) + "\" rel=\"nofollow";
        }

        public static string CreateHrefToInternalRedirect(UrlHelper urlHelper, int guestID, short linkID) {
            return urlHelper.Action("Internal", "Redirect", new { id = GetRedirectID(guestID, linkID) }) + "\" rel=\"nofollow";
        }

        public string ProcessStaticContentForLinks(UrlHelper urlHelper, BaseModel baseModel, string text) {
            text = text ?? string.Empty;
            var guestID = baseModel.SessionModule.GuestID;
            text = ProcessTextToRedirectLinks(text, _externalLinkBeginText, linkID => CreateHrefToExternalRedirect(urlHelper, guestID, linkID));
            text = ProcessTextToRedirectLinks(text, _internalLinkBeginText, linkID => CreateHrefToInternalRedirect(urlHelper, guestID, linkID));
            return text;
        }

        private static string GetRedirectID(int guestID, short linkID) {
            return ApplicationControllerBase.CryptoManager.EncryptString((guestID * _shift + linkID).ToString());
        }

        private static string ProcessTextToRedirectLinks(string text, string beginLinkID, Func<short, string> linkIdCreator) {
            var startProcessIndex = 0;
            int startLinkPos;
            while ((startLinkPos = text.IndexOf(beginLinkID, startProcessIndex, StringComparison.InvariantCultureIgnoreCase)) != -1) {
                startProcessIndex = startLinkPos;
                var endLinkPos = text.IndexOf(_linkEndText, startProcessIndex, StringComparison.InvariantCultureIgnoreCase);
                if (endLinkPos == -1) {
                    return text;
                }
                var startIndex = startLinkPos + beginLinkID.Length;
                var textID = text.Substring(startIndex, endLinkPos - startIndex);
                short intID;
                if (short.TryParse(textID, out intID)) {
                    var link = linkIdCreator(intID);
                    if (!link.IsNullOrEmpty()) {
                        text = text.Substring(0, startLinkPos) + link + text.Substring(endLinkPos + _linkEndText.Length);
                    }
                }
            }
            return text;
        }
    }
}