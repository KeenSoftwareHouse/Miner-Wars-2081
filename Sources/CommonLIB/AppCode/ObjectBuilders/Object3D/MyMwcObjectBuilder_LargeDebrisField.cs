using System;
using System.IO;
using System.Net;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    //  IMPORTANT: Never change numeric values and never delete any enum item. These numbers are referenced e.g. from database.
    public enum MyMwcObjectBuilder_LargeDebrisField_TypesEnum : ushort
    {
        Debris84 = 1
    }

    public class MyMwcObjectBuilder_LargeDebrisField : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_LargeDebrisField_TypesEnum DebrisType;

        internal MyMwcObjectBuilder_LargeDebrisField()
            : base()
        {
        }

        public MyMwcObjectBuilder_LargeDebrisField(MyMwcObjectBuilder_LargeDebrisField_TypesEnum debrisType)
        {
            DebrisType = debrisType;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)DebrisType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            DebrisType = (MyMwcObjectBuilder_LargeDebrisField_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.LargeDebrisField;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            // Debris Field Type
            MyMwcLog.IfNetVerbose_AddToLog("DebrisType: " + DebrisType);
            MyMwcMessageOut.WriteObjectBuilderLargeDebrisFieldTypesEnum(DebrisType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Debris Field Type
            MyMwcObjectBuilder_LargeDebrisField_TypesEnum? debrisFieldType = MyMwcMessageIn.ReadObjectBuilderLargeDebrisFieldTypesEnumEx(binaryReader, senderEndPoint);
            if (debrisFieldType == null) return NetworkError();
            DebrisType = debrisFieldType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("DebrisType: " + DebrisType);
            return true;
        }
    }
}
