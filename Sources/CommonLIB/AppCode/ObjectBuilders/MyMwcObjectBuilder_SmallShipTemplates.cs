using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_SmallShipTemplates : MyMwcObjectBuilder_Base
    {
        public List<MyMwcObjectBuilder_SmallShipTemplate> SmallShipTemplates { get; set; }

        internal MyMwcObjectBuilder_SmallShipTemplates()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShipTemplates(List<MyMwcObjectBuilder_SmallShipTemplate> smallShipTemplates) 
            : this()
        {
            SmallShipTemplates = smallShipTemplates;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            if (SmallShipTemplates != null) 
            {
                foreach (var smallShipTemplate in SmallShipTemplates) 
                {
                    smallShipTemplate.RemapEntityIds(remapContext);
                }
            }    
        }

        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Small ship templates
            int countTemplates = SmallShipTemplates == null ? 0 : SmallShipTemplates.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countTemplates: " + countTemplates);
            MyMwcMessageOut.WriteInt32(countTemplates, binaryWriter);
            for (int i = 0; i < countTemplates; i++)
            {
                SmallShipTemplates[i].Write(binaryWriter);
            }
        }

        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            //  Small ship templates
            int? countTemplates = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countTemplates == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countTemplates: " + countTemplates);
            SmallShipTemplates = new List<MyMwcObjectBuilder_SmallShipTemplate>(countTemplates.Value);
            for (int i = 0; i < countTemplates; i++)
            {
                MyMwcObjectBuilder_SmallShipTemplate templateBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_SmallShipTemplate;
                if (templateBuilder == null) return NetworkError();
                if (templateBuilder.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                SmallShipTemplates.Add(templateBuilder);
            }

            return true;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShipTemplates;
        }
    }
}
