using System;

namespace Project_B.CodeServerSide {
    public static class ProjectBConsts {
        public static readonly char[] TrimmedChars = { ' ', '.', ',', '\r', '\n', '(', ')' };
        public static DateTime DefaultLinuxUtc => new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    }
}