using System.Web.Mvc;
using MainLogic.WebFiles;

namespace Project_B.Controllers {
    public class DefaultController : ApplicationControllerBase {
        // GET: Default
        public ActionResult Index() {
            return View();
        }
    }
}