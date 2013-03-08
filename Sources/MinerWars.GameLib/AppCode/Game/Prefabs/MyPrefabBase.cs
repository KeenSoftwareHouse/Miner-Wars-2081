#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;

using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.HUD;
using KeenSoftwareHouse.Library.Memory;
using MinerWars.CommonLIB.AppCode.Import;

#endregion

namespace MinerWars.AppCode.Game.Prefabs
{
    abstract class MyPrefabBase : MyEntity
    {
        public static float PrefabDamageMultiplier = 1.0f;

        private List<Tuple<MyPrefabLamp, Matrix>> m_lamps;
        private List<Tuple<MyLight, Matrix>> m_lights;
        private List<MyParticleEffect> m_sparkEffects = new List<MyParticleEffect>();
        private List<Tuple<MyParticleEffect, Matrix>> m_effects = new List<Tuple<MyParticleEffect, Matrix>>();
        protected MyPrefabConfiguration m_config;

        protected MyPrefabContainer m_owner = null;  //owner container

        protected int m_prefabId;
        EventHandler m_sparksEffectUpdateEvent;
        EventHandler m_sparksEffectDeleteEvent;
        public Matrix LocalOrientation;
        public bool Modified;
        protected bool m_isInitialized = false;

        private bool? m_requiresEnergy = null;
        public bool? RequiresEnergy
        {
            get
            {
                return m_requiresEnergy.HasValue ? m_requiresEnergy.Value : m_config.RequiresEnergy;
            }
            set
            {
                m_requiresEnergy = value;
                WorkingChanged();
            }
        }

        //public MyPrefabGenerator Generator { get; set; }
        private int m_generatorsCount;
        public int GeneratorsCount
        {
            get
            {
                return m_generatorsCount;
            }
            set
            {
                m_generatorsCount = value;
                RecheckNeedsUpdate();
            }
        }
        private float m_electricCapacity;
        public float ElectricCapacity
        {
            get
            {
                return m_electricCapacity;
            }
            protected set
            {
                float oldValue = m_electricCapacity;
                m_electricCapacity = value;
                if (value > 0f && oldValue <= 0f ||
                    value <= 0f && oldValue > 0f)
                {
                    WorkingChanged();
                }
                m_electricCapacity = MathHelper.Clamp(m_electricCapacity, m_config.MinElectricCapacity, m_config.MaxElectricCapacity);
            }
        }

        public List<MyPrefabSnapPoint> SnapPoints { get; set; }

        private Matrix[] m_smokeMatrices;
        private MyParticleEffect[] m_smokeEffects;

        protected List<MyPrefabKinematicPartBase> m_kinematicParts;

        public IEnumerable<MyPrefabKinematicPartBase> GetKinematicParts()
        {
            return m_kinematicParts;
        }

        public override bool Enabled
        {
            set
            {
                bool changed = Enabled != value;
                base.Enabled = value;
                if (changed)
                {
                    WorkingChanged();
                }
            }
        }
        
        protected bool m_needsUpdate = false;
        private static List<MyElement> m_destroyHelper = new List<MyElement>();
        private static List<MyPrefabBase> m_prefabsToDestroy = new List<MyPrefabBase>(128);
        private static List<MyDummyPoint> m_particlesToStop = new List<MyDummyPoint>(128);

        private bool? m_causesAlarm = null;
        public bool CausesAlarm 
        {
            get
            {
                return m_causesAlarm.HasValue ? m_causesAlarm.Value : m_config.CausesAlarm;
            }
        }

        protected virtual void WorkingChanged()
        {
            RecheckNeedsUpdate();
            UpdateKinematicParts();

            m_enableEmissivity = IsWorking();
        }

        private void UpdateKinematicParts()
        {
            foreach (MyPrefabKinematicPartBase part in m_kinematicParts)
            {
                part.On = IsWorking();
            }
        }

        public virtual bool IsWorking()
        {
            return Enabled && IsElectrified() && m_activated;
        }

        public virtual bool IsAbleToWork()
        {
            return IsElectrified() && m_activated;
        }

        public MyPrefabBase(MyPrefabContainer owner)
        {
            m_owner = owner;
            SnapPoints = new List<MyPrefabSnapPoint>();
            m_kinematicParts = new List<MyPrefabKinematicPartBase>();
            Save = false;

            m_sparksEffectUpdateEvent = sparksEffect_OnUpdate;
            m_sparksEffectDeleteEvent = sparksEffect_OnDelete;
        }

        protected virtual void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {

        }

        public virtual void Init(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyPrefabBase.Init");

            StringBuilder displayNameSb = GetDisplayNameSb(displayName);// (displayName == null) ? null : new StringBuilder(displayName);
            //StringBuilder hudLabelTextSb = new StringBuilder(Enum.GetName(typeof(MyModelsEnum), prefabConfig.ModelLod0Enum));

            //TODO: Config.MaterialType 
            //m_prefabTypeEnum = prefabTypeEnum;
            m_prefabId = objectBuilder.GetObjectBuilderId().Value;
            m_config = prefabConfig;

            base.Init(displayNameSb, prefabConfig.ModelLod0Enum, prefabConfig.ModelLod1Enum, GetOwner(), null, objectBuilder, prefabConfig.ModelCollisionEnum);

            m_needsUpdate = m_config.NeedsUpdate;
            if (!string.IsNullOrEmpty(objectBuilder.DisplayName))
            {
                DisplayName = objectBuilder.DisplayName;
            }

            if (HasAvailableFactionMaterial(objectBuilder.FactionAppearance))
            {
                MaterialIndex = (ushort)objectBuilder.FactionAppearance;
            }

            this.LocalMatrix = Matrix.CreateWorld(relativePosition, localOrientation.Forward, localOrientation.Up);
            
            CastShadows = true;
            
            ////This solves bad saved values in DB   //this solves old prefabs max value
            //if (objectBuilder.PrefabMaxHealth == 0 || objectBuilder.PrefabMaxHealth == MyGameplayConstants.MAX_HEALTH_MAX)
            //{
            //    objectBuilder.PrefabMaxHealth = m_gameplayProperties.MaxHealth;
            //}

            ////This solves bad saved values in DB   //this solves old prefabs max value
            //if (objectBuilder.PrefabHealthRatio == 0 || objectBuilder.PrefabHealthRatio == 500 ||
            //    objectBuilder.PrefabHealthRatio == MyGameplayConstants.HEALTH_MAX)
            //{
            //    objectBuilder.PrefabHealthRatio = m_gameplayProperties.MaxHealth;
            //}            

            Flags |= EntityFlags.EditableInEditor;

            //MaxHealth = objectBuilder.PrefabMaxHealth;
            //Health = objectBuilder.PrefabHealthRatio;
            SetMaxHealth(objectBuilder.PrefabMaxHealth);
            HealthRatio = objectBuilder.PrefabHealthRatio;

            ElectricCapacity = objectBuilder.ElectricCapacity;

            if (RequiresEnergy != null && RequiresEnergy.Value == false)
            {   //We dont want these kind of prefabs to be charged every time sector is loaded
                m_electricCapacity = m_config.MaxElectricCapacity;
            }

            m_causesAlarm = objectBuilder.CausesAlarm;
            m_requiresEnergy = objectBuilder.RequiresEnergy;
            AIPriority = objectBuilder.AIPriority;

            if (m_config.InitPhysics)
            {
                InitPhysics();
            }

            SnapPoints = GetSnapPoints(GetModelLod0(), this);
            InitKinematicParts();

            m_smokeMatrices = GetSmokePoints(GetModelLod0());
            m_smokeEffects = new MyParticleEffect[m_smokeMatrices.Length];
            Debug.Assert(m_smokeMatrices != null);
            Debug.Assert(m_smokeEffects != null);

            InitLampsAndLights();
            InitPrefab(displayName, relativePosition, localOrientation, objectBuilder, prefabConfig);
            m_isInitialized = true;
            
            WorkingChanged();
            Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        protected virtual StringBuilder GetDisplayNameSb(string displayName) 
        {
            return (displayName == null) ? null : new StringBuilder(displayName);
        }

        /// <summary>
        /// IMPORTANT!!! Don't override this in other prefabs, override SetPrefabHudMarker()
        /// </summary>
        protected override void SetHudMarker()
        {
            //SetPrefabHudMarker();
            MyHud.ChangeText(this, new StringBuilder(this.DisplayName), MyGuitargetMode.Neutral);
        }

        //protected virtual void SetPrefabHudMarker() 
        //{
        //    MyHud.ChangeText(this, new StringBuilder(this.DisplayName), MyGuitargetMode.Neutral);
        //}

        private void InitPhysics()
        {
            InitPhysicsInternal();
        }

        protected virtual void InitPhysicsInternal() 
        {
            InitTrianglePhysics(MyMaterialType.METAL, 1.0f, ModelCollision, ModelLod0, collisionLayer: (ushort)MyConstants.COLLISION_LAYER_PREFAB);
        }

        private void InitKinematicParts()
        {
            // detect kinematic parts
            // !!! now prepared only for kinematic rotating parts !!!
            foreach (var dummyKVP in GetModelLod0().Dummies)
            {
                if (dummyKVP.Key.StartsWith("Dummy_Kinematic"))
                {
                    MyModelDummy dummy = dummyKVP.Value;
                    MyModelsEnum rotatingPartModel = MyModels.GetModelEnumByAssetName(dummy.CustomData["LINKEDMODEL"].ToString());
                    //TODO: read kinematic type, kinematic max velocity from dummies
                    //TODO: read kinematic cues from config?
                    MyPrefabKinematicRotatingPart rotatingPart = new MyPrefabKinematicRotatingPart(this.GetOwner());
                    Vector3 rotationVector = new Vector3((float)Convert.ToDouble(dummy.CustomData["ROTATION.DIR.X"]), -(float)Convert.ToDouble(dummy.CustomData["ROTATION.DIR.Z"]), (float)Convert.ToDouble(dummy.CustomData["ROTATION.DIR.Y"]));
                    Matrix partMatrix = dummy.Matrix;
                    rotatingPart.Init(this, rotatingPartModel, m_config.MaterialType, partMatrix, m_config.RotatingVelocity, m_config.LoopRotatingCue, m_config.LoopRotatingDamagedCue, m_config.StartRotatingCue, m_config.EndRotatingCue, rotationVector, Activated);
                    m_kinematicParts.Add(rotatingPart);
                }
            }
        }

        private void InitLampsAndLights()
        {
            Matrix worldMatrix = Matrix.Multiply(m_owner.WorldMatrix, LocalMatrix);

            Dictionary<string, MyModelDummy> dummies = MyModels.GetModelOnlyDummies(ModelLod0.ModelEnum).Dummies;
            foreach (KeyValuePair<string, MyModelDummy> dummy in dummies)
            {
                float scale = 4 * dummy.Value.Matrix.Left.Length();
                if (dummy.Key.StartsWith("LAMP"))
                {
                    if (m_lamps == null)
                    {
                        m_lamps = new List<Tuple<MyPrefabLamp, Matrix>>();
                    }
                    m_lamps.Add(new Tuple<MyPrefabLamp, Matrix>(new MyPrefabLamp(dummy.Value.Matrix.Translation + worldMatrix.Translation, 5 * scale, 20 * scale, 980), dummy.Value.Matrix));
                }
                if (dummy.Key.Contains("LIGHT") && (!dummy.Key.Contains("POINT_LIGHT_POS")))
                {
                    if (m_lights == null)
                    {
                        m_lights = new List<Tuple<MyLight, Matrix>>();
                    }
                    MyLight light = MyLights.AddLight();
                    light.Start(MyLight.LightTypeEnum.PointLight, worldMatrix.Translation + dummy.Value.Matrix.Translation, new Vector4(0, 1, 0, 1), 2f, 25 * scale);
                    m_lights.Add(new Tuple<MyLight, Matrix>(light, dummy.Value.Matrix));
                }
                if (dummy.Key.ToUpper().Contains("PARTICLE"))
                {
                    string particleIDString = dummy.Key.ToUpper().Replace("PARTICLE_", "");
                    if (particleIDString.Contains("_"))
                        particleIDString = particleIDString.Substring(0, particleIDString.IndexOf("_"));
                    int particleID = Convert.ToInt32(particleIDString);

                    MyParticleEffect effect = MyParticlesManager.CreateParticleEffect(particleID, true);
                    Matrix matrix = dummy.Value.Matrix * worldMatrix;
                    effect.WorldMatrix = matrix;
                    effect.AutoDelete = false;
                    m_effects.Add(new Tuple<MyParticleEffect, Matrix>(effect, dummy.Value.Matrix));
                }
            }
            UpdateChildrenPosition();
        }

        public void GetDefaultWaypointData(List<Vector3> positions, List<string> names = null, List<string> parentNames = null)
        {
            var dummies = ModelLod0 == null ? new Dictionary<string, MyModelDummy>() : MyModels.GetModelOnlyDummies(ModelLod0.ModelEnum).Dummies;

            foreach (KeyValuePair<string, MyModelDummy> dummy in dummies)
            {
                if (dummy.Key.StartsWith("Waypoint"))
                {
                    if (positions != null) positions.Add(Vector3.Transform(dummy.Value.Matrix.Translation, WorldMatrix));
                    if (names != null) names.Add(dummy.Key);
                    object parentName;
                    string parentNameAsString = null;
                    if (dummy.Value.CustomData.TryGetValue("ParentName", out parentName))
                        parentNameAsString = parentName as string;
                    if (parentNames != null) parentNames.Add(parentNameAsString);
                }
            }

        }

        /// <summary>Add waypoints defined in the prefab (as dummy points with the name "Waypoint...").</summary>
        /// <returns>A list of new waypoints.</returns>
        public List<MyWayPoint> InitWaypoints()
        {
            var dummies = ModelLod0 == null ? new Dictionary<string, MyModelDummy>() : MyModels.GetModelOnlyDummies(ModelLod0.ModelEnum).Dummies;
            var namedWayPoints = new Dictionary<string, MyWayPoint>();

            // pass 1: create waypoints
            foreach (KeyValuePair<string, MyModelDummy> dummy in dummies)
            {
                if (dummy.Key.StartsWith("Waypoint"))
                {
                    Vector3 position = Vector3.Transform(dummy.Value.Matrix.Translation, WorldMatrix);
                    namedWayPoints.Add(dummy.Key, MyWayPointGraph.CreateWaypoint(position, this));
                }
            }

            // pass 2: connect waypoints that are very close
            foreach (var v in namedWayPoints.Values)
                foreach (var w in namedWayPoints.Values)
                    if (v != w && MyWayPoint.Distance(v, w) < 0.5f)
                        MyWayPoint.Connect(v, w);

            // pass 3: connect waypoints to their parent waypoints
            foreach (KeyValuePair<string, MyModelDummy> dummy in dummies)
            {
                if (dummy.Key.StartsWith("Waypoint"))
                {
                    MyWayPoint wayPoint = namedWayPoints[dummy.Key];
                    object parentName;
                    if (dummy.Value.CustomData.TryGetValue("ParentName", out parentName))
                    {
                        if (parentName is string)
                        {
                            MyWayPoint parentWayPoint;
                            if (namedWayPoints.TryGetValue(parentName as string, out parentWayPoint))
                                MyWayPoint.Connect(wayPoint, parentWayPoint);
                        }
                    }
                }
            }

            // pass 4: all waypoints need to be in one connected component
            System.Diagnostics.Debug.Assert(MyWayPoint.Connected(namedWayPoints.Values),
                "Prefab " + (this is MyPrefab ? (this as MyPrefab).PrefabType.ToString() : "id_" + PrefabId.ToString()) + " has separated waypoints.");

            return namedWayPoints.Values.ToList();
        }

        /// <summary>Get the waypoint that's a child of the current prefab and closest to the given position.</summary>
        public MyWayPoint GetClosestWayPointTo(Vector3 position)
        {
            MyWayPoint best = null;
            float bestDistance = float.PositiveInfinity;

            foreach (var child in MyWayPoint.FilterWayPoints(Children))
            {
                float distance = Vector3.Distance(position, child.Position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = child;
                }
            }
            return best;
        }

        private static Matrix[] GetSmokePoints(MyModel model)
        {
            return (from dummy in model.Dummies
                    where dummy.Key.StartsWith("destruction", StringComparison.InvariantCultureIgnoreCase)
                    select dummy.Value.Matrix).ToArray();
        }

        public static List<MyPrefabSnapPoint> GetSnapPoints(MyModel model, MyPrefabBase prefab)
        {
            List<MyPrefabSnapPoint> snapPoints = new List<MyPrefabSnapPoint>();

            foreach (var dummy in model.Dummies)
            {
                if (dummy.Key.StartsWith("SNAPPOINT", StringComparison.InvariantCultureIgnoreCase))
                {
                    var customData = dummy.Value.CustomData;
                    var snapPoint = new MyPrefabSnapPoint(prefab);

                    // Get rid of scale in rotation part
                    snapPoint.Matrix = Matrix.CreateWorld(dummy.Value.Matrix.Translation, dummy.Value.Matrix.Forward, dummy.Value.Matrix.Up);
                    snapPoint.SnapType = new MyPrefabSnapPoint.MyPrefabSnapPointType("OBJECT_", "", dummy.Value.CustomData);
                    snapPoint.Name = dummy.Key;
                    snapPoints.Add(snapPoint);

                    string targetPostfix = "TARGET_BUILD_TYPE";
                    foreach (var target in customData)
                    {
                        if (target.Key.StartsWith(targetPostfix))
                        {
                            string postfix = target.Key.Substring(targetPostfix.Length);
                            snapPoint.SnapTargets.Add(new MyPrefabSnapPoint.MyPrefabSnapPointType(
                                "TARGET_",
                                postfix,
                                dummy.Value.CustomData));
                        }
                    }
                }
            }

            return snapPoints;
        }

        //////////////////////////////////////////////////////////////////////////
        // IMyEntity INTERFACE
        //////////////////////////////////////////////////////////////////////////

        public MyPrefabContainer GetOwner()
        {
            return m_owner;
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <returns></returns>
        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject))
            {
                if (m_lamps != null)
                {
                    for (int i = 0; i < m_lamps.Count; i++)
                    {
                        m_lamps[i].Item1.Draw();
                    }
                }

                if (MyFakes.SHOW_UNPOWERED_PREFABS)
                {
                    m_diffuseColor = ElectricCapacity > 0 ? Vector3.One : Vector3.One * 0.3f;
                }

                /*  //Crashes in parallel update because object could have been deleted
                foreach (MyParticleEffect sparkEffect in m_sparkEffects)
                {
                    //sparkEffect.WasVisibleLastFrame = LastFrameVisibilityEnum.VisibleLastFrame;
                    sparkEffect.RenderCounter = MyRender.RenderCounter;
                } */

                foreach (var effect in m_effects)
                {
                    MyParticlesManager.CustomDraw(effect.Item1);
                }
                if (m_smokeEffects != null)
                {
                    foreach (var effect in m_smokeEffects)
                    {
                        if (effect != null)
                        {
                            MyParticlesManager.CustomDraw(effect);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override bool DebugDraw()
        {
            //// For Debug purposes
            //foreach (var snapPoint in SnapPoints)
            //{
            //    MyDebugDraw.DrawAxis(snapPoint.Matrix * WorldMatrix, 50, 1);
            //}

            //base.DebugDrawAABB();

            return base.DebugDraw();
        }

        /// <summary>
        /// Gets parent inverted matrix and test whether entity can move to target location
        /// </summary>
        /// <param name="moveIndicator"></param>
        /// <param name="parentWorldInv"></param>
        /// <returns></returns>
        private bool PrepareMove(Vector3 moveIndicator, out Matrix parentWorldInv)
        {
            MyPrefabContainer container = Parent as MyPrefabContainer;
            System.Diagnostics.Debug.Assert(container != null);

            parentWorldInv = Parent.GetWorldMatrixInverted();
            Matrix SubeCubeInv = parentWorldInv;

            Vector3 relativePosTmp = MyUtils.GetRoundedVector3(MyUtils.GetTransform(moveIndicator, ref parentWorldInv), 1);
            if (MyPrefabContainer.IsPrefabOutOfContainerBounds(relativePosTmp) == true)
                return false;

            return true;
        }

        public override bool MoveAndRotate(Vector3 moveIndicator, Matrix orientation)
        {
            Matrix parentWorldInv;
            if (PrepareMove(moveIndicator, out parentWorldInv))
            {
                Matrix oldWorldMatrix = Matrix.Invert(this.WorldMatrix);
                Vector3 oldPosition = this.GetPosition();

                Vector3 relativePos = MyUtils.GetRoundedVector3(MyUtils.GetTransform(moveIndicator, ref parentWorldInv), 1);
                Matrix localOrientation;
                Matrix.Multiply(ref orientation, ref parentWorldInv, out localOrientation);
                this.LocalMatrix = Matrix.CreateWorld(relativePos, localOrientation.Forward, localOrientation.Up);
                LocalOrientation = localOrientation;
                Modified = true;

                return true;
            }
            return false;
        }

        public override bool CanMoveAndRotate(Vector3 moveIndicator, Matrix orientation)
        {
            Matrix parentMatrixInv;
            return PrepareMove(moveIndicator, out parentMatrixInv);
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabBase objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabBase;
            objectBuilder.PositionInContainer = MyPrefabContainer.GetRelativePositionInContainerCoords(this.LocalMatrix.Translation);

            float yaw, pitch, roll;
            Matrix rot = this.LocalMatrix;
            rot.Translation = Vector3.Zero;
            MyUtils.RotationMatrixToYawPitchRoll(ref rot, out yaw, out pitch, out roll);
            objectBuilder.AnglesInContainer = new Vector3(yaw, pitch, roll);

            Debug.Assert(MyUtils.IsValid(objectBuilder.AnglesInContainer), "Invalid angles in container, look at matrix 'rot'!");

            //if (MaxHealth == m_gameplayProperties.MaxHealth)
            //{
            //    objectBuilder.PrefabMaxHealth = MyGameplayConstants.MAX_HEALTH_MAX;
            //}
            //else
            //{
            //    objectBuilder.PrefabMaxHealth = MaxHealth;
            //}

            //if (Health == m_gameplayProperties.MaxHealth)
            //    objectBuilder.PrefabHealthRatio = MyGameplayConstants.HEALTH_MAX;
            //else
            //    objectBuilder.PrefabHealthRatio = Health;
            objectBuilder.PrefabHealthRatio = HealthRatio;
            objectBuilder.PrefabMaxHealth = GetMaxHealth();

            objectBuilder.DisplayName = DisplayName;

            objectBuilder.FactionAppearance = (MyMwcObjectBuilder_Prefab_AppearanceEnum)MaterialIndex;

            objectBuilder.CausesAlarm = m_causesAlarm;
            objectBuilder.RequiresEnergy = m_requiresEnergy;
            objectBuilder.AIPriority = AIPriority;

            return objectBuilder;
        }


        /// <summary>
        /// Called when [activated] which for entity means that was added to scene.
        /// </summary>
        /// <param name="source">The source of activation.</param>
        protected override void OnActivated(object source)
        {
            base.OnActivated(source);
            if (m_isInitialized)
            {
                WorkingChanged();
            }
            UpdateActivatedState();
            //UpdateRenderobject(true);
        }

        /// <summary>
        /// Called when [deactivated] which for entity means that was removed from scene.
        /// </summary>
        /// <param name="source">The source of deactivation.</param>
        protected override void OnDeactivated(object source)
        {
            base.OnDeactivated(source);
            if (m_isInitialized)
            {
                WorkingChanged();
            }
            UpdateActivatedState();
           // UpdateRenderobject(false);
        }

        private void UpdateActivatedState() 
        {
            if (m_owner != null)
            {
                if (Activated)
                {
                    if (m_lamps != null)
                    {
                        foreach (var prefabLamp in m_lamps)
                        {
                            prefabLamp.Item1.Light.LightOn = true;
                        }
                    }
                    if (m_lights != null)
                    {
                        foreach (var light in m_lights)
                        {
                            light.Item1.LightOn = true;
                        }
                    }

                    m_owner.ActivatePrefab(this);
                }
                else
                {            
                    if (m_lamps != null)
                    {
                        foreach (var prefabLamp in m_lamps)
                        {
                            prefabLamp.Item1.Light.LightOn = false;
                        }
                    }
                    if (m_lights != null)
                    {
                        foreach (var light in m_lights)
                        {
                            light.Item1.LightOn = false;
                        }
                    }

                    m_owner.DeactivatePrefab(this);
                }
            }
        }

        public override bool IsSelectableAsChild()
        {
            return true;
        }

        public override void ResetRotation()
        {
            MoveAndRotate(this.GetPosition(), Parent.GetOrientation());
        }

        public override void Close()
        {
            // Remove possible linked snap points from editor
            MyEditor.Static.RemoveLinkedSnapPoints(this);

            foreach (var effect in m_effects)
            {
                MyParticlesManager.RemoveParticleEffect(effect.Item1);
            }
            m_effects.Clear();

            base.Close();

            // ((MyPrefabContainer)Parent).RemovePrefab(this); // Parent is null for some prefabs (e.g. some large ship weapons)

            if (m_smokeEffects != null)
            {
                for (int i = 0; i < m_smokeEffects.Length; i++)
                {
                    if (m_smokeEffects[i] != null)
                    {
                        MyParticlesManager.RemoveParticleEffect(m_smokeEffects[i]);
                    }
                }
                m_smokeEffects = null;
            }

            if (m_owner != null)
            {
                m_owner.RemovePrefab(this);
            }

            if (m_lamps != null)
            {
                foreach (var prefabLamp in m_lamps)
                {
                    prefabLamp.Item1.Close();
                }
            }
            if (m_lights != null)
            {
                for (int i = 0; i < m_lights.Count; i++)
                {
                    MyLights.RemoveLight(m_lights[i].Item1); m_lights[i] = null;
                }
            }
        }

        /// <summary>
        /// Generates explosion and debris.
        /// </summary>
        protected virtual void Explode()
        {
            //if (Physics.Enabled)
            //{
            //    Physics.Enabled = false;
            //    Physics.Clear();
            //}

            // only if prefab is big enough, make it explode and create debris))
            if (WorldVolumeHr.Radius > m_config.MinSizeForExplosion)
            {
                DestroyPrefabsInside();

                MyExplosion newExplosion = MyExplosions.AddExplosion();
                if (newExplosion != null)
                {
                    BoundingSphere explosionSphere = WorldVolumeHr;
                    explosionSphere.Radius *= m_config.ExplosionRadiusMultiplier;

                    float particleScale = 1;
                   /* if (explosionSphere.Radius > MyExplosionsConstants.EXPLOSION_RADIUS_MAX)
                    {
                        particleScale = explosionSphere.Radius / MyExplosionsConstants.EXPLOSION_RADIUS_MAX;
                        explosionSphere.Radius = MathHelper.Min(explosionSphere.Radius, MyExplosionsConstants.EXPLOSION_RADIUS_MAX);
                    }
                    else*/
                    if (m_config.ExplosionType == MyExplosionTypeEnum.BOMB_EXPLOSION)
                    {
                        particleScale = MathHelper.Max(1, WorldVolumeHr.Radius * 0.2f);
                    }
                    if (m_config.ExplosionType == MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION)
                    {
                        //Scaled by smallship size
                        particleScale = MathHelper.Max(1, WorldVolumeHr.Radius / 20f);
                    }
                    if (m_config.ExplosionType == MyExplosionTypeEnum.LARGE_SHIP_EXPLOSION)
                    {
                        //Scaled by KAI size
                        particleScale = MathHelper.Max(1, WorldVolumeHr.Radius / 300f);
                    }

                    MyVoxelMap voxelMap = MyVoxelMaps.GetOverlappingWithSphere(ref explosionSphere);
                    MyExplosionDebrisModel.CreateExplosionDebris(ref explosionSphere, MyGroupMask.Empty, this, voxelMap);
                    float voxelCutoutScale = 0.6f; // Prefabs do smaller voxel cutout

                    MyExplosionInfo explosionInfo = new MyExplosionInfo()
                    {
                        PlayerDamage = 0,
                        Damage = m_config.ExplosionDamage,
                        EmpDamage = 0,
                        ExplosionType = m_config.ExplosionType,
                        ExplosionSphere = explosionSphere,
                        LifespanMiliseconds = MyExplosionsConstants.EXPLOSION_LIFESPAN,
                        VoxelCutoutScale = voxelCutoutScale,
                        ParticleScale = particleScale * m_config.ExplosionParticleEffectScale,
                        HitEntity = this,
                        CheckIntersections = false,
                        CreateDecals = false,
                        CreateDebris = false,
                        CreateParticleEffect = true,
                        PlaySound = true,
                    };

                    newExplosion.Start(ref explosionInfo);
                }
            }
            else
            {
                var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_SmallPrefab);
                effect.WorldMatrix = Matrix.CreateWorld(WorldVolume.Center, WorldMatrix.Forward, WorldMatrix.Up);
                //effect.UserRadiusMultiplier = .1f * WorldVolumeHr.Radius;
                effect.UserScale = 0.05f * m_config.ExplosionRadiusMultiplier * m_config.ExplosionParticleEffectScale * WorldVolumeHr.Radius;
            }
        }

        /// <summary>
        /// Using OBBs, detects prefabs that are COMPLETELY contained in this prefab's OBB (size multiplied by
        /// MyPrefabConstants.EXPLOSION_DELETE_MULTIPLIER).
        /// </summary>
        private void DestroyPrefabsInside()
        {
            DestroyPrefabsInsideHr();

            foreach (var prefabToDestroy in m_prefabsToDestroy)
            {
                MyScriptWrapper.OnEntityDeath(prefabToDestroy, this);
                prefabToDestroy.MarkForClose();
            }

            foreach (var particleToStop in m_particlesToStop)
            {
                particleToStop.DisableParticleEffect();
            }

            m_prefabsToDestroy.Clear();
            m_particlesToStop.Clear();
        }

        private void DestroyPrefabsInsideHr()
        {
            MyOrientedBoundingBox largerOBB;
            BoundingBox largerAABB;
            GetEnlargedBoundingBoxes(out largerOBB, out largerAABB);

            RemoveContainedPrefabs(largerOBB, largerAABB);

            DisableContainedDummyParticles(largerOBB, largerAABB);
        }

        private void RemoveContainedPrefabs(MyOrientedBoundingBox largerOBB, BoundingBox largerAABB)
        {
            var intersectingElements = MyEntities.GetElementsInBox(ref largerAABB);

            using (var intersectingElements2 = PoolList<MyRBElement>.Get())
            {
                intersectingElements2.AddRange(intersectingElements);
                intersectingElements.Clear();

                foreach (var rbElement in intersectingElements2)
                {
                    var rigidBody = rbElement.GetRigidBody();
                    if (rigidBody == null)
                        continue;
                    var containedEntity = ((MyPhysicsBody)rigidBody.m_UserData).Entity;

                    var candidateToDestroy = containedEntity.GetTopMostParent(typeof(MyPrefabBase)) as MyPrefabBase;

                    if (candidateToDestroy != null && candidateToDestroy != this)
                    {
                        var position = candidateToDestroy.WorldVolumeHr.Center;

                        // if contained entity can be destroyed, is smaller than me and has center inside me, close it
                        if (WorldVolumeHr.Radius > candidateToDestroy.WorldVolumeHr.Radius &&
                            largerOBB.Contains(ref position) &&
                            candidateToDestroy.IsDestructible)
                        {
                            if (candidateToDestroy.m_isExploded)
                                continue;

                            candidateToDestroy.m_isExploded = true;
                            m_prefabsToDestroy.Add(candidateToDestroy);

                            candidateToDestroy.DestroyPrefabsInsideHr();
                        }

                        continue;
                    }
                }
            }

            intersectingElements.Clear();

        }

        private void DisableContainedDummyParticles(MyOrientedBoundingBox largerOBB, BoundingBox largerAABB)
        {
            MyRender.GetEntitiesFromPrunningStructure(ref largerAABB, m_destroyHelper);

            foreach (var elem in m_destroyHelper)
            {
                var entity = ((MyRenderObject)elem).Entity;

                var dummyPoint = entity as MyDummyPoint;
                if (dummyPoint != null && !dummyPoint.CanSurvivePrefabDestruction())
                {
                    var position = dummyPoint.GetPosition();

                    // if contained entity can be destroyed, is smaller than me and has center inside me, close it
                    if (largerOBB.Contains(ref position))
                    {
                        m_particlesToStop.Add(dummyPoint);
                    }

                    continue;
                }
            }

            m_destroyHelper.Clear();
        }


        private void GetEnlargedBoundingBoxes(out MyOrientedBoundingBox largerOBB, out BoundingBox largerAABBHr)
        {
            // compute larger OBB
            var obb = MyOrientedBoundingBox.CreateFromBoundingBox(GetModelLod0().BoundingBox).Transform(WorldMatrix);

            var averageHalfExtent = (obb.HalfExtent.X + obb.HalfExtent.Y + obb.HalfExtent.Z) / 3f;

            float explosionDeleteMultiplier = MathHelper.Clamp(MyPrefabConstants.EXPLOSION_DELETE_MULTIPLIER_BY_SIZE * averageHalfExtent, 1.0f, 1.6f);

            var obbHalfExtentIncrease = obb.HalfExtent * explosionDeleteMultiplier;
            var obbHalfExtentIncreaseClamped = Vector3.Clamp(obbHalfExtentIncrease,
                                                             MyPrefabConstants.MIN_SIZE_INCREASE_FOR_EXPLOSION,
                                                             obbHalfExtentIncrease);
            largerOBB = obb;
            largerOBB.HalfExtent += obbHalfExtentIncreaseClamped;

            // compute larger AABB
            var minToMax = WorldAABBHr.Max - WorldAABBHr.Min;
            minToMax *= explosionDeleteMultiplier;
            minToMax += MyPrefabConstants.MIN_SIZE_INCREASE_FOR_EXPLOSION;
            largerAABBHr = new BoundingBox(WorldAABBHr.Min - minToMax, WorldAABBHr.Max + minToMax);
        }

        public override void UpdateBeforeSimulation()
        {
            if (IsVisible() && IsDamaged() && !m_isExploded)
                DrawDestructionEffects(GetDamageRatio());

            //UpdateAABBHr();

            //float electricityAddStep = MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS * (HasGenerator() ? 1f : -1f);
            ElectricCapacity = (HasGenerator() ? ElectricCapacity + MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS : 0f);

            RecheckNeedsUpdate();

            //base.UpdateBeforeSimulation();
            if (IsWorking())
            {
                UpdatePrefabBeforeSimulation();
            }
        }

        protected virtual void UpdatePrefabBeforeSimulation()
        {

        }

        /// <summary>
        /// DoDamage on prefabs can be modified by cheats (FRIEND_NEUTRAL_CANT_DIE)
        /// </summary>
        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MinerWars.AppCode.Game.Utils.MyDamageType damageType, MinerWars.AppCode.Game.Utils.MyAmmoType ammoType, MinerWars.AppCode.Game.Entities.MyEntity damageSource, bool justDeactivate)
        {
            if (MyFakes.INDESTRUCTIBLE_PREFABS)
            {
                return;
            }

            if (m_isExploded)
                return;

            if (MyGuiScreenGamePlay.Static != null && MySession.PlayerShip != null && damageSource != null)
            {
                var playerFactionRelation = MyFactions.GetFactionsRelation(this, damageSource);

                // Disable damage on friendly prefabs when FRIEND_NEUTRAL_CANT_DIE cheat is enabled
                if (playerFactionRelation == MyFactionRelationEnum.Friend &&
                    MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.FRIEND_NEUTRAL_CANT_DIE))
                {
                    return;
                }
            }

            damage *= PrefabDamageMultiplier;

            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
            ElectricCapacity -= empDamage;

            //NeedsUpdate = true;
            RecheckNeedsUpdate();
                    
            if (IsDead() && !m_isExploded)
            {
                if (justDeactivate)
                {
                    //Activate(false);
                    MarkForClose();
                }
                else
                {
                    MarkForClose();
                    Explode();
                }

                MakeWaypointsFree();

                m_isExploded = true;

                if (CausesAlarm)
                {
                    m_owner.LaunchAlarm(this, damageSource);
                }

            }
        }

        /// <summary>
        /// Take all waypoints under a prefab and put them in MyEntities (to make them survive prefab destruction).
        /// </summary>
        private void MakeWaypointsFree()
        {
            var list = new List<MyWayPoint>();

            foreach (var child in Children)
            {
                var waypoint = child as MyWayPoint;
                if (waypoint != null)
                    list.Add(waypoint);
            }

            foreach (var waypoint in list)
            {
                RemoveChild(waypoint, true);
                //waypoint.Activate(false, false);  // breaks ticket 5354
                MyEntities.Add(waypoint);
            }
        }

        public override void UpdateAfterSimulation()
        {
            //base.UpdateAfterSimulation();

            if (IsVisible())
            {
                if (m_smokeEffects != null)
                {
                    if (m_smokeEffects.Length > 0)
                    {
                        UpdateSmokeEffects();
                    }
                }
                if (IsWorking())
                {
                    UpdatePrefabAfterSimulation();
                }
            }
        }

        protected virtual void UpdatePrefabAfterSimulation()
        {

        }

        private void UpdateSmokeEffects()
        {
            float ratio = GetDamageRatio();
            if (ratio < MyPrefabConstants.DAMAGED_HEALTH)
            {
                for (int i = 0; i < m_smokeEffects.Length; i++)
                {
                    if (m_smokeEffects[i] != null)
                    {
                        m_smokeEffects[i].Stop();
                        m_smokeEffects[i] = null;
                    }
                }
            }
            else
            {
                ratio = (ratio / MyPrefabConstants.DAMAGED_HEALTH) - 1;

                int smokesActive = Convert.ToInt32(MathHelper.Lerp(1, m_smokeMatrices.Length, ratio));

                // update active effects
                for (int i = 0; i < smokesActive; i++)
                {
                    var smokeEffect = m_smokeEffects[i];
                    if (smokeEffect == null)
                    {
                        // alternate between 3 different effects
                        smokeEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Damage_SmokeDirectionalA + (i % 3), true);
                        smokeEffect.AutoDelete = false;

                        m_smokeEffects[i] = smokeEffect;
                    }

                    smokeEffect.UserBirthMultiplier = 1;
                    smokeEffect.WorldMatrix = m_smokeMatrices[i] * WorldMatrix;
                }
            }
        }

        public override void OnWorldPositionChanged(object source)
        {
            //UpdateAABBHr(); //Do we need Hr version at all?
            /*  //let owner/caller to handle this
            if (m_owner.WorldAABBHr.Contains(WorldAABBHr) != ContainmentType.Contains)
            {
                m_owner.UpdateAABB();
            } */

            UpdateChildrenPosition();

            base.OnWorldPositionChanged(source);
        }

        private void UpdateChildrenPosition()
        {
            foreach (var effectPair in m_effects)
            {
                effectPair.Item1.WorldMatrix = effectPair.Item2 * this.WorldMatrix;
            }

            if (m_lights != null)
            {
                foreach (var light in m_lights)
                {
                    light.Item1.SetPosition((light.Item2 * WorldMatrix).Translation);
                }
            }

            if (m_lamps != null)
            {
                foreach (var lamp in m_lamps)
                {
                    lamp.Item1.MoveTo((lamp.Item2 * WorldMatrix).Translation);
                }
            }
        }

        /// <summary>
        /// IMPORTANT: this is not really 'render' method. It only manipulates particle effects.
        /// </summary>
        protected void DrawDestructionEffects(float damagedPercentage)
        {
            float heavyDamagePercentage = (damagedPercentage - MyExplosionsConstants.DAMAGE_SPARKS) / (1 - MyExplosionsConstants.DAMAGE_SPARKS);

            if (heavyDamagePercentage < MyMwcMathConstants.EPSILON)
                return;

            MyModel model = GetModelLod0();

            if (MyMwcUtils.GetRandomInt((int)(MyExplosionsConstants.FRAMES_PER_SPARK / (heavyDamagePercentage * System.Math.Min(model.BoundingSphere.Radius, 100) * 0.005f))) > 0)
                //if (MyMwcUtils.GetRandomInt((int)(MyExplosionsConstants.FRAMES_PER_SPARK / heavyDamagePercentage)) > 0)
                return;

            if (!MyCamera.IsInFrustum(this.WorldAABB))
                return;

            int randomVertexIndex = MyMwcUtils.GetRandomInt(0, model.GetVerticesCount() - 1);
            Vector3 randomVertex = model.GetVertex(randomVertexIndex);
            var wMatrix = this.WorldMatrix;
            Vector3 vertexInWorldSpace = MyUtils.GetTransform(randomVertex, ref wMatrix);


            MyLight light = MyLights.AddLight();
            if (light != null)
            {
                light.Start(MyLight.LightTypeEnum.PointLight, 1.0f);
                light.PointOn = false;
                light.Range = 1.0f;
            }

            MyParticleEffect sparksEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Damage_Sparks);
            if (sparksEffect != null)
            {
                sparksEffect.WorldMatrix = Matrix.CreateTranslation(vertexInWorldSpace);
                sparksEffect.UserRadiusMultiplier = 0.003f * model.BoundingSphere.Radius;
                sparksEffect.Tag = light;
                sparksEffect.OnUpdate += m_sparksEffectUpdateEvent;
                sparksEffect.OnDelete += m_sparksEffectDeleteEvent;
                m_sparkEffects.Add(sparksEffect);
            }
            MyAudio.AddCue3D(MySoundCuesEnum.SfxSpark, WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero);
        }

        void sparksEffect_OnDelete(object sender, EventArgs e)
        {
            MyParticleEffect explosionEffect = sender as MyParticleEffect;
            var light = explosionEffect.Tag as MyLight;

            if (light != null)
            {
                MyLights.RemoveLight(light);
            }

            explosionEffect.Tag = null;
            m_sparkEffects.Remove(explosionEffect);
        }

        static void sparksEffect_OnUpdate(object sender, EventArgs e)
        {
            MyParticleEffect explosionEffect = sender as MyParticleEffect;
            MyLight light = explosionEffect.Tag as MyLight;

            if (light != null)
            {
                light.PointOn = true;
                light.SetPosition(explosionEffect.WorldMatrix.Translation);
                light.Range = MathHelper.Clamp((float)explosionEffect.GetAABB().Size().Length() / 2.0f, 1.0f, MyLightsConstants.MAX_POINTLIGHT_RADIUS);
                light.Intensity = 3 * MyMwcUtils.GetRandomFloat(0.6f, 0.9f);

                MyAnimatedPropertyVector4 animatedProperty; float time;
                explosionEffect.GetGenerations()[3].Color.GetPreviousValue(0.0f, out animatedProperty, out time);

                Vector4 lightColor;
                animatedProperty.GetInterpolatedValue<Vector4>(0.5f, out lightColor);
                light.Color = lightColor;
            }
        }

        public override MyMwcObjectBuilder_FactionEnum Faction
        {
            get
            {
                if (m_owner != null)
                {
                    return m_owner.Faction;
                }
                return base.Faction;
            }
            set
            {
                //  System.Diagnostics.Debug.Assert(false, "Prefab always takes faction from container");
            }
        }

        //public MyMwcObjectBuilder_Prefab_TypesEnum PrefabType
        //{
        //    get { return m_prefabTypeEnum; }
        //}

        public CategoryTypesEnum PrefabCategory
        {
            get { return m_config.CategoryType; }
        }

        public SubCategoryTypesEnum? PrefabSubCategory
        {
            get { return m_config.SubCategoryType; }
        }

        public PrefabTypesFlagEnum PrefabTypeFlag
        {
            get { return m_config.PrefabTypeFlag; }
        }

        public int PrefabId
        {
            get { return m_prefabId; }
        }

        protected override bool CanBeAddedToRender()
        {
            return ModelLod0 != null;
        }

        /// <summary>
        /// Returns true if this prefab has available textures for specific factions.
        /// Large ship prefabs, large weapon prefabs and the CUSTOM object don't.
        /// This is dependent on artists. Would be better if this was read
        /// </summary>
        public bool HasAvailableFactionMaterial(MyMwcObjectBuilder_Prefab_AppearanceEnum material)
        {
            var factionSpecific = m_config.FactionSpecific;

            bool hasMaterialForAllFactions = !factionSpecific.HasValue;
            return hasMaterialForAllFactions || factionSpecific.Value == material;
        }

        public MyPrefabConfiguration GetConfiguration()
        {
            return (MyPrefabConfiguration)m_config;
        }

        public bool IsElectrified()
        {
            return ElectricCapacity > 0;
        }

        public void RemoveKinematicPart(MyPrefabKinematicPartBase part)
        {
            m_kinematicParts.Remove(part);
        }

        protected bool HasGenerator()
        {
            return GeneratorsCount > 0 || (RequiresEnergy != null && !RequiresEnergy.Value) || PrefabCategory == CategoryTypesEnum.GENERATOR || MyFakes.ENABLE_PREFABS_AUTO_CHARGING;
        }

        protected void RecheckNeedsUpdate()
        {
            bool newNeedsUpdate =
                    PrefabNeedsUpdateNow ||
                    IsDamaged() ||
                    //Nabijeni
                    (ElectricCapacity < m_config.MaxElectricCapacity && HasGenerator())
                    ||
                    //Vybijeni
                    (ElectricCapacity > m_config.MinElectricCapacity && !HasGenerator());

            if (NeedsUpdate != newNeedsUpdate)
            {
                NeedsUpdate = newNeedsUpdate;
            }
        }

        protected virtual bool PrefabNeedsUpdateNow
        {
            get { return Enabled && IsElectrified() && m_needsUpdate; }
        }

        public void SetCausesAlarm(bool? causesAlarm)
        {
            m_causesAlarm = causesAlarm;
        }

        public bool DefaultCausesAlarm()
        {
            return !m_causesAlarm.HasValue;
        }
    }
}
