using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombieSlayer
{
    public partial class GameMenu: Form
    {
        public GameMenu()
        {
            InitializeComponent();
            btnStart.Location = new Point(
                (this.ClientSize.Width - btnStart.Width) / 2, 200
            );
            btnExit.Location = new Point(
                (this.ClientSize.Width - btnExit.Width) / 2, 256
            );
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Form1 game = new Form1();
            game.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
