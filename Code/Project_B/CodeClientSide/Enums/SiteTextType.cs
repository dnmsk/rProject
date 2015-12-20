namespace Project_B.CodeClientSide.Enums {
    public enum SiteTextType : short {
        Unknown = 0,

        HeaderSiteName = 10,
        HeaderCompetition = 11,
        HeaderCompetitionLive = 12,
        HeaderHistory = 13,
        HeaderBookmaker = 14,
        HeaderLoginHello = 15,
        HeaderLogOff = 16,
        HeaderAccountManageTitle = 17,
        HeaderRegister = 18,
        HeaderLogin = 19,

        FooterSiteLanguage = 50,
        FooterAdminEmail = 51,

        SubHeaderAllCompetitions = 100,
        SubHeaderProfitable = 101,

        GridOddTitleWin1 = 1000,
        GridOddTitleDraw = 1001,
        GridOddTitleWin2 = 1002,
        GridOddTitleWin1Draw = 1003,
        GridOddTitleWin1Win2 = 1004,
        GridOddTitleDrawWin2 = 1005,
        GridOddTitleHcap1 = 1006,
        GridOddTitleHcap2 = 1007,
        GridOddTitleUnder = 1008,
        GridOddTitleOver = 1009,

        GridOddFooterSeeMore = 1010,
        GridOddResult = 1011,
        GridOddRoi = 1012,

        GameRoiWidgetHeader = 1050,
        GameRoiHowToGet = 1051,
        GameRoiHowToGetAlt = 1052,
        GameRoi1X2 = 1053,
        // ReSharper disable once InconsistentNaming
        GameRoi1X_2 = 1054,
        // ReSharper disable once InconsistentNaming
        GameRoi12_X = 1055,
        // ReSharper disable once InconsistentNaming
        GameRoi1_X2 = 1056,
        GameRoiHandicap = 1057,
        GameRoiTotal = 1058,

        ChartTitle = 1070,

        FilterBtnDateName = 1100,
        FilterBtnOnlyBetName = 1101,
        FilterBtnAllName = 1102,

    }
}