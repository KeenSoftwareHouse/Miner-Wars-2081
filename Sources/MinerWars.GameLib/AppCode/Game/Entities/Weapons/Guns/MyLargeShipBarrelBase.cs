using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.Physics;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.TransparentGeometry;
    using MinerWars.AppCode.Game.HUD;
    using MinerWars.AppCode.Game.Editor;
    using System.Diagnostics;
    using MinerWars.AppCode.Game.Entities.SubObjects;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.Import;

    abstract class MyLargeShipBarrelBase : MyEntity
    {
        public MyEntityIdentifier? LastShotId;

        #region BARREL SETTINGS
        // used ammo type for this barrel:
        private MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum m_ammoType;

        private List<MyModelDummy> m_muzzleDummies = new List<MyModelDummy>();
        protected int m_activeMuzzle = 0;

        // time diffs:
        protected int m_lastTimeShoot;
        private int m_lastTimeSmooke;

        // Here because of the usability with the other types of barrels:
        public float BarrelElevationMin { get; protected set; } // is actually set to nice friendly angle..
        protected MyParticleEffect m_shotSmoke;

        List<MyRBElement> m_collisionBox = new List<MyRBElement>();            

        protected int m_smokeLastTime;             // Smoke time stamp
        protected int m_smokeToGenerate;           // How much moke to generate
        protected float m_muzzleFlashLength;        // Length of muzzle flash
        protected float m_muzzleFlashRadius;        // Radius of the muzzle flash

        #endregion

        public MyLargeShipBarrelBase()
        {            
            m_ammoType = 0;
            m_lastTimeShoot = 0;
            m_lastTimeSmooke = 0;
            //BarrelElevationMin = -0.507997036f;
            BarrelElevationMin = -0.6f;
        }


        public void Init(StringBuilder hudLabelText, MyModelsEnum? modelLod0Enum, MyModelsEnum? modelLod1Enum,
            Matrix localMatrix, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType, MyLargeShipGunBase parentObject)
        {
            base.Init(hudLabelText, modelLod0Enum, modelLod1Enum, parentObject, null, null);

            LocalMatrix = localMatrix;
            ((MyLargeShipGunBase)parentObject).InitializationBarrelMatrix = LocalMatrix;

            // Check for the dummy cubes for the muzzle flash positions:
            if (ModelLod0 != null)
            {
                // test for one value:
                StringBuilder sb = new StringBuilder();
                sb.Append(MyLargeShipWeaponsConstants.MUZZLE_FLASH_NAME_ONE);
                if (ModelLod0.Dummies.Count > 0)
                {
                    if (ModelLod0.Dummies.ContainsKey(sb.ToString()))
                    { // one muzzle flash value:
                        m_muzzleDummies.Add(ModelLod0.Dummies[sb.ToString()]);
                    }
                    else
                    {
                        // more muzzle flashes values:
                        int num = 0;
                        for (int i = 0; i < ModelLod0.Dummies.Count; ++i)
                        {
                            sb.Clear();
                            sb.Append(MyLargeShipWeaponsConstants.MUZZLE_FLASH_NAME_MODE);
                            sb.Append(i.ToString());

                            if (ModelLod0.Dummies.ContainsKey(sb.ToString()))
                            {
                                ++num;
                            }
                        }
                        for (int i = 0; i < ModelLod0.Dummies.Count; ++i)
                        {
                            sb.Clear();
                            sb.Append(MyLargeShipWeaponsConstants.MUZZLE_FLASH_NAME_MODE);
                            sb.Append(i.ToString());
                            if (ModelLod0.Dummies.ContainsKey(sb.ToString()))
                            {
                                m_muzzleDummies.Add(ModelLod0.Dummies[sb.ToString()]);
                            }
                        }
                    }
                }
            }

            //base.InitSpherePhysics(MyMaterialType.METAL, ModelLod0, 9999999.0f, 1.0f, MyConstants.COLLISION_LAYER_ALL, RigidBodyFlag.RBF_RBO_STATIC);
            if (this.Physics != null)
            {
                //this.Physics.Enabled = true;
                this.Physics.Update();
            }

            m_ammoType = ammoType;                        
            Save = false;
            //NeedsUpdate = true; //No, barrel is updated from parent
        }

        public override MyMwcObjectBuilder_FactionEnum Faction
        {
            get
            {
                return (Parent != null) ? Parent.Faction : MyMwcObjectBuilder_FactionEnum.None;
            }
            set
            {
                //  System.Diagnostics.Debug.Assert(false, "Prefab always takes faction from container");
            }
        }

        public float SearchingDistance { get { return GetWeaponBase().PrefabParent.SearchingDistance; } }        

        public override void OnWorldPositionChanged(object source)
        {
            bool tooFar = (Vector3.DistanceSquared(MyGuiScreenGamePlay.Static.ControlledEntity.GetPosition(), this.GetPosition()) > MyLargeShipWeaponsConstants.MAX_ROTATION_UPDATE_DISTANCE * MyLargeShipWeaponsConstants.MAX_ROTATION_UPDATE_DISTANCE);
            if (!tooFar)
            {
                base.OnWorldPositionChanged(source);
            }
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            base.Draw(renderObject);            
            /*if (!IsControlledByPlayer())
            {
                //return base.Draw();
            }
            else
            {
                //DrawCrossHair();
            }*/
            return true;
        }

        public override void Close()
        {
            base.Close();

            if (m_shotSmoke != null)
            {
                MyParticlesManager.RemoveParticleEffect(m_shotSmoke);
                m_shotSmoke = null;
            }
        }

        public virtual bool StartShooting()
        {
            if (IsControlledByPlayer())
            {
                return true;
            }

            return true;
            /* //do we need this?
            if (GetWeaponBase().IsActive())
            {
                // test if there isnt something in the way of the fire:
                MyEntity target = GetWeaponBase().GetTarget();

                Matrix worldMatrix = WorldMatrix;
                Vector3 effectivePosition = MyUtils.GetTransform(m_muzzleDummies[m_activeMuzzle].Matrix.Translation, ref worldMatrix);


                MyLine shootingRay = new MyLine(effectivePosition, target.WorldMatrix.Translation, true);
                var result = MyEntities.GetIntersectionWithLine(ref shootingRay, this, null, ignoreChilds: true);
                MyEntity collisionEntity = result.HasValue ? result.Value.PhysObject : null;

                if (collisionEntity == null)
                {
                    StopShooting();
                    return false;
                }

                MyEntity intersected = collisionEntity;
                while (intersected != null)
                {
                    if (intersected == target) 
                        return true;
                    intersected = intersected.Parent;
                }
            }       */

            StopShooting();
            return false;
        }

        public virtual void StopShooting()
        {
            GetWeaponBase().StopShootingSound();
        }

        protected MyLargeShipGunBase GetWeaponBase()
        {
            return (MyLargeShipGunBase)Parent;
        }

        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum GetAmmoType()
        {
            return m_ammoType;
        }        

        protected List<MyModelDummy> GetMuzzleFlashMatrix()
        {
            return m_muzzleDummies;
        }

        protected float GetDeviatedAngleByDamageRatio() 
        {
            MyPrefabLargeWeapon prefabLargeWeapon = GetWeaponBase().PrefabParent;
            if (MySession.PlayerShip != null &&
               MyFactions.GetFactionsRelation(prefabLargeWeapon, MySession.PlayerShip) == MyFactionRelationEnum.Enemy)
            {
                float degrees = (float)Math.Pow(120, prefabLargeWeapon.GetDamageRatio() * 1.5 - 1.2) * 4f;
                return MathHelper.ToRadians(degrees);
            }
            return 0f;
        }

        protected Vector3 GetDeviatedVector(MyAmmoProperties ammoProperties)
        {
            float deviateAngle = ammoProperties.DeviateAngle;

            //  Create one projectile - but deviate projectile direction be random angle
            if (((Parent as MyGunBase).IsThisGunFriendly() || MyGuiScreenGamePlay.Static.ControlledEntity == (Parent as MyLargeShipGunBase).PrefabParent))
            {
                deviateAngle += MyGameplayConstants.GameplayDifficultyProfile.DeviatingAnglePlayerOnEnemy;
            }
            else
            {
                deviateAngle += MyGameplayConstants.GameplayDifficultyProfile.DeviatingAngleEnemyLargeWeaponOnPlayer;
                deviateAngle += GetDeviatedAngleByDamageRatio();
            }

            return MyUtilRandomVector3ByDeviatingVector.GetRandom(WorldMatrix.Forward, deviateAngle);
        }

        protected void AddProjectile(MyAmmoProperties ammoProperties, Vector3 muzzlePosition)
        {
            float deviateAngle = ammoProperties.DeviateAngle;

            Vector3 projectileForwardVector = GetDeviatedVector(ammoProperties);

            Vector3 velocity = ((MyLargeShipGunBase)Parent).PrefabParent.Parent.Physics.LinearVelocity;
            MyProjectiles.Add(ammoProperties, Parent, muzzlePosition, velocity, projectileForwardVector, false, 1.0f, this, ((MyLargeShipGunBase)Parent).WeaponOwner);
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            Parent.DoDamage(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
        }

        public override float Health
        {
            get { return Parent.Health; }
            set
            {
                Parent.Health = value;
            }
        }


        private void DrawCrossHair()
        {            
            if (!MyHud.Visible)
            {
                return;
            }

            // Recompute crosshair size for zoom mode
            float crosshairSize = 1150;
            if (MyCamera.Zoom.GetZoomLevel() < 1)
            {
                crosshairSize = crosshairSize / (float)(Math.Tan(MyCamera.FieldOfView / 2) / Math.Tan(MyCamera.Zoom.GetFOV() / 2));
            }

            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.Crosshair, Vector4.One, WorldMatrix.Translation + WorldMatrix.Forward * 25000,
                WorldMatrix.Up, WorldMatrix.Right, crosshairSize, 1);
        }

        public bool IsControlledByPlayer()
        {
            return MyGuiScreenGamePlay.Static.IsControlledByPlayer((Parent as MyLargeShipGunBase).PrefabParent);
        }

        public override MyEntity GetBaseEntity()
        {
            return GetWeaponBase().PrefabParent;
        }

        public MyLine GetLine()
        {
            return new MyLine(WorldMatrix.Translation, WorldMatrix.Translation + WorldMatrix.Forward * 5000, true);
        }

        public abstract Matrix GetViewMatrix();

        protected void IncreaseSmoke()
        {
            m_smokeToGenerate += MyAutocanonConstants.SMOKE_INCREASE_PER_SHOT;
            m_smokeToGenerate = MyMwcUtils.GetClampInt(m_smokeToGenerate, 0, MyAutocanonConstants.SMOKES_MAX);
        }

        protected void DecreaseSmoke()
        {
            m_smokeToGenerate -= MyAutocanonConstants.SMOKE_DECREASE;
            m_smokeToGenerate = MyMwcUtils.GetClampInt(m_smokeToGenerate, 0, MyAutocanonConstants.SMOKES_MAX);
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            DecreaseSmoke();
        }

        public void RemoveSmoke()
        {
            m_smokeToGenerate = 0;
        }
    }
}
