using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Data.SqlClient;
using System;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelHand_Sphere : MyMwcObjectBuilder_VoxelHand_Shape
    {
        public float Radius;           //  Radius of sphere

        [Obsolete("Changed to public for migration, set to internal when done")]
        public MyMwcObjectBuilder_VoxelHand_Sphere()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelHand_Sphere(MyMwcPositionAndOrientation positionAndOrientation, float radius, MyMwcVoxelHandModeTypeEnum voxelHandModeType)
            : base(positionAndOrientation, voxelHandModeType)
        {
            Radius = radius;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelHand_Sphere;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("Radius: " + Radius.ToString());
            MyMwcMessageOut.WriteFloat(Radius, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            float? radius = MyMwcMessageIn.ReadFloat(binaryReader);
            if (radius == null) return NetworkError();
            Radius = radius.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Radius: " + Radius.ToString());

            return true;
        }
    }
}
