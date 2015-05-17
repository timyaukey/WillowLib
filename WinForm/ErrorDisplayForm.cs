using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.WinForm
{
    public partial class ErrorDisplayForm : Form
    {
        private ErrorList mErrors;

        private ErrorDisplayForm()
        {
            InitializeComponent();
            mErrors = null;
        }

        public static void Show(ErrorList errors)
        {
            ErrorDisplayForm frm = new ErrorDisplayForm();
            frm.ShowInternal(errors);
        }

        private void ShowInternal(ErrorList errors)
        {
            mErrors = errors;
            errorList.Items.Clear();
            foreach (UserError error in mErrors)
            {
                string severityCode=string.Empty;
                switch (error.Severity)
                {
                    case ErrorSeverity.Info: severityCode = "info"; break;
                    case ErrorSeverity.Warning: severityCode = "warning"; break;
                    case ErrorSeverity.Severe: severityCode = "severe"; break;
                }
                string txt = string.Format("({0}) {1}", severityCode, error.Message);
                errorList.Items.Add(txt);
            }
            this.ShowDialog();
        }
    }
}
