using CommonUtils.Code;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public interface IOddsProvider {
        BrokerData LoadLive(SportType sportType, LanguageType language);
        BrokerData LoadRegular(SportType sportType, LanguageType language);
        WebRequestHelper RequestHelper { get; }
    }
}