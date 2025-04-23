using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ZombieSlayer
{
    public partial class Form1 : Form
    {
        #region Game State Classes
        private class GameState
        {
            public bool GoLeft;
            public bool GoRight;
            public bool GoUp;
            public bool GoDown;
            public bool GameOver;
            public string Facing = "up";
            public int PlayerHealth = 100;
            public int Ammo = 10;
            public int Score = 0;
            public bool HealingBonusActive = false;
        }

        private class GameSettings
        {
            public readonly int PlayerSpeed = 10;
            public readonly int NormalZombieSpeed = 3;
            public readonly int BigZombieSpeed = 2;
            public readonly int SmallZombieSpeed = 5;
            public readonly int BigZombieHealth = 3;
            public readonly int SmallZombieHealth = 1;
            public readonly int ZombieSpawnCount = 3;
            public readonly int AmmoDropAmount = 5;
            public readonly int HealAmount = 20;
        }
        #endregion

        #region Zombie System
        private enum ZombieType { Normal, Big, Small }

        private class Zombie
        {
            public PictureBox PictureBox;
            public int Health;
            public int Speed;
            public string Tag;
            public int HitsTaken;
        }

        private Dictionary<ZombieType, Func<Zombie>> _zombieFactories;
        private List<Zombie> _zombies;
        #endregion

        #region Game Objects
        private GameState _state;
        private GameSettings _settings;
        private Random _rnd;
        #endregion

        public Form1()
        {
            InitializeComponent();
            InitializeGameSystems();
            InitializeGame();
        }

        private void InitializeGameSystems()
        {
            _state = new GameState();
            _settings = new GameSettings();
            _rnd = new Random();
            _zombies = new List<Zombie>();

            _zombieFactories = new Dictionary<ZombieType, Func<Zombie>>();
            _zombieFactories.Add(ZombieType.Normal, () => new Zombie
            {
                Health = 1,
                Speed = 3,
                Tag = "zombie",
                PictureBox = new PictureBox
                {
                    Size = new Size(80, 80),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = Properties.Resources.zDown
                }
            });

            _zombieFactories.Add(ZombieType.Big, () => new Zombie
            {
                Health = 3,
                Speed = 2,
                Tag = "bigZombie",
                HitsTaken = 0,
                PictureBox = new PictureBox
                {
                    Size = new Size(110, 110),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = Properties.Resources.BzDown
                }
            });

            _zombieFactories.Add(ZombieType.Small, () => new Zombie
            {
                Health = 1,
                Speed = 5,
                Tag = "smallZombie",
                PictureBox = new PictureBox
                {
                    Size = new Size(70, 70),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = Properties.Resources.SzDown
                }
            });
        }

        private void InitializeGame()
        {
            this.KeyPreview = true;
            player.Image = Properties.Resources.pUp;
            player.BackColor = Color.Transparent;
            RestartGame();
        }

        private void RestartGame()
        {
            foreach (var zombie in _zombies.ToArray())
            {
                RemoveZombie(zombie);
            }

            foreach (Control control in Controls.OfType<PictureBox>().ToList())
            {
                if (control.Tag?.ToString() == "healing" ||
                    control.Tag?.ToString() == "ammo" ||
                    control.Tag?.ToString() == "bullet")
                {
                    Controls.Remove(control);
                    control.Dispose();
                }
            }

            _state.PlayerHealth = 100;
            _state.Ammo = 10;
            _state.Score = 0;
            _state.GameOver = false;
            _state.HealingBonusActive = false;
            _state.GoLeft = false;
            _state.GoRight = false;
            _state.GoUp = false;
            _state.GoDown = false;
            _state.Facing = "up";

            player.Image = Properties.Resources.pUp;
            HealthBar.Value = 100;
            txtAmmo.Text = "Ammo: 10";
            txtScore.Text = "Kills: 0";

            for (int i = 0; i < _settings.ZombieSpawnCount; i++)
            {
                SpawnZombie();
            }

            GameTimer.Start();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (_state.GameOver)
                return;

            _state.PlayerHealth = Math.Max(0, _state.PlayerHealth);
            HealthBar.Value = _state.PlayerHealth;

            if (_state.PlayerHealth <= 0)
            {
                GameOver();
                return;
            }

            UpdatePlayer();
            UpdateZombies();
            UpdateBullets();
            CheckCollisions();
            UpdateBonuses();
            UpdateUI();
        }

        private void UpdatePlayer()
        {
            if (_state.GoLeft && player.Left > 0)
            {
                player.Left -= _settings.PlayerSpeed;
            }
            if (_state.GoRight && player.Right < ClientSize.Width)
            {
                player.Left += _settings.PlayerSpeed;
            }
            if (_state.GoUp && player.Top > 47)
            {
                player.Top -= _settings.PlayerSpeed;
            }
            if (_state.GoDown && player.Bottom < ClientSize.Height)
            {
                player.Top += _settings.PlayerSpeed;
            }
        }

        private void UpdateUI()
        {
            HealthBar.Value = _state.PlayerHealth;
            txtAmmo.Text = "Ammo: " + _state.Ammo;
            txtScore.Text = "Kills: " + _state.Score;
        }

        private void SpawnZombie()
        {
            ZombieType zombieType = GetRandomZombieType();
            Zombie zombie = _zombieFactories[zombieType]();

            zombie.PictureBox.Tag = zombie.Tag;
            zombie.PictureBox.Left = _rnd.Next(0, ClientSize.Width - zombie.PictureBox.Width);
            zombie.PictureBox.Top = _rnd.Next(50, ClientSize.Height - zombie.PictureBox.Height);

            _zombies.Add(zombie);
            Controls.Add(zombie.PictureBox);
            player.BringToFront();
        }

        private ZombieType GetRandomZombieType()
        {
            int chance = _rnd.Next(100);
            if (chance < 50)
                return ZombieType.Normal;
            else if (chance < 75)
                return ZombieType.Big;
            else
                return ZombieType.Small;
        }

        private void UpdateZombies()
        {
            foreach (Zombie zombie in _zombies.ToArray())
            {
                if (zombie.PictureBox.Left > player.Left)
                {
                    zombie.PictureBox.Left -= zombie.Speed;
                }
                if (zombie.PictureBox.Right < player.Right)
                {
                    zombie.PictureBox.Left += zombie.Speed;
                }
                if (zombie.PictureBox.Top > player.Top)
                {
                    zombie.PictureBox.Top -= zombie.Speed;
                }
                if (zombie.PictureBox.Bottom < player.Bottom)
                {
                    zombie.PictureBox.Top += zombie.Speed;
                }

                SetZombieImage(zombie);
            }
        }

        private void SetZombieImage(Zombie zombie)
        {
            bool movingLeft = zombie.PictureBox.Left > player.Left;
            bool movingUp = zombie.PictureBox.Top > player.Top;

            if (movingLeft && Math.Abs(zombie.PictureBox.Top - player.Top) < 20)
            {
                zombie.PictureBox.Image = GetZombieImage(zombie, "left");
            }
            else if (!movingLeft && Math.Abs(zombie.PictureBox.Top - player.Top) < 20)
            {
                zombie.PictureBox.Image = GetZombieImage(zombie, "right");
            }
            else if (movingUp)
            {
                zombie.PictureBox.Image = GetZombieImage(zombie, "up");
            }
            else
            {
                zombie.PictureBox.Image = GetZombieImage(zombie, "down");
            }
        }

        private Image GetZombieImage(Zombie zombie, string direction)
        {
            if (zombie.Tag == "bigZombie")
            {
                switch (direction)
                {
                    case "left": return Properties.Resources.BzLeft;
                    case "right": return Properties.Resources.BzRight;
                    case "up": return Properties.Resources.BzUp;
                    default: return Properties.Resources.BzDown;
                }
            }
            else if (zombie.Tag == "smallZombie")
            {
                switch (direction)
                {
                    case "left": return Properties.Resources.SzLeft;
                    case "right": return Properties.Resources.SzRight;
                    case "up": return Properties.Resources.SzUp;
                    default: return Properties.Resources.SzDown;
                }
            }
            else
            {
                switch (direction)
                {
                    case "left": return Properties.Resources.zLeft;
                    case "right": return Properties.Resources.zRight;
                    case "up": return Properties.Resources.zUp;
                    default: return Properties.Resources.zDown;
                }
            }
        }

        private void RemoveZombie(Zombie zombie)
        {
            Controls.Remove(zombie.PictureBox);
            zombie.PictureBox.Dispose();
            _zombies.Remove(zombie);
        }

        private void UpdateBullets()
        {
            // Bullets are updated via their individual timers
        }

        private void CheckCollisions()
        {
            foreach (Zombie zombie in _zombies.ToArray())
            {
                if (player.Bounds.IntersectsWith(zombie.PictureBox.Bounds))
                {
                    int damage = zombie.Tag == "bigZombie" ? 2 : 1;
                    _state.PlayerHealth = Math.Max(0, _state.PlayerHealth - damage);

                    if (_state.PlayerHealth <= 0)
                    {
                        GameOver();
                        return;
                    }
                }
            }

            foreach (Control bullet in Controls.OfType<PictureBox>().ToList())
            {
                if (bullet.Tag?.ToString() == "bullet")
                {
                    foreach (Zombie zombie in _zombies.ToArray())
                    {
                        if (bullet.Bounds.IntersectsWith(zombie.PictureBox.Bounds))
                        {
                            Controls.Remove(bullet);
                            bullet.Dispose();

                            if (zombie.Tag == "bigZombie")
                            {
                                zombie.HitsTaken++;
                                if (zombie.HitsTaken >= zombie.Health)
                                {
                                    RemoveZombie(zombie);
                                    _state.Score++;
                                    SpawnZombie();
                                }
                            }
                            else
                            {
                                RemoveZombie(zombie);
                                _state.Score++;
                                SpawnZombie();
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateBonuses()
        {
            if (_state.PlayerHealth < 10 && !_state.HealingBonusActive)
            {
                SpawnHealingBonus();
                _state.HealingBonusActive = true;
            }

            foreach (Control bonus in Controls.OfType<PictureBox>().ToList())
            {
                if (bonus.Tag?.ToString() == "healing" && player.Bounds.IntersectsWith(bonus.Bounds))
                {
                    _state.PlayerHealth = Math.Min(100, _state.PlayerHealth + _settings.HealAmount);
                    _state.HealingBonusActive = false;
                    Controls.Remove(bonus);
                    bonus.Dispose();
                }
                else if (bonus.Tag?.ToString() == "ammo" && player.Bounds.IntersectsWith(bonus.Bounds))
                {
                    _state.Ammo += _settings.AmmoDropAmount;
                    Controls.Remove(bonus);
                    bonus.Dispose();
                }
            }
        }

        private void SpawnHealingBonus()
        {
            PictureBox healing = new PictureBox
            {
                Image = Properties.Resources.Heal,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = "healing",
                Left = _rnd.Next(10, ClientSize.Width - 40),
                Top = _rnd.Next(60, ClientSize.Height - 40)
            };
            Controls.Add(healing);
            healing.BringToFront();
        }

        private void SpawnAmmoBonus()
        {
            PictureBox ammo = new PictureBox
            {
                Image = Properties.Resources.Ammo,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = "ammo",
                Left = _rnd.Next(10, ClientSize.Width - 40),
                Top = _rnd.Next(60, ClientSize.Height - 40)
            };
            Controls.Add(ammo);
            ammo.BringToFront();
        }

        private void ShootBullet(string direction)
        {
            if (_state.Ammo <= 0)
                return;

            _state.Ammo--;
            PictureBox bullet = new PictureBox
            {
                Image = CreateBulletImage(),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = "bullet",
                Left = player.Left + (player.Width / 2),
                Top = player.Top + (player.Height / 2)
            };

            Controls.Add(bullet);
            bullet.BringToFront();

            Timer timer = new Timer { Interval = 20 };
            timer.Tick += (s, e) =>
            {
                switch (direction)
                {
                    case "left":
                        bullet.Left -= 20;
                        break;
                    case "right":
                        bullet.Left += 20;
                        break;
                    case "up":
                        bullet.Top -= 20;
                        break;
                    case "down":
                        bullet.Top += 20;
                        break;
                }

                if (bullet.Left < 0 || bullet.Left > ClientSize.Width ||
                    bullet.Top < 0 || bullet.Top > ClientSize.Height)
                {
                    Controls.Remove(bullet);
                    bullet.Dispose();
                    timer.Stop();
                }
            };
            timer.Start();

            if (_state.Ammo == 0)
            {
                SpawnAmmoBonus();
            }
        }

        private Bitmap CreateBulletImage()
        {
            Bitmap bmp = new Bitmap(10, 10);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(Brushes.Red, 0, 0, 10, 10);
                g.DrawEllipse(Pens.DarkRed, 0, 0, 10, 10);
            }
            return bmp;
        }

        private void GameOver()
        {
            _state.GameOver = true;
            GameTimer.Stop();
            player.Image = Properties.Resources.DeadPlayer;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (_state.GameOver)
                return;

            switch (e.KeyCode)
            {
                case Keys.A:
                    _state.GoLeft = true;
                    _state.Facing = "left";
                    player.Image = Properties.Resources.pLeft;
                    break;
                case Keys.D:
                    _state.GoRight = true;
                    _state.Facing = "right";
                    player.Image = Properties.Resources.pRight;
                    break;
                case Keys.W:
                    _state.GoUp = true;
                    _state.Facing = "up";
                    player.Image = Properties.Resources.pUp;
                    break;
                case Keys.S:
                    _state.GoDown = true;
                    _state.Facing = "down";
                    player.Image = Properties.Resources.pDown;
                    break;
                case Keys.Space:
                    if (_state.Ammo > 0)
                    {
                        ShootBullet(_state.Facing);
                    }
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (_state.GameOver)
                    {
                        RestartGame();
                    }
                    break;
                case Keys.Escape:
                    TogglePause();
                    break;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    _state.GoLeft = false;
                    break;
                case Keys.D:
                    _state.GoRight = false;
                    break;
                case Keys.W:
                    _state.GoUp = false;
                    break;
                case Keys.S:
                    _state.GoDown = false;
                    break;
            }
        }

        private void TogglePause()
        {
            if (_state.GameOver)
                return;

            if (GameTimer.Enabled)
            {
                GameTimer.Stop();
                btnPause.Text = "Resume";
            }
            else
            {
                GameTimer.Start();
                btnPause.Text = "Pause";
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            TogglePause();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            GameMenu menu = new GameMenu();
            menu.Show();
            this.Hide();
        }
    }
}
