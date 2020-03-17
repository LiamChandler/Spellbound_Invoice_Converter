namespace Spellbound_Invoice_Converter
{
    partial class SpellboundInvoiceConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellboundInvoiceConverter));
            this.buttonSelect = new System.Windows.Forms.Button();
            this.labelSelectedCSV = new System.Windows.Forms.Label();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.labelCustomerData = new System.Windows.Forms.Label();
            this.buttonCustomerData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(12, 12);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(120, 23);
            this.buttonSelect.TabIndex = 0;
            this.buttonSelect.Text = "Select CSV";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // labelSelectedCSV
            // 
            this.labelSelectedCSV.AutoSize = true;
            this.labelSelectedCSV.Location = new System.Drawing.Point(138, 17);
            this.labelSelectedCSV.Name = "labelSelectedCSV";
            this.labelSelectedCSV.Size = new System.Drawing.Size(0, 13);
            this.labelSelectedCSV.TabIndex = 1;
            // 
            // buttonConvert
            // 
            this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonConvert.Location = new System.Drawing.Point(12, 70);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(120, 23);
            this.buttonConvert.TabIndex = 2;
            this.buttonConvert.Text = "Convert Data";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // labelCustomerData
            // 
            this.labelCustomerData.AutoSize = true;
            this.labelCustomerData.Location = new System.Drawing.Point(138, 46);
            this.labelCustomerData.Name = "labelCustomerData";
            this.labelCustomerData.Size = new System.Drawing.Size(0, 13);
            this.labelCustomerData.TabIndex = 4;
            // 
            // buttonCustomerData
            // 
            this.buttonCustomerData.Location = new System.Drawing.Point(12, 41);
            this.buttonCustomerData.Name = "buttonCustomerData";
            this.buttonCustomerData.Size = new System.Drawing.Size(120, 23);
            this.buttonCustomerData.TabIndex = 3;
            this.buttonCustomerData.Text = "CustomerData";
            this.buttonCustomerData.UseVisualStyleBackColor = true;
            this.buttonCustomerData.Click += new System.EventHandler(this.buttonCustomerData_Click);
            // 
            // SpellboundInvoiceConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 105);
            this.Controls.Add(this.labelCustomerData);
            this.Controls.Add(this.buttonCustomerData);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.labelSelectedCSV);
            this.Controls.Add(this.buttonSelect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SpellboundInvoiceConverter";
            this.Text = "Spellbound Invoice Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Label labelSelectedCSV;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.Label labelCustomerData;
        private System.Windows.Forms.Button buttonCustomerData;
    }
}

