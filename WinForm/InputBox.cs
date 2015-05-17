using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Willowsoft.WillowLib.WinForm
{
    /// <summary>
    /// A modal dialog which asks the user to enter a one line string.
    /// </summary>
    public partial class InputBox : Form
    {
        private string mResponse;

        public InputBox()
        {
            mResponse = null;
            InitializeComponent();
        }

        /// <summary>
        /// Display the window modally.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="windowCaption"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string Show(string prompt, string windowCaption, string defaultValue)
        {
            lblInstructions.Text = prompt;
            this.Text = windowCaption;
            txtInput.Text = defaultValue;
            mResponse = null;
            this.ShowDialog();
            return mResponse;
        }

        public static string Ask(string prompt, string windowCaption, string defaultValue)
        {
            using (InputBox frm = new InputBox())
            {
                return frm.Show(prompt, windowCaption, defaultValue);
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOkay_Click(object sender, EventArgs e)
        {
            mResponse = txtInput.Text;
            this.Close();
        }
    }
}
