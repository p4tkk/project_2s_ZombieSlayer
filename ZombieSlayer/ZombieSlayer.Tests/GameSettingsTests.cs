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
    }
}
