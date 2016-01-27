using System;
using System.Linq;
using CommonUtils.Code;
using PinnacleWrapper;
using PinnacleWrapper.Enums;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public class ASportApiProvider : BrokerBase {
        private readonly PinnacleClient _pinnacleClient;

        public ASportApiProvider(WebRequestHelper requestHelper) : base(requestHelper) {
            //_pinnacleClient = new PinnacleClient(CurrentConfiguration.StringSimple[SectionName.ApiLogin], CurrentConfiguration.StringSimple[SectionName.ApiPassword], "EUR", OddsFormat.DECIMAL);
        }

        public override BrokerType BrokerType => BrokerType.ASport;

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            var sports = _pinnacleClient.GetSports().Result;
            var needed = sports.Select(s => s.Id);
            throw new NotImplementedException();
        }
    }
}