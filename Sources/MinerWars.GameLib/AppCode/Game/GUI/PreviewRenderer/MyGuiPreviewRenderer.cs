using System;
//using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.App;
//using MinerWarsMath;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Lights;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

using MinerWars.AppCode.Game.GUI.Core;
using System.IO;
using MinerWars.AppCode.Game.GUI.Helpers;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;

namespace MinerWars.AppCode.Game.GUI
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Color = MinerWarsMath.Color;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using MathHelper = MinerWarsMath.MathHelper;
    using MinerWars.CommonLIB.AppCode.Import;

    /// <summary>
    /// Used for rendering models for GUI previews. 
    /// Uses three lights to light prefabs:
    /// 1. Key light - directional (sun)
    /// 2. Fill light - point
    /// 3. Back light - point
    /// </summary>
    class MyGuiPreviewRenderer
    {
        private static MyEntity m_fakeEntity;
        private static MyEntity FakeEntity
        {
            get
            {
                if (m_fakeEntity == null)
                {
                    m_fakeEntity = new MyStaticAsteroid();
                }
                return m_fakeEntity; 
            }
        }

        private MyRender.MyRenderSetup m_setup;
        Texture m_fullSizeRT;

        public MyGuiPreviewRenderer()
        {
           // SetRenderSetup();
        }

        private void SetRenderSetup()
        {
            return;
            m_setup = new MyRender.MyRenderSetup();

            //m_setup.Fov = MathHelper.ToRadians(75);

            m_setup.CallerID = MyRenderCallerEnum.GUIPreview;
                                    
            m_setup.RenderTargets = new Texture[1];


            m_setup.EnabledModules = new HashSet<MyRenderModuleEnum>();
            m_setup.EnabledModules.Add(MyRenderModuleEnum.VoxelHand);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Decals);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.CockpitWeapons);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SectorBorder);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DrawSectorBBox);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DrawCoordSystem);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Explosions);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.BackgroundCube);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.GPS);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TestField);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.AnimatedParticles);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Lights);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.TransparentGeometryForward);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Projectiles);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.DebrisField);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.ThirdPerson);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.Editor);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarObjects);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SolarMapGrid);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.PrunningStructure);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.SunWind);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.IceStormWind);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.PrefabContainerManager);
            m_setup.EnabledModules.Add(MyRenderModuleEnum.PhysicsPrunningStructure);

            m_setup.EnabledRenderStages = new HashSet<MyRenderStage>();
            m_setup.EnabledRenderStages.Add(MyRenderStage.PrepareForDraw);
            m_setup.EnabledRenderStages.Add(MyRenderStage.Background);
            m_setup.EnabledRenderStages.Add(MyRenderStage.LODDrawStart);
            m_setup.EnabledRenderStages.Add(MyRenderStage.LODDrawEnd);
            m_setup.EnabledRenderStages.Add(MyRenderStage.AlphaBlendPreHDR);
            m_setup.EnabledRenderStages.Add(MyRenderStage.AlphaBlend);

 

            m_setup.EnabledPostprocesses = new HashSet<MyPostProcessEnum>();
            m_setup.EnabledPostprocesses.Add(MyPostProcessEnum.VolumetricSSAO2);
            m_setup.EnabledPostprocesses.Add(MyPostProcessEnum.HDR);
            

            //m_setup.BackgroundColor = new Color(0.17f, 0.18f, .25f, 0.0f);
            m_setup.BackgroundColor = new Color(Vector4.Zero);

            m_setup.EnableHDR = false;
            m_setup.EnableSun = true;
            m_setup.ShadowRenderer = MyRender.GetShadowRenderer(); // Default shadow render
            m_setup.EnableSmallLights = true;
            m_setup.EnableDebugHelpers = false;
            m_setup.EnableEnvironmentMapping = false;
            m_setup.EnableOcclusionQueries = false;

            m_setup.LodTransitionNear = 20000;
            m_setup.LodTransitionFar = 21000;
            m_setup.LodTransitionBackgroundStart = 50000;
            m_setup.LodTransitionBackgroundStart = 51000;

            m_setup.RenderElementsToDraw = new List<MyRender.MyRenderElement>();
            m_setup.TransparentRenderElementsToDraw = new List<MyRender.MyRenderElement>();
            m_setup.LightsToUse = new List<MyLight>();
            var fillLight = new MyLight();
            fillLight.Start(MyLight.LightTypeEnum.PointLight, .5f * new Vector4(1, 0.95f, 0.8f, 1), 1, 119); // fill light
            m_setup.LightsToUse.Add(fillLight);
            var backLight = new MyLight();
            backLight.Start(MyLight.LightTypeEnum.PointLight, .5f * new Vector4(1, 0.9f, 0.9f, 1), 1, 119); // back light
            m_setup.LightsToUse.Add(backLight);  
        }

        /// <summary>
        /// IMPORTANT: using Lod1Normals here, since this is done in a separate time to normal rendering
        /// </summary>
        public Texture RenderPrefabPreview(int prefabId, MyPrefabConfiguration config, MyMwcObjectBuilder_Prefab_AppearanceEnum appearance, int width, int height, float lightsIntensity = 2.5f)
        {             
            m_fullSizeRT = MyRender.GetRenderTarget(MyRenderTargets.SSAO);

            if (m_fullSizeRT == null || MyGuiScreenGamePlay.Static == null)
            {
                return null;
            }

            MyFakes.RENDER_PREVIEWS_WITH_CORRECT_ALPHA = true;

            Texture renderTarget = new Texture(MyMinerGame.Static.GraphicsDevice, width, height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            PrepareRender(width, height);
            MyRender.Sun.Intensity = lightsIntensity;
            foreach (var light in m_setup.LightsToUse)
            {
                light.Intensity = lightsIntensity;
            }

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = false;
            }

            bool weapon = false;

            if (config.BuildType == BuildTypesEnum.MODULES && config.CategoryType == CategoryTypesEnum.WEAPONRY)
            {
                MyModel baseModel = null;
                MyModel barrelModel = null;
                Matrix baseMatrix = Matrix.Identity;
                Matrix barrelMatrix = Matrix.Identity;

                weapon = MyLargeShipGunBase.GetVisualPreviewData((MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)prefabId, ref baseModel, ref barrelModel, ref baseMatrix, ref barrelMatrix);

                if (weapon)
                {
                    m_setup.ViewMatrix = Matrix.Identity;

                    SetupRenderElements(baseModel, baseMatrix, (int) appearance);
                    SetupRenderElements(barrelModel, barrelMatrix, (int)appearance);

                    SetupLights(baseModel);

                    MyRender.PushRenderSetup(m_setup);
                    MyRender.Draw();
                    MyRender.PopRenderSetup();

                    BlitToThumbnail(MyMinerGame.Static.GraphicsDevice, renderTarget);
                }
            }
            
            if (!weapon)
            {
                //load new model from prefab config
                MyModel model = MyModels.GetModelForDraw(config.ModelLod0Enum);

                float distanceMultiplier = 2f;

                Matrix viewMatrix = Matrix.Identity;
                m_setup.ViewMatrix = viewMatrix;

                //generate world matrix
                Matrix worldMatrix = Matrix.Identity;
                worldMatrix.Translation = -model.BoundingSphere.Center;
                
                worldMatrix *= config.PreviewPointOfView.Transform;

                worldMatrix *= Matrix.CreateRotationY(-.85f * MathHelper.PiOver4);
                worldMatrix *= Matrix.CreateRotationX(.35f * MathHelper.PiOver4);
                worldMatrix.Translation += new Vector3(0, 0, -model.BoundingSphere.Radius * distanceMultiplier);

                SetupRenderElements(model, worldMatrix, (int)appearance);

                SetupLights(model);

                //MyGuiManager.TakeScreenshot();
                MyRender.PushRenderSetup(m_setup);
                MyRender.EnableShadows = false;
                MyRender.Draw();
                MyRender.EnableShadows = true;
                MyRender.PopRenderSetup();
                //MyGuiManager.StopTakingScreenshot();

                BlitToThumbnail(MyMinerGame.Static.GraphicsDevice, renderTarget);
            }

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = true;
            }
            
            MyFakes.RENDER_PREVIEWS_WITH_CORRECT_ALPHA = false;
                               
            return renderTarget;
        }

        /// <summary>
        /// IMPORTANT: using Lod1Normals here, since this is done in a separate time to normal rendering
        /// </summary>
        public Texture RenderModelPreview(MyModel model, int width, int height, float lightsIntensity = 2f)
        {             
            m_fullSizeRT = MyRender.GetRenderTarget(MyRenderTargets.Auxiliary0);

            if (m_fullSizeRT == null || MyGuiScreenGamePlay.Static == null)
                return null;

            Device device = MyMinerGame.Static.GraphicsDevice;

            Texture renderTarget = new Texture(device, width, height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            PrepareRender(width, height);
            MyRender.Sun.Intensity = lightsIntensity;
            foreach (var light in m_setup.LightsToUse)
            {
                light.Intensity = lightsIntensity;
            }

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = false;
            }

            float distance = 2.1f;

            Matrix viewMatrix = Matrix.Identity;
            m_setup.ViewMatrix = viewMatrix;

            m_setup.EnableNear = false;

            //generate world matrix
            Matrix worldMatrix = Matrix.Identity;
            
            worldMatrix.Translation = -model.BoundingSphere.Center;
            worldMatrix *= Matrix.CreateRotationY(-3.0f * MathHelper.PiOver4);
            worldMatrix *= Matrix.CreateRotationX(0.7f * MathHelper.PiOver4);
            worldMatrix.Translation += new Vector3(0, 0, -model.BoundingSphere.Radius * distance);
           
            SetupRenderElements(model, worldMatrix);

            SetupLights(model);

            MyRender.PushRenderSetup(m_setup);
            MyRender.Draw();
            MyRender.PopRenderSetup();

            BlitToThumbnail(device, renderTarget);

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = true;
            }

            return renderTarget;   

            return null;
        }

        public Texture RenderAsteroidMaterialPreview(MyMwcVoxelMaterialsEnum material, int width, int height, float lightsIntensity = 2f)
        {               
            m_fullSizeRT = MyRender.GetRenderTarget(MyRenderTargets.Auxiliary0);

            if (m_fullSizeRT == null || MyGuiScreenGamePlay.Static == null)
                return null;

            Device device = MyMinerGame.Static.GraphicsDevice;

            Texture renderTarget = new Texture(device, width, height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            PrepareRender(width, height);

            MyRender.Sun.Direction = new Vector3(1.5f, -1.5f, -1);
            MyRender.Sun.Direction.Normalize();

            MyRender.Sun.Intensity = lightsIntensity;
            foreach (var light in m_setup.LightsToUse)
            {
                light.Intensity = 3 * lightsIntensity;
            }

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = false;
            }

            float distance = 2.1f;

            Matrix viewMatrix = Matrix.Identity;
            m_setup.ViewMatrix = viewMatrix;

            m_setup.EnableNear = false;


            //var modelEnum = MyStaticAsteroid.GetModelsFromType(MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A).LOD0;
            var modelEnum = MyModelsEnum.sphere_smooth;
            var model = MyModels.GetModelOnlyData(modelEnum);
            var test = model.BoundingSphere.Radius;
            model.SetDrawTechnique(MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID);

            
            FakeEntity.VoxelMaterial = material;
            FakeEntity.InitDrawTechniques();


            //generate world matrix
            Matrix worldMatrix = Matrix.Identity;

            worldMatrix.Translation = -model.BoundingSphere.Center;
            //worldMatrix *= Matrix.CreateRotationY(-3.0f * MathHelper.PiOver4);
            //worldMatrix *= Matrix.CreateRotationX(0.7f * MathHelper.PiOver4);
            worldMatrix.Translation += new Vector3(0, 0, -model.BoundingSphere.Radius * distance);

            SetupRenderElements(model, worldMatrix, staticAsteroid: true);

            SetupLights(model);

            MyRender.PushRenderSetup(m_setup);
            MyRender.Draw();
            MyRender.PopRenderSetup();

            BlitToThumbnail(device, renderTarget);

            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Visible = true;
            }

            return renderTarget; 

            return null;
        }

        private void PrepareRender(int width, int height)
        {                    
            m_setup.RenderTargets[0] = m_fullSizeRT;

            m_setup.RenderElementsToDraw.Clear();
            m_setup.TransparentRenderElementsToDraw.Clear();

            // make actual viewport one pixel larger in order to remove the deformed border caused by antialiasing
            m_setup.Viewport = new Viewport(0, 0, 2 * width, 2 * height);
            m_setup.AspectRatio = 1;
            m_setup.EnableOcclusionQueries = false;
            m_setup.EnableNear = false;

            MyRender.Sun.Direction = new Vector3(.5f, -.3f, -1);
            MyRender.Sun.Direction.Normalize();
            MyRender.Sun.Color = Vector4.One;
            MyRender.EnableSun = true;   
        }

        private void SetupLights(MyModel baseModel)
        {
            m_setup.LightsToUse[0].SetPosition(baseModel.BoundingSphere.Center + new Vector3(baseModel.BoundingSphere.Radius, -0.4f * baseModel.BoundingSphere.Radius, baseModel.BoundingSphere.Radius));
            m_setup.LightsToUse[1].SetPosition(baseModel.BoundingSphere.Center + new Vector3(0.9f * baseModel.BoundingSphere.Radius, -0.5f * baseModel.BoundingSphere.Radius, -1.0f * baseModel.BoundingSphere.Radius));
        }

        private void BlitToThumbnail(Device device, Texture renderTarget)
        {       
            MyMinerGame.SetRenderTarget(renderTarget, null);
            var screenEffect = MyRender.GetEffect(MyEffects.Scale) as MyEffectScale;
            Debug.Assert(screenEffect != null);
            screenEffect.SetTechnique(MyEffectScale.Technique.HWScalePrefabPreviews);
            screenEffect.SetSourceTextureMod(m_fullSizeRT);
            //screenEffect.SetScale(2f * new Vector2(renderTarget.Width / (float)m_fullSizeRT.Width, renderTarget.Height / (float)m_fullSizeRT.Height));
            screenEffect.SetScale(2f * new Vector2((renderTarget.GetLevelDescription(0).Width - 1) / (float)m_fullSizeRT.GetLevelDescription(0).Width, (renderTarget.GetLevelDescription(0).Height - 1) / (float)m_fullSizeRT.GetLevelDescription(0).Height));
            MyGuiManager.GetFullscreenQuad().Draw(screenEffect);
            MyMinerGame.SetRenderTarget(null, null);  
        }

        private void SetupRenderElements(MyModel model, Matrix worldMatrix, int materialIndex = 0, bool staticAsteroid = false)
        {              
            model.LoadInDraw();

            var meshList = model.GetMeshList();
            foreach (MyMesh mesh in meshList)
            {
                if (materialIndex > mesh.Materials.Length - 1)
                    materialIndex = 0;

                var material = staticAsteroid ? MyVoxelMaterials.GetMaterialForMesh(FakeEntity.VoxelMaterial) : mesh.Materials[materialIndex];

                material.PreloadTexture(Managers.LoadingMode.Immediate);

                if (material.DrawTechnique == MyMeshDrawTechnique.DECAL)
                    continue;

                if (staticAsteroid && material.DrawTechnique == MyMeshDrawTechnique.MESH)
                {
                    material.DrawTechnique = MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID;
                }

                //Do not use render alocator, it will reuse elements
                MyRender.MyRenderElement renderElement = new MyRender.MyRenderElement();
                //MyRender.AllocateRenderElement(out renderElement);

                renderElement.Entity = FakeEntity;

                renderElement.VertexBuffer = model.VertexBuffer;
                renderElement.IndexBuffer = model.IndexBuffer;
                renderElement.IndexStart = mesh.IndexStart;
                renderElement.TriCount = mesh.TriCount;

                //renderElement.UseChannels = false;

                renderElement.WorldMatrixForDraw = worldMatrix;
                renderElement.WorldMatrix = worldMatrix;

                renderElement.Material = material;
                renderElement.DrawTechnique = material.DrawTechnique;
                renderElement.VoxelBatch = null;

                if (material.DrawTechnique == MyMeshDrawTechnique.HOLO)
                    m_setup.TransparentRenderElementsToDraw.Add(renderElement);
                else
                    m_setup.RenderElementsToDraw.Add(renderElement);
            }      
        }

        /// <summary>
        /// Create all prefab previews as dds files
        /// Filename is derived from MyMwcObjectBuilder_Prefab_TypesEnum
        /// </summary>
        public void CreatePreviewsToFiles(string path, int sizeInPixels)
        {
            MyFakes.RENDER_PREVIEWS_WITH_CORRECT_ALPHA = true;

            if (MyHudSectorBorder.Enabled)
                MyHudSectorBorder.SwitchSectorBorderVisibility();

            var directoryInfo = Directory.CreateDirectory(path);

            CreateAndCompressPrefabPreviews(sizeInPixels, directoryInfo);

            //CreateAndCompressGUIHelpers(sizeInPixels, directoryInfo);

            MyFakes.RENDER_PREVIEWS_WITH_CORRECT_ALPHA = false;
        }

        private void CreateAndCompressPrefabPreviews(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            GeneratePrefabPreviews(sizeInPixels, directoryInfo);

            // compress using nvDXT.exe to DXT3:
            var prefabsDirectory = directoryInfo.GetDirectories("Prefabs")[0];
            foreach (var dir in prefabsDirectory.EnumerateDirectories())
            {
                CompressDDSFiles(dir);
            }
        }

        private void CreateAndCompressGUIHelpers(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            GenerateStaticAsteroidPreviews(sizeInPixels, directoryInfo);

            GenerateAsteroidMaterialPreviews(sizeInPixels, directoryInfo);

            var guiHelpersDirectory = directoryInfo.GetDirectories("GuiHelpers")[0];
            CompressDDSFiles(guiHelpersDirectory);
        }

        public static string GetPreviewFileName(MyPrefabConfiguration config, int prefabId)
        {
            if (config.BuildType == BuildTypesEnum.MODULES && config.CategoryType == CategoryTypesEnum.WEAPONRY)
            {
                MyModelsEnum baseModelEnum, barelModelEnum;
                if (MyLargeShipGunBase.GetModelEnums((MyMwcObjectBuilder_PrefabLargeWeapon_TypesEnum)prefabId, out baseModelEnum, out barelModelEnum))
                {
                    return Path.GetFileName(MyModels.GetModelAssetName(baseModelEnum));
                }
            }            

            return Path.GetFileName(MyModels.GetModelAssetName(config.ModelLod0Enum));
        }

        private void GenerateSmallShipPreviews(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            var directoryGuiHelpers = Directory.CreateDirectory(Path.Combine(directoryInfo.FullName, "GuiHelpers"));
            int index = 1;
            foreach (MyMwcObjectBuilder_SmallShip_TypesEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues)
            {
                Debug.WriteLine(string.Format("Exporting SmallShip Preview {0}/{1} ..", index++,
                                              MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues.Length));

                var modelEnum = MyShipTypeConstants.GetShipTypeProperties(enumValue).Visual.ModelLod0Enum;
                var model = MyModels.GetModelForDraw(modelEnum);

                string assetName = Path.GetFileName(MyModels.GetModelAssetName(modelEnum));

                var result = RenderModelPreview(model, sizeInPixels, sizeInPixels);
                string fileName = Path.Combine(directoryGuiHelpers.FullName, string.Format("{0}.dds", assetName));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                //TODO
                //MyDDSFile.DDSToFile(fileName, true, result, false);
            }
        }

        void GenerateAsteroidMaterialPreviews(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            var directoryGuiHelpers = Directory.CreateDirectory(Path.Combine(directoryInfo.FullName, "GuiHelpers"));
            int index = 1;
            foreach (MyMwcVoxelMaterialsEnum enumValue in MyGuiAsteroidHelpers.MyMwcVoxelMaterialsEnumValues)
            {
                Debug.WriteLine(string.Format("Exporting voxel Material Preview {0}/{1} ..", index++,
                                              MyGuiAsteroidHelpers.MyMwcVoxelMaterialsEnumValues.Count));

                var result = RenderAsteroidMaterialPreview(enumValue, sizeInPixels, sizeInPixels, 1);

                var materialHelper = MyGuiAsteroidHelpers.GetMyGuiVoxelMaterialHelper(enumValue);
                string fileName = Path.Combine(directoryGuiHelpers.FullName, string.Format("{0}.dds", Path.GetFileName(materialHelper.Icon.Name)));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                //TODO
                //MyDDSFile.DDSToFile(fileName, true, result, false);
            }
        }

        private void GenerateStaticAsteroidPreviews(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            var directoryGuiHelpers = Directory.CreateDirectory(Path.Combine(directoryInfo.FullName, "GuiHelpers"));
            int index = 1;
            foreach (MyMwcObjectBuilder_StaticAsteroid_TypesEnum enumValue in MyGuiAsteroidHelpers.MyMwcStaticAsteroidTypesEnumValues)
            {
                Debug.WriteLine(string.Format("Exporting Static Asteroid Preview {0}/{1} ..", index++,
                                              MyGuiAsteroidHelpers.MyMwcStaticAsteroidTypesEnumValues.Count));

                var modelEnum = MyStaticAsteroid.GetModelsFromType(enumValue).LOD0;
                var model = MyModels.GetModelOnlyData(modelEnum);
                model.SetDrawTechnique(MyMeshDrawTechnique.MESH);

                string assetName = Path.GetFileName(MyModels.GetModelAssetName(modelEnum));
                assetName = assetName.Substring(assetName.Length - 5, 5) == "_LOD0"
                                ? assetName.Substring(0, assetName.Length - 5)
                                : assetName;

                var result = RenderModelPreview(model, sizeInPixels, sizeInPixels);
                string fileName = Path.Combine(directoryGuiHelpers.FullName, string.Format("{0}.dds", assetName));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                //TODO
                //MyDDSFile.DDSToFile(fileName, true, result, false);
            }
        }

        private void GeneratePrefabPreviews(int sizeInPixels, DirectoryInfo directoryInfo)
        {
            var directoryPrefabs = Directory.CreateDirectory(Path.Combine(directoryInfo.FullName, "Prefabs"));

            //foreach (MyMwcObjectBuilderTypeEnum enumValue in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            var enumValue = MyMwcObjectBuilderTypeEnum.Prefab;
            {
                int index = 0;
                //var prefabIds = MyMwcObjectBuilder_Base.GetObjectBuilderIDs(enumValue);
                var prefabIds = new[] { 534 };

                //if (enumValue == MyMwcObjectBuilderTypeEnum.PrefabFoundationFactory) continue;
                // scanners has own preview texture
                //if (enumValue == MyMwcObjectBuilderTypeEnum.PrefabScanner) continue;

                foreach (int prefabId in prefabIds)
                {
                    if (prefabId == (ushort)MyMwcObjectBuilder_Prefab_TypesEnum.SimpleObject)
                        continue;

                    MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(enumValue, prefabId);

                    if (config == null)
                        continue;

                    string assetName = GetPreviewFileName(config, prefabId);

                    foreach (var faction in MyGuiPrefabHelpers.MyMwcFactionTextureEnumValues)
                    //var faction = MyMwcObjectBuilder_Prefab_AppearanceEnum.None;
                    {
                        var appearance = (MyMwcObjectBuilder_Prefab_AppearanceEnum)faction;

                        if (config.FactionSpecific.HasValue && config.FactionSpecific.Value != appearance)
                            continue;

                        var prefabTypeName = MyGuiObjectBuilderHelpers.GetGuiHelper(enumValue, prefabId).Description;
                        Debug.WriteLine(string.Format("Exporting prefab preview for {0}: {1}/{2} ..", prefabTypeName,
                                                      1 + (ushort) faction + 8 * index, 8 * prefabIds.Length));

                        var result = RenderPrefabPreview(prefabId, config, appearance,
                                                         sizeInPixels, sizeInPixels, 1.5f);

                        string fileName = Path.Combine(directoryPrefabs.FullName,
                                                       "v" + String.Format("{0:00}", (ushort) faction + 1),
                                                       string.Format("{0}.dds", assetName));

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        //TODO
                        //MyDDSFile.DDSToFile(fileName, true, result, false);
                    }

                    index++;
                }
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
