using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.Code.BrokerProvider.Configuration;
using Project_B.Code.Data;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider {
    public abstract class BrokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof (BrokerBase).FullName);

        private readonly WebRequestHelper _requestHelper;
        protected static readonly JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer();

        public string Url { get; private set; }

        public abstract BrokerType BrokerType { get; }

        public BrokerConfiguration CurrentConfiguration {
            get { return ConfigurationContainer.Instance.BrokerConfiguration[BrokerType]; }
        }

        public BrokerBase(WebRequestHelper requestHelper) {
            _requestHelper = requestHelper;
        }

        //protected HtmlBlockHelper HtmlBlock;
        protected string LoadPage(string url, string postData = null, string contentType = null) {
            try {
                Url = url;
                var loadResult = _requestHelper.GetContent(url, postData, contentType);
                if (loadResult.Item1 != HttpStatusCode.OK) {
                    Logger.Error("status = " + loadResult.Item1);
                }
                return loadResult.Item2;
                //HtmlBlock = new HtmlBlockHelper(loadResult.Item2);

            }
            catch (Exception ex) {
                Logger.Error("url: {0} \r\n" + ex);
            }
            return null;
        }

        public abstract List<CompetitionParsed> BuildCompetitions(string htmlContent);

        protected static string GetParamValueForCompetition(SportType sportType, Dictionary<SportType, string> srcDict, string strJoinDelim) {
            var listParams = new List<string>();
            foreach (var competition in srcDict) {
                if ((sportType & competition.Key) > 0) {
                    listParams.Add(competition.Value);
                }
            }
            return listParams.Count > 0 ? listParams.StrJoin(strJoinDelim) : string.Empty;
        }
    }
}