using System;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.World;

namespace MinerWars.AppCode.Game.Voxels
{
    static class MyVoxelMaterials
    {
        static MyVoxelMaterial[] m_materials;
        static MyMeshMaterial[] m_meshMaterials;

        static int voxelMaterialCount = -1;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMaterials.LoadData");

            m_materials = new MyVoxelMaterial[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1];
            m_meshMaterials = new MyMeshMaterial[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1];

            Add(MyMwcVoxelMaterialsEnum.Indestructible_01, "Indestructible_01", true, true, 0.6f, 100, false);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_02, "Indestructible_02", true, true, 0.6f, 100, false);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_03, "Indestructible_03", true, true, 0.6f, 100, false);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_04, "Indestructible_04", true, true, 0.6f, 100, false);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_05_Craters_01, "Indestructible_05_Craters_01", true, true, 0.1f, 100, false);
            Add(MyMwcVoxelMaterialsEnum.Ice_01, "Ice_01", false, true, 1, 20.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Treasure_01, "Treasure_01", false, false, 0.9f, 10.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Treasure_02, "Treasure_02", false, false, 0.7f, 20.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Iron_01, "Iron_01", false, false, 0.7f, 2.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Iron_02, "Iron_02", false, false, 0.6f, 2, false);
            Add(MyMwcVoxelMaterialsEnum.Stone_01, "Stone_01", false, true, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_02, "Stone_02", false, true, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_03, "Stone_03", false, true, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_04, "Stone_04", false, true, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_05, "Stone_05", false, false, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_06, "Stone_06", false, false, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_07, "Stone_07", false, false, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_08, "Stone_08", false, false, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_10, "Stone_10", false, true, 0.6f, 100.0f, true);
            Add(MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, "Stone_13_Wall_01", false, true, 0.6f, 50, true);
            Add(MyMwcVoxelMaterialsEnum.Uranite_01, "Uraninite_01", false, false, 1.2f, 50.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Helium3_01, "Helium3_01", false, false, 0.4f, 20.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Helium4_01, "Helium4_01", false, true, 0.9f, 60.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Organic_01, "Organic_01", false, false, 0, 1, false);
            Add(MyMwcVoxelMaterialsEnum.Gold_01, "Gold_01", false, false, 1.5f, 2.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Silver_01, "Silver_01", false, false, 0.8f, 1.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Nickel_01, "Nickel_01", false, false, 0.6f, 2, false);
            Add(MyMwcVoxelMaterialsEnum.Magnesium_01, "Magnesium_01", false, true, 0.6f, 2, false);
            Add(MyMwcVoxelMaterialsEnum.Platinum_01, "Platinum_01", false, false, 0.8f, 2.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Silicon_01, "Silicon_01", false, true, 2.0f, 50.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Cobalt_01, "Cobalt_01", false, false, 0.6f, 10.0f, false);
            Add(MyMwcVoxelMaterialsEnum.Snow_01, "Snow_01", false, false, 0.1f, 0.1f, false);
            Add(MyMwcVoxelMaterialsEnum.Lava_01, "lava_01", false, true, 0, 1, false);
            Add(MyMwcVoxelMaterialsEnum.Concrete_01, "Concrete_01", false, true, 0.6f, 2, false);
            Add(MyMwcVoxelMaterialsEnum.Concrete_02, "Concrete_02", false, true, 0.6f, 2, false);
            Add(MyMwcVoxelMaterialsEnum.Sandstone_01, "Sandstone_01", false, true, 0.6f, 2, false);
            Add(MyMwcVoxelMaterialsEnum.Stone_Red, "Stone_Red", false, true, 0.6f, 2, false);

            foreach (MyMwcVoxelMaterialsEnum material in Enum.GetValues(typeof(MyMwcVoxelMaterialsEnum)))
            {
                MyCommonDebugUtils.AssertRelease(Get(material) != null);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            if (m_materials != null)
            {
                foreach (var m in m_materials)
                {
                    m.UnloadContent();

                    //if (m.GetTextures().TextureDiffuseForAxisXZ != null && m.GetTextures().TextureDiffuseForAxisXZ.IsValid)
                    //{
                    //    m.GetTextures().TextureDiffuseForAxisXZ.Unload();
                    //}
                    //if (m.GetTextures().TextureDiffuseForAxisY != null && m.GetTextures().TextureDiffuseForAxisY.IsValid)
                    //{
                    //    m.GetTextures().TextureDiffuseForAxisY.Unload();
                    //}
                    //if (m.GetTextures().TextureNormalMapForAxisXZ != null && m.GetTextures().TextureNormalMapForAxisXZ.IsValid)
                    //{
                    //    m.GetTextures().TextureNormalMapForAxisXZ.Unload();
                    //}
                    //if (m.GetTextures().TextureNormalMapForAxisY != null && m.GetTextures().TextureNormalMapForAxisY.IsValid)
                    //{
                    //    m.GetTextures().TextureNormalMapForAxisY.Unload();
                    //}
                }
                m_materials = null;
                m_meshMaterials = null;
            }
        }

        public static void MarkAllAsUnused()
        {
            if (m_materials != null)
            {
                foreach (MyVoxelMaterial mat in m_materials)
                {
                    mat.UseFlag = false;
                }
            }
        }

        public static void UnloadUnused()
        {
            if (m_materials != null)
            {
                foreach (MyVoxelMaterial mat in m_materials)
                {
                    if (mat.UseFlag == false)
                    {
                        mat.UnloadContent();
                    }
                }
            }
        }

        //  Here we load only textures, effects, etc, no voxel-maps.
        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyVoxelMaterials.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMaterials::LoadContent");

            if (m_materials != null)
            {
                foreach (MyVoxelMaterial mat in m_materials)
                {
                    mat.LoadContent();
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelMaterials.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyVoxelMaterials.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            if (m_materials != null)
            {
                foreach (MyVoxelMaterial mat in m_materials)
                {
                    mat.UnloadContent();
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelMaterials.UnloadContent - END");
        }

        public static void ReloadContent()
        {
            UnloadContent();
            LoadContent();
        }

        static void Add(MyMwcVoxelMaterialsEnum materialEnum, string assetName, bool isIndestructible, bool useTwoTextures, float specularShininess, float specularPower, bool hasBuilderVersion)
        {
            //  Check if not yet assigned
            MyCommonDebugUtils.AssertRelease(m_materials[(int)materialEnum] == null);

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("new MyVoxelMaterial");

            //  Create and add into array
            MyVoxelMaterial voxelMaterial = new MyVoxelMaterial(materialEnum, assetName, isIndestructible, useTwoTextures, specularShininess, specularPower, hasBuilderVersion);
            m_materials[(int)materialEnum] = voxelMaterial;
            m_meshMaterials[(int)materialEnum] = new MyMeshMaterial("Textures\\Voxels\\" + assetName + "_ForAxisXZ", assetName, voxelMaterial.GetTextures().TextureDiffuseForAxisXZ, voxelMaterial.GetTextures().TextureNormalMapForAxisXZ);
        }


        public static MyMwcVoxelMaterialsEnum GetAllowedVoxelMaterial(MyMwcVoxelMaterialsEnum materialEnum)
        {
            if (MySector.AllowedMaterials == null)
                return materialEnum;

            bool materialAllowed = MySector.AllowedMaterials.Contains((int)materialEnum);
            materialAllowed |= MySector.AllowedMaterials.Count == 0;

            if (!materialAllowed)
            {
                //System.Diagnostics.Debug.Assert(false, "Usage of non compatible sector voxel material! (" + materialEnum.ToString() + ")"); // Assert temporarily turned off
                //To avoid asserts
                //MySector.PrimaryMaterials.Add(materialEnum);

                MyMwcVoxelMaterialsEnum newMaterialEnum = materialEnum;

                if (MySector.AllowedMaterials.Count > 0)
                    newMaterialEnum = (MyMwcVoxelMaterialsEnum)MySector.AllowedMaterials[0];
                
                //MyMwcLog.WriteLine("Voxel material " + materialEnum.ToString() + " is not compatible with this sector and is replaced by " + newMaterialEnum.ToString());

                materialEnum = newMaterialEnum;
            }

            return materialEnum;
        }

        public static MyVoxelMaterial Get(MyMwcVoxelMaterialsEnum materialEnum)
        {
            materialEnum = GetAllowedVoxelMaterial(materialEnum);

            return m_materials[(int)materialEnum];
        }

        public static MyMeshMaterial GetMaterialForMesh(MyMwcVoxelMaterialsEnum materialEnum)
        {
            return m_meshMaterials[(int)materialEnum];
        }

        public static int GetMaterialsCount()
        {
            if (voxelMaterialCount == -1)
            {
                voxelMaterialCount = MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1;
            }
            return voxelMaterialCount;
        }

        public static bool IsIndestructible(MyMwcVoxelMaterialsEnum material)
        {
            return m_materials[(int)material].IsIndestructible;
        }
    }
}
