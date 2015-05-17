using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.Data.Sql
{
    public class SqlDbSession : IDbSession
    {
        private string mConnectionString;
        private ConnectionPool mPool;

        public SqlDbSession(string databaseKeyword)
        {
            mConnectionString = ConfigurationManager.ConnectionStrings[databaseKeyword].ConnectionString;
            mPool = null;
        }

        public IDbSession Activate()
        {
            mPool = new ConnectionPool(mConnectionString);
            return this;
        }

        public void Dispose()
        {
            if (mPool != null)
                mPool.Dispose();
            mPool = null;
        }

        public PooledConnection GetConnection()
        {
            if (mPool == null)
                throw new InvalidOperationException("SqlDbSession.Activate() must be called before GetConnection()");
            return new PooledConnection(mPool);
        }

        /// <summary>
        /// Debugging assertion that the main pool has no allocated connections.
        /// </summary>
        public void AssertIdle()
        {
            mPool.AssertIdle();
        }

        /// <summary>
        /// Create an ITranScope. This is how applications should enclose
        /// activities in a transaction, not by creating TransactionScope
        /// objects directly. Other implementations of IDbSession can return other
        /// implementations of ITranScope for unit testing which don't create
        /// a real TransactionScope internally.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public ITranScope CreateTranScope()
        {
            return new TransactionScopeWrapper();
        }

        /// <summary>
        /// Force the current TransactionScope to escalate from a local SQL Server
        /// transaction to a distributed transaction. This happens automatically 
        /// when more than one SqlConnection enlists in the TransactionScope, but
        /// that may generate an exception if any of those connections is doing
        /// anything which prevents the promotion. The known case of this is if
        /// a SqlConnection is already in use by a SqlDataReader, for example you
        /// get a SqlEnumerator and then attempt to open another SqlConnection.
        /// </summary>
        [DebuggerStepThrough]
        public void ForceDistributedTransaction()
        {
            using (new PooledConnection(mPool))
            {
                using (new PooledConnection(mPool))
                {
                }
            }
        }

        public string ConnectionInfo
        {
            [DebuggerStepThrough]
            get { return mConnectionString; }
        }
    }
}
