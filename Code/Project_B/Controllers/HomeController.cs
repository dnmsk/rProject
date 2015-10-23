using System.Globalization;
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

        public ActionResult DetectLanguage() {
            CultureInfo cul;
            try {
                cul = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
            }
            catch {
                cul = CultureInfo.CreateSpecificCulture("en-US");
            }
            var param = "En";
            switch (cul.TwoLetterISOLanguageName.ToLower()) {
                case "ru":
                    param = "Ru";
                    break;
            }
            return RedirectToAction("Index", new {language = param});
        }
    }
}