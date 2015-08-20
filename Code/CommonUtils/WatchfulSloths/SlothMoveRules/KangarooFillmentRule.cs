using System;
using System.Collections.Generic;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class KangarooFillmentRule : SlothMoveByTime<bool> {
        private static List<Action> _actions = new List<Action>(); 

        static KangarooFillmentRule() {
            new KangarooFillmentRule();
        }
        
        public static void AddUpdateAction(Action act) {
            lock (_actions) {
                _actions.Add(act);
            }
        }

        public KangarooFillmentRule() : base(RunAllUpdateActions, new TimeSpan(0, 0, 0, 1), RunAllUpdateActions()) {
            WatchfulSloth.Instance.SetMove(this);
        }

        private static bool RunAllUpdateActions() {
            var newUpdateActions = new List<Action>();
            List<Action> oldUpdateActions;
            lock (_actions) {
                oldUpdateActions = _actions;
                _actions = newUpdateActions;
            }
            foreach (var oldUpdateAction in oldUpdateActions) {
                oldUpdateAction();
//                RunTask(oldUpdateAction);
            }
            return true;
        }

//        private static IEnumerable RunTask(Action act) {
//            act();
//            yield return null;
//        } 
    }
}
