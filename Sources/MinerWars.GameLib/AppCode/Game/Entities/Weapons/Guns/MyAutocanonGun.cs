using System;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    
    using Models;
    using TransparentGeometry;
    using SysUtils;
    using SysUtils.Utils;
    using Utils;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.TransparentGeometry.Particles;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.World.Global;

    class MyAutocanonGun : MySmallShipGunBase
    {
        const MySoundCuesEnum AUTOCANON_RELEASE = MySoundCuesEnum.WepAutocanonRel3d;
        const MySoundCuesEnum AUTOCANON_ATTACK = MySoundCuesEnum.WepAutocanon3Fire3d_nonLoop;
        const MySoundCuesEnum AUTOCANON_ATTACK_LOOP = MySoundCuesEnum.WepAutocanonFire3d;

        float m_rotationAngle;                          //  Actual rotation angle (not rotation speed) around Z axis
        int m_lastTimeShoot;                            //  When was this gun last time shooting

        float m_rotationTimeout;

        bool m_cannonMotorEndPlayed;

        //  Muzzle flash parameters, with random values at each shot
        float m_muzzleFlashLength;
        float m_muzzleFlashRadius;

        //  When gun fires too much, we start generating smokes at the muzzle
        int m_smokeLastTime;
        int m_smokesToGenerate;


        MyAutocannonBarrel m_barrel;
        Matrix m_barrelMatrix;

        MyParticleEffect m_smokeEffect;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.Autocannon_Base, MyMaterialType.METAL, parentObject, 
                position, forwardVector, upVector, objectBuilder);

            m_rotationAngle = MyMwcUtils.GetRandomRadian();
            m_lastTimeShoot = MyConstants.FAREST_TIME_IN_PAST;
            m_smokeLastTime = MyConstants.FAREST_TIME_IN_PAST;
            m_smokesToGenerate = 0;
            m_cannonMotorEndPlayed = true;
            m_rotationTimeout = (float)MyAutocanonConstants.ROTATION_TIMEOUT + MyMwcUtils.GetRandomFloat(-500, +500);

            m_barrelMatrix = ModelLod0.Dummies["BARREL_POSITION"].Matrix;
            m_barrel = new MyAutocannonBarrel();
            m_barrel.Init(null, m_barrelMatrix, this);
        }

        //  Every child of this base class must implement Shot() method, which shots projectile or missile.
        //  Method returns true if something was shot. False if not (because interval between two shots didn't pass)
        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (GetParentMinerShip() == null)
            {
                return false;
            }

            //  Allow shoting only at intervals
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) < MyAutocanonConstants.SHOT_INTERVAL_IN_MILISECONDS && !IsDummy) return false;
            //  Stop 'release cue' if playing
            MySoundCue? autocanonReleaseCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_RELEASE);
            if ((autocanonReleaseCue != null) && (autocanonReleaseCue.Value.IsPlaying == true))
            {
                autocanonReleaseCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            //  Angle of muzzle flash particle
            m_muzzleFlashLength = MyMwcUtils.GetRandomFloat(3, 4) * m_barrel.GetMuzzleSize();
            m_muzzleFlashRadius = MyMwcUtils.GetRandomFloat(1.8f, 2.2f) * m_barrel.GetMuzzleSize();

            //  Increase count of smokes to draw
            SmokesToGenerateIncrease();

            //Use looping cue only in playership
            MySoundCuesEnum attackCue = GetParentMinerShip() == MySession.PlayerShip ? AUTOCANON_ATTACK_LOOP : AUTOCANON_ATTACK;

            //  Start 'attack and loop' cue (shooting)
            MySoundCue? autocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(attackCue);
            if ((autocanonAttackLoopCue == null) || (autocanonAttackLoopCue.Value.IsPlaying == false))
            {
                //MyMwcLog.WriteLine("Adding new AUTOCANNON attack loop");
                GetParentMinerShip().UnifiedWeaponCueSet(
                    attackCue,
                    MyAudio.AddCue2dOr3d(GetParentMinerShip(), attackCue,
                    m_positionMuzzleInWorldSpace, WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity));
            }

            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic;
            if (usedAmmo != null) //TODO: bot fires without ammo
            {
                ammoType = usedAmmo.AmmoType;
            }

            MyAmmoProperties ammoProperties = MyAmmoConstants.GetAmmoProperties(ammoType);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyAutocannonGun.Shot add projectile");

            if(MyMwcFinalBuildConstants.ENABLE_TRAILER_SAVE)
            {
                MinerWars.AppCode.Game.Trailer.MyTrailerSave.UpdateGunShot(this.Parent, Trailer.MyTrailerGunsShotTypeEnum.PROJECTILE);
            }

            AddProjectile(ammoProperties, this);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            m_cannonMotorEndPlayed = false;
            m_lastTimeShoot = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            //  We shot one projectile
            return true;
        }

        public override void Close()
        {
            StopAllSounds();
            base.Close();
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            if (m_barrel != null)
            {
                m_positionMuzzleInWorldSpace = GetMuzzlePosition(m_barrel.GetMuzzlePosition());
                m_barrel.SetData(ref m_worldMatrixForRenderingFromCockpitView, m_rotationAngle);
            }
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            //  Cannon is rotating while shoting. After that, it will slow-down.
            float normalizedRotationSpeed = 1.0f - MathHelper.Clamp((float)(MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) / m_rotationTimeout, 0, 1);
            normalizedRotationSpeed = MathHelper.SmoothStep(0, 1, normalizedRotationSpeed);
            m_rotationAngle -= normalizedRotationSpeed * MyAutocanonConstants.ROTATION_SPEED_PER_SECOND * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            Matrix worldMatrix = this.WorldMatrix;

            m_positionMuzzleInWorldSpace = GetMuzzlePosition(m_barrel.GetMuzzlePosition());
            m_barrel.SetData(ref m_worldMatrixForRenderingFromCockpitView, m_rotationAngle);

            //  Handle 'motor loop and motor end' cues
            if ((m_cannonMotorEndPlayed == false) && ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot) > MyAutocanonConstants.SHOT_INTERVAL_IN_MILISECONDS))
            {
                //  Stop 'shooting loop' cue
                if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_lastTimeShoot + MyAutocanonConstants.MIN_TIME_RELEASE_INTERVAL_IN_MILISECONDS)
                {
                    MySoundCue? autocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_ATTACK_LOOP);
                    if ((autocanonAttackLoopCue != null) && (autocanonAttackLoopCue.Value.IsPlaying == true))
                    {
                        autocanonAttackLoopCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                    }
                    autocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_ATTACK);
                    if ((autocanonAttackLoopCue != null) && (autocanonAttackLoopCue.Value.IsPlaying == true))
                    {
                        autocanonAttackLoopCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                    }

                    //  Start 'release' cue
                    MySoundCue? autocanonReleaseCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_RELEASE);
                    if ((autocanonReleaseCue == null) || (autocanonReleaseCue.Value.IsPlaying == false))
                    {
                        GetParentMinerShip().UnifiedWeaponCueSet(
                            AUTOCANON_RELEASE,
                            MyAudio.AddCue2dOr3d(GetParentMinerShip(), AUTOCANON_RELEASE,
                                Parent.GetPosition(), Parent.WorldMatrix.Forward, Parent.WorldMatrix.Up,
                                Parent.Physics.LinearVelocity));
                    }

                    m_cannonMotorEndPlayed = true;
                }
            }

            //  Update sound position
            MySoundCue? updateAutocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_ATTACK_LOOP);
            if ((updateAutocanonAttackLoopCue != null) && (updateAutocanonAttackLoopCue.Value.IsPlaying == true))
            {
                MyAudio.UpdateCuePosition(updateAutocanonAttackLoopCue, m_positionMuzzleInWorldSpace, WorldMatrix.Forward,
                    WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
            updateAutocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_ATTACK);
            if ((updateAutocanonAttackLoopCue != null) && (updateAutocanonAttackLoopCue.Value.IsPlaying == true))
            {
                MyAudio.UpdateCuePosition(updateAutocanonAttackLoopCue, m_positionMuzzleInWorldSpace, WorldMatrix.Forward,
                    WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }

            //  Update sound position
            MySoundCue? updateAutocanonReleaseCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_RELEASE);
            if ((updateAutocanonReleaseCue != null) && (updateAutocanonReleaseCue.Value.IsPlaying == true))
            {
                MyAudio.UpdateCuePosition(updateAutocanonReleaseCue, m_positionMuzzleInWorldSpace, WorldMatrix.Forward,
                    WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }

            //  If gun fires too much, we start generating smokes at the muzzle
            if ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_smokeLastTime) >= (MyAutocanonConstants.SMOKES_INTERVAL_IN_MILISECONDS))
            {
                m_smokeLastTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;

                SmokesToGenerateDecrease();

                if (m_smokesToGenerate > 0 && m_smokeEffect == null)
                {
                    if (MyCamera.GetDistanceWithFOV(GetPosition()) < 150)
                    {
                        m_smokeEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Smoke_Autocannon);
                        m_smokeEffect.WorldMatrix = WorldMatrix;
                        m_smokeEffect.OnDelete += new EventHandler(m_smokeEffect_OnDelete);
                    }
                }
            }

            if (m_smokeEffect != null)
            {
                float smokeOffset = 0.2f;
                if ((MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == MinerWars.AppCode.Game.GUI.MyCameraAttachedToEnum.PlayerMinerShip) &&
                    (Parent == MySession.PlayerShip))
                {
                    smokeOffset = 0.0f;
                }

                m_smokeEffect.WorldMatrix = Matrix.CreateTranslation(m_positionMuzzleInWorldSpace + worldMatrix.Forward * smokeOffset);
                m_smokeEffect.UserBirthMultiplier = m_smokesToGenerate;
            }
        }

        void m_smokeEffect_OnDelete(object sender, EventArgs e)
        {
            m_smokeEffect = null;
        }


        //  Only for drawing this object, because some objects need to use special world matrix
        public Matrix GetWorldMatrixForDraw(Matrix matrix)
        {
            Matrix outMatrix;
            Matrix.Multiply(ref matrix, ref MyCamera.InversePositionTranslationMatrix, out outMatrix);
            return outMatrix;
        }


        //  Draw muzzle flash not matter if in frustum (it's because it's above the frustum)
        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;
            
            m_barrel.Draw();

            if (MyMinerGame.IsPaused())
                return false;

            //  Draw muzzle flash
            int deltaTime = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeShoot;
            if (deltaTime <= MyMachineGunConstants.MUZZLE_FLASH_MACHINE_GUN_LIFESPAN)
            {
                MyParticleEffects.GenerateMuzzleFlash(m_positionMuzzleInWorldSpace, WorldMatrix.Forward, m_muzzleFlashRadius, m_muzzleFlashLength, MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD_NEAR);
            }

            if (m_smokeEffect != null)
                m_smokeEffect.Near = MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD_NEAR;

            return true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null) 
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            }
            return objectBuilder;
        }

        void ClampSmokesToGenerate()
        {
            m_smokesToGenerate = MyMwcUtils.GetClampInt(m_smokesToGenerate, 0, MyAutocanonConstants.SMOKES_MAX);
        }

        void SmokesToGenerateIncrease()
        {
            m_smokesToGenerate += MyAutocanonConstants.SMOKE_INCREASE_PER_SHOT;
            ClampSmokesToGenerate();
        }

        void SmokesToGenerateDecrease()
        {
            m_smokesToGenerate -= MyAutocanonConstants.SMOKE_DECREASE;
            ClampSmokesToGenerate();
        }

        public override void StopAllSounds()
        {
            base.StopAllSounds();

            var minerShip = GetParentMinerShip();
            if (minerShip != null)
            {
                MySoundCue? autocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_ATTACK_LOOP);
                if ((autocanonAttackLoopCue != null) && (autocanonAttackLoopCue.Value.IsPlaying == true))
                {
                    autocanonAttackLoopCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
                autocanonAttackLoopCue = GetParentMinerShip().UnifiedWeaponCueGet(AUTOCANON_ATTACK);
                if ((autocanonAttackLoopCue != null) && (autocanonAttackLoopCue.Value.IsPlaying == true))
                {
                    autocanonAttackLoopCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
            }
        }
    }


    class MyAutocannonBarrel : MyEntity
    {
        float m_rotationAngle;                          //  Actual rotation angle (not rotation speed) around Z axis
        Matrix m_worldMatrixForRenderingFromCockpitView = Matrix.Identity;
        Matrix m_muzzleMatrix;

        public virtual void Init(StringBuilder hudLabelText, Matrix localMatrix, MyAutocanonGun parentObject)
        {
            base.Init(hudLabelText, MyModelsEnum.Autocannon_Barrel, null, parentObject, null, null);
            LocalMatrix = Matrix.CreateTranslation(localMatrix.Translation);

            m_frustumCheckBeforeDrawEnabled = false;
            m_muzzleMatrix = ModelLod0.Dummies["MUZZLE_PARTICLE"].Matrix;
            Save = false;
        }

        public void SetData(ref Matrix worldMatrixForRenderingFromCockpitView, float rotationAngle)
        {
            m_worldMatrixForRenderingFromCockpitView = worldMatrixForRenderingFromCockpitView;
            m_rotationAngle = rotationAngle;
        }

        public override Matrix GetWorldMatrixForDraw()
        {          
            Matrix cannonToDraw;
            
            if ((MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == MinerWars.AppCode.Game.GUI.MyCameraAttachedToEnum.PlayerMinerShip)
             &&  (this.Parent.Parent == MySession.PlayerShip))
            {
                if (MyFakes.MWBUILDER)
                {
                    float stepRatio = ((MySmallShip)Parent.Parent).GetStepRatio();
                    stepRatio = stepRatio * 2 - 1;
                    //Matrix rot = Matrix.CreateRotationY((float)System.Math.Sin((float)MyMinerGameDX.TotalGamePlayTimeInMilliseconds * 0.005f) * 0.6f);
                    Matrix rot = Matrix.CreateRotationY((float)System.Math.Sin(stepRatio * MathHelper.TwoPi * 0.5f) * 0.6f);

                    cannonToDraw = Matrix.CreateRotationZ(m_rotationAngle) * rot * LocalMatrix * m_worldMatrixForRenderingFromCockpitView;
                }
                else
                {
                    cannonToDraw = Matrix.CreateRotationZ(m_rotationAngle) * LocalMatrix * m_worldMatrixForRenderingFromCockpitView;
                }
            }
            else  
            {
                cannonToDraw = Matrix.CreateRotationZ(m_rotationAngle) * WorldMatrix;

                if (Parent == null)
                    return cannonToDraw;

                cannonToDraw = (Parent as MyAutocanonGun).GetWorldMatrixForDraw(cannonToDraw);
            }

            return cannonToDraw;
        }
        public override bool DebugDraw()
        {
            if (!base.DebugDraw())
                return false;

            if (MyMwcFinalBuildConstants.DrawHelperPrimitives)
            {
                MyDebugDraw.DrawSphereWireframe(GetPosition(), 0.1f, new Vector3(0, 1, 0), 1);
                MyDebugDraw.DrawAxis(WorldMatrix, 5, 1);
            }

            return true;
        }

        public Vector3 GetMuzzlePosition()
        {
            return (m_muzzleMatrix * WorldMatrix).Translation;
        }

        public float GetMuzzleSize()
        {
            return m_muzzleMatrix.Right.Length();
        }

        
    }
}