using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.Models;

namespace Project_B.Controllers {
    public class ErrorController : ProjectControllerBase {
        /// <summary>
        /// Показывает 500 ошибку
        /// </summary>
        public ActionResult Internal() {
            Response.StatusCode = 500;
            return View("Error500");
        }

        /// <summary>
        /// Показывает 404 ошибку
        /// </summary>
        [ActionLog(ProjectBActions.PageErrorNotFound)]
        public ActionResult NotFound() {
            Response.StatusCode = 404;
            return View("Error404", new StaticPageBaseModel(this));
        }
    }
}