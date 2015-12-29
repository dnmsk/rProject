using System;
using CommonUtils.Core.Config;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.WatchfulThreads;

namespace CommonUtils.WatchfulSloths.SlothMoveRules {
    public class SlothMovePlodding : Singleton<SlothMovePlodding> {
        private readonly WatchfulHolder _watchfulHolder;
        public SlothMovePlodding() {
            _watchfulHolder = new WatchfulHolder(5, "SlothMovePlodding");
        }
        
        public void AddAction(Action action) {
            if (ConfigHelper.TestMode) {
                action();
            } else {
                _watchfulHolder.AddTask(action);
            }
        }
    }
}
