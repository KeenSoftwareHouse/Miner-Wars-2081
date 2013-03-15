using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Physics;
using MinerWarsMath;


namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using GUI;
    using MinerWarsMath;
    using Models;
    using Physics;
    using Prefabs;
    using Utils;
    using MinerWars.AppCode.Game.Prefabs;
    using MinerWars.AppCode.Game.Entities;
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
    using CommonLIB.AppCode.Utils;
    using System;
    using Render;
    using System.Text;
    using SubObjects;
    using MinerWars.AppCode.Game.Entities.Weapons;
    using MinerWars.AppCode.Game.Explosions;
    using MinerWars.AppCode.Game.Voxels;
    using MinerWars.AppCode.Game.GUI.Core;
    using MinerWars.AppCode.Game.GUI.Prefabs;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Entities.EntityDetector;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
    using MinerWars.AppCode.Game.HUD;
    using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Audio.Dialogues;
    using MinerWars.AppCode.Physics.Collisions;
    using MinerWars.AppCode.Game.Localization;

    class MyPrefabLargeWeapon : MyPrefabBase, IMyUseableEntity, IMyHasGuiControl
    {
        private MyLargeShipGunBase m_gun;
        private MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum weaponType;
        private MyPrefabConfigurationLargeWeapon prefabConfiguration;

        private MyEntityDetector m_targetsDetector;
        private MyEntityDetector m_potentialTargetsDetector;
        private bool m_foundSmallShipsPositionChange = false;
        private List<IMyEntityDetectorCriterium> m_targetDetectorCriterias = new List<IMyEntityDetectorCriterium>();
        private List<IMyEntityDetectorCriterium> m_potentialTargetDetectorCriterias = new List<IMyEntityDetectorCriterium>();
        //private List<MySmallShip> m_foundSmallShips = new List<MySmallShip>();
        private MySmallShip m_target;
        private MySmallShip m_potentialTarget;

        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            if (m_gun != null)
            {
                m_gun.Enabled = IsWorking();
            }
            m_targetsDetector.TrySetStatus(IsWorking());
            m_potentialTargetsDetector.TrySetStatus(IsWorking());
        }

        public MyLargeShipGunBase GetGun() 
        {
            return m_gun;
        }

        public override MyMwcObjectBuilder_FactionEnum Faction
        {
            get
            {
                return MyGuiScreenGamePlay.Static.IsControlledByPlayer(this) ? MySession.Static.Player.Faction : base.Faction;
            }
            set
            {
                base.Faction = value;
            }
        }

        public MyPrefabLargeWeapon(MyPrefabContainer owner) : base(owner) 
        {
            CheckExplosionObstacles = false;
        }        

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {            
            prefabConfiguration = prefabConfig as MyPrefabConfigurationLargeWeapon;
            MyMwcObjectBuilder_PrefabLargeWeapon largeWeaponBuilder = objectBuilder as MyMwcObjectBuilder_PrefabLargeWeapon;
            weaponType = largeWeaponBuilder.PrefabLargeWeaponType;

            UseProperties = new MyUseProperties(MyUseType.FromHUB | MyUseType.Solo, MyUseType.FromHUB);
            if (largeWeaponBuilder.UseProperties == null)
            {
                UseProperties.Init(MyUseType.FromHUB | MyUseType.Solo, MyUseType.FromHUB, 1, 4000, false);
            }
            else
            {                
                UseProperties.Init(largeWeaponBuilder.UseProperties);
            }            

            // create & initialize weapon:
            MyLargeShipGunBase.CreateAloneWeapon(ref m_gun, displayName, Vector3.Zero, Matrix.Identity, largeWeaponBuilder, Activated);
            AddChild(m_gun);

            m_gun.PrefabParent = this;
            m_gun.Enabled = IsWorking();
            m_gun.SetRandomRotation();

           // if (largeWeaponBuilder.SearchingDistance == 2000)
             //   largeWeaponBuilder.SearchingDistance = 1000;

            this.LocalMatrix = Matrix.CreateWorld(relativePosition, localOrientation.Forward, localOrientation.Up);
            m_searchingDistance = MathHelper.Clamp(largeWeaponBuilder.SearchingDistance, MyLargeShipWeaponsConstants.MIN_SEARCHING_DISTANCE, MyLargeShipWeaponsConstants.MAX_SEARCHING_DISTANCE); 
            m_targetDetectorCriterias.Add(new MyEntityDetectorCriterium<MySmallShip>(1, IsPossibleTarget, true));
            m_targetsDetector = new MyEntityDetector(true);
            
            m_potentialTargetsDetector = new MyEntityDetector(true);
            m_potentialTargetDetectorCriterias.Add(new MyEntityDetectorCriterium<MySmallShip>(1, IsPotentialTarget, true));
            //m_targetsDetector.OnEntityEnter += OnTargetDetected;
            //m_targetsDetector.OnEntityLeave += OnTargetLost;
            m_targetsDetector.OnNearestEntityChange += OnNearestTargetChange;
            m_potentialTargetsDetector.OnNearestEntityChange += OnNearestPotentialTargetChange;
            //m_targetsDetector.OnEntityPositionChange += OnTargetPositionChanged;
            InitDetector(Activated);
        }

        protected override void SetHudMarker()
        {
            MyHud.ChangeText(this, new StringBuilder(), null, SearchingDistance, HUD.MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | HUD.MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | HUD.MyHudIndicatorFlagsEnum.SHOW_DISTANCE | HUD.MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_FACTION_RELATION_MARKER | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER | MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR, new HUD.MyHudDisplayFactionRelation(false, false, true));
        }

        private float m_searchingDistance;
        public float SearchingDistance 
        {
            get { return m_searchingDistance; }
            set 
            {
                value = MathHelper.Clamp(value, MyLargeShipWeaponsConstants.MIN_SEARCHING_DISTANCE, MyLargeShipWeaponsConstants.MAX_SEARCHING_DISTANCE);
                bool changed = m_searchingDistance != value;
                m_searchingDistance = value;
                if (changed) 
                {
                    m_targetsDetector.Size = new Vector3(SearchingDistance * 2f, 0f, 0f);
                    m_potentialTargetsDetector.Size = new Vector3(SearchingDistance * 2f, 0f, 0f);
                }
            }
        }

        //private void OnTargetDetected(MyEntityDetector sender, MyEntity entity, int criterias)
        //{
        //    m_foundSmallShips.Add(entity as MySmallShip);
        //    m_foundSmallShipsPositionChange = true;
        //}

        //private void OnTargetLost(MyEntityDetector sender, MyEntity entity)
        //{
        //    m_foundSmallShips.Remove(entity as MySmallShip);
        //}

        //private void OnTargetPositionChanged(MyEntityDetector sender, MyEntity entity, Vector3 position)
        //{
        //    m_foundSmallShipsPositionChange = true;
        //}

        private void OnNearestTargetChange(MyEntityDetector sender, MyEntity oldNearestEntity, MyEntity newNearestEntity)
        {
            m_target = newNearestEntity as MySmallShip;
            if (m_target != null)
            {
            }

        }

        private void OnNearestPotentialTargetChange(MyEntityDetector sender, MyEntity oldNearestEntity, MyEntity newNearestEntity)
        {
            m_potentialTarget = newNearestEntity as MySmallShip;
        }

        private bool IsPossibleTarget(MySmallShip smallShip, params object[] args)
        {
            if (!IsPotentialTarget(smallShip, args))
                return false;

            MyLine testRay = new MyLine(m_gun.GetBarell().GetPosition(), smallShip.GetPosition(), false);
            Vector3? result = MyEntities.GetAnyIntersectionWithLine(ref testRay, this, smallShip, true, true, false, true);
            //smallShip.GetIntersectionWithLine(ref testRay, out result);
            return result == null;
        }

        private bool IsPotentialTarget(MySmallShip smallShip, params object[] args)
        {
            if (MyFactions.GetFactionsRelation(GetOwner(), smallShip) != MyFactionRelationEnum.Enemy)
            {
                return false;
            }
            
            if (smallShip == MySession.PlayerShip && (Vector3.DistanceSquared(smallShip.GetPosition(), this.GetPosition()) > MyGameplayConstants.GameplayDifficultyProfile.LargeWeaponMaxAttackingDistanceForPlayer * MyGameplayConstants.GameplayDifficultyProfile.LargeWeaponMaxAttackingDistanceForPlayer))
                return false;
              
            return true;
        }

        public bool IsGuided() 
        {
            return PrefabLargeWeaponType == MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED4 ||
                   PrefabLargeWeaponType == MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED6 ||
                   PrefabLargeWeaponType == MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum.P352_A02_LARGESHIP_MISSILE_GUIDED9;
        }

        public void InitDetector(bool activated)
        {
            m_targetsDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(SearchingDistance * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, m_targetDetectorCriterias);
            m_targetsDetector.SetSensorDetectRigidBodyTypes(MyConstants.RIGIDBODY_TYPE_SHIP);
            m_targetsDetector.TrySetStatus(Enabled);
            if (activated)
            {
                m_targetsDetector.PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;
            }
            else
            {
                m_targetsDetector.PersistentFlags |= MyPersistentEntityFlags.Deactivated;
            }

            m_potentialTargetsDetector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(SearchingDistance * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, m_potentialTargetDetectorCriterias);
            m_potentialTargetsDetector.SetSensorDetectRigidBodyTypes(MyConstants.RIGIDBODY_TYPE_SHIP);
            m_potentialTargetsDetector.TrySetStatus(Enabled);
            if (activated)
            {
                m_potentialTargetsDetector.PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;
            }
            else
            {
                m_potentialTargetsDetector.PersistentFlags |= MyPersistentEntityFlags.Deactivated;
            }
        }

        //public void CloseDetector()
        //{
        //    m_targetsDetector.Close();            
        //}

        public override void Close()
        {
            MyDecals.RemoveModelDecals(m_gun);
            base.Close();
        }

        public void GetClosestSmallShipInSearchDistance(out MySmallShip closestShip)
        {
            closestShip = m_target;
        }

        public void GetClosestPotentialSmallShipInSearchDistance(out MySmallShip closestShip)
        {
            closestShip = m_potentialTarget;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabLargeWeapon objectBuilderWeapon = (MyMwcObjectBuilder_PrefabLargeWeapon)base.GetObjectBuilderInternal(getExactCopy);

            //This is duplicate from MyPrefab
            objectBuilderWeapon.PositionInContainer = MyPrefabContainer.GetRelativePositionInContainerCoords(this.LocalMatrix.Translation);

            float yaw, pitch, roll;
            Matrix rot = this.LocalMatrix;
            rot.Translation = Vector3.Zero;

            MyUtils.RotationMatrixToYawPitchRoll(ref rot, out yaw, out pitch, out roll);
            objectBuilderWeapon.AnglesInContainer = new Vector3(yaw, pitch, roll);
            //end of duplicate from  MyPrefab

            objectBuilderWeapon.PrefabLargeWeaponType = weaponType;
            //objectBuilderWeapon.PrefabHealthRatio = Health;

            objectBuilderWeapon.UseProperties = UseProperties.GetObjectBuilder();
            objectBuilderWeapon.SearchingDistance = SearchingDistance;
            objectBuilderWeapon.AimingDistance = 0;

            return objectBuilderWeapon;
        }

        public override MyModel GetModelLod0()
        {
            //return m_gun == null ? null : m_gun.GetModelLod0();
            if (ModelLod0 == null) 
            {
                return m_gun == null ? null : m_gun.GetModelLod0();
            }
            return base.GetModelLod0();
        }

        public override MyModel GetModelLod1()
        {
            //return m_gun == null ? null : m_gun.GetModelLod1();
            if (ModelLod1 == null)
            {
                return m_gun == null ? null : m_gun.GetModelLod1();
            }
            return base.GetModelLod1();
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            // Nothing, visible parts are GunBase and Barrel
            return true;
        }

        public void DebugDrawRange() 
        {         /*
            Matrix world = Matrix.CreateWorld(WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up);
            Vector4 color = Color.Blue.ToVector4();
            color.W *= 0.1f;
            MySimpleObjectDraw.DrawTransparentSphere(ref world, SearchingDistance, ref color, true, 24);
                    */
                   
            float distance = 5000;
            float minSearchDistance = 5000;

            if ((Vector3.Distance(MyCamera.Position, this.GetPosition()) < distance)
                && (SearchingDistance < minSearchDistance))
            {
                Matrix world = Matrix.CreateWorld(WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up);
                Vector3 color = Color.Blue.ToVector3();
                MyDebugDraw.DrawSphereWireframe(world.Translation, SearchingDistance, color, 1f);
            }        
            
        }
                      /*
        protected override void Explode()
        {
            MyExplosion newExplosion = MyExplosions.AddExplosion();
            if (newExplosion != null)
            {
                BoundingSphere explosionSphere = WorldVolumeHr;
                explosionSphere.Radius *= m_config.ExplosionRadiusMultiplier;
                MyVoxelMap voxelMap = MyVoxelMaps.GetOverlappingWithSphere(ref explosionSphere);
                MyExplosionDebrisModel.CreateExplosionDebris(ref explosionSphere, MyGroupMask.Empty, this, voxelMap);
                newExplosion.Start(0, MyPrefabConstants.VOLUME_DAMAGE_MULTIPLIER * m_config.ExplosionDamageMultiplier * WorldVolumeHr.Radius, 0, m_config.ExplosionType, explosionSphere, MyExplosionsConstants.EXPLOSION_LIFESPAN);
            }
        }               */

        public override bool GetIntersectionWithLine(ref MyLine line, out MyIntersectionResultLineTriangleEx? t, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            return m_gun.GetIntersectionWithLine(ref line, out t);
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            return m_gun.GetIntersectionWithLine(ref line, out v, useCollisionModel);
        }

        public MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum PrefabLargeWeaponType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabLargeWeapon";
        }

        public MyGuiControlEntityUse GetGuiControl(IMyGuiControlsParent parent)
        {
            return new MyGuiControlPrefabLargeWeaponUse(parent, this);
        }

        public override string GetCorrectDisplayName()
        {
            string displayName = base.GetCorrectDisplayName();

            if (displayName == "Front Turret")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.FrontTurret).ToString();
            }

            if (displayName == "Back Turret")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.BackTurret).ToString();
            }

            if (displayName == "Bottom Turret")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.BottomTurret).ToString();
            }

            return displayName;
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

        public void Fire()
        {
            m_gun.Fire();
        }

        public void StopFire()
        {
            m_gun.StopFire();
        }

        public void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator, bool afterburner)
        {
            m_gun.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator, afterburner);
        }

        public Matrix GetViewMatrix()
        {
            return m_gun.GetViewMatrix();
        }

        public override Vector3 GetHUDMarkerPosition()
        {
            return m_gun.GetHUDMarkerPosition();
        }

        public bool IsDamagedForWarnignAlert()
        {
            return HealthRatio <= MyLargeShipWeaponsConstants.WARNING_DAMAGE_ALERT_LEVEL;
        }

        public MyLine GetLine()
        {
            return m_gun.GetBarell().GetLine();
        }

        public void StopAimingSound()
        {
            m_gun.StopAimingSound();
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            if (damageSource != null && damageSource == MySession.PlayerShip)
            {
                var factionRelation = MyFactions.GetFactionsRelation(damageSource.Faction, Faction);
                if ((factionRelation == MyFactionRelationEnum.Friend || factionRelation == MyFactionRelationEnum.Neutral) &&
                    !MyGuiScreenGamePlay.Static.IsCheatEnabled(MyGameplayCheatsEnum.FRIEND_NEUTRAL_CANT_DIE) &&
                    !MyFakes.INDESTRUCTIBLE_PREFABS)
                {
                    MySession.PlayerShip.AddFriendlyFireDamage(this, damage);
                    return;
                }
            }

            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
        }

        public void SetRotation(float angle, float elevation)
        {
            m_gun.SetRotation(angle, elevation);
        }        
    }
}
