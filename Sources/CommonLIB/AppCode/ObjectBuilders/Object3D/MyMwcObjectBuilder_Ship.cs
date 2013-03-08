using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public abstract class MyMwcObjectBuilder_Ship : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_FactionEnum Faction { get; set; }
        public MyMwcObjectBuilder_Inventory Inventory { get; set; }
        public string DisplayName { get; set; }

        protected MyMwcObjectBuilder_Ship()
            : base()
        {            
        }

        protected MyMwcObjectBuilder_Ship(MyMwcObjectBuilder_Inventory inventory)
            : this()
        {
            Inventory = inventory;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            Inventory.RemapEntityIds(remapContext);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);            

            //  Inventory
            bool isInventory = Inventory != null;
            MyMwcMessageOut.WriteBool(isInventory, binaryWriter);
            if (isInventory) Inventory.Write(binaryWriter);
            MyMwcMessageOut.WriteInt32((int)Faction, binaryWriter);

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
            Faction = (MyMwcObjectBuilder_FactionEnum)MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);

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
                DisplayName = null;
            }

            return true;
        }
    }
}
