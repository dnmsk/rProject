using System.Linq;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.Models;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace Project_B.Controllers {
    public class AssetsController : ProjectControllerBase {

        public ActionResult Js(string id) {
            return Content(GetContent(Bundle.JavaScript(), id), "text/javascript");
        }

        public ActionResult Css(string id) {
            return Content(GetContent(Bundle.Css(), id), "text/css");
        }

        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
        public ActionResult Index() {
            return View();
        }

        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
        public ActionResult Images(int id = 0) {
            return View();
        }

        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
        [HttpGet]
        [HttpPost]
        public ActionResult Image(int id) {
            switch (Request.RequestType.ToUpper()) {
                case "POST":
                    id = ImgController.UploadImageFromRequest(Request, 1).First();
                    break;
                case "GET":
                default:
                    break;
            }
            return View();
        }
        
        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
        public ActionResult PagesForType(ProjectBActions id) {
            var staticPageBaseModel = new StaticPageBaseModel(GetBaseModel()) {PageKey = id};
            return PartialView("PageEditor/PagesForType", staticPageBaseModel);
        }

        [ValidateInput(false)]
        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
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

        private string GetContent(IRenderable bundle, string id) {
            ImgController.MarkResponseAsCached(Response);
            return bundle.RenderCached(id.ToLowerInvariant());
        }
    }
}