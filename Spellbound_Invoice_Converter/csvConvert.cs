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
            LinkedList<LinkedList<String>> lines = new LinkedList<LinkedList<string>>();
            String line;

            try
            {
                StreamReader sr = new StreamReader(file);
                line = sr.ReadLine();

                while(line != null)
                {
                    lines.AddLast(new LinkedList<string>(line.Split(',')));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                foreach(LinkedList<String> l in lines)
                {
                    int i = 0;
                    foreach(String s in l)
                    {
                        Debug.Write(i + " = " + s + '\t');
                        i++;
                    }
                    Debug.Write("\n");
                }
            }
        }
    }
}
