using System;
using System.Web.Mvc;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace Project_B.Controllers {
    public class AssetsController : Controller {
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

        [HttpPost]
        public ActionResult GetStaticPageRaw(int id) {
            return new JsonResult {
                Data = ProjectProvider.Instance.StaticPageProvider.GetStaticPageModel(id) ?? new StaticPageTransport {
                    Languagetype = LanguageType.English
                }
            };
        }

        [HttpPost]
        public ActionResult SaveStaticPageRaw(StaticPageTransport data) {
            return new JsonResult {
                Data = ProjectProvider.Instance.StaticPageProvider.SaveStaticPageModel(data)
            };
        }
    }
}