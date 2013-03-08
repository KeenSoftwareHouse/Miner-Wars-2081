using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Data.SqlClient;
using System;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelHand_Cylinder : MyMwcObjectBuilder_VoxelHand_Shape
    {
        public float Radius1;
        public float Radius2;
        public float Length;

        internal MyMwcObjectBuilder_VoxelHand_Cylinder()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelHand_Cylinder(MyMwcPositionAndOrientation positionAndOrientation, float radius1, float radius2, float length, MyMwcVoxelHandModeTypeEnum voxelHandModeType)
            : base(positionAndOrientation, voxelHandModeType)
        {
            Radius1 = radius1;
            Radius2 = radius2;
            Length = length;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelHand_Cylinder;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("Radius1: " + Radius1.ToString());
            MyMwcLog.IfNetVerbose_AddToLog("Radius2: " + Radius2.ToString());
            MyMwcLog.IfNetVerbose_AddToLog("Length: " + Length.ToString());
            MyMwcMessageOut.WriteFloat(Radius1, binaryWriter);
            MyMwcMessageOut.WriteFloat(Radius2, binaryWriter);
            MyMwcMessageOut.WriteFloat(Length, binaryWriter);            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            float? radius1 = MyMwcMessageIn.ReadFloat(binaryReader);
            if (radius1 == null) return NetworkError();
            Radius1 = radius1.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Radius1: " + Radius1.ToString());

            float? radius2 = MyMwcMessageIn.ReadFloat(binaryReader);
            if (radius2 == null) return NetworkError();
            Radius2 = radius2.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Radius2: " + Radius1.ToString());

            float? length = MyMwcMessageIn.ReadFloat(binaryReader);
            if (length == null) return NetworkError();
            Length = length.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Length: " + Length.ToString());

            return true;            
        }
    }    
}
