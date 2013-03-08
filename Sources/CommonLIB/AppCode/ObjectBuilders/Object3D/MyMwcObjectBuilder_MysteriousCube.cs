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
    public enum MyMwcObjectBuilder_MysteriousCube_TypesEnum : byte
    {
        Type1 = 1,
        Type2 = 2,
        Type3 = 3,
    }

    public class MyMwcObjectBuilder_MysteriousCube : MyMwcObjectBuilder_Object3dBase
    {
        public MyMwcObjectBuilder_MysteriousCube_TypesEnum MysteriousCubeType { get; set; }

        public MyMwcObjectBuilder_MysteriousCube() : base() 
        {
        }

        public MyMwcObjectBuilder_MysteriousCube(MyMwcPositionAndOrientation positionAndOrientation)
            : base(positionAndOrientation) 
        {
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.MysteriousCube;
        }

        public override int? GetObjectBuilderId()
        {
            return (int)MysteriousCubeType;
        }

        internal override void SetObjectBuilderIdInternal(int? objectBuilderId)
        {
            MysteriousCubeType = (MyMwcObjectBuilder_MysteriousCube_TypesEnum)Convert.ToUInt16(objectBuilderId);
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Cargo box type
            MyMwcLog.IfNetVerbose_AddToLog("MysteriousCubeType: " + MysteriousCubeType);
            MyMwcMessageOut.WriteObjectBuilderMysteriousCubeTypesEnum(MysteriousCubeType, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Cargo box type
            MyMwcObjectBuilder_MysteriousCube_TypesEnum? mysteriousCubeType = MyMwcMessageIn.ReadObjectBuilderMysteriousCubeTypesEnumEx(binaryReader, senderEndPoint);
            if (mysteriousCubeType == null) return NetworkError();
            MysteriousCubeType = mysteriousCubeType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("MysteriousCubeType: " + MysteriousCubeType);

            return true;
        }
    }
}
