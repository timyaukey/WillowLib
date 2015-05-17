using System;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.Data.Entity
{
    public interface IPersistable
    {
        bool IsDirty { get; set; }
        bool IsDeleted { get; set; }
        bool IsPersisted { get; }
        void Validate(ErrorList errors);
    }
}
