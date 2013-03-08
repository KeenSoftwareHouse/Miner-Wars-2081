using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;

//  Incomming / received message

//  Method that have "Ex" suffix in the name are considered to be safe. They will not throw an exception
//  when error during reading the data. 

//  This class is used to read one packet. It's wrapper for READING bytes/doubles/string/vectors/etc from bit/byte array.
//  IMPORTANT: In constructor it allocated byte array, so preallocate it in client and then use one instance all.

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public static class MyMwcMessageIn
    {
        static bool ReadBool(BinaryReader binaryReader)
        {
            return binaryReader.ReadBoolean();
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static bool? ReadBoolEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadBool(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        public static bool ReadByteNullableEx(ref byte? value, BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                if (ReadBool(binaryReader))
                {
                    value = ReadByte(binaryReader);
                }
                else
                {
                    value = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return false;
            }
        }

        public static bool ReadByteEx(ref byte value, BinaryReader reader, EndPoint sender)
        {
            try
            {
                value = reader.ReadByte();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, sender);
                return false;
            }
        }

        static byte ReadByte(BinaryReader binaryReader)
        {
            return binaryReader.ReadByte();
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static byte? ReadByteEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadByte(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        public static bool ReadInt32Ex(ref Int32 value, BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                value = ReadInt32(binaryReader);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return false;
            }
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static Int32? ReadInt32Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadInt32(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static Int64? ReadInt64Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadInt64(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static UInt16? ReadUInt16Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadUInt16(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        public static bool ReadUInt32Ex(ref UInt32 result, BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                result = ReadUInt32(binaryReader);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return false;
            }
        }

        public static UInt32? ReadUInt32Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadUInt32(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        public static UInt64? ReadUInt64Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadUInt64(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static UInt64? ReadUInt64(BinaryReader binaryReader)
        {
            return binaryReader.ReadUInt64();
        }

        static Byte4 ReadByte4(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            Byte4 result = new Byte4();
            result.PackedValue = binaryReader.ReadUInt32();

            return result;
        }

        public static Byte4? ReadByte4Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadByte4(binaryReader, senderEndPoint);
            }
            catch (Exception e)
            {
                ExceptionDuringReadingMessage(e, senderEndPoint);
                return null;
            }
        }

        public static Int16 ReadInt16(BinaryReader binaryReader)
        {
            return binaryReader.ReadInt16();
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static Int16? ReadInt16Ex(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadInt16(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static Color ReadColor(BinaryReader binaryReader)
        {
            return new Color(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static Color? ReadColorEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadColor(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static Int32 ReadInt32(BinaryReader binaryReader)
        {
            return binaryReader.ReadInt32();
        }

        static Int64 ReadInt64(BinaryReader binaryReader)
        {
            return binaryReader.ReadInt64();
        }

        static UInt16 ReadUInt16(BinaryReader binaryReader)
        {
            return binaryReader.ReadUInt16();
        }

        static UInt32 ReadUInt32(BinaryReader binaryReader)
        {
            return binaryReader.ReadUInt32();
        }

        public static float ReadFloat(BinaryReader binaryReader)
        {
            float result = binaryReader.ReadSingle();
            MyMwcUtils.FixDenormalizedFloat(ref result);

            if (!MyCommonDebugUtils.IsValid(result))
            {
                throw new InvalidOperationException("Cannot read float, float is infinity or NaN");
            }
            return result;
        }

        static DateTime ReadDateTime(BinaryReader binaryReader)
        {
            return new DateTime(binaryReader.ReadInt64());
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static DateTime? ReadDateTimeEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadDateTime(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static Vector3 ReadVector3(BinaryReader binaryReader)
        {
            float x = ReadFloat(binaryReader);
            float y = ReadFloat(binaryReader);
            float z = ReadFloat(binaryReader);
            return new Vector3(x, y, z);
        }

        static Vector4 ReadVector4(BinaryReader binaryReader)
        {
            float x = ReadFloat(binaryReader);
            float y = ReadFloat(binaryReader);
            float z = ReadFloat(binaryReader);
            float w = ReadFloat(binaryReader);
            return new Vector4(x, y, z, w);
        }
        
        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static Vector3? ReadVector3FloatEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadVector3(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }


        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static Vector4? ReadVector4FloatEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadVector4(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVector3Int? ReadVector3IntEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                int? x = ReadInt32Ex(binaryReader, senderEndPoint);
                if (x == null) return null;

                int? y = ReadInt32Ex(binaryReader, senderEndPoint);
                if (y == null) return null;

                int? z = ReadInt32Ex(binaryReader, senderEndPoint);
                if (z == null) return null;

                return new MyMwcVector3Int(x.Value, y.Value, z.Value);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static MyMwcPositionAndOrientation ReadPositionAndOrientation(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            return new MyMwcPositionAndOrientation(
                ReadVector3(binaryReader),
                ReadVector3(binaryReader),
                ReadVector3(binaryReader));
        }

        public static bool ReadPositionAndOrientationEx(ref MyMwcPositionAndOrientation positionAndOrientation, BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            var value = ReadPositionAndOrientationEx(binaryReader, senderEndPoint);
            if (value.HasValue)
            {
                positionAndOrientation = value.Value;
                return true;
            }
            return false;
        }
        
        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcPositionAndOrientation? ReadPositionAndOrientationEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadPositionAndOrientation(binaryReader, senderEndPoint);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static MyMwcVector3Sbyte ReadVector3Sbyte(BinaryReader binaryReader)
        {
            sbyte x = binaryReader.ReadSByte();
            sbyte y = binaryReader.ReadSByte();
            sbyte z = binaryReader.ReadSByte();
            return new MyMwcVector3Sbyte(x, y, z);
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVector3Sbyte? ReadVector3SbyteEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadVector3Sbyte(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        static MyMwcVector3Short ReadVector3Short(BinaryReader binaryReader)
        {
            short x = binaryReader.ReadInt16();
            short y = binaryReader.ReadInt16();
            short z = binaryReader.ReadInt16();
            return new MyMwcVector3Short(x, y, z);
        }
        
        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVector3Short? ReadVector3ShortEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                return ReadVector3Short(binaryReader);
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndPoint);
                return null;
            }
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilderTypeEnum? ReadObjectBuilderTypeEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilderTypeEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilderTypeEnum? ret = MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilderTypeEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_TypesEnum? ReadObjectBuilderSmallShipTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilder3DSmallShipTypesEnumEx: " + retByte);

            //  Convert
            MyMwcObjectBuilder_SmallShip_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_TypesEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? ReadObjectBuilderSmallShipWeaponTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipWeaponTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum? ReadObjectBuilderSmallShipAmmoTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipAmmoTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_Tool_TypesEnum? ReadObjectBuilderSmallShipToolTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipToolTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_Tool_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Tool_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_LargeShip_TypesEnum? ReadObjectBuilderLargeShipTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilder3DLargeShipTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_LargeShip_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_LargeShip_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum? ReadObjectBuilderLargeShipAmmoTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderLargeShipAmmoTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_LargeShip_Ammo_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_PrefabContainer_TypesEnum? ReadObjectBuilderPrefabContainerTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderPrefabContainerTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_PrefabContainer_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_PrefabContainer_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_Prefab_TypesEnum? ReadObjectBuilderPrefabTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderPrefabTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_Prefab_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_Prefab_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_FireKeyEnum? ReadObjectBuilderSmallShipAssignmentOfAmmoFireKeyEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipAssignmentOfAmmoFireKeyEnumEx: " + retByte);

            //  Convert
            MyMwcObjectBuilder_FireKeyEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_FireKeyEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_AmmoGroupEnum? ReadObjectBuilderSmallShipAssignmentOfAmmoGroupEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipAssignmentOfAmmoGroupEnumEx: " + retByte);

            //  Convert
            MyMwcObjectBuilder_AmmoGroupEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_AmmoGroupEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_Engine_TypesEnum? ReadObjectBuilderSmallShipEngineTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipEngineTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_Engine_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Engine_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum? ReadObjectBuilderSmallShipHackingToolTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipHackingToolTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallDebris_TypesEnum? ReadObjectBuilderSmallDebrisTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallDebrisTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallDebris_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallDebris_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_LargeDebrisField_TypesEnum? ReadObjectBuilderLargeDebrisFieldTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_LargeDebrisField_TypesEnum: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_LargeDebrisField_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_LargeDebrisField_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_StaticAsteroid_TypesEnum? ReadObjectBuilderStaticAsteroidTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("MyMwcObjectBuilder_StaticAsteroid_TypesEnum: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_StaticAsteroid_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_StaticAsteroid_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_CargoBox_TypesEnum? ReadObjectBuilderCargoBoxTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderCargoBoxTypesEnumEx: " + retByte);

            //  Convert
            MyMwcObjectBuilder_CargoBox_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_CargoBox_TypesEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_MysteriousCube_TypesEnum? ReadObjectBuilderMysteriousCubeTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderMysteriousBoxTypesEnumEx: " + retByte);

            //  Convert
            MyMwcObjectBuilder_MysteriousCube_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_MysteriousCube_TypesEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }
        
        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVoxelFilesEnum? ReadVoxelFileEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            short? retShort = ReadInt16Ex(binaryReader, senderEndPoint);
            if (retShort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadVoxelFileEnumEx: " + retShort);

            //  Convert
            MyMwcVoxelFilesEnum? ret = MyMwcClientServer.GetEnumFromNumber<MyMwcVoxelFilesEnum, short>(retShort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVoxelMaterialsEnum? ReadVoxelMaterialsEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadVoxelMaterialsEnumEx: " + retByte);

            //  Convert
            MyMwcVoxelMaterialsEnum? ret = MyMwcClientServer.GetEnumFromNumber<MyMwcVoxelMaterialsEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVoxelMapMergeTypeEnum? ReadVoxelMapMergeTypeEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadVoxelMapMergeTypeEnumEx: " + retByte);

            //  Convert
            MyMwcVoxelMapMergeTypeEnum? ret = MyMwcClientServer.GetEnumFromNumber<MyMwcVoxelMapMergeTypeEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcVoxelHandModeTypeEnum? ReadVoxelHandModeTypeEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadVoxelHandModeTypeEnumEx: " + retByte);

            //  Convert
            MyMwcVoxelHandModeTypeEnum? ret = MyMwcClientServer.GetEnumFromNumber<MyMwcVoxelHandModeTypeEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_EntityDetector_TypesEnum? ReadObjectBuilderEntityDetectorTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilder3DSmallShipTypesEnumEx: " + retByte);

            //  Convert
            MyMwcObjectBuilder_EntityDetector_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_EntityDetector_TypesEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        public static MyMwcInventoryTemplateTypeEnum? ReadObjectBuilderInventoryTemplateTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            byte? retByte = ReadByteEx(binaryReader, senderEndPoint);
            if (retByte == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderInventoryTemplateTypesEnumEx: " + retByte);

            //  Convert
            MyMwcInventoryTemplateTypeEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcInventoryTemplateTypeEnum, byte>(retByte.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        /// <summary>
        /// Creates new array of objects
        /// </summary>
        public static bool ReadArray<T>(ref T[] result, BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
            where T : MyMwcObjectBuilder_Base
        {
            int? count = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!count.HasValue) return false;

            result = new T[count.Value];
            for (int i = 0; i < count.Value; i++)
            {
                var obj = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as T;
                if (obj == null || !obj.Read(binaryReader, senderEndPoint, gameVersion)) return false;
                result[i] = obj;
            }
            return true;
        }

        public static bool ReadObjectCollection<T>(ICollection<T> addObjectToCollection, BinaryReader binaryReader, EndPoint senderEndPoint, int gameVersion)
            where T : MyMwcObjectBuilder_Base
        {
            int? count = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!count.HasValue) return false;
            for (int i = 0; i < count.Value; i++)
            {
                var obj = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(binaryReader, senderEndPoint) as T;
                if (obj == null || !obj.Read(binaryReader, senderEndPoint, gameVersion)) return false;
                addObjectToCollection.Add(obj);
            }
            return true;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcSectorIdentifier? ReadSectorIdentifierEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Convert
            short? sectorTypeShort = ReadInt16Ex(binaryReader, senderEndPoint);
            if (sectorTypeShort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("SectorTypeShort: " + sectorTypeShort);

            //  Convert
            MyMwcSectorTypeEnum? sectorType = MyMwcClientServer.GetEnumFromNumber<MyMwcSectorTypeEnum, short>(sectorTypeShort.Value, senderEndPoint);
            if (sectorType == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("SectorType: " + (int)sectorType);

            bool? hasValueUserId = ReadBoolEx(binaryReader, senderEndPoint);
            if (hasValueUserId == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("hasValueUserId: " + hasValueUserId.ToString());

            int? userId = null;
            if (hasValueUserId.Value)
            {
                userId = ReadInt32Ex(binaryReader, senderEndPoint);
                if (userId == null) return null;
                MyMwcLog.IfNetVerbose_AddToLog("UserId: " + userId);
            }

            int? x = ReadInt32Ex(binaryReader, senderEndPoint);
            if (x == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("X: " + x);

            int? y = ReadInt32Ex(binaryReader, senderEndPoint);
            if (y == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("Y: " + y);

            int? z = ReadInt32Ex(binaryReader, senderEndPoint);
            if (z == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("Z: " + z);

            //if ((x.Value < -MyMwcNetworkingMaxValuesConstants.SECTOR_IDENTIFIER_MAX_DISTANCE) ||
            //    (x.Value > +MyMwcNetworkingMaxValuesConstants.SECTOR_IDENTIFIER_MAX_DISTANCE) ||
            //    (y.Value < -MyMwcNetworkingMaxValuesConstants.SECTOR_IDENTIFIER_MAX_DISTANCE) ||
            //    (y.Value > +MyMwcNetworkingMaxValuesConstants.SECTOR_IDENTIFIER_MAX_DISTANCE) ||
            //    (z.Value < -MyMwcNetworkingMaxValuesConstants.SECTOR_IDENTIFIER_MAX_DISTANCE) ||
            //    (z.Value > +MyMwcNetworkingMaxValuesConstants.SECTOR_IDENTIFIER_MAX_DISTANCE))
            //{
            //    return null;
            //}

            string sectorName = ReadStringEx(binaryReader, senderEndPoint);
            if (sectorName == null) return null;

            return new MyMwcSectorIdentifier(sectorType.Value, userId, new MyMwcVector3Int(x.Value, y.Value, z.Value), sectorName);
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcUserDetail? ReadUserDetailEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  read userId
            int? userId = ReadInt32Ex(binaryReader, senderEndPoint);
            if (userId == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("UserId: " + userId);

            // read displayName
            string displayName = ReadStringEx(binaryReader, senderEndPoint);
            if (displayName == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("DisplayName: " + displayName);

            return new MyMwcUserDetail(userId.Value, displayName);
        }

        public static string ReadString(BinaryReader binaryReader)
        {
            return binaryReader.ReadString();
        }

        public static bool ReadStringEx(ref string value, BinaryReader reader, EndPoint senderEndpoint)
        {
            try
            {
                value = ReadString(reader);
                return value.Length < MyMwcNetworkingConstants.MAX_STRING_LENGTH;
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderEndpoint);
                return false;
            }
        }

        public static bool ReadNullableStringEx(BinaryReader binaryReader, EndPoint senderEndPoint, out string result)
        {
            result = null;

            bool? hasStringValue = MyMwcMessageIn.ReadBoolEx(binaryReader, senderEndPoint);
            if (!hasStringValue.HasValue) return false;
            if (hasStringValue.Value)
            {
                result = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndPoint);
                if (result == null) return false;
            }
            return true;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        //  IMPORTANT: THIS METHOD RETURNS ONLY STRING NO LONGER THAT 500 CHARACTERS. THAT'S PROTECTION AGAINST ATTACKERS.
        //  IF YOU NEED LONGER STRING, YOU MUST MAKE NEW METHOD AND THINK ABOUT ITS PROTECTION
        public static string ReadStringEx(BinaryReader binaryReader, EndPoint senderAddress)
        {
            try
            {
                string s = ReadString(binaryReader);
                if (s.Length > MyMwcNetworkingConstants.MAX_STRING_LENGTH)
                {
                    return null;
                }
                else
                {
                    return s;
                }
            }
            catch (Exception ex)
            {
                ExceptionDuringReadingMessage(ex, senderAddress);
                return null;
            }
        }

        //  If exception occured during reading memory stream, we log this information or inform cheater alert, but ignore the error - application continues.
        //  This is protection against users who may send messages with wrong format
        //  Of course if IGNORE_SOCKET_EXCEPTIONS == false, then exception is rethrown.
        static void ExceptionDuringReadingMessage(Exception ex, EndPoint senderEndPoint)
        {
            MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.EXCEPTION_DURING_READING_MESSAGE, senderEndPoint, ex.ToString());

            if (MyMwcFinalBuildConstants.IGNORE_SOCKET_EXCEPTIONS == false)
            {
                throw ex;
            }
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_Armor_TypesEnum? ReadObjectBuilderSmallShipArmorTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipArmorTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_Armor_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Armor_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_SmallShip_Radar_TypesEnum? ReadObjectBuilderSmallShipRadarTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderSmallShipRadarTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_SmallShip_Radar_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Radar_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_Ore_TypesEnum? ReadObjectBuilderOreTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderOreTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_Ore_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_Ore_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        //  Same as Read***() but if exception occurs during reading, we log that exception and ignore it or rethrow
        //  If ignored, null is returned and caller knows that this message failed to read.
        public static MyMwcObjectBuilder_Blueprint_TypesEnum? ReadObjectBuilderBlueprintTypesEnumEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            //  Read
            ushort? retUshort = ReadUInt16Ex(binaryReader, senderEndPoint);
            if (retUshort == null) return null;
            MyMwcLog.IfNetVerbose_AddToLog("ReadObjectBuilderBlueprintTypesEnumEx: " + retUshort);

            //  Convert
            MyMwcObjectBuilder_Blueprint_TypesEnum? ret =
                MyMwcClientServer.GetEnumFromNumber<MyMwcObjectBuilder_Blueprint_TypesEnum, ushort>(retUshort.Value, senderEndPoint);
            if (ret == null) return null;
            return ret;
        }

        public static EndPoint ReadEndpointEx(BinaryReader binaryReader, EndPoint senderEndPoint)
        {
            try
            {
                byte len = binaryReader.ReadByte();
                var bytes = binaryReader.ReadBytes(len);
                var port = binaryReader.ReadInt32();
                return new IPEndPoint(new IPAddress(bytes), port);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool ReadResultCode(ref MyResultCodeEnum resultCode, BinaryReader reader, EndPoint sender)
        {
            byte resultCodeByte = 0;
            return ReadByteEx(ref resultCodeByte, reader, sender) && MyMwcUtils.GetEnumFromNumber<MyResultCodeEnum, byte>(resultCodeByte, ref resultCode);
        }
    }
}