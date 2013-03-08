using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.Inventory
{    
    static class MyInventoryTemplates 
    {
        private static Dictionary<int, MyInventoryTemplate> m_templates;

        static MyInventoryTemplates() 
        {
            m_templates = new Dictionary<int, MyInventoryTemplate>();
            List<MyInventoryTemplateItem> helperTemplateItems = new List<MyInventoryTemplateItem>();

            #region Basic merchant templates
            // Army merchant template            
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, 3, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper, 1, 2, 1f);



            // Medicine merchant template
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT, 1, 3, 0.5f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT, 1, 8, 0.5f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE, 1, 5, 0.5f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE, 1, 3, 0.5f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.MEDIKIT, 1, 4, 1f);

            // Blueprint merchant template
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.BasicConstructionKit, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.AdvancedConstructionKit, 1, 1, 0.66f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.FortificationKit, 1, 1, 0.33f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.HonorableKit, 1, 1, 0.1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.SuperiorConstructionKit, 1, 1, 0.01f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.UtilitiesKit, 1, 1, 0.33f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcObjectBuilderTypeEnum.Blueprint, (int)MyMwcObjectBuilder_Blueprint_TypesEnum.WeaponKit, 1, 1, 0.33f);

            // Tools merchant template
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER, 1, 1, 0.2f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT, 2, 10, 0.5f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT, 2, 10, 0.5f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_5, 1, 1, 0.1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_5, 1, 1, 0.1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.High_Endurance, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_2, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_3, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_4, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_5, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_2, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_3, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_4, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_5, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_2, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_3, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_4, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_5, 1, 1, 1f);



            // Mixed merchant template - create from all others templates but with 20% chance to refill
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy, MyMwcInventoryTemplateTypeEnum.MerchantMedicine, MyMwcInventoryTemplateTypeEnum.MerchantBlueprint, MyMwcInventoryTemplateTypeEnum.MerchantTools);
            foreach (MyInventoryTemplateItem templateItem in helperTemplateItems)
            {
                AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMixed, templateItem.ObjectBuilderType, templateItem.ObjectBuilderId, templateItem.CountToRefillMin, templateItem.CountToRefillMax, templateItem.AmountRatioMin, templateItem.AmountRatioMax, templateItem.ChanceToRefill * 0.2f);
            }

            NormalizeDistribution(MyMwcInventoryTemplateTypeEnum.MerchantTools, 5);
            NormalizeDistribution(MyMwcInventoryTemplateTypeEnum.MerchantArmy, 5);
            NormalizeDistribution(MyMwcInventoryTemplateTypeEnum.MerchantMixed, 5);

            #endregion

            #region MP merchant templates
            // MPMerchantFrontLine
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic, 2, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, 2, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic, 2, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.PERFORMANCE_ENHANCING_MEDICINE, 1, 5, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT, 5, 10, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT, 5, 10, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantFrontLine, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT, 2, 5, 1f);


            // MPMerchantSupport
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive, 1, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb, 1, 8, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.High_Endurance, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.MEDIKIT, 1, 3, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper, 1, 3, 1f);

            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneCN, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneSS, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MPMerchantSupport, MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS, 1, 1, 1f);
            #endregion

            #region Army merchant tier templates
            // Army merchant template Tier 1
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic, 3, 8, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Advanced, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun, 1, 2, 1f);

            // Army merchant template Tier 2                                                                                                                                            
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic, 2, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front, 1, 1, 1f);

            // Army merchant template Tier 3
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.High_Endurance, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back, 1, 1, 1f);

            // Army merchant template Tier 4
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun, 1, 2, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper, 1, 2, 1f);

            // Army merchant template Tier 5
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell, 1, 5, 1f);

            // Army merchant template Tier 6
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer, 1, 2, 1f);

            // Army merchant template Tier 7
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7, MyMwcObjectBuilderTypeEnum.SmallShip_Armor, (int)MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Solar_Wind, 1, 1, 1f);

            // Army merchant template Tier 8
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP, 1, 5, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int)MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem, 1, 3, 1f);

            // Army merchant template Tier 9
            // Empty now

            // Army merchant template Tier Special
            // Empty now
            #endregion

            #region Medicine merchant tier templates
            // Medicine merchant template Tier 1
            // Empty now

            // Medicine merchant template Tier 2                                                                                                                                            
            // Empty now

            // Medicine merchant template Tier 3
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.PERFORMANCE_ENHANCING_MEDICINE, 1, 3, 1f);

            // Medicine merchant template Tier 4
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_4, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_3).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.ANTIRADIATION_MEDICINE, 1, 3, 1f);

            // Medicine merchant template Tier 5
            // Empty now

            // Medicine merchant template Tier 6
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_6, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_4).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE, 1, 3, 1f);

            // Medicine merchant template Tier 7
            // Empty now

            // Medicine merchant template Tier 8
            // Empty now

            // Medicine merchant template Tier 9
            // Empty now

            // Medicine merchant template Tier Special
            // Empty now
            #endregion

            #region Tools merchant tier templates
            // Tools merchant template Tier 1
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneUS, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_2, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.FUEL_KIT, 2, 6, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_KIT, 1, 3, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.OXYGEN_KIT, 2, 6, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REPAIR_KIT, 2, 10, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device, 1, 1, 1f);            

            // Tools merchant template Tier 2
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_2, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_1, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_2, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2, MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2, 1, 1, 1f);

            // Tools merchant template Tier 3
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_3, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER, 1, 1, 1f);

            // Tools merchant template Tier 4
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneCN, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_3, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_3, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4, MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_3, 1, 1, 1f);

            // Tools merchant template Tier 5
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_5, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_5, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear, 1, 1, 1f);

            // Tools merchant template Tier 6
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_5).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_4, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_4, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6, MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.NANO_REPAIR_TOOL, 1, 3, 1f);

            // Tools merchant template Tier 7
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser, 1, 1, 1f);            
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_4, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7, MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_4, 1, 1, 1f);

            // Tools merchant template Tier 8
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Weapon, (int)MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8, MyMwcObjectBuilderTypeEnum.Drone, (int)MyMwcObjectBuilder_Drone_TypesEnum.DroneSS, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Nuclear_5, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_5, 1, 1, 1f);            

            // Tools merchant template Tier 9
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_9, GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8).TemplateItems);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_9, MyMwcObjectBuilderTypeEnum.SmallShip_Engine, (int)MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.Chemical_5, 1, 1, 1f);
            AddTemplateItem(MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_9, MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_5, 1, 1, 1f);

            // Tools merchant template Tier Special
            // Empty now
            #endregion

            #region Mixed merchant tier templates
            // Mixed merchant Tier 1
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_1, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_1, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_1);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_1, helperTemplateItems);

            // Mixed merchant Tier 2
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_2, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_2, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_2);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_2, helperTemplateItems);

            // Mixed merchant Tier 3
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_3, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_3, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_3);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_3, helperTemplateItems);

            // Mixed merchant Tier 4
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_4, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_4, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_4);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_4, helperTemplateItems);

            // Mixed merchant Tier 5
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_5, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_5, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_5);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_5, helperTemplateItems);

            // Mixed merchant Tier 6
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_6, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_6, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_6);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_6, helperTemplateItems);

            // Mixed merchant Tier 7
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_7, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_7, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_7);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_7, helperTemplateItems);

            // Mixed merchant Tier 8
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_8, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_8, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_8);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_8, helperTemplateItems);

            // Mixed merchant Tier 9
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_9, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_9, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_9);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_9, helperTemplateItems);

            // Mixed merchant Tier Special
            helperTemplateItems.Clear();
            GetInventoryTemplateItems(ref helperTemplateItems, MyMwcInventoryTemplateTypeEnum.MerchantArmy_Tier_Special, MyMwcInventoryTemplateTypeEnum.MerchantMedicine_Tier_Special, MyMwcInventoryTemplateTypeEnum.MerchantTools_Tier_Special);
            AddTemplateItems(MyMwcInventoryTemplateTypeEnum.MerchantMixed_Tier_Special, helperTemplateItems);
            #endregion
        }

        private static void NormalizeDistribution(MyMwcInventoryTemplateTypeEnum templateId, int amount)
        {
            var templates = m_templates[(int)templateId].TemplateItems;
            foreach (var template in templates)
            {
                template.ChanceToRefill = amount / (float)templates.Count;
            }
        }

        private static void AddTemplateItem(MyMwcInventoryTemplateTypeEnum templateType, MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId, int countToRefillMin, int countToRefillMax, float amountRatioMin, float amountRatioMax, float chanceToRefill) 
        {
            AddTemplateItem(templateType, new MyInventoryTemplateItem(objectBuilderType, objectBuilderId, amountRatioMin, amountRatioMax, countToRefillMin, countToRefillMax, chanceToRefill));
        }

        private static void AddTemplateItem(MyMwcInventoryTemplateTypeEnum templateType, MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId, int countToRefillMin, int countToRefillMax, float chanceToRefill)
        {
            AddTemplateItem(templateType, objectBuilderType, objectBuilderId, countToRefillMin, countToRefillMax, 1f, 1f, chanceToRefill);
        }

        private static void AddTemplateItem(MyMwcInventoryTemplateTypeEnum templateType, MyInventoryTemplateItem templateItem) 
        {
            if (!m_templates.ContainsKey((int)templateType)) 
            {
                m_templates.Add((int)templateType, new MyInventoryTemplate(templateType));
            }

            m_templates[(int)templateType].AddTemplateItem(templateItem);
        }

        private static void AddTemplateItems(MyMwcInventoryTemplateTypeEnum templateType, IEnumerable<MyInventoryTemplateItem> templateItems) 
        {
            foreach (MyInventoryTemplateItem templateItem in templateItems)
            {
                AddTemplateItem(templateType, templateItem);
            }
        }

        private static MyInventoryTemplate GetInventoryTemplate(MyMwcInventoryTemplateTypeEnum templateType) 
        {
            MyInventoryTemplate template = null;
            m_templates.TryGetValue((int)templateType, out template);
            Debug.Assert(template != null);
            return template;
        }

        private static void GetInventoryTemplateItems(ref List<MyInventoryTemplateItem> collectionToFill, params MyMwcInventoryTemplateTypeEnum[] templateTypes) 
        {
            if (templateTypes != null) 
            {
                foreach (MyMwcInventoryTemplateTypeEnum templateType in templateTypes)
                {
                    if (ContainsAnyItems(templateType)) 
                    {
                        collectionToFill.AddRange(GetInventoryTemplate(templateType).TemplateItems);
                    }
                }
            }
        }

        public static void RefillInventory(MyInventory inventory) 
        {
            Debug.Assert(inventory.TemplateType != null);
            MyInventoryTemplate template = GetInventoryTemplate(inventory.TemplateType.Value);
            template.RefillInventory(inventory);
        }

        public static bool ContainsAnyItems(MyMwcInventoryTemplateTypeEnum templateType) 
        {
            MyInventoryTemplate template = null;
            m_templates.TryGetValue((int)templateType, out template);
            return template != null && template.TemplateItems != null && template.TemplateItems.Count > 0;
        }
    }

    class MyInventoryTemplateItem 
    {
        public MyMwcObjectBuilderTypeEnum ObjectBuilderType { get; private set; }
        public int ObjectBuilderId { get; private set; }
        public float AmountRatioMin { get; private set; }
        public float AmountRatioMax { get; private set; }
        public int CountToRefillMin { get; private set; }
        public int CountToRefillMax { get; private set; }
        public float ChanceToRefill { get; set; }

        public MyInventoryTemplateItem(MyMwcObjectBuilderTypeEnum objectBuilderType, int objectBuilderId, float amountRatioMin, float amountRatioMax,
            int countToRefillMin, int countToRefillMax, float chanceToRefill) 
        {
            if (!MyMwcObjectBuilder_Base.IsObjectBuilderIdValid(objectBuilderType, objectBuilderId)) 
            {
                throw new ArgumentException("Invalid objectbuilderId!");
            }
            if (amountRatioMin < 0f) 
            {
                throw new ArgumentOutOfRangeException("amountRatioMin");
            }
            if (amountRatioMax < 0f)
            {
                throw new ArgumentOutOfRangeException("amountRatioMax");
            }
            if (amountRatioMax < amountRatioMin) 
            {
                throw new ArgumentException("Amount ratio max can't be lesser than amount ratio min!");
            }
            if (countToRefillMin < 0)
            {
                throw new ArgumentOutOfRangeException("countToRefillMin");
            }
            if (countToRefillMax < 0)
            {
                throw new ArgumentOutOfRangeException("countToRefillMax");
            }
            if (countToRefillMax < countToRefillMin)
            {
                throw new ArgumentException("Count to refill max can't be lesser than count to refill min!");
            }
            if (chanceToRefill < 0f || chanceToRefill > 1f) 
            {
                throw new ArgumentOutOfRangeException("chanceToRefill");
            }

            ObjectBuilderType = objectBuilderType;
            ObjectBuilderId = objectBuilderId;
            AmountRatioMin = amountRatioMin;
            AmountRatioMax = amountRatioMax;
            CountToRefillMin = countToRefillMin;
            CountToRefillMax = countToRefillMax;
            ChanceToRefill = chanceToRefill;
        }
    }

    class MyInventoryTemplate
    {
        public MyMwcInventoryTemplateTypeEnum TemplateType { get; private set; }
        public List<MyInventoryTemplateItem> TemplateItems { get; private set; }

        public MyInventoryTemplate(MyMwcInventoryTemplateTypeEnum templateType)
            : this(templateType, new List<MyInventoryTemplateItem>()) 
        {
        }

        public MyInventoryTemplate(MyMwcInventoryTemplateTypeEnum templateType, List<MyInventoryTemplateItem> templateItems) 
        {
            TemplateType = templateType;
            TemplateItems = templateItems;
        }

        public void AddTemplateItem(MyInventoryTemplateItem templateItem) 
        {
            TemplateItems.Add(templateItem);
        }

        public void RefillInventory(MyInventory inventory) 
        {
            foreach (MyInventoryTemplateItem templateItem in TemplateItems) 
            {
                // test chance to refill
                if (MyMwcUtils.GetRandomFloat(0.0001f, 1f) > templateItem.ChanceToRefill) 
                {
                    continue;
                }

                // get count to refill
                int existingCount = inventory.GetInventoryItemsCount(templateItem.ObjectBuilderType, templateItem.ObjectBuilderId);
                int countToRefill = MyMwcUtils.GetRandomInt(templateItem.CountToRefillMin, templateItem.CountToRefillMax) - existingCount;
                for (int i = 0; i < countToRefill; i++) 
                {
                    if (inventory.IsFull) 
                    {
                        return;
                    }

                    // get amount ratio to refill
                    float amountRatio = MyMwcUtils.GetRandomFloat(templateItem.AmountRatioMin, templateItem.AmountRatioMax);
                    if(amountRatio > 0f)
                    {
                        MyMwcObjectBuilder_Base objectBuilder = MyMwcObjectBuilder_Base.CreateNewObject(templateItem.ObjectBuilderType, templateItem.ObjectBuilderId);
                        MyInventoryItem inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(objectBuilder);
                        inventoryItem.Amount = (float)(int)(amountRatio * inventoryItem.MaxAmount);
                        inventory.AddInventoryItem(inventoryItem);
                    }
                }
            }
        }
    }
}
