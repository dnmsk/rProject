using MainLogic.Consts;
using NUnit.Framework;

namespace UnitTestProject.Unit {
    [TestFixture]
    public class DbRestrictionsTest {
        [TestCase(15, DbRestrictions.STRING_FIELD_LENGTH_15)]
        [TestCase(128, DbRestrictions.STRING_FIELD_LENGTH_128)]
        [TestCase(256, DbRestrictions.STRING_FIELD_LENGTH_256)]
        [TestCase(512, DbRestrictions.STRING_FIELD_LENGTH_512)]
        public void StringLengthTest(int expected, int actual) {
            Assert.AreEqual(expected, actual);
        }

        [TestCase(9.9d, 4 ,2, 9.9d)]
        [TestCase(99.9d, 4 ,2, 99.9d)]
        [TestCase(9.99d, 4 ,2, 9.99d)]
        [TestCase(99.99d, 4 ,2, 99.99d)]
        [TestCase(99.99d, 4 ,2, 99.999d)]
        [TestCase(99.99d, 4 ,2, 999.99d)]
        [TestCase(99.99d, 4 ,2, 999.999d)]
        public void RoundDecimalTest(decimal expected, int precision, int scale, decimal number) {
            Assert.AreEqual(expected, DbRestrictions.RoundDecimal(number, precision, scale));
        }
    }
}
