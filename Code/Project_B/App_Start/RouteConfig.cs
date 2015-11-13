using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles;
using Project_B.CodeClientSide.Routes;

namespace Project_B {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new LowercaseRoute(
                url: "Assets/{action}/{id}",
                defaults: new RouteValueDictionary(new { controller = "Assets", action = "Index", language = "", id = UrlParameter.Optional }),
                routeHandler:new MvcRouteHandler()));
            /*
            routes.MapRoute(
                name: "Style",
                url: "Assets/{action}/{id}",
                defaults: new { controller = "Assets", action = "Index", language = "", id = UrlParameter.Optional }
            );
            *//*
            routes.MapRoute(
                name: "Error",
                url: "Error/{action}",
                defaults: new { controller = "Error", action = UrlParameter.Optional, language = "" }
            );
            */
            var langs = new[] {"En", "Ru"};
            var valuesConstraint = new ExpectedValuesConstraint(langs);
            /*
            routes.MapRoute(
                name: "Default",
                url: "{language}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { language = valuesConstraint }
            );
            */
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
