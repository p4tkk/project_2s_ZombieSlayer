using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZombieSlayer
{
    /// <summary>
    /// Представляет пулю, выстреленную игроком, с автоматическим движением и уничтожением при выходе за пределы экрана.
    /// </summary>
    public class Bullet : IDisposable
    {
        public const int Speed = 20;
        private const int BulletSize = 5;

        private readonly PictureBox _bullet = new PictureBox();
        private readonly Timer _bulletTimer = new Timer();
        private bool _disposed;

        /// <summary>
        /// Направление движения пули (left, right, up, down).
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Начальная позиция пули по горизонтали.
        /// </summary>
        public int BulletLeft { get; set; }

        /// <summary>
        /// Начальная позиция пули по вертикали.
        /// </summary>
        public int BulletTop { get; set; }

        /// <summary>
        /// Создаёт пулю и добавляет её на форму, начиная движение.
        /// </summary>
        /// <param name="form">Форма, на которой будет отображаться пуля.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если передана null форма.</exception>
        public void MakeBullet(Form form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            _bullet.BackColor = Color.White;
            _bullet.Size = new Size(BulletSize, BulletSize);
            _bullet.Tag = "bullet";
            _bullet.Left = BulletLeft;
            _bullet.Top = BulletTop;
            _bullet.BringToFront();

            form.Controls.Add(_bullet);

            _bulletTimer.Interval = Speed;
            _bulletTimer.Tick += OnBulletTimerTick;
            _bulletTimer.Start();
        }

        /// <summary>
        /// Обрабатывает событие таймера для передвижения пули.
        /// </summary>
        private void OnBulletTimerTick(object sender, EventArgs e)
        {
            switch (Direction)
            {
                case "left":
                    _bullet.Left -= Speed;
                    break;
                case "right":
                    _bullet.Left += Speed;
                    break;
                case "up":
                    _bullet.Top -= Speed;
                    break;
                case "down":
                    _bullet.Top += Speed;
                    break;
            }

            if (_bullet.Left < 10 || _bullet.Left > 1450 ||
                _bullet.Top < 10 || _bullet.Top > 600)
            {
                Dispose();
            }
        }

        /// <summary>
        /// Освобождает ресурсы, используемые пулей.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SimulateMove()
        {
            OnBulletTimerTick(null, EventArgs.Empty);
        }

        /// <summary>
        /// Защищённая реализация освобождения ресурсов.
        /// </summary>
        /// <param name="disposing">Указывает, нужно ли освобождать управляемые ресурсы.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _bulletTimer?.Stop();
                _bulletTimer?.Dispose();
                _bullet?.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Финализатор, вызываемый при сборке мусора, если Dispose не был вызван явно.
        /// </summary>
        ~Bullet()
        {
            Dispose(false);
        }
    }
}
