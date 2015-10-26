using System;
using Project_B.Code.Data;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider {
    public interface IResultHistoryProvider {
        BrokerData Load(DateTime date, SportType sportType, LanguageType english);
    }
}
