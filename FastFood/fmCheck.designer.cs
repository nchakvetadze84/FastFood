namespace FastFood
{
    partial class fmCheck
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbCheck = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbReciveAmount = new System.Windows.Forms.Label();
            this.lbTime = new System.Windows.Forms.Label();
            this.lbPayment = new System.Windows.Forms.Label();
            this.teCheckNo = new System.Windows.Forms.TextBox();
            this.cbSale = new System.Windows.Forms.ComboBox();
            this.lbSale = new System.Windows.Forms.Label();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.lbChange = new System.Windows.Forms.Label();
            this.teReciveAmount = new System.Windows.Forms.TextBox();
            this.teDate = new System.Windows.Forms.TextBox();
            this.teTime = new System.Windows.Forms.TextBox();
            this.teAmount = new System.Windows.Forms.TextBox();
            this.teChange = new System.Windows.Forms.TextBox();
            this.teSaleAmount = new System.Windows.Forms.TextBox();
            this.lbSaleAmount = new System.Windows.Forms.Label();
            this.lbChangeForm = new System.Windows.Forms.Label();
            this.cbPayForm = new System.Windows.Forms.ComboBox();
            this.lbLanguage = new System.Windows.Forms.Label();
            this.gbLanguage = new System.Windows.Forms.GroupBox();
            this.rbGeo = new System.Windows.Forms.RadioButton();
            this.rbENG = new System.Windows.Forms.RadioButton();
            this.panel_num_pad = new System.Windows.Forms.Panel();
            this.gbLanguage.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCheck
            // 
            this.lbCheck.Location = new System.Drawing.Point(3, 15);
            this.lbCheck.Name = "lbCheck";
            this.lbCheck.Size = new System.Drawing.Size(118, 16);
            this.lbCheck.TabIndex = 0;
            this.lbCheck.Text = "ჩეკის ნომერი";
            this.lbCheck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbDate
            // 
            this.lbDate.Location = new System.Drawing.Point(3, 46);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(118, 16);
            this.lbDate.TabIndex = 2;
            this.lbDate.Text = "თარიღი";
            this.lbDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbReciveAmount
            // 
            this.lbReciveAmount.Location = new System.Drawing.Point(3, 202);
            this.lbReciveAmount.Name = "lbReciveAmount";
            this.lbReciveAmount.Size = new System.Drawing.Size(118, 16);
            this.lbReciveAmount.TabIndex = 6;
            this.lbReciveAmount.Text = "მიღებული თანხა";
            this.lbReciveAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbTime
            // 
            this.lbTime.Location = new System.Drawing.Point(3, 77);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(118, 16);
            this.lbTime.TabIndex = 4;
            this.lbTime.Text = "დრო";
            this.lbTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbPayment
            // 
            this.lbPayment.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPayment.Location = new System.Drawing.Point(3, 109);
            this.lbPayment.Name = "lbPayment";
            this.lbPayment.Size = new System.Drawing.Size(118, 16);
            this.lbPayment.TabIndex = 8;
            this.lbPayment.Text = "თანხა";
            this.lbPayment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // teCheckNo
            // 
            this.teCheckNo.Location = new System.Drawing.Point(129, 12);
            this.teCheckNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teCheckNo.Name = "teCheckNo";
            this.teCheckNo.ReadOnly = true;
            this.teCheckNo.Size = new System.Drawing.Size(116, 23);
            this.teCheckNo.TabIndex = 9;
            this.teCheckNo.TabStop = false;
            // 
            // cbSale
            // 
            this.cbSale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSale.FormattingEnabled = true;
            this.cbSale.Location = new System.Drawing.Point(129, 136);
            this.cbSale.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbSale.Name = "cbSale";
            this.cbSale.Size = new System.Drawing.Size(116, 24);
            this.cbSale.TabIndex = 4;
            this.cbSale.SelectedIndexChanged += new System.EventHandler(this.cbSale_SelectedIndexChanged);
            // 
            // lbSale
            // 
            this.lbSale.Location = new System.Drawing.Point(3, 141);
            this.lbSale.Name = "lbSale";
            this.lbSale.Size = new System.Drawing.Size(118, 16);
            this.lbSale.TabIndex = 11;
            this.lbSale.Text = "მომსახურება";
            this.lbSale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(12, 332);
            this.btOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(87, 28);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "gadaxda";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(425, 332);
            this.btCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(87, 28);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "gauqmeba";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lbChange
            // 
            this.lbChange.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbChange.Location = new System.Drawing.Point(3, 233);
            this.lbChange.Name = "lbChange";
            this.lbChange.Size = new System.Drawing.Size(118, 16);
            this.lbChange.TabIndex = 22;
            this.lbChange.Text = "ხურდა";
            this.lbChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // teReciveAmount
            // 
            this.teReciveAmount.Location = new System.Drawing.Point(129, 199);
            this.teReciveAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teReciveAmount.Name = "teReciveAmount";
            this.teReciveAmount.Size = new System.Drawing.Size(116, 23);
            this.teReciveAmount.TabIndex = 0;
            this.teReciveAmount.TextChanged += new System.EventHandler(this.teReciveAmount_TextChanged);
            // 
            // teDate
            // 
            this.teDate.Location = new System.Drawing.Point(129, 43);
            this.teDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teDate.Name = "teDate";
            this.teDate.ReadOnly = true;
            this.teDate.Size = new System.Drawing.Size(116, 23);
            this.teDate.TabIndex = 24;
            this.teDate.TabStop = false;
            // 
            // teTime
            // 
            this.teTime.Location = new System.Drawing.Point(129, 74);
            this.teTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teTime.Name = "teTime";
            this.teTime.ReadOnly = true;
            this.teTime.Size = new System.Drawing.Size(116, 23);
            this.teTime.TabIndex = 25;
            this.teTime.TabStop = false;
            // 
            // teAmount
            // 
            this.teAmount.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.teAmount.Location = new System.Drawing.Point(129, 105);
            this.teAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teAmount.Name = "teAmount";
            this.teAmount.ReadOnly = true;
            this.teAmount.Size = new System.Drawing.Size(116, 23);
            this.teAmount.TabIndex = 26;
            this.teAmount.TabStop = false;
            // 
            // teChange
            // 
            this.teChange.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.teChange.Location = new System.Drawing.Point(129, 230);
            this.teChange.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teChange.Name = "teChange";
            this.teChange.ReadOnly = true;
            this.teChange.Size = new System.Drawing.Size(116, 23);
            this.teChange.TabIndex = 27;
            this.teChange.TabStop = false;
            // 
            // teSaleAmount
            // 
            this.teSaleAmount.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.teSaleAmount.Location = new System.Drawing.Point(129, 168);
            this.teSaleAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.teSaleAmount.Name = "teSaleAmount";
            this.teSaleAmount.ReadOnly = true;
            this.teSaleAmount.Size = new System.Drawing.Size(116, 23);
            this.teSaleAmount.TabIndex = 29;
            this.teSaleAmount.TabStop = false;
            this.teSaleAmount.TextChanged += new System.EventHandler(this.teSaleAmount_TextChanged);
            // 
            // lbSaleAmount
            // 
            this.lbSaleAmount.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSaleAmount.ForeColor = System.Drawing.Color.Red;
            this.lbSaleAmount.Location = new System.Drawing.Point(3, 171);
            this.lbSaleAmount.Name = "lbSaleAmount";
            this.lbSaleAmount.Size = new System.Drawing.Size(118, 16);
            this.lbSaleAmount.TabIndex = 28;
            this.lbSaleAmount.Text = "თანხა ფასდაკლებით";
            this.lbSaleAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbChangeForm
            // 
            this.lbChangeForm.Location = new System.Drawing.Point(3, 264);
            this.lbChangeForm.Name = "lbChangeForm";
            this.lbChangeForm.Size = new System.Drawing.Size(118, 16);
            this.lbChangeForm.TabIndex = 31;
            this.lbChangeForm.Text = "გადახდის ფორმა";
            this.lbChangeForm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbPayForm
            // 
            this.cbPayForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPayForm.FormattingEnabled = true;
            this.cbPayForm.Location = new System.Drawing.Point(129, 261);
            this.cbPayForm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbPayForm.Name = "cbPayForm";
            this.cbPayForm.Size = new System.Drawing.Size(116, 24);
            this.cbPayForm.TabIndex = 30;
            // 
            // lbLanguage
            // 
            this.lbLanguage.Location = new System.Drawing.Point(3, 298);
            this.lbLanguage.Name = "lbLanguage";
            this.lbLanguage.Size = new System.Drawing.Size(118, 16);
            this.lbLanguage.TabIndex = 32;
            this.lbLanguage.Text = "ჩეკის ენა";
            this.lbLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbLanguage
            // 
            this.gbLanguage.Controls.Add(this.rbGeo);
            this.gbLanguage.Controls.Add(this.rbENG);
            this.gbLanguage.Location = new System.Drawing.Point(129, 286);
            this.gbLanguage.Name = "gbLanguage";
            this.gbLanguage.Size = new System.Drawing.Size(117, 34);
            this.gbLanguage.TabIndex = 33;
            this.gbLanguage.TabStop = false;
            // 
            // rbGeo
            // 
            this.rbGeo.AutoSize = true;
            this.rbGeo.Checked = true;
            this.rbGeo.Location = new System.Drawing.Point(5, 11);
            this.rbGeo.Name = "rbGeo";
            this.rbGeo.Size = new System.Drawing.Size(51, 20);
            this.rbGeo.TabIndex = 1;
            this.rbGeo.TabStop = true;
            this.rbGeo.Text = "GEO";
            this.rbGeo.UseVisualStyleBackColor = true;
            // 
            // rbENG
            // 
            this.rbENG.AutoSize = true;
            this.rbENG.Location = new System.Drawing.Point(62, 11);
            this.rbENG.Name = "rbENG";
            this.rbENG.Size = new System.Drawing.Size(50, 20);
            this.rbENG.TabIndex = 0;
            this.rbENG.Text = "ENG";
            this.rbENG.UseVisualStyleBackColor = true;
            // 
            // panel_num_pad
            // 
            this.panel_num_pad.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel_num_pad.Location = new System.Drawing.Point(265, 15);
            this.panel_num_pad.Name = "panel_num_pad";
            this.panel_num_pad.Size = new System.Drawing.Size(247, 299);
            this.panel_num_pad.TabIndex = 34;
            // 
            // fmCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(524, 369);
            this.ControlBox = false;
            this.Controls.Add(this.panel_num_pad);
            this.Controls.Add(this.gbLanguage);
            this.Controls.Add(this.lbLanguage);
            this.Controls.Add(this.lbChangeForm);
            this.Controls.Add(this.cbPayForm);
            this.Controls.Add(this.teSaleAmount);
            this.Controls.Add(this.lbSaleAmount);
            this.Controls.Add(this.teChange);
            this.Controls.Add(this.teAmount);
            this.Controls.Add(this.teTime);
            this.Controls.Add(this.teDate);
            this.Controls.Add(this.teReciveAmount);
            this.Controls.Add(this.lbChange);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.lbSale);
            this.Controls.Add(this.cbSale);
            this.Controls.Add(this.teCheckNo);
            this.Controls.Add(this.lbPayment);
            this.Controls.Add(this.lbReciveAmount);
            this.Controls.Add(this.lbTime);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbCheck);
            this.Font = new System.Drawing.Font("Sylfaen", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "fmCheck";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.fmCheck_Load);
            this.gbLanguage.ResumeLayout(false);
            this.gbLanguage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbCheck;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbReciveAmount;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label lbPayment;
        private System.Windows.Forms.TextBox teCheckNo;
        private System.Windows.Forms.ComboBox cbSale;
        private System.Windows.Forms.Label lbSale;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label lbChange;
        private System.Windows.Forms.TextBox teReciveAmount;
        private System.Windows.Forms.TextBox teDate;
        private System.Windows.Forms.TextBox teTime;
        private System.Windows.Forms.TextBox teAmount;
        private System.Windows.Forms.TextBox teChange;
        private System.Windows.Forms.TextBox teSaleAmount;
        private System.Windows.Forms.Label lbSaleAmount;
        private System.Windows.Forms.Label lbChangeForm;
        private System.Windows.Forms.ComboBox cbPayForm;
        private System.Windows.Forms.Label lbLanguage;
        private System.Windows.Forms.GroupBox gbLanguage;
        private System.Windows.Forms.RadioButton rbGeo;
        private System.Windows.Forms.RadioButton rbENG;
        private System.Windows.Forms.Panel panel_num_pad;

    }
}