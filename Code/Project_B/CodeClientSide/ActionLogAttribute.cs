using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles.Policy;

namespace Project_B.CodeClientSide {
    public class ActionLogAttribute : ActionFilterAttribute {
        private static readonly Dictionary<string, int?> _cachePreviousPageActionIds = new Dictionary<string, int?>();
        public Enum ActionToLog { get; }

        public ActionLogAttribute(ProjectBActions actionToLog) {
            ActionToLog = actionToLog;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (ProductionPolicy.IsProduction()) {
                var controller = filterContext.Controller as ProjectControllerBase;
                if (controller != null) {
                    int? arg = null;
                    var lastControllerAction = controller.GetPreviousControllerAction();
                    if (lastControllerAction != null) {
                        arg = GetPreviousPageActionId(filterContext.RequestContext, lastControllerAction);
                    }
                    controller.LogAction(ActionToLog, arg.HasValue ? arg : (short) controller.CurrentLanguage);
                }
            }
            base.OnActionExecuting(filterContext);
        }

        private static int? GetPreviousPageActionId(RequestContext requestContext, Tuple<string, string> controllerAction) {
            var key = string.Format("{0}/{1}", controllerAction.Item1, controllerAction.Item2).ToLowerInvariant();
            int? value;
            if (!_cachePreviousPageActionIds.TryGetValue(key, out value)) {
                var factory = ControllerBuilder.Current.GetControllerFactory();
                var controller = factory.CreateController(requestContext, controllerAction.Item1);
                var methodInfo = controller.GetType().GetMethod(controllerAction.Item2);
                if (methodInfo != null) {
                    var previousAttribute = methodInfo.GetCustomAttribute(typeof(ActionLogAttribute)) as ActionLogAttribute;
                    if (previousAttribute != null) {
                        value = Convert.ToInt16(previousAttribute.ActionToLog);
                    }
                }
                _cachePreviousPageActionIds[key] = value;
            }
            return value;
        }
    }
}