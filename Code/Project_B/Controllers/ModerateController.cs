using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.Models;

namespace Project_B.Controllers {
    [CheckCredential(UserPolicyLocal.IsPageEditor, true)]
    public class ModerateController : ProjectControllerBase {
        
        public ActionResult Index() {
            return View(new StaticPageBaseModel(GetBaseModel()));
        }

        public ActionResult SiteText(SiteTextType siteText = SiteTextType.Unknown, string text = null) {

            return View(new StaticPageBaseModel(GetBaseModel()));
        }

        public ActionResult ExternalLinks(int linkId = default(int), string text = null) {

            return View(new StaticPageBaseModel(GetBaseModel()));
        }
        
        public ActionResult Files(short startID = 0, int limit = 12) {
            var totalFilesCount = ProjectProvider.Instance.WebFileProvider.GetTotalFilesCount();
            startID = startID == default(short) ? totalFilesCount : startID;
            var fileInfos = ProjectProvider.Instance.WebFileProvider.GetFileInfos(startID, limit);
            var pages = (totalFilesCount / limit) + (totalFilesCount % limit > 0 ? 1 : 0);
            var m = (dynamic)new ExpandoObject();
            var model = new StaticPageBaseModel<dynamic>(GetBaseModel()) {
                ControllerModel = m
            };
            m.currentPage = pages - (startID / limit);
            m.pages = pages;
            m.currentLimit = limit;
            m.files = fileInfos;
            return View(model);
        }
        
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
        
        public ActionResult PagesForType(ProjectBActions id) {
            var staticPageBaseModel = new StaticPageBaseModel(GetBaseModel()) { PageKey = id };
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