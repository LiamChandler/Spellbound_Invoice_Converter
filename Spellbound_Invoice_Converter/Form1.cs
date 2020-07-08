using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Spellbound_Invoice_Converter
{
    public partial class SpellboundInvoiceConverter : Form
    {
        string dataFileLocation;
        static string businessDataLocation;

        public static double dueDateDays;
        static csvConvert csvConverter;

        private static int invoiceNumber;

        public static DataTable config = new DataTable();

        public SpellboundInvoiceConverter()
        {
            InitializeComponent();

            config = importConfig(true);

            businessDataLocation = Directory.GetCurrentDirectory() + "\\BusinessData.csv";

            if (config != null)
            {
                invoiceNumber = Int32.Parse((string)config.Rows.Find("CurrentInvoiceNumber")[1]);
                dueDateDays = Int32.Parse((string)config.Rows.Find("InvoiceDueDatePeriod")[1]);
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (config == null)
                MessageBox.Show("Please make sure that there is 'config.csv' file in the directory");
            else
            {
                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "CSV Files (*.csv)|*.csv";
                if (file.ShowDialog() == DialogResult.OK)
                {
                    dataFileLocation = file.FileName;
                }

                if (dataFileLocation != null)
                {
                    csvConverter = new csvConvert();
                    csvConverter.ConvertCSV(dataFileLocation, businessDataLocation);
                    exportConfig();
                }
                else
                    MessageBox.Show("Please select a .csv file to convert.");
            }
        }

        public static int getInvoiceNumber()
        {
            invoiceNumber++;
            return invoiceNumber;
        }

        private DataTable importConfig(bool showMessage)
        {
            try
            {
                DataTable dt = new DataTable();
                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\config.csv"))
                {
                    // Add header line to table
                    string[] headers = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                    foreach (string h in headers)
                    {
                        dt.Columns.Add(h.Trim('"'));
                    }

                    // Add information from CSV to table
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] rows = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                        if (!line.StartsWith("#") && rows[0].CompareTo("") != 0)
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                dr[i] = rows[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }

                DataColumn[] keyColumns = new DataColumn[1];
                keyColumns[0] = dt.Columns[0];
                dt.PrimaryKey = keyColumns;

                return dt;
            }
            catch (Exception e)
            {
                if (showMessage)
                {
                    StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\configExample.csv");

                    sw.WriteLine("Setting,Value,Discription");
                    sw.WriteLine("CurrentInvoiceNumber,1,Get from Xero to align with previous invoices");
                    sw.WriteLine("InvoiceDueDatePeriod,14,Amount of time for the invoice to be returned");
                    sw.WriteLine("InventoryItemCode,,normally 'Tour'");
                    sw.WriteLine("AccountCode,,Normally '271'");
                    sw.WriteLine("TaxType,,Normally '15% GST on Income'");
                    sw.Flush();
                    sw.Close();

                    MessageBox.Show("You need to create a 'config.csv' file in the directory.\n An example file has been created.");
                }
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        private void exportConfig()
        {
            config.Rows.Find("CurrentInvoiceNumber")[1] = invoiceNumber.ToString();

            StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\config.csv");

            sw.WriteLine("Setting,Value,Discription");
            object[] cols;
            foreach (DataRow row in config.Rows)
            {
                cols = row.ItemArray;
                for (int i = 0; i < cols.Length; i++)
                {
                    if (i != 0)
                        sw.Write(",");
                    sw.Write(cols[i]);
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();
            Debug.WriteLine("Config Saved");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
