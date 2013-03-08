using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MySolarSystemGenerator
    {
        private Random rnd;
        private MySolarSystemMapData m_data;

        public MySolarSystemGenerator(int seed)
        {
            rnd = new Random(seed);
        }

        public MySolarSystemMapData SolarSystemData
        {
            get
            {
                return m_data;
            }
        }

        private void AddEntity(MySolarSystemMapData data, Vector3 posMillKm, float radiusMillKm, string name, MySolarSystemEntityEnum entityType, Color color, object entityData = null)
        {
            float radius = radiusMillKm * MyBgrCubeConsts.MILLION_KM;
            Vector3 offset;
            MyMwcVector3Int sector = MySolarSystemUtils.MillionKmToSectors(posMillKm, out offset);
            var entity = new MySolarSystemMapEntity(sector, offset, radius, name, entityType, color);
            entity.EntityData = entityData;
            data.Entities.Add(entity);
        }

        public void Generate(int sectorCacheCapacity)
        {
            m_data = new MySolarSystemMapData(sectorCacheCapacity);
            
            // Sun
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.SUN_RADIUS, MyTextsWrapper.Get(MyTextsWrapperEnum.Sun).ToString(), MySolarSystemEntityEnum.Sun, Color.White);

            // Orbits (just orbit lines)
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.MERCURY_POSITION.Length(), "Mercury orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.VENUS_POSITION.Length(), "Venus orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.EARTH_POSITION.Length(), "Earth orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.MARS_POSITION.Length(), "Mars orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.JUPITER_POSITION.Length(), "Jupiter orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.SATURN_POSITION.Length(), "Saturn orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.URANUS_POSITION.Length(), "Uranus orbit", MySolarSystemEntityEnum.Orbit, Color.White);
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.NEPTUNE_POSITION.Length(), "Neptune orbit", MySolarSystemEntityEnum.Orbit, Color.White);

            foreach (var a in MySolarSystemConstants.Areas)
            {
                a.Value.AddUniverseEntities(m_data);
                m_data.Areas.Add(a.Key);
            }

            if (MyFakes.ENABLE_RANDOM_STATIONS_IN_SOLAR_SYSTEM && 
                MyMissions.ActiveMission == null && 
                MyMissions.GetAvailableMissions().Count == 0)
            {
                foreach (var a in MyFactions.FactionAreas)
                {
                    if (a.Key == CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_FactionEnum.CSR)
                        continue;
                    foreach (var circle in a.Value)
                    {
                        Random r = new Random(circle.Position.GetHashCode() ^ circle.Radius.GetHashCode());

                        var radius = circle.Radius / 100000000;

                        float count = MathHelper.Pi * radius * radius;
                        count = MathHelper.Clamp(count, 0, 10);
                        if (count < 1)
                        {
                            count = r.Next(0, 2);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            float dist = rnd.Float() * circle.Radius;
                            float angle = (float)(rnd.NextDouble() * 2 * MathHelper.Pi);
                            float x = (float)Math.Sin(angle) * dist;
                            float z = (float)Math.Cos(angle) * dist;

                            var pos = circle.GetCenter() + new Vector3(x, 0, z);
                            //pos = circle.GetCenter();

                            Vector3 offset;
                            MyMwcVector3Int sector = MySolarSystemUtils.KmToSectors(pos, out offset);
                            var mark = new MySolarSystemMapNavigationMark(sector, "", null, Color.White, MyTransparentMaterialEnum.SolarMapOutpost);
                            mark.Importance = rnd.Next(2, 5) / 4.0f;
                            mark.DrawVerticalLine = false;
                            m_data.NavigationMarks.Add(mark);
                            m_data.ImportantObjects.Add(new MyImportantSolarObject() { NavigationMark = mark, TemplateGroup = MyTemplateGroupEnum.RandomStations });
                        }
                    }
                }
            }
            
            // Filip resize solar map
            const float factionMapScale = 1.75f;

            // Faction map - size of map is same as saturn orbit
            AddEntity(m_data, Vector3.Zero, MyBgrCubeConsts.SATURN_POSITION.Length() / factionMapScale, "Faction map", MySolarSystemEntityEnum.FactionMap, Color.White);

            AddEntity(m_data, new Vector3(-352, 0, -40), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionRussian).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.IndianRed, MyTransparentMaterialEnum.FactionRussia);
            AddEntity(m_data, new Vector3(-432, 0, -628), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionRussian).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.IndianRed, MyTransparentMaterialEnum.FactionRussia);
            AddEntity(m_data, new Vector3(0, 0, 300), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionChineseShort).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.Brown, MyTransparentMaterialEnum.FactionChina);
            AddEntity(m_data, new Vector3(-150, 0, 95), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionJapan).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.DeepSkyBlue, MyTransparentMaterialEnum.FactionJapan);
            AddEntity(m_data, new Vector3(300, 0, 108), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionFreeAsia).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.HotPink, MyTransparentMaterialEnum.FactionFreeAsia);
            AddEntity(m_data, new Vector3(300, 0, -40), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionSaudiShort).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.Gold, MyTransparentMaterialEnum.FactionSaudi);
            AddEntity(m_data, new Vector3(220, 0, -180), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionEuroamericanShort).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.CornflowerBlue, MyTransparentMaterialEnum.FactionEAC);
            //AddEntity(m_data, new Vector3(65, 0, -135), 0, "CSR", MySolarSystemEntityEnum.FactionInfo, Color.DarkKhaki, MyTransparentMaterialEnum.FactionCSR);
            AddEntity(m_data, new Vector3(37, 0, -160), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionIndia).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.SandyBrown, MyTransparentMaterialEnum.FactionIndia);
            AddEntity(m_data, new Vector3(-30, 0, -160), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionChurchShort).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.Yellow, MyTransparentMaterialEnum.FactionChurch);
            AddEntity(m_data, new Vector3(-85, 0, -150), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionOmnicorpShort).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.LightGreen, MyTransparentMaterialEnum.FactionOmnicorp);
            AddEntity(m_data, new Vector3(-108, 0, -358), 0, MyTextsWrapper.Get(MyTextsWrapperEnum.FactionFourthReichShort).ToString(), MySolarSystemEntityEnum.FactionInfo, Color.LightSteelBlue, MyTransparentMaterialEnum.FactionFourthReich);

            // Some navigation marks for testing            
            //AddNavigationMark(m_data, new MyMwcVector3Int(1000000, 0, 1000000), "Mark1");
            //AddNavigationMark(m_data, new MyMwcVector3Int(3000000, 0, -1500000), "Mark2");
        }
    }
}
