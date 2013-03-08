using System;
using System.Collections.Generic;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Textures;
using System.Linq;
using MinerWars.AppCode.Game.Entities;

namespace MinerWars.AppCode.Game.GUI.Helpers
{

    static class MyGuiSmallShipHelpers
    {
        //Dictionaries for bindings                                                                                         
        static Dictionary<int, Dictionary<int, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum>> m_ammoBinding = new Dictionary<int, Dictionary<int, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum>>();

        //Dictionaries for helpers        
        static Dictionary<int, MyGuiSmallShipHelperAmmo> m_ammoGroups = new Dictionary<int, MyGuiSmallShipHelperAmmo>();        
        static Dictionary<int, MyGuiHelperBase> m_botPlayerRelationships = new Dictionary<int, MyGuiHelperBase>();
        static Dictionary<int, MyGuiHelperBase> m_shipFactionNationality = new Dictionary<int, MyGuiHelperBase>();
        static Dictionary<int, MyGuiHelperBase> m_botBehaviours = new Dictionary<int, MyGuiHelperBase>();
        static Dictionary<int, MyGuiHelperBase> m_botAITemplates = new Dictionary<int, MyGuiHelperBase>();
        static Dictionary<int, MyGuiHelperBase> m_patrolMode = new Dictionary<int, MyGuiHelperBase>();

        //Arrays of enums values
        public static List<MyMwcObjectBuilder_FireKeyEnum> MyMwcObjectBuilder_SmallShip_AssignmentOfAmmo_FireKeyEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_ToolEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_AssignmentOfAmmo_GroupEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_Ammo_TypesEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_Engine_TypesEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_Weapon_TypesEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_TypesEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_BotPlayerRelationshipEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_ShipFactionNationalityEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_BotBehaviourEnumValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_BotAITemplateValues { get; private set; }
        public static Array MyMwcObjectBuilder_SmallShip_PatrolModes { get; private set; }

        /// <summary>
        /// Initializes the <see cref="MyGuiSmallShipHelpers"/> class.
        /// </summary>
        static MyGuiSmallShipHelpers()
        {
            MyMwcObjectBuilder_SmallShip_AssignmentOfAmmo_FireKeyEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_FireKeyEnum)).Cast<MyMwcObjectBuilder_FireKeyEnum>().ToList();
            MyMwcObjectBuilder_SmallShip_ToolEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum));
            MyMwcObjectBuilder_SmallShip_AssignmentOfAmmo_GroupEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_AmmoGroupEnum));
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum));
            MyMwcObjectBuilder_SmallShip_Engine_TypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum));
            MyMwcObjectBuilder_SmallShip_Weapon_TypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum));
            MyMwcObjectBuilder_SmallShip_TypesEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_TypesEnum));
            MyMwcObjectBuilder_SmallShip_ShipFactionNationalityEnumValues = Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum));
            MyMwcObjectBuilder_SmallShip_BotAITemplateValues = Enum.GetValues(typeof(MyAITemplateEnum));
            MyMwcObjectBuilder_SmallShip_PatrolModes = Enum.GetValues(typeof(MyPatrolMode));

            #region Ammo/Weapon binding

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_High_Speed,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_SAPHEI,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Automatic_Rifle_With_Silencer_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Automatic_Rifle_With_Silencer);
            
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_High_Speed,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_SAPHEI,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Sniper_EMP,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Sniper);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_High_Speed,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Armor_Piercing_Incendiary,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_SAPHEI,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_EMP,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_High_Speed,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_Armor_Piercing_Incendiary,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_SAPHEI,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Machine_Gun_EMP,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Machine_Gun);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_High_Speed,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Armor_Piercing,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Shotgun_Explosive,
                MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Shotgun);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_High_Speed,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Armor_Piercing_Incendiary,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_SAPHEI,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Proximity_Explosive,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_Tunnel_Buster,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Cannon_EMP,
                MyMwcObjectBuilder_AmmoGroupEnum.Cannon, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Cannon);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Asteroid_Killer,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Directional_Explosive,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Gravity_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Illuminating_Shell,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Sphere_Explosive,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_EMP_Bomb,
                MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front);

            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic,
                MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_BioChem,
                MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_EMP,
                MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Radar_Detection,
                MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Engine_Detection,
                MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);
            BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Guided_Missile_Visual_Detection,
                MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Missile_Launcher);

            #endregion

            #region Create Bot Player Relationship helpers

            #endregion

            #region Create Ship Faction Nationality helpers

            foreach (int factionEnum in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
            {
                m_shipFactionNationality.Add(factionEnum, new MyGuiHelperBase(MyFactionConstants.GetFactionProperties((MyMwcObjectBuilder_FactionEnum)factionEnum).NameEnum));
            }

            #endregion

            #region Create Bot Behaviour helpers

            #endregion

            #region Create Bot AI Templates helpers
            m_botAITemplates.Add((int)MyAITemplateEnum.DEFAULT, new MyGuiHelperBase(MyTextsWrapperEnum.Default));
            m_botAITemplates.Add((int)MyAITemplateEnum.AGGRESIVE, new MyGuiHelperBase(MyTextsWrapperEnum.Aggressive));
            m_botAITemplates.Add((int)MyAITemplateEnum.DEFENSIVE, new MyGuiHelperBase(MyTextsWrapperEnum.Defensive));
            m_botAITemplates.Add((int)MyAITemplateEnum.FLEE, new MyGuiHelperBase(MyTextsWrapperEnum.Flee));
            m_botAITemplates.Add((int)MyAITemplateEnum.CRAZY, new MyGuiHelperBase(MyTextsWrapperEnum.Crazy));
            m_botAITemplates.Add((int)MyAITemplateEnum.PASSIVE, new MyGuiHelperBase(MyTextsWrapperEnum.Passive));
            #endregion

            #region Create Patrol Mode helpers
            m_patrolMode.Add((int)MyPatrolMode.CYCLE, new MyGuiHelperBase(MyTextsWrapperEnum.Cycle));
            m_patrolMode.Add((int)MyPatrolMode.PING_PONG, new MyGuiHelperBase(MyTextsWrapperEnum.PingPong));
            m_patrolMode.Add((int)MyPatrolMode.ONE_WAY, new MyGuiHelperBase(MyTextsWrapperEnum.OneWay));
            #endregion
        }

        public static MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? GetWeaponType(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo, MyMwcObjectBuilder_AmmoGroupEnum group)
        {
            MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum ret;
            Dictionary<int, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum> dic;
            if (m_ammoBinding.TryGetValue((int)ammo, out dic))
            {
                if (dic.TryGetValue((int)group, out ret))
                {
                    return ret;
                }
            }
            return null;
        }

        public static MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? GetFirstWeaponType(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo)
        {
            Dictionary<int, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum> dic;
            if (m_ammoBinding.TryGetValue((int)ammo, out dic))
            {
                foreach (var kv in dic)
                {
                    return kv.Value;
                }
            }
            return null;
        }

        public static bool IsAmmoInGroup(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo, MyMwcObjectBuilder_AmmoGroupEnum group)
        {
            return (m_ammoBinding.ContainsKey((int)ammo) && m_ammoBinding[(int)ammo].ContainsKey((int)group));

        }

        private static void BindAmmo(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo, MyMwcObjectBuilder_AmmoGroupEnum group, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon)
        {
            if (!m_ammoBinding.ContainsKey((int)ammo)) 
                m_ammoBinding[(int)ammo] = new Dictionary<int, MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum>();
            m_ammoBinding[(int)ammo][(int)group] = weapon;
        }        

        public static MyGuiSmallShipHelperAmmo GetMyGuiSmallShipHelperAmmo(MyMwcObjectBuilder_AmmoGroupEnum ammo)
        {
            MyGuiSmallShipHelperAmmo ret;
            if (m_ammoGroups.TryGetValue((int)ammo, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiHelperBase GetMyGuiSmallShipFactionNationality(MyMwcObjectBuilder_FactionEnum shipFaction)
        {
            MyGuiHelperBase ret;
            if (m_shipFactionNationality.TryGetValue((int)shipFaction, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiHelperBase GetMyGuiSmallShipBotAITemplate(MyAITemplateEnum aiTemplate)
        {
            MyGuiHelperBase ret;
            if (m_botAITemplates.TryGetValue((int)aiTemplate, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiHelperBase GetMyGuiSmallShipPatrolMode(MyPatrolMode patrolMode)
        {
            MyGuiHelperBase ret;
            if (m_patrolMode.TryGetValue((int)patrolMode, out ret))
                return ret;
            else
                return null;
        }


        public static void UnloadContent()
        {            
            m_ammoGroups.Clear();         
        }

        public static void LoadContent()
        {
            #region Create ammo groups helpers

            m_ammoGroups.Add((int)MyMwcObjectBuilder_AmmoGroupEnum.Bullet,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\GroupBullet", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGroupBullet));

            m_ammoGroups.Add((int)MyMwcObjectBuilder_AmmoGroupEnum.Cannon,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\GroupCannon", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGroupCannon));

            m_ammoGroups.Add((int)MyMwcObjectBuilder_AmmoGroupEnum.Missile,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\GroupMissile", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGroupMissile));

            m_ammoGroups.Add((int)MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\GroupUniversalLauncherBack", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGroupUniversalLauncherBack));

            m_ammoGroups.Add((int)MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront,
                new MyGuiSmallShipHelperAmmo(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\GroupUniversalLauncherFront", flags: TextureFlags.IgnoreQuality),
                    MyTextsWrapperEnum.AmmoGroupUniversalLauncherFront));

            #endregion            
        }
    }
}
