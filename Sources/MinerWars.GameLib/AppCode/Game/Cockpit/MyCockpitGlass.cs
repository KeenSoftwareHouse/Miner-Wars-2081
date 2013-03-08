using MinerWarsMath;
//using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Entities;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Entities.Ships.SubObjects;
using MinerWars.AppCode.Game.TransparentGeometry;

using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;

//  Draws cockpit glass (scratches, etc). Left, front and right window.

namespace MinerWars.AppCode.Game.Cockpit
{
    static class MyCockpitGlass
    {
        //Bounding sphere used to calculate nearby lights
        static BoundingSphere m_boundingSphereForLights = new BoundingSphere();


        static MyCockpitGlass()
        {
            // Cockpit interior drawn like entity under small ship
            MyRender.RegisterRenderModule(MyRenderModuleEnum.CockpitGlass, "Cockpit glass", Draw, MyRenderStage.AlphaBlend, 200, true);
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyCockpitGlass.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyCockpitGlass::LoadContent");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();    
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyCockpitGlass.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyCockpitGlass.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyCockpitGlass.UnloadContent - END");
        }


        public static bool CanDrawCockpit()
        {
            return MyCockpitWeapons.CanDrawCockpitWeapons() && MySession.PlayerShip.Config.ViewMode.Current == MyViewModeTypesEnum.CockpitOn;
        }

        public static void Draw()
        {         
            if (!CanDrawCockpit())
                return;

            MyModel model = MyModels.GetModelForDraw(MySession.PlayerShip.CockpitGlassModelEnum);

            RasterizerState.CullNone.Apply();

            if (MyRenderConstants.RenderQualityProfile.ForwardRender)
                DepthStencilState.DepthRead.Apply();
            else
            {
                 MyStateObjects.DepthStencil_StencilReadOnly.Apply();
            }

            BlendState.NonPremultiplied.Apply();

            MyEffectCockpitGlass effect = (MyEffectCockpitGlass)MyRender.GetEffect(MyEffects.CockpitGlass);

            float glassDirtAlpha = MathHelper.Lerp(MyCockpitGlassConstants.GLASS_DIRT_MIN, MyCockpitGlassConstants.GLASS_DIRT_MAX,
                MySession.PlayerShip.GlassDirtLevel);
            effect.SetGlassDirtLevelAlpha(new Vector4(glassDirtAlpha, 0,0,0));

            effect.SetWorldMatrix(MySession.PlayerShip.PlayerHeadForCockpitInteriorWorldMatrix);
            effect.SetViewMatrix(MyCamera.ViewMatrix);

            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                Matrix projection = MyCamera.ProjectionMatrixForNearObjects;

                effect.SetWorldViewProjectionMatrix(MySession.PlayerShip.PlayerHeadForCockpitInteriorWorldMatrix * MyCamera.ViewMatrixAtZero * projection);
            }
            else
            {
                effect.SetWorldViewProjectionMatrix(MySession.PlayerShip.PlayerHeadForCockpitInteriorWorldMatrix * MyCamera.ViewProjectionMatrixAtZero);
            }

            MyMeshMaterial cockpitMaterial = model.GetMeshList()[0].Materials[0];
            cockpitMaterial.PreloadTexture();
            effect.SetCockpitGlassTexture(cockpitMaterial.DiffuseTexture);

            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                Texture depthRT = MyRender.GetRenderTarget(MyRenderTargets.Depth);
                effect.SetDepthTexture(depthRT);

                effect.SetHalfPixel(MyUtils.GetHalfPixel(depthRT.GetLevelDescription(0).Width, depthRT.GetLevelDescription(0).Height));
            }

            Vector4 sunColor = MySunWind.GetSunColor();
            effect.SetSunColor(new Vector3(sunColor.X, sunColor.Y, sunColor.Z));

            effect.SetDirectionToSun(MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized());

            effect.SetAmbientColor(Vector3.Zero);
            effect.SetReflectorPosition(MyCamera.Position - 4 * MySession.PlayerShip.WorldMatrix.Forward);

            if (MySession.PlayerShip.Light != null)
            {
                effect.SetNearLightColor(MySession.PlayerShip.Light.Color);
                effect.SetNearLightRange(MySession.PlayerShip.Light.Range);
            }

            MyRender.GetShadowRenderer().SetupShadowBaseEffect(effect);
            effect.SetShadowBias(0.001f);

            m_boundingSphereForLights.Center = MySession.PlayerShip.GetPosition();
            MyLights.UpdateEffect(effect, ref m_boundingSphereForLights, true);

            effect.Begin();
            model.Render();
            effect.End();

            MyCockpitGlassDecals.Draw(effect); 
        }
    }
}
