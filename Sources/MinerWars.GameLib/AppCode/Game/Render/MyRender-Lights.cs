#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;

using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render.EnvironmentMap;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

#endregion

namespace MinerWars.AppCode.Game.Render
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

    static partial class MyRender
    {
        #region Lights

        static public void UpdateForwardLights()
        {
            const float RADIUS_FOR_LIGHTS = 400;

            BoundingSphere sphere = new BoundingSphere(MyCamera.Position, RADIUS_FOR_LIGHTS);

            GetRenderProfiler().StartProfilingBlock("Setup lights");

            MyEffectModelsDNS effectDNS = (MyEffectModelsDNS)GetEffect(MyEffects.ModelDNS);
            MyLights.UpdateEffect(effectDNS.DynamicLights, ref sphere, true);
            MyLights.UpdateEffectReflector(effectDNS.Reflector, true);

            MyEffectVoxels effectVoxels = (MyEffectVoxels)GetEffect(MyEffects.VoxelsMRT);
            MyLights.UpdateEffect(effectVoxels.DynamicLights, ref sphere, true);
            MyLights.UpdateEffectReflector(effectVoxels.Reflector, true);

            MyEffectDecals effectDecals = (MyEffectDecals)GetEffect(MyEffects.Decals);
            MyLights.UpdateEffect(effectDecals.DynamicLights, ref sphere, true);
            MyLights.UpdateEffectReflector(effectDecals.Reflector, true);

            GetRenderProfiler().EndProfilingBlock(); //Setup lights
        }


        static void RenderSpotLight(MyLightRenderElement lightElement, MyEffectPointLight effectPointLight)
        {
            MyLight light = lightElement.Light;

            Matrix lightViewProjectionShadow = Matrix.Identity;

            // Always cull clockwise (render inner parts of object), depth test is done in PS using light radius and cone angle
            RasterizerState.CullClockwise.Apply();
            DepthStencilState.None.Apply();

            //m_device.BlendState = BlendState.Additive;
            //Need to use max because of overshinning places where multiple lights shine
            MyStateObjects.Light_Combination_BlendState.Apply();

            if (lightElement.RenderShadows && lightElement.ShadowMap != null)
            {
                m_spotShadowRenderer.SetupSpotShadowBaseEffect(effectPointLight, lightElement.ShadowLightViewProjection, lightElement.ShadowMap);
            }
            effectPointLight.SetNearSlopeBiasDistance(4);

            effectPointLight.SetLightPosition(light.Position);
            effectPointLight.SetLightIntensity(light.Intensity);
            effectPointLight.SetSpecularLightColor(light.SpecularColor);
            effectPointLight.SetFalloff(light.Falloff);

            effectPointLight.SetLightViewProjection(lightElement.View * lightElement.Projection);
            effectPointLight.SetReflectorDirection(light.ReflectorDirection);
            effectPointLight.SetReflectorConeMaxAngleCos(1 - light.ReflectorConeMaxAngleCos);
            effectPointLight.SetReflectorColor(light.ReflectorColor);
            effectPointLight.SetReflectorRange(light.ReflectorRange);
            effectPointLight.SetReflectorIntensity(light.ReflectorIntensity);
            effectPointLight.SetReflectorTexture(light.ReflectorTexture);
            effectPointLight.SetReflectorFalloff(light.ReflectorFalloff);

            if (lightElement.RenderShadows)
                effectPointLight.SetTechnique(effectPointLight.DefaultSpotShadowTechnique);
            else
                effectPointLight.SetTechnique(effectPointLight.DefaultSpotTechnique);

            MySimpleObjectDraw.DrawConeForLight(effectPointLight, lightElement.World);
        }


        static void RenderSpotLights(List<MyLightRenderElement> spotLightElements, MyEffectPointLight effectPointLight)
        {
            if (spotLightElements.Count == 0)
            {
                return;
            }

            spotLightElements.Sort(MyLightRenderElement.SpotComparer); // Sort by texture
            var lastTexture = spotLightElements[0].Light.ReflectorTexture;

            m_renderProfiler.StartProfilingBlock("RenderSpotLightList");
            foreach (var spotElement in spotLightElements)
            {
                RenderSpotLight(spotElement, effectPointLight);
                lastTexture = spotElement.Light.ReflectorTexture;
            }
            m_renderProfiler.EndProfilingBlock();
        }

        private static void AddSpotLightRenderElement(MyLight light)
        {
            float cosAngle = 1 - light.ReflectorConeMaxAngleCos;

            // Near clip is 5 to prevent cockpit bugs
            float nearClip = 5;
            float c = nearClip / cosAngle;

            // 'a' is "screen size" at near clip (a, c and nearclip makes right triangle)
            float a = (float)Math.Sqrt(c * c - nearClip * nearClip);
            if (nearClip < light.ReflectorRange)
            {
                Matrix lightView = Matrix.CreateLookAt(light.Position, light.Position + light.ReflectorDirection, light.ReflectorUp);

                float distanceSquared = Vector3.DistanceSquared(MyCamera.Position, light.Position);

                bool drawShadows = DrawPlayerLightShadow || (PlayerLight != light);
                drawShadows &= distanceSquared < light.ShadowDistance * light.ShadowDistance * MyRenderConstants.RenderQualityProfile.SpotShadowsMaxDistanceMultiplier;

                //drawShadows &= distanceSquared < 200*200 * MyRenderConstants.RenderQualityProfile.SpotShadowsMaxDistanceMultiplier;

                Matrix lightProjection = Matrix.CreatePerspectiveOffCenter(-a, a, -a, a, nearClip, light.ReflectorRange);

                bool renderShadows = EnableSpotShadows && MyRender.CurrentRenderSetup.EnableSmallLightShadows.Value && drawShadows ;

                MyLightRenderElement lightElement = null;
                lightElement = m_spotLightsPool.Allocate(true);
                Debug.Assert(lightElement != null, "Out of lights, increase pool");
                if (lightElement != null)
                {
                    lightElement.Light = light;
                    lightElement.World = light.SpotWorld;
                    lightElement.View = lightView;
                    lightElement.Projection = lightProjection;
                    lightElement.RenderShadows = renderShadows;
                    lightElement.BoundingBox = light.SpotBoundingBox;
                    lightElement.UseReflectorTexture = (light.LightOwner == MyLight.LightOwnerEnum.SmallShip);

                    if (renderShadows)
                    {
                        Matrix lightViewProjectionShadow = m_spotShadowRenderer.CreateViewProjectionMatrix(lightView, a, nearClip, light.ReflectorRange);
                        lightElement.ShadowLightViewProjection = lightViewProjectionShadow;
                    }
                    m_spotLightRenderElements.Add(lightElement);
                }
            }
        }

        private static void PrepareLights()
        {
            m_renderProfiler.StartProfilingBlock("Prepare lights");

            // Select small lights and do frustum check
            if (MyRender.CurrentRenderSetup.EnableSmallLights.Value)
            {
                List<MyLight> usedLights = null;
                if (CurrentRenderSetup.LightsToUse == null)
                {
                    var frustum = MyCamera.GetBoundingFrustum();
                    MyLights.UpdateSortedLights(ref frustum);
                    usedLights = MyLights.GetSortedLights();
                }
                else
                {
                    usedLights = CurrentRenderSetup.LightsToUse;
                }

                m_pointLights.Clear();
                m_hemiLights.Clear();
                m_spotLightRenderElements.Clear();
                m_spotLightsPool.DeallocateAll();
                foreach (var light in usedLights)
                {
                    if (light.LightOn) // Light is on
                    {
                        if ((light.LightType & MyLight.LightTypeEnum.PointLight) != 0 && (light.LightType & MyLight.LightTypeEnum.Hemisphere) == 0) // Light is point
                        {
                            if (light.IsPointLightInFrustum())
                            {
                                m_pointLights.Add(light);
                            }
                        }
                        if ((light.LightType & MyLight.LightTypeEnum.Hemisphere) != 0) // Light is hemi
                        {
                            if (light.IsPointLightInFrustum())
                            {
                                m_hemiLights.Add(light);
                            }
                        }
                        if ((light.LightType & MyLight.LightTypeEnum.Spotlight) != 0 && light.ReflectorOn) // Light is spot
                        {
                            if (light.IsSpotLightInFrustum())
                            {
                                AddSpotLightRenderElement(light);
                            }
                        }
                    }
                }
            }

            m_renderProfiler.EndProfilingBlock();
        }

        private static void RenderSpotShadows()
        {
            // Render spot shadows (for first n spot lights, lights are sorted by camera distance)
            if (MyRender.CurrentRenderSetup.EnableSmallLights.Value && MyRender.EnableSpotShadows)
            {
                m_renderProfiler.StartProfilingBlock("Render spot shadows");
                int currentShadowTarget = 0;
                foreach (var spotElement in m_spotLightRenderElements)
                {
                    if (currentShadowTarget >= m_spotShadowRenderTargets.Length)
                    {
                        spotElement.ShadowMap = null;
                        continue;
                    }
                    if (spotElement.RenderShadows)
                    {
                        Texture shadowMapRt = (Texture)m_spotShadowRenderTargets[currentShadowTarget];
                        Texture shadowMapDepthRt = (Texture)m_spotShadowRenderTargetsZBuffers[currentShadowTarget];
                        m_spotShadowRenderer.RenderForLight(spotElement.ShadowLightViewProjection, ref spotElement.BoundingBox, shadowMapRt, shadowMapDepthRt, currentShadowTarget);
                        spotElement.ShadowMap = shadowMapRt;
                        currentShadowTarget++;
                    }
                }
                m_renderProfiler.EndProfilingBlock();
            }
        }

        internal static void RenderLights()
        {
            PrepareLights();

            RenderSpotShadows();

            m_renderProfiler.StartProfilingBlock("Render lights");
            MyMinerGame.SetRenderTarget(GetRenderTarget(MyRenderTargets.Auxiliary1), null, SetDepthTargetEnum.RestoreDefault);
            MyMinerGame.Static.GraphicsDevice.Clear(ClearFlags.Target, new ColorBGRA(0.0f), 1, 0);

            SetCorrectViewportSize();

            if (MyRender.CurrentRenderSetup.EnableSmallLights.Value)
            {
                MyEffectPointLight effectPointLight = (MyEffectPointLight) MyRender.GetEffect(MyEffects.PointLight);
                Texture diffuseRT = MyRender.GetRenderTarget(MyRenderTargets.Diffuse);
                effectPointLight.SetNormalsRT(MyRender.GetRenderTarget(MyRenderTargets.Normals));
                effectPointLight.SetDiffuseRT(diffuseRT);
                effectPointLight.SetDepthsRT(MyRender.GetRenderTarget(MyRenderTargets.Depth));
                effectPointLight.SetHalfPixel(diffuseRT.GetLevelDescription(0).Width, diffuseRT.GetLevelDescription(0).Height);
                effectPointLight.SetScale(GetScaleForViewport(diffuseRT));

                Matrix invViewProjMatrix = Matrix.Invert(MyCamera.ViewProjectionMatrix);
                Matrix invViewMatrix = Matrix.Invert(MyCamera.ViewMatrix);

                effectPointLight.SetCameraPosition(MyCamera.Position);
                effectPointLight.SetViewMatrix(MyCamera.ViewMatrix);
                effectPointLight.SetInvViewMatrix(invViewMatrix);

                DepthStencilState.None.Apply();
                MyStateObjects.Light_Combination_BlendState.Apply();

                //Render each light with a model specific to the light
                m_renderProfiler.StartProfilingBlock("PointLight");

                var cullRationSq = MyRenderConstants.DISTANCE_LIGHT_CULL_RATIO * MyRenderConstants.DISTANCE_LIGHT_CULL_RATIO;
                          
                effectPointLight.SetTechnique(effectPointLight.DefaultTechnique);
                foreach (MyLight light in m_pointLights)
                {
                    float distanceSq = Vector3.DistanceSquared(MyCamera.Position, light.PositionWithOffset);
                    var hasVolumetricGlare = light.GlareOn && light.Glare.Type == MyLightGlare.GlareTypeEnum.Distant;
                    var isTooFarAway = (light.Range * light.Range) < (distanceSq / cullRationSq);

                    if (!isTooFarAway)
                    {
                        // Always cull clockwise (render inner parts of object), depth test is done is PS using light radius
                        RasterizerState.CullClockwise.Apply();

                        effectPointLight.SetLightPosition(light.PositionWithOffset);
                        effectPointLight.SetLightIntensity(light.Intensity);
                        effectPointLight.SetSpecularLightColor(light.SpecularColor);
                        effectPointLight.SetFalloff(light.Falloff);

                        effectPointLight.SetLightRadius(light.Range);
                        effectPointLight.SetReflectorTexture(light.ReflectorTexture);
                        effectPointLight.SetLightColor(new Vector3(light.Color.X, light.Color.Y, light.Color.Z));
                        effectPointLight.SetTechnique(effectPointLight.DefaultTechnique);
                        MySimpleObjectDraw.DrawSphereForLight(effectPointLight, ref light.PositionWithOffset, light.Range, ref MyMath.Vector3One, 1);
                        MyPerformanceCounter.PerCameraDraw.LightsCount++;
                    }
                    if(!isTooFarAway || hasVolumetricGlare)
                        light.Draw();
                }


                m_renderProfiler.EndProfilingBlock();


                m_renderProfiler.StartProfilingBlock("Hemisphere");

                foreach (MyLight light in m_hemiLights)
                {
                    // compute bounding box
                    //Vector3 center = light.Position;// - light.Range * new Vector3(0,1,0);
                    //Vector3 extend = new Vector3(light.Range, light.Range, light.Range);
                    //m_lightBoundingBox.Min = center - extend;
                    //m_lightBoundingBox.Max = center + extend;
                    // Always cull clockwise (render inner parts of object), depth test is done is PS using light radius
                    if (Vector3.Dot(light.ReflectorDirection, MyCamera.Position - light.Position) > 0 && light.PointBoundingSphere.Contains(MyCamera.Position) == MinerWarsMath.ContainmentType.Contains)
                    {
                        RasterizerState.CullNone.Apply(); //zevnitr
                    }
                    else
                    {
                        RasterizerState.CullCounterClockwise.Apply(); //zvenku
                    }

                    effectPointLight.SetLightPosition(light.Position);
                    effectPointLight.SetLightIntensity(light.Intensity);
                    effectPointLight.SetSpecularLightColor(light.SpecularColor);
                    effectPointLight.SetFalloff(light.Falloff);

                    effectPointLight.SetLightRadius(light.Range);
                    effectPointLight.SetReflectorTexture(light.ReflectorTexture);
                    effectPointLight.SetLightColor(new Vector3(light.Color.X, light.Color.Y, light.Color.Z));
                    effectPointLight.SetTechnique(effectPointLight.DefaultHemisphereTechnique);

                    Matrix world = Matrix.CreateScale(light.Range) * Matrix.CreateWorld(light.Position, light.ReflectorDirection, light.ReflectorUp);
                    MySimpleObjectDraw.DrawHemisphereForLight(effectPointLight, ref world, ref MyMath.Vector3One, 1);
                    light.Draw();

                    MyPerformanceCounter.PerCameraDraw.LightsCount++;
                }                
                m_renderProfiler.EndProfilingBlock();


                m_renderProfiler.StartProfilingBlock("Spotlight");
                RenderSpotLights(m_spotLightRenderElements, effectPointLight);

                m_renderProfiler.EndProfilingBlock();

                if (EnableSpectatorReflector && DrawSpectatorReflector && SpectatorReflector != null && SpectatorReflector.LightOn && SpectatorReflector.ReflectorOn)
                {
                    SpectatorReflector.ReflectorDirection = MyCamera.ForwardVector;
                    SpectatorReflector.ReflectorUp = MyCamera.UpVector;
                    SpectatorReflector.SetPosition(MyCamera.Position);

                    effectPointLight.SetLightPosition(SpectatorReflector.Position);
                    effectPointLight.SetReflectorTexture(null);
                    effectPointLight.SetReflectorDirection(SpectatorReflector.ReflectorDirection);
                    effectPointLight.SetReflectorConeMaxAngleCos(1 - SpectatorReflector.ReflectorConeMaxAngleCos);
                    effectPointLight.SetReflectorColor(SpectatorReflector.ReflectorColor);
                    effectPointLight.SetReflectorRange(SpectatorReflector.ReflectorRange);
                    effectPointLight.SetCameraPosition(MyCamera.Position);

                    // Special case, for camera reflector
                    effectPointLight.SetReflectorIntensity(MyMinerShipConstants.MINER_SHIP_NEAR_REFLECTOR_INTENSITY * MySmallShip.ReflectorIntensityMultiplier);
                    effectPointLight.SetReflectorFalloff(MyMinerShipConstants.MINER_SHIP_NEAR_REFLECTOR_FALLOFF);

                    effectPointLight.SetTechnique(effectPointLight.DefaultSpotTechnique);
                    MySimpleObjectDraw.DrawConeForLight(effectPointLight, SpectatorReflector.SpotWorld);


                    // Always cull clockwise (render inner parts of object), depth test is done is PS using light radius
                    RasterizerState.CullClockwise.Apply();

                    effectPointLight.SetLightIntensity(MyMinerShipConstants.MINER_SHIP_NEAR_LIGHT_INTENSITY);
                    effectPointLight.SetSpecularLightColor(Color.White.ToVector3());
                    effectPointLight.SetFalloff(1.0f);

                    effectPointLight.SetLightRadius(MyMinerShipConstants.MINER_SHIP_NEAR_LIGHT_RANGE);
                    effectPointLight.SetLightColor(new Color(MyReflectorConstants.SHORT_REFLECTOR_LIGHT_COLOR).ToVector3());
                    effectPointLight.SetTechnique(effectPointLight.DefaultSpotTechnique);

                    MySimpleObjectDraw.DrawSphereForLight(effectPointLight, ref MyCamera.Position, MyMinerShipConstants.MINER_SHIP_NEAR_LIGHT_RANGE, ref MyMath.Vector3One, 1);
                    MyPerformanceCounter.PerCameraDraw.LightsCount++;

                }
            }
             
            DepthStencilState.None.Apply();
            RasterizerState.CullCounterClockwise.Apply();
            
            MyStateObjects.Sun_Combination_BlendState.Apply();

            m_renderProfiler.StartProfilingBlock("Sun light");

            if (EnableSun && CurrentRenderSetup.EnableSun.Value)
            {
                //Sun light
                MyEffectDirectionalLight effectDirectionalLight = MyRender.GetEffect(MyEffects.DirectionalLight) as MyEffectDirectionalLight;
                Texture diffuseRTSun = MyRender.GetRenderTarget(MyRenderTargets.Diffuse);
                effectDirectionalLight.SetNormalsRT(MyRender.GetRenderTarget(MyRenderTargets.Normals));
                effectDirectionalLight.SetDiffuseRT(diffuseRTSun);
                effectDirectionalLight.SetDepthsRT(MyRender.GetRenderTarget(MyRenderTargets.Depth));
                effectDirectionalLight.SetHalfPixelAndScale(diffuseRTSun.GetLevelDescription(0).Width, diffuseRTSun.GetLevelDescription(0).Height, GetScaleForViewport(diffuseRTSun));

                effectDirectionalLight.SetCameraMatrix(Matrix.Invert(MyCamera.ViewMatrix));

                effectDirectionalLight.SetAmbientMinimumAndIntensity(new Vector4(AmbientColor * AmbientMultiplier, EnvAmbientIntensity));
                effectDirectionalLight.SetTextureEnvironmentMain(MyEnvironmentMap.EnvironmentMainMap);
                effectDirectionalLight.SetTextureEnvironmentAux(MyEnvironmentMap.EnvironmentAuxMap);
                effectDirectionalLight.SetTextureAmbientMain(MyEnvironmentMap.AmbientMainMap);
                effectDirectionalLight.SetTextureAmbientAux(MyEnvironmentMap.AmbientAuxMap);
                effectDirectionalLight.SetTextureEnvironmentBlendFactor(MyEnvironmentMap.BlendFactor);
                effectDirectionalLight.SetCameraPosition(MyCamera.Position);

                //Set distance where no slope bias will be applied (because of cockpit artifacts)
                effectDirectionalLight.SetNearSlopeBiasDistance(3);

                effectDirectionalLight.ShowSplitColors(ShowCascadeSplits);
                effectDirectionalLight.SetShadowBias(0.0001f * MyRenderConstants.RenderQualityProfile.ShadowBiasMultiplier);
                effectDirectionalLight.SetSlopeBias(0.00002f);
                effectDirectionalLight.SetSlopeCascadeMultiplier(20.0f); //100 makes artifacts in prefabs

                MyRender.GetShadowRenderer().SetupShadowBaseEffect(effectDirectionalLight);

                effectDirectionalLight.SetLightDirection(-m_sun.Direction); //*-1 because of shader opts
                effectDirectionalLight.SetLightColorAndIntensity(new Vector3(m_sun.Color.X, m_sun.Color.Y, m_sun.Color.Z), m_sun.Intensity);
                effectDirectionalLight.SetBacklightColorAndIntensity(new Vector3(m_sun.BackColor.X, m_sun.BackColor.Y, m_sun.BackColor.Z), m_sun.BackIntensity);
                //m_sun.SpecularColor = {X:0,9137255 Y:0,6078432 Z:0,2078431} //nice yellow
                effectDirectionalLight.SetSpecularLightColor(m_sun.SpecularColor);
                effectDirectionalLight.EnableCascadeBlending(MyRenderConstants.RenderQualityProfile.EnableCascadeBlending);

                effectDirectionalLight.SetFrustumCorners(MyRender.GetShadowRenderer().GetFrustumCorners());

                effectDirectionalLight.SetEnableAmbientEnvironment(EnableEnvironmentMapAmbient && MyRenderConstants.RenderQualityProfile.EnableEnvironmentals && CurrentRenderSetup.EnableEnvironmentMapping.Value);
                effectDirectionalLight.SetEnableReflectionEnvironment(EnableEnvironmentMapReflection && MyRenderConstants.RenderQualityProfile.EnableEnvironmentals && CurrentRenderSetup.EnableEnvironmentMapping.Value);

                if (EnableShadows && MyRender.CurrentRenderSetup.ShadowRenderer != null)
                    effectDirectionalLight.SetTechnique(effectDirectionalLight.DefaultTechnique);
                else
                    effectDirectionalLight.SetTechnique(effectDirectionalLight.DefaultWithoutShadowsTechnique);

                MyGuiManager.GetFullscreenQuad().Draw(effectDirectionalLight);
            }
            m_renderProfiler.EndProfilingBlock();

            // Blend in background
            if (true) // blend background
            {                  
                m_renderProfiler.StartProfilingBlock("Blend background");
                if (MyFakes.RENDER_PREVIEWS_WITH_CORRECT_ALPHA)
                {
                    // for some reason the other option does not give 0 alpha for the background when rendering gui preview images
                    MyStateObjects.Additive_NoAlphaWrite_BlendState.Apply();
                }
                else
                {
                    MyStateObjects.NonPremultiplied_NoAlphaWrite_BlendState.Apply();
                    //BlendState.NonPremultiplied.Apply();
                }
                DepthStencilState.None.Apply();
                RasterizerState.CullCounterClockwise.Apply();
                         
                MyEffectBlendLights effectBlendLights = MyRender.GetEffect(MyEffects.BlendLights) as MyEffectBlendLights;
                Texture diffuseRT = GetRenderTarget(MyRenderTargets.Diffuse);
                MyCamera.SetupBaseEffect(effectBlendLights, m_currentSetup.FogMultiplierMult);
                effectBlendLights.SetDiffuseTexture(diffuseRT);
                effectBlendLights.SetNormalTexture(GetRenderTarget(MyRenderTargets.Normals));
                effectBlendLights.SetDepthTexture(GetRenderTarget(MyRenderTargets.Depth));
                effectBlendLights.SetHalfPixel(diffuseRT.GetLevelDescription(0).Width, diffuseRT.GetLevelDescription(0).Height);
                effectBlendLights.SetScale(GetScaleForViewport(diffuseRT));
                effectBlendLights.SetBackgroundTexture(GetRenderTarget(MyRenderTargets.Auxiliary0));

                effectBlendLights.SetTechnique(effectBlendLights.DefaultTechnique);

                MyGuiManager.GetFullscreenQuad().Draw(effectBlendLights);
                m_renderProfiler.EndProfilingBlock();

                // Blend in emissive light, overwrite emissivity (alpha)
                m_renderProfiler.StartProfilingBlock("Copy emisivity");

                if (MyPostProcessHDR.RenderHDRThisFrame())
                    MyStateObjects.AddEmissiveLight_BlendState.Apply();
                else
                    MyStateObjects.AddEmissiveLight_NoAlphaWrite_BlendState.Apply();

                effectBlendLights.SetTechnique(effectBlendLights.CopyEmissivityTechnique);
                MyGuiManager.GetFullscreenQuad().Draw(effectBlendLights);


                bool showDebugLighting = false;

                if (ShowSpecularIntensity)
                {
                    effectBlendLights.SetTechnique(MyEffectBlendLights.Technique.OnlySpecularIntensity);
                    showDebugLighting = true;
                }
                else
                    if (ShowSpecularPower)
                    {
                        effectBlendLights.SetTechnique(MyEffectBlendLights.Technique.OnlySpecularPower);
                        showDebugLighting = true;
                    }
                    else
                        if (ShowEmissivity)
                        {
                            effectBlendLights.SetTechnique(MyEffectBlendLights.Technique.OnlyEmissivity);
                            showDebugLighting = true;
                        }
                        else
                            if (ShowReflectivity)
                            {
                                effectBlendLights.SetTechnique(MyEffectBlendLights.Technique.OnlyReflectivity);
                                showDebugLighting = true;
                            }

                if (showDebugLighting)
                {
                    BlendState.Opaque.Apply();
                    MyGuiManager.GetFullscreenQuad().Draw(effectBlendLights);
                }

                m_renderProfiler.EndProfilingBlock();
            }

            //TakeScreenshot("Accumulated_lights", GetRenderTarget(MyRenderTargets.Lod0Depth), MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
            /*TakeScreenshot("EnvironmentMap_1", GetRenderTargetCube(MyRenderTargets.EnvironmentCube), MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
            TakeScreenshot("EnvironmentMap_2", GetRenderTargetCube(MyRenderTargets.EnvironmentCubeAux), MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
            TakeScreenshot("AmbientMap_1", GetRenderTargetCube(MyRenderTargets.AmbientCube), MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
            TakeScreenshot("AmbientMap_2", GetRenderTargetCube(MyRenderTargets.AmbientCubeAux), MyEffectScreenshot.ScreenshotTechniqueEnum.Default);
              */
            m_renderProfiler.EndProfilingBlock();
        }

        #endregion
    }
}