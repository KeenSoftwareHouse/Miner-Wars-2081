using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.TransparentGeometry;
using ParallelTasks;
using System.Diagnostics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Audio;

using MinerWars.AppCode.Physics.Collisions;

namespace MinerWars.AppCode.Game.Prefabs
{
    delegate void OnEntityScanned(MyPrefabScanner sender, MyEntity scannedEntity);

    class MyPrefabScanner : MyPrefabBase, IMyUseableEntity, IMyHasGuiControl
    {
        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            m_offsetCurrent = 0f;
            m_scannerForward = true;
            m_scanningPart.SetOn(IsWorking());

            if (!IsWorking()) 
            {
                StopSound();
            }
        }

        public Color Color { get; set; }

        private Vector3 _size;
        public Vector3 Size
        {
            get
            {
                return _size;           
            }
            set
            {
                _size = value;
                var size = new Vector3(Math.Max(1, _size.X), Math.Max(1, _size.Y), Math.Max(1, _size.Z));
                m_localAABB = new BoundingBox((_size*-2), _size*2);
            }
        }

        public float ScanningSpeed { get; set; }

        private MyPrefabScanningPartBase m_scanningPart;
        private List<MyEntity> m_scannedEntities;
        
        private float m_offsetCurrent = 0f;         // in m
        private bool m_scannerForward = true;       // scanner part move direction

        private MySoundCue? m_scannerSound;

        public MyPrefabScanner(MyPrefabContainer owner) 
            : base(owner)
        {            
            m_scannedEntities = new List<MyEntity>();
            OnEntityScanned += m_owner.LaunchAlarm;
        }

        public event OnEntityScanned OnEntityScanned;

        public void NotifyEntityScanned(MyEntity scannedEntity)
        {
            if (!m_scannedEntities.Contains(scannedEntity)) 
            {
                m_scannedEntities.Add(scannedEntity);
                if (OnEntityScanned != null)                 
                {
                    OnEntityScanned(this, scannedEntity);
                }
            }
        }

        public static bool CanBeScannedCriterium(MyEntity entity, params object[] args)
        {
            MyPrefabScanningPartBase scanningPart = args[0] as MyPrefabScanningPartBase;
            return entity is MySmallShip && MyFactions.GetFactionsRelation(scanningPart.GetScanner(), entity) == MyFactionRelationEnum.Enemy;
        }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabScanner objectBuilderScanner = objectBuilder as MyMwcObjectBuilder_PrefabScanner;

            UseProperties = new MyUseProperties(MyUseType.FromHUB, MyUseType.FromHUB | MyUseType.Solo);
            if (objectBuilderScanner.UseProperties == null)
            {
                UseProperties.Init(MyUseType.FromHUB | MyUseType.Solo, MyUseType.FromHUB | MyUseType.Solo, 3, 4000, false);
            }
            else
            {                
                UseProperties.Init(objectBuilderScanner.UseProperties);
            }

            Size = objectBuilderScanner.Size;
            Color = objectBuilderScanner.Color;
            ScanningSpeed = objectBuilderScanner.ScanningSpeed;
            InitScanningPart();

            Flags |= EntityFlags.EditableInEditor;            
        }

        public void InitScanningPart() 
        {
            if (PrefabScannerType == MyMwcObjectBuilder_PrefabScanner_TypesEnum.Plane)
            {
                m_scanningPart = new MyPrefabScanningPartPlane(this);
            }
            else if (PrefabScannerType == MyMwcObjectBuilder_PrefabScanner_TypesEnum.Rays)
            {
                m_scanningPart = new MyPrefabScanningPartRays(this);
            }
            m_scanningPart.Init();
        }

        public void ReinitScanningPart() 
        {
            //m_scanningPart.Close();
            //m_scanningPart.Init();
            m_scanningPart.Reinit();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabScanner objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabScanner;
            objectBuilder.Color = Color;
            objectBuilder.Size = Size;
            objectBuilder.ScanningSpeed = ScanningSpeed;            
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();

            return objectBuilder;
        }

        protected override void UpdatePrefabBeforeSimulation()
        {
            base.UpdatePrefabBeforeSimulation();            
            UpdateScanningPart();

            if (m_scanningPart.IntersectsWithControlledEntity())
            {
                PlaySound();
            }
            else 
            {
                StopSound();
            }
        }        

        private void PlaySound() 
        {
            if (m_scannerSound == null || !m_scannerSound.Value.IsPlaying) 
            {
                m_scannerSound = MyAudio.AddCue2D(MySoundCuesEnum.SfxScanner);
            }
        }

        private void StopSound() 
        {
            if (m_scannerSound != null && m_scannerSound.Value.IsPlaying) 
            {
                m_scannerSound.Value.Stop(SharpDX.XACT3.StopFlags.Release);                
            }
            m_scannerSound = null;
        }

        private void UpdateScanningPart() 
        {
            m_offsetCurrent += (m_scannerForward ? 1 : -1) * ScanningSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            if (m_offsetCurrent > Size.Z)
            {
                m_offsetCurrent = Size.Z;
                m_scannerForward = false;
            }
            if (m_offsetCurrent < 0f)
            {
                m_offsetCurrent = 0f;
                m_scannerForward = true;
            }
            m_scanningPart.SetOffset(m_offsetCurrent);            

            m_scanningPart.Update();
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            var s = base.WorldAABB.Size();
            if (!MyGuiScreenGamePlay.Static.IsGameActive())
            {
                Matrix world = Matrix.CreateWorld(WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up);
                Vector4 color = Color.Green.ToVector4();
                color.W *= 0.3f;
                BoundingBox localBoundingBox = new BoundingBox(-Size / 2f, Size / 2f);
                MySimpleObjectDraw.DrawTransparentBox(ref world, ref localBoundingBox, ref color, true, 1);
            }
            //base.Draw();

            if (IsWorking())
            {
                m_scanningPart.Draw();
            }
            return true;
        }                                    

        public MyMwcObjectBuilder_PrefabScanner_TypesEnum PrefabScannerType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabScanner_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabScanner";
        }

        public override void Close()
        {
            OnEntityScanned -= m_owner.LaunchAlarm;
            base.Close();
            if (m_scanningPart != null)
            {
                m_scanningPart.Close();
                m_scanningPart = null;
            }
            m_scannedEntities.Clear();
            m_scannedEntities = null;
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            v = null;
            Ray ray = new Ray(line.From, line.Direction);
            float? ds = null;
            BoundingBox boundingBox = new BoundingBox(WorldMatrix.Translation - Size / 2f, WorldMatrix.Translation + Size / 2f);
            ds = ray.Intersects(boundingBox);            

            if (ds == null)
                return false;

            v = line.From + line.Direction * ds;
            return true;
        }

        public override bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            BoundingBox boundingBox = new BoundingBox(WorldMatrix.Translation - Size / 2f, WorldMatrix.Translation + Size / 2f);
            return boundingBox.Intersects(sphere);
        }

        #region IMyUseablePrefab
        public MyGuiControlEntityUse GetGuiControl(IMyGuiControlsParent parent)
        {
            return new MyGuiControlPrefabUse(parent, this);
        }

        public MyEntity GetEntity()
        {
            return this;
        }

        public void Use(MySmallShip useBy)
        {
            MyGuiManager.AddScreen(new MyGuiScreenEntityUseSolo(this));
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
            Use(useBy);
        }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return IsWorking() && (MyFactions.GetFactionsRelation(usedBy, this) == MyFactionRelationEnum.Friend || UseProperties.IsHacked);
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return IsWorking() && (MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Neutral ||
                MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Enemy);
        }

        public MyUseProperties UseProperties
        {
            get;
            set;
        }
        #endregion
    }
    
    abstract class MyPrefabScanningPartBase
    {
        protected MyPrefabScanner m_scanner;        

        protected float m_offset;
        private Vector3 m_size;
        protected Vector3 Size
        {
            get
            {
                return m_size;
            }
            set 
            {
                m_size = value;                

                Vector3 extents = new Vector3(m_size.X / 2f, m_size.Y / 2f, Math.Min(m_size.X, m_size.Y) / 2f);            
                m_bbForSoundDetection = new BoundingBox(-extents, extents);
            }
        }
        protected bool m_on;

        private BoundingBox m_bbForSoundDetection;

        public MyPrefabScanningPartBase(MyPrefabScanner scanner)
        {
            m_scanner = scanner;
        }

        public Matrix WorldMatrix 
        {
            get 
            {
                Vector3 localOffset = Vector3.Backward * (-m_scanner.Size.Z / 2f + m_offset);
                Vector3 position = Vector3.Transform(localOffset, m_scanner.WorldMatrix);
                return Matrix.CreateWorld(position, m_scanner.WorldMatrix.Forward, m_scanner.WorldMatrix.Up);
            }
        }

        protected virtual float ColorMultiplicator
        {
            get { return 0.1f; }
        }

        protected Vector4 Color 
        {
            get 
            {
                Color color = m_scanner.Color;
                Vector4 vctColor = color.ToVector4() * ColorMultiplicator;
                return vctColor;
            }
        }

        public abstract void Draw();

        public abstract void Update();

        public virtual void Init() 
        {
            Debug.Assert(!m_on);
            Size = new Vector3(m_scanner.Size.X, m_scanner.Size.Y, 0.1f);
        }

        public virtual void Reinit() 
        {
            Debug.Assert(!m_on);
            Size = new Vector3(m_scanner.Size.X, m_scanner.Size.Y, 0.1f);
        }

        public virtual void Close() 
        {

        }

        public virtual void SetOffset(float offset)
        {
            m_offset = offset;
        }

        public virtual void SetOn(bool value) 
        {
            m_on = value;
        }

        public MyPrefabScanner GetScanner() 
        {
            return m_scanner;
        }

        public bool IntersectsWithControlledEntity()
        {
            Matrix worldMatrix = WorldMatrix;
            MyEntity controlledEntity = MyGuiScreenGamePlay.Static.ControlledEntity;
            if (Vector3.DistanceSquared(worldMatrix.Translation, controlledEntity.GetPosition()) > m_bbForSoundDetection.Size().LengthSquared()) 
            {
                return false;
            }
            Matrix invMatrix = Matrix.Invert(WorldMatrix);
            Vector3 localPlayerBSCenter = Vector3.Transform(MyGuiScreenGamePlay.Static.ControlledEntity.WorldVolume.Center, invMatrix);
            BoundingSphere localPlayerBS = new BoundingSphere(localPlayerBSCenter, MyGuiScreenGamePlay.Static.ControlledEntity.WorldVolume.Radius);
                     
            return m_bbForSoundDetection.Intersects(localPlayerBS);
        }
    }

    class MyPrefabScanningPartPlane : MyPrefabScanningPartBase
    {
        private MyEntityDetector m_entityDetector;
        private List<IMyEntityDetectorCriterium> m_scannerCriterias = new List<IMyEntityDetectorCriterium>();

        public MyPrefabScanningPartPlane(MyPrefabScanner scanner)
            : base(scanner)
        {            
            m_scannerCriterias.Add(new MyEntityDetectorCriterium<MyEntity>(1, MyPrefabScanner.CanBeScannedCriterium, true, this));
            m_entityDetector = new MyEntityDetector();            
            m_entityDetector.OnEntityEnter += OnEntityEnter;            
            m_scanner.OnPositionChanged += OnScannerPositionChanged;
        }

        public override void Init()
        {
            base.Init();
            MyMwcObjectBuilder_EntityDetector detectorBuilder = new MyMwcObjectBuilder_EntityDetector(Size, MyMwcObjectBuilder_EntityDetector_TypesEnum.Box);
            m_entityDetector.Init(null, detectorBuilder, null, WorldMatrix, m_scannerCriterias);
        }

        private void OnScannerPositionChanged(object sender, EventArgs e)
        {
            UpdateEntityDetectorPosition();
        }

        private void OnEntityEnter(MyEntityDetector sender, MyEntity entity, int meetCriterias)
        {
            m_scanner.NotifyEntityScanned(entity);            
        }

        public override void SetOffset(float offset)
        {
            base.SetOffset(offset);
            UpdateEntityDetectorPosition();
        }

        private void UpdateEntityDetectorPosition() 
        {
            if (m_entityDetector != null) 
            {
                m_entityDetector.SetWorldMatrix(WorldMatrix);
            }            
        }

        private float uvOffset = 0f;
        public override void Draw()
        {
            if (m_on)
            {
                Matrix world = WorldMatrix;
                MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.scanner_01, Color, world.Translation, world.Left, world.Up, Size.X / 2f, Size.Y / 2f, new Vector2(uvOffset, uvOffset), 0, true);                
            }
        }

        public override void SetOn(bool value)
        {
            base.SetOn(value);
            if (m_on && !m_entityDetector.IsOn())
            {
                m_entityDetector.On();
            }
            else if (!m_on && m_entityDetector.IsOn())
            {
                m_entityDetector.Off();
            }
        }

        public override void Update() 
        {
            uvOffset += 0.01f;       
        }

        public override void Close()
        {
            base.Close();
            if (m_entityDetector != null) 
            {
                m_entityDetector.Close();
                m_entityDetector = null;
            }
        }

        public override void Reinit()
        {
            base.Reinit();
            m_entityDetector.Size = Size;
        }
    }

    class MyPrefabScanningPartRays : MyPrefabScanningPartBase
    {
        class BackgroundScan : IWork
        {
            private MyEntity m_scannedEntity;
            private MyLine m_line;
            private MyPrefabScanner m_owner;
            private static readonly Type[] m_typesToScan = new Type[] { typeof(MySmallShip) };

            public BackgroundScan(MyPrefabScanner owner)
            {
                m_owner = owner;
            }

            public MyEntity GetScannedEntity()
            {
                return m_scannedEntity;
            }

            public void Init(MyLine line)
            {
                m_scannedEntity = null;
                m_line = line;
            }

            public void DoWork()
            {
                m_scannedEntity = null;
                var result = MyEntities.GetIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref m_line, m_typesToScan, true);
                if (result != null)
                {
                    m_scannedEntity = result.Value.Entity;
                }
                else 
                {
                    m_scannedEntity = null;
                }
                //m_scannedEntity = MyEntities.GetAllIntersectionWithLine(ref m_line, m_owner, null, true, true);
            }

            public WorkOptions Options
            {
                get { return new WorkOptions() { MaximumThreads = 1 }; }
            }
        }
        
        private const float SPACE_BETWEEN_RAYS = 2.8f;      // in meters
        private const float RAY_THICKNESS = 0.1f;           // in meters

        private BackgroundScan m_work;
        private Task m_task;
        private bool m_running = false;
        private List<Vector3> m_raysStartPositions;
        private int m_currentIndex = 0;

        public MyPrefabScanningPartRays(MyPrefabScanner scanner)
            : base(scanner)
        {
            m_work = new BackgroundScan(scanner);
            m_raysStartPositions = new List<Vector3>();
        }

        public override void Init()
        {
            base.Init();
            InitRays();
        }

        public override void Reinit()
        {
            base.Reinit();
            InitRays();
        }

        public override void Close()
        {
            base.Close();
            m_raysStartPositions.Clear();
            m_raysStartPositions = null;
            m_work = null;
        }

        private void InitRays() 
        {
            m_raysStartPositions.Clear();
            int raysCount = GetRaysCount();
            Debug.Assert(raysCount >= 0);
            float raysHeight = raysCount * RAY_THICKNESS + Math.Max(0, raysCount - 2) * SPACE_BETWEEN_RAYS;
            float rayY = (RAY_THICKNESS - raysHeight) / 2f;
            for (int i = 0; i < raysCount; i++)
            {
                Vector3 rayPosition = new Vector3(-Size.X / 2f, rayY + i * SPACE_BETWEEN_RAYS, 0f);
                m_raysStartPositions.Add(rayPosition);
            }            
        }

        private int GetRaysCount() 
        {
            int raysCount = 0;
            float heightLeft = Size.Y;

            while (heightLeft >= RAY_THICKNESS) 
            {
                raysCount++;
                heightLeft -= RAY_THICKNESS;
                heightLeft -= SPACE_BETWEEN_RAYS;
            }

            return raysCount;
        }

        protected override float ColorMultiplicator
        {
            get
            {
                return 5f;
            }
        }
        
        public override void SetOffset(float offset)
        {
            base.SetOffset(offset);            
        }        

        public override void Draw()
        {
            if (m_on)
            {
                Matrix world = WorldMatrix;
                for (int i = 0; i < m_raysStartPositions.Count; i++)
                {
                    Vector3 rayPosition = m_raysStartPositions[i];
                    Vector3 rayStart = Vector3.Transform(rayPosition, world);                    
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.LightRay, Color, rayStart, world.Right, Size.X, RAY_THICKNESS);
                }
            }
        }

        public override void Update()
        {
            if (m_on)
            {
                if (!m_running)
                {
                    if (m_currentIndex >= m_raysStartPositions.Count)
                    {
                        m_currentIndex = 0;
                    }
                    Matrix world = WorldMatrix;
                    Vector3 rayPosition = m_raysStartPositions[m_currentIndex];
                    Vector3 rayStart = Vector3.Transform(rayPosition, world);
                    Vector3 rayEnd = rayStart + world.Right * Size.X;
                    MyLine rayLine = new MyLine(rayStart, rayEnd, true);
                    m_work.Init(rayLine);
                    m_task = Parallel.Start(m_work);
                    m_running = true;
                    m_currentIndex++;
                }

                if (m_running && m_task.IsComplete)
                {
                    MyEntity scannedEntity = m_work.GetScannedEntity();
                    if (scannedEntity != null && MyPrefabScanner.CanBeScannedCriterium(scannedEntity, this))
                    {
                        m_scanner.NotifyEntityScanned(scannedEntity);
                    }
                    m_running = false;
                }
            }
        }
    }    
}
