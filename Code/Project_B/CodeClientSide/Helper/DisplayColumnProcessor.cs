using System.Collections.Generic;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public static class DisplayColumnProcessor {
        private static readonly SiteText[] _shortHeaders = {
            SiteText.GridOddRoi1X2,
            SiteText.GridOddTitleWin1,
            SiteText.GridOddTitleWin2,
            SiteText.GridOddTitleHcap1,
            SiteText.GridOddTitleHcap2,
            SiteText.GridOddTitleUnder,
            SiteText.GridOddTitleOver,
            SiteText.GridResult,
        };

        private static readonly SiteText[] _fullHeaders = {
            SiteText.GridOddRoi1X2,
            SiteText.GridOddTitleWin1,
            SiteText.GridOddTitleDraw,
            SiteText.GridOddTitleWin2,
            SiteText.GridOddTitleWin1Draw,
            SiteText.GridOddTitleWin1Win2,
            SiteText.GridOddTitleDrawWin2,
            SiteText.GridOddTitleHcap1,
            SiteText.GridOddTitleHcap2,
            SiteText.GridOddTitleUnder,
            SiteText.GridOddTitleOver,
            SiteText.GridResult,
        };

        private static readonly Dictionary<SiteText, DisplayColumnType> _displayColumnHeadersMap = new Dictionary<SiteText, DisplayColumnType> {
            { SiteText.GridOddTitleWin1, DisplayColumnType.TraditionalOdds},
            { SiteText.GridOddTitleDraw, DisplayColumnType.TraditionalOdds},
            { SiteText.GridOddTitleWin2, DisplayColumnType.TraditionalOdds},
            { SiteText.GridOddTitleWin1Draw, DisplayColumnType.AdditionalOdds},
            { SiteText.GridOddTitleWin1Win2, DisplayColumnType.AdditionalOdds},
            { SiteText.GridOddTitleDrawWin2, DisplayColumnType.AdditionalOdds},
            { SiteText.GridOddTitleHcap1, DisplayColumnType.HandicapOdds},
            { SiteText.GridOddTitleHcap2, DisplayColumnType.HandicapOdds},
            { SiteText.GridOddTitleUnder, DisplayColumnType.TotalOdds},
            { SiteText.GridOddTitleOver, DisplayColumnType.TotalOdds},
            { SiteText.GridResult, DisplayColumnType.Result},
            { SiteText.GridOddRoi1X2, DisplayColumnType.Roi1X2},
        };

        private static readonly Dictionary<BetOddType, DisplayColumnType> _displayColumnOddMap = new Dictionary<BetOddType, DisplayColumnType> {
            { BetOddType.Win1, DisplayColumnType.TraditionalOdds},
            { BetOddType.Draw, DisplayColumnType.TraditionalOdds},
            { BetOddType.Win2, DisplayColumnType.TraditionalOdds},
            { BetOddType.Win1Draw, DisplayColumnType.AdditionalOdds},
            { BetOddType.Win1Win2, DisplayColumnType.AdditionalOdds},
            { BetOddType.DrawWin2, DisplayColumnType.AdditionalOdds},
            { BetOddType.Handicap1, DisplayColumnType.HandicapOdds},
            { BetOddType.Handicap2, DisplayColumnType.HandicapOdds},
            { BetOddType.TotalUnder, DisplayColumnType.TotalOdds},
            { BetOddType.TotalOver, DisplayColumnType.TotalOdds},
        };

        public static bool NeedDrawCell(BetOddType betOddType, DisplayColumnType displayColumn) {
            DisplayColumnType displayColumnType;
            if (_displayColumnOddMap.TryGetValue(betOddType, out displayColumnType)) {
                 return displayColumn.HasFlag(displayColumnType);
            }
            return false;
        }

        public static bool NeedDrawCell(SiteText betOddType, DisplayColumnType displayColumn) {
            DisplayColumnType displayColumnType;
            if (_displayColumnHeadersMap.TryGetValue(betOddType, out displayColumnType)) {
                return displayColumn.HasFlag(displayColumnType);
            }
            return false;
        }

        public static readonly Dictionary<SportType, SiteText[]> TableHeaders =
            new Dictionary<SportType, SiteText[]> {
                {SportType.Basketball, _shortHeaders},
                {SportType.Football, _fullHeaders},
                {SportType.IceHockey, _fullHeaders},
                {SportType.Tennis, _shortHeaders},
                {SportType.Volleyball, _shortHeaders}
            };

        private static readonly BetOddType[] _shortPercentile = {
            BetOddType.Win1,
            BetOddType.Win2
        };

        private static readonly BetOddType[] _fullPercentile = {
            BetOddType.Win1,
            BetOddType.Draw,
            BetOddType.Win2
        };

        public static readonly Dictionary<SportType, BetOddType[]> PercentileMap =
            new Dictionary<SportType, BetOddType[]> {
                {SportType.Basketball, _shortPercentile},
                {SportType.Football, _fullPercentile},
                {SportType.IceHockey, _fullPercentile},
                {SportType.Tennis, _shortPercentile},
                {SportType.Volleyball, _shortPercentile}
            };

        public static readonly SportType[] SportTypesBySum = {SportType.Tennis, SportType.Volleyball};
        public static readonly BetOddType[] OddsWithAdvanced = {
            BetOddType.Handicap1,
            BetOddType.Handicap2,
            BetOddType.TotalUnder,
            BetOddType.TotalOver,
        };
        public static readonly BetOddType[] OddsToInvertAdvanced = {
            BetOddType.Handicap1
        };

        public static readonly BetOddType[] OddsWithSymbol = {
            BetOddType.Handicap1,
            BetOddType.Handicap2,
        };
    }
}