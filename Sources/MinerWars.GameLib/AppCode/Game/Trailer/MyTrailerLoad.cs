using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using SysUtils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Managers.Session;
using System.Linq;

//  This class runs whole trailer (loads maps, object paths, shots, etc)

namespace MinerWars.AppCode.Game.Trailer
{
    public enum MyTrailerGunsShotTypeEnum : byte
    {
        PROJECTILE,
        MISSILE,
        MISSILE_GUIDED,
        HARVESTING_ORE,
        DRILLING_DEVICE
    }

    static class MyTrailerLoad
    {
        public static MyTrailerXmlAnimation TrailerAnimation = null;              //  Reference to trailer animation - load world based on it if trailer active
        public static MyTrailerXmlAnimation[] Animations;
        public static bool AnimationSelectedFromMenu;
        //  Every object we track stores here its per-frame data
        static Dictionary<MyEntity, Dictionary<int, MyPhysObjectTrackedTickData>> m_attachedPhysObjects;

        static int m_activeTick;
        static int m_fromTick;
        static int m_toTick;
        static bool m_isEnabled;

        static readonly int m_fadeInOutInTicks = MillisecondsToTick(4000);


        public static void LoadAnimation()
        {
            MyMwcLog.WriteLine("MyTrailerLoad.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyTrailerLoad::LoadAnimation");

            if (!AnimationSelectedFromMenu)
            {
                MyTrailerXml res = LoadTrailerXml();
                Animations = res.Animation;

                if (MyGuiScreenGamePlay.Static.IsMainMenuActive())
                {
                    SetTrailerAnimation(res.MainMenuAnimation);
                }
                else if (MyGuiScreenGamePlay.Static.IsGameActive())
                {
                    SetTrailerAnimation(res.GameAnimation);
                }
                else if (MyGuiScreenGamePlay.Static.IsFlyThroughActive())
                {
                    if (string.IsNullOrEmpty(res.FlyThroughOrCreditsAnimation))
                    {
                        //  Pick random animation
                        if ((res.Animation != null) && (Animations.Length > 0))
                        {
                            TrailerAnimation = res.Animation[MyMwcUtils.GetRandomInt(0, Animations.Length)];
                        }
                    }
                    else
                    {
                        SetTrailerAnimation(res.FlyThroughOrCreditsAnimation);
                    }
                }
            }
            AnimationSelectedFromMenu = false;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyTrailerLoad.LoadContent() - END");
        }

        static void SetTrailerAnimation(string animationToPlay)
        {
            if (string.IsNullOrEmpty(animationToPlay))
            {
                TrailerAnimation = null;
            }
            else
            {
                //  Find animation by name
                if (Animations != null)
                {
                    foreach (MyTrailerXmlAnimation tempAnimation in Animations)
                    {
                        if (tempAnimation.Name == animationToPlay)
                        {
                            TrailerAnimation = tempAnimation;
                        }
                    }
                }
            }
        }

        public static void LoadFromUserFolder(out string nextFreeName, out Matrix? lastShipPos)
        {
            if (m_attachedPhysObjects == null) // When triler was not loaded
            {
                m_attachedPhysObjects = new Dictionary<MyEntity, Dictionary<int, MyPhysObjectTrackedTickData>>();
            }

            foreach (var obj in m_attachedPhysObjects)
            {
                var entity = obj.Key;
                if (MyEntities.GetEntities().Contains(entity))
                {
                    obj.Key.MarkForClose();
                }
            }
            m_attachedPhysObjects.Clear();

            lastShipPos = null;

            var path = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Trailer");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            int maxFileName = 0;

            var files = Directory.GetFiles(path, "*.tracked");
            for (int i = 0; i < files.Length; i++)
            {
                string fileWithoutExt = Path.GetFileNameWithoutExtension(files[i]);
                int num;
                if (int.TryParse(fileWithoutExt, out num) && num > maxFileName)
                {
                    maxFileName = num;
                }

                var smallShip = MyGuiScreenGamePlay.Static.CreateFakeMinerShip(null, MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D.MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, new Vector3((i * 100) + 10000, (i * 100) + 10000, (i * 100) + 10000), false, 1.0f);
                AttachPhysObjectFullPath(smallShip, files[i]);
            }

            if(m_attachedPhysObjects.Count > 0)
            {
                var firstTick = m_attachedPhysObjects.Values.Last().First().Value;
                var m = firstTick.Orientation;
                m.Translation = firstTick.Position;
                lastShipPos = m;
            }

            m_isEnabled = true;

            m_fromTick = MillisecondsToTick(0); // start now
            m_toTick = MillisecondsToTick(1000 * 1000); // max length 20min
            m_activeTick = m_fromTick;

            nextFreeName = (maxFileName + 1).ToString("D3");
        }

        //  Trailer will play from specified time (fromMillisecond) to specified time (toMillisecond) and then rewind again to beggining (fromMillisecond)
        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyTrailerLoad.LoadContent - START");
            MyMwcLog.IncreaseIndent();

            m_isEnabled = (TrailerAnimation != null) && 
                (
                (MyGuiScreenGamePlay.Static.IsFlyThroughActive()) || 
                (MyGuiScreenGamePlay.Static.IsMainMenuActive()) || 
                ((MyGuiScreenGamePlay.Static.IsGameActive()) && (MyMwcFinalBuildConstants.ENABLE_TRAILER_ANIMATION_IN_GAMEPLAY_SCREEN))
                );
            MyMwcLog.WriteLine("m_isEnabled: " + m_isEnabled.ToString(), LoggingOptions.TRAILERS);
            if (m_isEnabled == false) return;

            if (m_isEnabled == true)
            {
                MyMwcLog.WriteLine("Animation Name: " + TrailerAnimation.Name, LoggingOptions.TRAILERS);

                if (TrailerAnimation == null)
                {
                    //  If we didn't find choosen animation we can't load trailer
                    m_isEnabled = false;
                }
                else
                {
                    m_fromTick = MillisecondsToTick(TrailerAnimation.TimeStartInMilliseconds);
                    m_toTick = MillisecondsToTick(TrailerAnimation.TimeEndInMilliseconds);

                    m_activeTick = m_fromTick;

                    //  Add ships
                    if (TrailerAnimation.Ship == null)
                    {
                        //  If we didn't find choosen animation we can't load trailer
                        m_isEnabled = false;
                    }
                    else
                    {
                        m_attachedPhysObjects = new Dictionary<MyEntity, Dictionary<int, MyPhysObjectTrackedTickData>>();
                        for (int i = 0; i < TrailerAnimation.Ship.Length; i++)
                        {
                            MyTrailerXmlAnimationShip ship = TrailerAnimation.Ship[i];

                            MySmallShip smallShip;
                            if (((MyGuiScreenGamePlay.Static.IsFlyThroughActive()) || (MyGuiScreenGamePlay.Static.IsMainMenuActive())) && (i == (TrailerAnimation.Ship.Length - 1)))
                            {
                                //  In fly-through, last loaded ship is always player's ship, so it serves as camera
                                smallShip = MySession.PlayerShip;
                                if (smallShip.Light != null)
                                    smallShip.Light.LightOn = false;
                            }
                            else
                            {
                                //smallShip = MyGuiScreenGamePlay.Static.CreateFakeMinerShip("TrailerShip " + i.ToString(), MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D.MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, new Vector3((i * 100) + 10000, (i * 100) + 10000, (i * 100) + 10000), false, 1.0f);
                                smallShip = MyGuiScreenGamePlay.Static.CreateFakeMinerShip(null, MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D.MyMwcObjectBuilder_SmallShip_TypesEnum.GETTYSBURG, new Vector3((i * 100) + 10000, (i * 100) + 10000, (i * 100) + 10000), false, 1.0f);
                                //smallShip.Light.ReflectorEnabled = true;
                            }

                            

                            AttachPhysObject(smallShip, ship.Filepath);
                        }
                    }
                }
            }


            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyTrailerLoad.LoadContent - END");
        }

        static MyTrailerXml LoadTrailerXml()
        {
            MyTrailerXml res = null;
            StreamReader str = null;

            try
            {
                //  Filename ends with "xmlx" because "xml" is already used by XNA content pipeline
                str = new StreamReader(MyMinerGame.Static.RootDirectory + "\\Trailer\\Trailer.xmlx");
                System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(MyTrailerXml));
                res = (MyTrailerXml)xSerializer.Deserialize(str);
            }
            catch (Exception ex)
            {
                //  Log this exception, but game will continue. Of course no trailer animation can run.
                MyMwcLog.WriteLine("Exception during reading and deserializing xml: " + ex.ToString());
                m_isEnabled = false;
            }
            finally
            {
                if (str != null) str.Close();
            }

            return res;
        }

        public static bool IsEnabled()
        {
            return m_isEnabled;
        }

        static int MillisecondsToTick(int millisecond)
        {
            return (millisecond * (int)MyConstants.PHYSICS_STEPS_PER_SECOND) / 1000;
        }

        static int TicksToMilliseconds(int ticks)
        {
            return (ticks * 1000) / (int)MyConstants.PHYSICS_STEPS_PER_SECOND;
        }

        //  Fading in/out at the beginning and at the end of trailer
        public static float GetBackgroundFadeAlpha()
        {
            MyCommonDebugUtils.AssertDebug(m_isEnabled == true);

            int fromFade = m_fromTick + m_fadeInOutInTicks;
            int toFade = m_toTick - m_fadeInOutInTicks;

            if (m_activeTick <= fromFade)
            {
                return (float)(fromFade - m_activeTick) / (float)m_fadeInOutInTicks;
            }
            else if (m_activeTick >= toFade)
            {
                return (float)(m_activeTick - toFade) / (float)m_fadeInOutInTicks;
            }
            else
            {
                return 0;
            }
        }

        //  This must be called as last thing in main update method
        static void IncreaseActiveTick()
        {
            m_activeTick++;

            if (m_activeTick >= m_toTick)
            {
                m_activeTick = m_fromTick;
            }
        }

        static void AttachPhysObjectFullPath(MyEntity physObject, string fullPath)
        {
            MyMwcLog.WriteLine("MyTrailedLoad.AttachPhysObject - START", LoggingOptions.TRAILERS);
            MyMwcLog.IncreaseIndent(LoggingOptions.TRAILERS);

            Dictionary<int, MyPhysObjectTrackedTickData> ticks = new Dictionary<int, MyPhysObjectTrackedTickData>(MyTrailerConstants.MAX_TRACKED_TICKS);

            MyMwcLog.WriteLine("File: " + fullPath, LoggingOptions.TRAILERS);

            //  Open file, BinaryReader must use ASCII encoding, otherwise PeekChar will have problems when trying to convert some values to chars
            using (FileStream fs = File.OpenRead(fullPath))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.ASCII))
                {
                    int tickIndex = 0;
                    while (br.PeekChar() != -1)
                    {
                        MyPhysObjectTrackedTickData tickData = new MyPhysObjectTrackedTickData();

                        MyUtils.BinaryRead(br, ref tickData.Position);
                        MyUtils.BinaryRead(br, ref tickData.Orientation);
                        tickData.ReflectorLevel = br.ReadSingle();
                        tickData.EngineLevel = br.ReadSingle();

                        int gunShot = br.ReadInt32();
                        if (gunShot == -1)
                        {
                            tickData.GunShot = null;
                        }
                        else
                        {
                            tickData.GunShot = (MyTrailerGunsShotTypeEnum)gunShot;
                        }

                        ticks.Add(tickIndex, tickData);
                        tickIndex++;
                    }

                    MyMwcLog.WriteLine("Ticks: " + ticks.Count, LoggingOptions.TRAILERS);
                    MyMwcLog.WriteLine("Milliseconds: " + TicksToMilliseconds(ticks.Count), LoggingOptions.TRAILERS);
                }
            }

            m_attachedPhysObjects.Add(physObject, ticks);

            MyMwcLog.DecreaseIndent(LoggingOptions.TRAILERS);
            MyMwcLog.WriteLine("MyTrailedLoad.AttachPhysObject - END", LoggingOptions.TRAILERS);
        }

        //  Attachs phys object to tracked data from a file, so this class updates position/orientation and even shots exactly as object in the file did 
        static void AttachPhysObject(MyEntity physObject, string file)
        {
            AttachPhysObjectFullPath(physObject, MyMinerGame.Static.RootDirectory + "\\Trailer\\" + file);
        }

        public static void Update()
        {
            if (m_isEnabled == false) return;

            foreach (KeyValuePair<MyEntity, Dictionary<int, MyPhysObjectTrackedTickData>> kvp in m_attachedPhysObjects)
            {
                MyEntity physObject = kvp.Key;

                Dictionary<int, MyPhysObjectTrackedTickData> ticks = kvp.Value;

                MyPhysObjectTrackedTickData tickData;

                //  Check if thick is in the dictionary, because if not, our trailer is probably running longer then tracked data and we don't want to throw exception
                //  So object just stops receiving data from here and start behaving normaly
                if (ticks.TryGetValue(m_activeTick, out tickData) == true)
                {
                    MySmallShip minerShip = (MySmallShip)physObject;

                    if (minerShip.IsDead())
                    {
                        continue;
                    }

                    Matrix tickMatrix = tickData.Orientation;
                    tickMatrix.Translation = tickData.Position;
                    minerShip.Physics.LinearVelocity = (minerShip.WorldMatrix.Translation - tickData.Position) / MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

                    minerShip.SetWorldMatrix(tickMatrix);
                    minerShip.TrailerUpdate(tickData.ReflectorLevel, tickData.EngineLevel);

                    if (tickData.GunShot != null)
                    {
                        MyMwcObjectBuilder_FireKeyEnum fireKey = MyMwcObjectBuilder_FireKeyEnum.Primary;
                        switch (tickData.GunShot.Value)
                        {
                            case MyTrailerGunsShotTypeEnum.PROJECTILE:
                                fireKey = MyMwcObjectBuilder_FireKeyEnum.Primary;
                                break;
                            case MyTrailerGunsShotTypeEnum.MISSILE:
                                fireKey = MyMwcObjectBuilder_FireKeyEnum.Secondary;
                                break;
                            case MyTrailerGunsShotTypeEnum.MISSILE_GUIDED:
                                fireKey = MyMwcObjectBuilder_FireKeyEnum.Third;
                                break;
                            case MyTrailerGunsShotTypeEnum.DRILLING_DEVICE:
                                fireKey = MyMwcObjectBuilder_FireKeyEnum.Fourth;
                                break;
                            case MyTrailerGunsShotTypeEnum.HARVESTING_ORE:
                                fireKey = MyMwcObjectBuilder_FireKeyEnum.Fifth;
                                break;

                            default:
                                throw new MyMwcExceptionApplicationShouldNotGetHere();
                                break;
                        }

                        minerShip.Weapons.Fire(fireKey);
                    }
                }
            }

            IncreaseActiveTick();
        }

        public static void RemoveFromTrailer(MyEntity entity)
        {
            if (m_attachedPhysObjects.ContainsKey(entity))
            {
                m_attachedPhysObjects.Remove(entity);
            }
        }

        public static bool IsTrailerShip(MyEntity entity)
        {
            return m_attachedPhysObjects.ContainsKey(entity);
        }

        /*public static void GenerateFakeExplosionParticles(BoundingSphere explosionSphere, Vector3 dirToCamera)
        {
            //  Explosion particles
            for (int i = 0; i < 100; i++)
            {
                MyParticle newParticle = MyParticles.AddParticle();

                if (newParticle != null)
                {
                    float startColor = MyMwcUtils.GetRandomFloat(1.0f, 1.0f);
                    float endColor = MyMwcUtils.GetRandomFloat(1.0f, 1.0f);
                    float startAlpha = MyMwcUtils.GetRandomFloat(0.8f, 1.0f);
                    float endAlpha = MyMwcUtils.GetRandomFloat(0.8f, 1.0f);

                    Vector3 randomDirection = MyUtilRandomVector3ByDeviatingVector.GetRandom(dirToCamera, MathHelper.ToRadians(20));// MyMwcUtils.GetRandomVector3HemisphereNormalized(dirToCamera); //MyMwcUtils.GetRandomVector3Normalized();

                    Vector3 offset = randomDirection * MyMwcUtils.GetRandomFloat(explosionSphere.Radius * 0.2f, explosionSphere.Radius * 0.5f);

                    float radius = MathHelper.Lerp(explosionSphere.Radius * 0.1f, explosionSphere.Radius * 0.01f, MathHelper.Clamp(offset.Length() / (explosionSphere.Radius * 0.5f), 0, 1));
                    
                    newParticle.StartPointParticle(
                        MyParticleTexturesEnum.Explosion,
                        MyParticleBlendType.Additive, false,
                        1000 * 1000 * 1000,
                        explosionSphere.Center + offset,
                        Vector3.Zero,
                        new Vector4(startColor, startColor, startColor, startAlpha),
                        new Vector4(endColor, endColor, endColor, endAlpha),
                        MyMwcUtils.GetRandomFloat(MathHelper.Pi * -2.0f, MathHelper.Pi * 2.0f),
                        radius,
                        radius,
                        MyMwcUtils.GetRandomFloat(MathHelper.Pi * -0.3f, MathHelper.Pi * 0.3f), 
                        true);
                }
            }

            //  Dust particles
            for (int i = 0; i < 100; i++)
            {
                MyParticle newParticle = MyParticles.AddParticle();

                if (newParticle != null)
                {
                    //  This are small dust particles scattered all around, but we make them only in direction where camera can see them.

                    Vector3 randomDirection = MyUtilRandomVector3ByDeviatingVector.GetRandom(dirToCamera, MathHelper.ToRadians(20));// MyMwcUtils.GetRandomVector3HemisphereNormalized(dirToCamera); //MyMwcUtils.GetRandomVector3Normalized();
                    float randomOffset = MyMwcUtils.GetRandomFloat(explosionSphere.Radius * 0.5f, explosionSphere.Radius * 0.9f);
                    Vector3 randomVelocity = Vector3.Zero;

                    Vector3 newPosition = explosionSphere.Center + randomDirection * randomOffset;

                    float startColor = 0.35f * MathHelper.Lerp(0.2f, 0.5f, MathHelper.Clamp(Vector3.Distance(newPosition, explosionSphere.Center) / explosionSphere.Radius, 0, 1));
                    float endColor = startColor;

                    float radius = explosionSphere.Radius * 0.05f;//MathHelper.Lerp(explosionSphere.Radius * 0.05f, explosionSphere.Radius * 0.01f, MathHelper.Clamp(offset.Length() / (explosionSphere.Radius * 0.9f), 0, 1));

                    newParticle.StartPointParticle(
                        MyParticleTexturesEnum.Smoke,
                        MyParticleBlendType.AlphaBlended, true,
                        1000 * 1000 * 1000,
                        newPosition,
                        Vector3.Zero,
                        new Vector4(startColor, startColor, startColor, 0.5f),
                        new Vector4(endColor, endColor, endColor, 0.5f),
                        MyMwcUtils.GetRandomFloat(MathHelper.Pi * -2.0f, MathHelper.Pi * 2.0f),
                        radius,
                        radius,
                        MyMwcUtils.GetRandomFloat(MathHelper.Pi * -0.5f, MathHelper.Pi * 0.5f), 
                        true);
                }
            }


            //  Explosion debris line particles
            for (int i = 0; i < 100; i++)
            {
                MyParticle newParticle = MyParticles.AddParticle();

                if (newParticle != null)
                {
                    float startAlpha = MyMwcUtils.GetRandomFloat(0.8f, 1.0f);
                    float endAlpha = startAlpha;

                    Vector3 randomDirection = MyUtilRandomVector3ByDeviatingVector.GetRandom(dirToCamera, MathHelper.ToRadians(20));// MyMwcUtils.GetRandomVector3HemisphereNormalized(dirToCamera);
                    float randomOffset = MyMwcUtils.GetRandomFloat(explosionSphere.Radius * 0.3f, explosionSphere.Radius * 0.5f);

                    float startLength = MyMwcUtils.GetRandomFloat(explosionSphere.Radius * 0.3f, explosionSphere.Radius * 0.9f);
                    float endLength = startLength + MyMwcUtils.GetRandomFloat(explosionSphere.Radius * 0.1f, explosionSphere.Radius * 0.2f);

                    newParticle.StartLineParticle(
                        MyParticleTexturesEnum.Explosion,
                        MyParticleBlendType.Additive, false,
                        1000 * 1000 * 1000,
                        explosionSphere.Center + randomDirection * randomOffset,
                        Vector3.Zero,
                        new Vector4(1, 1, 1, startAlpha),
                        new Vector4(1, 1, 1, endAlpha),
                        randomDirection,
                        MyMwcUtils.GetRandomFloat(0.04f, 0.2f),
                        startLength,
                        endLength,
                        true);
                }
            }

            //  Smoke debris line particles
            for (int i = 0; i < 100; i++)
            {
                MyParticle newParticle = MyParticles.AddParticle();

                if (newParticle != null)
                {
                    float startAlpha = MyMwcUtils.GetRandomFloat(0.5f, 0.8f);
                    float endAlpha = startAlpha - 0.3f;

                    Vector3 randomDirection = MyUtilRandomVector3ByDeviatingVector.GetRandom(dirToCamera, MathHelper.ToRadians(20));// MyMwcUtils.GetRandomVector3HemisphereNormalized(dirToCamera);
                    float randomOffset = MyMwcUtils.GetRandomFloat(explosionSphere.Radius * 0.6f, explosionSphere.Radius * 1.0f);

                    float startColor = MyMwcUtils.GetRandomFloat(0.5f, 1.0f);
                    float endColor = startColor - MyMwcUtils.GetRandomFloat(0.1f, 0.3f);

                    float startLength = MyMwcUtils.GetRandomFloat(0.9f, 1f);
                    float endLength = startLength + MyMwcUtils.GetRandomFloat(0.1f, 0.2f);

                    newParticle.StartLineParticle(
                        MyParticleTexturesEnum.ExplosionSmokeDebrisLine,
                        MyParticleBlendType.AlphaBlended, true,
                        1000 * 1000 * 1000,
                        explosionSphere.Center + randomDirection * randomOffset,
                        Vector3.Zero,
                        new Vector4(startColor, startColor, startColor, startAlpha),
                        new Vector4(endColor, endColor, endColor, endAlpha),
                        randomDirection,
                        MyMwcUtils.GetRandomFloat(0.2f, 0.3f),
                        startLength,
                        endLength,
                        true);
                }
            }
        }*/
    }
}
