﻿using System.Collections.Generic;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public static class DisplayColumnProcessor {
        private static readonly SiteTextType[] _shortHeaders = {
            SiteTextType.GridOddRoi,
            SiteTextType.GridOddTitleWin1,
            SiteTextType.GridOddTitleWin2,
            SiteTextType.GridOddTitleHcap1,
            SiteTextType.GridOddTitleHcap2,
            SiteTextType.GridOddTitleUnder,
            SiteTextType.GridOddTitleOver,
            SiteTextType.GridOddResult,
        };

        private static readonly SiteTextType[] _fullHeaders = {
            SiteTextType.GridOddRoi,
            SiteTextType.GridOddTitleWin1,
            SiteTextType.GridOddTitleDraw,
            SiteTextType.GridOddTitleWin2,
            SiteTextType.GridOddTitleWin1Draw,
            SiteTextType.GridOddTitleWin1Win2,
            SiteTextType.GridOddTitleDrawWin2,
            SiteTextType.GridOddTitleHcap1,
            SiteTextType.GridOddTitleHcap2,
            SiteTextType.GridOddTitleUnder,
            SiteTextType.GridOddTitleOver,
            SiteTextType.GridOddResult,
        };

        private static readonly Dictionary<SiteTextType, DisplayColumnType> _displayColumnHeadersMap = new Dictionary<SiteTextType, DisplayColumnType> {
            { SiteTextType.GridOddTitleWin1, DisplayColumnType.TraditionalOdds},
            { SiteTextType.GridOddTitleDraw, DisplayColumnType.TraditionalOdds},
            { SiteTextType.GridOddTitleWin2, DisplayColumnType.TraditionalOdds},
            { SiteTextType.GridOddTitleWin1Draw, DisplayColumnType.AdditionalOdds},
            { SiteTextType.GridOddTitleWin1Win2, DisplayColumnType.AdditionalOdds},
            { SiteTextType.GridOddTitleDrawWin2, DisplayColumnType.AdditionalOdds},
            { SiteTextType.GridOddTitleHcap1, DisplayColumnType.HandicapOdds},
            { SiteTextType.GridOddTitleHcap2, DisplayColumnType.HandicapOdds},
            { SiteTextType.GridOddTitleUnder, DisplayColumnType.TotalOdds},
            { SiteTextType.GridOddTitleOver, DisplayColumnType.TotalOdds},
            { SiteTextType.GridOddResult, DisplayColumnType.Result},
            { SiteTextType.GridOddRoi, DisplayColumnType.Roi1X2},
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

        public static readonly BetOddType[] OddsToInvertAdvanced = { BetOddType.Handicap1 };

        public static bool NeedDrawCell(BetOddType betOddType, DisplayColumnType displayColumn) {
            DisplayColumnType displayColumnType;
            return _displayColumnOddMap.TryGetValue(betOddType, out displayColumnType) && displayColumn.HasFlag(displayColumnType);
        }

        public static bool NeedDrawCell(SiteTextType betOddType, DisplayColumnType displayColumn) {
            DisplayColumnType displayColumnType;
            return _displayColumnHeadersMap.TryGetValue(betOddType, out displayColumnType) && displayColumn.HasFlag(displayColumnType);
        }

        public static readonly Dictionary<SportType, SiteTextType[]> TableHeaders =
            new Dictionary<SportType, SiteTextType[]> {
                {SportType.Basketball, _shortHeaders},
                {SportType.Football, _fullHeaders},
                {SportType.IceHockey, _fullHeaders},
                {SportType.Tennis, _shortHeaders},
                {SportType.Volleyball, _shortHeaders}
            };

        public static readonly BetOddType[] OddsWithSymbol = {
            BetOddType.Handicap1,
            BetOddType.Handicap2,
        };
        public static readonly BetOddType[] OddsWithAdvanced = {
            BetOddType.Handicap1,
            BetOddType.Handicap2,
            BetOddType.TotalUnder,
            BetOddType.TotalOver,
        };
    }
}