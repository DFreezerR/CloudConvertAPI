using System;
using System.Drawing;
using System.Windows.Forms;

namespace CloudConvertApp
{
    public partial class FSettings : Form
    {
        private bool isHeaderPressed;
        private Point prevMousePos;

        public FSettings()
        {
            InitializeComponent();
            txtAPIKey.Text = Settings.APIKey;
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            isHeaderPressed = true;
            prevMousePos = e.Location;
        }

        private void pnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (isHeaderPressed)
            {
                Point newMousePos = e.Location;
                var deltaX = newMousePos.X - prevMousePos.X;
                var deltaY = newMousePos.Y - prevMousePos.Y;
                Point newPos = new Point(this.Location.X + deltaX, this.Location.Y + deltaY);
                this.Location = newPos;
                this.Update();
            }
        }

        private void pnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            isHeaderPressed = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void FSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.formMain.Enabled = true;
        }

        private void bntSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.APIKey = txtAPIKey.Text;
            Properties.Settings.Default.Save();
            Settings.Update();
            Program.formMain.logger.Log("API key updated!");
            this.Close();

        }

    }
}