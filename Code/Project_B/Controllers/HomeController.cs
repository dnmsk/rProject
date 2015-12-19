using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HomeController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.Home;

        [ActionLog(ProjectBActions.PageHomeIndex)]
        [ActionProfile(ProjectBActions.PageHomeIndex)]
        public ActionResult Index() {
            var model = new StaticPageBaseModel(this);
            return new ActionResultCached(
                true,
                () => model.StaticPageTransport.LastModifyDateUtc,
                () => View(model));
        }

        [ActionLog(ProjectBActions.PageHomeAbout)]
        [ActionProfile(ProjectBActions.PageHomeAbout)]
        public ActionResult About() {
            return View(new StaticPageBaseModel(this));
        }

        [ActionLog(ProjectBActions.PageHomeContact)]
        [ActionProfile(ProjectBActions.PageHomeContact)]
        public ActionResult Contact() {
            return View(new StaticPageBaseModel(this));
        }

        public ActionResult DetectLanguage() {
            CultureInfo cul;
            try {
                cul = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
            } catch {
                cul = CultureInfo.CreateSpecificCulture("en-US");
            }
            var param = "En";
            switch (cul.TwoLetterISOLanguageName.ToLower()) {
                case "ru":
                    param = "Ru";
                    break;
            }
            var routeValueDict = new RouteValueDictionary {
                {"language", param },
                {"id", RouteData.Values["any3"] as string}
            };
            Request.Url
                ?.Query
                .Split(new [] {"?", "&"}, StringSplitOptions.RemoveEmptyEntries)
                .Each(kv => {
                    var splitted = kv.Split('=');
                    routeValueDict[splitted[0]] = splitted[1];
                });
            return RedirectToAction(
                RouteData.Values["any2"] as string ?? "Index", 
                RouteData.Values["any1"] as string ?? "Home", 
                routeValueDict);
        }
    }
}