using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Renders
{
    internal static class MyIceStorm
    {
        public const int MaxTimeMs = 40000;
        public const int FadeOutTimeMs = 6000;
        public const int FadeInTimeMs = 10000;
        private const int MaxSoundDistance = 200;
        public const float CenterBias = 8f;
        public const int MaxSparkCount = 8;
        public const float MaxAmbientVolume = 0.6f;
        public const float MaxSmokeAlpha = 0.5f;
        public const int SmokeCount = 200;
        public const int MaxIceCount = 20;
        public const int IceEveryMs = 1500;
        public const int SparkEveryMs = 1000;
        public const int SmokeSphereRadius = 5000;
        public static bool IsActive;
        private static MySoundCue? ambientSound;


        private static readonly List<SmokeParticle> smokeParticles = new List<SmokeParticle>();
        private static readonly List<IceParticle> iceParticles = new List<IceParticle>();
        private static readonly List<ElectricStorm> storms = new List<ElectricStorm>();


        private static int startTime;
        private static Vector3 sphereCenter;
        private static int lastUpdateMs;
        private static readonly Random random = new Random();

        private static readonly List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> asteroidTypes =
            new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>(10);

        static MyIceStorm()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.IceStormWind, "IceStorm wind", Draw, MyRenderStage.PrepareForDraw);
        }

        //  Random vector distributed over the circle about normal. 
        //  Returns random vector that always lies on circle
        public static Vector3 GetRandomVector3CircleNormalizedFixed(Vector3 normal)
        {
            return MyMwcUtils.GetRandomVector3Normalized()*normal;
        }

        public static void Draw()
        {
            if (!IsActive) return;

            // main smoke
            foreach (SmokeParticle part in smokeParticles)
            {
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Smoke, part.Color, part.Pos, 900,
                                                        part.Angle, 0, true);
            }

            float darkeningPhase;
            float dt;
            GetDarkeningPhase(out darkeningPhase, out dt);

            // small pieces of debris
            if (darkeningPhase > 0.2)
            {
                var color = new Vector4(1, 1, 1, darkeningPhase);

                for (int i = 0; i < 100; i++)
                {
                    Vector3 pos = MyCamera.Position +
                                  GetRandomVector3CircleNormalizedFixed(MyCamera.ForwardVector)*
                                  MyMwcUtils.GetRandomFloat(0, 500) + MyCamera.ForwardVector*20;
                    MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.particle_stone, color, pos,
                                                            MyMwcUtils.GetRandomFloat(0.008f, 0.05f),
                                                            MyMwcUtils.GetRandomRadian());
                }

                for (int i = 0; i < 100; i++)
                {
                    Vector3 pos = MyCamera.Position +
                                  GetRandomVector3CircleNormalizedFixed(MyCamera.ForwardVector)*
                                  MyMwcUtils.GetRandomFloat(0, 500) + MyCamera.ForwardVector*20;
                    MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Sparks_b, color, pos,
                                                            MyMwcUtils.GetRandomFloat(0.008f, 0.05f),
                                                            MyMwcUtils.GetRandomRadian());
                }
            }

            // storm aftersparks
            foreach (ElectricStorm storm in storms)
            {
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Sparks_a, new Vector4(1f, 1f, 1f, 1f),
                                                        storm.Position +
                                                        MyMwcUtils.GetRandomVector3Normalized()*
                                                        MyMwcUtils.GetRandomFloat(0, 50),
                                                        MyMwcUtils.GetRandomFloat(10, 20), MyMwcUtils.GetRandomRadian());
            }
        }


        private static void Clear()
        {
            IsActive = false;
            foreach (IceParticle icePart in iceParticles)
            {
                icePart.AsteroidEntity.MarkForClose();
                MyParticlesManager.RemoveParticleEffect(icePart.TrailEffect);
                StopCue(icePart.Sound);
            }
            iceParticles.Clear();
            smokeParticles.Clear();

            foreach (ElectricStorm storm in storms)
            {
                // MyParticlesManager.RemoveParticleEffect(storm.Effect); autoclear
                StopCue(storm.Sound);
            }

            storms.Clear();
            StopCue(ambientSound);
        }

        private static void StopCue(MySoundCue? cue)
        {
            if ((cue != null) && cue.Value.IsPlaying)
            {
                cue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
        }


        public static void Start()
        {
            Clear();

            IsActive = true;
            startTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            ambientSound = MyAudio.AddCue3D(MySoundCuesEnum.SfxSolarWind,
                                            MyCamera.Position + MyCamera.ForwardVector*MaxSoundDistance, Vector3.Forward,
                                            Vector3.Up, Vector3.Zero, 0);


            sphereCenter = MyCamera.Position + MyCamera.ForwardVector*400;


            for (int i = 0; i < SmokeCount; i++)
            {
                Vector3 pos = sphereCenter +
                              MyMwcUtils.GetRandomVector3Normalized()*MyMwcUtils.GetRandomFloat(0, SmokeSphereRadius);
                var smokePart = new SmokeParticle
                                    {
                                        Angle = MyMwcUtils.GetRandomRadian(),
                                        Color = Vector4.Zero,
                                        AngularVelocity = MyMwcUtils.GetRandomFloat(-0.15f, 0.15f),
                                        Pos = pos,
                                        Velocity =
                                            MyMwcUtils.GetRandomVector3Normalized()*MyMwcUtils.GetRandomFloat(0, 30f)
                                    };
                smokeParticles.Add(smokePart);
            }
        }

        public static void LoadContent()
        {
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyIceStorm.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            Clear();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyIceStorm.UnloadContent - END");
        }

        static List<IceParticle> m_iceList = new List<IceParticle>();

        public static void Update()
        {
            //  Update only if sun wind is active
            if (IsActive == false) return;

            float darkeningPhase;
            float dt;
            int relTime = GetDarkeningPhase(out darkeningPhase, out dt);

            if (relTime > MaxTimeMs)
            {
                Clear();
                return;
            }


            MyAudio.UpdateCuePosition(ambientSound,
                                      MyCamera.Position + MyCamera.ForwardVector*-MaxSoundDistance*(1 - darkeningPhase),
                                      MyCamera.ForwardVector, MyCamera.UpVector, Vector3.Zero);
            MyAudio.UpdateCueVolume(ambientSound, darkeningPhase*MaxAmbientVolume);

            // update smoke 
            foreach (SmokeParticle part in smokeParticles)
            {
                Vector3 toCamera = (MyCamera.Position - part.Pos);
                toCamera.Normalize();
                float alpha = darkeningPhase*MaxSmokeAlpha;
                part.Color = new Vector4(alpha, alpha, alpha, alpha);
                //part.Color.W = darkeningPhase;
                part.Pos += part.Velocity*dt + toCamera*CenterBias*dt;
                part.Angle += part.AngularVelocity*dt;
            }


            // remove old ice and sparks

            m_iceList.Clear();
            foreach (IceParticle particle in iceParticles)
            {
                if (particle.StartTime + 4000 < relTime)
                    m_iceList.Add(particle);
            }

            foreach (IceParticle ice in m_iceList)
            {
                ice.AsteroidEntity.MarkForClose();
                Debug.Assert(ice.TrailEffect != null, "ice.TrailEffect != null");
                ice.TrailEffect.Stop();
                ice.TrailEffect = null;
                StopCue(ice.Sound);
                iceParticles.Remove(ice);
            }

            int c = 0;
            while (c < storms.Count)
            {
                ElectricStorm storm = storms[c];
                if (storm.StartTime + 1500 < relTime)
                {
                    storms.RemoveAt(c);
                    continue;
                }
                c++;
            }



            // if its dark add new sparks and ice balls
            if (darkeningPhase >= 1)
            {
                if (storms.Count < MaxSparkCount && MyMwcUtils.GetRandomInt(SparkEveryMs) < dt*1000.0f)
                {
                    var storm = new ElectricStorm
                                    {
                                        Position =
                                            MyCamera.Position + MyCamera.ForwardVector*250 +
                                            MyMwcUtils.GetRandomVector3HemisphereNormalized(MyCamera.ForwardVector)*
                                            MyMwcUtils.GetRandomFloat(0, 300),
                                        StartTime = relTime,
                                        Effect =
                                            MyParticlesManager.CreateParticleEffect(
                                                (int) MyParticleEffectsIDEnum.Damage_Sparks),
                                    };
                    storm.Effect.WorldMatrix = Matrix.CreateTranslation(storm.Position);
                    storm.Effect.AutoDelete = true;
                    storm.Effect.UserScale = 2;
                    storm.Sound = MyAudio.AddCue2D(MySoundCuesEnum.SfxSpark);
                    storms.Add(storm);
                }


                if (iceParticles.Count < MaxIceCount && MyMwcUtils.GetRandomInt(IceEveryMs) < dt*1000.0f)
                {
                    Vector3 dir = MyMwcUtils.GetRandomVector3HemisphereNormalized(MyCamera.ForwardVector);
                    Vector3 pos = MyCamera.Position + MyCamera.ForwardVector*250 +
                                  MyMwcUtils.GetRandomVector3Normalized()*MyMwcUtils.GetRandomFloat(0, 200) +
                                  dir*MyMwcUtils.GetRandomFloat(0, 500);
                    MyMwcObjectBuilder_StaticAsteroid rockModel =
                        MySectorGenerator.GenerateStaticAsteroid(MyMwcUtils.GetRandomFloat(0.1f, 2f),
                                                                 MyStaticAsteroidTypeSetEnum.A,
                                                                 MyMwcVoxelMaterialsEnum.Ice_01, pos, random,
                                                                 asteroidTypes);
                    Matrix matrix = Matrix.CreateFromAxisAngle(MyMwcUtils.GetRandomVector3Normalized(),
                                                               MyMwcUtils.GetRandomFloat(0, MathHelper.Pi));
                    matrix.Translation = pos;
                    MyEntity asteroid = MyEntities.CreateFromObjectBuilderAndAdd(null, rockModel, matrix);
                    asteroid.Physics.Enabled = false;
                    asteroid.CastShadows = false;

                    MyParticleEffect effect =
                        MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.Smoke_CannonShot);
                    Vector3 velocity = -dir*MyMwcUtils.GetRandomInt(150, 400);
                    iceParticles.Add(new IceParticle
                                         {
                                             StartTime = relTime,
                                             Position = pos,
                                             Direction = -dir,
                                             AsteroidEntity = asteroid,
                                             TrailEffect = effect,
                                             RotAxis = MyMwcUtils.GetRandomVector3Normalized(),
                                             RotAngle = MyMwcUtils.GetRandomRadian(),
                                             AngularVelocity = MyMwcUtils.GetRandomFloat(0.2f, 10f),
                                             Velocity = velocity,
                                             Sound =
                                                 MyAudio.AddCue3D(MySoundCuesEnum.WepSniperHighFire2d, pos, dir,
                                                                  dir*-dir, velocity)
                                         });
                }
            }

            // update ice parts
            foreach (IceParticle particle in iceParticles)
            {
                particle.RotAngle += particle.AngularVelocity * dt;
                particle.Position += particle.Velocity * dt;
                Matrix matrix = Matrix.CreateFromAxisAngle(particle.RotAxis, particle.RotAngle);
                matrix.Translation = particle.Position;
                particle.AsteroidEntity.SetWorldMatrix(matrix);
                Matrix trans = Matrix.CreateTranslation(-particle.Direction * 10);
                particle.TrailEffect.WorldMatrix = matrix * trans;
                MyAudio.UpdateCuePosition(particle.Sound, particle.Position, particle.Direction,
                                          particle.Direction * -particle.Direction, particle.Velocity);
            }



            lastUpdateMs = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        private static int GetDarkeningPhase(out float darkeningPhase, out float dt)
        {
            int time = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            int relTime = time - startTime;
            dt = (time - (float) lastUpdateMs)/1000.0f;

            darkeningPhase = relTime/(float) FadeInTimeMs;
            if (relTime > MaxTimeMs - FadeOutTimeMs) darkeningPhase = (MaxTimeMs - relTime)/(float) FadeOutTimeMs;
            if (darkeningPhase > 1) darkeningPhase = 1;
            if (darkeningPhase < 0) darkeningPhase = 0;
            return relTime;
        }

        #region Nested type: ElectricStorm

        public class ElectricStorm
        {
            public MyParticleEffect Effect;
            public Vector3 Position;
            public MySoundCue? Sound;
            public int StartTime;
        }

        #endregion

        #region Nested type: IceParticle

        public class IceParticle
        {
            public float AngularVelocity;
            public MyEntity AsteroidEntity;
            public Vector3 Direction;
            public Vector3 Position;
            public float RotAngle;
            public Vector3 RotAxis;
            public MySoundCue? Sound;
            public int StartTime;
            public MyParticleEffect TrailEffect;
            public Vector3 Velocity;
        }

        #endregion

        #region Nested type: SmokeParticle

        public class SmokeParticle
        {
            public float Angle;
            public float AngularVelocity;
            public Vector4 Color;
            public Vector3 Pos;
            public Vector3 Velocity;
        }

        #endregion
    }
}