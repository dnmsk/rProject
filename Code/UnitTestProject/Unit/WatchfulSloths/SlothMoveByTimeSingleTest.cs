using System;
using System.Threading;
using CommonUtils.WatchfulSloths;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using Moq;
using NUnit.Framework;

namespace UnitTestProject.Unit.WatchfulSloths {
    [TestFixture]
    public class SlothMoveByTimeSingleTest {
        /// <summary>
        /// Первый запуск должен состояться
        /// </summary>
        [Test]
        public void NeedMoveFirstTimeTest() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22));
            var m = new SlothMoveByTimeSingle<object>(() => null, new TimeSpan(100, 0, 0), default(int), mock.Object);

            // NOTE : Act.
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsTrue(isneed);
        }

        /// <summary>
        /// Пока идет движение, запуск не возможен.
        /// </summary>
        [Test]
        public void NoNeedMoveWhileMoving() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22));
            bool needStop = false;
            var m = new SlothMoveByTimeSingle<object>(() => {
                while (!needStop) {
                    Thread.Sleep(1);
                }
                return null;
            }, new TimeSpan(100, 0, 0), default(int), mock.Object);
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 02, 22));
            
            var thread = new Thread(m.Move);
            thread.Start();
            Thread.Sleep(1);
            
            // NOTE : Act.
            var isneed = m.IsNeedMove;
            needStop = true;

            // NOTE : Assert.
            Assert.IsFalse(isneed);
        }

        /// <summary>
        /// После выполнения таска, запуск возможен.
        /// </summary>
        [Test]
        public void NeedMoveAfterStop() {
            // NOTE : Arrange.
            var mock = new Mock<IWatch>();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 01, 22));
            var m = new SlothMoveByTimeSingle<object>(() => null, new TimeSpan(100, 0, 0), default(int), mock.Object);
            m.Move();
            mock.Setup(w => w.Now()).Returns(new DateTime(2013, 02, 22));
            
            // NOTE : Act.
            var isneed = m.IsNeedMove;
            
            // NOTE : Assert.
            Assert.IsTrue(isneed);
        }
    }
}
