using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.Code.Data;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider {
    public class BetProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BetProvider).FullName);

        private static readonly SportType[] _sportWithAdvancedDetail = {
            SportType.Football, SportType.IceHockey,
        };

        public BetProvider() : base(_logger) {}

        public void AddBetParsed(int competitionItemID, BrokerType brokerType, SportType sportType, OddParsed[] oddsParsed) {
            InvokeSafeSingleCall(() => {
                var betWithAdvancedDb = Bet.DataSource
                    .Join(JoinType.Left, BetAdvanced.Fields.BetID, Bet.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(Bet.Fields.CompetitionitemID, competitionItemID)
                    .WhereEquals(Bet.Fields.BrokerID, brokerType)
                    .Sort(Bet.Fields.ID, SortDirection.Desc)
                    .First();
                var betAdvancedDb = betWithAdvancedDb.GetJoinedEntity<BetAdvanced>();
                var bet = Bet.GetBetFromOdds(oddsParsed);
                var betAdvanced = BetAdvanced.GetBetFromOdds(oddsParsed);
                if (!betWithAdvancedDb.IsEqualsTo(bet) || _sportWithAdvancedDetail.Contains(sportType) && betAdvancedDb != null && !betAdvanced.IsEqualsTo(betAdvanced)) {
                    bet.CompetitionitemID = competitionItemID;
                    bet.Save();
                    if (_sportWithAdvancedDetail.Contains(sportType)) {
                        betAdvanced.BetID = bet.ID;
                        betAdvanced.Save();
                    }
                }
                return null;
            }, (object) null);
        }
    }
}