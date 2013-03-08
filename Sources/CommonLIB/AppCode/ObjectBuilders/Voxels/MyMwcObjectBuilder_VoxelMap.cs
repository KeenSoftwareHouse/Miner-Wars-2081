using System.Collections.Generic;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Data.SqlClient;
using System;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWarsMath;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels
{
    public class MyMwcObjectBuilder_VoxelMap : MyMwcObjectBuilder_Object3dBase
    {
        //public MyMwcVector3Short VoxelMapPositionInVoxelCoords;     //  Position of voxel map relative to sector center, in voxel coords
        public MyMwcVoxelFilesEnum VoxelFile;                       //  Voxel file (*.vox)
        public MyMwcVoxelMaterialsEnum VoxelMaterial;               //  Default voxel material for loading voxel map
        public List<MyMwcObjectBuilder_VoxelMap_MergeContent> MergeContents = new List<MyMwcObjectBuilder_VoxelMap_MergeContent>();
        public List<MyMwcObjectBuilder_VoxelMap_MergeMaterial> MergeMaterials = new List<MyMwcObjectBuilder_VoxelMap_MergeMaterial>();
        public List<MyMwcObjectBuilder_VoxelHand_Shape> VoxelHandShapes = new List<MyMwcObjectBuilder_VoxelHand_Shape>();

        public byte[] VoxelData = null;

        internal MyMwcObjectBuilder_VoxelMap()
            : base()
        {
        }

        public MyMwcObjectBuilder_VoxelMap(Vector3 position, MyMwcVoxelFilesEnum voxelFile, MyMwcVoxelMaterialsEnum voxelMaterial)
            : base()
        {
            PositionAndOrientation.Position = position;
            VoxelFile = voxelFile;
            VoxelMaterial = voxelMaterial;
        }

        public override void RemapEntityIds(IMyEntityIdRemapContext remapContext)
        {
            base.RemapEntityIds(remapContext);
            foreach (var content in MergeContents)
            {
                content.RemapEntityIds(remapContext);
            }

            foreach (var materials in MergeMaterials)
            {
                materials.RemapEntityIds(remapContext);
            }

            foreach (var shapes in VoxelHandShapes)
            {
                shapes.RemapEntityIds(remapContext);
            }
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.VoxelMap;
        }

        //  Write this object into message-out
        internal override void Write(BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            //MyMwcLog.IfNetVerbose_AddToLog("VoxelMapPositionInVoxelCoords: " + VoxelMapPositionInVoxelCoords.ToString());
            //MyMwcMessageOut.WriteVector3Short(VoxelMapPositionInVoxelCoords, binaryWriter);
            
            MyMwcLog.IfNetVerbose_AddToLog("VoxelFile: " + VoxelFile.ToString());
            MyMwcMessageOut.WriteVoxelFilesEnum(VoxelFile, binaryWriter);

            MyMwcLog.IfNetVerbose_AddToLog("VoxelMaterial: " + VoxelMaterial.ToString());
            MyMwcMessageOut.WriteVoxelMaterialsEnum(VoxelMaterial, binaryWriter);

            //  Merge Contents
            int countMergeContents = MergeContents == null ? 0 : MergeContents.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countMergeContents: " + countMergeContents);
            MyMwcMessageOut.WriteInt32(countMergeContents, binaryWriter);
            for (int i = 0; i < countMergeContents; i++)
            {
                MergeContents[i].Write(binaryWriter);
            }

            //  Merge Materials
            int countMergeMaterials = MergeMaterials == null ? 0 : MergeMaterials.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countMergeMaterials: " + countMergeMaterials);
            MyMwcMessageOut.WriteInt32(countMergeMaterials, binaryWriter);
            for (int i = 0; i < countMergeMaterials; i++)
            {
                MergeMaterials[i].Write(binaryWriter);
            }

            //  Voxel Hand Shapes
            int countVoxelHandShapes = VoxelHandShapes == null ? 0 : VoxelHandShapes.Count;
            MyMwcLog.IfNetVerbose_AddToLog("countVoxelHandShapes: " + countVoxelHandShapes);
            MyMwcMessageOut.WriteInt32(countVoxelHandShapes, binaryWriter);
            for (int i = 0; i < countVoxelHandShapes; i++)
            {
                VoxelHandShapes[i].Write(binaryWriter);
            }

            int length = VoxelData != null ? VoxelData.Length : 0;
            MyMwcMessageOut.WriteInt32(length, binaryWriter);
            if (length > 0)
            {
                foreach (var b in VoxelData)
                {
                    MyMwcMessageOut.WriteByte(b, binaryWriter);
                }
            }
        }

        //  Read this object from message-in
        internal override bool Read(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            if (base.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();

            // Initialize new members to default values
            VoxelData = null;

            if (gameVersion > 01089000)
            {
                return ReadCurrent(binaryReader, senderEndPoint, gameVersion);
            }
            else
            {
                return Read01089000(binaryReader, senderEndPoint, gameVersion);
            }

            //MyMwcVector3Short? voxelMapPositionInVoxelCoords = MyMwcMessageIn.ReadVector3ShortEx(binaryReader, senderEndPoint);
            //if (voxelMapPositionInVoxelCoords == null) return NetworkError();
            //VoxelMapPositionInVoxelCoords = voxelMapPositionInVoxelCoords.Value;
            //MyMwcLog.IfNetVerbose_AddToLog("VoxelMapPositionInVoxelCoords: " + VoxelMapPositionInVoxelCoords.ToString());
        }

        bool ReadCurrent(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            MyMwcVoxelFilesEnum? voxelFile = MyMwcMessageIn.ReadVoxelFileEnumEx(binaryReader, senderEndPoint);
            if (voxelFile == null) return NetworkError();
            VoxelFile = voxelFile.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelFile: " + VoxelFile.ToString());

            MyMwcVoxelMaterialsEnum? voxelMaterial = MyMwcMessageIn.ReadVoxelMaterialsEnumEx(binaryReader, senderEndPoint);
            if (voxelMaterial == null) return NetworkError();
            VoxelMaterial = voxelMaterial.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelMaterial: " + VoxelMaterial.ToString());

            //  Merge Contents
            int? countMergeContents = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countMergeContents == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countMergeContents: " + countMergeContents);
            MergeContents = new List<MyMwcObjectBuilder_VoxelMap_MergeContent>(countMergeContents.Value);
            for (int i = 0; i < countMergeContents; i++)
            {
                MyMwcObjectBuilder_VoxelMap_MergeContent newMC = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_VoxelMap_MergeContent;
                if (newMC == null) return NetworkError();
                if (newMC.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                MergeContents.Add(newMC);
            }

            //  Merge Materials
            int? countMergeMaterials = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countMergeMaterials == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countMergeMaterials: " + countMergeMaterials);
            MergeMaterials = new List<MyMwcObjectBuilder_VoxelMap_MergeMaterial>(countMergeMaterials.Value);
            for (int i = 0; i < countMergeMaterials; i++)
            {
                MyMwcObjectBuilder_VoxelMap_MergeMaterial newMM = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_VoxelMap_MergeMaterial;
                if (newMM == null) return NetworkError();
                if (newMM.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                MergeMaterials.Add(newMM);
            }

            //  Voxel Hand Shapes
            int? countVoxelHandShapes = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countVoxelHandShapes == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countVoxelHandShapes: " + countVoxelHandShapes);
            VoxelHandShapes = new List<MyMwcObjectBuilder_VoxelHand_Shape>(countVoxelHandShapes.Value);
            for (int i = 0; i < countVoxelHandShapes; i++)
            {
                MyMwcObjectBuilder_VoxelHand_Shape voxelHandShape = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_VoxelHand_Shape;
                if (voxelHandShape == null) return NetworkError();
                if (voxelHandShape.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                VoxelHandShapes.Add(voxelHandShape);
            }

            int? dataLength = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!dataLength.HasValue) return NetworkError();
            if (dataLength.Value > 0)
            {
                VoxelData = new byte[dataLength.Value];
                for (int i = 0; i < dataLength.Value; i++)
                {
                    byte? b = MyMwcMessageIn.ReadByteEx(binaryReader, senderEndPoint);
                    if (!b.HasValue) return NetworkError();
                    VoxelData[i] = b.Value;
                }
            }

            return true;
        }

        bool Read01089000(BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
        {
            MyMwcVoxelFilesEnum? voxelFile = MyMwcMessageIn.ReadVoxelFileEnumEx(binaryReader, senderEndPoint);
            if (voxelFile == null) return NetworkError();
            VoxelFile = voxelFile.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelFile: " + VoxelFile.ToString());

            MyMwcVoxelMaterialsEnum? voxelMaterial = MyMwcMessageIn.ReadVoxelMaterialsEnumEx(binaryReader, senderEndPoint);
            if (voxelMaterial == null) return NetworkError();
            VoxelMaterial = voxelMaterial.Value;
            MyMwcLog.IfNetVerbose_AddToLog("VoxelMaterial: " + VoxelMaterial.ToString());

            //  Merge Contents
            int? countMergeContents = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countMergeContents == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countMergeContents: " + countMergeContents);
            MergeContents = new List<MyMwcObjectBuilder_VoxelMap_MergeContent>(countMergeContents.Value);
            for (int i = 0; i < countMergeContents; i++)
            {
                MyMwcObjectBuilder_VoxelMap_MergeContent newMC = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_VoxelMap_MergeContent;
                if (newMC == null) return NetworkError();
                if (newMC.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                MergeContents.Add(newMC);
            }

            //  Merge Materials
            int? countMergeMaterials = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countMergeMaterials == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countMergeMaterials: " + countMergeMaterials);
            MergeMaterials = new List<MyMwcObjectBuilder_VoxelMap_MergeMaterial>(countMergeMaterials.Value);
            for (int i = 0; i < countMergeMaterials; i++)
            {
                MyMwcObjectBuilder_VoxelMap_MergeMaterial newMM = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_VoxelMap_MergeMaterial;
                if (newMM == null) return NetworkError();
                if (newMM.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                MergeMaterials.Add(newMM);
            }

            //  Voxel Hand Shapes
            int? countVoxelHandShapes = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (countVoxelHandShapes == null) return NetworkError();
            MyMwcLog.IfNetVerbose_AddToLog("countVoxelHandShapes: " + countVoxelHandShapes);
            VoxelHandShapes = new List<MyMwcObjectBuilder_VoxelHand_Shape>(countVoxelHandShapes.Value);
            for (int i = 0; i < countVoxelHandShapes; i++)
            {
                MyMwcObjectBuilder_VoxelHand_Shape voxelHandShape = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as MyMwcObjectBuilder_VoxelHand_Shape;
                if (voxelHandShape == null) return NetworkError();
                if (voxelHandShape.Read(binaryReader, senderEndPoint, gameVersion) == false) return NetworkError();
                VoxelHandShapes.Add(voxelHandShape);
            }

            return true;
        }
    }
}