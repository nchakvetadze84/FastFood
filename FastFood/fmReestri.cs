using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FastFood
{
    public partial class fmReestri : Form
    {
        public fmReestri()
        {
            InitializeComponent();
        }

        private void fmReestri_Load(object sender, EventArgs e)
        {
            Text = Globals.GetString("Register");
            dataGridView1.DataSource = DBObject.InvokeTString(@"SELECT  (ROW_NUMBER() OVER(ORDER BY ID)) AS ID, CAST(CONVERT( CHAR(8),[Date] , 112) as smalldatetime) as [DATE], 
                                            Check_No, SumPriceWithSale,Sale,PayForm,Complited 
                                            FROM dbo.Orders");
        }
    }
}
