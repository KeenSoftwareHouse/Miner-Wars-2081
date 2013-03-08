using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Data.SqlClient;
using System;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelHand_Cuboid : MyMwcObjectBuilder_VoxelHand_Shape
    {
        public float Width1;
        public float Depth1;
        public float Width2;
        public float Depth2;
        public float Length;

        internal MyMwcObjectBuilder_VoxelHand_Cuboid()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelHand_Cuboid(MyMwcPositionAndOrientation positionAndOrientation, float width1, float depth1, float width2, float depth2, float length, MyMwcVoxelHandModeTypeEnum voxelHandModeType)
            : base(positionAndOrientation, voxelHandModeType)
        {
            Width1 = width1;
            Depth1 = depth1;
            Width2 = width2;
            Depth2 = depth2;
            Length = length;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelHand_Cuboid;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("Width1: " + Width1.ToString());
            //MyMwcLog.IfNetVerbose_AddToLog("Length: " + Length.ToString());
            MyMwcMessageOut.WriteFloat(Width1, binaryWriter);
            //MyMwcMessageOut.WriteFloat(Length, binaryWriter);            
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            float? size = MyMwcMessageIn.ReadFloat(binaryReader);
            if (size == null) return NetworkError();
            Width1 = size.Value;
            MyMwcLog.IfNetVerbose_AddToLog("Width1: " + Width1.ToString());

            //float? length = MyMwcMessageIn.ReadFloat(binaryReader);
            //if (length == null) return NetworkError();
            //Length = length.Value;
            //MyMwcLog.IfNetVerbose_AddToLog("Length: " + Length.ToString());

            return true;            
        }
    }    
}
