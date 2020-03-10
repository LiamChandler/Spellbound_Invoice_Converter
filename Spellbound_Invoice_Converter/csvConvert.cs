using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Spellbound_Invoice_Converter
{
    class csvConvert
    {
        csvConvert()
        {

        }

        public static void ConvertCSV(String file)
        {
            // Make variables
            List<List<String>> lines = new List<List<string>>();
            String line;

            // Read file contents into 2D list
            try
            {
                StreamReader sr = new StreamReader(file);
                line = sr.ReadLine();

                while(line != null)
                {
                    lines.Add(new List<string>(line.Split(',')));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            //  Sort data into groups by agent

            List<Agent> agents = new List<Agent>();
            List<List<String>> erroredLines = new List<List<string>>();

            bool firstEntry = true;
            foreach(List<string> currentLine in lines)
            {
                // Check if the first line
                if (!firstEntry)
                {
                    try
                    {
                        Client currentClient = new Client();

                        // Parse Data
                        String[] tmp = currentLine[0].Substring(0,10).Split('-');
                        currentClient.date = new DateTime((int)Int32.Parse(tmp[0]), (int)Int32.Parse(tmp[1]), (int)Int32.Parse(tmp[2]));
                        currentClient.orderNumber = currentLine[45];
                        currentClient.agent = currentLine[6];
                        currentClient.name = currentLine[16];
                        currentClient.paidToAgent = float.Parse(currentLine[39]);
                        currentClient.commission = float.Parse(currentLine[42]);
                        currentClient.amountOwned = currentClient.paidToAgent - currentClient.commission;
                        currentClient.agentRefernce = currentLine[45];

                        // Add Client to Agent, if no matching agent exists create one
                        bool added = false;
                        if(agents.Count != 0)
                        {
                            foreach(Agent a in agents)
                            {
                                if(a.Name.CompareTo(currentClient.agent) == 0)
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
                            newAgent.clients.Add(currentClient);
                            agents.Add(newAgent);
                        }
                    }
                    //  Catch errored lines and add them to the errored list
                    catch
                    {
                        Debug.WriteLine("Could not parse line: " + currentLine);
                        erroredLines.Add(currentLine);
                    }
                }
                else
                    firstEntry = false;
            }
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
