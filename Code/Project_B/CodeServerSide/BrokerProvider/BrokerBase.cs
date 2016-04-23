using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public abstract class BrokerBase<T> : IBrokerBase where T : IQueryableWrapper {
        private readonly IQueryableWrapper _queryableWrapper;

        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof (BrokerBase<T>).FullName);
        protected static readonly JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer {
            MaxJsonLength = 99999999,
            RecursionLimit = 9999
        };

        public WebRequestHelper RequestHelper => _queryableWrapper.RequestHelper;

        public abstract BrokerType BrokerType { get; }
        public abstract BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language);
        public abstract BrokerData LoadLive(SportType sportType, LanguageType language);
        public abstract BrokerData LoadRegular(SportType sportType, LanguageType language);
        public BrokerConfiguration CurrentConfiguration => ConfigurationContainer.Instance.BrokerConfiguration[BrokerType];

        protected BrokerBase(IQueryableWrapper queryableWrapper) {
            _queryableWrapper = queryableWrapper;
            _queryableWrapper?.ProcessConfig(CurrentConfiguration);
        }

        protected string LoadPage(string url, List<string> postData = null, string contentType = "application/x-www-form-urlencoded") {
            return _queryableWrapper.LoadPage(url, postData, contentType);
        }
        
        protected string FormatUrl(SectionName urlTargetSection, object obj) {
            return CurrentConfiguration.StringSimple[urlTargetSection].HaackFormat(obj);
        }
        protected static IEnumerable<SportType> GetSportSimpleTypes(SportType sportType) {
            return sportType.GetFlags<SportType>().Where(f => f != SportType.All && f != SportType.Unknown);
        }

        protected string GetLanguageParam(LanguageType language) {
            var languageParams = CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsLanguageUrlParam];
            string languageParam;
            if (!languageParams.TryGetValue(language.ToString(), out languageParam) &&
                !languageParams.TryGetValue(LanguageType.Default.ToString(), out languageParam)) {
                languageParam = "en";
            }
            return languageParam;
        }
        
        protected static Dictionary<string, object> ToD(object obj) {
            return (Dictionary<string, object>)obj;
        }
        protected static object[] ToA(object obj) {
            return (object[])obj;
        }

        public T QuerySender { get; set; }
    }
}