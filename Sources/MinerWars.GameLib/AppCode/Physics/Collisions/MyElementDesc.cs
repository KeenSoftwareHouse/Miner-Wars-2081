using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Physics
{
    /// <summary>
    /// element descriptor
    /// </summary>
    abstract class MyElementDesc
    {
        public virtual bool IsValid()
        {
            return true;
        }

    }
}
