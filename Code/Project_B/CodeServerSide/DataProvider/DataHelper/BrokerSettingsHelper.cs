using System;
using System.Net;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using CommonUtils.WatchfulSloths.WatchfulThreads;
using MainLogic;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public class BrokerSettingsHelper : Singleton<BrokerSettingsHelper> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BrokerSettingsHelper).FullName);

        public const string BOOKMAKER_S = "bookmakers";
        private const string _classPrefix = "_b";
        private short _incr;

        private readonly MultipleKangooCache<BrokerType, Tuple<BrokerCompetitionSettings, BrokerPageIcon>> _languageTypeToName;

        public BrokerSettingsHelper() {
            _languageTypeToName = new MultipleKangooCache<BrokerType, Tuple<BrokerCompetitionSettings, BrokerPageIcon>>(MainLogicProvider.WatchfulSloth,
                tuples => {
                    var brokers = Broker.DataSource.AsList();
                    foreach (var broker in brokers) {
                        var brokerPageIcon = new BrokerPageIcon {
                            PageUrl = broker.InternalPage,
                            TargetUrl = broker.ExternalUrl,
                            IconClass = _classPrefix
                        };
                        tuples[broker.BrokerType] = Tuple.Create(broker.CompetitionSettings, brokerPageIcon);
                        var oddsProvider = BookPage.Instance.GetBrokerProvider(broker.BrokerType);
                        TaskRunner.Instance.AddAction(() => {
                            Tuple<HttpStatusCode, byte[]> data = null;
                            foreach (var proto in new[] { "http://", "https://" }) {
                                try {
                                    data = oddsProvider.RequestHelper.GetContentRaw(proto + brokerPageIcon.TargetUrl + "/favicon.ico");
                                    break;
                                } catch(Exception ex) {
                                    _logger.Error(ex);
                                }
                            }
                            if (data.Item1 == HttpStatusCode.OK) {
                                lock (tuples) {
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
                }, TimeSpan.FromHours(1));
            SquishItMinifierStatic.Instance
                                  .Css(BOOKMAKER_S)
                                  .AddString(string.Format(".{0}{{display:inline-block;width:18px;height:18px;vertical-align:text-bottom;background-position: center center;background-repeat: no-repeat;}}", _classPrefix))
                                  .AsCached(BOOKMAKER_S);
        }

        public BrokerCompetitionSettings GetSettings(BrokerType brokerType) {
            return GetBrokerRow(brokerType).Item1;
        }

        public BrokerPageIcon GetBroker(BrokerType brokerType) {
            return GetBrokerRow(brokerType).Item2 ?? new BrokerPageIcon();
        }

        public BrokerType[] GetBrokerTypes => _languageTypeToName.Keys;

        private Tuple<BrokerCompetitionSettings, BrokerPageIcon> GetBrokerRow(BrokerType brokerType) {
            Tuple<BrokerCompetitionSettings, BrokerPageIcon> tuple;
            return _languageTypeToName.TryGetValue(brokerType, out tuple) ? tuple : new Tuple<BrokerCompetitionSettings, BrokerPageIcon>(BrokerCompetitionSettings.Default, null);
        }
    }
}