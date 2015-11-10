using System;

namespace Project_B.CodeServerSide.BrokerProvider {
    public class DateTimeToGmtFixer {
        private readonly short _deltaFromGmt;

        public DateTimeToGmtFixer(short deltaFromGmt) {
            _deltaFromGmt = deltaFromGmt;
        }

        public DateTime FixToGmt(DateTime dateTime) {
            return dateTime != DateTime.MinValue ? dateTime.AddHours(-_deltaFromGmt) : dateTime;
        }
    }
}