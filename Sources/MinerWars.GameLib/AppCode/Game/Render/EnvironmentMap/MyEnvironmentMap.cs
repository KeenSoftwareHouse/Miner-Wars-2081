using System;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.App;
using MinerWars.CommonLIB.AppCode.Utils;

//using MinerWarsMath.Graphics;
using MinerWarsMath;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace MinerWars.AppCode.Game.Render.EnvironmentMap
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;

    class MyEnvironmentMap
    {
        static MyEnvironmentMap()
        {
            MyMinerGame.GraphicsDeviceManager.DeviceReset += delegate { Reset(); };
            MyRenderConstants.OnRenderQualityChange += delegate { Reset(); };
        }

        static MyEnvironmentMapRenderer m_environmentMapRendererMain = new MyEnvironmentMapRenderer();
        static MyEnvironmentMapRenderer m_environmentMapRendererAux = new MyEnvironmentMapRenderer();

        static bool m_renderInstantly = true;

        /// <summary>
        /// Maximal distance MainMap from MainMapPosition where MainMap is rendered.
        /// Further than this distance, AuxMap is set as MainMap.
        /// </summary>
        public static float MainMapMaxDistance = 400;

        /// <summary>
        /// When camera moves more than this distance, cube map is refreshed in one frame to prevent blinking when "teleporting" camera or moving extremelly fast
        /// </summary>
        public static float InstantRefreshDistance = MainMapMaxDistance * 1.5f;

        /// <summary>
        /// Distance from MainMapPosition where MainMap begins to blend with AuxMap.
        /// </summary>
        public static float BlendDistance = MainMapMaxDistance / 2 + 1;

        public static float Hysteresis = 40;

        public static float NearDistance
        {
            get
            {
                return m_environmentMapRendererMain.NearDistance;
            }
            set
            {
                if (value < 50) value = 50;
                m_environmentMapRendererMain.NearDistance = value;
                m_environmentMapRendererAux.NearDistance = value;
                Reset();
            }
        }

        public static float FarDistance
        {
            get
            {
                return m_environmentMapRendererMain.FarDistance;
            }
            set
            {
                if (value < 50) value = 50;
                if (value < NearDistance) value = NearDistance;
                m_environmentMapRendererMain.FarDistance = value;
                m_environmentMapRendererAux.FarDistance = value;
                Reset();
            }
        }

        public static Vector3? MainMapPosition { get; private set; }
        public static CubeTexture EnvironmentMainMap
        {
            get
            {
                return m_environmentMapRendererMain.Environment;
            }
        }

        public static Vector3? AuxMapPosition { get; private set; }
        public static CubeTexture EnvironmentAuxMap
        {
            get
            {
                return m_environmentMapRendererAux.Environment;
            }
        }

        public static CubeTexture AmbientMainMap
        {
            get
            {
                return m_environmentMapRendererMain.Ambient;
            }
        }

        public static CubeTexture AmbientAuxMap
        {
            get
            {
                return m_environmentMapRendererAux.Ambient;
            }
        }

        /// <summary>
        /// Gets or sets the BlendFactor, value is between 0.0f and 1.0f, 0.0f means show only MainMap, 1.0f means show only AuxMap
        /// </summary>
        public static float BlendFactor { get; private set; }

        /// <summary>
        /// Gets or sets duration of last update in miliseconds
        /// </summary>
        public static float LastUpdateTime { get; private set; }

        public static void SetSize(int size)
        {
            MyRender.CreateEnvironmentMapsRT(size);
            Reset();
        }

        public static void SetRenderTargets(CubeTexture envMain, CubeTexture envAux, CubeTexture ambMain, CubeTexture ambAux, Texture fullSizeRT)
        {
            m_environmentMapRendererMain.SetRenderTarget(envMain, ambMain, fullSizeRT);
            m_environmentMapRendererAux.SetRenderTarget(envAux, ambAux, fullSizeRT);
        }

        /// <summary>
        /// Causes maps to be recreated immediately
        /// </summary>
        public static void Reset()
        {
            MainMapPosition = null;
            AuxMapPosition = null;
            m_renderInstantly = true;
        }



        public static void Update()
        {
            //TODO: use only for profiling
            /*
            long startTime;
            MyWindowsAPIWrapper.QueryPerformanceCounter(out startTime);
              */

            bool renderEnviromentMaps = MyRender.EnableLights && MyRender.EnableLightsRuntime && MyRender.EnableSun && (MyRender.EnableEnvironmentMapAmbient || MyRender.EnableEnvironmentMapReflection);

            if (!renderEnviromentMaps)
            {
                return;
            }

            if (BlendDistance > MainMapMaxDistance)
            {
                throw new InvalidOperationException("BlendDistance must be lower than MainMapMaxDistance");
            }

            MyRender.RenderOcclusionsImmediatelly = true;

            Vector3 cameraPos = MyCamera.Position;

            if (MainMapPosition.HasValue && (cameraPos - MainMapPosition.Value).Length() > InstantRefreshDistance)
            {
                m_renderInstantly = true;
            }
            
            // Makes evironment camera pos 300m in front of real camera
            //cameraPos += Vector3.Normalize(MyCamera.ForwardVector) * 300

            if (MainMapPosition == null)
            {
                LastUpdateTime = 0;
                MainMapPosition = cameraPos;
                m_environmentMapRendererMain.StartUpdate(MainMapPosition.Value, m_renderInstantly);
                m_renderInstantly = false;

                BlendFactor = 0.0f;
            }
            else
            {
                float mainMapDistance = (MainMapPosition.Value - cameraPos).Length();

                // When behind blend distance
                if (mainMapDistance > BlendDistance)
                {
                    // Create AuxMap if not created
                    if (AuxMapPosition == null)
                    {
                        LastUpdateTime = 0;
                        AuxMapPosition = cameraPos;
                        m_environmentMapRendererAux.StartUpdate(AuxMapPosition.Value, m_renderInstantly);
                        m_renderInstantly = false;
                    }

                    // Wait till rendering done before blending
                    if (m_environmentMapRendererAux.IsDone())
                    {
                        // Set proper blend factor
                        BlendFactor = (mainMapDistance - BlendDistance) / (MainMapMaxDistance - BlendDistance);
                    }
                }
                else if ((mainMapDistance + Hysteresis) < BlendDistance)
                {
                    AuxMapPosition = null;
                }

                // If MainMap should not be even displayed...swap aux and main and display
                if (mainMapDistance > MainMapMaxDistance && m_environmentMapRendererAux.IsDone())
                {
                    var tmp = m_environmentMapRendererAux;
                    m_environmentMapRendererAux = m_environmentMapRendererMain;
                    m_environmentMapRendererMain = tmp;
                    MainMapPosition = cameraPos + MyMwcUtils.Normalize(MainMapPosition.Value - cameraPos) * BlendDistance;
                    AuxMapPosition = null;
                    BlendFactor = 0.0f;
                }
            }

            m_environmentMapRendererMain.ContinueUpdate();
            m_environmentMapRendererAux.ContinueUpdate();

            MyRender.RenderOcclusionsImmediatelly = false;

            /*
            long frq;
            MyWindowsAPIWrapper.QueryPerformanceFrequency(out frq);

            long stopTime;
            MyWindowsAPIWrapper.QueryPerformanceCounter(out stopTime);

            float updateTime = ((float)(stopTime - startTime)) / frq * 1000.0f;
            if(updateTime > LastUpdateTime)
            {
                LastUpdateTime = updateTime;
            }
             * */
        }
    }
}
