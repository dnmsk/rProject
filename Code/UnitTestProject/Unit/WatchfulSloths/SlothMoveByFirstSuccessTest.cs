using CommonUtils.WatchfulSloths.SlothMoveRules;
using NUnit.Framework;

namespace UnitTestProject.Unit.WatchfulSloths {
    /// <summary>
    /// Тест на правило движений ленивца до первого успешного результата.
    /// </summary>
    [TestFixture]
    public class SlothMoveByFirstSuccessTest {
        /// <summary>
        /// Если еще не двигались, то движение необходимо, даже если время не пришло.
        /// </summary>
        [Test]
        public void NeedMoveFirstTimeTest() {
            // NOTE : Arrange.
            var m = new SlothMoveByFirstSuccess<object>(() => null, default(int));

            // NOTE : Act.
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsTrue(isneed);
        }
        
        /// <summary>
        /// Если раз получили, то больше и не нужно.
        /// </summary>
        [Test]
        public void NotNeedMoveAfterSuccessTest() {
            // NOTE : Arrange.
            var m = new SlothMoveByFirstSuccess<object>(() => null, default(int));
            m.Move(0);

            // NOTE : Act.
            var isneed = m.IsNeedMove;

            // NOTE : Assert.
            Assert.IsFalse(isneed);
        }
    }
}