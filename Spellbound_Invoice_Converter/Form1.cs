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
		static csvConvert csvConverter;

		string dataFileLocation;
		static string businessDataLocation;
		private static int invoiceNumber;

		public static DataTable config = new DataTable();

		public SpellboundInvoiceConverter()
		{
			InitializeComponent();

			// Import data from the config.csv file
			config = importConfig();

			businessDataLocation = Directory.GetCurrentDirectory() + "\\BusinessData.csv";
		}

		// Select and process file
		private void buttonSelect_Click(object sender, EventArgs e)
		{
			if (config == null)
				MessageBox.Show("Please make sure that there is 'config.csv' file in the directory");
			else
			{
				if (textBoxInvoiceNumber.Text == "")
				{
					MessageBox.Show("Please input an invoice number.");
					return;
				}

				try
				{
					invoiceNumber = int.Parse(textBoxInvoiceNumber.Text);
				}
				catch
				{
					MessageBox.Show("Please input a valid invoice number.");
					return;
				}

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
			}
		}

		// Return the next invoice number
		public static int getInvoiceNumber()
		{
			invoiceNumber++;
			return invoiceNumber;
		}

		// Returns a DataTable filled with the contents of the given file
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
						dt.Columns.Add(h.Trim('"'));
					}

					// Add information from CSV to table
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						string[] rows = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

						// If line is not comment or empty then process the contents
						if (!line.StartsWith("#") && rows[0].CompareTo("") != 0)
						{
							DataRow dr = dt.NewRow();
							for (int i = 0; i < headers.Length; i++)
							{
								dr[i] = rows[i].Trim('"');
							}
							dt.Rows.Add(dr);
						}
					}
				}

				// Set column 0 to be the primary key
				DataColumn[] keyColumns = new DataColumn[1];
				keyColumns[0] = dt.Columns[0];
				dt.PrimaryKey = keyColumns;

				return dt;
			}
			catch (Exception e)
			{
				// Print the standard config file to the current directory
				StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\configExample.csv");

				sw.WriteLine("Setting,Value,Discription");
				sw.WriteLine("InventoryItemCode,,normally 'Tour'");
				sw.WriteLine("AccountCode,,Normally '271'");
				sw.WriteLine("TaxType,,Normally '15% GST on Income'");
				sw.Flush();
				sw.Close();

				MessageBox.Show("You need to create a valid 'config.csv' file in the directory.\n An example file has been created.");

				Debug.WriteLine(e.StackTrace);
			}
			return null;
		}

		// Update the config file to match the programs current state
		private void exportConfig()
		{
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

		// Close the program
		private void buttonExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
