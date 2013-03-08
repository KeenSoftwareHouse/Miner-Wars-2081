using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.Utils
{
    internal struct MyMwcVoxelMaterialProperties
    {
        public MyMwcObjectBuilder_Ore_TypesEnum? Ore;
        public float Percents;
    }

    static class MyMwcVoxelMaterialConstants
    {
        static Dictionary<int, MyMwcVoxelMaterialProperties> MaterialProperties = new Dictionary<int, MyMwcVoxelMaterialProperties>();

        static MyMwcVoxelMaterialConstants()
        {
            Add(MyMwcVoxelMaterialsEnum.Cobalt_01, MyMwcObjectBuilder_Ore_TypesEnum.COBALT, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Gold_01, MyMwcObjectBuilder_Ore_TypesEnum.GOLD, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Helium3_01, MyMwcObjectBuilder_Ore_TypesEnum.HELIUM, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Helium4_01, MyMwcObjectBuilder_Ore_TypesEnum.HELIUM, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Ice_01, MyMwcObjectBuilder_Ore_TypesEnum.ICE, 0.05f);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_01, MyMwcObjectBuilder_Ore_TypesEnum.INDESTRUCTIBLE, 0.30f);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_02, MyMwcObjectBuilder_Ore_TypesEnum.INDESTRUCTIBLE, 0.30f);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_03, MyMwcObjectBuilder_Ore_TypesEnum.INDESTRUCTIBLE, 0.30f);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_04, MyMwcObjectBuilder_Ore_TypesEnum.INDESTRUCTIBLE, 0.30f);
            Add(MyMwcVoxelMaterialsEnum.Indestructible_05_Craters_01, MyMwcObjectBuilder_Ore_TypesEnum.INDESTRUCTIBLE, 0.30f);
            Add(MyMwcVoxelMaterialsEnum.Iron_01, MyMwcObjectBuilder_Ore_TypesEnum.IRON, 0.05f);
            Add(MyMwcVoxelMaterialsEnum.Iron_02, MyMwcObjectBuilder_Ore_TypesEnum.IRON, 0.05f);
            Add(MyMwcVoxelMaterialsEnum.Lava_01, null, 0.00f);
            Add(MyMwcVoxelMaterialsEnum.Magnesium_01, MyMwcObjectBuilder_Ore_TypesEnum.MAGNESIUM, 0.10f);
            Add(MyMwcVoxelMaterialsEnum.Nickel_01, MyMwcObjectBuilder_Ore_TypesEnum.NICKEL, 0.05f);
            Add(MyMwcVoxelMaterialsEnum.Organic_01, MyMwcObjectBuilder_Ore_TypesEnum.ORGANIC, 0.10f);
            Add(MyMwcVoxelMaterialsEnum.Platinum_01, MyMwcObjectBuilder_Ore_TypesEnum.PLATINUM, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Silicon_01, MyMwcObjectBuilder_Ore_TypesEnum.SILICON, 0.20f);
            Add(MyMwcVoxelMaterialsEnum.Silver_01, MyMwcObjectBuilder_Ore_TypesEnum.SILVER, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Snow_01, MyMwcObjectBuilder_Ore_TypesEnum.ICE, 0.05f);
            Add(MyMwcVoxelMaterialsEnum.Stone_01, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_02, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_03, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_04, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_05, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_06, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_07, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_08, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_10, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Stone_13_Wall_01, MyMwcObjectBuilder_Ore_TypesEnum.STONE, 0.90f);
            Add(MyMwcVoxelMaterialsEnum.Treasure_01, MyMwcObjectBuilder_Ore_TypesEnum.TREASURE, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Treasure_02, MyMwcObjectBuilder_Ore_TypesEnum.TREASURE, 0.01f);
            Add(MyMwcVoxelMaterialsEnum.Uraninite_01, MyMwcObjectBuilder_Ore_TypesEnum.URANINITE, 0.01f);
        }

        private static void Add(MyMwcVoxelMaterialsEnum voxelMaterial, MyMwcObjectBuilder_Ore_TypesEnum? ore, float percents)
        {
            MaterialProperties.Add((int)voxelMaterial, new MyMwcVoxelMaterialProperties
            {
                Ore = ore,
                Percents = percents
            });
        }

        public static MyMwcVoxelMaterialProperties GetShipTypeProperties(MyMwcVoxelMaterialsEnum material)
        {
            Debug.Assert(MaterialProperties.ContainsKey((int)material));
            return MaterialProperties[(int)material];
        }
    }
}
