using System.Web.Mvc;
using Project_B.CodeClientSide;

namespace Project_B.Controllers {
    public class BookmakerController : ProjectControllerBase {
        // GET: Bookmaker
        [ActionLog(ProjectBActions.PageBookmakerPage)]
        public ActionResult Index(string id) {
            return View();
        }
    }
}