using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWarsMath.Graphics.PackedVector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using SysUtils;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;

//  Outgoing / sent message

//  Method that have "Ex" suffix in the name are considered to be safe. They will not throw an exception
//  when error during reading the data. 

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public static class MyMwcMessageOut
    {
        public static void WriteBool(bool val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val);
        }

        public static void WriteByteNullable(byte? val, BinaryWriter binaryWriter)
        {
            MyMwcMessageOut.WriteBool(val.HasValue, binaryWriter);
            if (val.HasValue)
            {
                WriteByte(val.Value, binaryWriter);
            }
        }

        public static void WriteByte(byte val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val);
        }
        
        public static void WriteByte4(Byte4 val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val.PackedValue);
        }

        public static void WriteInt16(Int16 val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val);
        }

        public static void WriteInt32(Int32 val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val);
        }

        public static void WriteInt64(Int64 val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val);
        }

        public static void WriteUInt32(UInt32 val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val);
        }

        public static void WriteDateTime(DateTime val, BinaryWriter binaryWriter)
        {
            WriteInt64(val.Ticks, binaryWriter);
        }

        public static void WriteVector3(Vector3 val, BinaryWriter binaryWriter)
        {
            WriteFloat(val.X, binaryWriter);
            WriteFloat(val.Y, binaryWriter);
            WriteFloat(val.Z, binaryWriter);
        }

        public static void WriteVector4(Vector4 val, BinaryWriter binaryWriter)
        {
            WriteFloat(val.X, binaryWriter);
            WriteFloat(val.Y, binaryWriter);
            WriteFloat(val.Z, binaryWriter);
            WriteFloat(val.W, binaryWriter);
        }

        public static void WritePositionAndOrientation(MyMwcPositionAndOrientation positionAndOrientation, BinaryWriter binaryWriter)
        {
            WriteVector3(positionAndOrientation.Position, binaryWriter);
            WriteVector3(positionAndOrientation.Forward, binaryWriter);
            WriteVector3(positionAndOrientation.Up, binaryWriter);
        }

        public static void WriteColor(Color val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val.R);
            binaryWriter.Write(val.G);
            binaryWriter.Write(val.B);
            binaryWriter.Write(val.A);
        }

        public static void WriteObjectBuilderTypeEnum(MyMwcObjectBuilderTypeEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteVoxelFilesEnum(MyMwcVoxelFilesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((short)val);
        }

        public static void WriteVoxelMaterialsEnum(MyMwcVoxelMaterialsEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteVoxelMapMergeTypeEnum(MyMwcVoxelMapMergeTypeEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteVoxelHandModeTypeEnum(MyMwcVoxelHandModeTypeEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteObjectBuilder3DSmallShipTypesEnum(MyMwcObjectBuilder_SmallShip_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteObjectBuilderSmallShipAmmoTypesEnum(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderSmallShipWeaponTypesEnum(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderSmallShipAssignmentOfAmmoFireKeyEnum(MyMwcObjectBuilder_FireKeyEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteObjectBuilderSmallShipAssignmentOfAmmoGroupEnum(MyMwcObjectBuilder_AmmoGroupEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteObjectBuilderSmallShipEngineTypesEnum(MyMwcObjectBuilder_SmallShip_Engine_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderSmallShipHackingToolTypesEnum(MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }
        
        public static void WriteObjectBuilderLargeShipAmmoTypesEnum(MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderSmallShipToolTypesEnum(MyMwcObjectBuilder_SmallShip_Tool_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderLargeShipTypesEnum(MyMwcObjectBuilder_LargeShip_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderSmallDebrisTypesEnum(MyMwcObjectBuilder_SmallDebris_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderLargeDebrisFieldTypesEnum(MyMwcObjectBuilder_LargeDebrisField_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderStaticAsteroidTypesEnum(MyMwcObjectBuilder_StaticAsteroid_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderPrefabContainerTypesEnum(MyMwcObjectBuilder_PrefabContainer_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderPrefabTypesEnum(MyMwcObjectBuilder_Prefab_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderCargoBoxTypesEnum(MyMwcObjectBuilder_CargoBox_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteObjectBuilderMysteriousCubeTypesEnum(MyMwcObjectBuilder_MysteriousCube_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((byte)val);
        }
        
        public static void WriteObjectBuilderEntityDetectorTypesEnum(MyMwcObjectBuilder_EntityDetector_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteObjectBuilderInventoryTemplateTypesEnum(MyMwcInventoryTemplateTypeEnum val, BinaryWriter binaryWriter) 
        {
            binaryWriter.Write((byte)val);
        }

        public static void WriteVector3Sbyte(MyMwcVector3Sbyte val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val.X);
            binaryWriter.Write(val.Y);
            binaryWriter.Write(val.Z);
        }
        
        public static void WriteVector3Short(MyMwcVector3Short val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val.X);
            binaryWriter.Write(val.Y);
            binaryWriter.Write(val.Z);
        }

        public static void WriteVector3Int(MyMwcVector3Int val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(val.X);
            binaryWriter.Write(val.Y);
            binaryWriter.Write(val.Z);
        }
        
        public static void WriteFloat(float val, BinaryWriter binaryWriter)
        {
            MyMwcUtils.FixDenormalizedFloat(ref val);

            if (!MyCommonDebugUtils.IsValid(val))
            {
                throw new InvalidOperationException("Cannot write invalid float (infinity or NaN) to network message!");
            }
            binaryWriter.Write(val);
        }

        public static void WriteString(string val, BinaryWriter binaryWriter)
        {
            //  String can't be null because null is used to signal error in reading the packet. Instead use empty string.
            MyCommonDebugUtils.AssertDebug(val != null);
            binaryWriter.Write(val);
        }

        public static void WriteNullableString(string value, BinaryWriter binaryWriter)
        {
            bool hasValue = value != null;
            MyMwcMessageOut.WriteBool(hasValue, binaryWriter);
            if (hasValue)
            {
                MyMwcMessageOut.WriteString(value, binaryWriter);
            }
        }

        public static void WriteCollection<T>(ICollection<T> collection, BinaryWriter binaryWriter)
            where T : MyMwcObjectBuilder_Base
        {
            WriteCollection(collection, collection.Count, binaryWriter);
        }

        public static void WriteCollection<T>(IEnumerable<T> collection, int count, BinaryWriter binaryWriter)
            where T : MyMwcObjectBuilder_Base
        {
            WriteInt32(count, binaryWriter);
            int i = 0;
            foreach (var obj in collection)
            {
                obj.Write(binaryWriter);
                i++;
            }
            Debug.Assert(count == i, "When writting object collection, number of elements found does not match count argument");
        }

        public static void WriteSectorIdentifier(MyMwcSectorIdentifier val, BinaryWriter binaryWriter)
        {
            WriteInt16((short)val.SectorType, binaryWriter);
            WriteBool(val.UserId.HasValue, binaryWriter);
            if (val.UserId.HasValue)
            {
                WriteInt32(val.UserId.Value, binaryWriter);
            }
            WriteVector3Int(val.Position, binaryWriter);
            WriteString(val.SectorName, binaryWriter);
        }

        public static void WriteUserDetail(MyMwcUserDetail val, BinaryWriter binaryWriter)
        {
            WriteInt32(val.UserId, binaryWriter);
            WriteString(val.DisplayName, binaryWriter);

        }

        public static void WriteObjectBuilderSmallShipArmorTypesEnum(MyMwcObjectBuilder_SmallShip_Armor_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }
        
        public static void WriteObjectBuilderSmallShipRadarTypesEnum(MyMwcObjectBuilder_SmallShip_Radar_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }
        
        public static void WriteObjectBuilderOreTypesEnum(MyMwcObjectBuilder_Ore_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }
        
        public static void WriteObjectBuildeBlueprintTypesEnum(MyMwcObjectBuilder_Blueprint_TypesEnum val, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((ushort)val);
        }

        public static void WriteEndpoint(EndPoint endpoint, BinaryWriter binaryWriter)
        {
            IPEndPoint ep = endpoint as IPEndPoint;
            if (ep == null)
                throw new InvalidOperationException("Cannot write other endpoint than IPEndPoint");

            var bytes = ep.Address.GetAddressBytes();
            binaryWriter.Write((byte)bytes.Length);
            binaryWriter.Write(bytes);
            binaryWriter.Write(ep.Port);
        }

        public static void WriteResultCode(MyResultCodeEnum resultCode, BinaryWriter writer)
        {
            writer.Write((byte)resultCode);
        }
    }
}