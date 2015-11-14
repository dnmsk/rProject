using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public class BookmakerHtmlHelper : Singleton<BookmakerHtmlHelper> {
        private readonly MultipleKangooCache<BrokerType, BrokerPageTransport> _bookmakerCache;

        public BookmakerHtmlHelper() {
            _bookmakerCache = new MultipleKangooCache<BrokerType, BrokerPageTransport>(MainLogicProvider.WatchfulSloth,
                cache => {
                    foreach (var brokerPagesTransport in ProjectProvider.Instance.StaticPageProvider.GetCurrentBrokerPageModels(false)) {
                        foreach (var pageTransport in brokerPagesTransport.Value) {
                            _bookmakerCache[pageTransport.BrokerType] = new BrokerPageTransport {
                                Faviconclass = pageTransport.Faviconclass,
                                IsTop = pageTransport.IsTop,
                                Pageurl = pageTransport.Pageurl,
                                Alt = pageTransport.Alt
                            };
                        }
                    }
                });
        }

        public BrokerPageTransport GetBroker(BrokerType brokerType) {
            BrokerPageTransport value;
            return _bookmakerCache.TryGetValue(brokerType, out value) ? value : new BrokerPageTransport();
        }
    }
}