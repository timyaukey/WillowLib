using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Willowsoft.WillowLib.Data.Sql
{
    // Abstract the TransactionScope interface, to allow 
    // mocking it in unit tests.
    public interface ITranScope : IDisposable
    {
        void Complete();
    }

    // Implementation of ITranScope which uses a real TransactionScope.
    public class TransactionScopeWrapper : ITranScope
    {
        private TransactionScope mInnerScope;

        public TransactionScopeWrapper()
        {
            // Create a TransactionScope with a reasonably long timeout period.
            // The default timeout period is short enough that it frequently 
            // expires when walking through code in the debugger.
            mInnerScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromHours(8));
        }

        public void Dispose()
        {
            mInnerScope.Dispose();
        }

        public void Complete()
        {
            mInnerScope.Complete();
        }
    }
}
