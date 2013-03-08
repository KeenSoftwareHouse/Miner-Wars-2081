using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Data.SqlClient;
using System;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelMap_MergeMaterial : MyMwcObjectBuilder_Base
    {
        public MyMwcVector3Short PositionInVoxelMapInVoxelCoords;       //  Position within voxel map, in voxel coords
        public MyMwcVoxelFilesEnum VoxelFile;                           //  Voxel file (*.vox)
        public MyMwcVoxelMaterialsEnum VoxelMaterial;                   //  Voxel material

        internal MyMwcObjectBuilder_VoxelMap_MergeMaterial()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelMap_MergeMaterial(MyMwcVector3Short positionInVoxelMapInVoxelCoords, MyMwcVoxelFilesEnum voxelFile,
            MyMwcVoxelMaterialsEnum voxelMaterial)
            : base()
        {
            PositionInVoxelMapInVoxelCoords = positionInVoxelMapInVoxelCoords;
            VoxelFile = voxelFile;
            VoxelMaterial = voxelMaterial;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelMap_MergeMaterial;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("PositionInVoxelMapInVoxelCoords: " + PositionInVoxelMapInVoxelCoords.ToString());
            MyMwcMessageOut.WriteVector3Short(PositionInVoxelMapInVoxelCoords, binaryWriter);
            
            MyMwcLog.IfNetVerbose_AddToLog("VoxelFile: " + VoxelFile.ToString());
            MyMwcMessageOut.WriteVoxelFilesEnum(VoxelFile, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("VoxelMaterial: " + VoxelMaterial.ToString());
            MyMwcMessageOut.WriteVoxelMaterialsEnum(VoxelMaterial, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            MyMwcVector3Short? positionInVoxelMapInVoxelCoords = MyMwcMessageIn.ReadVector3ShortEx(binaryReader, senderEndPoint);
            if (positionInVoxelMapInVoxelCoords == null) return NetworkError();
            PositionInVoxelMapInVoxelCoords = positionInVoxelMapInVoxelCoords.Value;
            MyMwcLog.IfNetVerbose_AddToLog("PositionInVoxelMapInVoxelCoords: " + PositionInVoxelMapInVoxelCoords.ToString());

            MyMwcVoxelFilesEnum? voxelFile = MyMwcMessageIn.ReadVoxelFileEnumEx(binaryReader, senderEndPoint);
            if (voxelFile == null) return NetworkError();
            VoxelFile = voxelFile.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelFile: " + VoxelFile.ToString());

            MyMwcVoxelMaterialsEnum? voxelMaterial = MyMwcMessageIn.ReadVoxelMaterialsEnumEx(binaryReader, senderEndPoint);
            if (voxelMaterial == null) return NetworkError();
            VoxelMaterial = voxelMaterial.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelMaterial: " + VoxelMaterial.ToString());

            return true;
        }
    }
}
