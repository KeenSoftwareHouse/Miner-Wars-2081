using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.VideoMode;

namespace MinerWars.AppCode.Game.Renders
{
    static class MyBloomPostprocess
    {
        /*static bool m_loaded;

        // Controls how bright a pixel needs to be before it will bloom.
        // Zero makes everything bloom equally, while higher values select
        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
        static float m_bloomThreshold;

        // Controls how much blurring is applied to the bloom image.
        // The typical range is from 1 up to 10 or so.
        static float m_blurAmount;

        // Controls the amount of the bloom and base images that
        // will be mixed into the final scene. Range 0 to 1.
        static float m_bloomIntensity;
        static float m_baseIntensity;

        // Independently control the color saturation of the bloom and
        // base images. Zero is totally desaturated, 1.0 leaves saturation
        // unchanged, while higher values increase the saturation level.
        static float m_bloomSaturation;
        static float m_baseSaturation;

        static SpriteBatch m_spriteBatch;

        static MyEffectBloomExtract m_bloomExtractEffect;
        static MyEffectBloomCombine m_bloomCombineEffect;
        static MyEffectGaussianBlur m_gaussianBlurEffect;

        static ResolveTexture2D m_resolveTarget;
        static RenderTarget2D m_renderTarget1;
        static RenderTarget2D m_renderTarget2;

        static EffectParameter m_weightsParameter, m_offsetsParameter;
        static float[] m_sampleWeights;
        static Vector2[] m_sampleOffsets;
        static int m_sampleCount;

        // Optionally displays one of the intermediate buffers used
        // by the bloom postprocess, so you can see exactly what is
        // being drawn into each rendertarget.
        public enum IntermediateBuffer
        {
            PreBloom,
            BlurredHorizontally,
            BlurredBothWays,
            FinalResult,
        }

        public static IntermediateBuffer ShowBuffer { get; set; }

        public static void Init()
        {
            //Init(0.25f, 4, 1.25f, 1, 1, 1);     //  Default
            //Init(0,      3,   1,     1,    1,       1);     //  Soft
            //Init(0.5f,   8,   2,     1,    0,       1);     //  Desaturated
            //Init(0.25f,  4,   2,     1,    2,       0);     //  Saturated
            //Init(0,      2,   1,     0.1f, 1,       1);     //  Blurry
            Change(0, 0.5f, 1, 0.1f, 1, 1);     //  Blurry less
            //Init(0.5f, 2, 1, 1, 1, 1);     //  Subtle - this looked good, but bloom without real HDR isn't very OK
            //Init(0, 0, 0, 1, 1, 1);     //  Bloom:OFF            
        }

        public static void Change(float bloomThreshold, float blurAmount, float bloomIntensity, float baseIntensity, float bloomSaturation, float baseSaturation)
        {
            m_bloomThreshold = bloomThreshold;
            m_blurAmount = blurAmount;
            m_bloomIntensity = bloomIntensity;
            m_baseIntensity = baseIntensity;
            m_bloomSaturation = bloomSaturation;
            m_baseSaturation = baseSaturation;            
        }

        public static void ChangeBlurAmount(float blurAmount)
        {
            m_blurAmount = blurAmount;
        }

        //  Load content does nothing special, because we really create all buffers and stuff in LoadInDraw
        public static void LoadContent()
        {
            m_loaded = false;
        }
        
        public static void UnloadContent()
        {
            if (m_spriteBatch != null) m_spriteBatch.Dispose();
            if (m_resolveTarget != null) m_resolveTarget.Dispose();
            if (m_renderTarget1 != null) m_renderTarget1.Dispose();
            if (m_renderTarget2 != null) m_renderTarget2.Dispose();
            m_loaded = false;
        }

        //  This one must be called at application start and also after every video options change
        //  It will dispose and then load all effects/buffers needed by cinematic (of course, only if cinematic is enabled)
        //	Special method that loads data into GPU, and can be called only from Draw method, never from LoadContent or from background thread.
        //	Because that would lead to empty vertex/index buffers if they are filled/created while game is minimized (remember the issue - alt-tab during loading screen)
        public static void LoadInDraw()
        {
            if (m_loaded) return;

            //  Load if cinematic is enabled and we aren't right now unloading the application and there is already a gameplay screen
            if ((MyVideoModeManager.GetCinematic() == true) && (MyGuiScreenGamePlay.Static != null))
            {
                GraphicsDevice graphicsDevice = MyMinerGame.Static.GraphicsDevice;

                ShowBuffer = IntermediateBuffer.FinalResult;

                m_spriteBatch = new SpriteBatch(graphicsDevice);

                m_bloomExtractEffect = new MyEffectBloomExtract(MyGuiScreenGamePlay.Static.GameBaseContentManager, "Effects\\BloomPostprocess\\MyBloomExtract");
                m_bloomCombineEffect = new MyEffectBloomCombine(MyGuiScreenGamePlay.Static.GameBaseContentManager, "Effects\\BloomPostprocess\\MyBloomCombine");
                m_gaussianBlurEffect = new MyEffectGaussianBlur(MyGuiScreenGamePlay.Static.GameBaseContentManager, "Effects\\BloomPostprocess\\MyGaussianBlur");

                // Look up the sample weight and offset effect parameters.
                m_weightsParameter = m_gaussianBlurEffect.SampleWeights;
                m_offsetsParameter = m_gaussianBlurEffect.SampleOffsets;

                // Look up how many samples our gaussian blur effect supports.
                m_sampleCount = m_weightsParameter.Elements.Count;

                // Create temporary arrays for computing our filter settings.
                m_sampleWeights = new float[m_sampleCount];
                m_sampleOffsets = new Vector2[m_sampleCount];

                // Look up the resolution and format of our main backbuffer.
                PresentationParameters pp = graphicsDevice.PresentationParameters;

                int width = pp.BackBufferWidth;
                int height = pp.BackBufferHeight;

                SurfaceFormat format = pp.BackBufferFormat;

                // Create a texture for reading back the backbuffer contents.
                m_resolveTarget = new ResolveTexture2D(graphicsDevice, width, height, 1, format);

                // Create two rendertargets for the bloom processing. These are half the
                // size of the backbuffer, in order to minimize fillrate costs. Reducing
                // the resolution in this way doesn't hurt quality, because we are going
                // to be blurring the bloom images in any case.
                width /= 2;
                height /= 2;

                if (MyVideoModeManager.GetAntiAliasing() == MyAntiAliasingEnum.OFF)
                {
                    m_renderTarget1 = new RenderTarget2D(graphicsDevice, width, height, 1, format);
                    m_renderTarget2 = new RenderTarget2D(graphicsDevice, width, height, 1, format);
                }
                else
                {
                    m_renderTarget1 = new RenderTarget2D(graphicsDevice, width, height, 1, format, pp.MultiSampleType, pp.MultiSampleQuality);
                    m_renderTarget2 = new RenderTarget2D(graphicsDevice, width, height, 1, format, pp.MultiSampleType, pp.MultiSampleQuality);
                }
            }

            m_loaded = true;
        }

        //  This is where it all happens. Grabs a scene that has already been rendered,
        //  and uses postprocess magic to add a glowing bloom effect over the top of it.
        public static void Draw()
        {
            if (MyVideoModeManager.GetCinematic() == false) return;

            LoadInDraw();

            GraphicsDevice graphicsDevice = MyMinerGame.Static.GraphicsDevice;

            // Resolve the scene into a texture, so we can
            // use it as input data for the bloom processing.
            graphicsDevice.ResolveBackBuffer(m_resolveTarget);

            // Pass 1: draw the scene into rendertarget 1, using a
            // shader that extracts only the brightest parts of the image.
            m_bloomExtractEffect.BloomThreshold.SetValue(m_bloomThreshold);

            DrawFullscreenQuad(m_resolveTarget, m_renderTarget1, m_bloomExtractEffect, IntermediateBuffer.PreBloom);

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)m_renderTarget1.Width, 0);

            DrawFullscreenQuad(m_renderTarget1, m_renderTarget2, m_gaussianBlurEffect, IntermediateBuffer.BlurredHorizontally);

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)m_renderTarget1.Height);

            DrawFullscreenQuad(m_renderTarget2, m_renderTarget1, m_gaussianBlurEffect, IntermediateBuffer.BlurredBothWays);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            graphicsDevice.SetRenderTarget(0, null);

            m_bloomCombineEffect.BloomIntensity.SetValue(m_bloomIntensity);
            m_bloomCombineEffect.BaseIntensity.SetValue(m_baseIntensity);
            m_bloomCombineEffect.BloomSaturation.SetValue(m_bloomSaturation);
            m_bloomCombineEffect.BaseSaturation.SetValue(m_baseSaturation);

            m_bloomCombineEffect.Timer.SetValue((float)(Environment.TickCount % 10000));

            graphicsDevice.Textures[1] = m_resolveTarget;

            Viewport viewport = graphicsDevice.Viewport;

            DrawFullscreenQuad(m_renderTarget1,
                               viewport.Width, viewport.Height,
                               m_bloomCombineEffect,
                               IntermediateBuffer.FinalResult);
        }

        //  Helper for drawing a texture into a rendertarget, using
        //  a custom shader to apply postprocessing effects.
        static void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, MyEffect effect, IntermediateBuffer currentBuffer)
        {
            GraphicsDevice graphicsDevice = MyMinerGame.Static.GraphicsDevice;
            graphicsDevice.SetRenderTarget(0, renderTarget);
            DrawFullscreenQuad(texture, renderTarget.Width, renderTarget.Height, effect, currentBuffer);
            graphicsDevice.SetRenderTarget(0, null);
        }

        //  Helper for drawing a texture into the current rendertarget,
        //  using a custom shader to apply postprocessing effects.
        static void DrawFullscreenQuad(Texture2D texture, int width, int height, MyEffect effect, IntermediateBuffer currentBuffer)
        {
            m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);

            // Begin the custom effect, if it is currently enabled. If the user
            // has selected one of the show intermediate buffer options, we still
            // draw the quad to make sure the image will end up on the screen,
            // but might need to skip applying the custom pixel shader.
            if (ShowBuffer >= currentBuffer)
            {
                effect.Effect.Begin();
                effect.Effect.CurrentTechnique.Passes[0].Begin();
            }

            // Draw the quad.
            m_spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            m_spriteBatch.End();

            MyRenderStatePool.RestoreAfterSpriteBatch();

            // End the custom effect.
            if (ShowBuffer >= currentBuffer)
            {
                effect.Effect.CurrentTechnique.Passes[0].End();
                effect.Effect.End();
            }
        }

        //  Computes sample weightings and texture coordinate offsets
        //  for one pass of a separable gaussian blur filter.
        static void SetBlurEffectParameters(float dx, float dy)
        {
            // The first sample always has a zero offset.
            m_sampleWeights[0] = ComputeGaussian(0);
            m_sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = m_sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < m_sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                m_sampleWeights[i * 2 + 1] = weight;
                m_sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                m_sampleOffsets[i * 2 + 1] = delta;
                m_sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < m_sampleWeights.Length; i++)
            {
                m_sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            m_weightsParameter.SetValue(m_sampleWeights);
            m_offsetsParameter.SetValue(m_sampleOffsets);
        }

        //  Evaluates a single point on the gaussian falloff curve.
        //  Used for setting up the blur filter weightings.
        static float ComputeGaussian(float n)
        {
            float theta = m_blurAmount;
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) * Math.Exp(-(n * n) / (2 * theta * theta)));
        }*/
    }
}
