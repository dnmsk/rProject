using System;
using System.Collections.Generic;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class SlothMovePlodding : SlothMoveByTime<bool> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(SlothMovePlodding).FullName);
        private static List<Action> _actionsToDo = new List<Action>();
        private static SlothMovePlodding _selfInstance;

        static SlothMovePlodding() {
            _selfInstance = new SlothMovePlodding();
        }

        public SlothMovePlodding() : base(RunAllUpdateActions, TimeSpan.FromMilliseconds(100), RunAllUpdateActions()) {
            WatchfulSloth.Instance.SetMove(this);
        }

        public static void AddAction(Action act) {
            if (ConfigHelper.TestMode) {
                act();
            } else {
                lock (_actionsToDo) {
                    _actionsToDo.Add(act);
                }
            }
        }

        private static bool RunAllUpdateActions() {
            var newUpdateActions = new List<Action>();
            List<Action> actionsToDo;
            lock (_actionsToDo) {
                actionsToDo = _actionsToDo;
                _actionsToDo = newUpdateActions;
            }
            foreach (var todoAction in actionsToDo) {
                try {
                    todoAction();
                } catch (Exception ex) {
                    _logger.Error(ex);
                }
                //                RunTask(oldUpdateAction);
            }
            return true;
        }
    }
}
