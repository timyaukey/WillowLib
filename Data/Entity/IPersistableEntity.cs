using System;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.Data.Entity
{
    public interface IPersistableEntity : IPersistable
    {
        int GetIdValue();
        void SetIdValue(int value);
    }
}
