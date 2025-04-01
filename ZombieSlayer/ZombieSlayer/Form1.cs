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
        Random rnd = new Random();
        List<PictureBox> zombiesList = new List<PictureBox>();
        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if(playerHealth >= 1)
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
            if(goLeft && player.Left > 0)
            {
                player.Left -= speed;
            }
            if(goRight && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }
            if(goUp && player.Top > 47)
            {
                player.Top -= speed;
            }
            if (goDown && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                if(x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if(x is PictureBox && x.Tag is string tag && tag == "zombie")
                {

                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                    }
                    // Двигаем зомби по направлению к игроку
                    if (x.Left > player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zLeft;
                        
                    }
                    if (x.Left < player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zRight;

                    }
                    if (x.Top > player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zUp;

                    }
                    if (x.Top < player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zDown;

                    }

                }

                foreach(Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        // Проверяем столкновение пули и зомби
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            // Удаляем пулю и зомби из формы и списка
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose(); 

                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();

                            zombiesList.Remove((PictureBox)x);

                            // Создаем нового зомби
                            MakeBigZombies();

                        }
                    }
                }

            }


        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.A)
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

            if(e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);

                if(ammo < 1)
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
        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Image = Properties.Resources.zDown;
            zombie.Tag = "zombie";
            zombie.Left = rnd.Next(0,900);
            zombie.Top = rnd.Next(0,800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
        }

        private void MakeBigZombies()
        {
            PictureBox zombieB = new PictureBox();
            zombieB.Size = new Size(100, 100);
            zombieB.Tag = "zombie";
            zombieB.Left = rnd.Next(0, 900);
            zombieB.Top = rnd.Next(0, 800);
            zombieB.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombieB);
            this.Controls.Add(zombieB);
            player.BringToFront();
        }

        private void DropAmmo()
        {
            // Создаем объект патрона и настраиваем его параметры
            PictureBox ammo = new PictureBox
            {
                BackColor = Color.White,
                Size = new Size(30, 30),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = "ammo"
            };

            // Случайное позиционирование патрона на форме
            ammo.Left = rnd.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = rnd.Next(60, this.ClientSize.Height - ammo.Height);

            // Добавляем патрон на форму и корректируем порядок отображения
            this.Controls.Add(ammo);
            ammo.BringToFront();
            player.BringToFront();
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.pUp;
            player.BackColor = Color.Transparent;

            foreach(PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();

            for(int i = 0; i < 3; i++)
            {
                MakeBigZombies();
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
