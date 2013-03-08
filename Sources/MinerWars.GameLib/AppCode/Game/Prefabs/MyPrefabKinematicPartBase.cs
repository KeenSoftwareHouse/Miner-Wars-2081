using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Utils;


namespace MinerWars.AppCode.Game.Prefabs
{
    abstract class MyPrefabKinematicPartBase : MyPrefabBase
    {
        protected float m_kinematicStart = 0.0f;     // in sec
        protected float m_kinematicEnd = 0.0f;       // in sec

        protected Matrix m_kinematicLocalMatrix;
        protected float m_kinematicVelocityMax;
        MyPrefabBase m_kinematicPartOwner;

        protected bool m_on;

        protected MySoundCue? m_loopSound = null;
        protected MySoundCue? m_loopDamagedSound = null;
        protected MySoundCue? m_startSound = null;
        protected MySoundCue? m_endSound = null;

        protected MySoundCuesEnum? m_loopSoundCue;
        protected MySoundCuesEnum? m_loopDamagedSoundCue;
        protected MySoundCuesEnum? m_startSoundCue;
        protected MySoundCuesEnum? m_endSoundCue;

        protected int m_lastUpdate;
        private int m_time;

        public MyPrefabKinematicPartBase(MyPrefabContainer owner)
            : base(owner)
        {
            Flags |= EntityFlags.NeedsId;
        }

        public void Init(MyPrefabBase kinematicPartOwner, MyMwcObjectBuilderTypeEnum prefabKinematicPartBuilderType, int? prefabKinematicPartBuilderId, MyModelsEnum modelEnum, MyMaterialType materialType, Matrix kinematicLocalMatrix, float kinematicVelocityMax, MySoundCuesEnum? loopSound, MySoundCuesEnum? loopDamagedSound, MySoundCuesEnum? startSound, MySoundCuesEnum? endSound, float kinematicStart, float kinematicEnd) 
        {
            m_needsUpdate = true;
            m_kinematicPartOwner = kinematicPartOwner;                        
            m_kinematicLocalMatrix = kinematicLocalMatrix;
            m_kinematicVelocityMax = kinematicVelocityMax;            
            m_kinematicStart = kinematicStart;
            m_kinematicEnd = kinematicEnd;

            m_loopSoundCue = loopSound;
            m_loopDamagedSoundCue = loopDamagedSound;
            m_startSoundCue = startSound;
            m_endSoundCue = endSound;

            MyMwcObjectBuilder_Base objbuilder = MyMwcObjectBuilder_Base.CreateNewObject(prefabKinematicPartBuilderType, prefabKinematicPartBuilderId);

            m_config = m_kinematicPartOwner.GetConfiguration();
            base.Init(null, modelEnum, null, kinematicPartOwner, null, objbuilder);            
            LocalMatrix = Matrix.Identity;

            MaterialIndex = m_kinematicPartOwner.MaterialIndex;

            
            //InitTrianglePhysics(materialType, 1.0f, ModelLod0, null, MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART);
            InitBoxPhysics(materialType, ModelLod0, 100, 0, MyConstants.COLLISION_LAYER_PREFAB_KINEMATIC_PART, RigidBodyFlag.RBF_RBO_STATIC);
            Physics.Enabled = true;
            
            m_kinematicPartOwner.OnPositionChanged += OwnerPositionChanged;
        }

        void OwnerPositionChanged(object sender, EventArgs eventArgs)
        {
            if (m_loopSound != null && m_loopSound.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(m_loopSound, m_kinematicPartOwner.WorldMatrix.Translation, Vector3.Forward, Vector3.Forward, Vector3.Zero);
            }

            if (m_loopDamagedSound != null && m_loopDamagedSound.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(m_loopDamagedSound, m_kinematicPartOwner.WorldMatrix.Translation, Vector3.Forward, Vector3.Forward, Vector3.Zero);
            }
        }

        protected override void SetHudMarker() 
        {
        }

        public bool On 
        {
            get { return m_on; }
            set 
            { 
                m_on = value;
                m_lastUpdate = m_time;
                RecheckNeedsUpdate();
            }
        }

        protected float KinematicVelocityMaxAbs 
        {
            get { return Math.Abs(m_kinematicVelocityMax); }
        }

        protected abstract bool IsKinematicActive();

        protected abstract void UpdateAfterSimulationKinematic(float dt);

        protected override void UpdatePrefabAfterSimulation()
        {
            m_time += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

            float dt = (m_time - m_lastUpdate) / 1000f;                        
            UpdateAfterSimulationKinematic(dt);
            m_lastUpdate = m_time;

            base.UpdatePrefabAfterSimulation();
        }

        protected void StopKinematicSounds(MySoundCue? except = null)
        {
            StopSound(ref m_startSound, except);
            StopSound(ref m_loopSound, except);
            StopSound(ref m_loopDamagedSound, except);
            StopSound(ref m_endSound, except);
        }

        protected void StopSound(ref MySoundCue? soundToStop, MySoundCue? except) 
        {
            bool stopSound = soundToStop != null && (except == null || except.Value.IsSame(soundToStop.Value));
            if (stopSound) 
            {
                if (soundToStop.Value.IsPlaying) 
                {
                    soundToStop.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);                    
                }
                soundToStop = null;
            }
        }

        protected void PlayKinematicStartSound()
        {
            if (m_startSound == null || !m_startSound.Value.IsPlaying)
            {
                if (m_startSoundCue.HasValue)
                {
                    m_startSound = MyAudio.AddCue3D(m_startSoundCue.Value, m_kinematicPartOwner.WorldMatrix.Translation, m_kinematicPartOwner.WorldMatrix.Forward, m_kinematicPartOwner.WorldMatrix.Up, new Vector3(0, 0, 0));
                }
            }
            StopKinematicSounds(m_startSound);
        }

        bool m_starting;
        bool m_playing;

        protected void PlayKinematicLoopingSound()
        {
            if (m_kinematicPartOwner.HealthRatio > MyPrefabConstants.DAMAGED_HEALTH || !HasSoundForDamagedState)
            {
                PlayLoopSound(ref m_loopSound, m_loopSoundCue);
                StopKinematicSounds(m_loopSound);
                MyAudio.UpdateCuePosition(m_loopSound, this.GetPosition(), Vector3.Forward, Vector3.Up, Vector3.Zero);
            }
            else
            {
                PlayLoopSound(ref m_loopDamagedSound, m_loopDamagedSoundCue);
                StopKinematicSounds(m_loopDamagedSound);
                MyAudio.UpdateCuePosition(m_loopSound, this.GetPosition(), Vector3.Forward, Vector3.Up, Vector3.Zero);
            }
        }

        bool HasSoundForDamagedState
        {
            get { return m_loopDamagedSoundCue != null; }
        }

        void PlayLoopSound(ref MySoundCue? sound, MySoundCuesEnum? soundCue)
        {
            if (soundCue != null)
            {
                if (m_starting && sound.HasValue && sound.Value.IsPlaying)
                {
                    m_starting = false;
                    m_playing = true;
                }
                if (sound == null || (m_playing && !sound.Value.IsPlaying))
                {
                    m_playing = false;

                    sound = MyAudio.AddCue3D(
                        soundCue.Value,
                        m_kinematicPartOwner.WorldMatrix.Translation,
                        Vector3.Forward,
                        Vector3.Forward,
                        Vector3.Zero);

                    m_starting = sound != null;
                }
            }
        }

        protected void PlayKinematicEndSound()
        {
            if (m_endSound == null || !m_endSound.Value.IsPlaying)
            {
                if (m_endSoundCue.HasValue)
                {
                    m_endSound = MyAudio.AddCue3D(m_endSoundCue.Value, m_kinematicPartOwner.WorldMatrix.Translation, m_kinematicPartOwner.WorldMatrix.Forward, m_kinematicPartOwner.WorldMatrix.Up, new Vector3(0, 0, 0));
                }
            }
            StopKinematicSounds(m_endSound);
        }        

        public override void Close()
        {
            StopKinematicSounds();
            m_kinematicPartOwner.RemoveKinematicPart(this);
            base.Close();
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            if (Parent != null) //Because in objects got for explosion damage can be parent and children together
            {
                Parent.DoDamage(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
            }
        }

        public override bool IsWorking()
        {
            return IsKinematicActive() || On;
        }

        protected override bool PrefabNeedsUpdateNow
        {
            get
            {
                return IsWorking();
            }
        }

        public override MyEntity GetBaseEntity()
        {
            return m_kinematicPartOwner;
        }
    }
}
