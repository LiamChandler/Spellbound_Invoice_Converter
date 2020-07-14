using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spellbound_Invoice_Converter
{
	class csvConvert
	{
		public List<string> erroredLines;	// List to store inparseable lines
		public static DataTable dataTable;	// List to hold customer data

		public void ConvertCSV(string dataFile, string customerData)
		{
			// Parse CSV's to tables
			erroredLines = new List<string>();
			dataTable = ParseClientDataToTable(dataFile);
			DataTable companyInfoTable = ParseBusinessDataToTable(customerData);

			// Create and show data editor to allow user to update any incomplete client data
			string search = "[Agent reference]=''";
			if (csvConvert.dataTable.Select(search).Length > 0)
			{
				IncompleteDataEditor edit = new IncompleteDataEditor();
				edit.ShowDialog();
			}

			// Parse data from table
			bool added;
			Client currentClient;
			List<Agent> agents = new List<Agent>();

			foreach (DataRow row in dataTable.Rows)
			{
				added = false;
				currentClient = new Client();

				// Parse Data
				string[] tmp = ((string)row[dataTable.Columns.IndexOf("Date")]).Replace("\"", "").Split(',')[0].Split('/');
				tmp[2] = "20" + tmp[2];
				currentClient.date = new DateTime((int)Int32.Parse(tmp[2]), (int)Int32.Parse(tmp[1]), (int)Int32.Parse(tmp[0]));
				currentClient.orderNumber = (string)row[dataTable.Columns.IndexOf("Order number")];
				currentClient.agent = (string)row[dataTable.Columns.IndexOf("Agent code")];
				currentClient.name = (string)row[dataTable.Columns.IndexOf("Customer name")];
				currentClient.paidToAgent = (string)row[dataTable.Columns.IndexOf("Paid to agent")];
				currentClient.agentRefernce = (string)row[dataTable.Columns.IndexOf("Agent reference")];

				// Add client to an agent
				if (agents.Count != 0) // If agent list not empty
				{
					// Try to find clients agent
					foreach (Agent a in agents)
					{
						if (currentClient.agent.CompareTo(a.Name) == 0)
						{
							a.clients.Add(currentClient);
							added = true;
							break;
						}
					}
				}
				// If agent could not be found, create agent and add client
				if (!added)
				{
					Agent newAgent = new Agent(currentClient.agent);
					DataRow agentData = companyInfoTable.Rows.Find(newAgent.Name);
					if (agentData != null)
					{
						newAgent.EmailAddress = (string)agentData[companyInfoTable.Columns.IndexOf("EmailAddress")];
						newAgent.POAddressLine1 = (string)agentData[companyInfoTable.Columns.IndexOf("POAddressLine1")];
						newAgent.POAddressLine2 = (string)agentData[companyInfoTable.Columns.IndexOf("POAddressLine2")];
						newAgent.POAddressLine3 = (string)agentData[companyInfoTable.Columns.IndexOf("POAddressLine3")];
						newAgent.POAddressLine4 = (string)agentData[companyInfoTable.Columns.IndexOf("POAddressLine4")];
						newAgent.POCity = (string)agentData[companyInfoTable.Columns.IndexOf("POCity")];
						newAgent.PORegion = (string)agentData[companyInfoTable.Columns.IndexOf("PORegion")];
						newAgent.POPostalCode = (string)agentData[companyInfoTable.Columns.IndexOf("POPostalCode")];
						newAgent.POCountry = (string)agentData[companyInfoTable.Columns.IndexOf("POCountry")];
						newAgent.Discount = float.Parse((string)agentData[companyInfoTable.Columns.IndexOf("Discount")]);
					}

					newAgent.clients.Add(currentClient);
					agents.Add(newAgent);
				}
			}

			Debug.WriteLine("Saving data to files");
			printAgents(agents, dataFile);
			DialogResult dr = MessageBox.Show("Sucessfully converted " + dataTable.Rows.Count + " clients into " + agents.Count + " businesses.\nWould you like to be taken to the output location?", "Output", MessageBoxButtons.YesNo);

			if (dr == DialogResult.Yes)
			{
				// Open explorer window at output folder
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
				{
					FileName = (dataFile.Substring(0, dataFile.LastIndexOf(".")) + "Output\\"),
					UseShellExecute = true,
					Verb = "open"
				});
			}

		}

		protected DataTable ParseClientDataToTable(string strFilePath)
		{
			DataTable dt = new DataTable();

			StreamReader sr = new StreamReader(new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

			// Add header line to table
			string header = sr.ReadLine();
			erroredLines.Add(header);
			string[] headers = Regex.Split(header, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

			foreach (string h in headers)
			{
				dt.Columns.Add(h.Trim('"'));
			}

			// Add information from CSV to table
			while (!sr.EndOfStream)
			{
				string line = sr.ReadLine();
				try
				{
					string[] rows = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

					// Check if row is complete, if not find rest of line.
					if (rows.Length < dt.Columns.Count)
					{
						while (Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Length < dt.Columns.Count)
							line = string.Concat(line, " ", sr.ReadLine());
						rows = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
					}

					// Add row if agent column has an entry
					if (((string)rows[dt.Columns.IndexOf("Agent")]).CompareTo("") != 0)
					{
						DataRow dr = dt.NewRow();
						for (int i = 0; i < headers.Length; i++)
						{
							dr[i] = rows[i].Trim('"');
						}
						dt.Rows.Add(dr);
					}
				}
				catch (Exception e)
				{
					erroredLines.Add(line);
					Debug.WriteLine(e.StackTrace);
				}

			}
			Debug.WriteLine(dt.Rows.Count + " lines sucessfully parsed");

			// Debug file outputs
			//printTable(dt, strFilePath);
			if (erroredLines.Count > 1)
				printErrored(strFilePath);


			return dt;
		}

		// Returns contents of BusinessData.csv at a DataTable
		protected DataTable ParseBusinessDataToTable(string strFilePath)
		{
			DataTable dt = new DataTable();
			using (StreamReader sr = new StreamReader(strFilePath))
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

					DataRow dr = dt.NewRow();
					for (int i = 0; i < headers.Length; i++)
					{
						dr[i] = rows[i].Trim('"');
					}
					dt.Rows.Add(dr);
				}
			}

			DataColumn[] keyColumns = new DataColumn[1];
			keyColumns[0] = dt.Columns[0];
			dt.PrimaryKey = keyColumns;
			return dt;
		}

		// Output the given list as csv files to the given location
		protected void printAgents(List<Agent> agents, string path)
		{
			string editedPath = path.Substring(0, path.LastIndexOf(".")) + "Output\\";

			// Create path if doesn't exist
			if (!Directory.Exists(editedPath))
				Directory.CreateDirectory(editedPath);
			else
			{
				// Remove all files from dir
				DirectoryInfo dir = new DirectoryInfo(editedPath);
				foreach (System.IO.FileInfo file in dir.GetFiles()) file.Delete();
				foreach (System.IO.DirectoryInfo subDirectory in dir.GetDirectories()) subDirectory.Delete(true);
			}

			foreach (Agent a in agents)
			{
				try
				{
					string tmp =  editedPath + a.InvoiceNumber + '-' + a.Name.Replace("\"", "") + ".csv";

					StreamWriter sw = new StreamWriter(@tmp);
					a.saveClients(sw);
					sw.Flush();
					sw.Close();
				}
				catch
				{
					MessageBox.Show("Unable to save file:\n" + editedPath + "\nIs the file being used?");
				}
			}
		}

		// Print any un parseable lines to a file
		protected void printErrored(string path)
		{
			string editedPath = path.Substring(0, path.LastIndexOf(".")) + "Output\\";

			// Create path if doesn't exist
			if (!Directory.Exists(editedPath))
				Directory.CreateDirectory(editedPath);

			StreamWriter sw = new StreamWriter(@editedPath + "ErroredOutput.csv");
			foreach (string row in erroredLines)
			{
				sw.WriteLine(row);
			}
			sw.Flush();
			sw.Close();
		}
	}

	// Contains all information about a particular agent
	class Agent
	{
		public List<Client> clients = new List<Client>();

		public string Name;                                                     // Needed
		public string EmailAddress = "";
		public string POAddressLine1 = "";
		public string POAddressLine2 = "";
		public string POAddressLine3 = "";
		public string POAddressLine4 = "";
		public string POCity = "";
		public string PORegion = "";
		public string POPostalCode = "";
		public string POCountry = "";
		public string InvoiceNumber = "INV-" + SpellboundInvoiceConverter.getInvoiceNumber().ToString().PadLeft(4, '0');      // Needed
		public string Reference = "";
		public DateTime InvoiceDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);               // Needed
		public DateTime DueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20);
		public string InventoryItemCode = (string)SpellboundInvoiceConverter.config.Rows.Find("InventoryItemCode")[1];
		// Description                                                          // Needed
		// Quantity                                                             // Needed
		// UnitAmount                                                           // Needed
		public float Discount = 0;
		public string AccountCode = (string)SpellboundInvoiceConverter.config.Rows.Find("AccountCode")[1];    // Needed
		public string TaxType = (string)SpellboundInvoiceConverter.config.Rows.Find("TaxType")[1];        // Needed
		public string TrackingName1 = "";
		public string TrackingOption1 = "";
		public string TrackingName2 = "";
		public string TrackingOption2 = "";
		public string Currency = "NZD";
		public string BrandingTheme = "";

		public Agent(string Name)
		{
			this.Name = Name;
		}

		// Prints all client informatin out to the given StreamWriter
		public void saveClients(StreamWriter sw)
		{
			// Print Header
			sw.WriteLine("ContactName,EmailAddress,POAddressLine1,POAddressLine2,POAddressLine3,POAddressLine4,POCity,PORegion,POPostalCode,POCountry,InvoiceNumber,Reference,InvoiceDate,DueDate,InventoryItemCode,Description,Quantity,UnitAmount,Discount,AccountCode,TaxType,TrackingName1,TrackingOption1,TrackingName2,TrackingOption2,Currency,BrandingTheme");

			// Print each 
			foreach (Client curr in clients)
			{
				sw.Write(Name + ',');
				sw.Write(EmailAddress + ',');
				sw.Write(POAddressLine1 + ',');
				sw.Write(POAddressLine2 + ',');
				sw.Write(POAddressLine3 + ',');
				sw.Write(POAddressLine4 + ',');
				sw.Write(POCity + ',');
				sw.Write(PORegion + ',');
				sw.Write(POPostalCode + ',');
				sw.Write(POCountry + ',');
				sw.Write(InvoiceNumber + ',');
				sw.Write(Reference + ',');
				sw.Write(InvoiceDate.ToShortDateString() + ','); ;
				sw.Write(DueDate.ToShortDateString() + ',');
				sw.Write(InventoryItemCode + ',');
				sw.Write(curr.agentRefernce + " " + curr.date.ToString("MMM d") + " " + curr.name + ','); // Description
				sw.Write("1" + ',');			// Quantity
				sw.Write(curr.paidToAgent + ',');
				sw.Write(Discount.ToString() + ',');
				sw.Write(AccountCode + ',');
				sw.Write(TaxType + ',');
				sw.Write(TrackingName1 + ',');
				sw.Write(TrackingOption1 + ',');
				sw.Write(TrackingName2 + ',');
				sw.Write(TrackingOption2 + ',');
				sw.Write(Currency + ',');
				sw.WriteLine(BrandingTheme);

			}
			Debug.WriteLine("Saved " + Name + " to file");
		}
	}

	class Client
	{
		public DateTime date;
		public string orderNumber;
		public string agent;
		public string name;
		public string paidToAgent;
		public string agentRefernce;
	}
}
