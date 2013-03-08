using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D
{
    public enum MyMwcObjectBuilder_EntityDetector_TypesEnum : byte
    {
        Box,
        Sphere,
    }


    public class MyMwcObjectBuilder_EntityDetector : MyMwcObjectBuilder_Object3dBase
    {
        public Vector3 Size { get; set; }
        public MyMwcObjectBuilder_EntityDetector_TypesEnum EntityDetectorType { get; set; }

        internal MyMwcObjectBuilder_EntityDetector()
            : base()
        {
            Size = Vector3.Zero;
        }

        public MyMwcObjectBuilder_EntityDetector(Vector3 size, MyMwcObjectBuilder_EntityDetector_TypesEnum entityDetectorType)            
        {
            Size = size;
            EntityDetectorType = entityDetectorType;
        }

        public override int? GetObjectBuilderId()
        {
            //only one main type
            return null;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.EntityDetector;
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            //  Entity Detector Type
            MyMwcObjectBuilder_EntityDetector_TypesEnum? entityDetectorType = MyMwcMessageIn.ReadObjectBuilderEntityDetectorTypesEnumEx(binaryReader, senderEndPoint);
            if (entityDetectorType == null) return NetworkError();
            EntityDetectorType = entityDetectorType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("EntityDetectorType: " + EntityDetectorType);

            // Size
            Vector3? size = MyMwcMessageIn.ReadVector3FloatEx(binaryReader, senderEndPoint);
            if (size == null) return NetworkError();
            Size = size.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());

            return true;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //  Entity detector Type
            MyMwcLog.IfNetVerbose_AddToLog("EntityDetectorType: " + EntityDetectorType);
            MyMwcMessageOut.WriteObjectBuilderEntityDetectorTypesEnum(EntityDetectorType, binaryWriter);

            // Size
            MyMwcLog.IfNetVerbose_AddToLog("Size: " + Size.ToString());
            MyMwcMessageOut.WriteVector3(Size, binaryWriter);
        }
    }
}
