using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_SectorObjectGroups: MyMwcObjectBuilder_Base
    {
        public List<MyMwcObjectBuilder_ObjectGroup> Groups;
        public List<MyMwcObjectBuilder_Base> Entities;
        
        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SectorObjectGroups;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var group in Groups)
            {
                group.RemapEntityIds(remapContext);
            }
            foreach (var entity in Entities)
            {
                entity.RemapEntityIds(remapContext);
            }
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            Groups = new List<MyMwcObjectBuilder_ObjectGroup>();
            Entities = new List<MyMwcObjectBuilder_Base>();

            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            if (!MyMwcMessageIn.ReadObjectCollection(Groups, binaryReader, senderEndPoint, gameVersion)) return NetworkError();
            if (!MyMwcMessageIn.ReadObjectCollection(Entities, binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            return true;
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcMessageOut.WriteCollection(Groups, binaryWriter);
            MyMwcMessageOut.WriteCollection(Entities, binaryWriter);
        }
    }
}
