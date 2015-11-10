using System;
using System.Collections.Generic;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.CodeServerSide.Entity.Interface {
    public static class BetMappingHelper<T> {
        public static readonly Dictionary<BetOddType, Func<IBet<T>, BetItemTransport>> OddsGetterMap = new Dictionary<BetOddType, Func<IBet<T>, BetItemTransport>> {
            {BetOddType.Win1, bet => FillBetItem(BuildBetItem(bet), bet.Win1, null) },
            {BetOddType.Win2, bet => FillBetItem(BuildBetItem(bet), bet.Win2, null) },
            {BetOddType.Handicap1, bet => FillBetItem(BuildBetItem(bet), bet.Hcap1, bet.Hcapdetail) },
            {BetOddType.Handicap2, bet => FillBetItem(BuildBetItem(bet), bet.Hcap2, bet.Hcapdetail) },
            {BetOddType.TotalUnder, bet => FillBetItem(BuildBetItem(bet), bet.Totalunder, bet.Totaldetail) },
            {BetOddType.TotalOver, bet => FillBetItem(BuildBetItem(bet), bet.Totalover, bet.Totaldetail) },
            {BetOddType.Win1Draw, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Win1draw, null), bet) },
            {BetOddType.Win1Win2, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Win1win2, null), bet) },
            {BetOddType.DrawWin2, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Drawwin2, null), bet) },
            {BetOddType.Draw, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Draw, null), bet) }
        };

        private static BetItemTransport ReturnIfNotNull<T>(Func<IBetAdvanced<T>, BetItemTransport> func, IBet<T> bet) {
            var betAdvanced = bet.GetJoinedEntity<IBetAdvanced<T>>();
            return betAdvanced == null ? new BetItemTransport() : func(betAdvanced);
        }

        private static BetItemTransport BuildBetItem<T>(IBet<T> bet) {
            return new BetItemTransport {
                BrokerType = bet.BrokerID,
                DateTimeUtc = bet.Datecreatedutc
            };
        }

        private static BetItemTransport FillBetItem(BetItemTransport betItemTransport, float? value, float? advanced) {
            betItemTransport.Odd = value ?? default(float);
            betItemTransport.AdvancedParam = advanced ?? default(float);
            return betItemTransport;
        }

    }
}