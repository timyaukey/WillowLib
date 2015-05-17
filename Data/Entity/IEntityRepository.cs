using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willowsoft.WillowLib.Data.Entity
{
    public interface IEntityRepository<TPersistable, TId>
        where TId : IEntityId
        where TPersistable : IPersistableEntity
    {
        TPersistable Get(TId id);
        void Insert(TPersistable entity);
        void Update(TPersistable entity);
        void Delete(TPersistable entity);
    }
}
