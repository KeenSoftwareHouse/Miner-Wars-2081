using System;
using System.Text;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    class MyPrefabKinematicPart : MyPrefabBase
    {
        static readonly float MIN_RADIAL_STATE_TRESHOLD = (float)Math.PI * 0.1f;
        static readonly float MAX_RADIAL_STATE_TRESHOLD = (float)Math.PI * 0.9f;
        static readonly float MIN_RADIAL_STATE = 0.0f;
        static readonly float MAX_RADIAL_STATE = (float)Math.PI;
        const int WAIT_TIME_TO_CLOSE = 2000;     // in ms
        const int WAIT_TIME_TO_OPEN = 0;     // in ms

        MyPrefabKinematic m_ownerKinematic;

        // TODO group mask used for avoiding collisions with debris from destroyed other parts. It does not work though.
        //MyGroupMask m_groupMask;

        Vector3 m_openLocalPosition;

        KinematicPrefabStateEnum m_direction;

        float m_radialState;       //<0;PI>, position  0 == closed, anything between is cos interpolated, PI == opened
        
        float m_openTime;
        float m_closeTime;

        float m_lastUpdateTime;
        float m_lastStateChangedTime;        

        bool m_looping;
        bool m_closeAfterFullOpen;
        
        bool m_playedCloseSound;
        bool m_playedOpenSound;
        // lets play sound for each part

        MySoundCue? m_loopSound = null;
        MySoundCue? m_startSound = null;
        MySoundCue? m_endSound = null;

        MySoundCuesEnum? m_loopSoundCue;
        MySoundCuesEnum? m_startSoundCue;
        MySoundCuesEnum? m_endSoundCue;        

        public MyPrefabKinematicPart(MyPrefabContainer owner) : base(owner) { }

        public void Init(MyPrefabKinematic owner, MyPrefabConfigurationKinematicPart config, float openTime, float closeTime, MyModelsEnum modelEnum, Matrix open, Matrix close, MyMaterialType matType, MySoundCuesEnum? loopSound, MySoundCuesEnum? startSound, MySoundCuesEnum? endSound/*, MyGroupMask grpMask*/, float? health, float? maxHealth, bool activated)
        {
            m_needsUpdate = config.NeedsUpdate;
            m_openTime = openTime * 1000;
            m_closeTime = closeTime * 1000;
            m_radialState = MAX_RADIAL_STATE;// 0;
            m_config = config;

            m_loopSoundCue = loopSound;
            m_startSoundCue = startSound;
            m_endSoundCue = endSound;

            m_openLocalPosition = open.Translation - close.Translation;
            
            m_looping = false;

            m_ownerKinematic = owner;
            m_lastUpdateTime = 0;

            MyMwcObjectBuilder_Base objbuilder = MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.PrefabKinematicPart, config.PrefabId);
            objbuilder.EntityId = MyEntityIdentifier.ToNullableInt(EntityId);

            StringBuilder hudLabelTextSb = null;

            base.Init(hudLabelTextSb, modelEnum, null, owner, null, objbuilder);
            if (maxHealth != null) 
            {
                MaxHealth = maxHealth.Value;
            }
            if (health != null)
            {
                Health = health.Value;
            }

            //LocalMatrix = Matrix.Invert(MyMath.NormalizeMatrix(close));
            LocalMatrix = Matrix.CreateTranslation(Vector3.Zero);

            InitTrianglePhysics(matType, 1.0f, ModelLod0, null, MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART);

            SetState(KinematicPrefabStateEnum.Sleeping);

            if (activated)
            {
                PersistentFlags &= ~MyPersistentEntityFlags.Deactivated;
            }
            else
            {
                PersistentFlags |= MyPersistentEntityFlags.Deactivated;
            }
            //StartClosing();
        }

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {            
        }

        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            m_lastUpdateTime = 0;
        }

        private void UpdateSoundPositions()
        {
            if (m_loopSound != null && m_loopSound.Value.IsPlaying)
                MyAudio.UpdateCuePosition(m_loopSound, m_ownerKinematic.WorldMatrix.Translation, m_ownerKinematic.WorldMatrix.Forward, m_ownerKinematic.WorldMatrix.Up, new Vector3(0, 0, 0));
            if (m_startSound != null && m_startSound.Value.IsPlaying)
                MyAudio.UpdateCuePosition(m_startSound, m_ownerKinematic.WorldMatrix.Translation, m_ownerKinematic.WorldMatrix.Forward, m_ownerKinematic.WorldMatrix.Up, new Vector3(0, 0, 0));
            if (m_endSound != null && m_endSound.Value.IsPlaying)
                MyAudio.UpdateCuePosition(m_endSound, m_ownerKinematic.WorldMatrix.Translation, m_ownerKinematic.WorldMatrix.Forward, m_ownerKinematic.WorldMatrix.Up, new Vector3(0, 0, 0));
        }

        public KinematicPrefabStateEnum GetState() 
        {
            return m_direction;
        }

        public override void Close()
        {
            StopAllSounds();            
            m_ownerKinematic.RemovePart(this);
            base.Close();
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            return base.Draw(renderObject);
        }
                  /*
        protected override void Explode()
        {
            if (WorldVolumeHr.Radius > MyExplosionsConstants.MIN_OBJECT_SIZE_TO_CAUSE_EXPLOSION_AND_CREATE_DEBRIS)
            {
                MyExplosion newExplosion = MyExplosions.AddExplosion();
                if (newExplosion != null)
                {
                    //float radius = MyMwcUtils.GetRandomFloat(MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MIN, MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MAX);
                    BoundingSphere explosionSphere = WorldVolumeHr;
                    explosionSphere.Radius *= m_config.ExplosionRadiusMultiplier;
                    //explosionSphere.Radius = MathHelper.Max(explosionSphere.Radius, MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MIN);
                    MyVoxelMap voxelMap = MyVoxelMaps.GetOverlappingWithSphere(ref explosionSphere);
                    MyExplosionDebrisModel.CreateExplosionDebris(ref explosionSphere, MyGroupMask.Empty, this, voxelMap);
                    newExplosion.Start(0, m_config.ExplosionDamageMultiplier * MyPrefabConstants.VOLUME_DAMAGE_MULTIPLIER * WorldVolumeHr.Radius, 0, m_config.ExplosionType, explosionSphere, MyExplosionsConstants.EXPLOSION_LIFESPAN);
                }
            }
            else
            {
                var effect = MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.Explosion_SmallPrefab);
                var positionInWorldSpace = WorldVolumeHr.Center;
                effect.WorldMatrix = Matrix.CreateTranslation(positionInWorldSpace);
            }
        }           */

        private void StopAllSounds()
        {

            if (m_loopSound != null && m_loopSound.Value.IsPlaying)
                m_loopSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            if (m_startSound != null && m_startSound.Value.IsPlaying)
                m_startSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            if (m_endSound != null && m_endSound.Value.IsPlaying)
                m_endSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);

            m_loopSound = null;
            m_endSound = null;
            m_startSound = null;
        }

        private void PlayStartSound(bool killOther)
        {

            if (m_endSound != null && m_endSound.Value.IsPlaying && killOther)
            {
                m_endSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_endSound = null;
            }
            if (m_loopSound != null && m_loopSound.Value.IsPlaying && killOther)
            {
                m_loopSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_loopSound = null;
            }
            if (m_startSound == null || !m_startSound.Value.IsPlaying)
                m_startSound = MyAudio.AddCue3D(m_startSoundCue.Value, m_ownerKinematic.WorldMatrix.Translation, m_ownerKinematic.WorldMatrix.Forward, m_ownerKinematic.WorldMatrix.Up, new Vector3(0, 0, 0));
        }

        private void PlayLoopingSound(bool killOther)
        {
            if (m_startSound != null && m_startSound.Value.IsPlaying && killOther)
            {
                m_startSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_startSound = null;
            }
            if (m_endSound != null && m_endSound.Value.IsPlaying && killOther)
            {
                m_endSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_endSound = null;
            }
            if (m_loopSound == null || !m_loopSound.Value.IsPlaying)
                m_loopSound = MyAudio.AddCue3D(m_loopSoundCue.Value, m_ownerKinematic.WorldMatrix.Translation, m_ownerKinematic.WorldMatrix.Forward, m_ownerKinematic.WorldMatrix.Up, new Vector3(0, 0, 0));
        }

        private void PlayEndSound(bool killOther)
        {
            if (m_loopSound != null && m_loopSound.Value.IsPlaying && killOther)
            {
                m_loopSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_loopSound = null;
            }
            if (m_startSound != null && m_startSound.Value.IsPlaying && killOther)
            {
                m_startSound.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_startSound = null;
            }
            if (m_endSound == null || !m_endSound.Value.IsPlaying)
                m_endSound = MyAudio.AddCue3D(m_endSoundCue.Value, m_ownerKinematic.WorldMatrix.Translation, m_ownerKinematic.WorldMatrix.Forward, m_ownerKinematic.WorldMatrix.Up, new Vector3(0, 0, 0));
        }

        public void StartOpening()
        {
            SetState(KinematicPrefabStateEnum.Opening);
            m_lastUpdateTime = time;
            m_closeAfterFullOpen = false;
        }

        public void StartClosing()
        {
            if (m_direction != KinematicPrefabStateEnum.Opening)
            {
                StartClosingNow();
            }
            else
            {
                m_closeAfterFullOpen = true;
            }
        }

        public void StartClosingNow() 
        {
            if (m_direction != KinematicPrefabStateEnum.Closing)
            {
                SetState(KinematicPrefabStateEnum.Closing);
                m_lastUpdateTime = time;
                m_closeAfterFullOpen = false;
            }
        }

        public void Sleep()
        {
            SetState(KinematicPrefabStateEnum.Sleeping);
            //StopAllSounds();            
        }

        private void SetState(KinematicPrefabStateEnum state) 
        {
            m_direction = state;
            m_lastStateChangedTime = time;
            RecheckNeedsUpdate();
        }

        public bool IsClosed() 
        {
            return m_radialState >= MAX_RADIAL_STATE;
        }

        public bool IsOpened() 
        {
            return m_radialState <= MIN_RADIAL_STATE;
        }

        private float time;
        protected override void UpdatePrefabAfterSimulation()
        {
            time += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

            float speed = 0; //(float)Math.PI / (float)m_periodTime;
            float deltaT = time - m_lastUpdateTime;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdateSoundPositions");
            UpdateSoundPositions();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PlayStartSound");
            if (m_direction == KinematicPrefabStateEnum.Opening && time - m_lastStateChangedTime >= WAIT_TIME_TO_OPEN)
            {
                speed = (float)Math.PI / (float)m_openTime;

                if (m_loopSoundCue != null && m_startSoundCue != null && m_endSoundCue != null)
                {
                    if (MAX_RADIAL_STATE > m_radialState && m_radialState > MIN_RADIAL_STATE_TRESHOLD)
                    {
                        PlayStartSound(true);
                        //play starting sound
                    }
                    if (MAX_RADIAL_STATE_TRESHOLD > m_radialState && m_radialState > MIN_RADIAL_STATE_TRESHOLD)
                    {
                        PlayLoopingSound(true);
                        //play looping sound
                    }
                    if (MIN_RADIAL_STATE_TRESHOLD > m_radialState && m_radialState > MIN_RADIAL_STATE)
                    {
                        PlayEndSound(true);
                        //end sound
                    }
                }
                else if (m_startSoundCue != null)
                {
                    if (!m_playedOpenSound)
                    {
                        PlayStartSound(true);
                        m_playedOpenSound = true;
                    }
                }
                m_playedCloseSound = false;
                m_radialState -= deltaT * speed;

                if (IsOpened())
                {                                              
                    m_radialState = MIN_RADIAL_STATE;
                    if (m_closeAfterFullOpen)
                    {
                        StartClosingNow();
                    }
                    else 
                    {
                        Sleep();
                    }
                }
            }
            if (m_direction == KinematicPrefabStateEnum.Closing && time - m_lastStateChangedTime >= WAIT_TIME_TO_CLOSE)
            {
                speed = (float)Math.PI / (float)m_closeTime;
                if (m_loopSoundCue != null && m_startSoundCue != null && m_endSoundCue != null)
                {
                    if (MIN_RADIAL_STATE < m_radialState && m_radialState < MIN_RADIAL_STATE_TRESHOLD)
                    {
                        PlayStartSound(true);
                        //play starting sound
                    }
                    if (MIN_RADIAL_STATE_TRESHOLD < m_radialState && m_radialState < MAX_RADIAL_STATE_TRESHOLD)
                    {
                        PlayLoopingSound(true);
                        //play looping sound
                    }
                    if (MAX_RADIAL_STATE_TRESHOLD < m_radialState && m_radialState < MAX_RADIAL_STATE)
                    {
                        PlayEndSound(true);
                        //end sound
                    }
                }
                else if (m_endSoundCue != null)
                {
                    if (!m_playedCloseSound)
                    {
                        PlayEndSound(true);
                        m_playedCloseSound = true;
                    }
                }
                m_playedOpenSound = false;
                m_radialState += deltaT * speed;

                if (IsClosed())
                {
                    Sleep();
                    m_radialState = MAX_RADIAL_STATE;
                }
            }

            

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("others");

            if (m_looping && m_direction == KinematicPrefabStateEnum.Closing && m_radialState >= MAX_RADIAL_STATE)
            {
                StartOpening();
            }

            if (m_looping && m_direction == KinematicPrefabStateEnum.Opening && m_radialState <= MIN_RADIAL_STATE)
            {
                StartClosing();
            }

            m_radialState = MathHelper.Clamp(m_radialState, MIN_RADIAL_STATE, MAX_RADIAL_STATE);
            m_lastUpdateTime = time;

            float relPos = (1 + (float)Math.Cos(m_radialState)) / 2.0f;
            Vector3 interpolatedPosition = m_openLocalPosition * relPos;
            this.LocalMatrix = Matrix.CreateTranslation(interpolatedPosition);

            base.UpdatePrefabAfterSimulation();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            if (damage > 0)
            {
                m_ownerKinematic.OrderOpenAndClose();
            }
            if (m_ownerKinematic.IsDestructible)
            {
                if (((MyPrefabConfigurationKinematicPart)m_config).m_damageType == DamageTypesEnum.SHARE_WITH_PARENT)
                {
                    m_ownerKinematic.DoDamage(playerDamage, damage, empDamage, damageType, ammoType, damageSource);
                    return;
                }

                base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabKinematicPart";
        }

        public override bool IsWorking()
        {
            return (m_ownerKinematic.IsWorking() || !m_ownerKinematic.Enabled) && m_direction != KinematicPrefabStateEnum.Sleeping;
        }

        protected override bool PrefabNeedsUpdateNow
        {
            get
            {
                return IsWorking();
            }
        }
    }
}
