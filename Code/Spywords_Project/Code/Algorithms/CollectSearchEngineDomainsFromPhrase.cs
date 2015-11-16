using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectSearchEngineDomainsFromPhrase : AlgoBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CollectSearchEngineDomainsFromPhrase).FullName);

        private readonly WebRequestHelper _yandexRequestHelper;
        private readonly WebRequestHelper _googleRequestHelper;
        public CollectSearchEngineDomainsFromPhrase() : base(TimeSpan.FromSeconds(30)) {
            _yandexRequestHelper = BuildRequestHelper("CookiesYandex", "yandex.ru", TimeSpan.FromSeconds(15));
            _googleRequestHelper = BuildRequestHelper("CookiesGoogle", "google.ru", TimeSpan.FromSeconds(15));
        }

        protected override void DoAction() {
            foreach (var phrase in GetPhrasesToCollect()) {
                var processGoogle = false;
                var processYandex = false;

                new Thread(() => {
                    try {
                        SaveDomains(phrase, GetGoogleDomains(phrase), SearchEngine.Google);
                    } catch (Exception ex) {
                        _logger.Error(ex);
                    }
                    processGoogle = true;
                }).Start();
                new Thread(() => {
                    try {
                        SaveDomains(phrase, GetYandexDomains(phrase), SearchEngine.Yandex);
                    } catch (Exception ex) {
                        _logger.Error(ex);
                    }
                    processYandex = true;
                }).Start();

                while (!processYandex || !processGoogle) {
                    Thread.Sleep(100);
                }
            }
        }

        private List<string> GetYandexDomains(Phrase phrase) {
            var yandexDomains = new List<string>();
            var yandexUrl = "https://yandex.ru/search/?text=" + HttpUtility.UrlEncode(phrase.Text) + "&lr=213";
            var yandexTries = 0;
            var retries = 0;
            while (true) {
                if (yandexTries == 2) {
                    break;
                }
                var yandexHtml = _yandexRequestHelper.GetContent(yandexUrl + (yandexTries == 0 ? string.Empty : ("&p=" + yandexTries)));
                _logger.Info("Go to YANDEX with query " + phrase.Text);
                if (yandexHtml == null || yandexHtml.Item1 != HttpStatusCode.OK) {
                    if (retries > 2) {
                        break;
                    }
                    retries++;
                    continue;
                }
                new HtmlBlockHelper(yandexHtml.Item2)
                    .ExtractBlock(new XPathQuery(".//span[@class='serp-url__item']/*[1]"))
                    .ForEach(b => yandexDomains.Add(b.Attributes["href"].Value.GetDomain().CutWww()));
                yandexTries++;
            }
            return yandexDomains;
        }

        private List<string> GetGoogleDomains(Phrase phrase) {
            var googleDomains = new List<string>();
            var googleHtml = _googleRequestHelper.GetContent(string.Format("https://www.google.ru/search?num=100&hl=ru&site=webhp&source=hp&q=" + HttpUtility.UrlEncode(phrase.Text)));
            _logger.Info("Go to GOOGLE with query " + phrase.Text);
            if (googleHtml.Item1 == HttpStatusCode.OK) {
                new HtmlBlockHelper(googleHtml.Item2)
                    .ExtractBlock(new XPathQuery(".//h3[@class='r']/a[@href]"))
                    .ForEach(b => googleDomains.Add(b.Attributes["href"].Value.Replace("/url?q=", string.Empty).GetDomain().CutWww()));
            }
            return googleDomains;
        }

        private static void SaveDomains(Phrase phrase, IEnumerable<string> domains, SearchEngine searchEngine) {
            foreach (var domain in domains) {
                try {
                    if (domain.IsNullOrWhiteSpace()) {
                        continue;
                    }
                    var domainEntity = GetDomainEntity(domain);
                    CreateOrUpdateDomainPhrase(domainEntity, phrase, searchEngine, SourceType.Search);
                } catch (Exception ex) {
                    _logger.Error(ex);
                }
            }
        }

        private static List<Phrase> GetPhrasesToCollect() {
            return Phrase.DataSource
                      .Join(JoinType.Inner, Phraseaccount.Fields.PhraseID, Phrase.Fields.ID, RetrieveMode.NotRetrieve)
                      .Join(JoinType.Left, typeof(Domainphrase), new DaoFilterAnd(
                          new DaoFilterEq(Domainphrase.Fields.PhraseID, new DbFnEmpty(Phrase.Fields.ID)),
                          new DaoFilter(new DbFnSimpleOp(Domainphrase.Fields.SourceType, FnMathOper.BitwiseAnd, (short) SourceType.Search), Oper.NotEq, default(short))
                      ), RetrieveMode.NotRetrieve)
                      .WhereNull(Domainphrase.Fields.ID)
                      .Where(new DbFnSimpleOp(Phraseaccount.Fields.SourceType, FnMathOper.BitwiseAnd, (short) SourceType.Search), Oper.NotEq, default(short))
                      .AsList(
                            Phrase.Fields.Text
                        );
        } 
   }
}