using System;
using System.Net;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public class BookmakerHtmlHelper : Singleton<BookmakerHtmlHelper> {
        public const string BOOKMAKER_S = "bookmakers";
        private readonly MultipleKangooCache<BrokerType, BrokerPageIcon> _bookmakerCache;
        private const string _classPrefix = "_b";
        private short _incr;

        public BookmakerHtmlHelper() {
            _bookmakerCache = new MultipleKangooCache<BrokerType, BrokerPageIcon>(null,
                cache => {
                    foreach (var brokerPagesTransport in ProjectProvider.Instance.StaticPageProvider.GetCurrentBrokerPageModels(false)) {
                        foreach (var pageTransport in brokerPagesTransport.Value) {
                            if (!cache.ContainsKey(pageTransport.BrokerType) &&
                                                            !pageTransport.TargetUrl.IsNullOrEmpty()) {
                                var brokerPageIcon = new BrokerPageIcon {
                                    PageUrl = pageTransport.Pageurl,
                                    TargetUrl = pageTransport.TargetUrl,
                                    IconClass = _classPrefix
                                };
                                cache[pageTransport.BrokerType] = brokerPageIcon;
                                var oddsProvider = BookPage.Instance.GetBrokerProvider(pageTransport.BrokerType);
                                var url = oddsProvider.CurrentConfiguration.StringSimple[SectionName.StringFaviconTarget];
                                if (url.IsNullOrEmpty()) {
                                    continue;
                                }
                                SlothMovePlodding.Instance.AddAction(() => {
                                    var data = oddsProvider.RequestHelper.GetContentRaw(url);
                                    if (data.Item1 == HttpStatusCode.OK) {
                                        lock (cache) {
                                            var currentClassName = _classPrefix + _incr++;
                                            brokerPageIcon.IconClass += " " + currentClassName;
                                            var base64 = Convert.ToBase64String(data.Item2);
                                            SquishItMinifierStatic.Instance
                                                                    .Css(BOOKMAKER_S)
                                                                    .AddString(string.Format(".{0}{{background-image:url(\"data:image/png;base64,{1}\");}}", currentClassName, base64))
                                                                    .AsCached(BOOKMAKER_S);
                                        }
                                    }
                                });
                            }
                        }
                    }
                });
            SquishItMinifierStatic.Instance
                                  .Css(BOOKMAKER_S)
                                  .AddString(string.Format(".{0}{{display:inline-block;width:18px;height:18px;vertical-align:text-bottom;background-position: center center;background-repeat: no-repeat;}}", _classPrefix))
                                  .AsCached(BOOKMAKER_S);
        }

        public BrokerPageIcon GetBroker(BrokerType brokerType) {
            BrokerPageIcon value;
            return _bookmakerCache.TryGetValue(brokerType, out value) ? value : new BrokerPageIcon();
        }
    }
}