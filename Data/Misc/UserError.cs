using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willowsoft.WillowLib.Data.Misc
{
    public enum ErrorSeverity
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Severe = 3
    }

    public class UserError
    {
        private ErrorSeverity mSeverity;
        private string mMessage;

        public UserError(ErrorSeverity severity, string message)
        {
            mSeverity = severity;
            mMessage = message;
        }

        public ErrorSeverity Severity
        {
            get { return mSeverity; }
        }

        public string Message
        {
            get { return mMessage; }
        }
    }

    public class WarningError : UserError
    {
        public WarningError(string message)
            : base(ErrorSeverity.Warning, message)
        {
        }
    }

    public class SevereError : UserError
    {
        public SevereError(string message)
            : base(ErrorSeverity.Severe, message)
        {
        }
    }

    public class ErrorList : List<UserError>
    {
        public ErrorSeverity MaxSeverity
        {
            get
            {
                ErrorSeverity maxSeverity = ErrorSeverity.None;
                foreach (UserError error in this)
                {
                    if (error.Severity > maxSeverity)
                        maxSeverity = error.Severity;
                }
                return maxSeverity;
            }
        }

        public void AddSevere(string message, params object[] args)
        {
            Add(new SevereError(string.Format(message, args)));
        }
    }
}
