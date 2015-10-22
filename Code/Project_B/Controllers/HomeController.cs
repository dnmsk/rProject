using System.Web.Mvc;
using MainLogic.WebFiles;

namespace Project_B.Controllers {
    public class HomeController : ApplicationControllerBase {
        public ActionResult Index() {
            return View(GetBaseModel());
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View(GetBaseModel());
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View(GetBaseModel());
        }
    }
}