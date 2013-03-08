using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using SysUtils.Utils;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    public enum MyMwcInventoryTemplateTypeEnum : byte
    {
        MerchantMixed = 1,
        MerchantArmy = 2,
        MerchantMedicine = 3,
        MerchantBlueprint = 4,
        MerchantTools = 5,
        MPMerchantFrontLine = 6,
        MPMerchantSupport = 7,

        // --- merchants by tiers ---
        MerchantMixed_Tier_1 = 8,
        MerchantMixed_Tier_2 = 9,
        MerchantMixed_Tier_3 = 10,
        MerchantMixed_Tier_4 = 11,
        MerchantMixed_Tier_5 = 12,
        MerchantMixed_Tier_6 = 13,
        MerchantMixed_Tier_7 = 14,
        MerchantMixed_Tier_8 = 15,
        MerchantMixed_Tier_9 = 16,
        MerchantMixed_Tier_Special = 17,

        MerchantArmy_Tier_1 = 18,
        MerchantArmy_Tier_2 = 19,
        MerchantArmy_Tier_3 = 20,
        MerchantArmy_Tier_4 = 21,
        MerchantArmy_Tier_5 = 22,
        MerchantArmy_Tier_6 = 23,
        MerchantArmy_Tier_7 = 24,
        MerchantArmy_Tier_8 = 25,
        MerchantArmy_Tier_9 = 26,
        MerchantArmy_Tier_Special = 27,

        MerchantMedicine_Tier_1 = 28,
        MerchantMedicine_Tier_2 = 29,
        MerchantMedicine_Tier_3 = 30,
        MerchantMedicine_Tier_4 = 31,
        MerchantMedicine_Tier_5 = 32,
        MerchantMedicine_Tier_6 = 33,
        MerchantMedicine_Tier_7 = 34,
        MerchantMedicine_Tier_8 = 35,
        MerchantMedicine_Tier_9 = 36,
        MerchantMedicine_Tier_Special = 37,

        MerchantTools_Tier_1 = 38,
        MerchantTools_Tier_2 = 39,
        MerchantTools_Tier_3 = 40,
        MerchantTools_Tier_4 = 41,
        MerchantTools_Tier_5 = 42,
        MerchantTools_Tier_6 = 43,
        MerchantTools_Tier_7 = 44,
        MerchantTools_Tier_8 = 45,
        MerchantTools_Tier_9 = 46,
        MerchantTools_Tier_Special = 47,

    }

    public class MyMwcObjectBuilder_Inventory : MyMwcObjectBuilder_SubObjectBase
    {
        public List<MyMwcObjectBuilder_InventoryItem> InventoryItems { get; set; }
        public int MaxItems { get; set; }
        public MyMwcInventoryTemplateTypeEnum? TemplateType { get; set; }
        public float? PriceCoeficient { get; set; }
        public bool UnlimitedCapacity { get; set; }

        internal MyMwcObjectBuilder_Inventory()
            : base()
        {
            InventoryItems = new List<MyMwcObjectBuilder_InventoryItem>();
        }

        public MyMwcObjectBuilder_Inventory(List<MyMwcObjectBuilder_InventoryItem> inventoryItems, int maxItems)
            : this(inventoryItems, maxItems, null, null)
        {
        }

        public MyMwcObjectBuilder_Inventory(List<MyMwcObjectBuilder_InventoryItem> inventoryItems, int maxItems, MyMwcInventoryTemplateTypeEnum? templateType, float? priceCoeficient)
        {
            InventoryItems = inventoryItems;
            MaxItems = maxItems;
            TemplateType = templateType;
            PriceCoeficient = priceCoeficient;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var item in InventoryItems)
            {
                item.RemapEntityIds(remapContext);
            }
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Inventory;
        }

        public void InsertBonusShipRazerClaw()
        {
            foreach (var item in InventoryItems)
            {
                var shipBuilder = item.ItemObjectBuilder as MyMwcObjectBuilder_SmallShip;
                if (shipBuilder != null && shipBuilder.ShipType == MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW)
                {
                    return;
                }
            }

            var razorBuilder = MyMwcObjectBuilder_SmallShip_Player.CreateDefaultShip(MyMwcObjectBuilder_SmallShip_TypesEnum.RAZORCLAW, MyMwcObjectBuilder_FactionEnum.Rainiers, 100);
            razorBuilder.Inventory.MaxItems = 60;
            InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(razorBuilder, 1));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void WriteTrace()
        {
            MyTrace.Indent(TraceWindow.Saving, "Inventory SmallShips");
            foreach (var item in this.InventoryItems)
            {
                var smallShip = item.ItemObjectBuilder as MyMwcObjectBuilder_SmallShip;
                if (smallShip != null)
                {
                    MyTrace.Send(TraceWindow.Saving, smallShip.DisplayName);
                }
            }
            MyTrace.UnIndent(TraceWindow.Saving);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Inventory items
            int countInventoryItems = InventoryItems == null ? 0 : InventoryItems.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countInventoryItems: " + countInventoryItems);
            MyMwcMessageOut.WriteInt32(countInventoryItems, binaryWriter);
            for (int i = 0; i < countInventoryItems; i++)
            {
                InventoryItems[i].Write(binaryWriter);
            }

            // Max items
            MyMwcLog.IfNetVerbose_AddToLog("MaxItems: " + MaxItems);
            MyMwcMessageOut.WriteInt32(MaxItems, binaryWriter);

            // Price coeficient
            bool hasPriceCoeficient = PriceCoeficient != null;
            MyMwcMessageOut.WriteBool(hasPriceCoeficient, binaryWriter);
            if (hasPriceCoeficient)
            {
                MyMwcLog.IfNetVerbose_AddToLog("PriceCoeficient: " + PriceCoeficient.Value);
                MyMwcMessageOut.WriteFloat(PriceCoeficient.Value, binaryWriter);
            }

            // Inventory template type
            bool hasInventoryTemplateType = TemplateType != null;
            MyMwcMessageOut.WriteBool(hasInventoryTemplateType, binaryWriter);
            if (hasInventoryTemplateType)
            {
                MyMwcLog.IfNetVerbose_AddToLog("TemplateType: " + TemplateType.Value);
                MyMwcMessageOut.WriteObjectBuilderInventoryTemplateTypesEnum(TemplateType.Value, binaryWriter);
            }

            // Unlimited capacity
            MyMwcLog.IfNetVerbose_AddToLog("UnlimitedCapacity: " + UnlimitedCapacity);
            MyMwcMessageOut.WriteBool(UnlimitedCapacity, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Inventory items
            int? countInventoryItems = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countInventoryItems == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countInventoryItems: " + countInventoryItems);
            InventoryItems = new List<MyMwcObjectBuilder_InventoryItem>(countInventoryItems.Value);
            for (int i = 0; i < countInventoryItems; i++)
            {
                MyMwcObjectBuilder_InventoryItem inventoryItem = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_InventoryItem;
                if (inventoryItem == null) return NetworkError();
                if (inventoryItem.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

                if (!MyMwcObjectBuilder_InventoryItem.IsDisabled(inventoryItem))
                {
                    InventoryItems.Add(inventoryItem);
                }
            }

            // Max items
            int? maxItems = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (maxItems == null) return NetworkError();
            MaxItems = maxItems.Value;
            MyMwcLog.IfNetVerbose_AddToLog("MaxItems: " + MaxItems);

            // Price coeficient
            bool? hasPriceCoeficient = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasPriceCoeficient.HasValue) return NetworkError();
            if (hasPriceCoeficient.Value)
            {
                float? priceCoeficient = MyMwcMessageIn.ReadFloat(binaryReader);
                if (priceCoeficient == null) return NetworkError();
                PriceCoeficient = priceCoeficient.Value;
                MyMwcLog.IfNetVerbose_AddToLog("PriceCoeficient: " + PriceCoeficient.Value);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("PriceCoeficient is: null");
                PriceCoeficient = null;
            }

            // Inventory template type
            bool? hasInventoryTemplateType = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasInventoryTemplateType.HasValue) return NetworkError();
            if (hasInventoryTemplateType.Value)
            {
                MyMwcInventoryTemplateTypeEnum? inventoryTemplateType = MyMwcMessageIn.ReadObjectBuilderInventoryTemplateTypesEnumEx(binaryReader, senderEndPoint);
                if (inventoryTemplateType == null) return NetworkError();
                TemplateType = inventoryTemplateType.Value;
                MyMwcLog.IfNetVerbose_AddToLog("TemplateType: " + TemplateType.Value);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("TemplateType is: null");
                TemplateType = null;
            }

            // Unlimited capacity
            bool? unlimitedCapacity = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (unlimitedCapacity == null) return NetworkError();
            UnlimitedCapacity = unlimitedCapacity.Value;
            MyMwcLog.IfNetVerbose_AddToLog("UnlimitedCapacity: " + UnlimitedCapacity);

            return true;
        }
    }
}
