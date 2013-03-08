using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;

using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Sessions;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.GUI;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Gameplay;
using SysUtils.Utils;


namespace MinerWars.AppCode.Game.Entities.Weapons
{
    class MyAmmoBase : MyEntity
    {
        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()

        protected Vector3 m_previousPosition; //last position 
        protected Vector3 m_origin; //start position 
        protected Vector3 m_initialVelocity; //starting velocity 
        protected MyEntityIdentifier m_ownerEntityID; //owner entity (can be closed during ammo lifetime)
        protected int m_elapsedMiliseconds;  //milliseconds from start
        protected int m_cascadeExplosionLevel; //to reduce the range of the cascaded explosions
        protected MyAmmoProperties m_ammoProperties;
        protected bool m_canByAffectedByExplosionForce = true;        
        //public bool IsDummy { get; set; }

        /// <summary>
        /// Need per frame position updates in multiplayer
        /// </summary>
        public bool GuidedInMultiplayer { get; set; }

        public int CascadedExplosionLevel { get { return m_cascadeExplosionLevel; } }

        /// <summary>
        /// (optional) Time to activate the ammo in milliseconeds, if applicable.
        /// Now used only in universal launcher shells, but can be extended to elsewhere.
        /// </summary>
        public int? TimeToActivate { get; set; }

        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType { get; set; }

        public MyAmmoBase()
            :base(false)
        {
            Save = false;
        }

        public virtual void Init(MyModelsEnum modelEnum, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoEnum, bool spherePhysics = true)
        {
            base.Init(null, modelEnum, null, null, null, null);

            AmmoType = ammoEnum;
            m_gameplayProperties = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)ammoEnum, Faction);
            m_ammoProperties = MyAmmoConstants.GetAmmoProperties(ammoEnum);

            //  Collision skin
            if (spherePhysics)
            {
                InitSpherePhysics(MyMaterialType.AMMO, ModelLod0, m_gameplayProperties.WeightPerUnit,
                                  MyPhysicsConfig.DefaultAngularDamping, MyConstants.COLLISION_LAYER_DEFAULT,
                                  RigidBodyFlag.RBF_DEFAULT);
            }
            else
            {
                InitBoxPhysics(MyMaterialType.AMMO, ModelLod0, m_gameplayProperties.WeightPerUnit,
                               MyPhysicsConfig.DefaultAngularDamping, MyConstants.COLLISION_LAYER_DEFAULT,
                               RigidBodyFlag.RBF_DEFAULT);
            }

            Physics.GetRBElementList()[0].Flags |= MyElementFlag.EF_MODEL_PREFER_LOD0;

            NeedsUpdate = true;
            RenderObjects[0].SkipIfTooSmall = false;
            CastShadows = false;
            Closed = true; //Because ammobase instance is going to pool. It is started by Start()

            IsDestructible = true;

            PreloadTextures();
        }

        public virtual void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner, StringBuilder displayName = null)
        {
            System.Diagnostics.Debug.Assert(Closed);
            Closed = false;
            GuidedInMultiplayer = false;

            if(this.EntityId.HasValue)
            {
                MyEntityIdentifier.RemoveEntity(this.EntityId.Value);
                this.EntityId = null;
            }

            if (this.NeedsId)
            {
                //if(owner.EntityId.HasValue) this.EntityId = MyEntityIdentifier.AllocateId(owner.EntityId.Value.PlayerId);
                //else this.EntityId = MyEntityIdentifier.AllocateId();
                this.EntityId = MyEntityIdentifier.AllocateId();
                MyEntityIdentifier.AddEntityWithId(this);
            }

            if (displayName != null && owner == MySession.PlayerShip)
            {
                DisplayName = displayName.ToString();
                MyHud.AddText(this, displayName, maxDistance: 1000);
            }

            m_isExploded = false;
            m_cascadeExplosionLevel = 0;
            m_origin = position;
            m_previousPosition = position;
            m_initialVelocity = initialVelocity;

            System.Diagnostics.Debug.Assert(owner.EntityId.HasValue, "Shooting entity must have ID");

            m_ownerEntityID = owner.EntityId.Value;
            m_elapsedMiliseconds = 0;



            Matrix ammoWorld = Matrix.CreateWorld(position, direction, owner.WorldMatrix.Up);

            SetWorldMatrix(ammoWorld);

            this.Physics.Clear();
            this.Physics.Enabled = true;
            this.Physics.LinearVelocity = initialVelocity;

            if (owner.Physics != null)
                this.Physics.GroupMask = owner.Physics.GroupMask;
            else
                this.Physics.GroupMask = MyGroupMask.Empty;

            //this.Physics.Enabled = true;

            this.Physics.ApplyImpulse(direction * this.Physics.Mass * impulseMultiplier, position);
            
            MyEntities.Add(this);
            NeedsUpdate = true;
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            m_elapsedMiliseconds += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
            m_previousPosition = this.WorldMatrix.Translation;
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();
            if (MyMultiplayerGameplay.IsRunning && GuidedInMultiplayer && !IsDummy)
            {
                MyMultiplayerGameplay.Static.UpdateAmmo(this);
            }
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (!base.Draw(renderObject))
                return false;

            if (ModelLod0 != null) ModelLod0.LoadInDraw();
            if (ModelLod1 != null) ModelLod1.LoadInDraw();

            return true;
        }

        public virtual void Explode()
        {
            //this.Physics.Enabled = false;
            //this.Physics.Clear();

            if (!IsDummy && !m_isExploded)
            {
                m_isExploded = true;
                if (MyMultiplayerGameplay.IsRunning)
                {
                    MyMultiplayerGameplay.Static.ExplodeAmmo(this);
                }
            }
        }

        public virtual void ExplodeCascade(int level)
        {
            m_cascadeExplosionLevel = level;
            Explode();
        }

        public bool CanBeAffectedByExplosionForce()
        {
            return m_canByAffectedByExplosionForce && !m_isExploded;
        }

        public MyEntity OwnerEntity
        {
            get 
            { 
                return MyEntities.GetEntityByIdOrNull(m_ownerEntityID); 
            }
        }

        public override void Close()
        {
            TimeToActivate = null;
            if (Physics.Enabled)
                Physics.Enabled = false;
            
            MyEntities.Remove(this);
            MyEntities.RemoveFromClosedEntities(this);

            CallAndClearOnClose();

            IsDummy = false;
            Closed = true;
            System.Diagnostics.Debug.Assert(!MyEntities.m_entitiesForUpdate.Contains(this));

            //Do not call base.Close because ammobase is always preallocated
            //base.Close();
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            if (MySession.Is25DSector)
            {
                // if (!(this is MySmallShipBot))
                {
                    if (this.Physics != null)
                    {
                        this.Physics.LinearVelocity = new Vector3(this.Physics.LinearVelocity.X, 0, this.Physics.LinearVelocity.Z);
                    }

                    SetPosition(new Vector3(GetPosition().X, 0, GetPosition().Z));
                }
            }
        }
    }
}
