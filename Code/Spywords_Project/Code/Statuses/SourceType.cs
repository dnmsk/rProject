using System;

namespace Spywords_Project.Code.Statuses {
    [Flags]
    public enum SourceType : short {
        Default = 0,
        Context = 1,
        Search = 2
    }
}