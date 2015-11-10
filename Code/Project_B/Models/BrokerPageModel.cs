using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class BrokerPageModel : StaticPageBaseModel<object> {
        private static readonly StaticPageWebCache<string, BrokerPageTransport> _brokerPagesCache = new StaticPageWebCache<string, BrokerPageTransport>(
            () => MainProvider.Instance.StaticPageProvider.GetCurrentBrokerPageModels(true),
            type => type.ToLowerInvariant());

        public BrokerPageTransport BrokerPageTransport { get; private set; }
        public BrokerPageModel(LanguageType languageType, string pageUrl, BaseModel baseModel) : base(languageType, PageType.BookmakerPage, baseModel) {
            BrokerPageTransport = _brokerPagesCache.GetPage(languageType, pageUrl) ?? new BrokerPageTransport();
        }
    }
}