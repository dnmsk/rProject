using System.Net;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Helper;

namespace Project_B.Controllers {
    public class RedirectController : ProjectControllerBase {
        
        [ActionLog(ProjectBActions.PageRedirectExternal)]
        public ActionResult External(string l) {
            var guestID = GetBaseModel().SessionModule.GuestID;
            var linkID = RedirectLinkHelper.Instance.GetRedirectID(guestID, l);
            if (linkID != default(int)) {
                LogAction(ProjectBActions.PageRedirectExternalConcrete, RedirectLinkHelper.Instance.GetRedirectDecryptedID(guestID, l));
                return new RedirectResult(RedirectLinkHelper.Instance.GetExternalLink(linkID), false);
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return new EmptyResult();
        }
        
        [ActionLog(ProjectBActions.PageRedirectInternal)]
        public ActionResult Internal(string l) {
            var guestID = GetBaseModel().SessionModule.GuestID;
            var linkID = RedirectLinkHelper.Instance.GetRedirectID(guestID, l);
            if (linkID != default(int)) {
                LogAction(ProjectBActions.PageRedirectInternalConcrete, RedirectLinkHelper.Instance.GetRedirectDecryptedID(guestID, l));
                return new RedirectResult(RedirectLinkHelper.Instance.GetInternalLink(linkID), false);
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return new EmptyResult();
        }
    }
}