using System.Web.Mvc;
using Project_B.CodeClientSide;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace Project_B.Controllers {
    public class AssetsController : ProjectControllerBase {
        protected override bool EnableStoreRequestData => false;

        [ActionProfile(ProjectBActions.PageAssetsJs)]
        public ActionResult Js(string id) {
            return Content(GetContent(Bundle.JavaScript(), id), "text/javascript");
        }

        [ActionProfile(ProjectBActions.PageAssetsCss)]
        public ActionResult Css(string id) {
            return Content(GetContent(Bundle.Css(), id), "text/css");
        }
        private string GetContent(IRenderable bundle, string id) {
            FileController.MarkResponseAsCached(Response);
            return bundle.RenderCached(id.ToLowerInvariant());
        }
    }
}