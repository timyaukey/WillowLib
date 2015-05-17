using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Willowsoft.WillowLib.Data.Sql;

namespace Willowsoft.WillowLib.Data.Entity
{
    /// <summary>
    /// A PersistedBindingList<> which persists individual objects to a 
    /// single IEntityRepository<>.
    /// </summary>
    /// <typeparam name="TPersistable"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class EntityBindingList<TPersistable, TId> : PersistedBindingList<TPersistable>
        where TId : IEntityId
        where TPersistable : IPersistableEntity
    {
        private IEntityRepository<TPersistable, TId> mRep;

        protected EntityBindingList(IDbSession session, IEntityRepository<TPersistable, TId> rep)
            : base(session)
        {
            mRep = rep;
        }

        protected override void Delete(TPersistable entity)
        {
            Repository.Delete(entity);
        }

        protected override void Insert(TPersistable entity)
        {
            Repository.Insert(entity);
        }

        protected override void Update(TPersistable entity)
        {
            Repository.Update(entity);
        }

        /// <summary>
        /// The repository used for all persistence operations.
        /// </summary>
        protected IEntityRepository<TPersistable, TId> Repository { get { return mRep; } }
    }
}
