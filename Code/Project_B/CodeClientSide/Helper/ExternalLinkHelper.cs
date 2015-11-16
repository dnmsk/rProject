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
    public class ExternalLinkHelper : Singleton<ExternalLinkHelper> {
        const int _shift = 100000;
        private const string _externalLinkBeginText = "{externalid:";
        private const string _externalLinkEndText = "}";
        private readonly MultipleKangooCache<short, string> _externalLinkCache = new MultipleKangooCache<short, string>(MainLogicProvider.WatchfulSloth,
            dictionary => {
                foreach (var externalLink in ExternalLink.DataSource.AsList(ExternalLink.Fields.Link)) {
                    dictionary[externalLink.ID] = externalLink.Link;
                }
            }, TimeSpan.FromHours(6));
        
        private static string GetRedirectID(int guestID, short linkID) {
            return ApplicationControllerBase.CryptoManager.EncryptString((guestID * _shift + linkID).ToString());
        }

        public short GetRedirectID(int guestID, string linkID) {
            long intId;
            if (UserAgentValidationPolicy.BOT_GUID != guestID && long.TryParse(ApplicationControllerBase.CryptoManager.DecryptString(linkID), out intId) && intId != default(int)) {
                return (short)(intId % (guestID * _shift));
            }
            return default(short);
        }
        public string GetExternalLink(short linkID) {
            string externalLink;
            if (_externalLinkCache.TryGetValue(linkID, out externalLink)) {
                SlothMovePlodding.AddAction(() => {
                    ExternalLink.DataSource
                        .WhereEquals(ExternalLink.Fields.ID, linkID)
                        .Update(ExternalLink.Fields.Followcount, new DbFnSimpleOp(ExternalLink.Fields.Followcount, FnMathOper.Add, 1));
                });
            }
            return externalLink;
        }
        
        public void SetExternalLink(short linkID, string linkTarget) {
            _externalLinkCache[linkID] = linkTarget;
        }

        public string CreateHrefToRedirect(UrlHelper urlHelper, BaseModel baseModel, short linkID) {
            return urlHelper.Action("Index", "Redirect", new { id = GetRedirectID(baseModel.SessionModule.GuestID, linkID) }) + "\" rel=\"nofollow";
        }

        public string ProcessStaticContentForExternalLink(UrlHelper urlHelper, BaseModel baseModel, string text) {
            text = text ?? string.Empty;
            var startProcessIndex = 0;
            int startLinkPos;
            while ((startLinkPos = text.IndexOf(_externalLinkBeginText, startProcessIndex, StringComparison.InvariantCultureIgnoreCase)) != -1) {
                startProcessIndex = startLinkPos;
                var endLinkPos = text.IndexOf(_externalLinkEndText, startProcessIndex, StringComparison.InvariantCultureIgnoreCase);
                if (endLinkPos == -1) {
                    return text;
                }
                var startIndex = startLinkPos + _externalLinkBeginText.Length;
                var textID = text.Substring(startIndex, endLinkPos - startIndex);
                short intID;
                if (short.TryParse(textID, out intID)) {
                    var link = CreateHrefToRedirect(urlHelper, baseModel, intID);
                    if (!link.IsNullOrEmpty()) {
                        text = text.Substring(0, startLinkPos) + link + text.Substring(endLinkPos + _externalLinkEndText.Length);
                    }
                }
            }
            return text;
        }
    }
}