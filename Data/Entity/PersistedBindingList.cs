using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Willowsoft.WillowLib.Data.Sql;

namespace Willowsoft.WillowLib.Data.Entity
{
    // A BindingList<> of IPersistable objects with abstract methods to handle persistence.
    public abstract class PersistedBindingList<TPersistable> : BindingList<TPersistable>
        where TPersistable : IPersistable
    {
        private IDbSession mSession;

        protected PersistedBindingList(IDbSession session)
        {
            mSession = session;
        }

        protected override void RemoveItem(int index)
        {
            TPersistable entity = Items[index];
            using (ITranScope trans = mSession.CreateTranScope())
            {
                using (mSession.Activate())
                {
                    Delete(entity);
                }
                trans.Complete();
            }
            entity.IsDeleted = true;
            base.RemoveItem(index);
        }

        protected abstract void Delete(TPersistable entity);

        // Called by BindingSourceHelper when it receives a CurrentChanged event
        // from the BindingSource and the old current entity is dirty, to commit
        // the old current entity to the repository.
        public void Save(TPersistable entity)
        {
            if (!entity.IsDeleted)
            {
                using (ITranScope trans = mSession.CreateTranScope())
                {
                    using (mSession.Activate())
                    {
                        if (entity.IsPersisted)
                            Update(entity);
                        else
                            Insert(entity);
                    }
                    trans.Complete();
                }
            }
        }

        protected abstract void Insert(TPersistable entity);

        protected abstract void Update(TPersistable entity);

        /// <summary>
        /// Used to add to the internal list in memory existing entities
        /// retrieved from a repository. This is generally called before
        /// displaying these entities in a UI somewhere.
        /// </summary>
        /// <param name="entities"></param>
        public void Add(IEnumerable<TPersistable> entities)
        {
            foreach (TPersistable entity in entities)
            {
                Add(entity);
            }
        }

        /// <summary>
        /// The entity name used in the UI.
        /// </summary>
        public abstract string EntityDisplayName { get; }
    }
}
