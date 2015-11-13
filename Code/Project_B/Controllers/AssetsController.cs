using System;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.Models;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace Project_B.Controllers {
    public class AssetsController : ProjectControllerBase {
        private readonly TimeSpan _fromDays = TimeSpan.FromDays(365);

        public ActionResult Js(string id) {
            return Content(GetContent(Bundle.JavaScript(), id), "text/javascript");
        }

        public ActionResult Css(string id) {
            return Content(GetContent(Bundle.Css(), id), "text/css");
        }

        private string GetContent<T>(BundleBase<T> bundle, string id) where T : BundleBase<T> {
            // Set max-age to a year from now
            Response.Cache.SetMaxAge(_fromDays);
            return bundle.RenderCached(id);
        }

        public ActionResult PagesForType(ProjectBActions id) {
            var staticPageBaseModel = new StaticPageBaseModel(GetBaseModel()) {PageKey = id};
            return PartialView("PageEditor/PagesForType", staticPageBaseModel);
        }

        [ValidateInput(false)]
        public ActionResult StaticPageEdit(ProjectBActions id, StaticPageTransport staticPageTransport) {
            switch (Request.RequestType.ToUpper()) {
                case "POST":
                    return StaticPageFormatJsonResult(staticPageTransport);
                case "PUT":
                    return StaticPageFormatJsonResult(ProjectProvider.Instance.StaticPageProvider.SaveStaticPageModel(id, staticPageTransport));
                case "GET":
                default:
                    return StaticPageFormatJsonResult(ProjectProvider.Instance.StaticPageProvider.GetStaticPageModel(id, staticPageTransport.ID) ?? staticPageTransport);
            }
        }

        private static JsonResult StaticPageFormatJsonResult(StaticPageTransport pageTransport) {
            return new JsonResult {
                Data = new {
                    StaticPageTransport = pageTransport
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}