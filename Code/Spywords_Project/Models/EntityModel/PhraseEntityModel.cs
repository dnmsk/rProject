using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Models.EntityModel {
    public class PhraseEntityModel {
        public int PhraseAccountID { get; set; }
        public string Text { get; set; }
        public int DomainsCount { get; set; }
        public int CollectedDomainsCount { get; set; }
        public int AdvertsGoogle { get; set; }
        public int AdvertsYandex { get; set; }
        public int PhraseID { get; set; }
        public PhraseStatus PhraseStatus { get; set; }
    }
}