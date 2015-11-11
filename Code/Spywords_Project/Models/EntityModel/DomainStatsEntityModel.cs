using System;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Models.EntityModel {
    public class DomainStatsEntityModel {
        public SearchEngine SearchEngine { get; set; }
        public int DomainID { get; set; }
        public string Domain { get; set; }
        public int VisitsMonth { get; set; }
        public int Advertsgoogle { get; set; }
        public int Advertsyandex { get; set; }
        public int Budgetgoogle { get; set; }
        public int Budgetyandex { get; set; }
        public DateTime? Datecollected { get; set; }
        public int Phrasesgoogle { get; set; }
        public int Phrasesyandex { get; set; }
        public DomainStatus Status { get; set; }
        public string[] Phones { get; set; }
        public string[] Emails { get; set; }
    }
}