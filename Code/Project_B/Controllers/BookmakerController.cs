using System.Net;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class BookmakerController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.Bookmakers;

        // GET: Bookmaker
        [ActionLog(ProjectBActions.PageBookmakerPage)]
        [ActionProfile(ProjectBActions.PageBookmakerPage)]
        public ActionResult Index(string id) {
            if (id.IsNullOrEmpty()) {
                return View(new StaticPageBaseModel(this));
            }
            var brokerPageModel = new BrokerPageModel(id, this);
            if (brokerPageModel.BrokerPageTransport == null) {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return new EmptyResult();
            }
            return View("BookmakerPage", brokerPageModel);
        }
    }
}