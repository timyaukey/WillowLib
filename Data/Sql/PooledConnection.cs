using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Willowsoft.WillowLib.Data.Sql
{
    /// <summary>
    /// An IDisposable container for a SqlConnection obtained from a ConnectionPool.
    /// Dispose() will return the SqlConnection to the ConnectionPool it was
    /// allocated from, without closing it.
    /// </summary>
    public class PooledConnection : IDisposable
    {
        private SqlConnection mCon;
        private ConnectionPool mPool;
        private bool mDisposed;

        [DebuggerStepThrough]
        public PooledConnection(ConnectionPool pool)
        {
            mDisposed=false;
            mPool = pool;
            mCon = mPool.Allocate();
        }

        public SqlConnection Con
        {
            [DebuggerStepThrough]
            get { return mCon; }
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
                // If disposing equals true, dispose all managed resources.
                if (disposing)
                {
                    mPool.Free(mCon);
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
            }
            mDisposed = true;
        }

        ~PooledConnection()
        {
            Dispose(false);
        }
    }
}
