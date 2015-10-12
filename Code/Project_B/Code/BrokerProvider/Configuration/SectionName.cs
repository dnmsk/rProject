﻿namespace Project_B.Code.BrokerProvider.Configuration {
    public enum SectionName : short {
        AaundefinedDefault = 0,

        //Url Reserve from 1 to 999
        Domain = 1,
        UrlLiveTarget = 2,
        UrlOddsTarget = 3,
        UrlResultTarget = 4,
        
        SimpleStringUserAgent = 901,

        //XPath Reserve from 1000 to 9999
        XPathToCategoryContainer = 1000,
        XPathToCategoryName = 1001,
        XPathToListCompetitionInCategory = 1002,
        XPathToCompetitionName = 1003,
        XPathToEventInList = 1004,
        XPathToOddsParticipants = 1005,
        XPathToOddsDate = 1006,
        XPathToOddsLiveResult = 1007,
        XPathToOddsFactor = 1008,
        XPathToEventResult = 1009,
        XPathToResultParticipants = 1010,
        XPathToResultDate = 1011,
        XPathToResultValue = 1012,
        XPathToOddsConfirmation = 1013,

        //StringSimple Reserve from 10000 to 14999
        StringDateTimeFormat = 10000,
        StringDateQueryFormat = 10001,
        StringMapStringsOddsParamJoin = 10002,
        StringMapStringsResultsParamJoin = 10003,
        StringOddConfirmation = 10004,
        StringOddBrokerID = 10005,
        StringDateTimeShortFormat = 10006,

        //Array Reserve from 15000 to 19999
        ArrayParticipantsSplitter = 15000,
        ArrayCookie = 15001,
        ArrayProxy = 15002,

        //Map Reserve from 30000 to Max
        MapStringsOddsParam = 30000,
        MapStringsResultsParam = 30001,

        //Max value, helps improve autogenerated values without const number
        ZZzReservedLast = short.MaxValue,
    }
}