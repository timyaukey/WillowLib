using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.Data.Sql
{
    public class SqlHelper
    {
        /// <summary>
        /// Return a new SqlDataAdapter whose SelectCommand is a new SqlCommand object
        /// created to execute the named stored procedure using a SqlConnection from
        /// the specified PooledConnection. This method should be used in a using() block
        /// to insure the PooledConnection is disposed.
        /// </summary>
        /// <param name="procName">The name of the stored procedure to execute.</param>
        /// <param name="pooledCon">The PooledConnection to use.</param>
        /// <returns>The SqlDataAdapter object.</returns>
        [DebuggerStepThrough]
        public static SqlDataAdapter CreateSelectAdapter(string procName, PooledConnection pooledCon)
        {
            SqlCommand cmd = SqlHelper.CreateProc(procName, pooledCon);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            return adapter;
        }

        /// <summary>
        /// Return a new SqlCommand object to execute the named stored procedure
        /// on the specified PooledConnection. The SqlCommand may return a result set
        /// or not. This method should be used in a using() block to insure the 
        /// PooledConnection is disposed.
        /// </summary>
        /// <param name="procName">The stored procedure name.</param>
        /// <param name="con">The connection to use.</param>
        /// <returns>The SqlCommand object.</returns>
        [DebuggerStepThrough]
        public static SqlCommand CreateProc(string procName, PooledConnection pooledCon)
        {
            SqlCommand cmd = new SqlCommand(procName, pooledCon.Con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 60 * 60;
            return cmd;
        }

        [DebuggerStepThrough]
        public static void AddParamVarchar(SqlCommand cmd, string paramName, string value)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.VarChar);
            param.Value = value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }

        [DebuggerStepThrough]
        public static void AddParamDatetime(SqlCommand cmd, string paramName, DateTime value)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.DateTime);
            param.Value = value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }

        //[DebuggerStepThrough]
        public static void AddParamDatetime(SqlCommand cmd, string paramName, DateTime? value)
        {
            // 1980-01-01T00:00:00+00:00
            SqlParameter param = new SqlParameter(paramName, SqlDbType.DateTime);
            param.IsNullable = true;
            if (value.HasValue)
                param.Value = value;
            else
                param.Value = DBNull.Value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }

        [DebuggerStepThrough]
        public static void AddParamInt(SqlCommand cmd, string paramName, int value)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.Int);
            param.Value = value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }

        [DebuggerStepThrough]
        public static SqlParameter AddParamOutputInt(SqlCommand cmd, string paramName)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);
            return param;
        }

        [DebuggerStepThrough]
        public static void AddParamMoney(SqlCommand cmd, string paramName, decimal value)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.Money);
            param.Value = value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }

        [DebuggerStepThrough]
        public static void AddParamTinyint(SqlCommand cmd, string paramName, int value)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.TinyInt);
            param.Value = value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }

        [DebuggerStepThrough]
        public static void AddParamTinyint(SqlCommand cmd, string paramName, bool value)
        {
            AddParamTinyint(cmd, paramName, value ? 1 : 0);
        }

        [DebuggerStepThrough]
        public static SqlParameter AddParamOutputId(SqlCommand cmd, string paramName)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);
            return param;
        }

        [DebuggerStepThrough]
        public static void AddParamInputId(SqlCommand cmd, string paramName, int value)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.Int);
            if (value == 0)
            {
                param.IsNullable = true;
                param.Value = DBNull.Value;
            }
            else
                param.Value = value;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
        }
    }
}
