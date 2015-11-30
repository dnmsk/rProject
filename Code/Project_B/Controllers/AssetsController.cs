using System.Dynamic;
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
            return View(new StaticPageBaseModel(GetBaseModel()));
        }

        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
        public ActionResult Files(short startID = 0, int limit = 12) {
            var totalFilesCount = ProjectProvider.Instance.WebFileProvider.GetTotalFilesCount();
            startID = startID == default(short) ? totalFilesCount : startID;
            var fileInfos = ProjectProvider.Instance.WebFileProvider.GetFileInfos(startID, limit);
            var pages = (totalFilesCount / limit) + (totalFilesCount % limit > 0 ? 1 : 0);
            var m = (dynamic) new ExpandoObject();
            var model = new StaticPageBaseModel<dynamic>(GetBaseModel()) {
                ControllerModel = m
            };
            m.currentPage = pages - (startID/limit);
            m.pages = pages;
            m.currentLimit = limit;
            m.files = fileInfos;
            return View(model);
        }

        [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
        public ActionResult File(short id = default(short)) {
            switch (Request.RequestType.ToUpper()) {
                case "POST":
                    id = id == default(short) 
                        ? FileController.UploadFileFromRequest(Request, 1).First()
                        : FileController.UpdateFileFromRequest(id, Request);
                    break;
                case "GET":
                default:
                    break;
            }
            var fileInfo = ProjectProvider.Instance.WebFileProvider.GetFileInfos(id, 1).First();
            var m = (dynamic)new ExpandoObject();
            var model = new StaticPageBaseModel<dynamic>(GetBaseModel()) {
                ControllerModel = m
            };
            m.file = fileInfo;
            return View(model);
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
            FileController.MarkResponseAsCached(Response);
            return bundle.RenderCached(id.ToLowerInvariant());
        }
    }
}