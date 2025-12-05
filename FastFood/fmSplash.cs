using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FastFood
{
    public partial class fmSplash : Form
    {
        public fmSplash()
        {
            InitializeComponent();
           
        }

        private void FmSplash_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
