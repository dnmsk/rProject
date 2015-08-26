using System.Web.Mvc;
using MainLogic.WebFiles;

namespace Spywords_Project.Controllers {
    public class HomeController : ApplicationControllerBase {
        public ActionResult Index() {
            return View(GetBaseModel());
        }

        public ActionResult About() {
            return View(GetBaseModel());
        }

        public ActionResult Contact() {
            ViewBag.Message = "Связь с разработчиком.";

            return View(GetBaseModel());
        }
    }
}