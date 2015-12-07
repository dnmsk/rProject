using System.Collections.Generic;
using System.Web.Mvc;

namespace MainLogic.WebFiles {
    public class NotModifiedResult : ActionResult {
        public override void ExecuteResult(ControllerContext context) {
            var response = context.HttpContext.Response;
            response.StatusCode = 304;
            response.StatusDescription = "Not Modified";
            response.SuppressContent = true;
        }
    }
}
