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
        public static void ConvertCSV(String file)
        {
            DataTable t =  ConvertCSVtoDataTable(file);
            bool added;
            Client currentClient;
            List<Agent> agents = new List<Agent>();

            foreach(DataRow row in t.Rows)
            {
                added = false;
                currentClient = new Client();

                // Parse Data
                String[] tmp = ((string)row[t.Columns.IndexOf("Date - ISO")]).Substring(0, 10).Split('-');
                currentClient.date = new DateTime((int)Int32.Parse(tmp[0]), (int)Int32.Parse(tmp[1]), (int)Int32.Parse(tmp[2]));

                currentClient.orderNumber       = (string)row[t.Columns.IndexOf("Order number")];
                currentClient.agent             = (string)row[t.Columns.IndexOf("Agent")];
                currentClient.name              = (string)row[t.Columns.IndexOf("Customer name")];
                currentClient.paidToAgent       = float.Parse((string)row[t.Columns.IndexOf("Paid to agent")]);
                currentClient.commission        = float.Parse((string)row[t.Columns.IndexOf("Commission")]);
                currentClient.amountOwned       = currentClient.paidToAgent - currentClient.commission;
                currentClient.agentRefernce     = (string)row[t.Columns.IndexOf("Agent reference")];

                if (agents.Count != 0)
                {
                    foreach(Agent a in agents)
                    {
                        if(currentClient.agent.CompareTo(a.Name) == 0)
                        {
                            a.clients.Add(currentClient);
                            added = true;
                            break;
                        }
                    }
                }
                if(!added)
                {
                    Agent newAgent = new Agent(currentClient.agent);
                    newAgent.clients.Add(currentClient);
                    agents.Add(newAgent);
                }
            }
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = Regex.Split(sr.ReadLine(), @"\s*(?:""(?<val>[^""]*(""""[^""]*)*)""\s*|(?<val>[^,]*))(?:,|$)");
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = Regex.Split(sr.ReadLine(), @"\s*(?:""(?<val>[^""]*(""""[^""]*)*)""\s*|(?<val>[^,]*))(?:,|$)");
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    // Add row if agent exists
                    if (((string)dr[dt.Columns.IndexOf("Agent")]).CompareTo("") != 0)
                        dt.Rows.Add(dr);
                }

            }
            Debug.WriteLine(dt.Rows.Count + " lines found");
            return dt;
        }
    }

    class Agent
    {
        public List<Client> clients = new List<Client>();

        public String Name;
        public String EmailAddress;
        public String POAddressLine1;
        public String POAddressLine2;
        public String POAddressLine3;
        public String POAddressLine4;
        public String POCity;
        public String PORegion;
        public String POPostalCode;
        public String POCountry;
        public String InvoiceNumber;
        public String Reference;
        public DateTime InvoiceDate = DateTime.Today;
        public DateTime DueDate = DateTime.Today.AddDays(Form1.dueDateDays);
        // InventoryItemCode
        // Description
        // Quantity
        // UnitAmount
        // Discount
        // AccountCode - Links to InventoryItemCode
        public String TaxType = "15% GST on Income";
        public String TrackingName1;
        public String TrackingOption1;
        public String TrackingName2;
        public String TrackingOption2;
        public String Currency = "NZD";
        public String BrandingTheme;

        public Agent(String Name)
        {
            this.Name = Name;
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
