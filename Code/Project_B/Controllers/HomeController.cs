using System.Web.Mvc;
using MainLogic.WebFiles;

namespace Project_B.Controllers {
    public class HomeController : ApplicationControllerBase {
        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}