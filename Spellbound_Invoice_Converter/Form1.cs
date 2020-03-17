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
    public partial class SpellboundInvoiceConverter : Form
    {
        string csvFile;
        string customerData;
        public static double dueDateDays = 21;
        static csvConvert csvConverter;

        public SpellboundInvoiceConverter()
        {
            InitializeComponent();
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            csvConverter = new csvConvert();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "CSV Files (*.csv)|*.csv";
            if (file.ShowDialog() == DialogResult.OK)
            {
                csvFile = file.FileName;
                labelSelectedCSV.Text = csvFile;
            }
        }

        private void buttonCustomerData_Click(object sender, EventArgs e)
        {
            csvConverter = new csvConvert();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "CSV Files (*.csv)|*.csv";
            if (file.ShowDialog() == DialogResult.OK)
            {
                customerData = file.FileName;
                labelCustomerData.Text = customerData;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            if (csvFile != null && customerData != null)
                csvConverter.ConvertCSV(csvFile, customerData);
            else
                MessageBox.Show("Please select both a csv to convert and a datasheet to pull customer data from");
        }
    }
}
