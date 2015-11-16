using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Models {
    public class BrokerPageModel : StaticPageBaseModel {
        private static readonly StaticPageWebCache<string, BrokerPageTransport> _brokerPagesCache = new StaticPageWebCache<string, BrokerPageTransport>(
            () => ProjectProvider.Instance.StaticPageProvider.GetCurrentBrokerPageModels(true),
            type => type.ToLowerInvariant());

        public BrokerPageTransport BrokerPageTransport { get; }

        public static bool ContainsBroker(string brokerName) {
            return _brokerPagesCache.GetPage(LanguageType.Default, brokerName) != null;
        }

        public BrokerPageModel(string pageUrl, ProjectControllerBase projectControllerBase) : base(projectControllerBase.GetBaseModel()) {
            BrokerPageTransport = _brokerPagesCache.GetPage(projectControllerBase.CurrentLanguage, pageUrl);
            if (BrokerPageTransport != null) {
                projectControllerBase.LogAction(ProjectBActions.PageBookmakerConcretePage, BrokerPageTransport.ID);
            }
        }
    }
}