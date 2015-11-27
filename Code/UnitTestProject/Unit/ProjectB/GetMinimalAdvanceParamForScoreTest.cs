using NUnit.Framework;
using Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc;

namespace UnitTestProject.Unit.ProjectB {
    [TestFixture]
    public class GetMinimalAdvanceParamForScoreTest {
        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(0, 0, 2, 0)]
        [TestCase(0, 0, 3, 0)]
        [TestCase(0x01, 0x01, 4, 1)]
        [TestCase(0x01, 0, 0, 1)]
        [TestCase(0x01, 0x01, 0, 1)]
        [TestCase(0x03, 0x01, 0, 2)]
        [TestCase(0x07, 0x03, 0, 3)]
        [TestCase(0x08, 0, 3, 1)]
        [TestCase(0xF8, 0x78, 3, 5)]
        public void GetMinimalAdvanceParamForScore(short expected, short initial, short score1, short score2) {
            Assert.AreEqual(expected, TennisLiveResultProcessor.GetMinimalAdvanceParamForScore(initial, score1, score2));
        }
    }
}
