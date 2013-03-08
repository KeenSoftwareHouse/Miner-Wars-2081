using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public class MyMwcObjectBuilder_GlobalData : MyMwcObjectBuilder_Base
    {
        public MyMwcObjectBuilder_SmallShipTemplates Templates;

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.GlobalData;
        }

        public override void ClearEntityId()
        {
            base.ClearEntityId();
            Templates.ClearEntityId();
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            Templates.RemapEntityIds(remapContext);
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion))
                return NetworkError();

            Templates = ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_SmallShipTemplates;
            if (Templates == null || !Templates.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            return true;
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            Templates.Write(binaryWriter);
        }
    }
}
