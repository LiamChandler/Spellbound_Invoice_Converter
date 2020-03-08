namespace Spellbound_Invoice_Converter
{
    partial class Form1
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
            this.buttonSelect = new System.Windows.Forms.Button();
            this.labelSelectedCSV = new System.Windows.Forms.Label();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(13, 13);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonSelect.TabIndex = 0;
            this.buttonSelect.Text = "Select CSV";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // labelSelectedCSV
            // 
            this.labelSelectedCSV.AutoSize = true;
            this.labelSelectedCSV.Location = new System.Drawing.Point(94, 18);
            this.labelSelectedCSV.Name = "labelSelectedCSV";
            this.labelSelectedCSV.Size = new System.Drawing.Size(73, 13);
            this.labelSelectedCSV.TabIndex = 1;
            this.labelSelectedCSV.Text = "Selected CSV";
            // 
            // buttonConvert
            // 
            this.buttonConvert.Location = new System.Drawing.Point(13, 179);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(75, 23);
            this.buttonConvert.TabIndex = 2;
            this.buttonConvert.Text = "button1";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 531);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.labelSelectedCSV);
            this.Controls.Add(this.buttonSelect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Label labelSelectedCSV;
        private System.Windows.Forms.Button buttonConvert;
    }
}

