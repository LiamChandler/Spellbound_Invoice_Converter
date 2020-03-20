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
        string csvFile;
        string customerData;
        public static double dueDateDays = 21;
        static csvConvert csvConverter;
        private static System.Timers.Timer saveTimer;

        public static int invoiceNumber;

        public static DataTable config = new DataTable();

        public SpellboundInvoiceConverter()
        {
            InitializeComponent();

            config = importConfig();

            invoiceNumber = Int32.Parse((string)config.Rows.Find("CurrentInvoiceNumber")[1]);
            dueDateDays = Int32.Parse((string)config.Rows.Find("InvoiceDueDatePeriod")[1]);
            customerData = (string)config.Rows.Find("LastCustomerDataLocation")[1];
            labelCustomerData.Text = customerData;

            saveTimer = new System.Timers.Timer();
            saveTimer.Interval = 60000;

            // Hook up the Elapsed event for the timer. 
            saveTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            saveTimer.AutoReset = true;

            // Start the timer
            saveTimer.Enabled = true;
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

        public static int getInvoiceNumber()
        {
            invoiceNumber++;
            return invoiceNumber;
        }

        private DataTable importConfig()
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
                        dt.Columns.Add(h);
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
            catch(Exception e)
            {
                MessageBox.Show("Please make sure that there is a 'config.csv' file in the program directory");
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            exportConfig();
        }

        private static void exportConfig()
        {
            StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\config.csv");

            sw.WriteLine("Setting,Value");
                object[] cols;
            foreach(DataRow row in config.Rows)
            {
                cols = row.ItemArray;
                sw.WriteLine(cols[0] + "," + cols[1]);
            }
            sw.Flush();
            sw.Close();
            Debug.WriteLine("Config Saved");
        }
        static void OnApplicationExit(object sender, EventArgs e)
        {
            exportConfig();
        }
    }
}
