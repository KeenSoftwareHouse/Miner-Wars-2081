using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using System.IO;
using System.Net;
using SysUtils.Utils;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using System;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{

    public enum BotDesireType
    {
        IDLE = 0,
        SEE_ENEMY = 1,
        ATACKED_BY_ENEMY = 2,
        NO_AMMO = 3,
        LOW_HEALTH = 4,
        KILL = 5,
        CURIOUS = 6,
        FLASHED = 7
    }

    public enum BotBehaviorType
    {
        IGNORE = 0,
        PATROL = 1,
        FOLLOW = 2,
        ATTACK = 3,
        RUN_AWAY = 4,
        PANIC = 5,
        KAMIKADZE = 6,
        IDLE = 7,
        CURIOUS = 8,
    }

    /// <summary>
    /// IMPORTANT: update MyGuiSmallShipHelpers and BotAITemplates when you change this
    /// </summary>
    public enum MyAITemplateEnum
    {
        DEFAULT = 0,
        AGGRESIVE = 1,
        DEFENSIVE = 2,
        FLEE = 3,
        CRAZY = 4,
        DRONE = 5,
        PASSIVE = 6,
    }

    /// <summary>
    /// Waypoint path patrol modes
    /// </summary>
    public enum MyPatrolMode
    {
        CYCLE,
        PING_PONG,
        ONE_WAY,
    }

    public class MyMwcObjectBuilder_SmallShip_Bot : MyMwcObjectBuilder_SmallShip
    {
        public MyAITemplateEnum AITemplate { get; set; }
        public float Aggressivity { get; set; }
        public float SeeDistance { get; set; }
        public float SleepDistance { get; set; }
        public MyPatrolMode PatrolMode { get; set; }
        public int? ShipTemplateID { get; set; }
        public uint? Leader { get; set; }
        public BotBehaviorType IdleBehavior { get; set; }
        public bool LeaderLostEnabled { get; set; }
        public bool ActiveAI { get; set; }
        public float SlowDown { get; set; } // Not stored in DB

        internal MyMwcObjectBuilder_SmallShip_Bot()
            : base()
        {
            IdleBehavior = BotBehaviorType.IDLE;
            SlowDown = 1.0f;
        }

        public MyMwcObjectBuilder_SmallShip_Bot(MyMwcObjectBuilder_SmallShip smallShip)
            : this(smallShip, MyAITemplateEnum.DEFAULT, 0, 1000, 1000, MyPatrolMode.CYCLE, null, null, BotBehaviorType.IDLE, smallShip.AIPriority, false, true)
        { 
        }

        public MyMwcObjectBuilder_SmallShip_Bot(MyMwcObjectBuilder_SmallShip smallShip,
            MyAITemplateEnum aiTemplate,
            float aggressivity,
            float seeDistance,
            float sleepDistance,
            MyPatrolMode patrolMode,
            int? shipTemplateId,
            uint? leader,
            BotBehaviorType idleBehavior,
            int aiPriority,
            bool leaderLostEnabled,
            bool activeAI)
            : this(smallShip.ShipType, smallShip.Inventory, smallShip.Weapons, smallShip.Engine, smallShip.AssignmentOfAmmo, smallShip.Armor, smallShip.Radar,
            smallShip.ShipMaxHealth, smallShip.ShipHealthRatio, smallShip.ArmorHealth, smallShip.Oxygen, smallShip.Fuel,
            smallShip.ReflectorLight, smallShip.ReflectorLongRange, smallShip.Faction, aiTemplate, aggressivity, seeDistance, sleepDistance, patrolMode, leader, idleBehavior, smallShip.ReflectorShadowDistance,
            aiPriority, leaderLostEnabled, activeAI) 
        {
            ShipTemplateID = shipTemplateId;
        }

        public MyMwcObjectBuilder_SmallShip_Bot(MyMwcObjectBuilder_SmallShip_TypesEnum shipType,
            MyMwcObjectBuilder_Inventory inventory,
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons,
            MyMwcObjectBuilder_SmallShip_Engine engine, 
            List<MyMwcObjectBuilder_AssignmentOfAmmo> assignmentOfAmmo,
            MyMwcObjectBuilder_SmallShip_Armor armor,
            MyMwcObjectBuilder_SmallShip_Radar radar,            
            float? shipMaxHealth,
            float shipHealthRatio,
            float armorHealth,
            float oxygen,
            float fuel,
            bool reflectorLight,
            bool reflectorLongRange,
            MyMwcObjectBuilder_FactionEnum shipFaction,
            MyAITemplateEnum aiTemplate,
            float aggressivity,
            float seeDistance,
            float sleepDistance,
            MyPatrolMode patrolMode,
            uint? leader,
            BotBehaviorType idleBehavior,
            float reflectorShadowDistance,
            int aiPriority,
            bool leaderLostEnabled,
            bool activeAI)
            : base(shipType, inventory, weapons, engine, assignmentOfAmmo, armor, radar, shipMaxHealth, shipHealthRatio, armorHealth, oxygen, fuel, reflectorLight, reflectorLongRange, reflectorShadowDistance, aiPriority)
        {
            Faction = shipFaction;
            AITemplate = aiTemplate;
            Aggressivity = aggressivity;
            SeeDistance = seeDistance;
            SleepDistance = sleepDistance;
            PatrolMode = patrolMode;
            Leader = leader;
            IdleBehavior = idleBehavior;
            LeaderLostEnabled = leaderLostEnabled;
            ActiveAI = activeAI;
            SlowDown = 1;
        }

        public override bool IsSameAs(MyMwcObjectBuilder_Ship bot)
        {
            return base.IsSameAs(bot);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_Bot;
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) 
                return NetworkError();

            if (gameVersion > 01085002)
            {
                return Read_Current(binaryReader, senderEndPoint, gameVersion);
            }
            else
            {
                return Read_01085002(binaryReader, senderEndPoint, gameVersion);
            }
        }

        private bool Read_Current(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            MyCommonDebugUtils.AssertDebug(Faction != 0);

            // AITemplate
            int? aiTemplate = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (aiTemplate == null) return NetworkError();
            AITemplate = (MyAITemplateEnum) aiTemplate.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AITemplate: " + AITemplate);

            //  Aggressivity
            float? aggressivity = MyMwcMessageIn.ReadFloat(binaryReader);
            if (aggressivity == null) return NetworkError();
            Aggressivity = aggressivity.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Aggressivity: " + Aggressivity);

            // SeeDistance
            float? seeDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (seeDistance == null) return NetworkError();
            SeeDistance = seeDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SeeDistance: " + SeeDistance);

            // SleepDistance
            float? sleepDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (sleepDistance == null) return NetworkError();
            SleepDistance = sleepDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SleepDistance: " + SleepDistance);

            // Patrol mode
            int? patrolMode = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (patrolMode == null) return NetworkError();
            PatrolMode = (MyPatrolMode) patrolMode.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PatrolMode: " + PatrolMode);

            // Ship template ID
            bool? hasShipTemplateID = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasShipTemplateID.HasValue) return NetworkError();

            if (hasShipTemplateID.Value)
            {
                int? shipTemplateID = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
                if (shipTemplateID == null) return NetworkError();
                ShipTemplateID = shipTemplateID.Value;
                MyMwcLog.IfNetVerbose_AddToLog("ShipTemplateID: " + ShipTemplateID);
            }
            else
            {
                ShipTemplateID = null;
                MyMwcLog.IfNetVerbose_AddToLog("ShipTemplateID: null");
            }

            // Leader
            bool? hasLeader = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasLeader.HasValue) return NetworkError();

            if (hasLeader.Value)
            {
                int? leader = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
                if (!leader.HasValue) return NetworkError();

                Leader = (uint) leader.Value;
                MyMwcLog.IfNetVerbose_AddToLog("Leader: " + Leader);
            }
            else
            {
                Leader = null;
                MyMwcLog.IfNetVerbose_AddToLog("Leader: null");
            }

            // Idle Behavior
            byte? idleBehavior = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (!idleBehavior.HasValue) NetworkError();

            IdleBehavior = (BotBehaviorType) idleBehavior.Value;
            MyMwcLog.IfNetVerbose_AddToLog("IdleBehavior: " + IdleBehavior);

            // Leader Lost Enabled
            bool? leaderLostEnabled = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!leaderLostEnabled.HasValue) return NetworkError();
            LeaderLostEnabled = leaderLostEnabled.Value;
            MyMwcLog.IfNetVerbose_AddToLog("LeaderLostEnabled: " + LeaderLostEnabled);

            // Active AI
            bool? activeAI = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!activeAI.HasValue) return NetworkError();
            ActiveAI = activeAI.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ActiveAI: " + ActiveAI);

            float slowDown = MyMwcMessageIn.ReadFloat(binaryReader);
            SlowDown = slowDown;
            MyMwcLog.IfNetVerbose_AddToLog("SlowDown: " + SlowDown);


            return true;
        }

        private bool Read_01085002(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            MyCommonDebugUtils.AssertDebug(Faction != 0);

            // AITemplate
            int? aiTemplate = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (aiTemplate == null) return NetworkError();
            AITemplate = (MyAITemplateEnum)aiTemplate.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AITemplate: " + AITemplate);

            //  Aggressivity
            float? aggressivity = MyMwcMessageIn.ReadFloat(binaryReader);
            if (aggressivity == null) return NetworkError();
            Aggressivity = aggressivity.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Aggressivity: " + Aggressivity);

            // SeeDistance
            float? seeDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (seeDistance == null) return NetworkError();
            SeeDistance = seeDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SeeDistance: " + SeeDistance);

            // SleepDistance
            float? sleepDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (sleepDistance == null) return NetworkError();
            SleepDistance = sleepDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("SleepDistance: " + SleepDistance);

            // Patrol mode
            int? patrolMode = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (patrolMode == null) return NetworkError();
            PatrolMode = (MyPatrolMode)patrolMode.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PatrolMode: " + PatrolMode);

            // Ship template ID
            bool? hasShipTemplateID = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasShipTemplateID.HasValue) return NetworkError();

            if (hasShipTemplateID.Value)
            {
                int? shipTemplateID = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
                if (shipTemplateID == null) return NetworkError();
                ShipTemplateID = shipTemplateID.Value;
                MyMwcLog.IfNetVerbose_AddToLog("ShipTemplateID: " + ShipTemplateID);
            }
            else
            {
                ShipTemplateID = null;
                MyMwcLog.IfNetVerbose_AddToLog("ShipTemplateID: null");
            }

            // Leader
            bool? hasLeader = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasLeader.HasValue) return NetworkError();

            if (hasLeader.Value)
            {
                int? leader = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
                if (!leader.HasValue) return NetworkError();

                Leader = (uint)leader.Value;
                MyMwcLog.IfNetVerbose_AddToLog("Leader: " + Leader);
            }
            else
            {
                Leader = null;
                MyMwcLog.IfNetVerbose_AddToLog("Leader: null");
            }

            // Idle Behavior
            byte? idleBehavior = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (!idleBehavior.HasValue) NetworkError();

            IdleBehavior = (BotBehaviorType)idleBehavior.Value;
            MyMwcLog.IfNetVerbose_AddToLog("IdleBehavior: " + IdleBehavior);

            // Leader Lost Enabled
            bool? leaderLostEnabled = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!leaderLostEnabled.HasValue) return NetworkError();
            LeaderLostEnabled = leaderLostEnabled.Value;
            MyMwcLog.IfNetVerbose_AddToLog("LeaderLostEnabled: " + LeaderLostEnabled);

            // Active AI
            bool? activeAI = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!activeAI.HasValue) return NetworkError();
            ActiveAI = activeAI.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ActiveAI: " + ActiveAI);

            SlowDown = 1;

            return true;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // AITemplate
            MyMwcLog.IfNetVerbose_AddToLog("AITemplate: " + AITemplate);
            MyMwcMessageOut.WriteByte((byte)AITemplate, binaryWriter);

            // Aggressivity
            MyMwcLog.IfNetVerbose_AddToLog("Aggressivity: " + Aggressivity);
            MyMwcMessageOut.WriteFloat(Aggressivity, binaryWriter);

            // SeeDistance
            MyMwcLog.IfNetVerbose_AddToLog("SeeDistance: " + SeeDistance);
            MyMwcMessageOut.WriteFloat(SeeDistance, binaryWriter);

            // SleepDistance
            MyMwcLog.IfNetVerbose_AddToLog("SleepDistance: " + SleepDistance);
            MyMwcMessageOut.WriteFloat(SleepDistance, binaryWriter);

            // PatrolMode
            MyMwcLog.IfNetVerbose_AddToLog("PatrolMode: " + PatrolMode);
            MyMwcMessageOut.WriteByte((byte)PatrolMode, binaryWriter);

            // Ship Template ID            
            if (ShipTemplateID != null)
            {
                MyMwcMessageOut.WriteBool(true, binaryWriter);
                
                MyMwcLog.IfNetVerbose_AddToLog("ShipTemplateID: " + ShipTemplateID);
                MyMwcMessageOut.WriteInt32(ShipTemplateID.Value, binaryWriter);
            }
            else 
            {
                MyMwcMessageOut.WriteBool(false, binaryWriter);

                MyMwcLog.IfNetVerbose_AddToLog("ShipTemplateID: null");
            }

            // Leader
            MyMwcMessageOut.WriteBool(Leader.HasValue, binaryWriter);
            if (Leader.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("Leader: " + Leader.Value);
                MyMwcMessageOut.WriteInt32((int)Leader.Value, binaryWriter);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("Leader: null");
            }

            // Idle Behavior
            MyMwcLog.IfNetVerbose_AddToLog("IdleBehavior: " + IdleBehavior);
            MyMwcMessageOut.WriteByte((byte)IdleBehavior, binaryWriter);

            // Leader Lost Enabled
            MyMwcLog.IfNetVerbose_AddToLog("LeaderLostEnabled: " + LeaderLostEnabled);
            MyMwcMessageOut.WriteBool(LeaderLostEnabled, binaryWriter);

            // Active AI
            MyMwcLog.IfNetVerbose_AddToLog("ActiveAI: " + ActiveAI);
            MyMwcMessageOut.WriteBool(ActiveAI, binaryWriter);

            // SlowDown
            MyMwcLog.IfNetVerbose_AddToLog("SlowDown: " + SlowDown);
            MyMwcMessageOut.WriteFloat(SlowDown, binaryWriter);
        }        

        public static MyMwcObjectBuilder_SmallShip_Bot CreateObjectBuilderWithAllItems(MyMwcObjectBuilder_SmallShip_TypesEnum shipType, int maxInventoryItems)
        {
            List<MyMwcObjectBuilder_SmallShip_Weapon> weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            List<MyMwcObjectBuilder_AssignmentOfAmmo> ammoAssignment = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
            List<MyMwcObjectBuilder_InventoryItem> inventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();

            // weapons
            foreach (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum weapon in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum)))
            {
                var weaponBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(weapon);
                weaponBuilder.SetAutoMount();
                weapons.Add(weaponBuilder);
                // we want have 2x autocanon
                if (weapon == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Autocanon)
                {
                    var autocannonBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(weapon);
                    autocannonBuilder.SetAutoMount();
                    weapons.Add(autocannonBuilder);
                }
            }

            // ammo assignment
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, MyMwcObjectBuilder_AmmoGroupEnum.Missile, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Missile_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart));
            ammoAssignment.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, MyMwcObjectBuilder_AmmoGroupEnum.Bullet, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Autocannon_Basic));

            // inventory items
            // ammo
            foreach (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammo in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)))
            {
                inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Ammo(ammo), 1000f));
            }

            // tools
            foreach (MyMwcObjectBuilder_SmallShip_Tool_TypesEnum tool in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum)))
            {
                inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Tool(tool), 1f));   
            }

            // radars
            foreach (MyMwcObjectBuilder_SmallShip_Radar_TypesEnum radar in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum)))
            {
                inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Radar(radar), 1f));
            }

            // engines
            foreach (MyMwcObjectBuilder_SmallShip_Engine_TypesEnum engine in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum)))
            {
                if (engine != MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1)
                {
                    inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Engine(engine), 1f));
                }
            }

            // armors
            foreach (MyMwcObjectBuilder_SmallShip_Armor_TypesEnum armor in Enum.GetValues(typeof(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum)))
            {
                if (armor != MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic)
                {
                    inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(new MyMwcObjectBuilder_SmallShip_Armor(armor), 1f));
                }
            }

            // foundation factory
            var foundationFactory = new MyMwcObjectBuilder_PrefabFoundationFactory();
            foundationFactory.PrefabHealthRatio = 1f;
            foundationFactory.PrefabMaxHealth = null;
            inventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(foundationFactory, 1f));
            inventoryItems.RemoveAll(x => MyMwcObjectBuilder_InventoryItem.IsDisabled(x));

            if (inventoryItems.Count > maxInventoryItems)
            {
                inventoryItems = inventoryItems.GetRange(0, maxInventoryItems);
            }

            MyMwcObjectBuilder_SmallShip_Bot builder =
                new MyMwcObjectBuilder_SmallShip_Bot(
                    shipType,
                    new MyMwcObjectBuilder_Inventory(inventoryItems, maxInventoryItems),
                    weapons,
                    new MyMwcObjectBuilder_SmallShip_Engine(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum.PowerCells_1),
                    ammoAssignment,
                    new MyMwcObjectBuilder_SmallShip_Armor(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum.Basic),
                    new MyMwcObjectBuilder_SmallShip_Radar(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum.Radar_1),
                    null, 1f, float.MaxValue, float.MaxValue, float.MaxValue,
                    true, false, MyMwcObjectBuilder_FactionEnum.None, MyAITemplateEnum.DEFAULT, 0, 1000, 1000, MyPatrolMode.CYCLE, null, BotBehaviorType.IDLE, 200f, 0, false, true);

            return builder;
        }

        public void CopyBotParameters(MyMwcObjectBuilder_SmallShip_Bot copyFrom) 
        {
            AITemplate = copyFrom.AITemplate;
            Aggressivity = copyFrom.Aggressivity;
            SeeDistance = copyFrom.SeeDistance;
            SleepDistance = copyFrom.SleepDistance;
            PatrolMode = copyFrom.PatrolMode;
            ShipTemplateID = copyFrom.ShipTemplateID;
        }

        // For debugging purposes
        public override string ToString()
        {
            return "Bot: " + DisplayName.ToString() + ", ID: " + (EntityId.HasValue ? EntityId.Value.ToString() : "null");
        }
    }
}
