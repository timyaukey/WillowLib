using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.Data.Sql
{
    public abstract class SqlEntityRepository<TPersistable, TId, TDataRow>
        : IEntityRepository<TPersistable, TId>
        where TId : IEntityId
        where TPersistable : IPersistableEntity
        where TDataRow : DataRow
    {
        private SqlDbSession mSession;

        public SqlEntityRepository(SqlDbSession session)
        {
            mSession = session;
        }

        public virtual TPersistable Get(TId id)
        {
            using (PooledConnection pooledCon = GetPooledConnection())
            {
                using (SqlDataAdapter adapter = SqlHelper.CreateSelectAdapter(
                    "dbo.Get" + EntityName, pooledCon))
                {
                    using (adapter.SelectCommand)
                    {
                        SqlHelper.AddParamInputId(adapter.SelectCommand, EntityIdVarName, id.Value);
                        return CreateEntities(adapter)[0];
                    }
                }
            }
        }

        public virtual void Insert(TPersistable entity)
        {
            using (PooledConnection pooledCon = GetPooledConnection())
            {
                using (SqlCommand cmd = SqlHelper.CreateProc("dbo.Insert" + EntityName, pooledCon))
                {
                    SqlParameter param = SqlHelper.AddParamOutputId(cmd, EntityIdVarName);
                    AddInsertUpdateParams(cmd, entity);
                    cmd.ExecuteNonQuery();
                    entity.SetIdValue((int)param.Value);
                }
            }
        }

        public virtual void Update(TPersistable entity)
        {
            using (PooledConnection pooledCon = GetPooledConnection())
            {
                using (SqlCommand cmd = SqlHelper.CreateProc("dbo.Update" + EntityName, pooledCon))
                {
                    SqlHelper.AddParamInputId(cmd, EntityIdVarName, entity.GetIdValue());
                    AddInsertUpdateParams(cmd, entity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public virtual void Delete(TPersistable entity)
        {
            using (PooledConnection pooledCon = GetPooledConnection())
            {
                using (SqlCommand cmd = SqlHelper.CreateProc("dbo.Delete" + EntityName, pooledCon))
                {
                    SqlHelper.AddParamInputId(cmd, EntityIdVarName, entity.GetIdValue());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public delegate void AddSqlParametersDelegate(SqlCommand cmd);

        public List<TPersistable> Search(string procName, AddSqlParametersDelegate addParams)
        {
            using (PooledConnection pooledCon = GetPooledConnection())
            {
                using (SqlDataAdapter adapter = SqlHelper.CreateSelectAdapter(procName, pooledCon))
                {
                    using (adapter.SelectCommand)
                    {
                        addParams(adapter.SelectCommand);
                        return CreateEntities(adapter);
                    }
                }
            }
        }

        public void ExecuteNonQuery(string procName, AddSqlParametersDelegate addParams)
        {
            using (PooledConnection pooledCon = GetPooledConnection())
            {
                using (SqlCommand cmd = SqlHelper.CreateProc(procName, pooledCon))
                {
                    addParams(cmd);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Subclasses may call this to construct IList<TPersistable> to return
        // from search methods.
        [DebuggerStepThrough]
        protected List<TPersistable> CreateEntities(IEnumerable<TDataRow> dataRows)
        {
            List<TPersistable> entities = new List<TPersistable>();
            foreach (TDataRow dataRow in dataRows)
            {
                entities.Add(CreateEntity(dataRow));
            }
            return entities;
        }

        public abstract List<TPersistable> CreateEntities(SqlDataAdapter adapter);

        // Create and return a TPersistable from a TDataRow.
        protected abstract TPersistable CreateEntity(TDataRow dataRow);

        protected abstract void AddInsertUpdateParams(SqlCommand cmd, TPersistable entity);

        protected abstract string EntityName { get; }

        protected virtual string EntityIdVarName
        {
            [DebuggerStepThrough]
            get { return "@" + EntityName + "Id"; }
        }

        [DebuggerStepThrough]
        protected virtual PooledConnection GetPooledConnection()
        {
            return mSession.GetConnection();
        }
    }
}
