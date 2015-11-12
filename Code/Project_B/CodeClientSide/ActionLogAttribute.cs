using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles.Policy;

namespace Project_B.CodeClientSide {
    public class ActionLogAttribute : ActionFilterAttribute {
        private static readonly Dictionary<string, ProjectBActions> _cachePreviousPageActionIds = new Dictionary<string, ProjectBActions>();
        public ProjectBActions ActionToLog { get; }

        public ActionLogAttribute(ProjectBActions actionToLog) {
            ActionToLog = actionToLog;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (ProductionPolicy.IsProduction()) {
                var controller = filterContext.Controller as ProjectControllerBase;
                if (controller != null) {
                    var arg = ProjectBActions.Undefined;
                    var lastControllerAction = controller.GetPreviousControllerAction();
                    if (lastControllerAction != null) {
                        arg = GetPageActionId(filterContext.RequestContext, lastControllerAction.Item1, lastControllerAction.Item2);
                    }
                    controller.LogAction(ActionToLog, arg != ProjectBActions.Undefined  ? (int) arg : (short) controller.CurrentLanguage);
                }
            }
            base.OnActionExecuting(filterContext);
        }

        public static ProjectBActions GetPageActionId(RequestContext requestContext, string controllerName, string actionName) {
            var key = string.Format("{0}/{1}", controllerName, actionName).ToLowerInvariant();
            ProjectBActions value;
            if (!_cachePreviousPageActionIds.TryGetValue(key, out value)) {
                var factory = ControllerBuilder.Current.GetControllerFactory();
                var controller = factory.CreateController(requestContext, controllerName);
                return GetPageActionId(key, controller, actionName);
            }
            return value;
        }

        public static ProjectBActions GetPageActionId(IController controller) {
            var projectControllerBase = controller as ProjectControllerBase;
            if (projectControllerBase == null) {
                return ProjectBActions.Undefined;
            }
            var controllerName = projectControllerBase.RouteData.Values["controller"] as string;
            var actionName = projectControllerBase.RouteData.Values["action"] as string;
            var key = string.Format("{0}/{1}", controllerName, actionName).ToLowerInvariant();
            ProjectBActions value;
            if (!_cachePreviousPageActionIds.TryGetValue(key, out value)) {
                value = GetPageActionId(key, controller, actionName);
                _cachePreviousPageActionIds[key] = value;
            }
            return value;
        }

        private static ProjectBActions GetPageActionId(string key, IController controller, string actionName) {
            var value = ProjectBActions.Undefined;
            var methodInfo = controller.GetType().GetMethod(actionName, BindingFlags.IgnoreCase, null, CallingConventions.HasThis, new [] {typeof(string), typeof(int), typeof(void)}, null);
            if (methodInfo != null) {
                var previousAttribute = methodInfo.GetCustomAttribute(typeof (ActionLogAttribute)) as ActionLogAttribute;
                if (previousAttribute != null) {
                    value = previousAttribute.ActionToLog;
                }
            }
            _cachePreviousPageActionIds[key] = value;
            return value;
        }
    }
}