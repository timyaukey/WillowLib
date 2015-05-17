using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.Data.Sql
{
    /// <summary>
    /// A pool of reusable SqlConnection objects opened with the same 
    /// connection string. SqlConnection objects are obtained with Allocate(),
    /// which will create and open them as necessary. Once returned by Allocate(),
    /// a SqlConnection will not be returned again until that SqlConnection is
    /// passed to Free(). Free() merely makes the SqlConnection available again,
    /// it does not close it.
    /// </summary>
    public class ConnectionPool : IDisposable
    {
        // Connection string shared by all SqlConnection objects in pool.
        // May be null if repositories will not call Allocate(), e.g. unit
        // test repositories backed by in-memory stores.
        private string mConnectionString;
        // All SqlConnection objects available to be returned by Allocate().
        private IList<SqlConnection> mFreeConnections;
        // All SqlConnection objects acceptable to Free().
        private IList<SqlConnection> mAllocatedConnections;
        // True iff Dispose(bool) has been called.
        private bool mDisposed;

        [DebuggerStepThrough]
        public ConnectionPool(string connectionString)
        {
            mConnectionString = connectionString;
            mFreeConnections = new List<SqlConnection>();
            mAllocatedConnections = new List<SqlConnection>();
        }

        /// <summary>
        /// Return a SqlConnection object, already opened. Pass the SqlConnection
        /// to Free() when you are done with it. Do not call Close() or Dispose()
        /// directly on this object, instead let ConnectionScope.Dispose() call 
        /// ConnectionPool.Dispose() when you are done using the ConnectionScope.
        /// </summary>
        /// <returns>The SqlConnection object.</returns>
        [DebuggerStepThrough]
        internal SqlConnection Allocate()
        {
            SqlConnection result;
            if (mFreeConnections.Count > 0)
            {
                result = mFreeConnections[0];
                mFreeConnections.Remove(result);
            }
            else
            {
                result = new SqlConnection(mConnectionString);
                result.Open();
            }
            // If there are more than 5 connections to the same database
            // then there is probably a connection leak somewhere.
            Debug.Assert(mAllocatedConnections.Count < 6);
            mAllocatedConnections.Add(result);
            return result;
        }

        /// <summary>
        /// Debugging assertion that the pool has no allocated connections.
        /// </summary>
        [DebuggerStepThrough]
        public void AssertIdle()
        {
            Debug.Assert(mAllocatedConnections.Count == 0);
        }

        /// <summary>
        /// Return a SqlConnection object to the pool, which makes it
        /// available to be returned by later calls to Allocate().
        /// </summary>
        /// <param name="con">The SqlConnection object.</param>
        [DebuggerStepThrough]
        internal void Free(SqlConnection con)
        {
            mAllocatedConnections.Remove(con);
            mFreeConnections.Add(con);
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.mDisposed)
            {
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                foreach (SqlConnection con in mAllocatedConnections)
                {
                    con.Dispose();
                }
                foreach (SqlConnection con in mFreeConnections)
                {
                    con.Dispose();
                }
                // If disposing equals true, dispose all managed resources.
                if (disposing)
                {
                    mAllocatedConnections.Clear();
                    mFreeConnections.Clear();
                }
            }
            mDisposed = true;
        }

        ~ConnectionPool()
        {
            Dispose(false);
        }
    }
}
