//using System;
//using System.Collections.Generic;
//using MinerWarsMath;
//using MinerWarsMath.Graphics;
//using MinerWars.AppCode.App;
//using MinerWars.AppCode.Game.GUI;
//using MinerWars.AppCode.Game.GUI.Core;
//using MinerWars.AppCode.Game.Entities;
//using MinerWars.AppCode.Game.Prefabs;
//using MinerWars.AppCode.Game.Radar;
//using MinerWars.AppCode.Game.Utils;
//using MinerWars.AppCode.Game.Voxels;
//using MinerWars.AppCode.Physics;
//using MinerWars.CommonLIB.AppCode.Generics;
//using SysUtils.Utils;
//using MinerWars.CommonLIB.AppCode.Utils;
//using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
//using MinerWars.AppCode.Game.World.Global;

////  This class draws 3D spherical radar with highlighted solar plane, distance to sun, phys objects as points, asteroids, etc
////  For drawing we use effect, vertex buffer, etc from MyHud; But here we use perspective projection.

//namespace MinerWars.AppCode.Game.HUD
//{
//    using Managers.Graphics.Textures;
//    using Managers.PhysicsManager.Physics;
//    using MinerWars.AppCode.Game.Missions;
//    using MinerWars.AppCode.Game.Managers.Session;
//    using MinerWars.AppCode.Game.Renders;
//    using MinerWars.CommonLIB.AppCode.Networking;
//    using MinerWars.AppCode.Game.Entities.Prefabs;
//    using MinerWars.AppCode.Game.Effects;
//    using MinerWars.AppCode.Game.Render;
//    using System.Diagnostics;

//    //  We use this class for depth sorting
//    public class MyPolygon : IComparable
//    {
//        public List<VertexPositionColorTexture> Points;
//        public int NumPoints;
//        float? m_averageZ;
//        float? m_averageY;

//        public MyPolygon()
//        {
//            Points = new List<VertexPositionColorTexture>(MyHudConstants.TRIANGLES_PER_HUD_QUAD * 3);
//        }

//        //  IMPORTANT: Must be called before we start adding points!
//        public void Start()
//        {
//            Points.Clear();
//            NumPoints = 0;
//            m_averageY = null;
//            m_averageZ = null;
//        }

//        //  IMPORTANT: Must be called after all points are added!
//        public void Finish()
//        {
//            NumPoints = Points.Count;

//            //  Calc average Y and Z of this poly
//            m_averageY = 0;
//            m_averageZ = 0;
//            foreach (VertexPositionColorTexture vert in Points)
//            {
//                m_averageY += vert.Position.Y;
//                m_averageZ += vert.Position.Z;
//            }
//            m_averageY /= NumPoints;
//            m_averageZ /= NumPoints;
//        }

//        public float GetAverageY()
//        {
//            return m_averageY.Value;
//        }

//        //  Used to sort methods base on Z-values
//        public int CompareTo(object obj)
//        {
//            //  m_averageZ could be null, but we don't check it here. If it null, than that 
//            //  means Finish() wasn't called so poly doesn't know its distance
//            return m_averageZ.Value.CompareTo(((MyPolygon)obj).m_averageZ.Value);
//        }
//    }    

//    static class MyHudRadar
//    {
//        //  This enums must have same name as source texture files used to create texture atlas
//        //  And only ".tga" files are supported.
//        //  IMPORTANT: If you change order or names in this enum, update it also in MyEnumsToStrings
//        public enum MyHudRadarTexturesEnum : byte
//        {
//            Arrow,
//            ImportantObject,
//            LargeShip,
//            Line,
//            RadarBackground,
//            RadarPlane,
//            SectorBorder,
//            SmallShip,
//            Sphere,
//            SphereGrid,
//            Sun,
//            OreDeposit_Treasure,
//            OreDeposit_Helium,
//            OreDeposit_Ice,
//            OreDeposit_Iron,
//            OreDeposit_Lava,
//            OreDeposit_Gold,
//            OreDeposit_Platinum,
//            OreDeposit_Silver,
//            OreDeposit_Silicon,
//            OreDeposit_Organic,
//            OreDeposit_Nickel,
//            OreDeposit_Magnesium,
//            OreDeposit_Uranite,
//            OreDeposit_Cobalt,
//            OreDeposit_Snow
//        }

//        class MyTypeAndColor
//        {
//            public MyHudRadarTexturesEnum Type { get; set; }
//            public Color Color { get; set; }

//            public MyTypeAndColor(MyHudRadarTexturesEnum type, Color color) 
//            {
//                Type = type;
//                Color = color;
//            }
//        }

//        static readonly MyTypeAndColor[] m_oreTypesAndColors;

//        static Matrix m_perspectiveProjectionMatrix;
//        static Matrix m_projectionMatrix2D;

//        static Texture2D m_texture;
//        static MyAtlasTextureCoordinate[] m_textureCoords;
//        static VertexPositionColorTexture[] m_vertices;
//        static MyStencilMask m_stencilOpaque;

//        //  Parameters updates/recalculated every update
//        static Vector3 m_actualCameraPosition;                                         //  Camera rotated every update, used only for polyline calculations
//        static MyPlane m_actualCameraPlane;
//        static List<MyPolygon> m_polygons;
//        static MyObjectsPoolSimple<MyPolygon> m_preAllocPolys;
//        static int m_quadsCount;
//        static float m_zoom;
//        static VertexPositionColorTexture[] outPoints;
//        static VertexPositionColorTexture[] inPoints;
//        static Vector3 m_radarCenter;

//        static Vector3[] m_sectorCornersTemp = new Vector3[MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX_CORNERS.Length];

//        // orientation matrix which we use to rotate objects on radar according to player rotation
//        static Matrix m_orientationMatrix;

//        private static bool m_blinkingCurrentDisplayed = false;
//        private static int m_blinkingLastTime = 0;
//        private static int m_blinkingInterval = 0;

//        private static List<MyDetectedObject> m_detectedObjects = new List<MyDetectedObject>();

//        static MyHudRadar() 
//        {
//            m_oreTypesAndColors = new MyTypeAndColor[Enum.GetValues(typeof(MyMwcVoxelMaterialsEnum)).Length];
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Cobalt_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Cobalt, Color.Brown);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Gold_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Gold, Color.Gold);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Helium3_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Helium, Color.Blue);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Helium4_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Helium, Color.Blue);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Ice_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Ice, Color.Aquamarine);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Iron_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Iron, Color.Red);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Iron_02] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Iron, Color.Red);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Lava_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Lava, Color.Red);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Magnesium_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Magnesium, Color.OrangeRed);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Nickel_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Nickel, Color.SaddleBrown);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Organic_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Organic, Color.GreenYellow);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Platinum_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Platinum, Color.LightGray);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Silicon_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Silicon, Color.Silver);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Silver_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Silver, Color.Silver);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Snow_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Snow, Color.Snow);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Treasure_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Treasure, Color.YellowGreen);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Treasure_02] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Treasure, Color.YellowGreen);
//            m_oreTypesAndColors[(int)MyMwcVoxelMaterialsEnum.Uranite_01] = new MyTypeAndColor(MyHudRadarTexturesEnum.OreDeposit_Uranite, Color.LightGreen);            
//        }

//        public static void LoadContent()
//        {
//            MyMwcLog.WriteLine("MyHudRadar.LoadContent() - START");
//            MyMwcLog.IncreaseIndent();
//            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHudRadar::LoadContent");

//            m_stencilOpaque = new MyStencilMask(0.5f, "RadarStencil");
//            m_stencilOpaque.LoadContent();

//            UpdateScreenSize();

//            ////  Initial zoom level is in the middle of min and max zoom lovels
//            //m_zoom = (MyHudConstants.RADAR_ZOOM_MIN + MyHudConstants.RADAR_ZOOM_MAX) / 2.0f;

//            //  Initial zoom level is in 90% of min and max zoom lovels
//            m_zoom = (MyHudConstants.RADAR_ZOOM_MAX - MyHudConstants.RADAR_ZOOM_MIN) * 0.1f + MyHudConstants.RADAR_ZOOM_MIN;

//            m_vertices = new VertexPositionColorTexture[MyHudConstants.MAX_HUD_RADAR_QUADS_COUNT * MyHudConstants.VERTEXES_PER_HUD_QUAD];

//            m_perspectiveProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MyHudConstants.RADAR_FIELD_OF_VIEW, MyCamera.HudRadarViewport.Width / (float)MyCamera.HudRadarViewport.Height /*MyHudConstants.RADAR_ASPECT_RATIO*/, 0.1f, 10000);
//            m_projectionMatrix2D = Matrix.CreateOrthographicOffCenter(0.0f, MyCamera.HudRadarViewport.Width, MyCamera.HudRadarViewport.Height, 0.0f, 0.0f, 1000);
//            m_polygons = new List<MyPolygon>(MyHudConstants.MAX_HUD_QUADS_COUNT);

//            m_preAllocPolys = new MyObjectsPoolSimple<MyPolygon>(MyHudConstants.MAX_HUD_QUADS_COUNT);

//            outPoints = new VertexPositionColorTexture[12];
//            inPoints = new VertexPositionColorTexture[12];

//            MyUtils.LoadTextureAtlas((MyCustomContentManager)MyMinerGame.Static.Content, MyEnumsToStrings.HudRadarTextures, "Textures\\HUD\\HudRadarAtlas", out m_texture, out m_textureCoords);

//            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
//            MyMwcLog.DecreaseIndent();
//            MyMwcLog.WriteLine("MyHudRadar.LoadContent() - END");
//        }

//        public static void UnloadContent()
//        {
//            if (m_stencilOpaque != null) m_stencilOpaque.UnloadContent();
//        }

//        public static void UpdateScreenSize()
//        {
//            if (m_stencilOpaque != null) m_stencilOpaque.UpdateScreenSize(MyCamera.HudRadarViewport);
//            m_radarCenter = new Vector3(MyCamera.HudRadarViewport.Width / 2f, 0, MyCamera.HudRadarViewport.Height / 2f);
//        }

//        //  Zooms in or out by specified delta. If zoomIn = true, then we are zooming in. If it's false, we are zooming out.
//        public static void Zoom(bool zoomIn)
//        {
//            if (zoomIn == true)
//            {
//                m_zoom -= MyHudConstants.RADAR_ZOOM_STEP;
//            }
//            else
//            {
//                m_zoom += MyHudConstants.RADAR_ZOOM_STEP;
//            }

//            m_zoom = MathHelper.Clamp(m_zoom, MyHudConstants.RADAR_ZOOM_MIN, MyHudConstants.RADAR_ZOOM_MAX);
//        }

//        public static float GetRadarZoomLevelInversed()
//        {
//            return MyHudConstants.RADAR_ZOOM_MAX - m_zoom + 1;
//        }

//        static void UpdateRadarCamera()
//        {
//            m_actualCameraPosition = MyHudConstants.ORIGINAL_CAMERA_POSITON;
//            m_actualCameraPlane = new MyPlane(Vector3.Zero, Vector3.Up);
//        }

//        public static void Draw()
//        {
//            MyCamera.EnableHudRadar();

//            m_orientationMatrix = Matrix.Invert(MySession.PlayerShip.WorldMatrix); // RBEl0 have identity matrix should be same.
//            m_orientationMatrix.Translation = Vector3.Zero;

//            //MyMinerGame.Static.GraphicsDevice.Clear(ClearOptions.Stencil, Color.White, 0.0f, 0);
//            m_stencilOpaque.DrawStencilMask();

//            ClearQuads();
//            UpdateRadarCamera();

//            bool displayDetectedObjects = true;
//            if (MySession.PlayerShip.ShipRadar != null && MySession.PlayerShip.ShipRadar.IsNearRadarJammerOrSunWind())
//            {
//                if (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_blinkingLastTime >= m_blinkingInterval)
//                {
//                    m_blinkingCurrentDisplayed = !m_blinkingCurrentDisplayed;
//                    m_blinkingLastTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
//                    m_blinkingInterval = MyMwcUtils.GetRandomInt(50, 150);
//                }                
//                displayDetectedObjects = m_blinkingCurrentDisplayed;
//            }

//            if (displayDetectedObjects)
//            {
//                AddFromShipRadar();
//            }

//            AddSunSphere();
//            AddSectorBorderLines();
//            AddPlayer();
//            AddMissions();

//            DrawBackground();
//            DrawVertices();
//        }                

//        private static void AddFromShipRadar()
//        {
//            if (MySession.PlayerShip.ShipRadar != null)
//            {
//                MySession.PlayerShip.ShipRadar.GetDetectedObjects(ref m_detectedObjects);
//                foreach (MyDetectedObject myDetectedObject in m_detectedObjects)
//                {
//                    Color color = MyHudConstants.LARGESHIP_MARKER_COLOR;
//                    MyHudRadarTexturesEnum type = MyHudRadarTexturesEnum.Sphere;
//                    Vector3 forward = Vector3.Forward;
//                    float sizeAsAtDistance = 600f;

//                    if (myDetectedObject.Object is MyEntity)
//                    {
//                        MyEntity entity = myDetectedObject.Object as MyEntity;
//                        if (MyMissions.IsMissionEntity(entity))
//                        {
//                            color = MyHudConstants.MISSION_MARKER_COLOR;
//                        }
//                        else
//                        {
//                            MyFactionRelationEnum status = MyFactions.GetFactionsRelation(entity, MySession.PlayerShip);
//                            switch (status)
//                            {
//                                case MyFactionRelationEnum.Friend:
//                                    color = MyHudConstants.FRIEND_MARKER_COLOR;
//                                    break;

//                                case MyFactionRelationEnum.Enemy:
//                                    color = MyHudConstants.BOT_MARKER_COLOR;
//                                    break;
//                            }
//                        }
//                        forward = entity.WorldMatrix.Forward;

//                        // ship
//                        if (myDetectedObject.Object is MyShip ||
//                            ((myDetectedObject.Object is MyPrefabBase) && (myDetectedObject.Object as MyPrefabBase).PrefabCategory == Prefabs.CategoryTypesEnum.LARGE_SHIPS) ||
//                            myDetectedObject.Object is MyPrefabContainer)
//                        {
//                            type = MyHudRadarTexturesEnum.LargeShip;
//                        }
//                        else
//                        {
//                            sizeAsAtDistance = 700f;
//                        }
//                        AddIconWithAxisLines(myDetectedObject.Position, color, type, sizeAsAtDistance, forward);
//                    }
//                    // ore deposit
//                    else if (myDetectedObject.Object is MyVoxelMapOreDepositCell)
//                    {
//                        MyVoxelMapOreDepositCell oreDeposit = (MyVoxelMapOreDepositCell)myDetectedObject.Object;
//                        bool detectedAll = false;
//                        foreach (MyDetectorBase detectorBase in myDetectedObject.DetectedBy)
//                        {
//                            if (detectorBase is MyAllKnowingRadar || detectorBase is MyPulseDetector)
//                            {
//                                detectedAll = true;
//                                break;
//                            }
//                        }
//                        if (detectedAll)
//                        {
//                            foreach (MyMwcVoxelMaterialsEnum voxelMaterial in oreDeposit.GetOreWithContent())
//                            {
//                                AddOreDeposit(myDetectedObject.Position, voxelMaterial, forward);
//                            }
//                        }
//                        else
//                        {
//                            foreach (MyDetectorBase detectorBase in myDetectedObject.DetectedBy)
//                            {
//                                if (detectorBase is MyOreDetector)
//                                {
//                                    MyOreDetector oreDetector = detectorBase as MyOreDetector;
//                                    AddOreDeposit(myDetectedObject.Position, oreDetector.OreMaterial, forward);
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        throw new MyMwcExceptionApplicationShouldNotGetHere();
//                    }
//                }                
//            }
//        }

//        private static void AddOreDeposit(Vector3 position, MyMwcVoxelMaterialsEnum voxelMaterial, Vector3 forward)
//        {
//            MyTypeAndColor oreTypeAndColor = m_oreTypesAndColors[(int)voxelMaterial];            
//            AddIconWithAxisLines(position, oreTypeAndColor.Color, oreTypeAndColor.Type, 300f, forward);
//        }

//        private static void AddMissions()
//        {
//            var list = MyMissions.GetAvailableMissions();
//            foreach (var mission in list)
//            {
//                //MyGuiScreenGamePlay.Static.GetSectorGroup()
//                if (mission.HasLocationEntity())
//                    AddIconWithAxisLines(mission.Location.Entity.GetPosition(), MyHudConstants.MISSION_MARKER_COLOR, MyHudRadarTexturesEnum.ImportantObject, 600f);
//            }

//            if (MyMissions.ActiveMission != null)
//            {
//                foreach (var activeSubMission in MyMissions.ActiveMission.ActiveObjectives)
//                {
//                    if (activeSubMission.HasLocationEntity())
//                    {
//                        AddIconWithAxisLines(activeSubMission.Location.Entity.GetPosition(), MyHudConstants.MISSION_MARKER_COLOR, MyHudRadarTexturesEnum.ImportantObject, 600f);
//                    }
//                }
//            }
//        }

//        private static void AddIconWithAxisLines(Vector3 position, Color color, MyHudRadarTexturesEnum type, float sizeAsAtDistance, Vector3? forwardVector = null)
//        {
//            Vector3 dir = new Vector3();
//            Vector3 point = new Vector3();
//            Vector3 projectedPoint = new Vector3();
//            float alpha = 1;

//            //bool physObjectIsInUpperPartOfPlane = true;

//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Solar2D)
//            {
//                //  In solar radar - we find direction from player to phys object then rotate it to according to vertical player rotation, then project it on solar plane and then draw
//                dir = position - MySession.PlayerShip.GetPosition();
//                //if (Vector3.Distance(position, Vector3.Down) < Vector3.Distance(position, Vector3.Up))
//                //    physObjectIsInUpperPartOfPlane = false;
//                float length = dir.Length();
//                dir = Vector3.Transform(dir, m_orientationMatrix);
//                MyUtils.ProjectPointOnPlane(ref dir, ref MyConstants.VECTOR3_UP, out dir);
//                dir = MyMwcUtils.Normalize(dir) * length;
//                dir = Rescale(dir);
//                point = m_radarCenter + dir;
//                projectedPoint = point;
//            }
//            else if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Player2D)
//            {
//                //  In players radar we find direction from player to phys object, then rotate it around players up vector and then project it on players plane
//                dir = position - MySession.PlayerShip.GetPosition();
//                //if (Vector3.Distance(position, MySession.PlayerShip.GetPosition() + MySession.PlayerShip.WorldMatrix.Down) < Vector3.Distance(position, MySession.PlayerShip.GetPosition() + MySession.PlayerShip.WorldMatrix.Up))
//                //    physObjectIsInUpperPartOfPlane = false;
//                float length = dir.Length();
//                //rotation = CalculateFullAngle(Vector3.Forward, new Vector3(MyGuiScreenGameBase.Static.PlayerShip.WorldMatrix.Forward.X, 0, MyGuiScreenGameBase.Static.PlayerShip.WorldMatrix.Forward.Z), Vector3.Up);
//                dir = Vector3.Transform(dir, m_orientationMatrix); //Vector3.Transform(dir, Matrix.CreateFromAxisAngle(MyGuiScreenGameBase.Static.PlayerShip.WorldMatrix.Up, rotation));
//                MyUtils.ProjectPointOnPlane(ref dir, MySession.PlayerShip.WorldMatrix.Up, out dir);
//                dir = MyMwcUtils.Normalize(dir) * length;
//                dir = Rescale(dir);
//                projectedPoint = m_radarCenter + dir;
//            }
//            else
//            {

//                point = Vector3.Transform(position - MyCamera.Position, MyCamera.ViewMatrixAtZero);

//                //if (Vector3.Distance(position, MySession.PlayerShip.GetPosition() + MySession.PlayerShip.WorldMatrix.Down) <
//                //    Vector3.Distance(position, MySession.PlayerShip.GetPosition() + MySession.PlayerShip.WorldMatrix.Up))
//                //    physObjectIsInUpperPartOfPlane = false;

//                projectedPoint = new Vector3(point.X, 0, point.Z);

//                //  Calculate fade with distance, anything past half range should begin to fade
//                alpha = 1;
//            }

//            if (alpha > 0)
//            {
//                if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Player2D ||
//                    MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Solar2D)
//                {
//                    //  rotation around billboad center so billboard points to correct direction
//                    float rot;
//                    if (forwardVector.HasValue)
//                        rot = -CalculateFullAngle(forwardVector.Value, new Vector3(MySession.PlayerShip.WorldMatrix.Forward.X, 0, MySession.PlayerShip.WorldMatrix.Forward.Z), Vector3.Up);
//                    else
//                        rot = 0;
//                    Add2DBillboard(type, new Vector3(projectedPoint.X, projectedPoint.Z, -1), MyHudConstants.RADAR_PHYS_OBJECT_SIZE, rot, color * alpha);

//                    //float arrowRotation = (physObjectIsInUpperPartOfPlane) ? 0 : MathHelper.Pi;
//                    //Add2DBillboard(MyHudRadarTexturesEnum.Arrow, new Vector3(projectedPoint.X + 10, projectedPoint.Z, -1), MyHudConstants.RADAR_PHYS_OBJECT_SIZE, arrowRotation, color * alpha);
//                }
//                else
//                {
//                    Color lineVerticalColor;
//                    Color pointColor;
//                    if (point.Y < 0)
//                    {
//                        lineVerticalColor = Color.DarkGray * ((180.0f / 255.0f) * alpha);
//                        pointColor = Color.Red * alpha;
//                    }
//                    else
//                    {
//                        lineVerticalColor = MyHudConstants.HUD_COLOR * alpha;
//                        pointColor = Color.Red * alpha;
//                    }

//                    Vector3 rescaledPoint = Rescale(point);
//                    Vector3 rescaledProjectedPoint = Rescale(projectedPoint);                    

//                    if ((rescaledPoint - rescaledProjectedPoint).Length() > MyMwcMathConstants.EPSILON)
//                    {
//                        // we calculate object size scale from distance from camere, because we want same object icon size on screen all time                                                                                                
//                        AddLine3D(rescaledPoint, rescaledProjectedPoint, lineVerticalColor, lineVerticalColor, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);

//                        //AddBillboard(type, rescaledPoint + MyHudConstants.HUD_RADAR_PHYS_OBJECT_POINT_DELTA,
//                        //    Rescale(MyHudConstants.RADAR_PHYS_OBJECT_SIZE) * 3, 0, color);
//                        float noScaleSize = GetNoscaleSizeFromPerspectiveDistance(rescaledPoint, MyHudConstants.RADAR_PHYS_OBJECT_SIZE, sizeAsAtDistance);
//                        AddBillboard(type, rescaledPoint + MyHudConstants.HUD_RADAR_PHYS_OBJECT_POINT_DELTA, noScaleSize, 0, color);

//                        //  Draw line from player to where object intersect horizontal plane
//                        Color lineHorizontalColor = Color.DimGray * alpha;
//                        AddLine3D(Vector3.Zero, rescaledProjectedPoint, lineHorizontalColor, lineHorizontalColor, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);

//                        //float arrowRotation = (physObjectIsInUpperPartOfPlane) ? 0 : MathHelper.Pi;
//                        ////AddBillboard(MyHudRadarTexturesEnum.Arrow, Rescale(point + (Vector3.Right * 50)) + MyHudConstants.HUD_RADAR_PHYS_OBJECT_POINT_DELTA,
//                        ////    Rescale(MyHudConstants.RADAR_PHYS_OBJECT_SIZE) * 3, arrowRotation, color);
//                        //AddBillboard(MyHudRadarTexturesEnum.Arrow, rescaledPoint + (Vector3.Right * noScaleSize * 1.2f) + MyHudConstants.HUD_RADAR_PHYS_OBJECT_POINT_DELTA,
//                        //    noScaleSize, arrowRotation, color);
//                    }
//                }
//            }
//        }

//        static float GetNoscaleSizeFromPerspectiveDistance(Vector3 objectPosition, float objectSize, float sizeAsAtDistance)
//        {
//            float rescaledObjectSize = objectSize * (m_actualCameraPosition - objectPosition + MyHudConstants.HUD_RADAR_PHYS_OBJECT_POINT_DELTA).Length() / sizeAsAtDistance;
//            return rescaledObjectSize;
//        }

//        static void DrawBackground()
//        {
//            //MyGuiManager.BeginSpriteBatch_StencilMask();
//            MyGuiManager.BeginSpriteBatch_StencilMask(MyStateObjects.STENCIL_MASK_REFERENCE_STENCIL);

//            //  In 2d radars we draw green bacground texture
//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Player2D ||
//                MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Solar2D)
//            {
//                MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudRadarTexturesEnum.RadarBackground);
//                Rectangle source = new Rectangle((int)(textureCoord.Offset.X * m_texture.Width), (int)(textureCoord.Offset.Y * m_texture.Height), (int)(textureCoord.Size.X * m_texture.Width), (int)(textureCoord.Size.Y * m_texture.Height));

//                MyGuiManager.DrawSpriteBatch(m_texture, new Rectangle(0, 0, MyCamera.HudRadarViewport.Width, MyCamera.HudRadarViewport.Height),
//                    source, MyHudConstants.HUD_RADAR_BACKGROUND_COLOR2D);
//            }
//            //  For 3D radars there is only blank texture
//            else
//            {
//                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Rectangle(0, 0, MyCamera.HudRadarViewport.Width, MyCamera.HudRadarViewport.Height),
//                    MyHudConstants.HUD_RADAR_BACKGROUND_COLOR);
//            }

//            MyGuiManager.EndSpriteBatch();
//            //MyGuiManager.EndSpriteBatch_StencilMask();
//        }

//        static void ClearQuads()
//        {
//            m_polygons.Clear();
//            m_quadsCount = 0;
//            m_preAllocPolys.ClearAllAllocated();
//        }

//        static void AddPlayer()
//        {
//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Player2D ||
//                MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Solar2D)
//            {
//                Add2DBillboard(MyHudRadarTexturesEnum.SmallShip, new Vector3(m_radarCenter.X, m_radarCenter.Z, 0), MyHudConstants.RADAR_PHYS_OBJECT_SIZE, 0, Color.Blue);
//            }
//            else
//            {
//                //  in 3d mode we add radar plane when filling vertex buffer - like radar plane
//                //AddBillboard(MyHudRadarTexturesEnum.SmallShip, Vector3.Zero, MyHudConstants.RADAR_PHYS_OBJECT_SIZE / 2f, 2f * ((float)Math.PI), Color.Blue);
//            }
//        }

//        //static void AddPhysObjects()
//        //{
//        //    BoundingBox radarBoundingBox = BoundingBoxHelper.InitialBox;
//        //    BoundingBoxHelper.AddPoint(MyCamera.Position + new Vector3(-MyHudConstants.RADAR_BOUNDING_BOX_SIZE_HALF, -MyHudConstants.RADAR_BOUNDING_BOX_SIZE_HALF, -MyHudConstants.RADAR_BOUNDING_BOX_SIZE_HALF), ref radarBoundingBox);
//        //    BoundingBoxHelper.AddPoint(MyCamera.Position + new Vector3(+MyHudConstants.RADAR_BOUNDING_BOX_SIZE_HALF, +MyHudConstants.RADAR_BOUNDING_BOX_SIZE_HALF, +MyHudConstants.RADAR_BOUNDING_BOX_SIZE_HALF), ref radarBoundingBox);
//        //    var skins = MyEntities.GetCollisionSkinsInIntersectingBoundingBox(ref radarBoundingBox);

//        //    foreach (MyRBElement skin in skins)
//        //    {
//        //        MyEntity physObject = ((MyGameRigidBody)skin.GetRigidBody().m_UserData).Entity;
//        //        //  we don't want to show all objects on radar like voxel maps or debris
//        //        if (!(physObject is MySmallShip || physObject is MyPrefabLargeShip)) continue;
//        //        Color col = MyHudConstants.LARGESHIP_MARKER_COLOR;
//        //        MyHudRadarTexturesEnum type = MyHudRadarTexturesEnum.SmallShip;
//        //        //mark friend here different color
//        //        MyFactionRelationEnum status = MyFactionRelationEnum.Neutral;

//        //        if (physObject is MyShip)
//        //            status = MyFactions.GetFactionsRelation(((MyShip)physObject).Faction, MySession.PlayerShip.Faction);

//        //        switch (status)
//        //        {
//        //            case MyFactionRelationEnum.Friend:
//        //                col = MyHudConstants.FRIEND_MARKER_COLOR;
//        //                break;

//        //            case MyFactionRelationEnum.Enemy:
//        //                col = MyHudConstants.BOT_MARKER_COLOR;
//        //                break;
//        //        }

//        //        if (physObject is MyPrefabLargeShip)
//        //            type = MyHudRadarTexturesEnum.LargeShip;

//        //        //  Skip voxel maps and player's ship and anything without a model
//        //        if ((physObject is MyVoxelMap) ||
//        //            (physObject is MyExplosionDebrisBase) ||
//        //            (physObject == MySession.PlayerShip) ||
//        //            (physObject.ModelLod0 == null)) continue;

//        //        AddIconWithAxisLines(physObject.GetPosition(), col, type, physObject.WorldMatrix.Forward);
//        //    }
//        //}

//        //  Calculates angle between players forward vector and vector3.forward in interval <0..2Pi> radians
//        static float CalculateFullAngle(Vector3 baseVector, Vector3 vector2, Vector3 upVector)
//        {
//            Vector3 forward = MyMwcUtils.Normalize(vector2);
//            float angle = MyUtils.GetAngleBetweenVectors(forward, baseVector);

//            Vector3 leftVector = Vector3.Transform(baseVector, Matrix.CreateFromAxisAngle(upVector, MathHelper.PiOver2));
//            float angleLeft = MyUtils.GetAngleBetweenVectors(leftVector, forward);

//            Vector3 northEastVector = Vector3.Transform(baseVector, Matrix.CreateFromAxisAngle(upVector, -MathHelper.PiOver4));
//            float angleNorthEast = MyUtils.GetAngleBetweenVectors(MyMwcUtils.Normalize(northEastVector), forward);

//            if (angleLeft < MathHelper.PiOver2 || (float.IsNaN(angleLeft) && angleNorthEast > MathHelper.PiOver2))
//            {
//                angle = MathHelper.Pi + (MathHelper.Pi - angle);
//            }
//            return angle;
//        }

//        //  Rescales coordinate according to actual zoom level
//        static Vector3 Rescale(Vector3 point)
//        {
//            return point / m_zoom;
//        }

//        //  Rescales size/length according to actual zoom level
//        static float Rescale(float val)
//        {
//            return val / m_zoom;
//        }

//        //  Add horizontal radar plane
//        static void AddRadarPlane(ref int index)
//        {
//            //  Rectangle representing the middle of the line
//            m_vertices[index + 0].Position = new Vector3(-1, 0, -1) * MyHudConstants.RADAR_PLANE_RADIUS;
//            m_vertices[index + 1].Position = new Vector3(+1, 0, -1) * MyHudConstants.RADAR_PLANE_RADIUS;
//            m_vertices[index + 2].Position = new Vector3(-1, 0, +1) * MyHudConstants.RADAR_PLANE_RADIUS;
//            m_vertices[index + 3].Position = new Vector3(+1, 0, -1) * MyHudConstants.RADAR_PLANE_RADIUS;
//            m_vertices[index + 4].Position = new Vector3(+1, 0, +1) * MyHudConstants.RADAR_PLANE_RADIUS;
//            m_vertices[index + 5].Position = new Vector3(-1, 0, +1) * MyHudConstants.RADAR_PLANE_RADIUS;

//            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudRadarTexturesEnum.RadarPlane);
//            m_vertices[index + 0].TextureCoordinate = textureCoord.Offset;
//            m_vertices[index + 1].TextureCoordinate = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y);
//            m_vertices[index + 2].TextureCoordinate = new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y);
//            m_vertices[index + 3].TextureCoordinate = m_vertices[index + 1].TextureCoordinate;
//            m_vertices[index + 4].TextureCoordinate = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y);
//            m_vertices[index + 5].TextureCoordinate = m_vertices[index + 2].TextureCoordinate;

//            Color color = Color.Gray * (180.0f / 255.0f);
//            m_vertices[index + 0].Color = color;
//            m_vertices[index + 1].Color = color;
//            m_vertices[index + 2].Color = color;
//            m_vertices[index + 3].Color = color;
//            m_vertices[index + 4].Color = color;
//            m_vertices[index + 5].Color = color;

//            index += 6;
//            m_quadsCount++;
//        }

//        //  Add player marked in 3d mode pointing forward
//        static void AddPlayerMarker3D(ref int index)
//        {
//            //  Rectangle representing the middle of the line
//            m_vertices[index + 0].Position = new Vector3(-1, 0, -1) * MyHudConstants.RADAR_PLANE_RADIUS * MyHudConstants.PLAYER_MARKER_MULTIPLIER;
//            m_vertices[index + 1].Position = new Vector3(+1, 0, -1) * MyHudConstants.RADAR_PLANE_RADIUS * MyHudConstants.PLAYER_MARKER_MULTIPLIER;
//            m_vertices[index + 2].Position = new Vector3(-1, 0, +1) * MyHudConstants.RADAR_PLANE_RADIUS * MyHudConstants.PLAYER_MARKER_MULTIPLIER;
//            m_vertices[index + 3].Position = new Vector3(+1, 0, -1) * MyHudConstants.RADAR_PLANE_RADIUS * MyHudConstants.PLAYER_MARKER_MULTIPLIER;
//            m_vertices[index + 4].Position = new Vector3(+1, 0, +1) * MyHudConstants.RADAR_PLANE_RADIUS * MyHudConstants.PLAYER_MARKER_MULTIPLIER;
//            m_vertices[index + 5].Position = new Vector3(-1, 0, +1) * MyHudConstants.RADAR_PLANE_RADIUS * MyHudConstants.PLAYER_MARKER_MULTIPLIER;

//            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudRadarTexturesEnum.SmallShip);
//            m_vertices[index + 0].TextureCoordinate = textureCoord.Offset;
//            m_vertices[index + 1].TextureCoordinate = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y);
//            m_vertices[index + 2].TextureCoordinate = new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y);
//            m_vertices[index + 3].TextureCoordinate = m_vertices[index + 1].TextureCoordinate;
//            m_vertices[index + 4].TextureCoordinate = new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y);
//            m_vertices[index + 5].TextureCoordinate = m_vertices[index + 2].TextureCoordinate;

//            m_vertices[index + 0].Color = MyHudConstants.PLAYER_MARKER_COLOR;
//            m_vertices[index + 1].Color = MyHudConstants.PLAYER_MARKER_COLOR;
//            m_vertices[index + 2].Color = MyHudConstants.PLAYER_MARKER_COLOR;
//            m_vertices[index + 3].Color = MyHudConstants.PLAYER_MARKER_COLOR;
//            m_vertices[index + 4].Color = MyHudConstants.PLAYER_MARKER_COLOR;
//            m_vertices[index + 5].Color = MyHudConstants.PLAYER_MARKER_COLOR;

//            index += 6;
//            m_quadsCount++;
//        }

//        //  Add a marker to where the from an object intersects the horizontal plane
//        static void AddPlaneMarker(Vector3 centerPoint)
//        {
//            //  Rectangle representing the middle of the line
//            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudRadarTexturesEnum.Sphere);

//            MyPolygon poly = m_preAllocPolys.Allocate();
//            if (poly != null)
//            {
//                poly.Start();
//                poly.Points.Add(new VertexPositionColorTexture(new Vector3(centerPoint.X - .05f, 0, centerPoint.Z - .05f) * 100, Color.White, textureCoord.Offset));
//                poly.Points.Add(new VertexPositionColorTexture(new Vector3(centerPoint.X + .05f, 0, centerPoint.Z - .05f) * 100, Color.White, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(new Vector3(centerPoint.X - .05f, 0, centerPoint.Z + .05f) * 100, Color.White, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(new Vector3(centerPoint.X + .05f, 0, centerPoint.Z - .05f) * 100, Color.White, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(new Vector3(centerPoint.X + .05f, 0, centerPoint.Z + .05f) * 100, Color.White, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(new Vector3(centerPoint.X - .05f, 0, centerPoint.Z + .05f * 100), Color.White, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                poly.Finish();

//                m_polygons.Add(poly);
//                m_quadsCount++;
//            }
//        }

//        //  Add 3D line into the lines list. Before that, we convert line to quad.
//        static void AddLine3D(Vector3 vertex0, Vector3 vertex1, Color color0, Color color1, float lineThicknessHalf)
//        {
//            Vector3 directionVector = vertex0 - vertex1;
//            if (directionVector.LengthSquared() <= MyMwcMathConstants.EPSILON_SQUARED) 
//            {
//                return;
//            }
//            Vector3 directionNormalized = MyMwcUtils.Normalize(directionVector);

//            MyPolyLine polyLine;
//            polyLine.LineDirectionNormalized = directionNormalized;
//            polyLine.Point0 = vertex0;
//            polyLine.Point1 = vertex1;
//            polyLine.Thickness = lineThicknessHalf;

//            MyQuad quad;
//            MyUtils.GetPolyLineQuad(out quad, ref polyLine, m_actualCameraPosition);

//            //  Rectangle representing the middle of the line

//            //  Calculating texture coordinates for 2D line is little hack. Because we use texture atlas, we must have some border
//            //  arround line texture. Another hack is that horizontal texture coordinate is always 0.5 (in texture) as we
//            //  don't want to interpolate between dark border and real line texture.
//            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudRadarTexturesEnum.Line);

//            MyPolygon poly = m_preAllocPolys.Allocate();
//            if (poly != null)
//            {
//                poly.Start();
//                poly.Points.Add(new VertexPositionColorTexture(quad.Point0, color0, textureCoord.Offset + new Vector2(textureCoord.Size.X * 0.5f, textureCoord.Size.Y * 0.25f)));
//                poly.Points.Add(new VertexPositionColorTexture(quad.Point1, color1, new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.25f)));
//                poly.Points.Add(new VertexPositionColorTexture(quad.Point3, color0, new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.75f)));
//                poly.Points.Add(new VertexPositionColorTexture(quad.Point1, color1, new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.25f)));
//                poly.Points.Add(new VertexPositionColorTexture(quad.Point2, color1, new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.75f)));
//                poly.Points.Add(new VertexPositionColorTexture(quad.Point3, color1, new Vector2(textureCoord.Offset.X + textureCoord.Size.X * 0.5f, textureCoord.Offset.Y + textureCoord.Size.Y * 0.75f)));
//                poly.Finish();

//                m_polygons.Add(poly);
//                m_quadsCount++;
//            }
//        }

//        //  Add textured billboard quad always pointing to camera
//        static void AddBillboard(MyHudRadarTexturesEnum texture, Vector3 origin, float radius, float angle, Color color)
//        {
//            MyQuad quad;
//            /*if (texture == MyHudRadarTexturesEnum.Arrow ||
//                texture == MyHudRadarTexturesEnum.ImportantObject ||
//                texture == MyHudRadarTexturesEnum.LargeShip ||
//                texture == MyHudRadarTexturesEnum.SmallShip)
//            {
//                int ewwer = 234;
//            }*/

//            if (MyUtils.GetBillboardQuadAdvancedRotated(out quad, origin, radius, angle, m_actualCameraPosition) != false)
//            {
//                MyAtlasTextureCoordinate textureCoord = GetTextureCoord(texture);

//                MyPolygon poly = m_preAllocPolys.Allocate();
//                if (poly != null)
//                {
//                    poly.Start();
//                    poly.Points.Add(new VertexPositionColorTexture(quad.Point0, color, textureCoord.Offset));
//                    poly.Points.Add(new VertexPositionColorTexture(quad.Point1, color, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//                    poly.Points.Add(new VertexPositionColorTexture(quad.Point3, color, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                    poly.Points.Add(new VertexPositionColorTexture(quad.Point1, color, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//                    poly.Points.Add(new VertexPositionColorTexture(quad.Point2, color, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                    poly.Points.Add(new VertexPositionColorTexture(quad.Point3, color, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                    poly.Finish();

//                    m_polygons.Add(poly);
//                    m_quadsCount++;
//                }
//            }
//        }

//        //  Add textured billboard always pointing to camera which can be used for ortographic view - it doesn't take in account view matrix when rendered
//        static void Add2DBillboard(MyHudRadarTexturesEnum texture, Vector3 origin, float radius, float angle, Color color)
//        {
//            Add2DBillboard(texture, origin, origin, radius, angle, color);
//        }

//        //  Add textured billboard always pointing to camera which can be used for ortographic view - it doesn't take in account view matrix when rendered
//        static void Add2DBillboard(MyHudRadarTexturesEnum texture, Vector3 origin, Vector3 rotationCenter, float radius, float angle, Color color)
//        {
//            Vector3 dir = Vector3.Zero;
//            Vector3 topLeft = new Vector3(origin.X - radius / 2f, origin.Y - radius / 2f, origin.Z);
//            dir = topLeft - rotationCenter;
//            dir = Vector3.Transform(dir, Matrix.CreateRotationZ(angle));
//            topLeft = rotationCenter + dir;

//            Vector3 topRight = new Vector3(origin.X + radius / 2f, origin.Y - radius / 2f, origin.Z);
//            dir = topRight - rotationCenter;
//            dir = Vector3.Transform(dir, Matrix.CreateRotationZ(angle));
//            topRight = rotationCenter + dir;

//            Vector3 bottomLeft = new Vector3(origin.X - radius / 2f, origin.Y + radius / 2f, origin.Z);
//            dir = bottomLeft - rotationCenter;
//            dir = Vector3.Transform(dir, Matrix.CreateRotationZ(angle));
//            bottomLeft = rotationCenter + dir;

//            Vector3 bottomRight = new Vector3(origin.X + radius / 2f, origin.Y + radius / 2f, origin.Z);
//            dir = bottomRight - rotationCenter;
//            dir = Vector3.Transform(dir, Matrix.CreateRotationZ(angle));
//            bottomRight = rotationCenter + dir;

//            MyAtlasTextureCoordinate textureCoord = GetTextureCoord(texture);

//            MyPolygon poly = m_preAllocPolys.Allocate();
//            if (poly != null)
//            {
//                poly.Start();
//                poly.Points.Add(new VertexPositionColorTexture(topLeft, color, textureCoord.Offset));
//                poly.Points.Add(new VertexPositionColorTexture(topRight, color, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(bottomLeft, color, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(topRight, color, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(bottomRight, color, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                poly.Points.Add(new VertexPositionColorTexture(bottomLeft, color, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//                poly.Finish();

//                m_polygons.Add(poly);
//                m_quadsCount++;
//            }
//        }

//        //  Adds the radar sphere and sun indicator
//        private static void AddSunSphere()
//        {
//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Solar2D)
//            {
//                Vector3 fwd = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
//                fwd = Vector3.Transform(fwd, m_orientationMatrix);
//                MyUtils.ProjectPointOnPlane(ref fwd, ref MyConstants.VECTOR3_UP, out fwd);
//                fwd = MyMwcUtils.Normalize(fwd);
//                Vector3 pos = m_radarCenter + (fwd * MyHudConstants.RADAR_PLANE_RADIUS) * 1.1f;
//                //  Rotation around sun billboard center. So every time sun billboard is always facing to radar center
//                float rot = -CalculateFullAngle(Vector3.Forward, new Vector3(MySession.PlayerShip.WorldMatrix.Forward.X, 0, MySession.PlayerShip.WorldMatrix.Forward.Z), Vector3.Up);
//                Add2DBillboard(MyHudRadarTexturesEnum.Sun, new Vector3(pos.X, pos.Z, -1), MyHudConstants.RADAR_SUN_BILLBOARD_SIZE, rot, Color.White);
//            }
//            else if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Player2D)
//            {
//                Vector3 dirToSun = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
//                Vector3 fwd = Vector3.Transform(dirToSun, m_orientationMatrix);
//                fwd = MyMwcUtils.Normalize(fwd);
//                Vector3 pos = (fwd * MyHudConstants.RADAR_PLANE_RADIUS) * 1.1f;
//                MyUtils.ProjectPointOnPlane(ref pos, MySession.PlayerShip.WorldMatrix.Up, out pos);
//                pos += m_radarCenter;
//                //  Rotation around sun billboard center. So every time sun billboard is always facing to radar center
//                float rot = -CalculateFullAngle(Vector3.Forward, new Vector3(MySession.PlayerShip.WorldMatrix.Forward.X, 0, MySession.PlayerShip.WorldMatrix.Forward.Z), Vector3.Up);
//                Add2DBillboard(MyHudRadarTexturesEnum.Sun, new Vector3(pos.X, pos.Z, -1), MyHudConstants.RADAR_SUN_BILLBOARD_SIZE, rot, Color.White);
//            }
//            else
//            {
//                Matrix viewMatrix = MyCamera.ViewMatrixAtZero;
//                Vector3 sunPosition = Vector3.Transform(MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized() * (MyHudConstants.RADAR_SPHERE_RADIUS + 1), viewMatrix);//GetSunPositionOnRadar();
//                Vector3 projectedPoint = new Vector3(sunPosition.X, 0, sunPosition.Z);
//                if ((sunPosition - projectedPoint).Length() > MyMwcMathConstants.EPSILON)
//                    AddLine3D(sunPosition, projectedPoint, Color.Orange, Color.Orange, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);

//                AddLine3D(Vector3.Zero, projectedPoint, Color.Orange, Color.Orange, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                //AddBillboard(MyHudRadarTexturesEnum.Sun, sunPosition, MyHudConstants.RADAR_PHYS_OBJECT_SIZE, 0, Color.White);
//                AddBillboard(MyHudRadarTexturesEnum.Sun, sunPosition, GetNoscaleSizeFromPerspectiveDistance(sunPosition, MyHudConstants.RADAR_PHYS_OBJECT_SIZE, 200f), 0, Color.White);                
//                AddSphere();
//            }
//        }

//        private static void AddSectorBorderLines()
//        {
//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Player2D ||
//                MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Solar2D)
//            {
//                Vector3[] corners = MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX_CORNERS;

//                //  We calculate how far we are to front and side sector border in percentages(-1..1). and according to this we draw sector border billboard rotated around player position on radar 
//                float percentageFront = (corners[4].Z - MySession.PlayerShip.GetPosition().Z) / MyMwcSectorConstants.SECTOR_SIZE_HALF;
//                percentageFront += 1;
//                //  We lineary decrease percentage because otherwise if player is near sector border this billboard would overlap player marker in radar
//                percentageFront *= MyHudConstants.RADAR_SECTOR_BORDER_REPOSITION;

//                float percentageSide = (corners[0].X - MySession.PlayerShip.GetPosition().X) / MyMwcSectorConstants.SECTOR_SIZE_HALF;
//                percentageSide += 1;
//                //  We lineary decrease percentage because otherwise if player is near sector border this billboard would overlap player marker in radar
//                percentageSide *= MyHudConstants.RADAR_SECTOR_BORDER_REPOSITION;

//                float rotation = -CalculateFullAngle(Vector3.Forward, new Vector3(MySession.PlayerShip.WorldMatrix.Forward.X, 0, MySession.PlayerShip.WorldMatrix.Forward.Z), Vector3.Up);
//                Add2DBillboard(MyHudRadarTexturesEnum.SectorBorder, new Vector3(m_radarCenter.X + (percentageSide * MyHudConstants.RADAR_PLANE_RADIUS), m_radarCenter.Z + (percentageFront * MyHudConstants.RADAR_PLANE_RADIUS), -1), new Vector3(m_radarCenter.X, m_radarCenter.Z, 1), MyHudConstants.RADAR_SECTOR_BORDER_BILLBOARD_SIZE, rotation, Color.White);
//            }
//            else
//            {
//                Matrix viewMatrix = MyCamera.ViewMatrixAtZero;
//                Vector3 playerPos = MySession.PlayerShip.GetPosition();

//                float multiplier = 1;
//                for (int i = 0; i < MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX_CORNERS.Length; i++)
//                {
//                    m_sectorCornersTemp[i] = Vector3.Transform(MyMwcSectorConstants.SAFE_SECTOR_SIZE_BOUNDING_BOX_CORNERS[i] - MyCamera.Position, viewMatrix);
//                    if (Vector3.Distance(playerPos, m_sectorCornersTemp[i]) > MyHudConstants.RADAR_PLANE_RADIUS * 1f)
//                    {
//                        float newMult = Vector3.Multiply(MyMwcUtils.Normalize(m_sectorCornersTemp[i]), MyHudConstants.RADAR_PLANE_RADIUS).Length() / m_sectorCornersTemp[i].Length();
//                        if (newMult < multiplier)
//                        {
//                            multiplier = newMult;
//                        }
//                    }
//                }

//                for (int i = 0; i < m_sectorCornersTemp.Length; i++)
//                {
//                    m_sectorCornersTemp[i] *= multiplier;
//                }

//                AddLine3D(m_sectorCornersTemp[0], m_sectorCornersTemp[1], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[1], m_sectorCornersTemp[5], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[5], m_sectorCornersTemp[4], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[4], m_sectorCornersTemp[0], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);

//                AddLine3D(m_sectorCornersTemp[3], m_sectorCornersTemp[2], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[2], m_sectorCornersTemp[6], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[6], m_sectorCornersTemp[7], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[7], m_sectorCornersTemp[3], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);

//                AddLine3D(m_sectorCornersTemp[0], m_sectorCornersTemp[3], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[1], m_sectorCornersTemp[2], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[5], m_sectorCornersTemp[6], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//                AddLine3D(m_sectorCornersTemp[4], m_sectorCornersTemp[7], MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.RADAR_SECTOR_BORDER_COLOR, MyHudConstants.DIRECTION_TO_SUN_LINE_THICKNESS);
//            }
//        }

//        //  Draws a sphere                                                  
//        static void AddSphere()
//        {
//            //    Vector3 center = Vector3.Zero;
//            //    float radius = MyHudConstants.RADAR_SPHERE_RADIUS;
//            //    int tessellation = 10;
//            //    int verticalSegments = tessellation;
//            //    int horizontalSegments = tessellation * 2;
//            //    Color sphereColor = Color.White * (90.0f / 255.0f);
//            //    int vertexIndexMiddle = m_quadsCount * MyHudConstants.VERTEXES_PER_HUD_QUAD;
//            //    int vertexCount = 0;

//            //    //  Draw fan at bottom of sphere
//            //    float latitude = (MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;
//            //    float dy = (float)Math.Sin(latitude);
//            //    float dxz = (float)Math.Cos(latitude);
//            //    MyAtlasTextureCoordinate textureCoord = GetTextureCoord(MyHudRadarTexturesEnum.SphereGrid);

//            //    for (int i = 0; i < horizontalSegments; i++)
//            //    {
//            //        float longitude = i * MathHelper.TwoPi / horizontalSegments;
//            //        float dx = (float)Math.Cos(longitude) * dxz;
//            //        float dz = (float)Math.Sin(longitude) * dxz;

//            //        float longitude2 = (i + 1) * MathHelper.TwoPi / horizontalSegments;
//            //        float dx2 = (float)Math.Cos(longitude2) * dxz;
//            //        float dz2 = (float)Math.Sin(longitude2) * dxz;

//            //        Vector3 normal = new Vector3(dx, dy, dz);
//            //        Vector3 normal2 = new Vector3(dx2, dy, dz2);

//            //        MyPolygon poly = m_preAllocPolys.Allocate();
//            //        if (poly != null)
//            //        {
//            //            poly.Start();
//            //            poly.Points.Add(new VertexPositionColorTexture(Vector3.Transform(normal2 * radius, MyCamera.ViewMatrixAtZero), sphereColor, textureCoord.Offset));
//            //            poly.Points.Add(new VertexPositionColorTexture(Vector3.Transform(normal * radius, MyCamera.ViewMatrixAtZero), sphereColor, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//            //            poly.Points.Add(new VertexPositionColorTexture(Vector3.Transform(Vector3.Down * radius, MyCamera.ViewMatrixAtZero), sphereColor, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//            //            poly.Finish();

//            //            m_polygons.Add(poly);
//            //            vertexCount += 3;
//            //        }
//            //    }

//            //    //  Create rings of vertices at progressively higher latitudes.
//            //    for (int i = 0; i < verticalSegments - 1; i++)
//            //    {
//            //        latitude = ((i + 1) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

//            //        dy = (float)Math.Sin(latitude);
//            //        dxz = (float)Math.Cos(latitude);
//            //        float latitude2 = ((i + 2) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

//            //        float dy2 = (float)Math.Sin(latitude2);
//            //        float dxz2 = (float)Math.Cos(latitude2);

//            //        //  Create a single ring of vertices at this latitude.
//            //        for (int j = 0; j < horizontalSegments; j++)
//            //        {
//            //            float longitude = j * MathHelper.TwoPi / horizontalSegments;

//            //            float dx = (float)Math.Cos(longitude) * dxz;
//            //            float dz = (float)Math.Sin(longitude) * dxz;

//            //            float longitude2 = (j + 1) * MathHelper.TwoPi / horizontalSegments;
//            //            float dx2 = (float)Math.Cos(longitude2) * dxz;
//            //            float dz2 = (float)Math.Sin(longitude2) * dxz;

//            //            float dx3 = (float)Math.Cos(longitude) * dxz2;
//            //            float dz3 = (float)Math.Sin(longitude) * dxz2;

//            //            float dx4 = (float)Math.Cos(longitude2) * dxz2;
//            //            float dz4 = (float)Math.Sin(longitude2) * dxz2;

//            //            Vector3 normal = new Vector3(dx, dy, dz);
//            //            Vector3 normal2 = new Vector3(dx2, dy, dz2);
//            //            Vector3 normal3 = new Vector3(dx3, dy2, dz3);
//            //            Vector3 normal4 = new Vector3(dx4, dy2, dz4);

//            //            MyPolygon poly = m_preAllocPolys.Allocate();
//            //            if (poly != null)
//            //            {
//            //                Vector3 point0 = MyUtils.GetTransform(normal * radius, ref MyCamera.ViewMatrixAtZero);
//            //                Vector3 point1 = MyUtils.GetTransform(normal2 * radius, ref MyCamera.ViewMatrixAtZero);
//            //                Vector3 point2 = MyUtils.GetTransform(normal3 * radius, ref MyCamera.ViewMatrixAtZero);
//            //                Vector3 point3 = MyUtils.GetTransform(normal2 * radius, ref MyCamera.ViewMatrixAtZero);
//            //                Vector3 point4 = MyUtils.GetTransform(normal4 * radius, ref MyCamera.ViewMatrixAtZero);
//            //                Vector3 point5 = MyUtils.GetTransform(normal3 * radius, ref MyCamera.ViewMatrixAtZero);

//            //                Color a = sphereColor;
//            //                if (point0.Z >= 0) a.A = 30;

//            //                poly.Start();
//            //                poly.Points.Add(new VertexPositionColorTexture(point0, a, textureCoord.Offset));
//            //                poly.Points.Add(new VertexPositionColorTexture(point1, a, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//            //                poly.Points.Add(new VertexPositionColorTexture(point2, a, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//            //                poly.Points.Add(new VertexPositionColorTexture(point3, a, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//            //                poly.Points.Add(new VertexPositionColorTexture(point4, a, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//            //                poly.Points.Add(new VertexPositionColorTexture(point5, a, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//            //                poly.Finish();

//            //                m_polygons.Add(poly);
//            //                vertexCount += 6;
//            //            }
//            //        }
//            //    }

//            //    latitude = ((verticalSegments) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

//            //    dy = (float)Math.Sin(latitude);
//            //    dxz = (float)Math.Cos(latitude);

//            //    //  Create vertex fan at top of sphere
//            //    for (int i = 1; i < horizontalSegments + 1; i++)
//            //    {
//            //        float longitude = i * MathHelper.TwoPi / horizontalSegments;
//            //        float dx = (float)Math.Cos(longitude) * dxz;
//            //        float dz = (float)Math.Sin(longitude) * dxz;

//            //        float longitude2 = (i - 1) * MathHelper.TwoPi / horizontalSegments;
//            //        float dx2 = (float)Math.Cos(longitude2) * dxz;
//            //        float dz2 = (float)Math.Sin(longitude2) * dxz;

//            //        Vector3 normal = new Vector3(dx, dy, dz);
//            //        Vector3 normal2 = new Vector3(dx2, dy, dz2);

//            //        MyPolygon poly = m_preAllocPolys.Allocate();
//            //        if (poly != null)
//            //        {
//            //            poly.Start();
//            //            poly.Points.Add(new VertexPositionColorTexture(Vector3.Transform(Vector3.Up * radius, MyCamera.ViewMatrixAtZero), sphereColor, textureCoord.Offset));
//            //            poly.Points.Add(new VertexPositionColorTexture(Vector3.Transform(normal * radius, MyCamera.ViewMatrixAtZero), sphereColor, new Vector2(textureCoord.Offset.X + textureCoord.Size.X, textureCoord.Offset.Y)));
//            //            poly.Points.Add(new VertexPositionColorTexture(Vector3.Transform(normal2 * radius, MyCamera.ViewMatrixAtZero), sphereColor, new Vector2(textureCoord.Offset.X, textureCoord.Offset.Y + textureCoord.Size.Y)));
//            //            poly.Finish();

//            //            m_polygons.Add(poly);
//            //            vertexCount += 3;
//            //        }
//            //    }
//            //    m_quadsCount += vertexCount / MyHudConstants.VERTEXES_PER_HUD_QUAD;
//        }

//        static MyAtlasTextureCoordinate GetTextureCoord(MyHudRadarTexturesEnum texture)
//        {
//            return m_textureCoords[(int)texture];
//        }

//        //  Fill vertex buffer with polygons sorted by depth
//        //  This imitates a z-buffer
//        static void CopyQuadsToVertices()
//        {
//            int x = 0;
//            m_polygons.Sort();

//            //  First add polygons below the plane
//            foreach (MyPolygon poly in m_polygons)
//            {
//                if (poly.GetAverageY() < 0)
//                {
//                    foreach (VertexPositionColorTexture vert in poly.Points)
//                    {
//                        m_vertices[x++] = vert;
//                    }
//                }
//            }

//            //  Then add the plane if we are in 3D mode
//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Normal3D)
//            {
//                AddRadarPlane(ref x);
//                AddPlayerMarker3D(ref x);
//            }

//            //  Then add polygons above the plane
//            foreach (MyPolygon poly in m_polygons)
//            {
//                if (poly.GetAverageY() >= 0)
//                {
//                    foreach (VertexPositionColorTexture vert in poly.Points)
//                    {
//                        m_vertices[x++] = vert;
//                    }
//                }
//            }
//        }

//        //  Finally draw all lines
//        static void DrawVertices()
//        {
//            if (m_quadsCount <= 0) return;

//            CopyQuadsToVertices();

//            GraphicsDevice device = MyMinerGame.Static.GraphicsDevice;

//            device.DepthStencilState = MyStateObjects.StencilMask_TestHudBegin_DepthStencilState;
//            device.BlendState = BlendState.NonPremultiplied;
//            device.RasterizerState = RasterizerState.CullNone;

//            MyEffectHudRadar effect = (MyEffectHudRadar)MyRender.GetEffect(MyEffects.HudRadar);

//            if (MySession.PlayerShip.Config.RadarType.Current == MyHudRadarTypesEnum.Normal3D)
//            {
//                //  normal 3D drawing
//                m_perspectiveProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MyHudConstants.RADAR_FIELD_OF_VIEW, MyCamera.HudRadarViewport.Width / (float)MyCamera.HudRadarViewport.Height /*MyHudConstants.RADAR_ASPECT_RATIO*/, 0.1f, 1000);
//                Vector3 cameraPos = MyHudConstants.ORIGINAL_CAMERA_POSITON;
//                Matrix viewMatrix = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);
//                effect.SetViewProjectionMatrix(viewMatrix * m_perspectiveProjectionMatrix);
//            }
//            else
//            {
//                //  2d drawing
//                effect.SetViewProjectionMatrix(m_projectionMatrix2D);
//            }

//            effect.SetBillboardTexture(m_texture);
//            effect.Apply();

//            device.DrawUserPrimitives(PrimitiveType.TriangleList, m_vertices, 0, m_quadsCount * MyHudConstants.TRIANGLES_PER_HUD_QUAD);
//        }
//    }
//}