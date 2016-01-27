using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public abstract class BrokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof (BrokerBase).FullName);
        protected static readonly JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer {
            MaxJsonLength = 99999999,
            RecursionLimit = 9999
        };
        
        public WebRequestHelper RequestHelper { get; }
        public abstract BrokerType BrokerType { get; }
        public abstract BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language);
        public abstract BrokerData LoadLive(SportType sportType, LanguageType language);
        public abstract BrokerData LoadRegular(SportType sportType, LanguageType language);
        public BrokerConfiguration CurrentConfiguration => ConfigurationContainer.Instance.BrokerConfiguration[BrokerType];

        protected BrokerBase(WebRequestHelper requestHelper) {
            RequestHelper = requestHelper;
        }

        private static int _tries = 2;
        protected string LoadPage(string url, List<string> postData = null, string contentType = "application/x-www-form-urlencoded") {
            for (var i = 0; i < _tries; i++) {
                try {
                    string post = null;
                    if (postData != null) {
                        post = postData.StrJoin("&");
                    }
                    var loadResult = RequestHelper.GetContent(url, post, contentType);
                    if (loadResult.Item1 != HttpStatusCode.OK) {
                        Logger.Error("status = " + loadResult.Item1);
                    }
                    return loadResult.Item2;
                } catch (Exception ex) {
                    Logger.Error("url: {0} \r\n" + ex, url);
                }
                Thread.Sleep(5*1000);
            }
            return null;
        }
        
        protected string FormatUrl(SectionName urlTargetSection, object obj) {
            return CurrentConfiguration.StringSimple[urlTargetSection].HaackFormat(obj);
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
    }
}