using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.App;
using System.Diagnostics;
using MinerWars.AppCode.Game.Managers;

namespace MinerWars.AppCode.Game.Voxels
{
    class MyVoxelMaterialTextures
    {
        public MyTexture2D TextureDiffuseForAxisXZ;
        public MyTexture2D TextureDiffuseForAxisY;
        public MyTexture2D TextureNormalMapForAxisXZ;
        public MyTexture2D TextureNormalMapForAxisY;
        public MyTexture2D TextureHeightMapForAxisXZ;
    }

    class MyVoxelMaterial
    {
        public bool UseTwoTextures { get; private set; }
        public float SpecularIntensity { get; private set; }
        public float SpecularPower { get; private set; }
        public bool IsIndestructible { get; private set; }
        public bool UseFlag;

        MyMwcVoxelMaterialsEnum m_materialEnum;
        MyVoxelMaterialTextures m_textures;
        string m_assetName;
        bool m_hasBuilderVersion;
        
        //  Parameter 'useTwoTexturesPerMaterial' tells us if we use two textures per material. One texture for axis XZ and second for axis Y.
        //  Use it for rock/stone materials. Don't use it for gold/silver, because there you don't need to make difference between side and bottom materials.
        //  Using this we save texture memory, but pixel shader still used differenced textures (two samplers looking to same texture)
        public MyVoxelMaterial(MyMwcVoxelMaterialsEnum materialEnum, string assetName, bool isIndestructible, bool useTwoTextures, float specularIntensity, float specularPower, bool hasBuilderVersion)
        {
            //  SpecularPower must be > 0, because pow() makes NaN results if called with zero
            MyCommonDebugUtils.AssertRelease(specularPower > 0);

            m_assetName = assetName;
            IsIndestructible = isIndestructible;
            SpecularIntensity = specularIntensity;
            SpecularPower = specularPower;
            UseTwoTextures = useTwoTextures;
            m_materialEnum = materialEnum;
            m_hasBuilderVersion = hasBuilderVersion && MyFakes.MWBUILDER;
        }

        //
        //force to reload all textures by dropping the old ones
        public void LoadContent()
        {
            //m_textures = null;
        }

        public void UnloadContent()
        {
            m_textures = null;
        }

        //  Get access to material textures with lazy loading mechanizm
        public MyVoxelMaterialTextures GetTextures()
        {
            if (m_textures == null)
            {
                m_textures = new MyVoxelMaterialTextures();

                //  Diffuse XZ
                m_textures.TextureDiffuseForAxisXZ = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Voxels\\" + m_assetName + "_ForAxisXZ_de" + (m_hasBuilderVersion ? "_mw" : ""), null, LoadingMode.Lazy);

                if (Render.MyRenderConstants.RenderQualityProfile.UseNormals)
                {
                    //  Normal map XZ
                    m_textures.TextureNormalMapForAxisXZ = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Voxels\\" + m_assetName + "_ForAxisXZ_ns" + (m_hasBuilderVersion ? "_mw" : ""), null, LoadingMode.Lazy);
                }

                if (Render.MyRenderConstants.RenderQualityProfile.UseHeightForVerticals)
                { 
                    //Height map XZ - its possible use height without normals?
                    m_textures.TextureHeightMapForAxisXZ = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Voxels\\" + m_assetName + "_ForAxisXZ_hm" + (m_hasBuilderVersion ? "_mw" : ""), null, LoadingMode.Lazy);
                }
                //  Diffuse Y
                if (UseTwoTextures)
                {
                    m_textures.TextureDiffuseForAxisY = MyTextureManager.GetTexture<MyTexture2D>("Textures\\Voxels\\" + m_assetName + "_ForAxisY_de" + (m_hasBuilderVersion ? "_mw" : ""), null, LoadingMode.Lazy);
                }
                else
                {
                    m_textures.TextureDiffuseForAxisY = m_textures.TextureDiffuseForAxisXZ;
                }

                if (Render.MyRenderConstants.RenderQualityProfile.UseNormals)
                {
                    //  Normal map Y
                    m_textures.TextureNormalMapForAxisY = UseTwoTextures ? MyTextureManager.GetTexture<MyTexture2D>("Textures\\Voxels\\" + m_assetName + "_ForAxisY_ns" + (m_hasBuilderVersion ? "_mw" : ""), null, LoadingMode.LazyBackground) : m_textures.TextureNormalMapForAxisXZ;
                }
                    /*
                CheckTexture(m_textures.TextureDiffuseForAxisXZ);
                CheckTexture(m_textures.TextureDiffuseForAxisY);

                if (Render.MyRenderConstants.RenderQualityProfile.UseNormals)
                {
                    CheckTexture(m_textures.TextureNormalMapForAxisXZ);
                    CheckTexture(m_textures.TextureNormalMapForAxisY);
                }     */
            }
            UseFlag = true;
            return m_textures;
        }

        /// <summary>
        /// Checks the normal map.
        /// </summary>
        /// <param name="texture">The texture.</param>
        private static void CheckTexture(MyTexture texture)
        {
            System.Diagnostics.Debug.Assert(texture != null, "Voxel texture missing");
            MyUtils.AssertTexture((MyTexture2D)texture);

            texture.TextureLoaded -= CheckTexture;
        }
    }
}
