using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Data.SqlClient;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelMap_Neighbour : MyMwcObjectBuilder_Base
    {
        public MyMwcVector3Sbyte RelativeSectorPosition;            //  Position of this neighbour sector relative to current sector (in sector coords, thus -1, or +1 or so)
        public MyMwcVector3Short VoxelMapPositionInVoxelCoords;     //  Position of voxel map relative to sector center, in voxel coords
        public MyMwcVoxelFilesEnum VoxelFile;                       //  Voxel file (*.vox)
        public MyMwcVoxelMaterialsEnum VoxelMaterial;               //  Default voxel material for loading voxel map

        internal MyMwcObjectBuilder_VoxelMap_Neighbour()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelMap_Neighbour(MyMwcVector3Sbyte relativeSectorPosition, 
            MyMwcVector3Short voxelMapPositionInVoxelCoords, MyMwcVoxelFilesEnum voxelFile,
            MyMwcVoxelMaterialsEnum voxelMaterial)
            : base()
        {
            RelativeSectorPosition = relativeSectorPosition;
            VoxelMapPositionInVoxelCoords = voxelMapPositionInVoxelCoords;
            VoxelFile = voxelFile;
            VoxelMaterial = voxelMaterial;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelMap_Neighbour;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("RelativeSectorPosition: " + RelativeSectorPosition.ToString());
            MyMwcMessageOut.WriteVector3Sbyte(RelativeSectorPosition, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("VoxelMapPositionInVoxelCoords: " + VoxelMapPositionInVoxelCoords.ToString());
            MyMwcMessageOut.WriteVector3Short(VoxelMapPositionInVoxelCoords, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("VoxelFile: " + VoxelFile.ToString());
            MyMwcMessageOut.WriteVoxelFilesEnum(VoxelFile, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("VoxelMaterial: " + VoxelMaterial.ToString());
            MyMwcMessageOut.WriteVoxelMaterialsEnum(VoxelMaterial, binaryWriter);
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            MyMwcVector3Sbyte? relativeSectorPosition = MyMwcMessageIn.ReadVector3SbyteEx(binaryReader, senderEndPoint);
            if (relativeSectorPosition == null) return NetworkError();
            RelativeSectorPosition = relativeSectorPosition.Value;
            MyMwcLog.IfNetVerbose_AddToLog("RelativeSectorPosition: " + RelativeSectorPosition.ToString());

            MyMwcVector3Short? voxelMapPositionInVoxelCoords = MyMwcMessageIn.ReadVector3ShortEx(binaryReader, senderEndPoint);
            if (voxelMapPositionInVoxelCoords == null) return NetworkError();
            VoxelMapPositionInVoxelCoords = voxelMapPositionInVoxelCoords.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelMapPositionInVoxelCoords: " + VoxelMapPositionInVoxelCoords.ToString());

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
