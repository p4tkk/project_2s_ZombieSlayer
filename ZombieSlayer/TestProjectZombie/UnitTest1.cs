using ZombieSlayer;

namespace TestProjectZombie
{
    [TestClass]
    public class UnitTest1
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