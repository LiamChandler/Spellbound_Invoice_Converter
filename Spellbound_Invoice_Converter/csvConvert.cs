using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Text.RegularExpressions;


namespace Spellbound_Invoice_Converter
{
    class csvConvert
    {
        public List<string> erroredLines;
        public void ConvertCSV(string dataFile, string customerData)
        {
            // Parse CSV's to tables
            erroredLines = new List<string>();
            DataTable dataTable = ParseClientDataToTable(dataFile);
            DataTable companyInfoTable = ParseCompanyDataToTable(customerData);

            // Parse data from table
            bool added;
            Client currentClient;
            List<Agent> agents = new List<Agent>();

            foreach (DataRow row in dataTable.Rows)
            {
                added = false;
                currentClient = new Client();

                // Parse Data
                String[] tmp = ((string)row[dataTable.Columns.IndexOf("Date - ISO")]).Substring(0, 10).Split('-');
                currentClient.date = new DateTime((int)Int32.Parse(tmp[0]), (int)Int32.Parse(tmp[1]), (int)Int32.Parse(tmp[2]));

                currentClient.orderNumber = (string)row[dataTable.Columns.IndexOf("Order number")];
                currentClient.agent = (string)row[dataTable.Columns.IndexOf("Agent code")];
                currentClient.name = (string)row[dataTable.Columns.IndexOf("Customer name")];
                currentClient.paidToAgent = float.Parse((string)row[dataTable.Columns.IndexOf("Paid to agent")]);
                currentClient.commission = float.Parse((string)row[dataTable.Columns.IndexOf("Commission")]);
                currentClient.amountOwned = currentClient.paidToAgent - currentClient.commission;
                currentClient.agentRefernce = (string)row[dataTable.Columns.IndexOf("Agent reference")];

                if (agents.Count != 0)
                {
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
                if (!added)
                {
                    Agent newAgent = new Agent(currentClient.agent);
                    DataRow agentData = companyInfoTable.Rows.Find( newAgent.Name);
                    if(agentData != null)
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
        }

        protected DataTable ParseClientDataToTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                // Add header line to table
                string header = sr.ReadLine();
                erroredLines.Add(header);
                string[] headers = Regex.Split(header, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                foreach (string h in headers)
                {
                    dt.Columns.Add(h);
                }

                // Add information from CSV to table
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    try
                    {
                        string[] rows = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        // Add row if agent exists
                        if (rows.Length < dt.Columns.Count)
                        {
                            erroredLines.Add(line);
                        }
                        else if (((string)rows[dt.Columns.IndexOf("Agent")]).CompareTo("") != 0)
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                dr[i] = rows[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    catch
                    {
                        erroredLines.Add(line);
                    }
                }
            }
            Debug.WriteLine(dt.Rows.Count + " lines sucessfully parsed");

            // Debug file outputs
            //printTable(dt, strFilePath);
            if (erroredLines.Count > 1)
                printErrored(strFilePath);

            return dt;
        }
        
        protected DataTable ParseCompanyDataToTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
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

                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }

            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = dt.Columns[0];
            dt.PrimaryKey = keyColumns;
            return dt;
        }

        protected void printTable(DataTable table, string path)
        {
            string editedPath = path.Substring(0, path.LastIndexOf(".")) + "Output\\";

            // Create path if doesn't exist
            if (!Directory.Exists(editedPath))
                Directory.CreateDirectory(editedPath);

            StreamWriter sw = new StreamWriter(@editedPath + "TableOutput.csv");
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i != 0)
                        sw.Write(",");
                    sw.Write((string)row.ItemArray[i]);
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();
        }

        protected void printAgents(List<Agent> agents, string path)
        {
            foreach (Agent a in agents)
            {
                string editedPath = path.Substring(0, path.LastIndexOf(".")) + "Output\\";

                // Create path if doesn't exist
                if (!Directory.Exists(editedPath))
                    Directory.CreateDirectory(editedPath);

                StreamWriter sw = new StreamWriter(@editedPath + a.Name.Replace("\"", "") + ".csv");
                a.saveClients(sw);
                sw.Flush();
                sw.Close();
            }
        }

        protected void printErrored(string path)
        {
            string editedPath = path.Substring(0, path.LastIndexOf(".")) + "Output\\";

            // Create path if doesn't exist
            if(!Directory.Exists(editedPath))
                Directory.CreateDirectory(editedPath);

            StreamWriter sw = new StreamWriter(@editedPath + "ErroredOutput.csv");
            foreach (String row in erroredLines)
            {
                sw.WriteLine(row);
            }
            sw.Flush();
            sw.Close();
        }
    }

    class Agent
    {
        public List<Client> clients = new List<Client>();

        public String Name;                                                     // Needed
        public String EmailAddress = "";
        public String POAddressLine1 = "";
        public String POAddressLine2 = "";
        public String POAddressLine3 = "";
        public String POAddressLine4 = "";
        public String POCity = "";
        public String PORegion = "";
        public String POPostalCode = "";
        public String POCountry = "";
        public String InvoiceNumber = SpellboundInvoiceConverter.getInvoiceNumber().ToString();      // Needed
        public String Reference = "";
        public DateTime InvoiceDate = DateTime.Today;                           // Needed
        public DateTime DueDate = DateTime.Today.AddDays(SpellboundInvoiceConverter.dueDateDays);    // Needed
        public String InventoryItemCode = (string)SpellboundInvoiceConverter.config.Rows.Find("InventoryItemCode")[1];
        // Description                                                          // Needed
        // Quantity                                                             // Needed
        // UnitAmount                                                           // Needed
        public float Discount = 0;
        public String AccountCode = (string)SpellboundInvoiceConverter.config.Rows.Find("AccountCode")[1];    // Needed
        public String TaxType = (string)SpellboundInvoiceConverter.config.Rows.Find("TaxType")[1];        // Needed
        public String TrackingName1 = "";
        public String TrackingOption1 = "";
        public String TrackingName2 = "";
        public String TrackingOption2 = "";
        public String Currency = "NZD";
        public String BrandingTheme = "";

        public Agent(String Name)
        {
            this.Name = Name;
        }

        public void saveClients(StreamWriter sw)
        {
            // Print Header
            sw.WriteLine("ContactName,EmailAddress,POAddressLine1,POAddressLine2,POAddressLine3,POAddressLine4,POCity,PORegion,POPostalCode,POCountry,InvoiceNumber,Reference,InvoiceDate,DueDate,InventoryItemCode,Description,Quantity,UnitAmount,Discount,AccountCode,TaxType,TrackingName1,TrackingOption1,TrackingName2,TrackingOption2,Currency,BrandingTheme");

            // Print each 
            foreach(Client c in clients)
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
                sw.Write(c.agentRefernce + " " + c.date.ToString("MMM d") + " " + c.name + ',');
                sw.Write("1" + ',');    // Quantity
                sw.Write(c.amountOwned.ToString() + ',');
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
        public String orderNumber;
        public String agent;
        public String name;
        public float paidToAgent;
        public float commission;
        public float amountOwned;
        public String agentRefernce;
    }
}
