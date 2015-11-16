using System.Net;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Helper;

namespace Project_B.Controllers {
    public class RedirectController : ProjectControllerBase {

        // GET: Redirect
        [ActionLog(ProjectBActions.PageRedirect)]
        public ActionResult Index(string id) {
            var linkID = ExternalLinkHelper.Instance.GetRedirectID(GetBaseModel().SessionModule.GuestID, id);
            LogAction(ProjectBActions.PageRedirectConcrete, linkID);
            if (linkID != default (int)) {
                return new RedirectResult(ExternalLinkHelper.Instance.GetExternalLink(linkID), false);
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return new EmptyResult();
        }
    }
}