using System;

namespace Spywords_Project.Code.Statuses {
    [Flags]
    public enum DomainStatus : short {
        Default = 0,
        Loaded = 1,
        SpywordsCollected = 2,
        EmailPhoneCollected = 4,
        PhrasesCollected = 8,
    }
}