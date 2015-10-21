using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace Project_B.Controllers {
    public class AssetsController : Controller {
        private readonly TimeSpan _fromDays = TimeSpan.FromDays(365);
        public readonly bool IsDebug = ((CompilationSection)ConfigurationManager.GetSection("system.web/compilation")).Debug;

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
    }
}