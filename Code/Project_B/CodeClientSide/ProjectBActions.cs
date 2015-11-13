namespace Project_B.CodeClientSide {
    public enum ProjectBActions {
        Undefined = 0,

        //NOTE Page open event from 1000 to 2000
        PageHomeIndex = 1000,
        PageHomeAbout = 1001,
        PageHomeContact = 1002,

        PageHistoryIndex = 1100,
        PageHistoryIndexConcrete = 1101,
        PageHistoryCompetitionID = 1102,
        PageHistoryCompetitionIDConcrete = 1103,
        PageHistoryCompetitorID = 1104,
        PageHistoryCompetitorIDConcrete = 1105,

        PageLiveIndex = 1200,
        PageLiveGameIDConcrete = 1201,
        PageLiveCompetitionItemID = 1202,
        PageLiveCompetitionItemIDConcrete = 1203,
        PageLiveGameID = 1204,
        PageLiveIndexConcrete = 1205,

        PageCompetitionIndex = 1300,
        PageCompetitionIndexConcrete = 1301,
        PageCompetitionItemID = 1302,
        PageCompetitionItemIDConcrete = 1303,
        PageCompetitionGame = 1304,
        PageCompetitionGameConcrete = 1305,

        PageBookmakerPage = 1500,
        PageBookmakerConcretePage = 1501,

        PageAccountLoginIndex = 1900,
        PageAccountLoginPost = 1901,

        PageErrorNotFound = 1990,

    }
}