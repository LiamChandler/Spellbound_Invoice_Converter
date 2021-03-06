﻿using System;
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
		// Select rows with incomplete data
		DataRow[] rows = csvConvert.invoiceDataTable.Select("[Paid to agent] like '' OR [Agent comments] like ''");

		// Set inital values
		int lastIndex = -1;
		int index = -1;

		public IncompleteDataEditor()
		{
			InitializeComponent();
			updateDetails();
		}

		// Select next customer and loop if at end of list
		private void buttonNext_Click(object sender, EventArgs e)
		{
			lastIndex = index;
			if (index == rows.Length - 1)
				index = 0;
			else
				index++;
			updateDetails();
		}

		// Select previous customer and loop if at start of list
		private void buttonPrevious_Click(object sender, EventArgs e)
		{
			lastIndex = index;
			if (index == 0)
				index = rows.Length - 1;
			else
				index--;
			updateDetails();
		}

		// Save the current data in window and update to next customer
		private void updateDetails()
		{
			// Check if it's the first run.
			if (index == -1)
				index = 0;
			else if(lastIndex != -1)
			{
				// Save Data
				rows[lastIndex][csvConvert.invoiceDataTable.Columns.IndexOf("Agent comments")] = textBoxAgentRefernce.Text;
				rows[lastIndex][csvConvert.invoiceDataTable.Columns.IndexOf("Paid to agent")] = textBoxPaidToAgent.Text;
			}

			// Update Title
			labelNumber.Text = "Customer " + (index + 1) + " of " + rows.Length;

			// Update Details

			try
			{
				String[] tmp = ((string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Booked Date")]).Replace("\"", "").Split(',')[0].Split('/');
				tmp[2] = "20" + tmp[2];
				textBoxDate.Text = new DateTime((int)Int32.Parse(tmp[2]), (int)Int32.Parse(tmp[1]), (int)Int32.Parse(tmp[0])).ToLongDateString();

				textBoxOrderNumber.Text = (string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Order number")];
				textBoxAgent.Text = (string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Agent")];
				textBoxCustomerNumber.Text = (string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Customer name")];

				textBoxPaidToAgent.Text = (string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Paid to agent")];

				textBoxAgentRefernce.Text = (string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Agent comments")];
				textBoxInternalNotes.Text = (string)rows[index][csvConvert.invoiceDataTable.Columns.IndexOf("Internal notes")];

				textBoxAgentRefernce.Focus();
			}
			catch(Exception e)
            {
				MessageBox.Show("Something went wrong reading the data.\n" + e.Message);
            }
		}

		private void buttonFinish_Click(object sender, EventArgs e)
		{
			updateDetails();
			this.Close();
		}
	}
}
