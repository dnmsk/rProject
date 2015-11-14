using System.Globalization;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HomeController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.Home;

        [ActionLog(ProjectBActions.PageHomeIndex)]
        public ActionResult Index() {
            return View(new StaticPageBaseModel(this));
        }

        [ActionLog(ProjectBActions.PageHomeAbout)]
        public ActionResult About() {
            return View(new StaticPageBaseModel(this));
        }

        [ActionLog(ProjectBActions.PageHomeContact)]
        public ActionResult Contact() {
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
            return RedirectToAction(
                RouteData.Values["any2"] as string ?? "Index", 
                RouteData.Values["any1"] as string ?? "Home", 
                new {
                    language = param,
                    id = RouteData.Values["any3"] as string
                });
        }
    }
}