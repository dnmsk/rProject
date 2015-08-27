using System;

namespace Spywords_Project.Code.Statuses {
    [Flags]
    public enum SearchEngine : short {
        Yandex = 1,
        Google = 2
    }
}