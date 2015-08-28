using System;
using System.Configuration;
using System.Text.RegularExpressions;
using CommonUtils.Core.Logger;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;

namespace Spywords_Project.Code.Algorithms {
    public abstract class AlgoBase {
        protected const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase;

        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof(AlgoBase).FullName);
        protected static readonly SpywordsQueryWrapper SpywordsQueryWrapper = new SpywordsQueryWrapper(
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