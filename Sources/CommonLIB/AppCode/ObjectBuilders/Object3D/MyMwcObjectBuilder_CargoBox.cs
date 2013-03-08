using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using System.IO;
using System.Net;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public enum MyMwcObjectBuilder_CargoBox_TypesEnum : byte
    {
        Type1 = 1,
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
        Type7 = 7,
        Type8 = 8,
        Type9 = 9,
        Type10 = 10,
        Type11 = 11,
        Type12 = 12,
        TypeProp_A = 13,
        TypeProp_B = 14,
        TypeProp_C = 15,
        TypeProp_D = 16,
        DroppedItems = 17,
    }

    public class MyMwcObjectBuilder_CargoBox : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_Inventory Inventory { get; set; }
        public MyMwcObjectBuilder_CargoBox_TypesEnum CargoBoxType { get; set; }
        public string DisplayName { get; set; }

        internal MyMwcObjectBuilder_CargoBox() : base() 
        {
            Inventory = new MyMwcObjectBuilder_Inventory();            
        }

        public MyMwcObjectBuilder_CargoBox(MyMwcObjectBuilder_Inventory inventory)            
        {
            Inventory = inventory;            
        }

        protected override void LoadPersistantFlags()
        {
 	        base.LoadPersistantFlags();
            PersistentFlags |= MyPersistentEntityFlags.DisplayOnHud;
            PersistentFlags &= ~MyPersistentEntityFlags.ActivatedOnDifficultyHard;
            PersistentFlags &= ~MyPersistentEntityFlags.ActivatedOnDifficultyNormal;
        }        

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            Inventory.RemapEntityIds(remapContext);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.CargoBox;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)CargoBoxType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            CargoBoxType = (MyMwcObjectBuilder_CargoBox_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Inventory
            bool isInventory = Inventory != null;
            MyMwcMessageOut.WriteBool(isInventory, binaryWriter);
            if (isInventory) Inventory.Write(binaryWriter);

            //  Cargo box type
            MyMwcLog.IfNetVerbose_AddToLog("CargoBoxType: " + CargoBoxType);
            MyMwcMessageOut.WriteObjectBuilderCargoBoxTypesEnum(CargoBoxType, binaryWriter);

            if (DisplayName != null)
            {
                MyMwcMessageOut.WriteBool(true, binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + this.DisplayName);
                MyMwcMessageOut.WriteString(DisplayName, binaryWriter);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + "null");
                MyMwcMessageOut.WriteBool(false, binaryWriter);
            }
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Inventory
            bool? isInventory = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isInventory == null) return NetworkError();
            if (isInventory.Value)
            {
                Inventory = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_Inventory;
                if (Inventory == null) return NetworkError();
                if (Inventory.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Inventory = null;
            }

            //  Cargo box type
            MyMwcObjectBuilder_CargoBox_TypesEnum? cargoBoxType = MyMwcMessageIn.ReadObjectBuilderCargoBoxTypesEnumEx(binaryReader, senderEndPoint);
            if (cargoBoxType == null) return NetworkError();
            CargoBoxType = cargoBoxType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("CargoBoxType: " + CargoBoxType);

            bool? hasDisplayName = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasDisplayName.HasValue) return NetworkError();
            if (hasDisplayName.Value)
            {
                string displayName = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (displayName == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + displayName);
                DisplayName = displayName;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + "null");
                Name = null;
            }

            return true;
        }
    }
}
