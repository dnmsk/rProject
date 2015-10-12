using System.Collections.Generic;
using Project_B.Code.Data;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider {
    public interface IOddsProvider {
        List<CompetitionParsed> LoadLive(SportType sportType);
        List<CompetitionParsed> LoadRegular(SportType sportType);
    }
}