using System.Web.Mvc;

namespace Project_Tracker.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }
    }
}