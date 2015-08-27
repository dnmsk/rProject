using System;
using System.Configuration;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;

namespace Spywords_Project.Code.Algorithms {
    public abstract class AlgoBase {
        protected static SpywordsQueryWrapper _spywordsQueryWrapper = new SpywordsQueryWrapper(
            ConfigurationManager.AppSettings["spywordsLogin"], 
            ConfigurationManager.AppSettings["spywordsPassword"]
        );

        protected AlgoBase(TimeSpan wakeupInterval) {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => {
                DoAction();
                return null;
            }, wakeupInterval, null));
        }

        protected abstract void DoAction();
    }
}