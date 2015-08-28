using System;

namespace Spywords_Project.Code.Statuses {
    [Flags]
    public enum SearchEngine : short {
        Default = 0,
        Yandex = 1,
        Google = 2
    }
}