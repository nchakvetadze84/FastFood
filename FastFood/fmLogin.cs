using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;

namespace FastFood
{
    public partial class fmLogin : Form
    {
        public DialogResult Result = DialogResult.None;

        

        public fmLogin()
        {
            InitializeComponent();
        }

        public void Show()
        {
            this.ShowDialog();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            cbUsers.DataSource = DBObject.InvokeTString("SELECT [USER_NAME] FROM  [dbo].[Users]");

            int j = 0;
            while (j < 11)
            {
                FFbutton NumPadButton = new FFbutton();
                NumPadButton.Name = "NumPadButton" + (j + 1).ToString();
                NumPadButton.Text = (j + 1).ToString();
                NumPadButton.FlatStyle = FlatStyle.Flat;
                NumPadButton.Width = panel_num_pad.Width / 3;
                NumPadButton.Height = panel_num_pad.Height / 4;
                NumPadButton.Click += new System.EventHandler(this.fFbutton4_Click);
                NumPadButton.Location = new System.Drawing.Point((panel_num_pad.Width / 3) * (j % 3), (panel_num_pad.Height / 4) * (j / 3));
                if (j == 9)
                    NumPadButton.Text = "<--";
                if (j == 10)
                    NumPadButton.Text = "0";

                j++;
                panel_num_pad.Controls.Add(NumPadButton);
            }  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int user = 1;

            if (user <= 0)
            {
                MessageBox.Show("სახელი ან პაროლი არასწორია", "ვალიდაცია", MessageBoxButtons.OK);
                Result = DialogResult.Cancel;
                return;
            }
            Result = DialogResult.OK;
            this.Close();

        }

        private void bten_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Globals.Language = "en";
            Globals.LoadData();
            int user = User.CheckPass(cbUsers.Text,User.GetString(User.EncriptPass(tbPassword.Text)));

            if (user <= 0)
            {
                MessageBox.Show("სახელი ან პაროლი არასწორია", "ვალიდაცია", MessageBoxButtons.OK);
                Result = DialogResult.Cancel;
                return;
            }
            Result = DialogResult.OK;
            this.Close();
    
        }

        private void btka_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Globals.Language = "ka";
            Globals.LoadData();

            int user = User.CheckPass(cbUsers.Text, User.GetString(User.EncriptPass(tbPassword.Text))); 

            if (user <= 0)
            {
                MessageBox.Show("სახელი ან პაროლი არასწორია", "ვალიდაცია", MessageBoxButtons.OK);
                Result = DialogResult.Cancel;
                return;
            }
            Result = DialogResult.OK;
            this.Close();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fFbutton4_Click(object sender, EventArgs e)
        {
            string ss = ((FFbutton)sender).Text;

            string v = tbPassword.Text.ToString();

            switch (ss)
            {

                case "<--":
                    {

                        if (v.Length > 1)
                        {
                            v = v.Substring(0, v.Length - 1);
                        }
                        else
                            v = "";
                        tbPassword.Text = v;
                        break;

                    }

                default:
                    if (v != "0")
                        tbPassword.Text = v + ss;
                    else
                        tbPassword.Text = ss;
                    break;

            }
        }
    }
}
