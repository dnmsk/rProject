namespace Spywords_Project.Models.EntityModel {
    public struct ProgressStatusSummary {
        public int PhrasesCount { get; set; }
        public int PhrasesLast30MinCount { get; set; }
        public int UserQueriesCount { get; set; }
        public int DomainsCount { get; set; }
        public int DomainsLast30MinCount { get; set; }
        public int EmailCount { get; set; }
        public int EmailCountDistinctDomain { get; set; }
        public int PhoneCount { get; set; }
        public int PhoneCountDistinct { get; set; }
    }
}