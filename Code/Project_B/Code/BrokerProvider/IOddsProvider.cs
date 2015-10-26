using Project_B.Code.Data;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider {
    public interface IOddsProvider {
        BrokerData LoadLive(SportType sportType, LanguageType language);
        BrokerData LoadRegular(SportType sportType, LanguageType language);
    }
}