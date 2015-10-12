using System;
using System.Collections.Generic;
using Project_B.Code.Data;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider {
    public interface IResultHistoryProvider {
        List<CompetitionParsed> Load(DateTime date, SportType sportType);
    }
}
