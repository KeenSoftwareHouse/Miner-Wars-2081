using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWarsMath;
using MinerWars.AppCode.Game.Prefabs;
using System.Text;
using MinerWars.AppCode.Game.Models;
using System;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.Collections.Generic;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;

using MinerWars.AppCode.Game.Physics;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Voxels;

using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.HUD;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.WayPoints;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.CommonLIB.AppCode.Import;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    public enum KinematicPrefabStateEnum
    {
        Closing,
        Opening,
        Sleeping,
    };


    class MyPrefabKinematic : MyPrefabBase, IMyUseableEntity, IMyHasGuiControl
    {
        private const float DETECT_RADIUS = 200f;
        private static readonly float? DETECTION_ANGLE = 0.6f;

        private MyPrefabKinematicPart[] m_parts;

        MySensor m_sensor;
        MyPrefabKinematicSensor m_sensorHandler;
        long m_transformationTimeInMillis; // how long should animation last, thus movement will be interpolated based on this
        MySoundCue? m_loopSound;
        MySoundCue? m_startSound;
        MySoundCue? m_endSound;

        MyGroupMask m_groupMask;

        private List<Tuple<MyWayPoint, MyWayPoint>> m_edges;

        public override bool Enabled
        {
            set
            {
                base.Enabled = value;
                UpdateHudAndCloseStatus();
            }
        }

        private void UpdateHudAndCloseStatus() 
        {
            if (Enabled || GetPrefabPartsCount() == 0)
            {
                RemoveLockedHud();
            }
            // when we disabled doors (lock) we want close them and display health bar
            else
            {
                AddLockedHud();
                OrderToClose();
            }
        }

        private void LoadWaypointEdges() 
        {
            m_edges.Clear();
            
            BoundingBox bb = BoundingBoxHelper.InitialBox;
            foreach(var part in m_parts)
            {
                if(part != null)
                {
                    BoundingBoxHelper.AddBBox(part.WorldAABB, ref bb);
                }
            }

            m_edges.AddRange(MyWayPointGraph.GetAllEdgesInBox(ref bb));
        }

        private void UpdateBlockedEdges() 
        {
            if (IsWorking())
            {
                foreach (var edge in m_edges)
                {
                    MyWayPoint.RemoveBlockedEdgesForBots(edge);
                    MyWayPoint.RemoveBlockedEdgesForPlayer(edge);
                }
            }
            else
            {
                foreach (var edge in m_edges)
                {
                    MyWayPoint.AddBlockedEdgesForBots(edge);

                    if (!IsDestructible)
                    {
                        bool isDestructible = false;
                        foreach (var par in m_parts)
                        {
                            if (par != null && par.IsDestructible)
                            {
                                isDestructible = true;
                                break;
                            }
                        }

                        if (!isDestructible)
                        {
                            MyWayPoint.AddBlockedEdgesForPlayer(edge);
                        }
                    }
                }
            }
        }

        public override string GetCorrectDisplayName()
        {
            string displayName = base.GetCorrectDisplayName();

            if (displayName == "Left Door")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.LeftDoor).ToString();
            }

            if (displayName == "Right Door")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.RightDoor).ToString();
            }

            return displayName;
        }

        private void AddLockedHud() 
        {
            DisplayOnHud = true;
            MyHud.ChangeText(this, MyTextsWrapper.Get(MyTextsWrapperEnum.DoorsLocked), MyGuitargetMode.Enemy, 200f, MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER);
        }

        private void RemoveLockedHud() 
        {
            MyHud.RemoveText(this);
        }

        protected override void SetHudMarker()
        {
            UpdateHudAndCloseStatus();
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
            if (damage > 0)
            {
                OrderOpenAndClose();
            }
        }

        public override float GetHUDDamageRatio()
        {                        
            float healtRatioTotal = 0f;            
            foreach (var kinematicPart in Parts) 
            {
                if (kinematicPart != null)
                {
                    healtRatioTotal += kinematicPart.HealthRatio;                    
                }
            }
            return 1f - (healtRatioTotal / ((MyPrefabConfigurationKinematic)m_config).KinematicParts.Count);
        }

        //public override void  UpdateBeforeSimulation()
        //{
        //    base.UpdateBeforeSimulation();
        //    if (IsWorking()) 
        //    {
        //        if (m_sensorHandler.GetDetectedEntitiesCount() > 0)
        //        {
        //            OrderToOpen();
        //        }
        //        else 
        //        {
        //            OrderToClose();
        //        }                
        //    }
        //    UpdateBlockedEdges();
        //}
        
        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            if (IsWorking()) 
            {
                if (m_sensorHandler.GetDetectedEntitiesCount() > 0)
                {
                    OrderToOpen();
                }
                else 
                {
                    OrderToClose();
                }                
            }
            UpdateBlockedEdges();
        }

        //private bool m_canOpen = true;
        //public bool CanOpen 
        //{
        //    get { return m_canOpen; }
        //    set
        //    {
        //        m_canOpen = value;
        //        if (value && m_sensorHandler.GetDetectedEntitiesCount() > 0)
        //        {
        //            OrderToOpen();
        //        }
        //    }
        //}

        private EventHandler m_onGameLoaded;        

        public MyPrefabKinematic(MyPrefabContainer owner)
            : base(owner)
        {
            m_onGameLoaded = new EventHandler(MyGuiScreenGamePlay_OnGameLoaded);
            MyGuiScreenGamePlay.OnGameLoaded += m_onGameLoaded;
            m_parts = new MyPrefabKinematicPart[MyMwcObjectBuilder_PrefabKinematic.MAX_KINEMATIC_PARTS];
            m_edges = new List<Tuple<MyWayPoint, MyWayPoint>>();
        }

        void MyGuiScreenGamePlay_OnGameLoaded(object sender, EventArgs e)
        {            
            MyGuiScreenGamePlay.OnGameLoaded -= m_onGameLoaded;            
            LoadWaypointEdges();
            UpdateBlockedEdges();
        }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyPrefabConfigurationKinematic prefabKinematicConfig = (MyPrefabConfigurationKinematic)prefabConfig;
            MyMwcObjectBuilder_PrefabKinematic kinematicBuilder = objectBuilder as MyMwcObjectBuilder_PrefabKinematic;                        

            MyModel model = MyModels.GetModelOnlyDummies(m_config.ModelLod0Enum);

            for (int i = 0; i < prefabKinematicConfig.KinematicParts.Count; i++)
            {
                MyPrefabConfigurationKinematicPart kinematicPart = prefabKinematicConfig.KinematicParts[i];
                MyModelDummy open, close;
                if (model.Dummies.TryGetValue(kinematicPart.m_open, out open) && model.Dummies.TryGetValue(kinematicPart.m_close, out close))
                {
                    float? kinematicPartHealth = kinematicBuilder.KinematicPartsHealth[i];
                    float? kinematicPartMaxHealth = kinematicBuilder.KinematicPartsMaxHealth[i];
                    uint? kinematicPartEntityId = kinematicBuilder.KinematicPartsEntityId[i];

                    // if health is not set or not destroyed, then create part
                    if (kinematicPartHealth == null || kinematicPartHealth != 0)
                    {
                        MyPrefabKinematicPart newPart = new MyPrefabKinematicPart(m_owner);
                        if (kinematicPartEntityId.HasValue)
                        {
                            newPart.EntityId = new MyEntityIdentifier(kinematicPartEntityId.Value);
                        }
                        Parts[i] = newPart;
                        newPart.Init(this, kinematicPart, prefabKinematicConfig.m_openTime, prefabKinematicConfig.m_closeTime, (MyModelsEnum)kinematicPart.m_modelMovingEnum, open.Matrix, close.Matrix, prefabKinematicConfig.MaterialType, prefabKinematicConfig.m_soundLooping, prefabKinematicConfig.m_soundOpening, prefabKinematicConfig.m_soundClosing/*, m_groupMask*/, kinematicPartHealth, kinematicPartMaxHealth, Activated);
                    }
                }
            }                        

            //make handler
            m_sensorHandler = new MyPrefabKinematicSensor(this);
            MySphereSensorElement sensorEl = new MySphereSensorElement();
            sensorEl.Radius = DETECT_RADIUS;
            sensorEl.LocalPosition = new Vector3(0, 0, 0);
            sensorEl.DetectRigidBodyTypes = MyConstants.RIGIDBODY_TYPE_SHIP;            
            sensorEl.SpecialDetectingAngle = DETECTION_ANGLE;
            MySensorDesc senDesc = new MySensorDesc();
            senDesc.m_Element = sensorEl;
            senDesc.m_Matrix = WorldMatrix;
            senDesc.m_SensorEventHandler = m_sensorHandler;
            m_sensor = new MySensor();
            m_sensor.LoadFromDesc(senDesc);
            MyPhysics.physicsSystem.GetSensorModule().AddSensor(m_sensor);

            GetOwner().UpdateAABB();

            UseProperties = new MyUseProperties(MyUseType.FromHUB | MyUseType.Solo, MyUseType.FromHUB);
            if (kinematicBuilder.UseProperties == null)
            {
                UseProperties.Init(MyUseType.FromHUB, MyUseType.FromHUB, 3, 4000, false);
            }
            else
            {
                UseProperties.Init(kinematicBuilder.UseProperties);
            }
            UpdateHudAndCloseStatus();
        }

        public override void OnWorldPositionChanged(object source)
        {
            if (m_sensor != null)
            {
                m_sensor.Matrix = base.WorldMatrix;
            }
            base.OnWorldPositionChanged(source);
        }

        public void OrderToOpen()
        {
            if (IsWorking())
            {
                foreach (MyPrefabKinematicPart part in Parts)
                {
                    if (part != null)
                    {
                        part.StartOpening();
                    }
                }
            }
        }

        public void OrderToClose()
        {
            if (IsWorking() || !Enabled)
            {
                foreach (MyPrefabKinematicPart part in Parts)
                {
                    if (part != null)
                    {
                        part.StartClosing();
                    }
                }
            }
        }

        public void OrderOpenAndClose() 
        {
            OrderToOpen();
            if (m_sensorHandler.GetDetectedEntitiesCount() <= 0)
            {
                OrderToClose();
            }
        }

        public override void Close()
        {
            MyGuiScreenGamePlay.OnGameLoaded -= m_onGameLoaded;
            if (m_sensor != null) 
            {
                //m_sensor.GetElement().ProxyData = MyElement.PROXY_UNASSIGNED;
                m_sensor.MarkForClose();
                MyPhysics.physicsSystem.GetSensorModule().RemoveSensor(m_sensor);
                m_sensor = null;
            }

            foreach (var part in Parts)
            {
                if (part != null && part.EntityId.HasValue)
                {
                    MyEntities.Remove(part);
                }
            }

            base.Close();
        }


        public void LoadContent()
        {

        }

        public void UnloadContent()
        {

        }                      

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabKinematic objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabKinematic;            
            MyPrefabConfigurationKinematic kinematicConfig = GetConfiguration() as MyPrefabConfigurationKinematic;

            for (int i = 0; i < kinematicConfig.KinematicParts.Count; i++) 
            {
                // part is destroyed
                if (Parts[i] == null)
                {
                    objectBuilder.KinematicPartsHealth[i] = 0f;
                    objectBuilder.KinematicPartsMaxHealth[i] = null;
                    objectBuilder.KinematicPartsEntityId[i] = null; // Won't transfer
                }
                else 
                {
                    objectBuilder.KinematicPartsHealth[i] = Parts[i].Health;
                    objectBuilder.KinematicPartsMaxHealth[i] = Parts[i].GetMaxHealth();
                    objectBuilder.KinematicPartsEntityId[i] = MyEntityIdentifier.ToNullableInt(Parts[i].EntityId);
                }
            }
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();

            return objectBuilder;
        }        

        public void RemovePart(MyPrefabKinematicPart part)
        {
            int partsCount = 0;
            for (int i = 0; i < MyMwcObjectBuilder_PrefabKinematic.MAX_KINEMATIC_PARTS; i++) 
            {
                if (Parts[i] == part) 
                {
                    Parts[i] = null;
                }
                if (Parts[i] != null) 
                {
                    partsCount++;
                }
            }
            if (partsCount == 0) 
            {
                RemoveLockedHud();
            }
        }

        private int GetPrefabPartsCount() 
        {
            int partsCount = 0;
            for (int i = 0; i < MyMwcObjectBuilder_PrefabKinematic.MAX_KINEMATIC_PARTS; i++)
            {                
                if (Parts[i] != null)
                {
                    partsCount++;
                }
            }
            return partsCount;
        }

        public MyMwcObjectBuilder_PrefabKinematic_TypesEnum PrefabKinematicType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabKinematic_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabKinematic";
        }

        public static bool DRAW_DEBUG_INFORMATION = false;

        private StringBuilder m_debugDrawSb = new StringBuilder();
        public override bool DebugDraw()
        {
            if (!base.DebugDraw()) 
            {
                return false;
            }

            if (DRAW_DEBUG_INFORMATION)
            {
                m_debugDrawSb.Clear();

                m_debugDrawSb.Append("Is working:");
                m_debugDrawSb.Append(IsWorking());
                m_debugDrawSb.AppendLine();

                m_debugDrawSb.Append("Enabled:");
                m_debugDrawSb.Append(Enabled);
                m_debugDrawSb.AppendLine();

                m_debugDrawSb.Append("Detected entities:");
                m_debugDrawSb.AppendInt32(m_sensorHandler.GetDetectedEntitiesCount());
                m_debugDrawSb.AppendLine();

                m_debugDrawSb.Append("Sensor interactions:");
                m_debugDrawSb.AppendInt32(m_sensor.m_Interactions.Count);
                m_debugDrawSb.AppendLine();

                KinematicPrefabStateEnum? state = null;
                bool? isOpened = null;
                bool? isClosed = null;
                foreach (var part in Parts)
                {
                    if (part != null)
                    {
                        Debug.Assert(state == null || state.Value == part.GetState());
                        Debug.Assert(isOpened == null || isOpened.Value == part.IsOpened());
                        Debug.Assert(isClosed == null || isClosed.Value == part.IsClosed());
                        state = part.GetState();
                        isOpened = part.IsOpened();
                        isClosed = part.IsClosed();
                    }
                }
                if (state != null && isOpened != null && isClosed != null)
                {
                    m_debugDrawSb.Append("State:");
                    switch (state.Value)
                    {
                        case KinematicPrefabStateEnum.Closing:
                            m_debugDrawSb.Append("Closing");
                            break;
                        case KinematicPrefabStateEnum.Opening:
                            m_debugDrawSb.Append("Opening");
                            break;
                        case KinematicPrefabStateEnum.Sleeping:
                            m_debugDrawSb.Append("Sleeping");
                            break;
                    }
                    m_debugDrawSb.AppendLine();
                    m_debugDrawSb.Append("IsOpened:");
                    m_debugDrawSb.Append(isOpened.Value);
                    m_debugDrawSb.AppendLine();
                    m_debugDrawSb.Append("IsClosed:");
                    m_debugDrawSb.Append(isClosed.Value);                    
                }


                MyDebugDraw.DrawText(WorldVolume.Center, m_debugDrawSb, Color.White, 0.5f);

                float radius = DETECTION_ANGLE != null ? DETECT_RADIUS * 0.5f : DETECT_RADIUS;
                MyDebugDraw.DrawSphereWireframe(WorldVolume.Center, radius, new Vector3(0f, 1f, 0f), 0.5f);
                if (DETECTION_ANGLE != null)
                {
                    float radius2 = (float)Math.Tan(DETECTION_ANGLE.Value) * DETECT_RADIUS;
                    Color color = Color.Red;
                    color.A = 55;
                    MyDebugDraw.DrawCone(WorldVolume.Center, WorldVolume.Center + WorldMatrix.Forward * DETECT_RADIUS, radius2, color);
                    MyDebugDraw.DrawCone(WorldVolume.Center, WorldVolume.Center + WorldMatrix.Backward * DETECT_RADIUS, radius2, color);
                    //MyDebugDraw.DrawSphereWireframe(WorldVolume.Center, DETECT_RADIUS, color.ToVector3(), 0.3f);
                }
            }

            return true;
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
            return IsAbleToWork() && (MyFactions.GetFactionsRelation(usedBy, this) == MyFactionRelationEnum.Friend || UseProperties.IsHacked);
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

        public MyPrefabKinematicPart[] Parts
        {
            get { return m_parts; }
        }

        #endregion
    }
}
