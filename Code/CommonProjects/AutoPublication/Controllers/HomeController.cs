using System.Web.Mvc;
using AutoPublication.Code;
using AutoPublication.Models;

namespace AutoPublication.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View(new PublicationModel {
                BuildPublishItems = BuildPublishProvider.Instance.GetPublishItems(),
                ZipBuildItems = TeamcityProvider.Instance.GetBuilds()
            });
        }

        public ActionResult UpdateBuildList() {
            TeamcityProvider.Instance.UpdateBuildCache();
            return RedirectToAction("Index");
        }

        public ActionResult AddPublicationPath(BuildPublishItem publishItem) {
            BuildPublishProvider.Instance.AddPublishPath(publishItem);
            return RedirectToAction("Index");
        }

        public ActionResult PublishBuild(BuildPublishItem publishItem, string pathToBuild) {
            BuildPublishProvider.Instance.PublishBuild(publishItem, pathToBuild);
            return RedirectToAction("Index");
        }

        public ActionResult RemovePublishPath(BuildPublishItem publishItem) {
            BuildPublishProvider.Instance.RemovePublishPath(publishItem);
            return RedirectToAction("Index");
        }
    }
}