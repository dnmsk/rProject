using System;
using CommonUtils.WatchfulSloths;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using Moq;
using NUnit.Framework;

namespace UnitTestProject.Unit.WatchfulSloths {
    /// <summary>
    /// Тест на правило движений ленивца по времени с сохраненным результатом.
    /// </summary>
    [TestFixture]
    public class SlothMoveByTimeTest {
        /// <summary>
        /// Если еще не двигались, то движение необходимо, даже если время не пришло.
        /// </summary>
        [Test]
        public void NeedMoveFirstTimeTest() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 00, 00));
            var m = new SlothMoveByTime<int>(() => default(int), new TimeSpan(100, 0, 0), default(int), mock.Object);

            // NOTE : Act.
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsTrue(isneed);
        }

        /// <summary>
        /// Проверка движения по прошедшему времени.
        /// </summary>
        [Test]
        public void MoveByTimeTest() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 00, 00));
            const int INTERVAL = 10;
            var m = new SlothMoveByTime<object>(() => null, new TimeSpan(0, INTERVAL, 0), default(int), mock.Object);
            m.Move(0);

            // NOTE : Act.
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, INTERVAL + 1, 00));
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsTrue(isneed, "К этому моменту по правилу уже нужно снова двигаться.");
        }
    }
}