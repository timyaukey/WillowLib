using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willowsoft.WillowLib.Data.Entity
{
    public class EntityId : IEntityId
    {
        private int mValue;

        [DebuggerStepThrough]
        public EntityId()
            : this(0)
        {
        }

        [DebuggerStepThrough]
        public EntityId(int value)
        {
            mValue = value;
        }

        #region IEntityId members

        public int Value
        {
            [DebuggerStepThrough]
            get { return mValue; }
            [DebuggerStepThrough]
            set { mValue = value; }
        }

        public bool IsNull
        {
            [DebuggerStepThrough]
            get { return mValue == 0; }
        }

        #endregion

        [DebuggerStepThrough]
        public override string ToString()
        {
            return mValue.ToString();
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;
            // Both instances must be the same exact type - otherwise different
            // EntityId subclasses with the same Value property would be equal.
            if (obj.GetType()!=GetType())
                return false;
            return mValue==((EntityId)obj).Value;
        }

        [DebuggerStepThrough]
        public static bool operator ==(EntityId id1, EntityId id2)
        {
            if (object.ReferenceEquals(id1, null))
            {
                return object.ReferenceEquals(id2, null);
            }
            return id1.Equals(id2);
        }

        [DebuggerStepThrough]
        public static bool operator !=(EntityId id1, EntityId id2)
        {
            if (object.ReferenceEquals(id1, null))
            {
                return !object.ReferenceEquals(id2, null);
            }
            return !id1.Equals(id2);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return mValue;
        }
    }
}
