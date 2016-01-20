using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.DataProvider.DataHelper;

namespace Project_B.Areas.Moderate {
    public class ModerateAreaRegistration : AreaRegistration {
        public override string AreaName => "Moderate";

        public override void RegisterArea(AreaRegistrationContext context) {
            var valuesConstraint = new ExpectedValuesConstraint(LanguageTypeHelper.Instance.GetIsoNames());
            context.MapRoute(
                url: "{language}/Moderate/{controller}/{action}/{id}",
                defaults: new RouteValueDictionary(new { controller = "Moderate", action = "Index", id = UrlParameter.Optional }),
                constraints: new RouteValueDictionary(new { language = valuesConstraint }),
                name: "Moderate_default",
                namespaces: new [] { "Project_B.Areas.Moderate.Controllers" });
        }
    }
}