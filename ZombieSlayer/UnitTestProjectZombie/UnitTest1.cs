using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using ZombieSlayer;
using System.Drawing;
using System;
using System.Reflection;
using System.Threading;
using System.Linq;

namespace UnitTestProjectZombie
{
    [TestClass]
    public class UnitTest1
    {
        private Bullet _bullet;

        [TestInitialize]
        public void SetUp()
        {
            _bullet = new Bullet();
        }

        [TestMethod]
        public void Constructor_InitializesWithDefaultValues()
        {
            Assert.IsNull(_bullet.Direction, "Direction должна быть null по умолчанию.");
            Assert.AreEqual(0, _bullet.BulletLeft, "BulletLeft должен быть 0 по умолчанию.");
            Assert.AreEqual(0, _bullet.BulletTop, "BulletTop должен быть 0 по умолчанию.");
        }
    }
}
