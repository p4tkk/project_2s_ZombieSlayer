using ZombieSlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBulletProject
{
    [TestClass]
    public class TestBulletClass : PageTest
    {
        [TestMethod]
        public void Constructor_InitializesWithDefaultValues()
        {
            Bullet _bullet = new Bullet();
            Assert.IsNull(_bullet.Direction);
            Assert.AreEqual(0, _bullet.BulletLeft);
            Assert.AreEqual(0, _bullet.BulletTop);
        }


    }
}
