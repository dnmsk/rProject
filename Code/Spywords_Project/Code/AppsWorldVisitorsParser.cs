using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.Code.WebRequestData;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;

namespace Spywords_Project.Code {
    public class AppsWorldVisitorsParser {
        private int _id;
        private readonly WebRequestHelper _webRequestHelper;
        private readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();

        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (AppsWorldVisitorsParser).FullName);

        public AppsWorldVisitorsParser(IEnumerable<Cookie> cookies, string referer) {
            var cookieContainer = new CookieContainer();
            cookies.Each(c => {
                cookieContainer.Add(c);
            });
            _webRequestHelper = new WebRequestHelper(null, cookieContainer) {
                MinRequestDelay = TimeSpan.FromSeconds(5)
            };
            _webRequestHelper.SetParam(WebRequestParamType.UserAgentString, new WebRequestParamString(referer));
            _webRequestHelper.SetParam(WebRequestParamType.AcceptString, new WebRequestParamString("application/json,application/javascript"));
            _webRequestHelper.SetParam(WebRequestParamType.ContentTypeString, new WebRequestParamString("application/json; charset=utf-8"));
            _webRequestHelper.SetParam(WebRequestParamType.HeadersArrayKeyValue,
                new WebRequestParamWebHeaderCollection(new WebHeaderCollection {
                })
            );
        }

        public List<string[]> ParseVisitors(Func<List<string[]>, string> pager) {
            var result = new List<string[]>();
            var prevResult = (List<string[]>)null;
            string nextUrl = null;
            while (!(nextUrl = pager(prevResult)).IsNullOrWhiteSpace()) {
                prevResult = GetResultForUrl(nextUrl);
                if (prevResult != null) {
                    result.AddRange(prevResult);
                }
            }
            return result;
        }

        private List<string[]> GetResultForUrl(string url) {
            var result = new List<string[]>();

            var data = _webRequestHelper.GetContent(url);
            if (data.Item1 != HttpStatusCode.OK) {
                return result;
            }
            var str = data.Item2;
            using (var f = File.Create(Path.Combine("E:\\del\\appsworld", _id++.ToString()))) {
                using (var sw = new StreamWriter(f)) {
                    sw.Write(str);
                }
            }
            var deserialized = _javaScriptSerializer.DeserializeObject(str);

            foreach (Dictionary<string, object> dict in (object[]) deserialized) {
                try {
                    var arr = new string[12];
                    arr[0] = new[] { dict.TryGetValueOrDefault("firstNames") as string ?? string.Empty,
                                dict.TryGetValueOrDefault("lastNames") as string ?? string.Empty
                        }.Where(s => !s.IsNullOrEmpty()).StrJoin(" ");
                    arr[1] = dict.TryGetValueOrDefault("companyName") as string ?? string.Empty;
                    arr[2] = dict.TryGetValueOrDefault("department") as string ?? string.Empty;
                    arr[3] = dict.TryGetValueOrDefault("jobTitle") as string ?? string.Empty;
                    arr[4] = dict.TryGetValueOrDefault("reasonForAttending") as string ?? string.Empty;
                    arr[5] = ((Dictionary<string, object>)dict.TryGetValueOrDefault("address") ?? new Dictionary<string, object>()).Values.Where(v => !(v as string).IsNullOrEmpty()).StrJoin(" ");
                    var contacts = ((Dictionary<string, object>)dict.TryGetValueOrDefault("contact") ?? new Dictionary<string, object>());
                    arr[6] = contacts.TryGetValueOrDefault("email") as string ?? string.Empty;
                    arr[7] = contacts.TryGetValueOrDefault("www") as string ?? string.Empty;
                    arr[8] = dict.TryGetValueOrDefault("fullDescription") as string ?? string.Empty;
                    arr[9] = dict.TryGetValueOrDefault("mugShotUrl") as string ?? string.Empty;
                    arr[10] = ((Dictionary<string, object>)dict.TryGetValueOrDefault("visitorGroup") ?? new Dictionary<string, object>()).TryGetValueOrDefault("name") as string ?? string.Empty;
                    arr[11] = ((Dictionary<string, object>)dict.TryGetValueOrDefault("contact") ?? new Dictionary<string, object>()).Where(v => !(v.Value as string).IsNullOrEmpty()).Select(kv =>
                        $"{kv.Key}:{kv.Value}").StrJoin(" ");
                    result.Add(arr);
                } catch(Exception ex) {
                    _logger.Error(ex);
                }
            }
            return result;
        } 
    }
}