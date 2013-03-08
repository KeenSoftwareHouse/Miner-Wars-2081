using System.Collections.Generic;
using System.IO;
using System.Net;
using MinerWarsMath;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using System;
using System.Data.SqlClient;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_TypesEnum : byte
    {
        LIBERATOR = 1,
        ENFORCER = 2,
        KAMMLER = 3,
        GETTYSBURG = 4,
        VIRGINIA = 5,
        BAER = 6,
        HEWER = 7,
        RAZORCLAW = 8,
        GREISER = 9,
        TRACER = 10,
        //MINER_SHIP_11_RIGJ = 11,
        //SMALL_SHIP_TALON = 12,
        JACKNIFE = 13,
        DOON = 14,
        HAMMER = 15,
        ORG = 16,
        YG = 17,
        HAWK = 18,
        PHOENIX = 19,
        LEVIATHAN = 20,
        ROCKHEATER = 21,
        STEELHEAD = 22,
        FEDER = 23,
        STANISLAV = 24
    }

    public class MyMwcObjectBuilder_SmallShip : MyMwcObjectBuilder_Ship
    {
        public MyMwcObjectBuilder_SmallShip_TypesEnum ShipType { get; set; }
        public List<MyMwcObjectBuilder_SmallShip_Weapon> Weapons { get; set; }
        [Obsolete("Ammo is now as inventory item")]
        public List<MyMwcObjectBuilder_SmallShip_Ammo> Ammo { get; set; }
        [Obsolete("Tool is now as inventory item")]
        public List<MyMwcObjectBuilder_SmallShip_Tool> Tools { get; set; }
        public MyMwcObjectBuilder_SmallShip_Engine Engine { get; set; }
        public List<MyMwcObjectBuilder_AssignmentOfAmmo> AssignmentOfAmmo { get; set; }
        public MyMwcObjectBuilder_SmallShip_Armor Armor { get; set; }
        public MyMwcObjectBuilder_SmallShip_Radar Radar { get; set; }
        public float? ShipMaxHealth { get; set; }
        public float ShipHealthRatio { get; set; }
        public float ArmorHealth { get; set; }
        public float Oxygen { get; set; }
        public float Fuel { get; set; }
        public bool ReflectorLight { get; set; }
        public bool ReflectorLongRange { get; set; }
        public float ReflectorShadowDistance { get; set; }
        public bool IsDummy { get; set; }
        public int AIPriority { get; set; }

        public uint? OwnerId { get; set; } // Not stored in DB

        internal MyMwcObjectBuilder_SmallShip()
            : base()
        {
            Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>();
            AssignmentOfAmmo = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
        }

        public MyMwcObjectBuilder_SmallShip(MyMwcObjectBuilder_SmallShip_TypesEnum shipType,
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
            float reflectorShadowDistance,
            int aiPriority)
                : base(inventory)
        {
            ShipType = shipType;
            Weapons = weapons;
            Engine = engine;                        
            AssignmentOfAmmo = assignmentOfAmmo;
            Armor = armor;
            Radar = radar;
            ShipMaxHealth = shipMaxHealth;
            ShipHealthRatio = shipHealthRatio;
            ArmorHealth = armorHealth;
            Oxygen = oxygen;
            Fuel = fuel;
            ReflectorLight = reflectorLight;
            ReflectorLongRange = reflectorLongRange;
            ReflectorShadowDistance = reflectorShadowDistance;
            AIPriority = aiPriority;
        }

        protected override void LoadPersistantFlags()
        {
            base.LoadPersistantFlags();
            PersistentFlags |= MyPersistentEntityFlags.DisplayOnHud;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            if (Weapons != null)
            {
                foreach (var weapon in Weapons)
                {
                    weapon.RemapEntityIds(remapContext);
                }
            }

            if (Ammo != null)
            {
                foreach (var ammo in Ammo)
                {
                    ammo.RemapEntityIds(remapContext);
                }
            }

            if (Tools != null)
            {
                foreach (var tool in Tools)
                {
                    tool.RemapEntityIds(remapContext);
                }
            }

            foreach (var assignment in AssignmentOfAmmo)
            {
                assignment.RemapEntityIds(remapContext);
            }

            if (Engine != null)
                Engine.RemapEntityIds(remapContext);
            
            if (Armor != null)
                Armor.RemapEntityIds(remapContext);

            if (Radar != null)
                Radar.RemapEntityIds(remapContext);

            if (OwnerId != null)
                OwnerId = remapContext.RemapEntityId(OwnerId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Ship Type
            int? objectBuilderId = GetObjectBuilderId();
            System.Diagnostics.Trace.Assert(objectBuilderId.HasValue);
            MyMwcLog.IfNetVerbose_AddToLog("ShipType: " + objectBuilderId);
            MyMwcMessageOut.WriteByte((byte) objectBuilderId.Value, binaryWriter);

            //  Weapons
            int countWeapons = Weapons == null ? 0 : Weapons.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countWeapons: " + countWeapons);
            MyMwcMessageOut.WriteInt32(countWeapons, binaryWriter);
            for (int i = 0; i < countWeapons; i++)
            {
                Weapons[i].Write(binaryWriter);
            }

            ////  Ammo
            //int countAmmo = Ammo == null ? 0 : Ammo.Count;
            //MyMwcLog.IfNetVerbose_AddToLog("countAmmo: " + countAmmo);
            //MyMwcMessageOut.WriteInt32(countAmmo, binaryWriter);
            //for (int i = 0; i < countAmmo; i++)
            //{
            //    Ammo[i].Write(binaryWriter);
            //}

            //  Assignment of ammo
            int countAssignAmmo = AssignmentOfAmmo == null ? 0 : AssignmentOfAmmo.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countAssignAmmo: " + countAssignAmmo);
            MyMwcMessageOut.WriteInt32(countAssignAmmo, binaryWriter);
            for (int i = 0; i < countAssignAmmo; i++)
            {
                AssignmentOfAmmo[i].Write(binaryWriter);
            }

            ////  Tools
            //int countTools = Tools == null ? 0 : Tools.Count;
            //MyMwcLog.IfNetVerbose_AddToLog("countTools: " + countTools);
            //MyMwcMessageOut.WriteInt32(countTools, binaryWriter);
            //for (int i = 0; i < countTools; i++)
            //{
            //    Tools[i].Write(binaryWriter);
            //}

            //  Engine
            bool isEngine = Engine != null;
            MyMwcMessageOut.WriteBool(isEngine, binaryWriter);
            if (isEngine) Engine.Write(binaryWriter);

            //  Armor
            bool isArmor = Armor != null;
            MyMwcMessageOut.WriteBool(isArmor, binaryWriter);
            if (isArmor) Armor.Write(binaryWriter);

            //  Radar
            bool isRadar = Radar != null;
            MyMwcMessageOut.WriteBool(isRadar, binaryWriter);
            if (isRadar) Radar.Write(binaryWriter);

            bool hasMaxHealth = ShipMaxHealth != null;
            MyMwcMessageOut.WriteBool(hasMaxHealth, binaryWriter);
            if (hasMaxHealth)
            {
                if (float.IsNaN(ShipMaxHealth.Value) || float.IsInfinity(ShipMaxHealth.Value))
                {
                    System.Diagnostics.Debug.Fail("ShipMaxHealth is: " + ShipMaxHealth.Value);
                    ShipMaxHealth = null; // MyGameplayConstants.MAX_HEALTH_MAX;
                }

                // Ship's max health
                MyMwcLog.IfNetVerbose_AddToLog("ShipMaxHealth: " + ShipMaxHealth.Value);
                MyMwcMessageOut.WriteFloat(ShipMaxHealth.Value, binaryWriter);
            }

            // Ship's health ratio
            MyMwcLog.IfNetVerbose_AddToLog("ShipHealthRatio: " + ShipHealthRatio);
            System.Diagnostics.Debug.Assert(ShipHealthRatio <= 1f, "Ship health ratio is greater than 1f");
            System.Diagnostics.Debug.Assert(ShipHealthRatio >= 0f, "Ship health ratio is lesser than 0f");
            MyMwcMessageOut.WriteFloat(ShipHealthRatio, binaryWriter);

            // Armor's health
            MyMwcLog.IfNetVerbose_AddToLog("ArmorHealth: " + ArmorHealth);
            MyMwcMessageOut.WriteFloat(ArmorHealth, binaryWriter);

            // Oxygen
            MyMwcLog.IfNetVerbose_AddToLog("Oxygen: " + Oxygen);
            MyMwcMessageOut.WriteFloat(Oxygen, binaryWriter);

            // Fuel
            MyMwcLog.IfNetVerbose_AddToLog("Fuel: " + Fuel);
            MyMwcMessageOut.WriteFloat(Fuel, binaryWriter);

            // Refelector light
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorLight: " + ReflectorLight);
            MyMwcMessageOut.WriteBool(ReflectorLight, binaryWriter);

            // Refelector long range
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorLongRange: " + ReflectorLongRange);
            MyMwcMessageOut.WriteBool(ReflectorLongRange, binaryWriter);

            // Reflector shadow distance
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorShadowDistance: " + ReflectorShadowDistance);
            MyMwcMessageOut.WriteFloat(ReflectorShadowDistance, binaryWriter);

            // AI Priority
            MyMwcLog.IfNetVerbose_AddToLog("AIPriority: " + AIPriority);
            MyMwcMessageOut.WriteInt32(AIPriority, binaryWriter);

            // OwnerId
            MyMwcLog.IfNetVerbose_AddToLog("OwnerId: " + OwnerId);
            MyMwcMessageOut.WriteBool(OwnerId.HasValue, binaryWriter);
            if (OwnerId.HasValue)
                MyMwcMessageOut.WriteUInt32(OwnerId.Value, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

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
            //  Ship Type
            byte? objectBuilderID = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (objectBuilderID == null) return NetworkError();
            SetObjectBuilderId(objectBuilderID.Value);
            MyMwcLog.IfNetVerbose_AddToLog("ShipType: " + objectBuilderID.Value);

            //  Weapons And Devices
            int? countWeapons = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countWeapons == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countWeaponsAndDevices: " + countWeapons);
            Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>(countWeapons.Value);
            for (int i = 0; i < countWeapons; i++)
            {
                MyMwcObjectBuilder_SmallShip_Weapon newWAD =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Weapon;
                if (newWAD == null) return NetworkError();
                if (newWAD.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

                // Disabled weapons are not added
                if (!MyMwcObjectBuilder_InventoryItem.IsDisabled(newWAD.GetObjectBuilderType(), newWAD.GetObjectBuilderId()))
                {
                    Weapons.Add(newWAD);
                }
            }

            //  Assignment of ammo
            int? countAssignAmmo = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countAssignAmmo == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countAssignAmmo: " + countAssignAmmo);
            AssignmentOfAmmo = new List<MyMwcObjectBuilder_AssignmentOfAmmo>(countAssignAmmo.Value);
            for (int i = 0; i < countAssignAmmo; i++)
            {
                MyMwcObjectBuilder_AssignmentOfAmmo newAssignAmmo =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_AssignmentOfAmmo;
                if (newAssignAmmo == null) return NetworkError();
                if (newAssignAmmo.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                AssignmentOfAmmo.Add(newAssignAmmo);
            }


            //  Engine
            bool? isEngine = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isEngine == null) return NetworkError();
            if (isEngine.Value)
            {
                Engine =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Engine;
                if (Engine == null) return NetworkError();
                if (Engine.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Engine = null;
            }

            //  Armor
            bool? isArmor = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isArmor == null) return NetworkError();
            if (isArmor.Value)
            {
                Armor =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Armor;
                if (Armor == null) return NetworkError();
                if (Armor.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Armor = null;
            }

            //  Radar
            bool? isRadar = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isRadar == null) return NetworkError();
            if (isRadar.Value)
            {
                Radar =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Radar;
                if (Radar == null) return NetworkError();
                if (Radar.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Radar = null;
            }

            bool? hasMaxHealth = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasMaxHealth.HasValue) return NetworkError();
            if (hasMaxHealth.Value)
            {
                //  Ship max health
                float? shipMaxHealth = MyMwcMessageIn.ReadFloat(binaryReader);
                if (shipMaxHealth == null) return NetworkError();
                ShipMaxHealth = shipMaxHealth.Value;
                MyMwcLog.IfNetVerbose_AddToLog("ShipMaxHealth: " + ShipMaxHealth.Value);

                if (float.IsNaN(ShipMaxHealth.Value) || float.IsInfinity(ShipMaxHealth.Value))
                {
                    System.Diagnostics.Debug.Fail("ShipMaxHealth is: " + ShipMaxHealth.Value);
                    ShipMaxHealth = null; // MyGameplayConstants.MAX_HEALTH_MAX;
                }
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("ShipMaxHealth is: null");
                ShipMaxHealth = null;
            }

            //  Ship health ratio
            float? shipHealthRatio = MyMwcMessageIn.ReadFloat(binaryReader);
            if (shipHealthRatio == null) return NetworkError();
            ShipHealthRatio = shipHealthRatio.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ShipHealthRatio: " + ShipHealthRatio);

            //  Armor health
            float? armorHealth = MyMwcMessageIn.ReadFloat(binaryReader);
            if (armorHealth == null) return NetworkError();
            ArmorHealth = armorHealth.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ArmorHealth: " + ArmorHealth);

            //  Oxygen
            float? oxygen = MyMwcMessageIn.ReadFloat(binaryReader);
            if (oxygen == null) return NetworkError();
            Oxygen = oxygen.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Oxygen: " + Oxygen);

            //  Fuel
            float? fuel = MyMwcMessageIn.ReadFloat(binaryReader);
            if (fuel == null) return NetworkError();
            Fuel = fuel.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Fuel: " + Fuel);

            // Reflector light
            bool? reflectorLight = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (reflectorLight == null) return NetworkError();
            ReflectorLight = reflectorLight.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorLight: " + ReflectorLight);

            // Reflector long range
            bool? reflectorLongRange = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (reflectorLongRange == null) return NetworkError();
            ReflectorLongRange = reflectorLongRange.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorLongRange: " + ReflectorLongRange);

            //  ReflectorShadowDistance
            float? reflectorShadowDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (reflectorShadowDistance == null) return NetworkError();
            ReflectorShadowDistance = reflectorShadowDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorShadowDistance: " + ReflectorShadowDistance);

            // AI Priority
            int? aiPriority = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (aiPriority == null) return NetworkError();
            AIPriority = aiPriority.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AIPriority: " + AIPriority);

            bool? hasOwnerId = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (hasOwnerId == null) return NetworkError();
            if (hasOwnerId.Value)
            {
                uint? ownerId = MyMwcMessageIn.ReadUInt32Ex(binaryReader, senderEndPoint);
                if (ownerId == null) return NetworkError();
                OwnerId = ownerId.Value;
            }
            else
            {
                OwnerId = null;
            }
            MyMwcLog.IfNetVerbose_AddToLog("OwnerId: " + OwnerId);

            return true;
        }

        private bool Read_01085002(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            //  Ship Type
            byte? objectBuilderID = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
            if (objectBuilderID == null) return NetworkError();
            SetObjectBuilderId(objectBuilderID.Value);
            MyMwcLog.IfNetVerbose_AddToLog("ShipType: " + objectBuilderID.Value);

            //  Weapons And Devices
            int? countWeapons = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countWeapons == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countWeaponsAndDevices: " + countWeapons);
            Weapons = new List<MyMwcObjectBuilder_SmallShip_Weapon>(countWeapons.Value);
            for (int i = 0; i < countWeapons; i++)
            {
                MyMwcObjectBuilder_SmallShip_Weapon newWAD =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Weapon;
                if (newWAD == null) return NetworkError();
                if (newWAD.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                Weapons.Add(newWAD);
            }

            //  Assignment of ammo
            int? countAssignAmmo = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countAssignAmmo == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countAssignAmmo: " + countAssignAmmo);
            AssignmentOfAmmo = new List<MyMwcObjectBuilder_AssignmentOfAmmo>(countAssignAmmo.Value);
            for (int i = 0; i < countAssignAmmo; i++)
            {
                MyMwcObjectBuilder_AssignmentOfAmmo newAssignAmmo =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_AssignmentOfAmmo;
                if (newAssignAmmo == null) return NetworkError();
                if (newAssignAmmo.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                AssignmentOfAmmo.Add(newAssignAmmo);
            }


            //  Engine
            bool? isEngine = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isEngine == null) return NetworkError();
            if (isEngine.Value)
            {
                Engine =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Engine;
                if (Engine == null) return NetworkError();
                if (Engine.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Engine = null;
            }

            //  Armor
            bool? isArmor = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isArmor == null) return NetworkError();
            if (isArmor.Value)
            {
                Armor =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Armor;
                if (Armor == null) return NetworkError();
                if (Armor.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Armor = null;
            }

            //  Radar
            bool? isRadar = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isRadar == null) return NetworkError();
            if (isRadar.Value)
            {
                Radar =
                    MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as
                    MyMwcObjectBuilder_SmallShip_Radar;
                if (Radar == null) return NetworkError();
                if (Radar.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Radar = null;
            }

            bool? hasMaxHealth = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasMaxHealth.HasValue) return NetworkError();
            if (hasMaxHealth.Value)
            {
                //  Ship max health
                float? shipMaxHealth = MyMwcMessageIn.ReadFloat(binaryReader);
                if (shipMaxHealth == null) return NetworkError();
                ShipMaxHealth = shipMaxHealth.Value;
                MyMwcLog.IfNetVerbose_AddToLog("ShipMaxHealth: " + ShipMaxHealth.Value);

                if (float.IsNaN(ShipMaxHealth.Value) || float.IsInfinity(ShipMaxHealth.Value))
                {
                    System.Diagnostics.Debug.Fail("ShipMaxHealth is: " + ShipMaxHealth.Value);
                    ShipMaxHealth = null; // MyGameplayConstants.MAX_HEALTH_MAX;
                }
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("ShipMaxHealth is: null");
                ShipMaxHealth = null;
            }

            //  Ship health ratio
            float? shipHealthRatio = MyMwcMessageIn.ReadFloat(binaryReader);
            if (shipHealthRatio == null) return NetworkError();
            ShipHealthRatio = shipHealthRatio.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ShipHealthRatio: " + ShipHealthRatio);

            //  Armor health
            float? armorHealth = MyMwcMessageIn.ReadFloat(binaryReader);
            if (armorHealth == null) return NetworkError();
            ArmorHealth = armorHealth.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ArmorHealth: " + ArmorHealth);

            //  Oxygen
            float? oxygen = MyMwcMessageIn.ReadFloat(binaryReader);
            if (oxygen == null) return NetworkError();
            Oxygen = oxygen.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Oxygen: " + Oxygen);

            //  Fuel
            float? fuel = MyMwcMessageIn.ReadFloat(binaryReader);
            if (fuel == null) return NetworkError();
            Fuel = fuel.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Fuel: " + Fuel);

            // Reflector light
            bool? reflectorLight = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (reflectorLight == null) return NetworkError();
            ReflectorLight = reflectorLight.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorLight: " + ReflectorLight);

            // Reflector long range
            bool? reflectorLongRange = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (reflectorLongRange == null) return NetworkError();
            ReflectorLongRange = reflectorLongRange.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorLongRange: " + ReflectorLongRange);

            //  ReflectorShadowDistance
            float? reflectorShadowDistance = MyMwcMessageIn.ReadFloat(binaryReader);
            if (reflectorShadowDistance == null) return NetworkError();
            ReflectorShadowDistance = reflectorShadowDistance.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ReflectorShadowDistance: " + ReflectorShadowDistance);

            // AI Priority
            int? aiPriority = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (aiPriority == null) return NetworkError();
            AIPriority = aiPriority.Value;
            MyMwcLog.IfNetVerbose_AddToLog("AIPriority: " + AIPriority);

            return true;
        }


        public virtual bool IsSameAs(MyMwcObjectBuilder_Ship e)
        {
            MyMwcObjectBuilder_SmallShip  ent = e as MyMwcObjectBuilder_SmallShip;
            if(ShipType != ent.ShipType)
                return false;
            if (Weapons != ent.Weapons)
                return false;
            if (ShipMaxHealth != ent.ShipMaxHealth)
                return false;
            if (ShipHealthRatio != ent.ShipHealthRatio)
                return false;
            if (ArmorHealth != ent.ArmorHealth)
                return false;
            if (Oxygen != ent.Oxygen)
                return false;
            if (Fuel != ent.Fuel)
                return false;
            if (ReflectorLight != ent.ReflectorLight)
                return false;
            if (ReflectorLongRange != ent.ReflectorLongRange)
                return false;
            return true;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)ShipType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            ShipType = (MyMwcObjectBuilder_SmallShip_TypesEnum)Convert.ToByte(objectBuilderId);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip;
        }
    }
}
