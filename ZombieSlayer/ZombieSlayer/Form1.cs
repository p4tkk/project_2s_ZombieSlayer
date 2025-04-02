using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombieSlayer
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int score = 0;
        int zombieSpeed = 3;
        int bigZombieSpeed = 2; // Большие зомби медленнее
        int smallZombieSpeed = 5; // Маленькие зомби быстрее
        int bigZombieHealth = 3; // Нужно попасть 3 раза, чтобы убить
        int smallZombieHealth = 1; // Маленькие умирают с одного попадания
        Random rnd = new Random();
        List<PictureBox> zombiesList = new List<PictureBox>();

        // Перечисление для типов зомби
        private enum ZombieType
        {
            Normal,
            Big,
            Small
        }

        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth >= 1)
            {
                HealthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.BackColor = Color.Red;
                GameTimer.Stop();
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;

            if (goLeft && player.Left > 0)
            {
                player.Left -= speed;
            }
            if (goRight && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }
            if (goUp && player.Top > 47)
            {
                player.Top -= speed;
            }
            if (goDown && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if (x is PictureBox && x.Tag is string zombieTag && (zombieTag == "zombie" || zombieTag == "bigZombie" || zombieTag == "smallZombie"))
                {
                    // Определяем скорость и урон в зависимости от типа зомби
                    int currentZombieSpeed = zombieSpeed;
                    int damage = 1;

                    if (zombieTag == "bigZombie")
                    {
                        currentZombieSpeed = bigZombieSpeed;
                        damage = 2;
                    }
                    else if (zombieTag == "smallZombie")
                    {
                        currentZombieSpeed = smallZombieSpeed;
                        damage = 1;
                    }

                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= damage;
                    }

                    // Двигаем зомби по направлению к игроку
                    if (x.Left > player.Left)
                    {
                        x.Left -= currentZombieSpeed;
                        SetZombieImage((PictureBox)x, "left");
                    }
                    if (x.Left < player.Left)
                    {
                        x.Left += currentZombieSpeed;
                        SetZombieImage((PictureBox)x, "right");
                    }
                    if (x.Top > player.Top)
                    {
                        x.Top -= currentZombieSpeed;
                        SetZombieImage((PictureBox)x, "up");
                    }
                    if (x.Top < player.Top)
                    {
                        x.Top += currentZombieSpeed;
                        SetZombieImage((PictureBox)x, "down");
                    }
                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && x.Tag is string bulletTargetTag &&
                        (bulletTargetTag == "zombie" || bulletTargetTag == "bigZombie" || bulletTargetTag == "smallZombie"))
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            // Удаляем пулю
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();

                            // Обработка попадания в зависимости от типа зомби
                            bool zombieKilled = false;

                            if (bulletTargetTag == "bigZombie")
                            {
                                if (x is PictureBox pb && pb.Tag is "bigZombie")
                                {
                                    if (pb.Name.StartsWith("hits:"))
                                    {
                                        int hits = int.Parse(pb.Name.Split(':')[1]);
                                        hits++;
                                        pb.Name = $"hits:{hits}";
                                        if (hits >= bigZombieHealth)
                                        {
                                            zombieKilled = true;
                                        }
                                    }
                                    else
                                    {
                                        pb.Name = "hits:1";
                                    }
                                }
                            }
                            else if (bulletTargetTag == "smallZombie")
                            {
                                zombieKilled = true; // Маленькие умирают с одного попадания
                            }
                            else // Обычный зомби
                            {
                                zombieKilled = true;
                            }

                            if (zombieKilled)
                            {
                                RemoveZombie(x);
                                score++;
                                MakeZombies();
                            }
                        }
                    }
                }
            }
        }
        // Новый метод для установки изображения зомби в зависимости от типа и направления
        private void SetZombieImage(PictureBox zombie, string direction)
        {
            string tag = zombie.Tag as string;

            if (tag == "bigZombie")
            {
                switch (direction)
                {
                  
                }
            }
            else if (tag == "smallZombie")
            {
                switch (direction)
                {
                   
                }
            }
            else // Обычный зомби
            {
                switch (direction)
                {
                    case "left": zombie.Image = Properties.Resources.zLeft; break;
                    case "right": zombie.Image = Properties.Resources.zRight; break;
                    case "up": zombie.Image = Properties.Resources.zUp; break;
                    case "down": zombie.Image = Properties.Resources.zDown; break;
                }
            }
        }

        // Метод для удаления зомби
        private void RemoveZombie(Control zombie)
        {
            this.Controls.Remove(zombie);
            ((PictureBox)zombie).Dispose();
            zombiesList.Remove((PictureBox)zombie);
        }

        // Модифицированный метод создания зомби с разными вероятностями
        private void MakeZombies()
        {
            // Генерируем случайное число для определения типа зомби
            int chance = rnd.Next(100);
            ZombieType zombieType;

            if (chance < 50) // 65% обычный
            {
                zombieType = ZombieType.Normal;
            }
            else if (chance < 75) // 20% большой (65+20=85)
            {
                zombieType = ZombieType.Big;
            }
            else // 15% маленький (остаток)
            {
                zombieType = ZombieType.Small;
            }

            PictureBox zombie = new PictureBox();

            // Настраиваем зомби в зависимости от типа
            switch (zombieType)
            {
                case ZombieType.Big:
                    zombie.Tag = "bigZombie";
                    zombie.BackColor = Color.White;
                    zombie.Size = new Size(110, 110); // Больший размер
                    break;

                case ZombieType.Small:
                    zombie.Tag = "smallZombie";
                    zombie.BackColor = Color.White;
                    zombie.Size = new Size(50, 50); // Меньший размер
                    break;

                default: // Normal
                    zombie.Tag = "zombie";
                    zombie.Image = Properties.Resources.zDown;
                    zombie.SizeMode = PictureBoxSizeMode.AutoSize;
                    break;
            }

            zombie.Left = rnd.Next(0, 900);
            zombie.Top = rnd.Next(0, 800);
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
        }

        // Остальные методы остаются без изменений
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.pLeft;
            }
            if (e.KeyCode == Keys.D)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.pRight;
            }
            if (e.KeyCode == Keys.W)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.pUp;
            }
            if (e.KeyCode == Keys.S)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.pDown;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.D)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.W)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.S)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);

                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }
        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox
            {
                BackColor = Color.White,
                Size = new Size(30, 30),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = "ammo"
            };

            ammo.Left = rnd.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = rnd.Next(60, this.ClientSize.Height - ammo.Height);

            this.Controls.Add(ammo);
            ammo.BringToFront();
            player.BringToFront();
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.pUp;
            player.BackColor = Color.Transparent;

            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goLeft = false;
            goDown = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;

            GameTimer.Start();
        }
    }
}