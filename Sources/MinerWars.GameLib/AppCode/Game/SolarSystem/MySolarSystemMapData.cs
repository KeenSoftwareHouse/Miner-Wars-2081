using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;
using KeenSoftwareHouse.Library.Collections;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Missions;
using SharpDX.Toolkit;

namespace MinerWars.AppCode.Game.SolarSystem
{
    abstract class MySolarSystemMapEntityBase
    {
    }

    enum MySolarSystemEntityEnum
    {
        Sun,
        Orbit,

        // Important objects
        StaticAsteroid, // Important asteroid
        VoxelAsteroid, // Important voxel asteroid
        Player,
        LargeShipIcon, // Important large ship
        OutpostIcon, // Important outpost

        LargeShip,

        DustField,
        AsteroidField,
        DebrisField,

        FactionMap,
        FactionInfo,
        AreaBorderLine,
    }

    class MySolarSystemMapEntity : MySolarSystemMapEntityBase
    {
        public MyMwcVector3Int Sector { get; private set; }

        // Position in sector [km]
        public Vector3 PositionInSector { get; private set; }

        // Entity radius [km]
        public float Radius { get; private set; }

        public string Name { get; private set; }
        public Color Color { get; private set; }

        public MySolarSystemEntityEnum EntityType { get; private set; }

        public object EntityData { get; set; }

        public MySolarSystemMapEntity(MyMwcVector3Int sector, Vector3 positionInSector, float radius, string name, MySolarSystemEntityEnum entityType, Color color)
        {
            //System.Diagnostics.Debug.Assert(Math.Abs(positionInSector.X) <= MyBgrCubeConsts.SECTOR_SIZE / 2 && Math.Abs(positionInSector.Y) <= MyBgrCubeConsts.SECTOR_SIZE / 2 && Math.Abs(positionInSector.Z) <= MyBgrCubeConsts.SECTOR_SIZE / 2, "Position is out of sector!");
            MinerWars.AppCode.Game.Utils.MyUtils.AssertIsValid(positionInSector);
            Sector = sector;
            PositionInSector = positionInSector;
            Radius = radius;
            Name = name;
            Color = color;
            EntityType = entityType;
        }

        public MySolarSystemMapEntity(MyMwcVector3Int sector, Vector3 positionInSector, float radius, string name, MySolarSystemEntityEnum entityType)
            : this(sector, positionInSector, radius, name, entityType, Color.White)
        {
        }
    }

    class MySolarSystemMapNavigationMark
    {
        public const float RADIUS = 40f;
        public const float SCALE_DISTANCE_FROM = 1000f;

        public MyMwcVector3Int Sector { get; private set; }        
        public string Name { get; private set; }        
        public Color Color { get; private set; }
        public MyMissionID? MissionID { get; private set; }
        public MyTransparentMaterialEnum Texture { get; private set; }
        public bool DrawVerticalLine { get; set; }
        public float Importance { get; set; }
        public bool Visible { get; set; }
        public Vector4 VerticalLineColor;
        public string Description;

        private bool m_isMouseOverPrevious;
        private bool m_isMouseOver;
        private Vector3 m_worldPosition;
        private float m_actualRenderedSize;
        private Vector2 m_screenPosition;
        private int m_offsetIndex;
        public HUD.MyHudTexturesEnum DirectionalTexture;
        public bool IsBlinking;
        public string Text;
        public float TextSize = 0.7f;
        public bool Highlight;
        public Vector3 Offset;
        public MyGuiFont Font;

        public MySolarSystemMapNavigationMark(MyMwcVector3Int sector, string name, MyMissionID? missionID, Color color, MyTransparentMaterialEnum texture)
        {
            Sector = sector;            
            Name = name;            
            Color = color;
            Texture = texture;
            Importance = 1;
            DrawVerticalLine = true;
            MissionID = missionID;
            Font = MyGuiManager.GetFontMinerWarsWhite();
        }

        public MySolarSystemMapNavigationMark(MyMwcVector3Int sector, string name) :
            this(sector, name, null, MyHudConstants.MISSION_MARKER_COLOR, MyTransparentMaterialEnum.SolarMapNavigationMark)
        {
        }

        private bool IsVisible(float distance)
        {
            if (Importance >= 1) return true;

            float visibilityDistance = Importance * 180;
            return visibilityDistance > distance;
        }

        public void Update(MySolarSystemMapCamera camera)
        {
            m_isMouseOverPrevious = m_isMouseOver;
            m_isMouseOver = false;

            UpdateWorldPosition(camera);

            Vector3 objToCamera = m_worldPosition;
            float dist = objToCamera.Length();
            Visible = IsVisible(dist);

            if (Vector3.Dot(camera.Forward, objToCamera) >= 0 && Visible)
            {
                UpdateRenderedSizeAndOffsetPosition();
                // we must use offset, because we need mark to be upper to other icons
                //m_worldPosition += camera.Up * m_actualRenderedSize * 4 * m_offsetIndex;                
                m_worldPosition += new Vector3(0f, 1f, 0f) * m_actualRenderedSize * 4 * m_offsetIndex;                

                m_screenPosition = GetScreenPosition(camera, m_worldPosition);
                Vector2 topLeft = GetScreenPosition(camera, m_worldPosition + camera.Left * m_actualRenderedSize + camera.Up * m_actualRenderedSize);
                Vector2 bottomRight = GetScreenPosition(camera, m_worldPosition - camera.Left * m_actualRenderedSize - camera.Up * m_actualRenderedSize);


                m_isMouseOver = MyGuiManager.MouseCursorPosition.X >= topLeft.X && MyGuiManager.MouseCursorPosition.X <= bottomRight.X &&
                                MyGuiManager.MouseCursorPosition.Y >= topLeft.Y && MyGuiManager.MouseCursorPosition.Y <= bottomRight.Y;
            }

            if (m_isMouseOverPrevious != m_isMouseOver)
            {
                MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseOver);
            }
        }

        public bool IsMouseOver()
        {
            return m_isMouseOver;
        }

        public Vector3 WorldPosition
        {
            get { return m_worldPosition; } 
        }

        public Vector2 ScreenPosition
        {
            get { return m_screenPosition; } 
        }

        public float RenderedSize
        {
            get { return m_actualRenderedSize; }
        }

        public void UpdateOffsetIndex(int offsetIndex)
        {
            m_offsetIndex = offsetIndex;
        }

        private void UpdateWorldPosition(MySolarSystemMapCamera camera)
        {
            MyMwcVector3Int sectorOffset;
            sectorOffset.X = Sector.X - camera.TargetSector.X;
            sectorOffset.Y = Sector.Y - camera.TargetSector.Y;
            sectorOffset.Z = Sector.Z - camera.TargetSector.Z;

            m_worldPosition = MySolarMapRenderer.MillionKmToGameUnits(MySolarSystemUtils.SectorsToMillionKm(sectorOffset)) +
                /*new Vector3(0f, RADIUS * m_offsetIndex, 0f) + */camera.CameraToTarget - camera.Target;
        }

        private Vector2 GetScreenPosition(MySolarSystemMapCamera camera, Vector3 worldPosition)
        {
            Vector3 target = SharpDXHelper.ToXNA(MyCamera.Viewport.Project(SharpDXHelper.ToSharpDX(worldPosition), SharpDXHelper.ToSharpDX(camera.GetProjectionMatrix()), SharpDXHelper.ToSharpDX(camera.GetViewMatrixAtZero()), SharpDXHelper.ToSharpDX(Matrix.Identity)));
            Vector2 projected2Dpoint = new Vector2(target.X, target.Y);
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(projected2Dpoint);
        }

        private void UpdateRenderedSizeAndOffsetPosition()
        {
            m_actualRenderedSize = MySolarSystemUtils.CalculateDistanceUnscalingTo(
                m_worldPosition, MySolarSystemMapNavigationMark.RADIUS, MySolarSystemMapNavigationMark.SCALE_DISTANCE_FROM);            
        }
    }

    class MySolarSystemMapNavigationMarks : IEnumerable<KeyValuePair<MyMwcVector3Int, List<MySolarSystemMapNavigationMark>>>
    {
        private Dictionary<MyMwcVector3Int, List<MySolarSystemMapNavigationMark>> m_navigationMarks;

        public MySolarSystemMapNavigationMarks()
        {
            m_navigationMarks = new Dictionary<MyMwcVector3Int, List<MySolarSystemMapNavigationMark>>();            
        }

        public void Add(MySolarSystemMapNavigationMark navigationMark)
        {
            if (!m_navigationMarks.ContainsKey(navigationMark.Sector))
            {
                m_navigationMarks[navigationMark.Sector] = new List<MySolarSystemMapNavigationMark>();
            }
            m_navigationMarks[navigationMark.Sector].Add(navigationMark);
            UpdateOffsetsInSector(navigationMark.Sector);

            Sort();
        }

        private void Sort()
        {
            m_navigationMarks = m_navigationMarks.OrderBy(n => n.Value.Max(i => i.Importance)).ToDictionary(k => k.Key, e => e.Value);
        }

        public void Remove(MySolarSystemMapNavigationMark navigationMark)
        {
            if (m_navigationMarks.ContainsKey(navigationMark.Sector))
            {
                m_navigationMarks[navigationMark.Sector].Remove(navigationMark);
                UpdateOffsetsInSector(navigationMark.Sector);
            }            
        }

        private void UpdateOffsetsInSector(MyMwcVector3Int sector)
        {
            for (int i = 0; i < m_navigationMarks[sector].Count; i++)
            {
                m_navigationMarks[sector][i].UpdateOffsetIndex(i);
            }
        }
        public MySolarSystemMapNavigationMark GetMostImportant()
        {
            return m_navigationMarks.Values.SelectMany(n => n).OrderByDescending(n => n.Importance).FirstOrDefault();
        }
        public IEnumerator<KeyValuePair<MyMwcVector3Int, List<MySolarSystemMapNavigationMark>>> GetEnumerator()
        {
            return m_navigationMarks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(MyMwcVector3Int sector)
        {
            return m_navigationMarks.ContainsKey(sector);
        }
    }

    class MySolarSystemMapSectorData
    {
        public MyMwcVector3Int SectorPosition;

        // Areas define occurence of entities
        // For now, sector can have only one area
        public MySolarSystemAreaEnum? Area;

        // Should be 1 in the center of area. On area border should be close to 0
        public float AreaInfluenceMultiplier = 0;

        // Generated entities (no need to be there all - for solar map, there could be only very large asteroids)
        public List<MySolarSystemMapEntity> Entities;

        /// <summary>
        /// Minimal entity size, smaller entities weren't generated.
        /// Sector may not contain all entities (especially small onces).
        /// </summary>
        public float MinimalEntitySize { get; private set; }

        public MySolarSystemMapSectorData(MyMwcVector3Int sectorPosition, float fromEntitySize)
        {
            SectorPosition = sectorPosition;
            Entities = new List<MySolarSystemMapEntity>();
            //AreaData = new MySolarSystemAreaData();
            MinimalEntitySize = fromEntitySize;
        }
    }

    class MyImportantSolarObject
    {
        public MyTemplateGroupEnum TemplateGroup;
        public MySolarSystemMapNavigationMark NavigationMark;
    }

    class MySolarSystemMapData
    {
        public List<MySolarSystemMapEntity> Entities { get; private set; }
        public List<MySolarSystemAreaEnum> Areas { get; private set; }
        public MySolarSystemMapNavigationMarks NavigationMarks { get; private set; }

        public List<MyImportantSolarObject> ImportantObjects { get; private set; }

        public LRUCache<MyMwcVector3Int, MySolarSystemMapSectorData> SectorData { get; private set; }

        public List<MySolarAreaBorderLine> AreasBorderLines { get; private set; }

        public MySolarSystemMapData(int sectorCacheCapacity)
        {
            Entities = new List<MySolarSystemMapEntity>();
            Areas = new List<MySolarSystemAreaEnum>();
            SectorData = new LRUCache<MyMwcVector3Int, MySolarSystemMapSectorData>(sectorCacheCapacity);
            NavigationMarks = new MySolarSystemMapNavigationMarks();
            ImportantObjects = new List<MyImportantSolarObject>();
            AreasBorderLines = new List<MySolarAreaBorderLine>();
        }
    }

    struct MySolarAreaBorderLine
    {
        public Vector3 AreaCenter;
        public float DistanceHigh;
        public float DistanceLow;
        public float Spread;
        public Vector4 col;
    }
}
