using System;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public interface IResultHistoryProvider {
        BrokerData Load(DateTime date, SportType sportType, LanguageType english);
    }
}
