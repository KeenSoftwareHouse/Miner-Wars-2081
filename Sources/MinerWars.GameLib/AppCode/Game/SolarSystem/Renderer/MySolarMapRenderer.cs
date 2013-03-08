using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Textures;
using SysUtils.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.GUI;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.HUD;
using MinerWars.CommonLIB.AppCode.Generics;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.VideoMode;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Managers;

using SharpDX.Direct3D9;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit;

namespace MinerWars.AppCode.Game.SolarSystem
{
    class MySolarMapRenderer
    {
        MyRender.MyRenderSetup m_setup = new MyRender.MyRenderSetup();
        MyRender.MyRenderSetup m_backup = new MyRender.MyRenderSetup();
        bool m_loaded = false;

        VertexBuffer m_gridVertexBuffer;
        VertexBuffer m_highlightVertexBuffer;
        MyTexture2D m_texture;

        MySolarSystemMapCamera m_currentCamera;
        MySolarSystemMapData m_currentSolarMapData;

        /// <summary>
        /// How many sectors render near camera target
        /// </summary>
        const float SECTOR_RENDER_HALFSIZE = 2;
        const int MAX_SOLAR_TEXTS = 512;
        MyObjectsPoolSimple<MyHudText> m_texts = new MyObjectsPoolSimple<MyHudText>(MAX_SOLAR_TEXTS);

        public MyMwcVector3Int? PlayerSector { get; set; }

        /// <summary>
        /// Sector size in game units
        /// </summary>
        //public const float SECTOR_SIZE_GAMEUNITS = 0.0001f;
        public const int SECTORS_PER_QUAD = 1001;

        // Dont draw object to close to camera
        const float minDistFromCamera = 0.0001f;

        public static float SectorToGameUnits(float sector)
        {
            return sector * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
        }

        public static Vector3 SectorToGameUnits(MyMwcVector3Int sector)
        {
            return new Vector3(sector.X * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS, sector.Y * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS, sector.Z * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS);
        }

        public static Vector3 SectorToMillionKm(MyMwcVector3Int sector)
        {
            return MySolarMapRenderer.GameUnitsToMillionKm(MySolarMapRenderer.SectorToGameUnits(sector));
        }

        public static Vector3 SectorToKm(MyMwcVector3Int sector)
        {
            return SectorToMillionKm(sector) * MyBgrCubeConsts.MILLION_KM;
        }

        public static float GameUnitsToSector(float gameUnits)
        {
            return (float)Math.Round(gameUnits / MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS);
        }

        public static MyMwcVector3Int GameUnitsToSector(Vector3 gameUnits)
        {
            return new MyMwcVector3Int((int)GameUnitsToSector(gameUnits.X), (int)GameUnitsToSector(gameUnits.Y), (int)GameUnitsToSector(gameUnits.Z));
        }

        public static float GameUnitsToKm(float gameUnits)
        {
            return gameUnits * MyBgrCubeConsts.SECTOR_SIZE / MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
        }

        public static float MillionKmToGameUnits(float millionKm)
        {
            return millionKm * MyBgrCubeConsts.MILLION_KM / MyBgrCubeConsts.SECTOR_SIZE * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
        }

        public static float GameUnitsToMillionKm(float gameUnits)
        {
            return gameUnits / (MyBgrCubeConsts.MILLION_KM / MyBgrCubeConsts.SECTOR_SIZE * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS);
        }

        public static float KmToGameUnits(float km)
        {
            return km / MyBgrCubeConsts.SECTOR_SIZE * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
        }

        public static Vector3 KmToGameUnits(Vector3 positionInKm)
        {
            return new Vector3(KmToGameUnits(positionInKm.X), KmToGameUnits(positionInKm.Y), KmToGameUnits(positionInKm.Z));
        }

        public static Vector3 MillionKmToGameUnits(Vector3 millionKm)
        {
            return new Vector3(MillionKmToGameUnits(millionKm.X), MillionKmToGameUnits(millionKm.Y), MillionKmToGameUnits(millionKm.Z));
        }

        public static Vector3 GameUnitsToMillionKm(Vector3 gameUnits)
        {
            return new Vector3(GameUnitsToMillionKm(gameUnits.X), GameUnitsToMillionKm(gameUnits.Y), GameUnitsToMillionKm(gameUnits.Z));
        }

        static Vector3 EntityPosition(MyMwcVector3Int sector, Vector3 positionInSector, MySolarSystemMapCamera camera)
        {
            MyMwcVector3Int sectorOffset;
            sectorOffset.X = sector.X - camera.TargetSector.X;
            sectorOffset.Y = sector.Y - camera.TargetSector.Y;
            sectorOffset.Z = sector.Z - camera.TargetSector.Z;

            return MillionKmToGameUnits(MySolarSystemUtils.SectorsToMillionKm(sectorOffset)) + KmToGameUnits(positionInSector) + camera.CameraToTarget - camera.Target;
        }

        static Vector3 EntityPosition(MySolarSystemMapEntity entity, MySolarSystemMapCamera camera)
        {
            return EntityPosition(entity.Sector, entity.PositionInSector, camera);            
        }

        private static Vector3[] GRID_HEIGHT_COLORS = new Vector3[]
        {
            new Vector3(0.95f, 0.85f, 0.1f),
	        new Vector3(1f, 0.5f, 0f),
	        new Vector3(1f, 0.1f, 0f),
	        new Vector3(0.6f, 0.35f, 1f),
	        new Vector3(0.05f, 0.9f, 0.95f), // 10.000 sectorsf, light blue
        };

        public MySolarMapRenderer()
        {
            SetRenderSetup();
        }

        static MySolarMapRenderer()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.SolarAreaBorders, "Show Solar areas borders", DrawHandler, MyRenderStage.DebugDraw, false);
        }

        public static void DrawHandler() { }

        void SetRenderSetup()
        {
            m_setup.CallerID = MyRenderCallerEnum.SolarMap;

            m_setup.EnabledModules = new HashSet<MyRenderModuleEnum>();
            m_setup.EnabledModules.Add(MyRenderModuleEnum.AnimatedParticlesPrepare);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TransparentGeometry);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DrawCoordSystem);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.BackgroundCube);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TestField);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.AnimatedParticles);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Lights);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TransparentGeometryForward);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Projectiles);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DebrisField);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarObjects);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarAreaBorders);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarMapGrid);
            

            m_setup.EnabledPostprocesses = new HashSet<MyPostProcessEnum>();


            m_setup.EnableHDR = false;
            m_setup.EnableSun = false;
            m_setup.EnableSmallLights = false;
            m_setup.EnableEnvironmentMapping = false;

            m_setup.RenderElementsToDraw = new List<MyRender.MyRenderElement>();
            m_setup.TransparentRenderElementsToDraw = new List<MyRender.MyRenderElement>();
        }

        public void LoadContent()
        {
            MyMwcLog.WriteLine("MyHudSectorBorder.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            m_loaded = false;

            m_texture = MyTextureManager.GetTexture<MyTexture2D>("Textures2\\Models\\Prefabs\\v01\\v01_holo_grid_orange_de");

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHudSectorBorder.LoadContent() - END");
        }

        public void UnloadContent()
        {
            if (m_gridVertexBuffer != null)
            {
                m_gridVertexBuffer.Dispose();
                m_gridVertexBuffer = null;
            }
            if (m_highlightVertexBuffer != null)
            {
                m_highlightVertexBuffer.Dispose();
                m_highlightVertexBuffer = null;
            }
            m_loaded = false;
        }

        /// <summary>
        /// Calculates linear interpolated blend
        /// </summary>
        /// <param name="lower">When value is lower or equal to lower, blend is 0</param>
        /// <param name="upper">When value is higher or equal to upper, blend is 1</param>
        /// <param name="value">Value to interpolate</param>
        /// <returns>Alpha from 0.0 to 1.0</returns>
        private float CalculateBlend(float lower, float upper, float value)
        {
            float val = (value - lower) / (upper - lower);
            if (val > 1)
                val = 1;
            else if (val < 0)
                val = 0;

            return val;
        }

        /// <summary>
        /// Adds the test.
        /// </summary>
        /// <param name="position">Position in game units.</param>
        /// <param name="str"></param>
        /// <param name="alignment"></param>
        /// <param name="scale"></param>
        /// <param name="offset"></param>
        private void AddText(Vector3 position, string str, MyGuiDrawAlignEnum alignment, float scale, Color color, MyGuiFont font, Vector2 offset = new Vector2())
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                return;
            }
                 
            Vector3 objToCamera = position - MyCamera.Position;
            if (Vector3.Dot(MyCamera.ForwardVector, objToCamera) < 0)
            {
                return;
            }

            Vector3 target = SharpDXHelper.ToXNA(MyCamera.Viewport.Project(SharpDXHelper.ToSharpDX(position), SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity)));
            Vector2 projectedPoint2D = new Vector2(target.X, target.Y) + offset / 1080 * MyGuiManager.GetFullscreenRectangle().Height;

            MyHudText text = m_texts.Allocate();
            if (text != null)
            {
                text.Start(font, projectedPoint2D, color, scale * 1.0f, alignment);
                text.Append(str);
            }      
        }        

        private void AddPointBillboardUnscaling(MyTransparentMaterialEnum material, Vector4 color, Vector3 origin, float radiusWhen1mFromCamera, float angle, int priority = 0, bool colorize = true)
        {
            float radius = MySolarSystemUtils.CalculateDistanceUnscaling(origin, radiusWhen1mFromCamera);
            MyTransparentGeometry.AddPointBillboard(material, color, origin, radius, angle, priority, colorize);
        }

        private void AddPointBillboardOrientedUnscaling(MyTransparentMaterialEnum material, Vector4 color, Vector3 origin, Vector3 left, Vector3 up, float radiusWhen1mFromCamera, float angle, int priority = 0)
        {
            float radius = MySolarSystemUtils.CalculateDistanceUnscaling(origin, radiusWhen1mFromCamera);
            MyTransparentGeometry.AddBillboardOriented(material, color, origin, left, up, radius, priority);
        }

        /// <summary>
        /// Adds point billboard, when billboard is far from camera, it stops shrinking.
        /// </summary>
        private void AddPointBillboardUnscalingFromDistance(MyTransparentMaterialEnum material, Vector4 color, Vector3 origin, float radius, float angle, float fixedSizeDistance, int priority = 0, bool colorize = true)
        {
            colorize = false;
            radius = MySolarSystemUtils.CalculateDistanceUnscalingFrom(origin, radius, fixedSizeDistance);
            MyTransparentGeometry.AddPointBillboard(material, color, origin, radius, angle, priority, colorize);
        }

        private void AddLineBillboardUnscaling(MyTransparentMaterialEnum material,
            Vector4 color, Vector3 origin, Vector3 directionNormalized, float length, float thicknessWhen1mFromCamera, float minDistFromCamera, int priority = 0)
        {
            Vector3 linePtB = origin + directionNormalized * length;
            Vector3 cameraPos = Vector3.Zero;

            float dist = MyUtils.GetPointLineDistance(ref origin, ref linePtB, ref cameraPos);
            if (dist < minDistFromCamera)
            {
                return;
            }

            //float thickness = (m_setup.CameraPosition.Value - origin).Length() * thicknessWhen1mFromCamera;
            float thickness = dist * thicknessWhen1mFromCamera;
            thickness = Math.Max(0.0000005f, thickness);
            MyTransparentGeometry.AddLineBillboard(material, color, origin, directionNormalized, length, thickness, priority);
        }

        Vector3 GetPositionRelativeToCamera(double x, double y, double z)
        {
            x -= m_currentCamera.TargetSector.X * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            y -= m_currentCamera.TargetSector.Y * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            z -= m_currentCamera.TargetSector.Z * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

            x -= m_currentCamera.Target.X;
            y -= m_currentCamera.Target.Y;
            z -= m_currentCamera.Target.Z;

            Vector3 result = new Vector3((float)x, (float)y, (float)z);
            result += m_currentCamera.CameraToTarget;
            return result;
        }

        void AddOrbit(float distFromSunInKm)
        {
            AddOrbit(distFromSunInKm, Color.Turquoise.ToVector4());
        }

        void AddOrbit(float distFromSunInKm, Vector4 color)
        {
            float distFromSunInSolarMapUnits = KmToGameUnits(distFromSunInKm);
            int stepCount = 157; // FastSin depends on this value
            float step = MathHelper.TwoPi / stepCount;

            double x = MyMath.FastSin(0) * distFromSunInSolarMapUnits;
            double z = MyMath.FastCos(0) * distFromSunInSolarMapUnits;
            Vector3 prevPos = GetPositionRelativeToCamera(x, 0, z);

            for (int i = 1; i <= stepCount; i++)
            {
                float angle = MathHelper.TwoPi / stepCount * i;

                x = MyMath.FastSin(angle) * distFromSunInSolarMapUnits;
                z = MyMath.FastCos(angle) * distFromSunInSolarMapUnits;
                Vector3 pos = GetPositionRelativeToCamera(x, 0, z);

                if (MyMwcUtils.HasValidLength(pos - prevPos))
                    DrawLine(prevPos, pos, 0.002f, color);
                //float dist = (pos - prevPos).Length();
                //AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapOrbitLine, color, prevPos, MyMwcUtils.Normalize(pos - prevPos), dist, 0.002f, minDistFromCamera, 1);
                prevPos = pos;
            }
        }

        void AddSun(MySolarSystemMapEntity entity)
        {
            // Size of billboard is 2.85x bigger than sun in it's center
            const float billboardSizeRatio = 1.85f;

            Vector3 pos = EntityPosition(entity, m_currentCamera);

            // Sun billboard size won't decrease when further than 150 million km
            float sunDistanceMinSize = MillionKmToGameUnits(150);
            AddPointBillboardUnscalingFromDistance(MyTransparentMaterialEnum.SolarMapSun, Vector4.One, pos, KmToGameUnits(entity.Radius) * billboardSizeRatio, 0, sunDistanceMinSize, 0, false);
            AddText(pos, entity.Name, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP, 0.6f, Color.Yellow, MyGuiManager.GetFontMinerWarsWhite());
        }

        void AddVerticalMarker(Vector3 position, Vector4 color)
        {
            //const float verticalMarkerLength = 10000;
            //const float minDist = 0;

            //AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapZeroPlaneLine, color, position, Vector3.Up, verticalMarkerLength, 0.005f, minDist, 3);
            //AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapZeroPlaneLine, color * Color.Turquoise.ToVector4() * 0.4f, position, -Vector3.Up, verticalMarkerLength, 0.005f, minDist, -5);
        }

        //void AddPlayerMarker()
        //{
        //    if (PlayerSector.HasValue)
        //    {
        //        Vector3 pos = EntityPosition(PlayerSector.Value, Vector3.Zero, m_currentCamera);

        //        AddVerticalMarker(pos);
        //        AddIcon(pos, 30, MyTransparentMaterialEnum.SolarMapSmallShip, Color.White, MySession.PlayerShip.Name, 1000);
        //    }
        //}

        private void AddLargeShip(MySolarSystemMapEntity entity)
        {
            Vector3 pos = EntityPosition(entity, m_currentCamera);
            const float radius = 30;
            AddIcon(pos, radius, MyTransparentMaterialEnum.SolarMapLargeShip, Color.White, entity.Name, null, 1000, false, Vector3.Zero);
        }

        private void AddOutpost(MySolarSystemMapEntity entity)
        {
            Vector3 pos = EntityPosition(entity, m_currentCamera);
            const float radius = 30;
            AddIcon(pos, radius, MyTransparentMaterialEnum.SolarMapOutpost, Color.White, entity.Name, null, 1000, false, Vector3.Zero);
        }

        void AddAsteroidField(MySolarSystemMapEntity entity)
        {
            Vector3 areaPos = EntityPosition(entity, m_currentCamera);
            float areaRadius = KmToGameUnits(entity.Radius);

            float blendSizeLower = SectorToGameUnits(10000);
            float blendSizeUpper = SectorToGameUnits(150000);
            float minAsteroidFieldDist = SectorToGameUnits(20000);
            float maxAsteroidFieldDist = SectorToGameUnits(200000);

            // Size of billboards in fraction of whole size
            const float billboardSizeRatio = 0.2f;
            const int billboardsPerAsteroidField = 25;

            float radius = billboardSizeRatio * areaRadius;

            Random rnd = new Random(entity.Sector.X ^ entity.Sector.Y ^ entity.Sector.Z ^ (int)entity.PositionInSector.X ^ (int)entity.PositionInSector.Y ^ (int)entity.PositionInSector.Z);

            for (int i = 0; i < billboardsPerAsteroidField; i++)
            {
                Vector3 dir = rnd.Direction();
                float maxDist = areaRadius - radius;
                Vector3 pos = areaPos + dir * maxDist;
                float dist = pos.Length();

                float alphaLower = CalculateBlend(minAsteroidFieldDist, minAsteroidFieldDist + blendSizeLower, dist);
                float alphaUpper = 1 - CalculateBlend(maxAsteroidFieldDist, maxAsteroidFieldDist + blendSizeUpper, dist);
                float alpha = alphaLower * alphaUpper;
                if (alpha < float.Epsilon)
                {
                    return;
                }
                Vector4 color = entity.Color.ToVector4();
                color *= alpha;
                //color.W = alpha;
                
                /*if (dist < minAsteroidFieldDist || dist > maxAsteroidFieldDist)
                {
                    return;
                }*/

                MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.SolarMapAsteroidField, color, pos, m_currentCamera.Up, m_currentCamera.Left, radius, -2);
            }
        }

        void AddAreaTexts(List<MySolarSystemAreaEnum> areas)
        {
            if (!MyRender.IsModuleEnabled(MyRenderStage.DebugDraw, MyRenderModuleEnum.SolarAreaBorders)) return;
            foreach (var areaEnum in areas)
            {
                var area = MySolarSystemConstants.Areas[areaEnum];
                Vector3 position = EntityPosition(new MyMwcVector3Int(), area.GetCenter(), m_currentCamera);
                AddText(position, area.Name, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, 0.6f, Color.White, MyGuiManager.GetFontMinerWarsWhite());
            }
        }

        void AddDustField(MySolarSystemMapEntity entity)
        {
            Vector3 pos = EntityPosition(entity, m_currentCamera);
            float radius = KmToGameUnits(entity.Radius);
            
            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.SolarMapDust, entity.Color.ToVector4() * 0.3f, pos, m_currentCamera.Up, m_currentCamera.Left, radius, -2, true);
        }

        void AddFactionMap(MySolarSystemMapEntity entity)
        {
            Vector3 pos = EntityPosition(entity, m_currentCamera);
            float radius = KmToGameUnits(entity.Radius);

            float height = Math.Abs(pos.Y);
            float lower = 2 * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            float upper = 1000 * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            float blend = CalculateBlend(lower, upper, height);

            //pos.Y = 0; // MillionKmToGameUnits(0.1f);
            Vector4 c = entity.Color.ToVector4();
            c.W = blend;
            c *= c.W;
            c.W *= 0.5f;
            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.SolarMapFactionMap, c, pos, Vector3.Forward, Vector3.Left, radius, -3);
        }

        void AddIcon(Vector3 pos, float radius, MyTransparentMaterialEnum texture, Vector4 color, string textDown, string textUp, float textSize, bool highlight, Vector3 offset, MyGuiFont font = null, float? importance = null)
        {
            AddIcon(pos, radius, texture, new Color(color), textDown, textUp, textSize, highlight, offset, font, importance);
        }

        void AddIcon(Vector3 pos, float radius, MyTransparentMaterialEnum texture, Color color, string textDown, string textUp, float textSize, bool highlight, Vector3 offset, MyGuiFont font = null, float? importance = null)
        {
            if (font == null)
                font = MyGuiManager.GetFontMinerWarsWhite();

            float scaledRadius = MySolarSystemUtils.CalculateDistanceUnscalingTo(pos, radius, MySolarSystemMapNavigationMark.SCALE_DISTANCE_FROM);

            float textScale = 1 / MySolarSystemUtils.CalculateDistanceUnscalingFrom(pos, 1 / 0.7f, MySolarSystemMapNavigationMark.SCALE_DISTANCE_FROM);// 1;// radius / scaledRadius / 2;

            Vector4 iconColor = Vector4.One;
            if (highlight)
            {
                iconColor *= 1.5f;
            }

            textSize = 1.35f;
            var position = pos + offset * scaledRadius + Vector3.Up / 2 * scaledRadius;
            MyTransparentGeometry.AddBillboardOriented(texture, iconColor, position, m_currentCamera.Up, m_currentCamera.Left, scaledRadius, 0200 + (int)importance);

            if (textUp != null)
            {
                AddText(position, textUp, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP, textSize * textScale, color, font, new Vector2(0, -90) * textScale);
            }

            if (textDown != null)
            {
                AddText(position, textDown, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP, textSize * textScale, color, font, new Vector2(0, -60) * textScale);
            }
        }

        void AddFactionInfo(MySolarSystemMapEntity entity)
        {
            Vector3 pos = EntityPosition(entity, m_currentCamera);
            const float radius = 20; // hard coded radius

            MyTransparentMaterialEnum texture = (MyTransparentMaterialEnum)entity.EntityData;

            AddIcon(pos, radius, texture, entity.Color, entity.Name, null, 0.7f, false, new Vector3(0,1,0));
        }

        void AddAsteroid(MySolarSystemMapEntity entity)
        {
            Vector3 pos = EntityPosition(entity, m_currentCamera);
            float radius = KmToGameUnits(entity.Radius);
            
            float dist = GameUnitsToKm((pos).Length());
            
            const float dist1km = 3000;
            float distForSize = entity.Radius * dist1km;

            const float dist1kmBlendStart = 2000;
            float distForSizeBlendStart = entity.Radius * dist1kmBlendStart;
            
            if (dist > distForSize || dist < minDistFromCamera)
            {
                //return;
            }

            float alpha = MathHelper.Clamp(1 - ((dist - distForSizeBlendStart) / (distForSize - distForSizeBlendStart)), 0, 1);

            float distToTarget = (pos - m_currentCamera.CameraToTarget).Length();
            alpha *= CalculateBlend(SectorToGameUnits(SECTOR_RENDER_HALFSIZE), SectorToGameUnits(1), distToTarget);

            if (alpha <= float.Epsilon)
            {
                return;
            }

            float asteroidSize = radius / MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS / 2 + 0.01f;
            AddIcon(pos, asteroidSize / 10000, MyTransparentMaterialEnum.SolarMapAsteroid, new Vector4(Vector3.One * alpha, alpha), null, null, 0.0001f, false, Vector3.Zero);

            double absY = pos.Y + m_currentCamera.Position.Y + m_currentCamera.PositionSector.Y * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

            Vector4 color = Vector4.One;
            if (absY < 0)
            {
                color = Color.Brown.ToVector4();
            }

            // Make alpha to power of 2 to make lines fade better
            alpha *= alpha;
            color *= alpha;
            color.W = alpha;

            if (absY != 0)
            {
                AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapZeroPlaneLine, color, pos, Vector3.Up, Math.Abs((float)-absY), 0.002f, 0);
            }
        }

        void AddDirectionNavigator(MyHudTexturesEnum texture, Vector3 position, string text, Vector4 color)
        {         
            Vector3 cameraToObject = position - MyCamera.Position;
            Vector2 projectedPoint2D;
            Vector3 target = SharpDXHelper.ToXNA(MyCamera.Viewport.Project(SharpDXHelper.ToSharpDX(position), SharpDXHelper.ToSharpDX(MyCamera.ProjectionMatrix), SharpDXHelper.ToSharpDX(MyCamera.ViewMatrix), SharpDXHelper.ToSharpDX(Matrix.Identity)));

            // if the target is behind the camera, flip coordinates along center of the screen
            if (Vector3.Dot(MyCamera.ForwardVector, cameraToObject) <= 0)
            {
                projectedPoint2D.X = MyMinerGame.ScreenSize.X / 2 - (target.X - MyMinerGame.ScreenSize.X / 2);
                projectedPoint2D.Y = MyMinerGame.ScreenSize.Y / 2 - (target.Y - MyMinerGame.ScreenSize.Y / 2);
            }
            else
            {
                projectedPoint2D.X = target.X;
                projectedPoint2D.Y = target.Y;
            }

            // if the position is visible, don't draw the direction navigator
            if (projectedPoint2D.X >= 0f &&
                projectedPoint2D.X <= MyMinerGame.ScreenSize.X &&
                projectedPoint2D.Y >= 0f &&
                projectedPoint2D.Y <= MyMinerGame.ScreenSize.Y)
            {
                return;
            }

            var textureCoords = MyHud.GetTextureCoord(texture);
            Vector2 textureSize = new Vector2(MyHud.Texture.Width, MyHud.Texture.Height);

            var sourceRectangle = new Rectangle((int)(textureCoords.Offset.X * textureSize.X), (int)(textureCoords.Offset.Y * textureSize.Y), (int)(textureCoords.Size.X * textureSize.X), (int)(textureCoords.Size.Y * textureSize.Y));


            // calculate direction from nonfixed coords
            MyTexture2D directionTexture = MyHud.Texture;
            Vector2 origin = new Vector2(sourceRectangle.Height / 2f, sourceRectangle.Width / 2f);
            Vector2 direction = Vector2.Normalize(projectedPoint2D - new Vector2(MyMinerGame.ScreenSizeHalf.X, MyMinerGame.ScreenSizeHalf.Y));

            // clip direction indicator position to screen coord            
            if (projectedPoint2D.X < 0f + origin.X) projectedPoint2D.X = 0f + origin.X;
            if (projectedPoint2D.X > MyMinerGame.ScreenSize.X - origin.X) projectedPoint2D.X = MyMinerGame.ScreenSize.X - origin.X;
            if (projectedPoint2D.Y < 0f + origin.Y) projectedPoint2D.Y = 0f + origin.Y;
            if (projectedPoint2D.Y > MyMinerGame.ScreenSize.Y - origin.Y) projectedPoint2D.Y = MyMinerGame.ScreenSize.Y - origin.Y;

            // calculate rotation of direciton texture
            double rotation = Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;

            MyGuiManager.DrawSpriteBatch(directionTexture, projectedPoint2D, sourceRectangle, new Color(color), (float)rotation, origin, 1f, SharpDX.Toolkit.Graphics.SpriteEffects.None, 0f);

            // draw text
            Vector2 normalizedProjectedPoint2D = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(projectedPoint2D);
            Vector2 textOffset = new Vector2(0, 0.05f);
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), new StringBuilder(text),
                                    normalizedProjectedPoint2D + textOffset, 0.7f, new Color(color),
                                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);   
        }

        void AddNavigationMark(MySolarSystemMapNavigationMark navigationMark)
        {
            if (navigationMark.Visible)
            {
                Vector4 color = navigationMark.IsMouseOver() || navigationMark.Highlight ?
                                    navigationMark.Color.ToVector4() * 1.5f:
                                    navigationMark.Color.ToVector4();

                if (navigationMark.DrawVerticalLine)
                {
                    AddVerticalMarker(navigationMark.WorldPosition, navigationMark.VerticalLineColor);
                }


                bool showIcon = true;
                if (navigationMark.IsBlinking)
                {
                    if (MyMinerGame.TotalTimeInMilliseconds % 800 < 80 || (MyMinerGame.TotalTimeInMilliseconds % 800 > 160 && MyMinerGame.TotalTimeInMilliseconds % 800 < 160 + 80))
                    {
                        showIcon = false;
                    }
                }

                if (showIcon)
                {
                    AddIcon(navigationMark.WorldPosition, MySolarSystemMapNavigationMark.RADIUS, navigationMark.Texture, color, navigationMark.Name, navigationMark.Text, navigationMark.TextSize, navigationMark.Highlight || navigationMark.IsMouseOver(), navigationMark.Offset, navigationMark.Font, navigationMark.Importance);
                }
            
                if (navigationMark.DrawVerticalLine)
                {
                    AddDirectionNavigator(navigationMark.DirectionalTexture, navigationMark.WorldPosition, navigationMark.Name, color);
                }
            }
        }

        void AddNavigationMarks(MySolarSystemMapNavigationMarks navigationMarks)
        {
            foreach (var sectorNavigationMarksKvp in navigationMarks)
            {
                foreach (var navigationMark in sectorNavigationMarksKvp.Value)
                {
                    AddNavigationMark(navigationMark);
                }
            }            
        }

        void AddAreasBorders(List<MySolarAreaBorderLine> lines)
        {
            if (!MyRender.IsModuleEnabled(MyRenderStage.DebugDraw, MyRenderModuleEnum.SolarAreaBorders)) return;
            foreach (MySolarAreaBorderLine line in lines)
            {
                //calculate area border lines
                float stepSize = MathHelper.TwoPi / 157f;

                Vector3 lastPosClockHigh, lastPosCounterHigh, lastPosClockLow, lastPosCounterLow, lastPosCounterOrbit, lastPosClockOrbit;
                Vector3 PosClockHigh, PosClockLow, PosClockOrbit, PosCounterLow, PosCounterOrbit, PosCounterHigh;
                if (line.Spread < 1)
                {
                    int stepsCount = (int)(157 * line.Spread / 2f);
                    float r = KmToGameUnits(line.AreaCenter.Length());

                    float initAngle = (float)Math.Atan2(KmToGameUnits(line.AreaCenter).Z, -KmToGameUnits(line.AreaCenter).X) - MathHelper.PiOver2;

                    double x = MyMath.FastSin(initAngle) * r;
                    double z = MyMath.FastCos(initAngle) * r;
                    lastPosClockOrbit = lastPosCounterOrbit = GetPositionRelativeToCamera(x, 0, z);
                    lastPosClockHigh = new Vector3((float)x, 0f, (float)z) * ((KmToGameUnits(line.DistanceHigh) / r) + 1); lastPosClockHigh = GetPositionRelativeToCamera(lastPosClockHigh.X, lastPosClockHigh.Y, lastPosClockHigh.Z);
                    lastPosClockLow = new Vector3((float)x, 0f, (float)z) * (-(KmToGameUnits(line.DistanceLow) / r) + 1); lastPosClockLow = GetPositionRelativeToCamera(lastPosClockLow.X, lastPosClockLow.Y, lastPosClockLow.Z);
                    lastPosCounterHigh = lastPosClockHigh;
                    lastPosCounterLow = lastPosClockLow;

                    float cos = MyMath.FastCos(stepSize);
                    float sin = MyMath.FastSin(stepSize);
                    float cosN = MyMath.FastCos(-stepSize);
                    float sinN = MyMath.FastSin(-stepSize);


                    for (int stepI = 1; stepI <= stepsCount; stepI++)
                    {
                        float angle = initAngle + stepI * -stepSize;
                        x = MyMath.FastSin(angle) * r;
                        z = MyMath.FastCos(angle) * r;
                        PosClockOrbit = GetPositionRelativeToCamera(x, 0, z);
                        //clockwise
                        //PosClockOrbit = new Vector3(lastPosClockOrbit.X * cos - lastPosClockOrbit.Y * sin, 0, lastPosClockOrbit.X * sin + lastPosClockOrbit.Y * cos);
                        PosClockHigh = new Vector3((float)x, 0f, (float)z) * (((KmToGameUnits(line.DistanceHigh) / r) * (stepsCount - stepI) / stepsCount) + 1); PosClockHigh = GetPositionRelativeToCamera(PosClockHigh.X, PosClockHigh.Y, PosClockHigh.Z);
                        PosClockLow = new Vector3((float)x, 0f, (float)z) * (-((KmToGameUnits(line.DistanceLow) / r) * (stepsCount - stepI) / stepsCount) + 1); PosClockLow = GetPositionRelativeToCamera(PosClockLow.X, PosClockLow.Y, PosClockLow.Z);
                        //DrawLine(lastPosClockOrbit, PosClockOrbit, 0.004f, line.col);
                        //draw line from lastPos to Pos (low & high)
                        if(MyMwcUtils.HasValidLength(KmToGameUnits(PosClockHigh) - KmToGameUnits(lastPosClockHigh)))
                            DrawLine(KmToGameUnits(lastPosClockHigh), KmToGameUnits(PosClockHigh), 0.003f, line.col);
                        if(MyMwcUtils.HasValidLength(KmToGameUnits(PosClockLow) - KmToGameUnits(lastPosClockLow)))
                            DrawLine(KmToGameUnits(lastPosClockLow), KmToGameUnits(PosClockLow), 0.003f, line.col);

                        lastPosClockHigh = PosClockHigh;
                        lastPosClockLow = PosClockLow;
                        lastPosClockOrbit = PosClockOrbit;

                        
                        //counter-clockwise
                        angle = initAngle + stepI * stepSize;
                        x = MyMath.FastSin(angle) * r;
                        z = MyMath.FastCos(angle) * r;
                        PosCounterOrbit = GetPositionRelativeToCamera(x, 0, z);
                        //clockwise
                        //PosClockOrbit = new Vector3(lastPosClockOrbit.X * cos - lastPosClockOrbit.Y * sin, 0, lastPosClockOrbit.X * sin + lastPosClockOrbit.Y * cos);
                        PosCounterHigh = new Vector3((float)x, 0f, (float)z) * (((KmToGameUnits(line.DistanceHigh) / r) * (stepsCount - stepI) / stepsCount) + 1); PosCounterHigh = GetPositionRelativeToCamera(PosCounterHigh.X, PosCounterHigh.Y, PosCounterHigh.Z);
                        PosCounterLow = new Vector3((float)x, 0f, (float)z) * (-((KmToGameUnits(line.DistanceLow) / r) * (stepsCount - stepI) / stepsCount) + 1); PosCounterLow = GetPositionRelativeToCamera(PosCounterLow.X, PosCounterLow.Y, PosCounterLow.Z);
                        //DrawLine(lastPosCounterOrbit, PosCounterOrbit, 0.004f, line.col);
                        //draw line from lastPos to Pos (low & high)
                        if (MyMwcUtils.HasValidLength(KmToGameUnits(PosCounterHigh) - KmToGameUnits(lastPosCounterHigh)))
                            DrawLine(KmToGameUnits(lastPosCounterHigh), KmToGameUnits(PosCounterHigh), 0.003f, line.col);
                        if (MyMwcUtils.HasValidLength(KmToGameUnits(PosCounterLow) - KmToGameUnits(lastPosCounterLow)))
                            DrawLine(KmToGameUnits(lastPosCounterLow), KmToGameUnits(PosCounterLow), 0.003f, line.col);

                        lastPosCounterHigh = PosCounterHigh;
                        lastPosCounterLow = PosCounterLow;
                        lastPosCounterOrbit = PosCounterOrbit;
                    }
                }
                else
                {
                    AddOrbit(line.AreaCenter.Length() + line.DistanceHigh, line.col);
                    AddOrbit(line.AreaCenter.Length() - line.DistanceLow, line.col);
                }
            }
        }

        private void DrawLine(Vector3 pos1, Vector3 pos2, float thickness, Vector4 color){
            float dist = (pos2 - pos1).Length();
            AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapZeroPlaneLine, color, pos1, MyMwcUtils.Normalize(pos2 - pos1), dist, thickness, 0, 1);
        }

        void AddBillboards(List<MySolarSystemMapEntity> entities)
        {
            const float maxAsteroidsPerSector = 64;
            int numAsteroids = 0;
            int numDustFields = 0;
            int numAsteroidFields = 0;
            //m_asteroidRandom = new Random(0);

            BoundingFrustum frustum = new BoundingFrustum(m_setup.ViewMatrix.Value * m_setup.ProjectionMatrix.Value);

            foreach (var e in entities)
            {
                if (e.EntityType != MySolarSystemEntityEnum.FactionMap && e.EntityType != MySolarSystemEntityEnum.Orbit)
                {
                    Vector3 pos = EntityPosition(e, m_currentCamera);

                    var distance = Vector3.Distance(m_currentCamera.Target, pos);
                    if (e.EntityType == MySolarSystemEntityEnum.DustField || e.EntityType == MySolarSystemEntityEnum.AsteroidField)
                    {
                        if (distance > 2000)
                        {
                            continue;
                        }
                    }

                    if (frustum.Contains(pos) != ContainmentType.Contains)
                    {
                        continue;
                    }
                }

                switch (e.EntityType)
                {
                    case MySolarSystemEntityEnum.Sun:
                        AddSun(e);
                        break;

                    case MySolarSystemEntityEnum.Orbit:
                        AddOrbit(e.Radius);
                        break;

                    case MySolarSystemEntityEnum.DustField:
                        AddDustField(e);
                        numDustFields++;
                        break;

                    case MySolarSystemEntityEnum.AsteroidField:
                        AddAsteroidField(e);
                        numAsteroidFields++;
                        break;

                    case MySolarSystemEntityEnum.StaticAsteroid:
                        if(numAsteroids < maxAsteroidsPerSector)
                        {
                            numAsteroids++;
                            AddAsteroid(e);
                        }
                        break;

                    case MySolarSystemEntityEnum.FactionMap:
                        AddFactionMap(e);
                        break;

                    //case MySolarSystemEntityEnum.FactionInfo:
                    //    AddFactionInfo(e);
                    //    break;

                    case MySolarSystemEntityEnum.LargeShipIcon:
                        AddLargeShip(e);
                        break;

                    case MySolarSystemEntityEnum.OutpostIcon:
                        AddOutpost(e);
                        break;
                }
            }
        }

        public void Draw(MySolarSystemMapCamera camera, MySolarSystemMapData solarData)
        {
            MyTransparentGeometry.ClearBillboards();
            
            m_currentCamera = camera;
            m_currentSolarMapData = solarData;
            m_setup.ViewMatrix = camera.GetViewMatrixAtZero();
            //m_setup.ViewMatrix *= Matrix.CreateTranslation(camera.Position);
            m_setup.CameraPosition = camera.Position;
            float nearClip = 0.1f * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            float farClip = 100000000 * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            m_setup.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MyCamera.FieldOfView, MyCamera.ForwardAspectRatio, nearClip, farClip);
            //m_setup.ProjectionMatrix = camera.GetProjectionMatrix();

            MyTransparentGeometry.EnableColorize = true;
            MyTransparentGeometry.ColorizeColor = new Color(0.8f, 0.8f, 0.8f, 0.0f);
            MyTransparentGeometry.ColorizePlaneDistance = m_currentCamera.Position.Y + m_currentCamera.PositionSector.Y * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            MyTransparentGeometry.ColorizePlaneNormal = Vector3.UnitY;
            MyRender.RegisterRenderModule(MyRenderModuleEnum.SolarMapGrid, "Solar map grid", DrawGrid, MyRenderStage.AlphaBlendPreHDR);
            MyRender.PushRenderSetupAndApply(m_setup, ref m_backup);

            m_texts.ClearAllAllocated();

            AddBillboards(m_currentSolarMapData.Entities);
            AddAreasBorders(m_currentSolarMapData.AreasBorderLines);
            AddAreaTexts(m_currentSolarMapData.Areas);
            { //Add available missions
                AddNavigationMarks(m_currentSolarMapData.NavigationMarks);
            }

            if (MyFakes.DRAW_FACTION_AREAS_IN_SOLAR_MAP)
            {
                DrawFactionsDebug();
                DrawEditedFactionDebug();
            }

            MyHudText text = m_texts.Allocate();
            if (text != null)
            {
                text.Start(MyGuiManager.GetFontMinerWarsWhite(), new Vector2(
                    MyGuiManager.GetSafeFullscreenRectangle().X + MyGuiManager.GetSafeFullscreenRectangle().Width,
                    MyGuiManager.GetSafeFullscreenRectangle().Y + MyGuiManager.GetSafeFullscreenRectangle().Height),
                    Color.White, 1.0f, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
                text.Append(m_currentCamera.TargetSector.ToString());
            }

            Vector3 pos = new Vector3(m_currentCamera.TargetSector.X, m_currentCamera.TargetSector.Y, m_currentCamera.TargetSector.Z);
            int dist = (int)Math.Ceiling(SECTOR_RENDER_HALFSIZE);
            for (int x = (int)pos.X - dist; x <= (int)pos.X + dist; x++)
            {
                int y = 0;
                //for (int y = (int)pos.Y - dist; y <= (int)pos.Y + dist; y++)
                {
                    for (int z = (int)pos.Z - dist; z <= (int)pos.Z + dist; z++)
                    {
                        var sectorPos = new MyMwcVector3Int(x, y, z);
                        //float minSize = (x == pos.X && y == pos.Y && z == pos.Z) ? 1.5f : 3;
                        //float minSize = 1.5f;                        
                        int maxEntityCount = (x == pos.X && y == pos.Y && z == pos.Z) ? 150 : 100;                        

                        //MySolarSystemMapSectorData sectorData;
                        //if (!m_currentSolarMapData.SectorData.TryGetValue(sectorPos, out sectorData))
                        //{                            
                        //    var g = new MySectorGenerator(MyGuiScreenSolarSystemMap.UNIVERSE_SEED);
                        //    sectorData = g.GenerateSectorEntities(m_currentSolarMapData, sectorPos, 0, maxEntityCount);
                        //    m_currentSolarMapData.SectorData[sectorPos] = sectorData;
                        //}
                        //AddBillboards(sectorData.Entities);                        
                    }
                }
            }
            
            MyRender.Draw();
            DrawTexts();
            //DrawSolarDebug();

            MyRender.PopRenderSetupAndRevert(m_backup);
            MyRender.UnregisterRenderModule(MyRenderModuleEnum.SolarMapGrid);
            //MyRender.UnregisterRenderModule(SOLAR_DEBUG_DRAW);
            MyRender.UnregisterRenderModule(MyRenderModuleEnum.SolarObjects);
            MyTransparentGeometry.EnableColorize = false;
        }

        private void DrawTexts()
        {
            if (m_texts.GetAllocatedCount() <= 0) return;

            for (int i = 0; i < m_texts.GetAllocatedCount(); i++)
            {
                MyHudText text = m_texts.GetAllocatedItem(i);

                //  Fix the scale for screen resolution
                float fixedScale = text.Scale * MyGuiManager.GetSafeScreenScale();
                Vector2 sizeInPixelsScaled = text.Font.MeasureString(text.GetStringBuilder(), fixedScale);
                Vector2 screenCoord = MyGuiManager.GetAlignedCoordinate(text.Position, sizeInPixelsScaled, text.Alignement);

                text.Font.DrawString(screenCoord, text.Color, text.GetStringBuilder(), fixedScale/*, text.Rotation*/);
            }
        }

        private void DrawSunDebug(MySolarSystemMapEntity entity)
        {
            Matrix world = Matrix.CreateScale(KmToGameUnits(entity.Radius));
            MyDebugDraw.DrawSphereWireframe(world, Color.Yellow.ToVector3(), 1.0f);
        }

        private void DrawSolarDebug()
        {
            foreach (var e in m_currentSolarMapData.Entities)
            {
                switch (e.EntityType)
                {
                    case MySolarSystemEntityEnum.Sun:
                        DrawSunDebug(e);
                        break;
                }
            }
        }


        void DrawEditedFactionDebug()
        {
            float radiusInSMU = KmToGameUnits((MySolarSystemUtils.SectorsToKm(MySolarMapAreaInput.center) - MySolarSystemUtils.SectorsToKm(MySolarMapAreaInput.point)).Length());
            if (radiusInSMU <= 0.001)
            {
                return;
            }
            Vector3 center = KmToGameUnits(MySolarSystemUtils.SectorsToKm(MySolarMapAreaInput.center));

            int stepCount = 157; // FastSin depends on this value
            float step = MathHelper.TwoPi / stepCount;

            double x = MyMath.FastSin(0) * radiusInSMU;
            double z = MyMath.FastCos(0) * radiusInSMU;
            Vector3 prevPos = GetPositionRelativeToCamera(x + center.X, 0, z + center.Z);

            for (int i = 1; i <= stepCount; i++)
            {
                float angle = MathHelper.TwoPi / stepCount * i;

                x = MyMath.FastSin(angle) * radiusInSMU;
                z = MyMath.FastCos(angle) * radiusInSMU;
                Vector3 pos = GetPositionRelativeToCamera(x + center.X, 0, z + center.Z);
                float dist = (pos - prevPos).Length();
                AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapOrbitLine, Color.Green.ToVector4(), prevPos, MyMwcUtils.Normalize(pos - prevPos), dist, 0.002f, minDistFromCamera, 1);
                prevPos = pos;
            }
        }

        void DrawFactionsDebug()
        {
            foreach (var faction in MyFactions.FactionAreas)
            {
                foreach (var area in faction.Value)
                {
                    float radiusInSMU = KmToGameUnits(area.Radius);
                    Vector3 center = KmToGameUnits(area.Position);

                    int stepCount = 157; // FastSin depends on this value
                    float step = MathHelper.TwoPi / stepCount;

                    double x = MyMath.FastSin(0) * radiusInSMU;
                    double z = MyMath.FastCos(0) * radiusInSMU;
                    Vector3 prevPos = GetPositionRelativeToCamera(x + center.X, 0, z + center.Z);

                    for (int i = 1; i <= stepCount; i++)
                    {
                        float angle = MathHelper.TwoPi / stepCount * i;

                        x = MyMath.FastSin(angle) * radiusInSMU;
                        z = MyMath.FastCos(angle) * radiusInSMU;
                        Vector3 pos = GetPositionRelativeToCamera(x + center.X, 0, z + center.Z);
                        float dist = (pos - prevPos).Length();
                        AddLineBillboardUnscaling(MyTransparentMaterialEnum.SolarMapOrbitLine, Color.Red.ToVector4(), prevPos, MyMwcUtils.Normalize(pos - prevPos), dist, 0.002f, minDistFromCamera, 1);
                        prevPos = pos;
                    }
                }
            }
        }

        private void DrawSolarObjects()
        {
            //MyRender.DrawModel(MyModels.GetModelForDraw(MySession.PlayerShip.CockpitInteriorModelEnum), m_currentCamera.GetViewMatrix(), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="blendInHeight"></param>
        /// <param name="blendOutHeight">When camera height bigger than blendOutHeight, then alpha is 0</param>
        /// <param name="color"></param>
        private void DrawGridLevel(float scale, float blendInHeight, float blendOutHeight, Vector3 color, float absHeight)
        {       
            const float maxGridAlpha = 0.2f;
            float alphaIn = MathHelper.Clamp((absHeight - blendInHeight) / blendInHeight * 2, 0.0f, 1.0f);
            float alphaOut = MathHelper.Clamp((blendOutHeight - absHeight) / blendOutHeight * 2, 0.0f, 1.0f);
            float finalAlpha = alphaIn * alphaOut;

            if (finalAlpha > 0)
            {
                // Draw grid
                double s = (double)scale * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

                Vector3 posSector;
                posSector.X = m_currentCamera.PositionSector.X;
                posSector.Y = m_currentCamera.PositionSector.Y;
                posSector.Z = m_currentCamera.PositionSector.Z;
                
                Vector3 offset = m_currentCamera.CalculatePositionModulo(s);
                offset.Y = -m_currentCamera.Position.Y - posSector.Y * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

                // Near clip is not calculated exactly, just aproximately, this value is than corrected to make sure there's no clipping problems
                const float safeCoef = 1.2f;

                Matrix world = Matrix.CreateScale(scale, 1, scale) * Matrix.CreateTranslation(offset);
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(MyCamera.FieldOfView, MyCamera.ForwardAspectRatio, blendInHeight * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS / safeCoef, blendOutHeight * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS);

                MyEffectSolarMapGrid effect = (MyEffectSolarMapGrid)MyRender.GetEffect(MyEffects.SolarMapGrid);
                effect.SetWorldMatrix(world);
                effect.SetViewProjectionMatrix(m_currentCamera.GetViewMatrixAtZero() * projection);

                effect.SetColorA(color);
                effect.SetAlpha(finalAlpha * maxGridAlpha);

                effect.Begin();
                MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, m_gridVertexBuffer, 0, MyVertexFormatPositionTexture.Stride);
                MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatPositionTexture.VertexDeclaration;
                MyMinerGame.Static.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

                effect.End();
                MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;

                // Draw highlight
                effect.SetColorA(Color.White.ToVector3());
                effect.SetAlpha(finalAlpha);

                Vector3 pos = -m_currentCamera.Position - posSector * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

                double tx = m_currentCamera.Target.X + m_currentCamera.TargetSector.X * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
                double tz = m_currentCamera.Target.Z + m_currentCamera.TargetSector.Z * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

                tx = MyMwcUtils.Round(tx, s);
                tz = MyMwcUtils.Round(tz, s);

                double px = m_currentCamera.Position.X + posSector.X * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
                double py = m_currentCamera.Position.Y + posSector.Y * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
                double pz = m_currentCamera.Position.Z + posSector.Z * (double)MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;

                double mx = tx - px;
                double my = 0 - py;
                double mz = tz - pz;

                Vector3 move = new Vector3((float)mx, (float)my, (float)mz);
                offset.Y = 0;
                move = m_currentCamera.CameraToTarget;
                move -= offset;
                move.X = (float)MyMwcUtils.Round(move.X, s);
                move.Z = (float)MyMwcUtils.Round(move.Z, s);
                move += offset;

                Matrix targetWorld = Matrix.CreateScale(scale, 1, scale) * Matrix.CreateTranslation(move);
                effect.SetWorldMatrix(targetWorld);

                effect.Begin();
                MyMinerGame.Static.GraphicsDevice.SetStreamSource(0, m_highlightVertexBuffer, 0, MyVertexFormatPositionTexture.Stride);
                MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatPositionTexture.VertexDeclaration;
                MyMinerGame.Static.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                effect.End();
                MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;
            }  
        }

        /// <summary>
        /// One game unit is one sector (200km) and one grid cell when zoomed in to most detailed grid
        /// </summary>
        private void DrawGrid()
        {
            LoadInDraw();
                          
            Device device = MyMinerGame.Static.GraphicsDevice;
            DepthStencilState.None.Apply();
            RasterizerState.CullNone.Apply();
            BlendState.Additive.Apply();

            MyEffectSolarMapGrid effect = (MyEffectSolarMapGrid)MyRender.GetEffect(MyEffects.SolarMapGrid);
            effect.SetGridTexture(m_texture);

            int gridTileCount = 5;
            int maxLevel = (int)Math.Ceiling(10.0f / gridTileCount * 7.0f);

            float absHeight = Math.Abs(m_currentCamera.PositionSector.Y) + Math.Abs(m_currentCamera.Position.Y) / MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS;
            float scale = absHeight;
            int level = (int)Math.Log(scale, gridTileCount);
            if (level < 2)
            {
                level = 2;
            }
            else if (level > maxLevel)
            {
                level = maxLevel;
            }

            if (scale > 0)
            {
                scale = (float)Math.Pow(gridTileCount, level); // Round to logaritm (base 10), so values are 1, 10, 100 or 1000...
            }

            float blendInHeight = scale;
            float blendOutHeight = scale * 200;



            Vector3 color = GRID_HEIGHT_COLORS[level % GRID_HEIGHT_COLORS.Length] * 0.6f;
            Vector3 color2 = GRID_HEIGHT_COLORS[(level - 1) % GRID_HEIGHT_COLORS.Length] * 0.6f;
            Vector3 color3 = GRID_HEIGHT_COLORS[(level - 2) % GRID_HEIGHT_COLORS.Length] * 0.6f;

            DrawGridLevel(scale, blendInHeight, blendOutHeight, color, absHeight);

            if (level <= 2) // At least draw two bottom grids
            {
                blendInHeight = 0.1f;
            }

            DrawGridLevel(scale / gridTileCount, blendInHeight / gridTileCount, blendOutHeight / gridTileCount, color2, absHeight);
            DrawGridLevel(scale / (gridTileCount * gridTileCount), blendInHeight / (gridTileCount * gridTileCount), blendOutHeight / (gridTileCount * gridTileCount), color3, absHeight);
                            
        }

        private VertexBuffer LoadVertexBuffer(Vector3 position, float sectorsPerQuad)
        {      
            System.Diagnostics.Debug.Assert(sectorsPerQuad % 2 == 1, "Sectors per quad must be odd number - vectors are centered");

            float halfSize = sectorsPerQuad * MySolarSystemMapCamera.SECTOR_SIZE_GAMEUNITS / 2;

            Vector3 bottomLeft = new Vector3(position.X - halfSize, 0, position.Z - halfSize);
            Vector3 bottomRight = new Vector3(position.X + halfSize, 0, position.Z - halfSize);
            Vector3 topLeft = new Vector3(position.X - halfSize, 0, position.Z + halfSize);
            Vector3 topRight = new Vector3(position.X + halfSize, 0, position.Z + halfSize);

            Vector2 textureTopLeft = new Vector2(1f, 0.0f) * sectorsPerQuad;
            Vector2 textureTopRight = new Vector2(0.0f, 0.0f) * sectorsPerQuad;
            Vector2 textureBottomLeft = new Vector2(1f, 1f) * sectorsPerQuad;
            Vector2 textureBottomRight = new Vector2(0.0f, 1f) * sectorsPerQuad;

            MyVertexFormatPositionTexture[] verts = new MyVertexFormatPositionTexture[6];
            verts[0] = new MyVertexFormatPositionTexture(topLeft, textureTopLeft);
            verts[1] = new MyVertexFormatPositionTexture(topRight, textureTopRight);
            verts[2] = new MyVertexFormatPositionTexture(bottomLeft, textureBottomLeft);
            verts[3] = new MyVertexFormatPositionTexture(bottomLeft, textureBottomLeft);
            verts[4] = new MyVertexFormatPositionTexture(topRight, textureTopRight);
            verts[5] = new MyVertexFormatPositionTexture(bottomRight, textureBottomRight);

            VertexBuffer vertexBuffer = new VertexBuffer(MyMinerGame.Static.GraphicsDevice,  MyVertexFormatPositionTexture.Stride * verts.Length, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            vertexBuffer.Lock(0, verts.Length * MyVertexFormatPositionTexture.Stride, LockFlags.None).WriteRange(verts);
            vertexBuffer.Unlock();
            return vertexBuffer;          
        }

        private void LoadInDraw()
        {
            if (m_loaded) return;

            m_gridVertexBuffer = LoadVertexBuffer(Vector3.Zero, SECTORS_PER_QUAD);
            m_highlightVertexBuffer = LoadVertexBuffer(Vector3.Zero, 1);

            m_loaded = true;
        }

        public MyMwcVector3Int GetTargetSector()
        {
            return m_currentCamera.TargetSector;
        }
    }
}
