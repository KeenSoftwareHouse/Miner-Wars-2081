using System;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Render;
using System.Collections.Generic;
using KeenSoftwareHouse.Library.Extensions;

//  This class render "sun wind" coming from the sun. It works for sun in any direction (don't have to be parallel with one of the axis) - though I haven't tested it.
//  There are large and small billboards. Large are because I don't want draw a lot of small billboards on edge, where player won't see anything. 
//  Important are small. These are only close to camera. We check on them how far they can reach (at start of sun wind) and they will go only there.
//  Only voxels and large ships can stop small billboards. Other objects are ignored.
//
//  Sound of sun wind is not exactly point sound source, because it lies on a line. So we hear it coming, then we are in the sound and then we hear it comint out.


namespace MinerWars.AppCode.Game.TransparentGeometry
{
    static class MySunWind
    {
        //  This isn't particle as we know it in MyParticles. It just stores information about one individual sun win billboard.
        class MySunWindBillboard
        {
            public Vector4 Color;
            public float Radius;
            public float InitialAngle;
            public float RotationSpeed;
            public Vector3 InitialAbsolutePosition;
        }

        //  This isn't particle as we know it in MyParticles. It just stores information about one individual sun win billboard.
        class MySunWindBillboardSmall : MySunWindBillboard
        {            
            public float MaxDistance;
            public int TailBillboardsCount;
            public float TailBillboardsDistance;

            public float[] RadiusScales;
        }

        //  True if sun wind is comming, otherwise false
        public static bool IsActive = false;
        public static bool IsVisible = true;

        // Actual position of sun wind
        public static Vector3 Position;

        //  Center of sun wind particles or wall of particles
        static Vector3 m_initialSunWindPosition;

        //  Direction which sun wind is coming from (from sun to camera)
        static Vector3 m_directionFromSunNormalized;

        //  These parameters are updated each UPDATE
        static MyPlane m_planeMiddle;
        static MyPlane m_planeFront;
        static MyPlane m_planeBack;
        static float m_distanceToSunWind;
        static Vector3 m_positionOnCameraLine;

        static int m_timeLastUpdate;

        //  Speed of sun wind, in meters per second
        static float m_speed;

        //  Vectors that define plane of sun wind
        static Vector3 m_rightVector;
        static Vector3 m_downVector;

        //  Strength of sun wind, in interval <0..1>. This will determine strength of sun color and other values.
        static float m_strength;

        //  When checking if how far small billboard can reach, we will check only objects of these type
        public static Type[] DoNotIgnoreTheseTypes = new Type[] { typeof(MyVoxelMap), typeof(MyPrefabBase), typeof(MyPrefab), typeof(MyPrefabLargeShip), typeof(MyStaticAsteroid) };

        static MySunWindBillboard[][] m_largeBillboards;
        
        static MySunWindBillboardSmall[][] m_smallBillboards;
        static bool m_smallBillboardsStarted;

        static MySoundCue? m_burningCue;

        static List<MyEntity> m_sunwindEntities = new List<MyEntity>(100);

        // MaxDistance values for SmallBillboards are computed in more updates
        // Small Billboards are ready when m_computedMaxDistances == SMALL_BILLBOARDS_SIZE.X * SMALL_BILLBOARDS_SIZE.Y
        static int m_computedMaxDistances;

        static MySunWind()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.SunWind, "Sun wind", Draw, Render.MyRenderStage.PrepareForDraw);
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MySunWind.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySunwind::LoadContent");

            //  Large billboards
            m_largeBillboards = new MySunWindBillboard[MySunWindConstants.LARGE_BILLBOARDS_SIZE.X][];
            for (int x = 0; x < MySunWindConstants.LARGE_BILLBOARDS_SIZE.X; x++)
            {
                m_largeBillboards[x] = new MySunWindBillboard[MySunWindConstants.LARGE_BILLBOARDS_SIZE.Y];
                for (int y = 0; y < MySunWindConstants.LARGE_BILLBOARDS_SIZE.Y; y++)
                {
                    m_largeBillboards[x][y] = new MySunWindBillboard();
                    MySunWindBillboard billboard = m_largeBillboards[x][y];

                    billboard.Radius = MyMwcUtils.GetRandomFloat(MySunWindConstants.LARGE_BILLBOARD_RADIUS_MIN, MySunWindConstants.LARGE_BILLBOARD_RADIUS_MAX);
                    billboard.InitialAngle = MyMwcUtils.GetRandomRadian();
                    billboard.RotationSpeed = MyMwcUtils.GetRandomSign() * MyMwcUtils.GetRandomFloat(MySunWindConstants.LARGE_BILLBOARD_ROTATION_SPEED_MIN, MySunWindConstants.LARGE_BILLBOARD_ROTATION_SPEED_MAX);

                    //billboard.Color = MySunWindConstants.BILLBOARD_COLOR;
                    //billboard.Color.X = MyMwcUtils.GetRandomFloat(0.5f, 3);
                    //billboard.Color.Y = MyMwcUtils.GetRandomFloat(0.5f, 2);
                    //billboard.Color.Z = MyMwcUtils.GetRandomFloat(0.5f, 2);
                    //billboard.Color.W = MyMwcUtils.GetRandomFloat(0.5f, 2);
                    billboard.Color.X = MyMwcUtils.GetRandomFloat(0.5f, 3);
                    billboard.Color.Y = MyMwcUtils.GetRandomFloat(0.5f, 1);
                    billboard.Color.Z = MyMwcUtils.GetRandomFloat(0.5f, 1);
                    billboard.Color.W = MyMwcUtils.GetRandomFloat(0.5f, 1);
                }
            }

            //  Small billboards
            m_smallBillboards = new MySunWindBillboardSmall[MySunWindConstants.SMALL_BILLBOARDS_SIZE.X][];
            for (int x = 0; x < MySunWindConstants.SMALL_BILLBOARDS_SIZE.X; x++)
            {
                m_smallBillboards[x] = new MySunWindBillboardSmall[MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y];
                for (int y = 0; y < MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y; y++)
                {
                    m_smallBillboards[x][y] = new MySunWindBillboardSmall();
                    MySunWindBillboardSmall billboard = m_smallBillboards[x][y];

                    billboard.Radius = MyMwcUtils.GetRandomFloat(MySunWindConstants.SMALL_BILLBOARD_RADIUS_MIN, MySunWindConstants.SMALL_BILLBOARD_RADIUS_MAX);
                    billboard.InitialAngle = MyMwcUtils.GetRandomRadian();
                    billboard.RotationSpeed = MyMwcUtils.GetRandomSign() * MyMwcUtils.GetRandomFloat(MySunWindConstants.SMALL_BILLBOARD_ROTATION_SPEED_MIN, MySunWindConstants.SMALL_BILLBOARD_ROTATION_SPEED_MAX);

                    //billboard.Color = MySunWindConstants.BILLBOARD_COLOR;
                    billboard.Color.X = MyMwcUtils.GetRandomFloat(0.5f, 1);
                    billboard.Color.Y = MyMwcUtils.GetRandomFloat(0.2f, 0.5f);
                    billboard.Color.Z = MyMwcUtils.GetRandomFloat(0.2f, 0.5f);
                    billboard.Color.W = MyMwcUtils.GetRandomFloat(0.1f, 0.5f);

                    billboard.TailBillboardsCount = MyMwcUtils.GetRandomInt(MySunWindConstants.SMALL_BILLBOARD_TAIL_COUNT_MIN, MySunWindConstants.SMALL_BILLBOARD_TAIL_COUNT_MAX);
                    billboard.TailBillboardsDistance = MyMwcUtils.GetRandomFloat(MySunWindConstants.SMALL_BILLBOARD_TAIL_DISTANCE_MIN, MySunWindConstants.SMALL_BILLBOARD_TAIL_DISTANCE_MAX);

                    billboard.RadiusScales = new float[billboard.TailBillboardsCount];
                    for (int i = 0; i < billboard.TailBillboardsCount; i++)
                    {
                        billboard.RadiusScales[i] = MyMwcUtils.GetRandomFloat(0.7f, 1.0f);
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySunWind.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MySunWind.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            IsActive = false;

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySunWind.UnloadContent - END");            
        }

        static float m_deltaTime;
        
        
        //  This method will start sun wind. Or if there is one coming, this will reset it so it will start again.
        public static void Start()
        {
            //  Activate sun wind
            IsActive = true;

            m_smallBillboardsStarted = false;

            m_timeLastUpdate = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            //  Place sun wind at farest possible negative Z position
            //Vector3 directionToSunNormalized = MyMwcUtils.Normalize(MyGuiScreenGameBase.Static.SunPosition - MyCamera.Position); MyMwcSectorGroups.Get(MyGuiScreenGameBase.Static.Sector.SectorGroup).GetDirectionToSunNormalized();
            Vector3 directionToSunNormalized = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
            m_initialSunWindPosition = MyCamera.Position + directionToSunNormalized * MySunWindConstants.SUN_WIND_LENGTH_HALF;
            m_directionFromSunNormalized = -directionToSunNormalized;

            //  Start the sound of burning (looping)
            StopCue();
            m_burningCue = MyAudio.AddCue3D(MySoundCuesEnum.SfxSolarWind, m_initialSunWindPosition, m_directionFromSunNormalized, Vector3.Up, Vector3.Zero);
            //MySounds.UpdateCuePitch(m_burningCue, MyMwcUtils.GetRandomFloat(-1, +1));

            m_speed = MyMwcUtils.GetRandomFloat(MySunWindConstants.SPEED_MIN, MySunWindConstants.SPEED_MAX);
            
            m_strength = MyMwcUtils.GetRandomFloat(0, 1);

            MyUtils.GetPerpendicularVector(ref m_directionFromSunNormalized, out m_rightVector);
            m_downVector = MyMwcUtils.Normalize(Vector3.Cross(m_directionFromSunNormalized, m_rightVector));

            StartBillboards();
            
            // Reinit computed max distances, they'll be computed in update
            m_computedMaxDistances = 0;

            m_deltaTime = 0;

            // Collect entities
            m_sunwindEntities.Clear();
            foreach (var entity in MyEntities.GetEntities())
            {
                if (!(entity is MySmallShip)) continue;

                // Do not move with indestructibles (NPCs etc)
                if (!entity.IsDestructible)
                    continue;

                m_sunwindEntities.Add(entity);
            }
        }

        
        public static void Update()
        {
            //  Update only if sun wind is active
            if (IsActive == false) return;

            //?
            float dT = ((float)MyMinerGame.TotalGamePlayTimeInMilliseconds - (float)m_timeLastUpdate) / 1000.0f;
            m_timeLastUpdate = MyMinerGame.TotalGamePlayTimeInMilliseconds;

            if((MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) || MyMinerGame.IsPaused())
                return;

            m_deltaTime += dT;

            float traveledDistance = m_speed * m_deltaTime;

            //  If sun wind finished its way, we will turn it off
            if (traveledDistance >= MySunWindConstants.SUN_WIND_LENGTH_TOTAL)
            {
                IsActive = false;
                StopCue();
                return;
            }

            Vector3 campos = MyCamera.Position;

            //  This is plane that goes through sun wind, it's in its middle
            m_planeMiddle = new MyPlane(m_initialSunWindPosition + m_directionFromSunNormalized * traveledDistance, m_directionFromSunNormalized);
            m_distanceToSunWind = MyUtils.GetDistanceFromPointToPlane(ref campos, ref m_planeMiddle);

            //  We make sure that sound moves always on line that goes through camera. So it's not in the middle of sun wind, more like middle where is camera.
            //  Reason is that I want the sound always go through camera.            
            m_positionOnCameraLine = MyCamera.Position - m_directionFromSunNormalized * m_distanceToSunWind;

            Vector3 positionFront = m_positionOnCameraLine + m_directionFromSunNormalized * 5000;
            Vector3 positionBack = m_positionOnCameraLine + m_directionFromSunNormalized * -5000;

            m_planeFront = new MyPlane(ref positionFront, ref m_directionFromSunNormalized);
            m_planeBack = new MyPlane(ref positionBack, ref m_directionFromSunNormalized);

            float distanceToFrontPlane = MyUtils.GetDistanceFromPointToPlane(ref campos, ref m_planeFront);
            float distanceToBackPlane = MyUtils.GetDistanceFromPointToPlane(ref campos, ref m_planeBack);


            Vector3 positionOfSound;
            if ((distanceToFrontPlane <= 0) && (distanceToBackPlane >= 0))
            {
                positionOfSound = MyCamera.Position;
            }
            else if (distanceToFrontPlane > 0)
            {
                positionOfSound = positionFront;
            }
            else
            {
                positionOfSound = positionBack;
            }

            //  Update position of sound. It works like this: we hear coming sound, then we are in the sound and then we hear it coming out.
            MyAudio.UpdateCuePosition(m_burningCue, positionOfSound, m_directionFromSunNormalized, -m_downVector, m_directionFromSunNormalized * m_speed);
            //MySounds.UpdateCuePosition(m_burningCue, positionOfSound, m_directionFromSunNormalized, Vector3.Up, Vector3.Zero);

            //MyLogManager.WriteLine("positionOfSound: " + MyUtils.GetFormatedVector3(positionOfSound, 3));
            //MyLogManager.WriteLine("m_directionFromSunNormalized: " + MyUtils.GetFormatedVector3(m_directionFromSunNormalized, 3));
            //MyLogManager.WriteLine("m_downVector: " + MyUtils.GetFormatedVector3(m_downVector, 3));

            Position = positionOfSound;

            //  Shake player's head
            float distanceToSound;
            Vector3.Distance(ref positionOfSound, ref campos, out distanceToSound);
            float shake = 1 - MathHelper.Clamp(distanceToSound / 1000, 0, 1);
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.IncreaseHeadShake(
                    MathHelper.Lerp(MyHeadShakeConstants.HEAD_SHAKE_AMOUNT_DURING_SUN_WIND_MIN,
                                    MyHeadShakeConstants.HEAD_SHAKE_AMOUNT_DURING_SUN_WIND_MAX, shake));
            }

            for (int i = 0; i < m_sunwindEntities.Count;)
			{
                if (m_sunwindEntities[i].Closed)
                {
                    m_sunwindEntities.RemoveAtFast(i);
                }
                else
                {
                    i++;
                }
            }

            //  Apply force to all objects that aren't static and are hit by sun wind (ignoring voxels and large ships)
            MyEntities.ApplySunWindForce(m_sunwindEntities, ref m_planeFront, ref m_planeBack, DoNotIgnoreTheseTypes, ref m_directionFromSunNormalized);

            //  Start small billboards
            if (m_distanceToSunWind <= MySunWindConstants.SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE)
            {
                Debug.Assert(m_computedMaxDistances == MySunWindConstants.SMALL_BILLBOARDS_SIZE.X * MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y, "Not all small billboard MaxDistances are computed!");
                m_smallBillboardsStarted = true;
            }

            ComputeMaxDistances();
        }

        public static bool IsActiveForHudWarning()
        {            
            if (!IsActive || MySession.PlayerShip == null)
            {
                return false;
            }

            Vector3 playerToSunwind = MySession.PlayerShip.GetPosition() - Position;
            Vector3 directionToPlayerNormalize = Vector3.Normalize(playerToSunwind);
            float dot = Vector3.Dot(m_directionFromSunNormalized, directionToPlayerNormalize);
            // if sun wind before player, always display hud warning
            if (dot >= 0f)
            {
                return true;
            }

            // if sun wind behind player, display hud waring only up to 1000m
            return playerToSunwind.LengthSquared() <= MySmallShipConstants.WARNING_SUN_WIND_MAX_DISTANCE_BEHIND_PLAYER_SQR;
        }

        static void StopCue()
        {
            if ((m_burningCue != null) && (m_burningCue.Value.IsPlaying == true))
            {
                m_burningCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }            
        }

        //  When sun wind is approaching camera position, we have to make sun color more bright
        public static Vector4 GetSunColor()
        {
            //  Increase sun color only if sun wind is really close
            float multiply = 1 - MathHelper.Clamp(Math.Abs(m_distanceToSunWind) / MySunWindConstants.SUN_COLOR_INCREASE_DISTANCE, 0, 1);
            multiply *= MathHelper.Lerp(MySunWindConstants.SUN_COLOR_INCREASE_STRENGTH_MIN, MySunWindConstants.SUN_COLOR_INCREASE_STRENGTH_MAX, m_strength);

            return new Vector4(MySector.SunProperties.SunDiffuse, 1.0f) * (1 + multiply);
        }

        //  When sun wind is approaching camera position, we have to make particle dust more transparent (or invisible), because it doesn't look when mixed with sun wind billboards
        public static float GetParticleDustFieldAlpha()
        {
            return (float)Math.Pow(MathHelper.Clamp(Math.Abs(m_distanceToSunWind) / MySunWindConstants.PARTICLE_DUST_DECREAS_DISTANCE, 0, 1), 4);
        }

        //  This method doesn't really draw. It just creates billboards that are later drawn in MyParticles.Draw()
        public static void Draw()
        {
            if (IsActive == false) return;
            if (IsVisible == false) return;

            //float deltaTime = ((float)MyMinerGame.TotalGamePlayTimeInMilliseconds - (float)m_timeStarted) / 1000.0f;
            float traveledDistance = m_speed * m_deltaTime;
            Vector3 deltaPosition = m_directionFromSunNormalized * traveledDistance;

            //  Draw LARGE billboards
            for (int x = 0; x < MySunWindConstants.LARGE_BILLBOARDS_SIZE.X; x++)
            {
                for (int y = 0; y < MySunWindConstants.LARGE_BILLBOARDS_SIZE.Y; y++)
                {
                    MySunWindBillboard billboard = m_largeBillboards[x][y];

                    Vector3 actualPosition = billboard.InitialAbsolutePosition + deltaPosition;

                    float distanceToCamera;
                    Vector3 campos = MyCamera.Position;
                    Vector3.Distance(ref actualPosition, ref campos, out distanceToCamera);
                    float alpha = 1 - MathHelper.Clamp(distanceToCamera / MySunWindConstants.LARGE_BILLBOARD_DISAPEAR_DISTANCE, 0, 1);

                    float distanceToCenterOfSunWind;
                    Vector3.Distance(ref actualPosition, ref campos, out distanceToCenterOfSunWind);

                    if (distanceToCenterOfSunWind < MySunWindConstants.SWITCH_LARGE_AND_SMALL_BILLBOARD_RADIUS)
                    {
                        alpha *= MathHelper.Clamp(distanceToCamera / MySunWindConstants.SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE, 0, 1);
                    }

                    //billboard.Color *= alpha;

                    MyTransparentGeometry.AddPointBillboard(
                        MyTransparentMaterialEnum.Explosion,
                        new Vector4(billboard.Color.X * alpha, billboard.Color.Y * alpha, billboard.Color.Z * alpha, alpha),
                        actualPosition,
                        billboard.Radius,
                        billboard.InitialAngle + billboard.RotationSpeed * m_deltaTime);
                }
            }

            //  Draw SMALL billboards
            //if (m_distanceToSunWind <= MySunWindConstants.SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE)
            //{
            //    if (m_smallBillboardsStarted == false)
            //    {
            //        StartSmallBillboards();
            //        m_smallBillboardsStarted = true;
            //    }

            if (m_smallBillboardsStarted == true)
            {
                for (int x = 0; x < MySunWindConstants.SMALL_BILLBOARDS_SIZE.X; x++)
                {
                    for (int y = 0; y < MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y; y++)
                    {
                        MySunWindBillboardSmall billboard = m_smallBillboards[x][y];

                        Vector3 actualPosition = billboard.InitialAbsolutePosition + deltaPosition;

                        for (int z = 0; z < billboard.TailBillboardsCount; z++)
                        {
                            Vector3 tempPosition = actualPosition - m_directionFromSunNormalized * (z - billboard.TailBillboardsCount / 2) * billboard.TailBillboardsDistance;

                            float distanceToCamera;
                            Vector3 campos = MyCamera.Position;
                            Vector3.Distance(ref tempPosition, ref campos, out distanceToCamera);

                            //distanceToCamera = Math.Abs(Vector3.Dot(tempPosition - campos, m_directionFromSunNormalized));

                            float alpha = 1 - MathHelper.Clamp((distanceToCamera) / (MySunWindConstants.SWITCH_LARGE_AND_SMALL_BILLBOARD_DISTANCE_HALF), 0, 1);

                            if (alpha > 0)
                            {
                                float distanceFromOrigin;
                                Vector3.Distance(ref tempPosition, ref billboard.InitialAbsolutePosition, out distanceFromOrigin);
                                if (distanceFromOrigin < billboard.MaxDistance)
                                {
                                    MyTransparentGeometry.AddPointBillboard(
                                        MyTransparentMaterialEnum.Explosion,
                                        new Vector4(billboard.Color.X * alpha, billboard.Color.Y * alpha, billboard.Color.Z * alpha, billboard.Color.W * alpha),
                                        tempPosition,
                                        billboard.Radius * billboard.RadiusScales[z],
                                        billboard.InitialAngle + billboard.RotationSpeed * m_deltaTime);
                                }
                            }
                        }
                    }
                }
            }
        }

        static void StartBillboards()
        {
            //  Initialize LARGE billboards
            for (int x = 0; x < MySunWindConstants.LARGE_BILLBOARDS_SIZE.X; x++)
            {
                for (int y = 0; y < MySunWindConstants.LARGE_BILLBOARDS_SIZE.Y; y++)
                {
                    MySunWindBillboard billboard = m_largeBillboards[x][y];

                    Vector3 positionRandomDelta = new Vector3(
                        MyMwcUtils.GetRandomFloat(MySunWindConstants.LARGE_BILLBOARD_POSITION_DELTA_MIN, MySunWindConstants.LARGE_BILLBOARD_POSITION_DELTA_MAX),
                        MyMwcUtils.GetRandomFloat(MySunWindConstants.LARGE_BILLBOARD_POSITION_DELTA_MIN, MySunWindConstants.LARGE_BILLBOARD_POSITION_DELTA_MAX),
                        MyMwcUtils.GetRandomFloat(MySunWindConstants.LARGE_BILLBOARD_POSITION_DELTA_MIN, MySunWindConstants.LARGE_BILLBOARD_POSITION_DELTA_MAX));

                    Vector3 positionRelative = new Vector3(
                        (x - MySunWindConstants.LARGE_BILLBOARDS_SIZE_HALF.X) * MySunWindConstants.LARGE_BILLBOARD_DISTANCE,
                        (y - MySunWindConstants.LARGE_BILLBOARDS_SIZE_HALF.Y) * MySunWindConstants.LARGE_BILLBOARD_DISTANCE,
                        (x - MySunWindConstants.LARGE_BILLBOARDS_SIZE_HALF.X) * MySunWindConstants.LARGE_BILLBOARD_DISTANCE * 0.2f);

                    billboard.InitialAbsolutePosition =
                        m_initialSunWindPosition +
                        m_rightVector * (positionRandomDelta.X + positionRelative.X) +
                        m_downVector * (positionRandomDelta.Y + positionRelative.Y) +
                        -1 * m_directionFromSunNormalized * (positionRandomDelta.Z + positionRelative.Z);
                }
            }

            Vector3 initialPositionOnCameraLine = MyCamera.Position - m_directionFromSunNormalized * MySunWindConstants.SUN_WIND_LENGTH_HALF;

            for (int x = 0; x < MySunWindConstants.SMALL_BILLBOARDS_SIZE.X; x++)
            {
                for (int y = 0; y < MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y; y++)
                {
                    MySunWindBillboardSmall billboard = m_smallBillboards[x][y];

                    Vector2 positionRandomDelta = new Vector2(
                        MyMwcUtils.GetRandomFloat(MySunWindConstants.SMALL_BILLBOARD_POSITION_DELTA_MIN, MySunWindConstants.SMALL_BILLBOARD_POSITION_DELTA_MAX),
                        MyMwcUtils.GetRandomFloat(MySunWindConstants.SMALL_BILLBOARD_POSITION_DELTA_MIN, MySunWindConstants.SMALL_BILLBOARD_POSITION_DELTA_MAX));

                    Vector2 positionRelative = new Vector2(
                        (x - MySunWindConstants.SMALL_BILLBOARDS_SIZE_HALF.X) * MySunWindConstants.SMALL_BILLBOARD_DISTANCE,
                        (y - MySunWindConstants.SMALL_BILLBOARDS_SIZE_HALF.Y) * MySunWindConstants.SMALL_BILLBOARD_DISTANCE);

                    billboard.InitialAbsolutePosition =
                        initialPositionOnCameraLine +
                        m_rightVector * (positionRandomDelta.X + positionRelative.X) +
                        m_downVector * (positionRandomDelta.Y + positionRelative.Y);
                }
            }
        }

        /// <summary>
        /// Compute MaxDistances for uninitialized SmallBillboards
        /// </summary>
        private static void ComputeMaxDistances()
        {
            int smallBillBoardsCount = MySunWindConstants.SMALL_BILLBOARDS_SIZE.X * MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y;
            if (m_computedMaxDistances < smallBillBoardsCount)
            {
                int cnt = (int)(smallBillBoardsCount / MySunWindConstants.SECONDS_FOR_SMALL_BILLBOARDS_INITIALIZATION / MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);
                while (m_computedMaxDistances < smallBillBoardsCount && cnt > 0)
                {
                    int x = m_computedMaxDistances % MySunWindConstants.SMALL_BILLBOARDS_SIZE.Y;
                    int y = m_computedMaxDistances / MySunWindConstants.SMALL_BILLBOARDS_SIZE.X;

                    var billBoard = m_smallBillboards[x][y];

                    ComputeMaxDistance(billBoard);

                    ++m_computedMaxDistances;
                    --cnt;
                }
            }
        }

        private static void ComputeMaxDistance(MySunWindBillboardSmall billboard)
        {
            Vector3 sunWindVector = m_directionFromSunNormalized * MySunWindConstants.SUN_WIND_LENGTH_HALF;
            var offset = (-m_directionFromSunNormalized * MySunWindConstants.RAY_CAST_DISTANCE);
            //  This line start where billboard starts and end at place that is farest possible place billboard can reach
            //  If intersection found, we will mark that place as small billboard's destination. It can't go further.
            MyLine line = new MyLine((sunWindVector + billboard.InitialAbsolutePosition) + offset, billboard.InitialAbsolutePosition + m_directionFromSunNormalized * MySunWindConstants.SUN_WIND_LENGTH_TOTAL, true);
            MyIntersectionResultLineTriangleEx? intersection = MyEntities.GetIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref line, DoNotIgnoreTheseTypes);
            billboard.MaxDistance = (intersection != null) ?
                (intersection.Value.Triangle.Distance - billboard.Radius) :
                MySunWindConstants.SUN_WIND_LENGTH_TOTAL;
        }
    }
}
