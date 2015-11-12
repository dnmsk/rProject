using System.Globalization;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HomeController : ProjectControllerBase {
        [ActionLog(ProjectBActions.PageHomeIndex)]
        public ActionResult Index() {
            return View(new StaticPageBaseModel(this));
        }

        [ActionLog(ProjectBActions.PageHomeAbout)]
        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View(new StaticPageBaseModel(this));
        }

        [ActionLog(ProjectBActions.PageHomeContact)]
        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View(new StaticPageBaseModel(this));
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