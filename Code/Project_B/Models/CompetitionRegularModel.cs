using System.Collections.Generic;
using Project_B.CodeClientSide.TransportType;

namespace Project_B.Models {
    public class CompetitionRegularModel {
        public List<CompetitionTransport> Competitions { get; set; } 

        public FilterModel Filter { get; set; }

        public CompetitionRegularModel() {
            Filter = new FilterModel();
        }
    }
}