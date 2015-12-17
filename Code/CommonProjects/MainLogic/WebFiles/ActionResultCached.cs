using System;
using System.Web;
using System.Web.Mvc;

namespace MainLogic.WebFiles {
    public class ActionResultCached : ActionResult {
        private readonly bool _isFound;
        private readonly Func<DateTime> _getLastModified;
        private readonly Func<ActionResult> _buildActionResult;

        public ActionResultCached(bool isFound, Func<DateTime> getLastModified, Func<ActionResult> buildActionResult) {
            _isFound = isFound;
            _getLastModified = getLastModified;
            _buildActionResult = buildActionResult;
        }

        public override void ExecuteResult(ControllerContext context) {
            var result = !_isFound 
                ? new HttpNotFoundResult() 
                : TryGetNotModifiedResult(context.HttpContext.Request, context.HttpContext.Response, _getLastModified != null ? _getLastModified() : DateTime.MinValue);
            result.ExecuteResult(context);
        }

        private ActionResult TryGetNotModifiedResult(HttpRequestBase request, HttpResponseBase response, DateTime lastModifyByServer) {
            DateTime lastModified;
            if (DateTime.TryParse(request.Headers["If-Modified-Since"], out lastModified)
                    && Math.Abs((lastModifyByServer - lastModified).TotalSeconds) <= 1) {
                return new NotModifiedResult();
            }
            if (lastModifyByServer == DateTime.MinValue) {
                var now = DateTime.UtcNow;
                response.Cache.SetLastModified(now);
                response.Cache.SetExpires(now);
            } else {
                response.Cache.SetLastModified(DateTime.SpecifyKind(lastModifyByServer, DateTimeKind.Utc));
            }
            return _buildActionResult();
        }
    }
}
