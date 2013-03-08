using System;

using Microsoft.Xna.Framework.Graphics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.App;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Lights;
using MinerWarsCustomContentImporters;
using MinerWars.AppCode.Game.GUI.Core;
using System.IO;
using MinerWars.AppCode.Game.GUI.Helpers;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Prefabs
{
    /// <summary>
    /// Used for rendering prefabs for thumbnail previews. 
    /// Uses three lights to light prefabs:
    /// 1. Key light - directional (sun)
    /// 2. Fill light - point
    /// 3. Back light - point
    /// </summary>
    class MyPrefabRenderer
    {
        readonly MyRender.MyRenderSetup m_setup = new MyRender.MyRenderSetup();
        RenderTarget2D m_fullSizeRT;

        public MyPrefabRenderer()
        {
            SetRenderSetup();
        }

        public void SetRenderTarget(RenderTarget2D fullSizeRT)
        {
            m_setup.RenderTargets[0] = new RenderTargetBinding(fullSizeRT);
            m_fullSizeRT = fullSizeRT;
        }

        private void SetRenderSetup()
        {
            m_setup.RenderTargets = new RenderTargetBinding[1];

            m_setup.DisabledModules.Add("Cockpit");
            m_setup.DisabledModules.Add("Cockpit glass");
            m_setup.DisabledModules.Add("Sun glow");
            m_setup.DisabledModules.Add("Sun glare and lens flare");
            m_setup.DisabledModules.Add("Update occlusions");
            m_setup.DisabledModules.Add("Transparent geometry");

            m_setup.SkippedRenderStages.Add(MyRenderStage.AlphaBlendPreHDR);
            m_setup.SkippedRenderStages.Add(MyRenderStage.AlphaBlend);
            m_setup.SkippedRenderStages.Add(MyRenderStage.DebugDraw);
            m_setup.SkippedRenderStages.Add(MyRenderStage.PrepareForDraw);

            m_setup.DisabledPostprocesses.Add("Flashes");
            m_setup.DisabledPostprocesses.Add("Volumetric SSAO 2");
            m_setup.DisabledPostprocesses.Add("Antialiasing");

            m_setup.BackgroundColor = new Color(0.17f, 0.18f, .25f, .5f);

            m_setup.EnableHDR = false;
            m_setup.EnableSun = true;
            m_setup.EnableSunShadows = true;
            m_setup.EnableSmallLights = true;
            m_setup.EnableDebugHelpers = false;
            m_setup.EnableEnvironmentMapping = false;

            m_setup.RenderElementsToDraw = new List<MyRender.MyRenderElement>();
            m_setup.TransparentRenderElementsToDraw = new List<MyRender.MyRenderElement>();
            m_setup.LightsToUse = new List<MyLight>();
            MyLight light = new MyLight();
            light.Start(MyLight.LightTypeEnum.PointLight, new Vector4(1, 0.95f, 0.8f, 1), 1, 119); // fill light
            m_setup.LightsToUse.Add(light);
            light = new MyLight();
            light.Start(MyLight.LightTypeEnum.PointLight, new Vector4(1, 0.9f, 0.6f, 1), 1, 119); // back light
            m_setup.LightsToUse.Add(light);
        }

        /// <summary>
        /// IMPORTANT: using Lod1Normals here, since this is done in a separate time to normal rendering
        /// </summary>
        public RenderTarget2D RenderPreview(MyMwcObjectBuilder_Prefab_TypesEnum enumValue, MyPrefabConfiguration config, int width, int height)
        {
            m_fullSizeRT = MyRender.GetRenderTarget(MyRenderTargets.Lod1Normals);

            if (m_fullSizeRT == null || MyGuiScreenGamePlay.Static == null)
                return null;

            m_setup.RenderTargets[0] = new RenderTargetBinding(m_fullSizeRT);

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = false;
            }

            GraphicsDevice device = MyMinerGame.Static.GraphicsDevice;

            RenderTarget2D renderTarget = new RenderTarget2D(device, width, height, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            m_setup.RenderElementsToDraw.Clear();
            m_setup.TransparentRenderElementsToDraw.Clear();

            // make actual viewport one pixel larger in order to remove the deformed border caused by antialiasing
            m_setup.Viewport = new Viewport(0, 0, 2 * renderTarget.Width, 2 * renderTarget.Height);
            m_setup.AspectRatio = 1;
            m_setup.Fov = MathHelper.ToRadians(70);

            MyRender.Sun.Direction = new Vector3(.5f, -.3f, -1);
            MyRender.Sun.Direction.Normalize();
            MyRender.Sun.Color = Vector4.One;
            MyRender.EnableSun = true;
            float previousSunIntensityMultiplier = MyRender.SunIntensityMultiplier;
            MyRender.SunIntensityMultiplier = 1.2f;

            if (config.BuildType == BuildTypesEnum.MODULES && config.CategoryType == CategoryTypesEnum.WEAPONRY)
            {
                MyModel baseModel = null;
                MyModel barrelModel = null;
                Matrix baseMatrix = Matrix.Identity;
                Matrix barrelMatrix = Matrix.Identity;

                bool result = MyLargeShipGunBase.GetVisualPreviewData(enumValue, ref baseModel, ref barrelModel, ref baseMatrix, ref barrelMatrix);

                if (result)
                {
                    m_setup.ViewMatrix = Matrix.Identity;

                    setupRenderElement(baseModel, baseMatrix);
                    setupRenderElement(barrelModel, barrelMatrix);

                    setupLights(baseModel);

                    MyRender.PushRenderSetup(m_setup);
                    MyRender.Draw();
                    MyRender.PopRenderSetup();

                    BlitToThumbnail(device, renderTarget);
                }
            }
            else
            {
                //load new model from prefab config
                MyModel model = MyModels.GetModelForDraw(config.ModelLod0Enum);

                float distance = 1.85f;

                Matrix viewMatrix = Matrix.Identity;
                m_setup.ViewMatrix = viewMatrix;

                //generate world matrix
                Matrix worldMatrix = Matrix.Identity;
                worldMatrix.Translation = -model.BoundingSphere.Center;
                worldMatrix *= Matrix.CreateRotationY(-.85f * MathHelper.PiOver4);
                worldMatrix *= Matrix.CreateRotationX(.35f * MathHelper.PiOver4);
                worldMatrix.Translation += new Vector3(0, 0, -model.BoundingSphere.Radius * distance);

                setupRenderElement(model, worldMatrix);

                setupLights(model);

                MyRender.PushRenderSetup(m_setup);
                MyRender.Draw();
                MyRender.PopRenderSetup();

                BlitToThumbnail(device, renderTarget);
            }

            MyRender.SunIntensityMultiplier = previousSunIntensityMultiplier;

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = true;
            }

            return renderTarget;
        }

        private void setupLights(MyModel baseModel)
        {
            m_setup.LightsToUse[0].Position = new Vector3(baseModel.BoundingSphere.Radius, -0.4f * baseModel.BoundingSphere.Radius, baseModel.BoundingSphere.Radius);
            m_setup.LightsToUse[0].Position += baseModel.BoundingSphere.Center;
            m_setup.LightsToUse[1].Position = new Vector3(1.3f * baseModel.BoundingSphere.Radius, 0, -baseModel.BoundingSphere.Radius);
            m_setup.LightsToUse[1].Position += baseModel.BoundingSphere.Center;
        }

        private void BlitToThumbnail(GraphicsDevice device, RenderTarget2D renderTarget)
        {
            device.SetRenderTarget(renderTarget);
            var screenEffect = MyRender.GetEffect(MyEffects.Scale) as MyEffectScale;
            Debug.Assert(screenEffect != null);
            screenEffect.SetTechnique(MyEffectScale.Technique.HWScalePrefabPreviews);
            screenEffect.SetSourceTextureMod(m_fullSizeRT);
            screenEffect.SetScale(2f * new Vector2(renderTarget.Width / (float)m_fullSizeRT.Width, renderTarget.Height / (float)m_fullSizeRT.Height));
            MyGuiManager.GetFullscreenQuad().Draw(screenEffect);
            device.SetRenderTarget(null);
        }

        private void setupRenderElement(MyModel model, Matrix worldMatrix)
        {
            model.LoadInDraw();

            foreach (MyMesh mesh in model.GetMeshList())
            {
                MyRender.MyRenderElement renderElement =
                    mesh.GetMaterial().DrawTechnique == MyMeshDrawTechnique.HOLO ?
                    MyRender.AddTransparentRenderElement(m_setup.TransparentRenderElementsToDraw) :
                    MyRender.AddRenderElement(m_setup.RenderElementsToDraw);
                if (renderElement == null)
                    return;

                renderElement.Entity = null;
                renderElement.DebugName = "";

                renderElement.VertexBuffer = model.VertexBuffer;
                renderElement.IndexBuffer = mesh.IndexBuffer;

                renderElement.WorldMatrixForDraw = worldMatrix;
                renderElement.WorldMatrix = worldMatrix;

                renderElement.BoundingBox = model.BoundingBox.Transform(renderElement.WorldMatrix);

                renderElement.Material = mesh.GetMaterial();
            }
        }

        /// <summary>
        /// Create all prefab previews as dds files
        /// Filename is derived from MyMwcObjectBuilder_Prefab_TypesEnum
        /// </summary>
        public void CreatePreviewsToFiles(string path, int sizeInPixels)
        {
            var directoryInfo = Directory.CreateDirectory(path);

            CreateDDSFiles(sizeInPixels, directoryInfo);

            // compress using nvDXT.exe to DXT1:
            CompressDDSFiles(directoryInfo);
        }

        public static string GetPreviewFileName(MyPrefabConfiguration config, MyMwcObjectBuilder_Prefab_TypesEnum enumValue)
        {
            if (config.BuildType == BuildTypesEnum.MODULES && config.CategoryType == CategoryTypesEnum.WEAPONRY)
            {
                MyModelsEnum baseModelEnum, barelModelEnum;
                if (MyLargeShipGunBase.GetModelEnums(enumValue, out baseModelEnum, out barelModelEnum))
                {
                    return Path.GetFileName(MyModels.GetModelAssetName(baseModelEnum));
                }
            }

            return Path.GetFileName(MyModels.GetModelAssetName(config.ModelLod0Enum));
        }

        private void CreateDDSFiles(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            int index = 1;
            foreach (MyMwcObjectBuilder_Prefab_TypesEnum enumValue in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                Debug.WriteLine(string.Format("Exporting Prefab Preview {0}/{1} ..", index++,
                                              MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues.Length));

                MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(enumValue);
                string lod0Name = GetPreviewFileName(config, enumValue);

                var result = RenderPreview(enumValue, config, sizeInPixels, sizeInPixels);
                string fileName = Path.Combine(directoryInfo.FullName, string.Format("{0}.dds", lod0Name));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                MyDDSFile.DDSToFile(fileName, true, result, false);
            }
        }

        private static void CompressDDSFiles(DirectoryInfo directoryInfo)
        {
            Process nvDXT = new Process();
            try
            {
                nvDXT.StartInfo.FileName = Path.Combine(@"C:\KeenSWH\MinerWars\MediaDevelopment\Tools\", "nvdxt.exe");
                nvDXT.StartInfo.Arguments = @"-all -dxt3";
                nvDXT.StartInfo.WorkingDirectory = directoryInfo.FullName;
                nvDXT.StartInfo.CreateNoWindow = true;
                Debug.WriteLine("Now compressing DDS files to DXT3.");
                nvDXT.Start();
                nvDXT.WaitForExit();

                // now trim the trailing underscore that nvDXT generates and move back to parent dir
                String[] oldFileNames = Directory.GetFiles(directoryInfo.FullName, "*_.dds");
                foreach (var oldName in oldFileNames)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(oldName);
                    var trimmedWithExtension = fileNameWithoutExtension.TrimEnd('_') + ".dds";
                    var newName = Path.Combine(directoryInfo.FullName, trimmedWithExtension);

                    if (File.Exists(newName))
                        File.Delete(newName);
                    File.Move(oldName, newName);
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("Did not succeed in writing DXT1-compressed files. IO error.");
            }
        }
    }
}
