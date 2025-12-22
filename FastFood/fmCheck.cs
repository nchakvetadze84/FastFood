using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Globalization;

namespace FastFood
{
    public partial class fmCheck : Form
    {
        private Order m_Order = new Order();

        public Order Order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }

        public fmCheck()
        {
            InitializeComponent();
        }

        private void fmCheck_Load(object sender, EventArgs e)
        {
            string[] salelist = new string[3] { "10%", "5%", "0%" };
            cbSale.DataSource = salelist;

            string[] list = new string[2] { Globals.Language == "ka" ? "ნაღდი" : "Cash", Globals.Language == "ka" ? "უნაღდო" : "Clearing" };
            cbPayForm.DataSource = list;

            btOK.Text = Globals.GetString("OK");
            btCancel.Text = Globals.GetString("Cancel");
            lbChange.Text = Globals.GetString("Change");
            lbChangeForm.Text = Globals.GetString("ChangeForm");
            lbCheck.Text = Globals.GetString("Check");
            lbDate.Text = Globals.GetString("Date");
            lbLanguage.Text = Globals.GetString("Language");
            lbPayment.Text = Globals.GetString("Payment");
            lbReciveAmount.Text = Globals.GetString("ReciveAmount");
            lbSale.Text = Globals.GetString("Sale");
            lbSaleAmount.Text = Globals.GetString("SaleAmount");
            lbTime.Text = Globals.GetString("Time");

            int j = 0;
            while (j < 12)
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
                if (j == 11)
                    NumPadButton.Text = ".";

                j++;
                panel_num_pad.Controls.Add(NumPadButton);
            }
            teCheckNo.Text = m_Order.CheckNo;
            teDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            teTime.Text = DateTime.Now.Hour.ToString() + ":" + ((DateTime.Now.Minute.ToString().Length == 1) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
            cbSale.SelectedIndex = 0;
            cbPayForm.SelectedIndex = 0;
            teAmount.Text = Convert.ToString(Order.Sum());

            cbSale_SelectedIndexChanged(null, null);
        }


        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int papaerwidth = 0;
            for (int k = 0; k < e.PageSettings.PrinterSettings.PaperSizes.Count; k++)
            {
                if (e.PageSettings.PrinterSettings.PaperSizes[k].PaperName == "Roll Paper 80 x 297 mm")

                    papaerwidth = e.PageSettings.PrinterSettings.PaperSizes[k].Width;
            }
            Graphics g = e.Graphics;

            String message = System.Environment.UserName;
            StringFormat strformat = new StringFormat();
            //strformat.LineAlignment = StringAlignment.Center;
            //strformat.Alignment = StringAlignment.Near;
            strformat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            Font messageFont = new Font("Arial", 10, System.Drawing.GraphicsUnit.Point);
            int y = 1, x = 5;
            g.DrawString(Globals.GetString("Company"), messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y, strformat);
            g.DrawString(Globals.GetString("Date") + " - " + teDate.Text, messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + messageFont.Height, strformat);
            g.DrawString(Globals.GetString("Time") + " - " + teTime.Text, messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + 2 * messageFont.Height, strformat);
            g.DrawString(("N" + m_Order.CheckNo).ToString(), messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + 3 * messageFont.Height, strformat);
            g.DrawLine(new Pen(Color.Black), new Point(x, y + 4 * messageFont.Height), new Point(papaerwidth, y + 4 * messageFont.Height));
            g.DrawString(Globals.GetString("Name"), messageFont, Brushes.Black, x, y + 4 * messageFont.Height);
            g.DrawString("N", messageFont, Brushes.Black, papaerwidth / 2 + 10, y + 4 * messageFont.Height);
            g.DrawString(Globals.GetString("Price"), messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + 4 * messageFont.Height, strformat);
            g.DrawLine(new Pen(Color.Black), new Point(x, y + 5 * messageFont.Height), new Point(papaerwidth, y + 5 * messageFont.Height));
            RectangleF printarea = new RectangleF();

            int i;
            for (i = 0; i < Order.OrderList.Count; i++)
            {
                printarea.X = x;
                printarea.Y = y + (i + 6) * messageFont.Height;
                printarea.Width = papaerwidth / 2 + 10;
                printarea.Height = messageFont.Height;
                g.DrawString(rbGeo.Checked ? Order.OrderList[i].Menu.Name : Order.OrderList[i].Menu.NameEn, messageFont, Brushes.Black, printarea);
                g.DrawString(Order.OrderList[i].Quantity.ToString(), messageFont, Brushes.Black, papaerwidth / 2 + 10, y + (i + 6) * messageFont.Height);
                g.DrawString((Order.OrderList[i].Quantity * Order.OrderList[i].Menu.Price).ToString(), messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + (i + 6) * messageFont.Height, strformat);
            }

            g.DrawLine(new Pen(Color.Black), new Point(x, y + (i + 7) * messageFont.Height), new Point(papaerwidth, y + (i + 7) * messageFont.Height));
            g.DrawString(Globals.GetString("ReciveAmount"), messageFont, Brushes.Black, x, y + (i + 7) * messageFont.Height);
            g.DrawString(string.Format("{0:f}", Convert.ToDecimal(teReciveAmount.Text)), messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + (i + 7) * messageFont.Height, strformat);
            i++;
            g.DrawString(Globals.GetString("Change"), messageFont, Brushes.Black, x, y + (i + 7) * messageFont.Height);
            g.DrawString(string.Format("{0:f}", Convert.ToDecimal(teChange.Text)), messageFont, Brushes.Black, e.PageSettings.PrintableArea.Width, y + (i + 7) * messageFont.Height, strformat);
            i++;
            g.DrawLine(new Pen(Color.Black), new Point(x, y + (i + 7) * messageFont.Height), new Point(papaerwidth, y + (i + 7) * messageFont.Height));
            g.DrawLine(new Pen(Color.Black), new Point(papaerwidth / 2 + 3, y + 4 * messageFont.Height), new Point(papaerwidth / 2 + 3, y + (i + 7) * messageFont.Height));
            g.DrawLine(new Pen(Color.Black), new Point(papaerwidth / 2 + 27, y + 4 * messageFont.Height), new Point(papaerwidth / 2 + 27, y + (i + 7) * messageFont.Height));
            i++;
            g.DrawString(Globals.GetString("Sum"), messageFont, Brushes.Black, x, y + (i + 7) * messageFont.Height);
            g.DrawString(teAmount.Text, new Font("Arial", 14, FontStyle.Bold, System.Drawing.GraphicsUnit.Point), Brushes.Black, e.PageSettings.PrintableArea.Width, y + (i + 7) * messageFont.Height, strformat);
            i++;

            i++;
            StringFormat ss = new StringFormat();
            ss.Alignment = StringAlignment.Center;
            g.DrawString(Globals.GetString("Thank") + "!", messageFont, Brushes.Black, papaerwidth / 2 - (e.Graphics.MeasureString(Globals.GetString("Thank") + "!", messageFont).Width) / 2, y + (i + 7) * messageFont.Height);
            i++;
            e.HasMorePages = false;
        }
        //Font boldfont = new Font("Arial", 14, FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        //        StringFormat strformat = new StringFormat();
        ////strformat.LineAlignment = StringAlignment.Center;
        ////strformat.Alignment = StringAlignment.Near;
        //strformat.FormatFlags = StringFormatFlags.DirectionRightToLeft;


        private void cbSale_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sale = 0;
            try
            {
                sale = Convert.ToInt16(cbSale.Text.Remove(cbSale.Text.Length - 1, 1));
            }
            catch
            {
                string s = "";
                foreach (OrderItem oi in m_Order.OrderList)
                {
                    s += oi.Menu.Id.ToString() + "," + oi.Quantity.ToString() + ";";
                }
                // TODO: procedures and functions are not transfered by pgloader
                decimal d = Convert.ToDecimal(DBObjectNew.InvokeString("select dbo.REEVALUATE_ORDER_COST_PRICE('" + s + "')"));
                teSaleAmount.Text = d.ToString();
                return;
            }
            if (teAmount.Text != String.Empty)
                teSaleAmount.Text = Convert.ToString(Convert.ToDecimal(teAmount.Text) + Convert.ToDecimal(teAmount.Text) * sale / 100);
        }

        private void teReciveAmount_TextChanged(object sender, EventArgs e)
        {
            teChange.Text = "";
            try
            {
                if (teReciveAmount.Text != String.Empty)
                    teChange.Text = Convert.ToString(Convert.ToDecimal(teReciveAmount.Text) - Convert.ToDecimal(teSaleAmount.Text));
            }
            catch
            {

            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            string lang = Globals.Language;
            try
            {

                decimal amountIn = Convert.ToDecimal(teReciveAmount.Text);
                if (amountIn < Convert.ToDecimal(teSaleAmount.Text))
                {
                    CYMessageBox.Show("მიღებული თანხა ნაკლები არ შეიძლება იყოს გადასახდელ თანხაზე", Globals.GetString("Error"), CYMessageBox.CYButtons.OK);
                    return;
                }
                Dictionary<string, object> param = new Dictionary<string, object>();
                // Edit Order
                DBObjectNew dbObject = new DBObjectNew("[dbo].[Orders]", "");
                param.Add("PrintTime", DateTime.Now.ToLongTimeString());
                param.Add("AmountIn", decimal.Parse(teReciveAmount.Text));
                param.Add("SumPrice", decimal.Parse(teAmount.Text));
                param.Add("Sale", cbSale.Text.Substring(0, cbSale.Text.Length - 1));
                param.Add("SumPriceWithSale", decimal.Parse(teSaleAmount.Text));
                param.Add("PayForm", cbPayForm.SelectedIndex + 1);
                param.Add("Complited", 1);
                dbObject.Update(m_Order.ID, param);

                param.Clear();

                // save
                DBObjectNew dbDetails = new DBObjectNew("[dbo].[Order_Details]", "");

                foreach (OrderItem oi in m_Order.OrderList)
                {
                    param.Add("Order_ID", m_Order.ID);
                    param.Add("Menu_ID", oi.Menu.Id);
                    param.Add("Quantity", oi.Quantity);
                    param.Add("Price", oi.Menu.Price);
                    dbDetails.Insert(param);
                    param.Clear();
                }

                Globals.Language = rbGeo.Checked ? "ka" : "en";
                PrintDocument printDocument1 = new PrintDocument();
                printDocument1.PrinterSettings.PrinterName = Convert.ToString(DBObjectNew.InvokeString("SELECT [Printer] FROM [dbo].[Menu_Types] WHERE ([Name_GE] = N'ბარი')"));
                printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
                if (printDocument1.PrinterSettings.IsValid)
                {
                    printDocument1.PrintController = new StandardPrintController();
                    printDocument1.Print();
                }

                DBObjectNew tmpdbo = new DBObjectNew("[dbo].[Order_Details_tmp]", "");
                tmpdbo.Delete(m_Order.ID);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {

                CYMessageBox.Show(" " + ex.Message, Globals.GetString("Error"), CYMessageBox.CYButtons.OK);
            }
            finally
            {
                Globals.Language = lang;
            }
        }

        private void fFbutton4_Click(object sender, EventArgs e)
        {
            string ss = ((FFbutton)sender).Text;

            string v = teReciveAmount.Text.ToString();

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
                        teReciveAmount.Text = v;
                        break;

                    }
                case ".":
                    {

                        if (v.Length > 0 && !v.Contains("."))
                            v = v + ".";
                        else if (v.Length == 0)
                            v = "";
                        teReciveAmount.Text = v;
                        break;

                    }

                default:
                    if (v != "0")
                        teReciveAmount.Text = v + ss;
                    else
                        teReciveAmount.Text = ss;
                    break;
            }
        }

        private void teSaleAmount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (teReciveAmount.Text != String.Empty)
                    teChange.Text = Convert.ToString(Convert.ToDecimal(teReciveAmount.Text) - Convert.ToDecimal(teSaleAmount.Text));
            }
            catch
            {

            }
        }

    }
}
