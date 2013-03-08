using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MySolarSystemAreaCircle: MySolarSystemArea
    {
        public Vector3 Position = Vector3.Zero;
        public float Radius = 0;

        public override float GetSectorInterpolator(MyMwcVector3Int sectorPosition)
        {
            Vector3 sectorPos = MySolarSystemUtils.SectorsToKm(sectorPosition);
            Vector3 fromSectorToCenter = sectorPos - Position;
            float fromSectorToCenterDist = fromSectorToCenter.Length();
            if (fromSectorToCenterDist > Radius)
            {
                // Out of circle zone
                return 0;
            }

            float cubicInterpolator = MathHelper.SmoothStep(0, 1, fromSectorToCenterDist / Radius);
            return cubicInterpolator;
        }

        public override void AddUniverseEntities(MySolarSystemMapData data)
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetCenter()
        {
            return Position;
        }

        public bool IsSectorInArea(MyMwcVector3Int sectorPosition)
        {
            Vector3 sectorPos = MySolarSystemUtils.SectorsToKm(sectorPosition);
            Vector3 fromSectorToCenter = sectorPos - Position;
            float fromSectorToCenterDist = fromSectorToCenter.Length();
            return fromSectorToCenterDist <= Radius;
        }
    }
}
