using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Drawing;
using System.Reflection;
using ZombieSlayer;

namespace ZombieSlayer.Tests
{
    [TestClass]
    public class Form1Tests
    {
        private Form1 _form;
        private Mock<Random> _randomMock;

        [TestInitialize]
        public void Setup()
        {
            _form = new Form1();
            _randomMock = new Mock<Random>();

            // Устанавливаем ClientSize для формы
            typeof(Form1).GetProperty("ClientSize")?.SetValue(_form, new Size(800, 600));

            // Подменяем Random через рефлексию
            typeof(Form1).GetField("_rnd", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_form, _randomMock.Object);
        }

        [TestMethod]
        public void SpawnHealingBonus_SetsCoordinatesWithinBounds()
        {
            // Arrange
            _randomMock.SetupSequence(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(10)  // Left
                .Returns(60); // Top

            // Act
            InvokeSpawnHealingBonus();

            // Assert
            _randomMock.Verify(r => r.Next(10, 760), Times.Once());
            _randomMock.Verify(r => r.Next(60, 560), Times.Once());
        }

        private void InvokeSpawnHealingBonus()
        {
            var methodInfo = typeof(Form1).GetMethod("SpawnHealingBonus", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo?.Invoke(_form, null);
        }
    }
}