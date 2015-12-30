using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.Core.Logger;
using MainLogic.WebFiles.Policy;

namespace Project_B.CodeClientSide {
    public class ActionLogAttribute : ActionFilterAttribute {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (ActionLogAttribute).FullName);

        private static readonly ConcurrentDictionary<string, ProjectBActions> _cachePreviousPageActionIds = new ConcurrentDictionary<string, ProjectBActions>();
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
                try {
                    var factory = ControllerBuilder.Current.GetControllerFactory();
                    var controller = factory.CreateController(requestContext, controllerName);
                    return GetPageActionId(key, controller, actionName);
                } catch (Exception ex) {
                    _logger.Error(ex);
                    value = ProjectBActions.Undefined;
                    _cachePreviousPageActionIds[key] = value;
                }
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
            var methodInfo = controller.GetType()
                    .GetMethods()
                    .FirstOrDefault(m => m.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));
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