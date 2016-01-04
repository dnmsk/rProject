using System.Collections.Generic;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData {
    public enum ProcessStatType {
        Default = 0,
        CompetitionFromRaw = 1,
        CompetitionSpecifyFromRaw = 2,
        CompetitorFromRaw = 3,
        CompetitionItemFromRaw = 4,
        Bet = 5,
        Result = 6
    }

    public class SummaryProcessStat {
        private readonly Dictionary<ProcessStatType, ProcessStat> _processStats = new Dictionary<ProcessStatType, ProcessStat>(); 
        
        public ProcessStat this[ProcessStatType processStatType] {
            get {
                ProcessStat stat;
                if (!_processStats.TryGetValue(processStatType, out stat)) {
                    stat = new ProcessStat();
                    _processStats[processStatType] = stat;
                }
                return stat;
            }
        }
    }
}