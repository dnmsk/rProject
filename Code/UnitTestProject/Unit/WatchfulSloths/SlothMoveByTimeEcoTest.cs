using System;
using CommonUtils.WatchfulSloths;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using Moq;
using NUnit.Framework;

namespace UnitTestProject.Unit.WatchfulSloths {
    /// <summary>
    /// Тест на правило движений ленивца по времени с приостановлением обновления.
    /// </summary>
    [TestFixture]
    public class SlothMoveByTimeEcoTest {
        /// <summary>
        /// Если еще не двигались, то движение необходимо, даже если время не пришло.
        /// </summary>
        [Test]
        public void NeedMoveFirstTimeTest() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22));
            var m = new SlothMoveByTimeEco<object>(() => null, new TimeSpan(100, 0, 0), default(int), mock.Object);

            // NOTE : Act.
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsTrue(isneed);
        }

        /// <summary>
        /// Проверка движения по прошедшему времени с учетом последнего запроса.
        /// - Двигаться нужно, т.к. хит был в период ожидания.
        /// </summary>
        [Test]
        public void MoveByTimeTest() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 00, 00));
            var m = new SlothMoveByTimeEco<object>(() => null, new TimeSpan(0, 20, 0), default(int), mock.Object);
            m.Move(0);

            // NOTE : Act.
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 10, 00));
            var r = m.Result;
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 25, 00));
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsTrue(isneed, "Двигаться нужно, т.к. был хит.");
        }

        /// <summary>
        /// Проверка движения по прошедшему времени с учетом последнего запроса.
        /// - Двигаться нет необходимости, т.к. хитов не было.
        /// </summary>
        [Test]
        public void MoveByTimeIgnoreTest() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 00, 00));
            const int INTERVAL = 5;
            var m = new SlothMoveByTimeEco<object>(() => null, new TimeSpan(0, INTERVAL, 0), default(int), mock.Object);
            m.Move(0);

            // NOTE : Act.
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, INTERVAL * 2, 00));
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsFalse(isneed, "Двигаться нет смысла.");
        }

        /// <summary>
        /// Проверка движения по прошедшему времени с учетом последнего запроса.
        /// - Двигаться нет необходимости, т.к. хитов в период ожидания не было.
        /// </summary>
        [Test]
        public void MoveByTimeIgnoreHitTest() {
            // NOTE : Arrange.
            const int INTERVAL = 5;
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, 00, 00));
            var m = new SlothMoveByTimeEco<object>(() => null, new TimeSpan(0, INTERVAL, 0), default(int), mock.Object);
            m.Move(0);

            // NOTE : Act.
            var r = m.Result;
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22, 10, INTERVAL * 2, 00));
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsFalse(isneed, "Двигаться нет смысла.");
        }
    }
}