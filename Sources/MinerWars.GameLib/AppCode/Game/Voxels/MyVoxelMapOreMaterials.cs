using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Voxels
{
    class MyOreRatioFromVoxelMaterial
    {
        public MyMwcObjectBuilder_Ore_TypesEnum OreType;
        public float Ratio;

        public MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum oreType, float ratio)
        {
            OreType = oreType;
            Ratio = ratio;
        }
    }

    class MyVoxelMaterialConfiguration
    {
        public bool CanBeHarvested { get; set; }
        public bool IsRareOre { get; set; }
        public MyOreRatioFromVoxelMaterial[] OreFromVoxelMaterial { get; set; }

        public MyVoxelMaterialConfiguration(bool canBeHarvested, bool isRareOre, MyOreRatioFromVoxelMaterial[] oreFromVoxelMaterial)
        {
            CanBeHarvested = canBeHarvested;
            IsRareOre = isRareOre;
            OreFromVoxelMaterial = oreFromVoxelMaterial;
        }
    }

    static class MyVoxelMapOreMaterials
    {                
        private static readonly MyVoxelMaterialConfiguration[] m_materialConfigurations = new MyVoxelMaterialConfiguration[MyMwcUtils.GetMaxValueFromEnum<MyMwcVoxelMaterialsEnum>() + 1];
        private static readonly int m_rareOreCount;

        static MyVoxelMapOreMaterials()
        {
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Cobalt_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.COBALT, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Concrete_01] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.CONCRETE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Concrete_02] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.CONCRETE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Gold_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.GOLD, 0.7f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Helium3_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.HELIUM, 0.7f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Helium4_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.HELIUM, 0.7f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Ice_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.ICE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Indestructible_01] = new MyVoxelMaterialConfiguration(false, false, null);
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Indestructible_02] = new MyVoxelMaterialConfiguration(false, false, null);
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Indestructible_03] = new MyVoxelMaterialConfiguration(false, false, null);
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Indestructible_04] = new MyVoxelMaterialConfiguration(false, false, null);
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Indestructible_05_Craters_01] = new MyVoxelMaterialConfiguration(false, false, null);
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Iron_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.IRON, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Iron_02] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.IRON, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Lava_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.LAVA, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Magnesium_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.MAGNESIUM, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Nickel_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.NICKEL, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Organic_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.ORGANIC, 0.09f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Platinum_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.PLATINUM, 0.8f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Sandstone_01] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.SANDSTONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_Red] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Silicon_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.SILICON, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Silver_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.SILVER, 0.9f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Snow_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.SNOW, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_01] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_02] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_03] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_04] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_05] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_06] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_07] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_08] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_10] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Stone_13_Wall_01] = new MyVoxelMaterialConfiguration(true, false, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.STONE, 1f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Treasure_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.TREASURE, 0.05f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Treasure_02] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.TREASURE, 0.05f) });
            m_materialConfigurations[(int)MyMwcVoxelMaterialsEnum.Uranite_01] = new MyVoxelMaterialConfiguration(true, true, new MyOreRatioFromVoxelMaterial[] { new MyOreRatioFromVoxelMaterial(MyMwcObjectBuilder_Ore_TypesEnum.URANITE, 0.3f) });

            m_rareOreCount = GetRareOreCount();
        }

        public static bool IsRareOre(MyMwcVoxelMaterialsEnum voxelMaterial)
        {
            return m_materialConfigurations[(int)voxelMaterial].IsRareOre;
        }

        public static bool CanBeHarvested(MyMwcVoxelMaterialsEnum voxelMaterial)
        {
            return m_materialConfigurations[(int) voxelMaterial].CanBeHarvested;
        }        

        private static int GetRareOreCount()
        {
            int oreCount = 0;
            foreach (MyVoxelMaterialConfiguration materialConfiguration in m_materialConfigurations)
            {
                if (materialConfiguration.IsRareOre)
                {
                    oreCount++;
                }
            }
            return oreCount;
        }

        public static int RareOreCount()
        {
            return m_rareOreCount;
        }

        public static MyOreRatioFromVoxelMaterial[] GetOreFromVoxelMaterial(MyMwcVoxelMaterialsEnum voxelMaterial)
        {
            return m_materialConfigurations[(int) voxelMaterial].OreFromVoxelMaterial;
        }
    }
}
