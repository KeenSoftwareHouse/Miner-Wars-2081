using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using System.IO;
using SysUtils.Utils;
using System.Net;
using MinerWarsMath;
using System.Data.SqlClient;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public abstract class MyMwcObjectBuilder_VoxelHand_Shape : MyMwcObjectBuilder_Base
    {
        public MyMwcPositionAndOrientation PositionAndOrientation;
        public MyMwcVoxelHandModeTypeEnum VoxelHandModeType;
        public MyMwcVoxelMaterialsEnum? VoxelHandMaterial;

        protected MyMwcObjectBuilder_VoxelHand_Shape() : base()
        {
        }

        protected MyMwcObjectBuilder_VoxelHand_Shape(MyMwcPositionAndOrientation positionAndOrientation, MyMwcVoxelHandModeTypeEnum voxelHandModeType)
            : base()
        {
            PositionAndOrientation = positionAndOrientation;
            VoxelHandModeType = voxelHandModeType;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("PositionAndOrientation: " + PositionAndOrientation.ToString());
            MyMwcMessageOut.WritePositionAndOrientation(PositionAndOrientation, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("VoxelHandModeType: " + VoxelHandModeType.ToString());
            MyMwcMessageOut.WriteVoxelHandModeTypeEnum(VoxelHandModeType, binaryWriter);

            MyMwcMessageOut.WriteBool(VoxelHandMaterial.HasValue, binaryWriter);
            if (VoxelHandMaterial.HasValue)
            {
                MyMwcLog.IfNetVerbose_AddToLog("VoxelHandMaterial: " + VoxelHandMaterial.ToString());
                MyMwcMessageOut.WriteVoxelMaterialsEnum(VoxelHandMaterial.Value, binaryWriter);
            }
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            MyMwcPositionAndOrientation? objectPositionAndOrientation = MyMwcMessageIn.ReadPositionAndOrientationEx(binaryReader, senderEndPoint);
            if (objectPositionAndOrientation == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("objectPositionAndOrientation: " + objectPositionAndOrientation.ToString());

            PositionAndOrientation = objectPositionAndOrientation.Value;

            MyMwcVoxelHandModeTypeEnum? voxelHandModeType = MyMwcMessageIn.ReadVoxelHandModeTypeEnumEx(binaryReader, senderEndPoint);
            if (voxelHandModeType == null) return NetworkError();
            VoxelHandModeType = voxelHandModeType.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelHandModeType: " + VoxelHandModeType.ToString());

            bool? isVoxelMaterial = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (isVoxelMaterial == null) return NetworkError();
            if (isVoxelMaterial.Value)
            {
                VoxelHandMaterial = MyMwcMessageIn.ReadVoxelMaterialsEnumEx(binaryReader, senderEndPoint);
            }
            else
            {
                VoxelHandMaterial = null;
            }

            return true;
        }
    }
}
