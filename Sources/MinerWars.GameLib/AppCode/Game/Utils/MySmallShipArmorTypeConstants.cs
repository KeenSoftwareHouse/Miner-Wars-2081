using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Utils
{
    public struct MySmallShipArmorTypeProperties
    {
        public float Resistance;
        public bool SolarWindImmunity;

        public Vector3 DiffuseColor;
    }

    static class MySmallShipArmorTypeConstants
    {
        static Dictionary<int, MySmallShipArmorTypeProperties> ArmorProperties = new Dictionary<int, MySmallShipArmorTypeProperties>();

        static MySmallShipArmorTypeConstants()
        {
            // Basic
            ArmorProperties.Add((int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic, new MySmallShipArmorTypeProperties
            {
                Resistance = 0.3f,
                DiffuseColor = new Vector3(0, 1, 0)
            });

            // Advanced
            ArmorProperties.Add((int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced, new MySmallShipArmorTypeProperties
            {
                Resistance = 0.6f,
                DiffuseColor = new Vector3(0, 0, 1)
            });

            // High_Endurance
            ArmorProperties.Add((int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.High_Endurance, new MySmallShipArmorTypeProperties
            {
                Resistance = 1.0f,
                DiffuseColor = new Vector3(1, 0, 0)
            });

            // Solar_Wind
            ArmorProperties.Add((int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind, new MySmallShipArmorTypeProperties
            {
                Resistance = 0.0f,
                SolarWindImmunity = true,
                DiffuseColor = new Vector3(1, 0, 0)
            });
        }

        public static MySmallShipArmorTypeProperties GetProperties(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum armorType)
        {
            return ArmorProperties[(int)armorType];
        }
    }
}
