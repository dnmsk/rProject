using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        protected static DateTime LinuxUtc => new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        protected static List<string> FormatCompetitionName(string competitionName) {
            return competitionName.RemoveAllTags()
                                  .Replace("&nbsp;", " ")
                                  .Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => s.Trim())
                                  .ToList();
        }
        
        protected string LoadPage(string url, string postData = null, string contentType = null) {
            try {
                var loadResult = RequestHelper.GetContent(url, postData, contentType);
                if (loadResult.Item1 != HttpStatusCode.OK) {
                    Logger.Error("status = " + loadResult.Item1);
                }
                return loadResult.Item2;
            }
            catch (Exception ex) {
                Logger.Error("url: {0} \r\n" + ex, url);
            }
            return null;
        }
        
        protected static string GetParamValueForCompetition(SportType sportType, Dictionary<string, string> srcDict, string strJoinDelim) {
            var listParams = new List<string>();
            foreach (var competition in srcDict) {
                if ((sportType & ((SportType)Enum.Parse(typeof(SportType), competition.Key))) > 0) {
                    listParams.Add(competition.Value);
                }
            }
            return listParams.Count > 0 ? listParams.StrJoin(strJoinDelim) : string.Empty;
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

        protected DateTime ParseDateTime(string date) {
            date = date.ToLower().Replace("мая", "май");
            var defaultDateTime = DateTime.MinValue;
            foreach (var dateTimeFormat in CurrentConfiguration.StringArray[SectionName.ArrayDateTimeFormat]) {
                var dateTime = StringParser.ToDateTime(date, defaultDateTime, dateTimeFormat);
                if (!dateTime.Equals(defaultDateTime)) {
                    return dateTime;
                }
            }
            return defaultDateTime;
        }

        protected static Dictionary<string, object> ToD(object obj) {
            return (Dictionary<string, object>)obj;
        }
        protected static object[] ToA(object obj) {
            return (object[])obj;
        }
    }
}