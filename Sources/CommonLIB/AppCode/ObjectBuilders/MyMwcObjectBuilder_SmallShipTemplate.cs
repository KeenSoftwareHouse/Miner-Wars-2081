using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_SmallShipTemplate : MyMwcObjectBuilder_Base
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public MyMwcObjectBuilder_SmallShip Builder { get; set; }        

        internal MyMwcObjectBuilder_SmallShipTemplate()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShipTemplate(int id, string name, MyMwcObjectBuilder_SmallShip builder) 
        {
            ID = id;
            Name = name;
            Builder = builder;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            Builder.RemapEntityIds(remapContext);          
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  ID
            MyMwcLog.IfNetVerbose_AddToLog("ID: " + ID);
            MyMwcMessageOut.WriteInt32(ID, binaryWriter);

            //  Name
            if (Name != null)
            {
                MyMwcMessageOut.WriteBool(true, binaryWriter);
                MyMwcLog.IfNetVerbose_AddToLog("Name: " + this.Name);
                MyMwcMessageOut.WriteString(Name, binaryWriter);
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("Name: " + "null");
                MyMwcMessageOut.WriteBool(false, binaryWriter);
            }

            //  Builder
            bool hasBuilder = Builder != null;
            MyMwcMessageOut.WriteBool(hasBuilder, binaryWriter);
            if (hasBuilder) Builder.Write(binaryWriter);
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            //  ID
            int? id = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (id == null) return NetworkError();
            ID = id.Value;
            MyMwcLog.IfNetVerbose_AddToLog("ID: " + ID);

            //  Name
            bool? hasName = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasName.HasValue) return NetworkError();
            if (hasName.Value)
            {
                string name = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (name == null) return NetworkError();
                MyMwcLog.IfNetVerbose_AddToLog("Name: " + Name);
                Name = name;
            }
            else
            {
                MyMwcLog.IfNetVerbose_AddToLog("Name: " + "null");
                Name = null;
            }

            //  Builder
            bool? hasBuilder = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (hasBuilder == null) return NetworkError();
            if (hasBuilder.Value)
            {
                Builder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_SmallShip;
                if (Builder == null) return NetworkError();
                if (Builder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
            }
            else
            {
                Builder = null;
            }

            return true;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShipTemplate;
        }
    }
}
