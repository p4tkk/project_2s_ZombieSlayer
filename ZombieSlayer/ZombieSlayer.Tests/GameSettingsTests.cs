using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using ZombieSlayer.Core;

namespace ZombieSlayer.Tests
{
    [TestClass]
    public class GameSettingsTests
    {
        [TestMethod]
        public void Instance_ThreadSafe_ReturnsSameObject()
        {
            GameSettings instance = null;
            Parallel.For(0, 100, _ => instance = GameSettings.Instance);
            Assert.AreSame(GameSettings.Instance, instance);
        }
        [TestMethod]
        public void GameSpeeds_AreBalanced()
        {
            // Arrange
            var settings = GameSettings.Instance;

            // Act & Assert для игрока
            int playerSpeed = settings.PlayerSpeed;
            Assert.AreEqual(10, playerSpeed,
                "Скорость игрока должна быть 10 для правильного баланса");

            // Act & Assert для зомби
            Assert.AreEqual(3, settings.NormalZombieSpeed,
                "Обычные зомби должны быть медленнее игрока (speed=3)");
            Assert.AreEqual(5, settings.SmallZombieSpeed,
                "Маленькие зомби должны быть быстрее обычных (speed=5)");
            Assert.AreEqual(2, settings.BigZombieSpeed,
                "Большие зомби должны быть самыми медленными (speed=2)");

            // Проверка баланса между персонажами
            Assert.IsTrue(playerSpeed > settings.NormalZombieSpeed,
                "Игрок должен быть быстрее обычных зомби");
            Assert.IsTrue(settings.SmallZombieSpeed > settings.NormalZombieSpeed,
                "Маленькие зомби должны быть быстрее обычных");
        }
    }
}
