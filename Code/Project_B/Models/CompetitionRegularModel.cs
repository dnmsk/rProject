using System.Collections.Generic;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;

namespace Project_B.Models {
    public class PageDisplaySettings {
        public DisplayColumnType DisplayColumn { get; set; }
        public int LimitToDisplayInGroup { get; set; }

        public PageDisplaySettings() {
            LimitToDisplayInGroup = int.MaxValue;
        }

    }
    public class CompetitionRegularModel {
        public List<CompetitionTransport> Competitions { get; set; } 

        public FilterModelBase Filter { get; set; }
        public PageDisplaySettings DisplaySettings { get; }

        public CompetitionRegularModel(PageDisplaySettings displaySettings) {
            DisplaySettings = displaySettings;
        }
    }
}