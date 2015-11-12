using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles;

namespace Project_B {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Style",
                url: "Assets/{action}/{id}",
                defaults: new { controller = "Assets", action = "Index", language = "", id = UrlParameter.Optional }
            );/*
            routes.MapRoute(
                name: "Error",
                url: "Error/{action}",
                defaults: new { controller = "Error", action = UrlParameter.Optional, language = "" }
            );
            */
            routes.MapRoute(
                name: "Default",
                url: "{language}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { language = new ExpectedValuesConstraint("En", "Ru") }
            );
            routes.MapRoute(
                name: "NoLanguage",
                url: "{any1}/{any2}/{any3}/{any4}",
                defaults: new { controller = "Home", action = "DetectLanguage", any1 = UrlParameter.Optional, any2 = UrlParameter.Optional, any3 = UrlParameter.Optional, any4 = UrlParameter.Optional, }
            );
        }
    }
}
