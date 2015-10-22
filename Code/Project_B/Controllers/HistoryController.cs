using System.Web.Mvc;
using MainLogic.WebFiles;

namespace Project_B.Controllers {
    public class HistoryController : ApplicationControllerBase {
        // GET: History
        public ActionResult Index() {
            return View();
        }
    }
}