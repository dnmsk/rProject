using System;

namespace AutoPublication.Models {
    public class BuildPublishItem {
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public string BuildName { get; set; }
        public DateTime? ProjectPublishDate { get; set; }
    }
}