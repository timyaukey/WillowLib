using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.Data.Entity
{
    public class Entity<TID> : IPersistableEntity
        where TID: EntityId
    {
        #region Private property fields

        private TID mId;
        private DateTime mCreateDate;
        private DateTime mModifyDate;
        
        #endregion

        private bool mIsDirty;
        private bool mIsDeleted;

        [DebuggerStepThrough]
        public Entity(TID id, DateTime createDate, DateTime modifyDate)
        {
            mId = id;
            mCreateDate = createDate;
            mModifyDate = modifyDate;
            mIsDirty = false;
            mIsDeleted = false;
        }

        #region Encapsulated fields

        public TID Id
        {
            [DebuggerStepThrough]
            get { return mId; }
            [DebuggerStepThrough]
            set { mId = value; }
        }

        public DateTime CreateDate
        {
            [DebuggerStepThrough]
            get { return mCreateDate; }
            [DebuggerStepThrough]
            set { PropertySet(ref mCreateDate, value); }
        }

        public DateTime ModifyDate
        {
            [DebuggerStepThrough]
            get { return mModifyDate; }
            [DebuggerStepThrough]
            set { PropertySet(ref mModifyDate, value); }
        }

        #endregion

        #region IPersistable Members

        public virtual bool IsDirty
        {
            [DebuggerStepThrough]
            get { return mIsDirty; }
            [DebuggerStepThrough]
            set { mIsDirty = value; }
        }

        public bool IsDeleted
        {
            [DebuggerStepThrough]
            get { return mIsDeleted; }
            [DebuggerStepThrough]
            set { mIsDeleted = value; }
        }

        public bool IsPersisted
        {
            [DebuggerStepThrough]
            get { return !mId.IsNull; }
        }

        [DebuggerStepThrough]
        public int GetIdValue()
        {
            return mId.Value;
        }

        [DebuggerStepThrough]
        public void SetIdValue(int value)
        {
            mId.Value = value;
        }

        public virtual void Validate(ErrorList errors)
        {
        }

        #endregion

        // This has to be a separate overload because some controls
        // represent empty string data as a null reference instead
        // of a zero length string. The known example is the 
        // DataGridView.
        protected void PropertySet(ref string field, string value)
        {
            if (string.IsNullOrEmpty(field))
            {
                if (string.IsNullOrEmpty(value))
                    return;
                field = value;
                mIsDirty = true;
                return;
            }
            if (field.Equals(value))
                return;
            field = value;
            mIsDirty = true;
        }

        protected void PropertySet<T>(ref T field, T value)
        {
            if (field.Equals(value))
                return;
            field = value;
            mIsDirty = true;
        }

        [DebuggerStepThrough]
        protected static int FieldLength(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            else
                return value.Length;
        }

        [DebuggerStepThrough]
        protected static void ValidateLength(string value, ErrorList errors,
            int minLength, int maxLength, string propertyName)
        {
            int length = FieldLength(value);
            if (length < minLength || length > maxLength)
            {
                errors.Add(new EntityLengthError(propertyName, minLength, maxLength));
            }
        }

        [DebuggerStepThrough]
        protected static void ValidateIdRequired(EntityId id, ErrorList errors,
            string propertyName)
        {
            if (id == null || id.IsNull)
            {
                errors.Add(new EntityNullIdError(propertyName));
            }
        }

        [DebuggerStepThrough]
        protected static void ValidateIntRange(string textValue, int minValue, int maxValue,
            ErrorList errors, string propertyName)
        {
            int value;
            if (int.TryParse(textValue, out value))
            {
                if (value < minValue || value > maxValue)
                {
                    errors.Add(new EntityIntRangeError(propertyName, minValue, maxValue));
                }
            }
            else
                errors.Add(new EntityIntRangeError(propertyName, minValue, maxValue));
        }

        [DebuggerStepThrough]
        protected static void ValidateDecimalRange(string textValue, decimal minValue, decimal maxValue,
            int maxPlaces, ErrorList errors, string propertyName)
        {
            decimal value;
            if (decimal.TryParse(textValue, out value))
            {
                decimal scaledUpValue = value;
                for (int place = 1; place <= maxPlaces; place++)
                    scaledUpValue = scaledUpValue * 10;
                if (scaledUpValue != decimal.Truncate(scaledUpValue))
                {
                    errors.Add(new EntityDecimalPenniesError(propertyName, maxPlaces));
                }
                else if (value < minValue || value > maxValue)
                {
                    errors.Add(new EntityDecimalRangeError(propertyName, minValue, maxValue));
                }
            }
            else
                errors.Add(new EntityDecimalRangeError(propertyName, minValue, maxValue));
        }
    }

    public class EntityValidationError : SevereError
    {
        public EntityValidationError(string message)
            : base(message)
        {
        }
    }

    public class EntityLengthError : EntityValidationError
    {
        public EntityLengthError(string propertyName, int minLength, int maxLength)
            : base(propertyName +
                    " must be between " + minLength.ToString() +
                    " and " + maxLength.ToString() + " characters")
        {
        }
    }

    public class EntityNullIdError : EntityValidationError
    {
        public EntityNullIdError(string propertyName)
            : base(propertyName + " is required")
        {
        }
    }

    public class EntityIntRangeError : EntityValidationError
    {
        public EntityIntRangeError(string propertyName, int minValue, int maxValue)
            : base(propertyName +
                    " must be and integer between " + minValue.ToString() +
                    " and " + maxValue.ToString())
        {
        }
    }

    public class EntityDecimalRangeError : EntityValidationError
    {
        public EntityDecimalRangeError(string propertyName, decimal minValue, decimal maxValue)
            : base(propertyName +
                    " must be and decimal between " + minValue.ToString() +
                    " and " + maxValue.ToString())
        {
        }
    }

    public class EntityDecimalPenniesError : EntityValidationError
    {
        public EntityDecimalPenniesError(string propertyName, int maxPlaces)
            : base(propertyName +
                    " may have at most " + maxPlaces.ToString() +
                    " decimal places")
        {
        }
    }

    public class EntityDuplicateKeyError : EntityValidationError
    {
        public EntityDuplicateKeyError(string propertyName)
            : base("Duplicate " + propertyName)
        {
        }
    }
}
