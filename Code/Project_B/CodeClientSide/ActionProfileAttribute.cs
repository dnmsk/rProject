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
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(SaveBagData, TimeSpan.FromHours(1), null));
        }

        private static object SaveBagData() {
            var oldData = _bag;
            _bag = new ConcurrentBag<InfoObject>();
            if (oldData.Any()) { 
                _logger.Info(Environment.NewLine + 
                    oldData.GroupBy(old => old.Action)
                            .Select(grouped => string.Format("{0};{1};{2};{3};{4};{5};", 
                                                            grouped.Key, 
                                                            grouped.Count(), 
                                                            (int) grouped.Average(d => d.ActionExecuted),
                                                            grouped.MaxOrDefault(d => d.ActionExecuted, default(int)),
                                                            (int) grouped.Average(d => d.ResultExecuted),
                                                            grouped.MaxOrDefault(d => d.ResultExecuted, default(int))
                            )).StrJoin(Environment.NewLine));
            }
            return null;
        }

        private readonly InfoObject _infoObject;
        private Stopwatch _stopwatch;
        public ActionProfileAttribute(ProjectBActions actionToLog) {
            ActionToLog = actionToLog;
            _infoObject = new InfoObject {
                Action = ActionToLog
            };
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            _stopwatch = Stopwatch.StartNew();
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            _infoObject.ActionExecuted = (int) _stopwatch.ElapsedMilliseconds;
            base.OnActionExecuted(filterContext);
        }
        
        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            base.OnResultExecuted(filterContext);
            _infoObject.ResultExecuted = (int)_stopwatch.ElapsedMilliseconds;
            _bag.Add(_infoObject);
        }

        private class InfoObject {
            public ProjectBActions Action { get; set; }
            public int ActionExecuted { get; set; }
            public int ResultExecuted { get; set; }
        }
    }
}