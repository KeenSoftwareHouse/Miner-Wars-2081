using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using SysUtils.Utils;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public delegate T? ReadHandler<T>(MyMessageReader reader) where T : struct;

    public struct MyMessageReader
    {
        EndPoint m_endpoint;
        BinaryReader m_binaryReader;

        public BinaryReader Reader { get { return m_binaryReader; } }
        public EndPoint EndPoint { get { return m_endpoint; } }

        public MyMessageReader(BinaryReader reader, EndPoint endpoint)
        {
            m_endpoint = endpoint;
            m_binaryReader = reader;
        }

        void HandleError(Exception e)
        {
            HandleError(e.ToString());
        }

        void HandleError(string error)
        {
            MyMwcCheaterAlert.AddAlert(MyMwcCheaterAlertType.EXCEPTION_DURING_READING_MESSAGE, m_endpoint, error);
        }

        #region BASE_TYPES
        public bool ReadBool(ref bool value)
        {
            try
            {
                value = m_binaryReader.ReadBoolean();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadSByte(ref sbyte value)
        {
            try
            {
                value = m_binaryReader.ReadSByte();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadInt16(ref Int16 value)
        {
            try
            {
                value = m_binaryReader.ReadInt16();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadInt32(ref Int32 value)
        {
            try
            {
                value = m_binaryReader.ReadInt32();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadInt64(ref Int64 value)
        {
            try
            {
                value = m_binaryReader.ReadInt64();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadByte(ref byte value)
        {
            try
            {
                value = m_binaryReader.ReadByte();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadUInt16(ref UInt16 value)
        {
            try
            {
                value = m_binaryReader.ReadUInt16();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadUInt32(ref UInt32 value)
        {
            try
            {
                value = m_binaryReader.ReadUInt32();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadUInt64(ref UInt64 value)
        {
            try
            {
                value = m_binaryReader.ReadUInt64();
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }
        
        public bool ReadFloat(ref float value)
        {
            try
            {
                value = m_binaryReader.ReadSingle();
                if (!MyCommonDebugUtils.IsValid(value))
                {
                    HandleError("Invalid float");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadDouble(ref double value)
        {
            try
            {
                value = m_binaryReader.ReadDouble();
                if (!MyCommonDebugUtils.IsValid(value))
                {
                    HandleError("Invalid double");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadStringNullable(ref string value)
        {
            try
            {
                bool notNull = false;
                if (!ReadBool(ref notNull))
                    return false;

                if (notNull)
                    return ReadString(ref value);

                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }


        public bool ReadString(ref string value)
        {
            try
            {
                value = m_binaryReader.ReadString();
                if (value.Length > MyMwcNetworkingConstants.MAX_STRING_LENGTH)
                {
                    HandleError("String is too long");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadBytes(ref byte[] bytes, int byteCount)
        {
            if (bytes == null)
                bytes = new byte[byteCount];

            Debug.Assert(bytes != null && bytes.Length >= byteCount, "Array is too small");
            try
            {
                for (int i = 0; i < byteCount; i++)
                {
                    if (!ReadByte(ref bytes[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }
        #endregion

        #region NULLABLE_TYPES
        public bool ReadByteNullable(ref byte? val)
        {
            try
            {
                if (m_binaryReader.ReadBoolean())
                {
                    val = m_binaryReader.ReadByte();
                }
                else
                {
                    val = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadInt32Nullable(ref Int32? val)
        {
            try
            {
                if (m_binaryReader.ReadBoolean())
                {
                    val = m_binaryReader.ReadInt32();
                }
                else
                {
                    val = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadUInt32Nullable(ref UInt32? val)
        {
            try
            {
                if (m_binaryReader.ReadBoolean())
                {
                    val = m_binaryReader.ReadUInt32();
                }
                else
                {
                    val = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadFloatNullable(ref float? value)
        {
            try
            {
                if (m_binaryReader.ReadBoolean())
                {
                    value = m_binaryReader.ReadSingle();
                    if (!MyCommonDebugUtils.IsValid(value.Value))
                    {
                        HandleError("Invalid float");
                        return false;
                    }
                }
                else
                {
                    value = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadVector3Nullable(ref Vector3? value)
        {
            try
            {
                if (m_binaryReader.ReadBoolean())
                {
                    Vector3 val = new Vector3();
                    if (ReadVector3(ref val))
                    {
                        value = val;
                        return true;
                    }
                    else
                    {
                        value = null;
                        return false;
                    }
                }
                else
                {
                    value = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadEnumNullable<T>(ref T? val) where T : struct, IConvertible
        {
            try
            {
                if (m_binaryReader.ReadBoolean())
                {
                    T value;
                    if (MyMwcEnums.ReadEnum<T>(m_binaryReader, out value))
                    {
                        val = value;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    val = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }
        #endregion

        #region XNA_TYPES
        public bool ReadVector3(ref Vector3 val)
        {
            return ReadFloat(ref val.X)
                && ReadFloat(ref val.Y)
                && ReadFloat(ref val.Z);
        }

        public bool ReadVector4(ref Vector4 val)
        {
            return ReadFloat(ref val.X)
                && ReadFloat(ref val.Y)
                && ReadFloat(ref val.Z)
                && ReadFloat(ref val.W);
        }
        #endregion

        #region COMPOSED_TYPES
        public bool ReadPositionAndOrientation(ref MyMwcPositionAndOrientation positionAndOrientation)
        {
            return ReadVector3(ref positionAndOrientation.Position)
                && ReadVector3(ref positionAndOrientation.Forward)
                && ReadVector3(ref positionAndOrientation.Up);
        }

        public bool ReadIPAddess(ref IPAddress address)
        {
            try
            {
                byte ipAddressBytesLength = 0;

                if (!ReadByte(ref ipAddressBytesLength))
                {
                    return false;
                }

                byte[] ipAddressBytes = new byte[ipAddressBytesLength];
                if (!ReadBytes(ref ipAddressBytes, ipAddressBytesLength))
                {
                    return false;
                }

                address = new IPAddress(ipAddressBytes);
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadIPEndPoint(ref IPEndPoint endpoint)
        {
            try
            {
                IPAddress address = null;
                ushort port = 0;
                if (!ReadIPAddess(ref address) || !ReadUInt16(ref port))
                {
                    return false;
                }
                endpoint = new IPEndPoint(address, port);
                return true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        public bool ReadObjectBuilder<T>(ref T objectBuilder)
            where T : MyMwcObjectBuilder_Base
        {
            objectBuilder = MyMwcObjectBuilder_Base.ReadAndCreateNewObject(this.Reader, this.EndPoint) as T;
            return objectBuilder != null && objectBuilder.Read(this.Reader, this.EndPoint, MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION);
        }

        #endregion

        #region ENUMS
        public bool ReadResultCode(ref MyResultCodeEnum val)
        {
            byte numericValue = 0;
            return ReadByte(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MyResultCodeEnum, byte>(numericValue, ref val);
        }

        public bool ReadObjectBuilderSmallShipAmmoTypesEnum(ref MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum val)
        {
            ushort numericValue = 0;
            return ReadUInt16(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum, ushort>(numericValue, ref val);
        }

        public bool ReadObjectBuilderSmallShipWeaponTypesEnum(ref MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum val)
        {
            ushort numericValue = 0;
            return ReadUInt16(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum, ushort>(numericValue, ref val);
        }

        public bool ReadObjectBuilder_FactionEnum(ref MyMwcObjectBuilder_FactionEnum val)
        {
            int numericValue = 0;
            return ReadInt32(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MyMwcObjectBuilder_FactionEnum, int>(numericValue, ref val);
        }

        public bool ReadSpecialWeaponEventEnum(ref MySpecialWeaponEventEnum val)
        {
            byte numericValue = 0;
            return ReadByte(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MySpecialWeaponEventEnum, byte>(numericValue, ref val);
        }

        public bool ReadMultiplayerStateEnum(ref MyMultiplayerStateEnum val)
        {
            byte numericValue = 0;
            return ReadByte(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MyMultiplayerStateEnum, byte>(numericValue, ref val);
        }

        public bool ReadSmallShipType(ref MyMwcObjectBuilder_SmallShip_TypesEnum val)
        {
            byte numericValue = 0;
            return ReadByte(ref numericValue) && MyMwcUtils.GetEnumFromNumber<MyMwcObjectBuilder_SmallShip_TypesEnum, byte>(numericValue, ref val);
        }

        public bool ReadEnum<T>(ref T val) where T : struct, IConvertible
        {
            try
            {
                return MyMwcEnums.ReadEnum<T>(m_binaryReader, out val);
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        #endregion

        #region COLLECTIONS
        public bool ReadList<T>(ref List<T> result)
           where T : struct, IReadWriteMessage
        {
            ushort count = 0;
            if (!ReadUInt16(ref count)) return false;
            if (result == null)
            {
                result = new List<T>(count);
            }
            else
            {
                result.Clear();
            }
            for (int i = 0; i < count; i++)
            {
                var item = new T();
                if (!item.Read(this)) return false;
                result.Add(item);
            }
            return true;
        }

        public bool ReadList<T>(ref List<T> result, ReadHandler<T> reader)
           where T : struct
        {
            ushort count = 0;
            if (!ReadUInt16(ref count)) return false;
            if (result == null)
            {
                result = new List<T>(count);
            }
            else
            {
                result.Clear();
            }
            for (int i = 0; i < count; i++)
            {
                T? item = reader(this);
                if (!item.HasValue) return false;
                result.Add(item.Value);
            }
            return true;
        }
        #endregion
    }
}
