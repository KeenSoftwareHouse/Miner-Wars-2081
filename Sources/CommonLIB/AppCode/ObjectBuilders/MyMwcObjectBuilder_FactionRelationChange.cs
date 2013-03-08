using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_FactionRelationChange : MyMwcObjectBuilder_Base
    {
        public MyMwcObjectBuilder_FactionEnum Faction1 { get; set; }
        public MyMwcObjectBuilder_FactionEnum Faction2 { get; set; }
        public float Relation { get; set; }

        internal MyMwcObjectBuilder_FactionRelationChange()
            : base()
        {
        }

        public MyMwcObjectBuilder_FactionRelationChange(MyMwcObjectBuilder_FactionEnum faction1, MyMwcObjectBuilder_FactionEnum faction2, float relation) 
        {
            Faction1 = faction1;
            Faction2 = faction2;
            Relation = relation;
        }        

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcMessageOut.WriteInt32((int)Faction1, binaryWriter);
            MyMwcMessageOut.WriteInt32((int)Faction2, binaryWriter);
            MyMwcMessageOut.WriteFloat(Relation, binaryWriter);
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            int? faction1 = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!faction1.HasValue) return NetworkError();
            Faction1 = (MyMwcObjectBuilder_FactionEnum)faction1.Value;

            int? faction2 = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!faction2.HasValue) return NetworkError();
            Faction2 = (MyMwcObjectBuilder_FactionEnum)faction2.Value;

            float? relation = MyMwcMessageIn.ReadFloat(binaryReader);
            if (!relation.HasValue) return NetworkError();
            Relation = relation.Value;

            return true;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.FactionRelationChange;
        }
    }
}
