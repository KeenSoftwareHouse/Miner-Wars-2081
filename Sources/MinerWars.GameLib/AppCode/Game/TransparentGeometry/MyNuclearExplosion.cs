#region Using
using System;
using System.Collections.Generic;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.CommonLIB.AppCode.Generics;
#endregion


namespace MinerWars.AppCode.Game.TransparentGeometry
{
    static class MyNuclearExplosion
    {
        class ExplosionLine
        {
            public int ActualTime;
            public Vector3 ActualDir;

            public int TotalTime;
            public Vector3 StartDir;
            public Vector3 EndDir;
        }

        enum NuclearState
        {
            INACTIVE,
            FADE_IN,
            FLASH,
            FLASH_IN,
            FADE_OUT,
        }

        public static event Action OnExplosionMax;
        public static event Action OnExplosionDone;

        static readonly float FADE_IN_TIME = 15000;
        static readonly float FLASH_TIME = 1000;
        static readonly float FLASH_IN_TIME = 5000;
        static readonly float FADE_OUT_TIME = 2000;
        static readonly float MAX_FLASH_INTENSITY = 10;

        static int m_StartMilliseconds;
        static float m_flashIntensity; //0-1
        static NuclearState m_State = NuclearState.INACTIVE;

        static Vector3 m_explosionCenter;
        static MySoundCue? m_burningCue;
        static float m_radius;
        static bool m_godRaysState;

        static MyObjectsPool<ExplosionLine> m_preallocatedExplosionLines = new MyObjectsPool<ExplosionLine>(20);   

        static MyNuclearExplosion()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.NuclearExplosion, "Nuclear explosion", Draw, MyRenderStage.PrepareForDraw, true);
        }

        public static void MakeExplosion(Vector3 explosionCenter)
        {
            m_explosionCenter = explosionCenter;

            if (m_State == NuclearState.INACTIVE || m_State == NuclearState.FLASH_IN)
            {
                m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_State = NuclearState.FADE_IN;

                m_burningCue = MyAudio.AddCue3D(MySoundCuesEnum.SfxSolarWind, m_explosionCenter, -MyCamera.ForwardVector, Vector3.Up, Vector3.Zero);
                MyAudio.AddCue3D(MySoundCuesEnum.SfxNuclearExplosion, m_explosionCenter, -MyCamera.ForwardVector, Vector3.Up, Vector3.Zero);
                m_radius = 100;

                MinerWars.AppCode.Game.TransparentGeometry.Particles.MyParticleEffect explosion = MinerWars.AppCode.Game.TransparentGeometry.Particles.MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_Huge);
                explosion.WorldMatrix = Matrix.CreateWorld(m_explosionCenter, MyCamera.ForwardVector, MyCamera.UpVector);
                explosion.UserScale = 3;
                explosion.AutoDelete = true;

                MyPostProcessGodRays godRays = (MyPostProcessGodRays)MyRender.GetPostProcess(MyPostProcessEnum.GodRays);
                m_godRaysState = godRays.Enabled;
                godRays.Enabled = true;
                MySector.GodRaysProperties.Enabled = true;
            }
            else
            if (m_State == NuclearState.FLASH)
            {
                m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }


        /// <summary>
        /// Render method is called directly by renderer. Depending on stage, post process can do various things 
        /// </summary>
        /// <param name="postProcessStage">Stage indicating in which part renderer currently is.</param>public override void RenderAfterBlendLights()
        public static void Draw()
        {
            switch (m_State)
            {
                case NuclearState.FADE_IN:
                    m_flashIntensity = (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_StartMilliseconds) / FADE_IN_TIME;
                    if (m_flashIntensity >= 1.0f)
                    {
                        m_State = NuclearState.FLASH;
                        m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    }
                    break;

                case NuclearState.FLASH:
                    m_flashIntensity = 1.0f;
                    if (MyMinerGame.TotalGamePlayTimeInMilliseconds > (m_StartMilliseconds + FLASH_TIME))
                    {
                        m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        m_State = NuclearState.FLASH_IN;
                    }
                    break;

                case NuclearState.FLASH_IN:
                    //m_flashIntensity = 1.0f - ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_StartMilliseconds) / 2000.0f/*MyFlashBombConstants.FADE_OUT_TIME*/);
                    m_flashIntensity = 1 + 10*(MyMinerGame.TotalGamePlayTimeInMilliseconds - m_StartMilliseconds) / FLASH_IN_TIME;
                    if (m_flashIntensity >= 11.0f)
                    {
                        if (m_burningCue != null && m_burningCue.Value.IsPlaying)
                        {
                            m_burningCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                        }

                        if (OnExplosionMax != null)
                        {
                            OnExplosionMax();
                        }

                        m_State = NuclearState.FADE_OUT;
                        m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    }
                    break;

                case NuclearState.FADE_OUT:
                    m_flashIntensity = 1 + MAX_FLASH_INTENSITY * (1.0f - ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_StartMilliseconds) /  FADE_OUT_TIME));
                    if (m_flashIntensity <= 0)
                    {
                        m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        m_State = NuclearState.INACTIVE;

                        MyPostProcessGodRays godRays = (MyPostProcessGodRays)MyRender.GetPostProcess(MyPostProcessEnum.GodRays);
                        godRays.Enabled = m_godRaysState;
                        MySector.GodRaysProperties.Enabled = false;

                        m_preallocatedExplosionLines.DeallocateAll();
                        m_flashIntensity = 0;

                        if (OnExplosionDone != null)
                            OnExplosionDone();
                    }
                    break;
            }

            if (m_State != NuclearState.INACTIVE)
            {
                MyRender.Sun.Direction = Vector3.Normalize(MyCamera.Position - m_explosionCenter);
                MyRender.Sun.Intensity = 10 * m_flashIntensity;
                MyRender.Sun.Color = Vector4.One;

                MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip.DoDamage(m_flashIntensity * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, 0, 0, MyDamageType.Radioactivity, MyAmmoType.None, null);
                MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip.IncreaseHeadShake(MathHelper.Clamp(2f * m_flashIntensity, 3, 10));
                
                UpdateExplosionLines();

                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.SunDisk, Vector4.One, m_explosionCenter, Math.Max(m_flashIntensity * 5000, 1), 0);

                foreach (LinkedListNode<ExplosionLine> explosionLine in m_preallocatedExplosionLines)
                {
                    MyTransparentGeometry.AddLineBillboard(MyTransparentMaterialEnum.Smoke_square_unlit, Vector4.One, m_explosionCenter, explosionLine.Value.ActualDir, 10000, 100);
                }

                MySector.GodRaysProperties.Exposition = 0.5f * m_flashIntensity; //0.077f
                MySector.GodRaysProperties.Weight = 2.5f * m_flashIntensity;      //1.27f
            }
        }

        static void UpdateExplosionLines()
        {
            foreach (LinkedListNode<ExplosionLine> explosionLine in m_preallocatedExplosionLines)
            {
                explosionLine.Value.ActualTime += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
                if (explosionLine.Value.ActualTime > explosionLine.Value.TotalTime)
                {
                    m_preallocatedExplosionLines.MarkForDeallocate(explosionLine);
                    continue;
                }

                explosionLine.Value.ActualDir = Vector3.Lerp(explosionLine.Value.StartDir, explosionLine.Value.EndDir, explosionLine.Value.ActualTime / (float)explosionLine.Value.TotalTime);
            }

            m_preallocatedExplosionLines.DeallocateAllMarked();
            if (m_State == NuclearState.FADE_IN && MyMwcUtils.GetRandomFloat(0, 1) > 0.75f)
            {
                ExplosionLine line = m_preallocatedExplosionLines.Allocate(true);
                if (line != null)
                {
                    line.TotalTime = 5000;
                    line.ActualTime = 0;
                    line.StartDir = MyMwcUtils.GetRandomVector3Normalized();

                    Vector3 rotDir = MyMwcUtils.GetRandomVector3Normalized();
                    Matrix rotMatrix = Matrix.CreateFromAxisAngle(rotDir, 0.3f);
                    line.EndDir = Vector3.Transform(line.StartDir, rotMatrix);
                }
            }
        }

        public static void Unload()
        {
            m_State = NuclearState.INACTIVE;
            m_preallocatedExplosionLines.DeallocateAll();
            m_flashIntensity = 0;
        }

    }
}
