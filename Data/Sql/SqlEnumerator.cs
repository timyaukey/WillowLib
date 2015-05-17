using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.Data.Sql
{
    /// <summary>
    /// An IDisposable IEnumerator which uses a SqlDataReader to construct objects
    /// to return. Dispose() disposes the SqlDataReader and the PooledConnection
    /// used to create the SqlDataReader.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SqlEnumerator<T> : IEnumerator<T>, IDisposable
        where T : class
    {
        #region Private fields

        private SqlConnection mConnection;
        private SqlDataReader mReader;
        private PooledConnection mPooledCon;
        // null if MoveNext() has not been called, or MoveNext() returned false.
        private T mCurrent;
        private bool mDisposed;

        #endregion

        #region Protected members

        /// <summary>
        /// Create an object with a SqlDataReader created from the SqlCommand
        /// passed. The SqlConnection object from that SqlCommand will be returned
        /// to the specified ConnectionPool when Dispose() is called on the enumerator.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="connectionPool"></param>
        protected SqlEnumerator(SqlCommand cmd, PooledConnection pooledCon)
        {
            mConnection = cmd.Connection;
            mReader = cmd.ExecuteReader();
            mPooledCon = pooledCon;
            mCurrent = null;
            mDisposed = false;
        }

        protected SqlDataReader Reader
        {
            get { return mReader; }
        }

        protected int GetOrdinal(string name)
        {
            return mReader.GetOrdinal(name);
        }

        protected int GetInt32(int ordinal)
        {
            return mReader.GetInt32(ordinal);
        }

        protected byte GetByte(int ordinal)
        {
            return mReader.GetByte(ordinal);
        }

        protected string GetString(int ordinal)
        {
            return mReader.GetString(ordinal);
        }

        protected DateTime GetDateTime(int ordinal)
        {
            return mReader.GetDateTime(ordinal);
        }

        protected abstract T CreateEntity();

        #endregion

        #region IEnumerator<T> Members

        public T Current
        {
            get { return mCurrent; }
        }

        #endregion

        #region IDisposable Members

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
                    mPooledCon.Dispose();
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                mReader.Dispose();
            }
            mDisposed = true;
        }

        ~SqlEnumerator()
        {
            Dispose(false);
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return mCurrent; }
        }

        public bool MoveNext()
        {
            if (mReader.Read())
                mCurrent = CreateEntity();
            else
                mCurrent = null;
            return mCurrent != null;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
