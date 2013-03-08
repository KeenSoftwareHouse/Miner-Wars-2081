#region Using

using System;
using System.Diagnostics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.World;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MathHelper = MinerWarsMath.MathHelper;
   


    /// <summary>
    /// Reusable component for drawing a lensflare effect over the top of a 3D scene.
    /// Copied from XNA's Lens Flare example.
    /// </summary>
    static class MySunGlare
    {
        /// <summary>
        /// The lensflare effect is made up from several individual flare graphics,
        /// which move across the screen depending on the position of the sun. This
        /// helper class keeps track of the position, size, and color for each flare.
        /// </summary>
        class MyFlare
        {
            public MyFlare(float position, float scale, Color color, string textureName)
            {
                Position = position;
                Scale = scale;
                Color = color;
                Texture = MyTextureManager.GetTexture<MyTexture2D>(textureName);
            }

            public float Position;
            public float Scale;
            public Color Color;
            public MyTexture2D Texture;
        }

        private static readonly int[] m_allowedCallers = { (int) MyRenderCallerEnum.Main, (int) MyRenderCallerEnum.SecondaryCamera };

        static float m_screenScale;

        //  How big a rectangle should we examine when issuing our occlusion queries?
        //  Increasing this makes the flares fade out more gradually when the sun goes
        //  behind scenery, while smaller query areas cause sudden on/off transitions.
        static float m_querySize;

        /// <summary>
        /// Set by the main game to tell us the position of the camera and sun.
        /// </summary>
        static Matrix m_view;

        /// <summary>
        /// Set by the main game to tell us the position of the camera and sun.
        /// </summary>
        static Matrix m_projection;

        //  Graphics objects.
        static MyTexture2D m_glareSprite;
        static MyTexture2D m_glowSprite;
        static MyTexture2D m_glowSprite2;
        static SpriteBatch m_spriteBatch;

        //  An occlusion query is used to detect when the sun is hidden behind scenery. We have separate queries for different callers.
        static readonly MyOcclusionQuery[] m_occlusionQueries = new MyOcclusionQuery[m_allowedCallers.Length];

        static readonly MyOcclusionQuery[] m_measurementOcclusionQueries = new MyOcclusionQuery[m_allowedCallers.Length];
        static readonly float[] m_occlusionMeasurementResults = new float[m_allowedCallers.Length];

        static MyLightGlare.State[] m_occlusionStates = new MyLightGlare.State[m_allowedCallers.Length];


        static BoundingBox m_occlusionBox;
        static readonly float[] m_visibilityRatios = new float[m_allowedCallers.Length]; // 0 - fully occluded, 1 - fully visible

        // Array describes the position, size, color, and texture for each individual
        // flare graphic. The position value lies on a line between the sun and the
        // center of the screen. Zero places a flare directly over the top of the sun,
        // one is exactly in the middle of the screen, fractional positions lie in
        // between these two points, while negative values or positions greater than
        // one will move the flares outward toward the edge of the screen. Changing
        // the number of flares, or tweaking their positions and colors, can produce
        // a wide range of different lensflare effects without altering any other code.
        static MyFlare[] m_flares;

        static Vector2 m_lightPosition = Vector2.Zero;
        private static Vector3 m_directionToSunNormalized = Vector3.Zero;
        private static float m_distanceToSun = -1;


        static MySunGlare()
        {                                                            
            MyRender.RegisterRenderModule(MyRenderModuleEnum.SunGlow, "Sun glow", DrawGlow, MyRenderStage.AlphaBlendPreHDR, 50, true);
            MyRender.RegisterRenderModule(MyRenderModuleEnum.SunGlow, "Sun glare and lens flare", DrawGlareAndFlare, MyRenderStage.AlphaBlend, 250, true);
        }

        public static void UpdateScreenSize()
        {
            //  This hardcoded values are here because Len's flare example used pixels and I want screen resolution independence.
            //  So here I get screen scale and then fix all sizes and positions from pixel to <0..1> resolution.

            //  Height is better because tripple-monitor could make mess with width if not calculated properly
            m_screenScale = MyMinerGame.ScreenSize.Y / 1000f;
        }

        //  IMPORTANT: This load content can be called only once in application - at the application start. Not for every game-play screen start.
        //  Reason is that OcclusionQuery will stop work if two or more instances are created, even if previous was Disposed. I don't know if 
        //  it's error or feature... I will just follow above rule.
        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MySunGlare.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
                          
            // Load the glow, ray and flare textures.
            m_glareSprite = MyTextureManager.GetTexture<MyTexture2D>("Textures\\SunGlare\\SunGlare");
            m_glowSprite = MyTextureManager.GetTexture<MyTexture2D>("Textures\\SunGlare\\sun_glow_main");
            m_glowSprite2 = MyTextureManager.GetTexture<MyTexture2D>("Textures\\SunGlare\\sun_glow_ray");

            m_flares = new[] {
                new MyFlare(-0.5f, 0.7f, new Color( 30,  40,  50), "Textures\\SunGlare\\flare1"),
                new MyFlare(0.3f, 0.4f, new Color(155, 165, 180), "Textures\\SunGlare\\flare1"),
                new MyFlare(1.2f, 1.0f, new Color(40,  50,  50), "Textures\\SunGlare\\flare1"),
                new MyFlare(1.5f, 1.5f, new Color( 80, 90,  100), "Textures\\SunGlare\\flare1"),
                new MyFlare(-0.3f, 0.7f, new Color(140,  150,  160), "Textures\\SunGlare\\flare2"),
                new MyFlare(0.6f, 0.9f, new Color( 85, 95,  100), "Textures\\SunGlare\\flare2"),
                new MyFlare(0.7f, 0.4f, new Color( 130, 150, 170), "Textures\\SunGlare\\flare2"),
                new MyFlare(-0.7f, 0.7f, new Color( 60, 60,  80), "Textures\\SunGlare\\flare3"),
                new MyFlare(0.0f, 0.6f, new Color( 20,  25,  30), "Textures\\SunGlare\\flare3"),
                new MyFlare(2.0f, 1.4f, new Color( 70,  85, 110), "Textures\\SunGlare\\flare3"),
            };

            // Create a SpriteBatch for drawing the glow and flare sprites.
            m_spriteBatch = new SpriteBatch(MyMinerGame.Static.GraphicsDevice, "SunGlare");

            // Create the occlusion query object. But only first time! Then just reuse it.
            for (int i = 0; i < m_occlusionQueries.Length; i++)
            {
                m_occlusionQueries[i] = MyOcclusionQueries.Get();
                m_occlusionStates[i] = MyLightGlare.State.IssueMeasure;
            }
            for (int i = 0; i < m_measurementOcclusionQueries.Length; i++)
            {
                m_measurementOcclusionQueries[i] = MyOcclusionQueries.Get();
            }
                    
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySunGlare.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            if (m_spriteBatch != null)
            {
                m_spriteBatch.Dispose();
                m_spriteBatch = null;
            }

            // Create the occlusion query object. But only first time! Then just reuse it.
            for (int i = 0; i < m_occlusionQueries.Length; i++)
            {
                MyOcclusionQueries.Return(m_occlusionQueries[i]);
            }
            for (int i = 0; i < m_measurementOcclusionQueries.Length; i++)
            {
                MyOcclusionQueries.Return(m_measurementOcclusionQueries[i]);
            }
        }

        public static Vector3 GetSunPosition()
        {
            return MyCamera.Position + .5f * m_directionToSunNormalized * MySunConstants.RENDER_SUN_DISTANCE;
        }

        public static Vector3 GetSunDirection()
        {
            return -m_directionToSunNormalized;
        }

        private static void DrawGlow()
        {
            // this should be computed every time the sector is changed. If it is not initialized, calculate now:
            m_distanceToSun = MyGuiScreenGamePlay.Static.GetPositionInMillionsOfKm().Length();

            m_directionToSunNormalized = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();

            float radius = 100000 * MySunConstants.SUN_SIZE_MULTIPLIER * MySunConstants.RENDER_SUN_DISTANCE / m_distanceToSun;
            radius = Math.Max(MySunConstants.MIN_SUN_SIZE * MySector.SunProperties.SunSizeMultiplier, radius);
            radius = Math.Min(MySunConstants.MAX_SUN_SIZE * MySector.SunProperties.SunSizeMultiplier, radius);

            float sunColorMultiplier = 3;

            sunColorMultiplier *= (1 - MySector.FogMultiplierForSun);

            m_querySize = .5f * radius;

            Vector3 sunPosition = GetSunPosition();
            radius *= .5f;
            var color = new Vector4(.95f * sunColorMultiplier, .65f * sunColorMultiplier, .35f * sunColorMultiplier, 1);
            //var color = new Vector4(sunColorMultiplier * MySector.SunColorWithFog, 1);
            MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.SunDisk, color, sunPosition, radius, 0);
        }

        //  Mesures how much of the sun is visible, by drawing a small rectangle,
        //  centered on the sun, but with the depth set to as far away as possible,
        //  and using an occlusion query to measure how many of these very-far-away
        //  pixels are not hidden behind the terrain.
        //
        //  The problem with occlusion queries is that the graphics card runs in
        //  parallel with the CPU. When you issue drawing commands, they are just
        //  stored in a buffer, and the graphics card can be as much as a frame delayed
        //  in getting around to processing the commands from that buffer. This means
        //  that even after we issue our occlusion query, the occlusion results will
        //  not be available until later, after the graphics card finishes processing
        //  these commands.
        //
        //  It would slow our game down too much if we waited for the graphics card,
        //  so instead we delay our occlusion processing by one frame. Each time
        //  around the game loop, we read back the occlusion results from the previous
        //  frame, then issue a new occlusion query ready for the next frame to read
        //  its result. This keeps the data flowing smoothly between the CPU and GPU,
        //  but also causes our data to be a frame out of date: we are deciding
        //  whether or not to draw our lensflare effect based on whether it was
        //  visible in the previous frame, as opposed to the current one! Fortunately,
        //  the camera tends to move slowly, and the lensflare fades in and out
        //  smoothly as it goes behind the scenery, so this out-by-one-frame error
        //  is not too noticeable in practice.

        static void UpdateOcclusion(int indexOfQuery)
        {

            switch (m_occlusionStates[indexOfQuery])
            {
                case MyLightGlare.State.IssueMeasure:
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLightGlare.State.IssueMeasure");

                        IssueQueries(indexOfQuery);
                        IssueOcclusionQuery(m_measurementOcclusionQueries[indexOfQuery], false);
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitMeasure;

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                    break;
                case MyLightGlare.State.WaitMeasure:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitMeasure1;
                    }
                    break;
                case MyLightGlare.State.WaitMeasure1:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitMeasure2;
                    }
                    break;
                case MyLightGlare.State.WaitMeasure2:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitMeasure3;
                    }
                    break;
                case MyLightGlare.State.WaitMeasure3:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.CheckMeasure;
                    }
                    break;
                case MyLightGlare.State.CheckMeasure:
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLightGlare.State.CheckMeasure");

                        if (m_measurementOcclusionQueries[indexOfQuery].IsComplete)
                        {

                            int measuredPixels = m_measurementOcclusionQueries[indexOfQuery].PixelCount;

                            //ATI
                            if (measuredPixels <= 0)
                                measuredPixels = 1;

                            m_occlusionMeasurementResults[indexOfQuery] = measuredPixels;
                            m_occlusionStates[indexOfQuery] = MyLightGlare.State.IssueOcc;
                        }
                        else
                        {
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                    break;

                case MyLightGlare.State.IssueOcc:
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLightGlare.State.IssueOcc");

                        IssueQueries(indexOfQuery);
                        IssueOcclusionQuery(m_occlusionQueries[indexOfQuery], true);
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitOcc;

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                    break;
                case MyLightGlare.State.WaitOcc:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitOcc1;
                    }
                    break;
                case MyLightGlare.State.WaitOcc1:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitOcc2;
                    }
                    break;
                case MyLightGlare.State.WaitOcc2:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.WaitOcc3;
                    }
                    break;
                case MyLightGlare.State.WaitOcc3:
                    {
                        m_occlusionStates[indexOfQuery] = MyLightGlare.State.CheckOcc;
                    }
                    break;
                case MyLightGlare.State.CheckOcc:
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyLightGlare.State.CheckOcc");

                        if ((m_occlusionQueries[indexOfQuery].IsComplete))
                        {
                            int occPixels = m_occlusionQueries[indexOfQuery].PixelCount;
                            //ATI
                            if (occPixels < 0)
                                occPixels = 0;
                            //if (occPixels < MySunConstants.MAX_SUNGLARE_PIXELS)
                            {
                                m_visibilityRatios[indexOfQuery] = MathHelper.Clamp(occPixels / (float)m_occlusionMeasurementResults[indexOfQuery], 0, 1);
                            }

                            m_occlusionStates[indexOfQuery] = MyLightGlare.State.IssueMeasure;
                        }

                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        private static void IssueQueries(int indexOfQuery)
        {
            Vector3 directionToSunNormalized = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
            Vector3 sunPosition = MyCamera.Position + directionToSunNormalized * MySunConstants.RENDER_SUN_DISTANCE;// MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd.Value;

            m_occlusionBox.Min = sunPosition - new Vector3(MathHelper.Max(m_querySize, MySunConstants.MIN_QUERY_SIZE));
            m_occlusionBox.Max = sunPosition + new Vector3(MathHelper.Max(m_querySize, MySunConstants.MIN_QUERY_SIZE));

            
        }

        private static void IssueOcclusionQuery(MyOcclusionQuery query, bool depthTest)
        {     
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("IssueOcclusionQuery");
            BlendState previousBlendState = BlendState.Current;
            MyStateObjects.DisabledColorChannels_BlendState.Apply();
            RasterizerState.CullNone.Apply();

            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                DepthStencilState.DepthRead.Apply();
            else
                DepthStencilState.None.Apply();

            query.Begin();

            //generate and draw bounding box of our renderCell in occlusion query 
            MySimpleObjectDraw.DrawOcclusionBoundingBox(m_occlusionBox, 1.0f, depthTest, true);

            query.End();

            previousBlendState.Apply();
                        
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        /// <summary>
        /// Reads and issues occlusions queries and draws sun glare and flare effects based on
        /// the result of the queries.
        /// </summary>
        public static void DrawGlareAndFlare()
        {
            Vector3 projectedPosition = CalculateProjectedPosition();

            m_lightPosition = new Vector2(projectedPosition.X,
                                                projectedPosition.Y);

            // determine caller index and if the caller is allowed
            Debug.Assert(MyRender.CurrentRenderSetup.CallerID != null, "MyRender.CurrentRenderSetup.CallerID cannot be null");
            int callerIndex = Array.IndexOf(m_allowedCallers, (int) MyRender.CurrentRenderSetup.CallerID);
            if (callerIndex < 0)
            {
                Debug.Assert(false, "Sun Glare is called by an unallowed caller.");
            }

            // Check whether the light is hidden behind the scenery.
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("UpdateOcclusion");
            UpdateOcclusion(callerIndex);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (m_visibilityRatios[callerIndex] < MyMwcMathConstants.EPSILON)
                return;

            float sizeInv = 1f / MathHelper.Max(m_querySize, MySunConstants.MIN_QUERY_SIZE);
            float viewportScale = 0.5f * (MyRender.GetScaleForViewport(null).X + MyRender.GetScaleForViewport(null).Y);
            var borderFactor = (50f / viewportScale) * sizeInv * (float) Math.Sqrt(m_occlusionMeasurementResults[callerIndex]);
            float borderFactorClamped = MathHelper.Clamp(borderFactor, 0, 1);

            // If it is visible, draw the flare effect.
            if (m_visibilityRatios[callerIndex] > 0 && borderFactorClamped > 0)
            {
                DrawGlare(borderFactorClamped, m_visibilityRatios[callerIndex]);
                DrawFlares(borderFactorClamped, m_visibilityRatios[callerIndex]);
            }
        }

        private static void DrawGlare(float occlusionFactor, float visibilityRatio)
        {
            //Dont draw glare if there are godrays, it is overexposed then
            if (MyRenderConstants.RenderQualityProfile.EnableGodRays && MySector.GodRaysProperties.Enabled)
                return;

            var zoom = MyRender.CurrentRenderSetup.EnableZoom ? MyCamera.Zoom.GetZoomLevel() : 1;
            zoom = MathHelper.Clamp(zoom, 0.1f, 1);

            float glowSize = 0.1f * m_querySize / zoom;

            float occlusionRatio = occlusionFactor * visibilityRatio;

            Vector2 origin = new Vector2(m_glowSprite.Width, m_glowSprite.Height) / 2;

            float viewportScale = 0.5f * (MyRender.GetScaleForViewport(null).X + MyRender.GetScaleForViewport(null).Y);
            float scale = glowSize * 2.0f / m_glowSprite.Width * viewportScale;

            m_directionToSunNormalized = MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
            float dot = Vector3.Dot(m_directionToSunNormalized, MyCamera.ForwardVector);

            float glareIntensity;
            if (dot > 0.9f)
                glareIntensity = dot * (1 + MySunConstants.MAX_GLARE_MULTIPLIER * (dot - 0.9f));
            else
                glareIntensity = .9f;


            m_spriteBatch.Begin(SpriteSortMode.Deferred, MyStateObjects.Additive_NoAlphaWrite_BlendState, SharpDX.Toolkit.Graphics.SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.Current);

            Matrix rot = MyCamera.ViewMatrix;
            rot.Translation = Vector3.Zero;
            float yaw, pitch, roll;
            MyUtils.RotationMatrixToYawPitchRoll(ref rot, out yaw, out pitch, out roll);

            // ----- small colored glares (bleeding, complements bloom): -----
            // this one isn't rotating with camera:
            float glare1Alpha = occlusionRatio * occlusionRatio * (1 - MySector.FogProperties.FogMultiplier);
            float glare1Size = scale;
            Color color1 = new Color(new Vector4(MySector.SunProperties.SunDiffuse, MathHelper.Clamp(glare1Alpha,0, 0.1f)));
            m_spriteBatch.Draw(m_glowSprite, SharpDXHelper.ToSharpDX(m_lightPosition), null, SharpDXHelper.ToSharpDX(color1), -pitch, SharpDXHelper.ToSharpDX(origin), glare1Size, SpriteEffects.None, 0);

            // rays glare - this one is rotating with camera
            float glare2Alpha = 0.6f * glareIntensity * glareIntensity * occlusionRatio * occlusionRatio * (1 - MySector.FogProperties.FogMultiplier);
            float glare2Size = 1.5f * glareIntensity * scale;
            Color color2 = new Color(new Vector4(MySector.SunProperties.SunDiffuse, glare2Alpha));
            m_spriteBatch.Draw(m_glowSprite2, SharpDXHelper.ToSharpDX(m_lightPosition), null, SharpDXHelper.ToSharpDX(color2), 0, SharpDXHelper.ToSharpDX(origin), glare2Size, SpriteEffects.None, 0);

            // large white glare - blinding effect
            float glare3Size = 40 * scale;
            m_spriteBatch.Draw(m_glareSprite, SharpDXHelper.ToSharpDX(m_lightPosition), null, new SharpDX.Color(1, 0, 1, glareIntensity * occlusionRatio * 0.3f), 0, SharpDXHelper.ToSharpDX(origin), glare3Size, SpriteEffects.None, 0);

            m_spriteBatch.End();
        }

        //  Draws the lensflare sprites, computing the position
        //  of each one based on the current angle of the sun.
        static void DrawFlares(float occlusionFactor, float visibilityRatio)
        {          
            Viewport viewport = MyMinerGame.Static.GraphicsDevice.Viewport;

            // Lensflare sprites are positioned at intervals along a line that
            // runs from the 2D light position toward the center of the screen.
            Vector2 screenCenter = new Vector2(viewport.Width, viewport.Height) / 2;

            Vector2 flareVector = screenCenter - m_lightPosition;

            // Draw the flare sprites using additive blending.
            m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            foreach (MyFlare flare in m_flares)
            {
                // Compute the position of this flare sprite.
                Vector2 flarePosition = m_lightPosition + flareVector * flare.Position * m_screenScale;

                // Set the flare alpha based on the previous occlusion query result.
                Vector4 flareColor = flare.Color.ToVector4();

                flareColor.W *= occlusionFactor * visibilityRatio * visibilityRatio * (1 - MySector.FogMultiplierForSun);
                flareColor.W *= (1 - MySector.FogProperties.FogMultiplier);

                // Center the sprite texture.
                Vector2 flareOrigin = new Vector2(flare.Texture.Width,
                                                  flare.Texture.Height) / 2;

                // Draw the flare.
                m_spriteBatch.Draw(flare.Texture, SharpDXHelper.ToSharpDX(flarePosition), null,
                                 new SharpDX.Color(flareColor.X, flareColor.Y, flareColor.Z, flareColor.W), 1, SharpDXHelper.ToSharpDX(flareOrigin),
                                 flare.Scale * m_screenScale, SpriteEffects.None, 0);
            }

            m_spriteBatch.End();   
        }

        static Vector3 CalculateProjectedPosition()
        {                               
            Vector3 directionFromSunNormalized = -MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();

            // Tell the lensflare component where our camera is positioned.
            m_view = MyCamera.ViewMatrix;
            m_projection = MyCamera.ProjectionMatrix;

            // The sun is infinitely distant, so it should not be affected by the
            // position of the camera. Floating point math doesn't support infinitely
            // distant vectors, but we can get the same result by making a copy of our
            // view matrix, then resetting the view translation to zero. Pretending the
            // camera has not moved position gives the same result as if the camera
            // was moving, but the light was infinitely far away. If our flares came
            // from a local object rather than the sun, we would use the original view
            // matrix here.
            Matrix infiniteView = m_view;

            infiniteView.Translation = Vector3.Zero;

            // Project the light position into 2D screen space.
            Viewport viewport = MyMinerGame.Static.GraphicsDevice.Viewport;

            Vector3 projectedPosition = SharpDXHelper.ToXNA( viewport.Project(SharpDXHelper.ToSharpDX(-directionFromSunNormalized), SharpDXHelper.ToSharpDX(m_projection), SharpDXHelper.ToSharpDX(infiniteView), SharpDXHelper.ToSharpDX(Matrix.Identity)));
            return projectedPosition; 
            return Vector3.Zero;
        }

        /// <summary>
        /// Should be called when player changes sectors.
        /// </summary>
        internal static void UpdateSectorInfo()
        {
            m_distanceToSun = MyGuiScreenGamePlay.Static.GetPositionInMillionsOfKm().Length() * 1e6f;
        }
    }

}
