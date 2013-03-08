using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWarsMath;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;

using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.Entities
{
    class MyMeteor : MyStaticAsteroid
    {
        private MyParticleEffect m_trailEffect;
        List<MyRBElement> m_collisionList = new List<MyRBElement>();
        private float m_size;
        
        int m_maxTime = 10000;
        int m_startTime;

        private MySoundCue? m_burningCue;

        private static List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> asteroids = new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>(5);

        public override void Init(StringBuilder displayName, Models.MyModelsEnum? modelLod0Enum, Models.MyModelsEnum? modelLod1Enum, MyEntity parentObject, float? scale, CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base objectBuilder, Models.MyModelsEnum? modelCollision = null, Models.MyModelsEnum? modelLod2Enum = null)
        {
            base.Init(displayName, modelLod0Enum, modelLod1Enum, parentObject, scale, objectBuilder, modelCollision, modelLod2Enum);

            this.CastShadows = false;
            this.NeedsUpdate = true;
            this.Save = false;
        }

        protected override void InitPhysics()
        {
           // this.InitBoxPhysics(MyMaterialType.ROCK, this.ModelLod0.BoundingBox.GetCenter(), this.ModelLod0.BoundingBoxSize * 0.65f, 10000000, 0, 0, RigidBodyFlag.RBF_DEFAULT);
           // this.InitTrianglePhysics(MyMaterialType.ROCK, 10000000, this.ModelLod1, this.ModelLod0, MyConstants.COLLISION_LAYER_METEOR);
            this.InitSpherePhysics(MyMaterialType.ROCK, this.ModelLod0, 10000000, 0, MyConstants.COLLISION_LAYER_METEOR, RigidBodyFlag.RBF_DEFAULT);

            this.Physics.CollisionLayer = MyConstants.COLLISION_LAYER_METEOR;

            this.Physics.AngularDamping = 0;
            this.Physics.LinearDamping = 0;
            this.Physics.MaxLinearVelocity = 5000;

        }

        public void Start(Vector3 direction, int? trailEffectId)
        {
            this.Physics.LinearVelocity = direction;
            this.Physics.AngularVelocity = new Vector3(MyMwcUtils.GetRandomFloat(0.2f, 1.5f), MyMwcUtils.GetRandomFloat(0.2f, 1.5f), 0);

            if (m_size == 0)
            {
                m_size = this.WorldVolume.Radius;
            }

            if (trailEffectId != null)
            {
                m_trailEffect = MyParticlesManager.CreateParticleEffect(trailEffectId.Value);
                m_trailEffect.AutoDelete = true;
                m_trailEffect.UserScale = this.WorldVolume.Radius / 10;
                m_trailEffect.UserBirthMultiplier /= 2;
                m_trailEffect.WorldMatrix = this.WorldMatrix;// worldMatrix;
            }

            m_burningCue = MyAudio.AddCue3D(MySoundCuesEnum.SfxMeteorFly, this.GetPosition(), this.GetForward(), Vector3.Up, direction);
            m_startTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            if (MyMultiplayerGameplay.IsHosting)
            {
                MyMultiplayerGameplay.Static.NewEntity(GetObjectBuilder(true), WorldMatrix);
            }
        }

        public override void Close()
        {
            if (m_trailEffect != null)
            {
                //MyParticlesManager.RemoveParticleEffect(m_trailEffect);
                m_trailEffect.Stop();
                m_trailEffect = null;
            }
            StopCue();
            base.Close();
        }

        private void StopCue()
        {
            if ((m_burningCue != null) && (m_burningCue.Value.IsPlaying == true))
            {
                m_burningCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
        }
        public override void UpdateBeforeSimulation()
        {
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_startTime + m_maxTime)
            {
                MarkForClose();
                return;
            }

            if (m_trailEffect != null)
            {
                m_trailEffect.WorldMatrix = Matrix.CreateWorld(this.WorldVolume.Center, m_trailEffect.WorldMatrix.Forward, m_trailEffect.WorldMatrix.Up);
            }

            if (m_burningCue != null)
            {
                MyAudio.UpdateCuePosition(m_burningCue, this.GetPosition(), this.GetForward(), Vector3.Up, Vector3.Zero);
            }
                           
            base.UpdateBeforeSimulation();
        }


        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            var collidedEntity = contactInfo.GetOtherEntity(this);

            if (!(collidedEntity is MyMeteor))
            {
                MyParticleEffect pe = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_Meteor);
                pe.WorldMatrix = this.WorldMatrix;
                pe.UserScale = this.m_size * 0.01f;


                MyExplosion newExplosion = MyExplosions.AddExplosion();
                MyExplosionInfo info = new MyExplosionInfo()
                {
                    PlayerDamage = 100,
                    Damage = 100,
                    EmpDamage = 0,
                    ExplosionType = MyExplosionTypeEnum.METEOR_EXPLOSION,
                    ExplosionSphere = new BoundingSphere(GetPosition(), m_size),
                    LifespanMiliseconds = MyExplosionsConstants.EXPLOSION_LIFESPAN,
                    ParticleScale = 1,
                    OwnerEntity = this,
                    HitEntity = collidedEntity,
                    ExplosionFlags = MyExplosionFlags.APPLY_FORCE_AND_DAMAGE | MyExplosionFlags.AFFECT_VOXELS | MyExplosionFlags.CREATE_DEBRIS /*| MyExplosionFlags.FORCE_DEBRIS*/,
                    PlaySound = true,
                    VoxelCutoutScale = 1,
                    VoxelExplosionCenter = GetPosition()
                };

                if (newExplosion != null)
                {
                    newExplosion.Start(ref info);
                }


                this.MarkForClose();
            }
        }



        public static MyMeteor GenerateMeteor(float sizeInMeters, Matrix worldMatrix, Vector3 position, MyMwcVoxelMaterialsEnum material)
        {
            int size = MySectorGenerator.FindAsteroidSize(sizeInMeters, MyMwcObjectBuilder_StaticAsteroid.AsteroidSizes);
            asteroids.Clear();
            MyMwcObjectBuilder_Meteor.GetAsteroids(size, MyStaticAsteroidTypeSetEnum.A, asteroids);
            int rndIndex = MyMwcUtils.GetRandomInt(0, asteroids.Count);

            var builder = new MyMwcObjectBuilder_Meteor(asteroids[rndIndex], material);
            builder.PositionAndOrientation.Position = position;
            builder.PositionAndOrientation.Forward = MyMwcUtils.GetRandomVector3Normalized();
            builder.PositionAndOrientation.Up = MyMwcUtils.GetRandomVector3Normalized();
            
            builder.UseModelTechnique = false;

            MyMeteor meteor = (MyMeteor)MyEntities.CreateFromObjectBuilderAndAdd(null, builder, worldMatrix);
            meteor.m_size = sizeInMeters;

            return meteor;
        }

        protected override CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            var meteorBuilder = (MyMwcObjectBuilder_Meteor)base.GetObjectBuilderInternal(getExactCopy);

            meteorBuilder.Direction = this.Physics.LinearVelocity;

            if (m_trailEffect != null)
            {
                meteorBuilder.EffectID = m_trailEffect.GetID();
            }

            return meteorBuilder;
        }       
    }
}
