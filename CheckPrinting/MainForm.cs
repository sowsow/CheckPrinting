using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckPrinting
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void textBoxInput_TextChanged(object sender, EventArgs e)
        {
            lbErrorMsg.Text = string.Empty;

            if (!string.IsNullOrEmpty(this.textBoxInput.Text))
            {  
                try
                {
                    textBoxOutput.Text = Utilities.ConvertNumberToPrinting(Decimal.Parse(this.textBoxInput.Text));
                }
                catch (Exception ex)
                {
                    lbErrorMsg.Text = ex.Message;
                }
            }
        }

        private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow 0-9, backspace, and decimal dots
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // Allow only 1 decimal dot
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }
    }
}
