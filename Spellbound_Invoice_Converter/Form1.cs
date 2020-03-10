using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spellbound_Invoice_Converter
{
    public partial class Form1 : Form
    {
        string csvFile;
        public static double dueDateDays = 14;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "CSV Files (*.csv)|*.csv";
            if (file.ShowDialog() == DialogResult.OK)
            {
                csvFile = file.FileName;
                labelSelectedCSV.Text = csvFile;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            csvConvert.ConvertCSV(csvFile); 
        }
    }
}
