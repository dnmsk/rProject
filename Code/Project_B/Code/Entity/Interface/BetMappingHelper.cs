using System;
using System.Collections.Generic;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Code.Entity.Interface {
    public static class BetMappingHelper<T> {
        public static readonly Dictionary<BetOddType, Func<IBet<T>, BetItem>> OddsGetterMap = new Dictionary<BetOddType, Func<IBet<T>, BetItem>> {
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

        private static BetItem ReturnIfNotNull<T>(Func<IBetAdvanced<T>, BetItem> func, IBet<T> bet) {
            var betAdvanced = bet.GetJoinedEntity<IBetAdvanced<T>>();
            return betAdvanced == null ? new BetItem() : func(betAdvanced);
        }

        private static BetItem BuildBetItem<T>(IBet<T> bet) {
            return new BetItem {
                BrokerType = bet.BrokerID,
                DateTimeUtc = bet.Datecreatedutc
            };
        }

        private static BetItem FillBetItem(BetItem betItem, float? value, float? advanced) {
            betItem.Odd = value ?? default(float);
            betItem.AdvancedParam = advanced ?? default(float);
            return betItem;
        }

    }
}