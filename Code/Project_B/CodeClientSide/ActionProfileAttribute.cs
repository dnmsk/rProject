using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;

namespace Project_B.CodeClientSide {
    public class ActionProfileAttribute : ActionFilterAttribute {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("WebActionProfile");

        public ProjectBActions ActionToLog { get; }
        
        private static ConcurrentBag<InfoObject> _bag = new ConcurrentBag<InfoObject>();

        static ActionProfileAttribute() {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(SaveBagData, TimeSpan.FromMinutes(1), null));
        }

        private static object SaveBagData() {
            var oldData = _bag;
            _bag = new ConcurrentBag<InfoObject>();
            if (oldData.Any()) { 
                _logger.Info(Environment.NewLine + 
                    oldData.GroupBy(old => old.Action)
                            .Select(grouped => {
                                var count = grouped.Count();
                                return string.Format("{0};{1};{2};{3};{4};{5};",
                                    grouped.Key,
                                    count,
                                    grouped.Sum(d => d.ActionExecuted) / count,
                                    grouped.Max(d => d.ActionExecuted),
                                    grouped.Sum(d => d.ResultExecuted) / count,
                                    grouped.Max(d => d.ResultExecuted));
                            })
                            .StrJoin(Environment.NewLine));
            }
            return null;
        }
        
        public ActionProfileAttribute(ProjectBActions actionToLog) {
            ActionToLog = actionToLog;
        }

        const string profile = "profile";
        const string stopwatch = "stopwatch";
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var infoObject = new InfoObject {
                Action = ActionToLog
            };
            filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[profile] = infoObject;
            filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[stopwatch] = Stopwatch.StartNew();
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            ((InfoObject)filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[profile]).ActionExecuted = 
                (int)((Stopwatch)filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[stopwatch]).ElapsedMilliseconds;
            base.OnActionExecuted(filterContext);
        }
        
        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            base.OnResultExecuted(filterContext);
            var infoObject = (InfoObject)filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[profile];
            infoObject.ResultExecuted = (int)((Stopwatch)filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[stopwatch]).ElapsedMilliseconds;
            _bag.Add(infoObject);
        }

        private class InfoObject {
            public ProjectBActions Action { get; set; }
            public int ActionExecuted { get; set; }
            public int ResultExecuted { get; set; }
        }
    }
}