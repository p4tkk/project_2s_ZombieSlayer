using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZombieSlayer;

namespace ZombieSlayer.Tests
{
    [TestClass]
    public class ZombieFactoryTests
    {
        [TestMethod]
        public void NormalZombieFactory_CreatesZombie_WithCorrectStats()
        {
            // Arrange
            var factory = new Form1.NormalZombieFactory(); // Явное указание класса

            // Act
            var zombie = factory.CreateZombie();

            // Assert
            Assert.IsNotNull(zombie, "Зомби не был создан");
            Assert.AreEqual(1, zombie.Health, "Некорректное здоровье зомби");
            Assert.AreEqual(3, zombie.Speed, "Некорректная скорость зомби");
            Assert.AreEqual("zombie", zombie.Tag, "Некорректный тег зомби");
        }
    }
}