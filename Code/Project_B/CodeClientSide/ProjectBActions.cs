namespace Project_B.CodeClientSide {
    public enum ProjectBActions {
        Undefined = 0,

        //NOTE Page open event from 1000 to 2000
        PageHomeIndex = 1000,
        PageHomeAbout = 1001,
        PageHomeContact = 1002,

        PageHistoryIndex = 1100,
        PageHistoryIndexConcrete = 1101,
        PageHistoryCompetitionUniqueID = 1102,
        PageHistoryCompetitionUniqueIDConcrete = 1103,
        PageHistoryCompetitorID = 1104,
        PageHistoryCompetitorIDConcrete = 1105,

        PageLiveIndex = 1200,
        PageLiveCompetitionItemIDConcrete = 1201,
        PageLiveCompetitionUniqueID = 1202,
        PageLiveCompetitionUniqueIDConcrete = 1203,
        PageLiveCompetitionItemID = 1204,
        PageLiveIndexConcrete = 1205,

        PageCompetitionIndex = 1300,
        PageCompetitionIndexConcrete = 1301,
        PageCompetitionUniqueID = 1302,
        PageCompetitionUniqueIDConcrete = 1303,
        PageCompetitionItemID = 1304,
        PageCompetitionItemIDConcrete = 1305,

        PageBookmakerPage = 1500,
        PageBookmakerConcretePage = 1501,

        PageAccountLoginIndex = 1900,
        PageAccountLoginPost = 1901,

        PageErrorNotFound = 1990,

    }
}