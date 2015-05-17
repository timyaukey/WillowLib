using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Willowsoft.WillowLib.Data.Sql;

namespace Willowsoft.WillowLib.Data.Entity
{
    public interface IDbSession : IDisposable
    {
        IDbSession Activate();
        void AssertIdle();
        ITranScope CreateTranScope();
        void ForceDistributedTransaction();
        string ConnectionInfo { get; }
    }
}
