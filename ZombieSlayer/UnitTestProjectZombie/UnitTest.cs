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
    public class UnitTest
    {
        private Bullet _bullet;
        private Form _testForm;

        [TestInitialize]
        public void SetUp()
        {
            _bullet = new Bullet();
            _testForm = new Form();
        }

        [TestMethod]
        public void Constructor_InitializesWithDefaultValues()
        {
            Assert.IsNull(_bullet.Direction, "Direction должна быть null по умолчанию.");
            Assert.AreEqual(0, _bullet.BulletLeft, "BulletLeft должен быть 0 по умолчанию.");
            Assert.AreEqual(0, _bullet.BulletTop, "BulletTop должен быть 0 по умолчанию.");
        }

        [TestMethod]
        public void MakeBullet_AddsBulletToForm()
        {
            _bullet.Direction = "right";
            _bullet.BulletLeft = 100;
            _bullet.BulletTop = 100;

            _bullet.MakeBullet(_testForm);

            Assert.AreEqual(1, _testForm.Controls.Count, "На форму должна быть добавлена ровно одна пуля.");
            Assert.AreEqual("bullet", _testForm.Controls[0].Tag, "Tag пули должен быть 'bullet'.");
            Assert.AreEqual(Color.White, _testForm.Controls[0].BackColor, "Цвет пули должен быть белым.");
            Assert.AreEqual(new Size(5, 5), _testForm.Controls[0].Size, "Размер пули должен быть 5x5.");
            Assert.AreEqual(100, _testForm.Controls[0].Left, "Координата Left должна соответствовать BulletLeft.");
            Assert.AreEqual(100, _testForm.Controls[0].Top, "Координата Top должна соответствовать BulletTop.");
        }
    }
}
