using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.Missions;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MyOrbitProperties
    {
        public Vector3 OrbitCenter; // Km
        public Vector3 AreaCenter; // Km
        public float MaxDistanceFromOrbitLow; // Km
        public float MaxDistanceFromOrbitHigh; // Km
        public float LongSpread; // between 0.0f and 1.0f
    }

    class MySolarSystemAreaOrbit: MySolarSystemArea
    {
        public float HalfAngle
        {
            get
            {
                return MathHelper.Clamp(MathHelper.TwoPi / 2 * OrbitProperties.LongSpread, 0, MathHelper.TwoPi / 2); 
            }
        }

        public MyOrbitProperties OrbitProperties;

        public override float GetSectorInterpolator(MyMwcVector3Int sectorPosition)
        {
            //Sem ja
            Vector3 sectorPos = MySolarSystemUtils.SectorsToKm(sectorPosition);
            //vzdalenost od stredu vesmiru ke mne  //stred vesmiru
            Vector3 centerToSector = sectorPos - OrbitProperties.OrbitCenter;
            float sectorToCenterDist = centerToSector.Length();

                                 //Stred pulmesice            //stred vesmiru
            Vector3 centerToZone = OrbitProperties.AreaCenter - OrbitProperties.OrbitCenter;
            float orbitRadius = centerToZone.Length();


            //vzdalenost mezi orbitama
            float distFromOrbit = Math.Abs(orbitRadius - sectorToCenterDist);
            if (distFromOrbit > OrbitProperties.MaxDistanceFromOrbitHigh)
            {
                // Not in area (not on ring)
                return 0;
            }

            //
            //float baseAngle = MyMath.AngleTo(centerToZone, Vector3.UnitZ).Y;
            //float angle = MyMath.AngleTo(centerToZone, centerToSector).Y;
            float angle = MyMath.AngleBetween(centerToSector, centerToZone);
            if (Math.Abs(angle) > HalfAngle)
            {
                // Not in area (other part of orbit)
                return 0;
            }

            float angleInterpolator = angle / HalfAngle;
            float maxDistFromOrbitAtAngle = MathHelper.SmoothStep(OrbitProperties.MaxDistanceFromOrbitHigh, OrbitProperties.MaxDistanceFromOrbitLow, angleInterpolator);
            if (distFromOrbit > maxDistFromOrbitAtAngle)
            {
                // Not in area (not in shape of zone)
                return 0;
            }

            float interpolator = distFromOrbit / maxDistFromOrbitAtAngle;
            interpolator = 1 - (float)Math.Pow(interpolator, 3);
            return interpolator;
        }

        private float CalculateStep(float maxDistanceFromOrbit)
        {
            float orbitRadius = OrbitProperties.AreaCenter.Length();
            float len = HalfAngle / 2 * orbitRadius;
            float stepCount = len / maxDistanceFromOrbit;
            const float overlap = 1;
            return HalfAngle / stepCount / overlap;
        }

        // Gets random position with normal distribution - most object in center, few objects on borders
        private Vector3 GetRandomPosition(Random rnd)
        {
            float baseAngle = MyMath.AngleTo(OrbitProperties.AreaCenter, Vector3.UnitZ).Y;
            float randNumber = MathHelper.Clamp(rnd.FloatNormal(0, 0.4f), -1, 1);
            float angleOffset = (randNumber) * HalfAngle;
            float angle = baseAngle + angleOffset;
            float interpolator = 1 - Math.Abs(angleOffset / HalfAngle);
            float maxDistFromOrbitAtAngle = MathHelper.SmoothStep(OrbitProperties.MaxDistanceFromOrbitLow, OrbitProperties.MaxDistanceFromOrbitHigh, interpolator);
            Vector2 distFromOrbit = Vector2.Normalize(new Vector2(rnd.FloatNormal(0, 0.2f), rnd.FloatNormal(0, 0.2f)));
            distFromOrbit *= maxDistFromOrbitAtAngle;
            float orbitRadius = OrbitProperties.AreaCenter.Length();
            float dist = orbitRadius + distFromOrbit.X;

            float x = (float)Math.Sin(angle) * dist;
            float z = (float)Math.Cos(angle) * dist;
            float y = 0;
            return OrbitProperties.OrbitCenter + new Vector3(x, y, z);
        }

        public void AddTemplateGroups(MySolarSystemMapData data)
        {
            return; // Template groups in solar map are disabled
            if (SolarMapData.TemplateGroups == null) return;

            Random rnd = new Random(0);
            foreach (var g in SolarMapData.TemplateGroups)
            {
                for (int i = 0; i < g.Count; i++)
                {
                    var pos = GetRandomPosition(rnd);
                    Vector3 offset;
                    MyMwcVector3Int sector = MySolarSystemUtils.KmToSectors(pos, out offset);

                    var mark = new MySolarSystemMapNavigationMark(sector, "", null, Color.White, TransparentGeometry.MyTransparentMaterialEnum.SolarMapOutpost);
                    mark.Importance = g.Importance;
                    mark.DrawVerticalLine = false;
                    data.NavigationMarks.Add(mark);
                    data.ImportantObjects.Add(new MyImportantSolarObject() { NavigationMark = mark, TemplateGroup = g.TemplateGroup });
                    //entities.Add(new MySolarSystemMapEntity(sector, offset, 0, "", MySolarSystemEntityEnum.OutpostIcon));
                }
            }
        }

        public override void AddUniverseEntities(MySolarSystemMapData data)
        {
            if (MyMissions.ActiveMission == null && MyMissions.GetAvailableMissions().Count == 0)
            {
                AddTemplateGroups(data);
            }

            const float maxObjCenterFromOrbit = 0.25f;

            Random rnd = new Random(0);
            float orbitRadius = OrbitProperties.AreaCenter.Length();
            float baseAngle = MyMath.AngleTo(OrbitProperties.AreaCenter, Vector3.UnitZ).Y;

            //count of groups per orbit side 
            float step = CalculateStep((OrbitProperties.MaxDistanceFromOrbitHigh + OrbitProperties.MaxDistanceFromOrbitLow) / 2);

            int testMaxCount = 0;

            for (float i = baseAngle - HalfAngle; i < baseAngle + HalfAngle; i += step)
            {
                float interpolator = 1 - Math.Abs((i - baseAngle) / HalfAngle); // (i + halfLen) / halfLen / 2;
                float maxDistFromOrbitAtAngle = MathHelper.SmoothStep(OrbitProperties.MaxDistanceFromOrbitLow, OrbitProperties.MaxDistanceFromOrbitHigh, interpolator);

                step = CalculateStep((OrbitProperties.MaxDistanceFromOrbitHigh + maxDistFromOrbitAtAngle) / 2);

                //Vector3 centerPos = new Vector3((float)Math.Sin(i) * distance, 0, (float)Math.Cos(i) * distance);

                //5 = billboards count in smaller group
                for (int j = 0; j < 2; j++)
                {
                    Vector2 distFromOrbit = Vector2.Normalize(new Vector2(rnd.Float(-1, 1), rnd.Float(-1, 1)));
                    //distFromOrbit = new Vector2();
                    distFromOrbit *= maxDistFromOrbitAtAngle * maxObjCenterFromOrbit;
                    float dist = orbitRadius + distFromOrbit.X;

                    float i2 = i + rnd.FloatCubic(-step * maxObjCenterFromOrbit, step * maxObjCenterFromOrbit);

                    float x = (float)Math.Sin(i2) * dist;
                    float z = (float)Math.Cos(i2) * dist;
                    float y = distFromOrbit.Y;
                    Vector3 pos = OrbitProperties.OrbitCenter + new Vector3(x, y, z);

                    Vector3 offset;
                    MyMwcVector3Int sector = MySolarSystemUtils.KmToSectors(pos, out offset);

                    float size = maxDistFromOrbitAtAngle * (1 - maxObjCenterFromOrbit);

                    if (this.SolarMapData != null)
                    {
                        Vector4 clr = new Vector4(this.SolarMapData.DustColor, 1.0f);
                        Color color = rnd.Color(new Color(clr - this.SolarMapData.DustColorVariability), new Color(clr + this.SolarMapData.DustColorVariability));

                        data.Entities.Add(new MySolarSystemMapEntity(sector, offset, 2 * size, "Dust", MySolarSystemEntityEnum.DustField, color));
                        testMaxCount++;
                    }
                    if ((AreaType & AreaEnum.PostPlanet) != 0 && j % 2 == 0)
                    {
                        data.Entities.Add(new MySolarSystemMapEntity(sector, offset, size * (1 - maxObjCenterFromOrbit), "Asteroids", MySolarSystemEntityEnum.AsteroidField, Color.White));
                        testMaxCount++;
                    }

                    //AddEntity(m_data, pos, radius * wide * 4, name + " dust", MySolarSystemEntityEnum.Test1_Dust, rnd.Color(baseColor, colorVariation));
                }
            }

            //Do not allow to solar area to add more than 1000 objects
            //System.Diagnostics.Debug.Assert(testMaxCount < 3000);

            MySolarAreaBorderLine newLine = new MySolarAreaBorderLine();
            newLine.AreaCenter = OrbitProperties.AreaCenter;
            newLine.DistanceHigh = OrbitProperties.MaxDistanceFromOrbitHigh;
            newLine.DistanceLow = OrbitProperties.MaxDistanceFromOrbitLow;
            newLine.Spread = OrbitProperties.LongSpread;
            newLine.col = new Vector4(SolarMapData.DustColor, 1f);
            data.AreasBorderLines.Add(newLine);
        }

        private Vector3 DirectionFromPointOnOrbit(Vector3 point)
        {
            Vector3 ret = point;
            ret.Normalize();
            return ret;
        }

        public override Vector3 GetCenter()
        {
            //We agreed that everything in our solar area is in plane
            System.Diagnostics.Debug.Assert(MyMwcUtils.IsZero(OrbitProperties.AreaCenter.Y));
            return OrbitProperties.AreaCenter;
        }
    }
}
