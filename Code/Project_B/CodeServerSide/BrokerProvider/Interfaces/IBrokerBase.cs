using System;
using CommonUtils.Code;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Interfaces {
    public interface IBrokerBase {
        WebRequestHelper RequestHelper { get; }
        BrokerType BrokerType { get; }
        BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language);
        BrokerData LoadLive(SportType sportType, LanguageType language);
        BrokerData LoadRegular(SportType sportType, LanguageType language);
    }
}