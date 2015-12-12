using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles;
using Project_B.CodeClientSide.Routes;
using Project_B.CodeServerSide.DataProvider.DataHelper;

namespace Project_B {
    public static class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new LowercaseRoute(
                url: "Assets/{action}/{id}",
                defaults: new RouteValueDictionary(new { controller = "Assets", action = "Index", id = UrlParameter.Optional }),
                routeHandler: new MvcRouteHandler()));
            routes.Add(new LowercaseRoute(
                url: "File/{id}/{type}/{hash}",
                defaults: new RouteValueDictionary(new { controller = "File", action = "Index" }),
                routeHandler:new MvcRouteHandler()));
            routes.Add(new LowercaseRoute(
                url: "r/e/{id}",
                defaults: new RouteValueDictionary(new { controller = "Redirect", action = "External" }),
                routeHandler:new MvcRouteHandler()));
            routes.Add(new LowercaseRoute(
                url: "r/i/{id}",
                defaults: new RouteValueDictionary(new { controller = "Redirect", action = "Internal" }),
                routeHandler:new MvcRouteHandler()));
            var valuesConstraint = new ExpectedValuesConstraint(LanguageTypeHelper.Instance.GetIsoNames());
            routes.Add(new LowercaseRoute(
                url: "{language}/{controller}/{action}/{id}",
                defaults: new RouteValueDictionary(new { controller = "Home", action = "Index", id = UrlParameter.Optional }),
                routeHandler: new MvcRouteHandler(),
                constraints: new RouteValueDictionary(new { language = valuesConstraint })));
            routes.MapRoute(
                name: "NoLanguage",
                url: "{any1}/{any2}/{any3}/{any4}",
                defaults: new { controller = "home", action = "detectlanguage", any1 = UrlParameter.Optional, any2 = UrlParameter.Optional, any3 = UrlParameter.Optional, any4 = UrlParameter.Optional, }
            );
        }
    }
}
