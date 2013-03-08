using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.IO;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects
{
    public class MyMwcObjectBuilder_FalseId : MyMwcObjectBuilder_SubObjectBase
    {        
        public int FactionIndex { get; set; }

        public MyMwcObjectBuilder_FactionEnum Faction 
        {
            get { return MyMwcFactionsByIndex.GetFaction(FactionIndex); }
            set { FactionIndex = MyMwcFactionsByIndex.GetFactionIndex(value); }
        }

        internal MyMwcObjectBuilder_FalseId()
            : base()
        {                 
        }

        public MyMwcObjectBuilder_FalseId(int factionIndex)
        {
            FactionIndex = factionIndex;
        }

        public MyMwcObjectBuilder_FalseId(MyMwcObjectBuilder_FactionEnum faction)
            : this(MyMwcFactionsByIndex.GetFactionIndex(faction))
        {            
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.FalseId;
        }        

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            FactionIndex = objectBuilderId.Value;
        }

        public override int? GetObjectBuilderId()
        {
            return FactionIndex;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcMessageOut.WriteInt32(FactionIndex, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            FactionIndex = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint).Value;

            return true;
        }
    }
}
