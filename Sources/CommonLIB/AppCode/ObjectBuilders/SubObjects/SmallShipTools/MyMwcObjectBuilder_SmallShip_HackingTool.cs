using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum : ushort
    {
        Level_1 = 1,
        Level_2 = 2,
        Level_3 = 3,
        Level_4 = 4,
        Level_5 = 5,
    }

    public class MyMwcObjectBuilder_SmallShip_HackingTool : MyMwcObjectBuilder_SubObjectBase
    {
        public MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum HackingToolType;

        internal MyMwcObjectBuilder_SmallShip_HackingTool()
            : base()
        {
        }

        public MyMwcObjectBuilder_SmallShip_HackingTool(MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum hackingToolType)
        {
            HackingToolType = hackingToolType;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)HackingToolType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            HackingToolType = (MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  HackingTool Type
            MyMwcLog.IfNetVerbose_AddToLog("HackingToolType: " + HackingToolType);
            MyMwcMessageOut.WriteObjectBuilderSmallShipHackingToolTypesEnum(HackingToolType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  HackingTool Type
            MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum? hackingToolType = MyMwcMessageIn.ReadObjectBuilderSmallShipHackingToolTypesEnumEx(binaryReader, senderEndPoint);
            if (hackingToolType == null) return NetworkError();
            HackingToolType = hackingToolType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("HackingToolType: " + HackingToolType);

            return true;
        }
    }
}
