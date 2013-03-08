using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.Radar
{
    interface IMyObjectToDetect
    {
        BoundingBox WorldAABB { get; }
        Vector3 GetPosition();
    }
}
