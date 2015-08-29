using System;

namespace MainLogic.Consts {
    public static class DbRestrictions {
        public const int STRING_FIELD_LENGTH_15 = 15;
        public const int STRING_FIELD_LENGTH_128 = 128;
        public const int STRING_FIELD_LENGTH_256 = 256;
        public const int STRING_FIELD_LENGTH_512 = 512;

        public static decimal RoundDecimal(decimal number, int precision, int scale) {
            var precisionPow = (int) Math.Pow(10, precision);
            var scalePow = (int) Math.Pow(10, scale);
            var val = (long) (number*scalePow);
            return (val % precisionPow) / (decimal) scalePow;
        }
    }
}
