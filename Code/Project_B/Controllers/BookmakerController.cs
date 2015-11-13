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
        public ActionResult Index(string id) {
            return id.IsNullOrEmpty() 
                ? View(new StaticPageBaseModel(this)) 
                : View("BookmakerPage", new BrokerPageModel(id, this));
        }
    }
}