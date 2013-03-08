using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    using System.Collections.Generic;
    using App;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    using Models;
    using Utils;
    using System;
    using KeenSoftwareHouse.Library.Trace;
    using SysUtils.Utils;
    using SysUtils;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.Managers.Session;

    //  This is instance of one debris phys object
    abstract class MyExplosionDebrisBase : MyEntity
    {
        //  This variables are persistent even betwen Start (because we don't need to change them)
        int m_maxLifeTimeInMiliseconds;
        Matrix m_initialOrientation;
        bool m_explosionType;

        //  Per-debris object randomization parameters, they influence how debris looks when rendered
        float m_randomizedDiffuseTextureColorMultiplier;

        //  This variables are changed during every Start() (when new object is allocated)
        int m_createdTime;


        public MyExplosionDebrisBase()
            : base(false)
        {
        }

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()
        public virtual void Init(MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, MyMaterialType materialType, float scale, List<MyRBElementDesc> collisionPrimitives, float mass)
        {
            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();

            base.Init(null, modelLod0Enum, modelLod1Enum, null, scale, null);

            m_maxLifeTimeInMiliseconds = MyMwcUtils.GetRandomInt(MyExplosionsConstants.EXPLOSION_DEBRIS_LIVING_MIN_IN_MILISECONDS, MyExplosionsConstants.EXPLOSION_DEBRIS_LIVING_MAX_IN_MILISECONDS);
            m_randomizedDiffuseTextureColorMultiplier = MyMwcUtils.GetRandomFloat(0.4f, 0.6f);
            m_initialOrientation = Matrix.CreateRotationX(MyMwcUtils.GetRandomRadian()) * Matrix.CreateRotationY(MyMwcUtils.GetRandomRadian()) * Matrix.CreateRotationZ(MyMwcUtils.GetRandomRadian());

            // create physics
            this.Physics = new MyPhysicsBody(this, mass, 0) { MaterialType = materialType };
            
            for (int i = 0; i < collisionPrimitives.Count; i++)
            {
                MyRBSphereElement sphereEl = (MyRBSphereElement)physobj.CreateRBElement(collisionPrimitives[i]);
                //
                sphereEl.Radius *= scale;
                this.Physics.AddElement(sphereEl, true);
            }
        }
        

        //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
        //  So don't initialize members here, do it in Start()
        protected virtual void Start(Vector3 position, float scale, MyGroupMask groupMask, bool explosionType)
        {
            Save = false;
            m_explosionType = explosionType;

            m_createdTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            Matrix worldMat = Matrix.Identity;//; Matrix.CreateScale(scale);
            Scale = scale;
            worldMat.Translation = position;

            SetWorldMatrix(worldMat);

            //  This is here because we are restarting the object after it was sleeping in object pool
            this.Physics.Clear();

            if (m_explosionType)
            {
                NeedsUpdate = true;
                this.Physics.PlayCollisionCueEnabled = true;
                //reset the physical radius of object!
                if ((this.Physics as MyPhysicsBody).GetRBElementList().Count > 0)
                {
                    MyRBElement element = (this.Physics as MyPhysicsBody).GetRBElementList()[0];
                    (element as MyRBSphereElement).Radius = this.ModelLod0.BoundingSphere.Radius * scale;
                }

                //this.Physics.Enabled = true;
                this.Physics.GroupMask = groupMask;
                this.Physics.AngularDamping = MySession.Is25DSector ? 0.5f : 0.1f;
                //this.Physics.Update();
            }
            else
                NeedsUpdate = false;
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (m_explosionType)
            {
                if (IsLiving() == false)
                {
                    Close();
                }
            }
        }

        //  This method must be called when this object dies or is removed
        //  E.g. it removes lights, sounds, etc
        public override void Close()
        {
            MyEntities.Remove(this);
            MyEntities.RemoveFromClosedEntities(this);

            //  This will move the object to default waiting position
            SetWorldMatrix(Matrix.Identity);
        }

        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            MyPhysicsBody ps1 = (MyPhysicsBody)contactInfo.m_RigidBody1.m_UserData;
            MyPhysicsBody ps2 = (MyPhysicsBody)contactInfo.m_RigidBody2.m_UserData;
            if (ps1.Entity is MyExplosionDebrisBase && ps2.Entity is MyExplosionDebrisBase)
            {
                MyCommonDebugUtils.AssertDebug(false);   
            }

            base.OnContactStart(contactInfo);
        }

        //  This method tells us how much this explosion debris lives. If 1.0, it isn't transparent. If between 1.0 and 0.0,
        //  it is alpha-blended. If 0.0, we remove it from buffer.
        bool IsLiving()
        {
            float deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_createdTime;
            return (deltaTime <= m_maxLifeTimeInMiliseconds);
        }

        public float GetRandomizedDiffuseTextureColorMultiplier()
        {
            return m_randomizedDiffuseTextureColorMultiplier;
        }

        public override bool IsSelectable()
        {
            return false;
        }

        public override bool IsSelectableAsChild()
        {
            return false;
        }
    }
}