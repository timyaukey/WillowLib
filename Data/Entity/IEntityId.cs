using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willowsoft.WillowLib.Data.Entity
{
    public interface IEntityId
    {
        int Value { get; set; }
        bool IsNull { get; }
    }
}
