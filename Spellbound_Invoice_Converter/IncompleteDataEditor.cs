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
    public partial class IncompleteDataEditor : Form
    {
        DataRow[] rows = csvConvert.dataTable.Select("[Agent reference]=''");
        int lastIndex = -1;
        int index = -1;

        public IncompleteDataEditor()
        {
            InitializeComponent();
            updateDetails();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            lastIndex = index;
            if (index == 0)
                index = rows.Length - 1;
            else
                index--;
            updateDetails();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            lastIndex = index;
            if (index == rows.Length - 1)
                index = 0;
            else
                index++;
            updateDetails();
        }

        private void updateDetails()
        {
            // Check if it's the first run.
            if (index == -1)
                index = 0;
            else
            {
                // Save Data
                rows[lastIndex][csvConvert.dataTable.Columns.IndexOf("Agent reference")] = textBoxAgentRefernce.Text;
            }

            // Update Title
            labelNumber.Text = "Customer " + (index + 1) + " of " + rows.Length;

            // Update Details

            String[] tmp = ((string)rows[index][csvConvert.dataTable.Columns.IndexOf("Date")]).Replace("\"", "").Split(',')[0].Split('/');
            tmp[2] = "20" + tmp[2];
            textBoxDate.Text = new DateTime((int)Int32.Parse(tmp[2]), (int)Int32.Parse(tmp[1]), (int)Int32.Parse(tmp[0])).ToLongDateString();

            textBoxOrderNumber.Text = (string)rows[index][csvConvert.dataTable.Columns.IndexOf("Order number")];
            textBoxAgent.Text = (string)rows[index][csvConvert.dataTable.Columns.IndexOf("Agent")];
            textBoxCustomerNumber.Text = (string)rows[index][csvConvert.dataTable.Columns.IndexOf("Customer name")];
            textBoxAgentRefernce.Text = (string)rows[index][csvConvert.dataTable.Columns.IndexOf("Agent reference")];
            textBoxInternalNotes.Text = (string)rows[index][csvConvert.dataTable.Columns.IndexOf("Internal notes")];

            textBoxAgentRefernce.Focus();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
