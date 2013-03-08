using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MinerWarsMath;
using System.Net;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Networking.Multiplayer;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    public struct MyMessageWriter
    {
        BinaryWriter m_binaryWriter;

        public BinaryWriter Writer { get { return m_binaryWriter; } }

        public MyMessageWriter(BinaryWriter writer)
        {
            m_binaryWriter = writer;
        }

        #region BASE_TYPES
        public void WriteBool(bool val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteSByte(sbyte val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteInt16(Int16 val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteInt32(Int32 val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteInt64(Int64 val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteByte(byte val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteUInt16(UInt16 val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteUInt32(UInt32 val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteUInt64(UInt64 val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteFloat(float val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteDouble(double val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteString(string val)
        {
            m_binaryWriter.Write(val);
        }

        public void WriteStringNullable(string val)
        {
            if (val == null)
            {
                WriteBool(false);
            }
            else
            {
                WriteBool(true);
                m_binaryWriter.Write(val);
            }
        }

        public void WriteBytes(byte[] bytes)
        {
            m_binaryWriter.Write(bytes);
        }
        #endregion

        #region XNA_TYPES
        public void WriteVector3(Vector3 val)
        {
            WriteFloat(val.X);
            WriteFloat(val.Y);
            WriteFloat(val.Z);
        }

        public void WriteVector4(Vector4 val)
        {
            WriteFloat(val.X);
            WriteFloat(val.Y);
            WriteFloat(val.Z);
            WriteFloat(val.W);
        }
        #endregion

        #region NULLABLE_TYPES
        public void WriteByteNullable(byte? val)
        {
            m_binaryWriter.Write(val.HasValue);
            if (val.HasValue) m_binaryWriter.Write(val.Value);
        }

        public void WriteInt32Nullable(Int32? val)
        {
            m_binaryWriter.Write(val.HasValue);
            if (val.HasValue) m_binaryWriter.Write(val.Value);
        }

        public void WriteUInt32Nullable(UInt32? val)
        {
            m_binaryWriter.Write(val.HasValue);
            if (val.HasValue) m_binaryWriter.Write(val.Value);
        }

        public void WriteFloatNullable(float? val)
        {
            m_binaryWriter.Write(val.HasValue);
            if (val.HasValue) WriteFloat(val.Value);
        }

        public void WriteVector3Nullable(Vector3? val)
        {
            m_binaryWriter.Write(val.HasValue);
            if (val.HasValue) WriteVector3(val.Value);
        }

        public void WriteEnumNullable<T>(T? val) where T : struct, IConvertible
        {
            m_binaryWriter.Write(val.HasValue);
            if (val.HasValue) WriteEnum(val.Value);
        }
        #endregion

        #region COMPOSED_TYPES
        public void WritePositionAndOrientation(MyMwcPositionAndOrientation positionAndOrientation)
        {
            WriteVector3(positionAndOrientation.Position);
            WriteVector3(positionAndOrientation.Forward);
            WriteVector3(positionAndOrientation.Up);
        }

        public void WriteIPAddress(IPAddress val)
        {
            byte[] addressBytes = val.GetAddressBytes();
            WriteByte((byte)addressBytes.Length); //  In case IP won't be 32bit, but 64bit
            WriteBytes(addressBytes);
        }

        public void WriteIPEndPoint(IPEndPoint val)
        {
            WriteIPAddress(val.Address);
            WriteUInt16((ushort)val.Port);
        }

        public void WriteObjectBuilder<T>(T objectBuilder)
            where T : MyMwcObjectBuilder_Base
        {
            objectBuilder.Write(this.Writer);
        }

        #endregion

        #region ENUMS
        public void WriteResultCode(MyResultCodeEnum val)
        {
            m_binaryWriter.Write((byte)val);
        }

        public void WriteObjectBuilderSmallShipAmmoTypesEnum(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum val)
        {
            m_binaryWriter.Write((ushort)val);
        }

        public void WriteObjectBuilderSmallShipWeaponTypesEnum(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum val)
        {
            m_binaryWriter.Write((ushort)val);
        }

        public void WriteObjectBuilder_FactionEnum(MyMwcObjectBuilder_FactionEnum val)
        {
            m_binaryWriter.Write((int)val);
        }

        public void WriteSpecialWeaponEventEnum(MySpecialWeaponEventEnum val)
        {
            m_binaryWriter.Write((byte)val);
        }

        public void WriteMultiplayerStateEnum(MyMultiplayerStateEnum val)
        {
            m_binaryWriter.Write((byte)val);
        }

        public void WriteSmallShipType(MyMwcObjectBuilder_SmallShip_TypesEnum val)
        {
            m_binaryWriter.Write((byte)val);
        }

        public void WriteEnum<T>(T val) where T : struct, IConvertible
        {
            MyMwcEnums.WriteEnum<T>(m_binaryWriter, val);
        }

        #endregion

        #region COLLECTIONS
        public void WriteList<T>(List<T> list)
            where T : struct, IReadWriteMessage
        {
            int count = list != null ? list.Count : 0;
            if (count > ushort.MaxValue) throw new InvalidOperationException("List can have only " + ushort.MaxValue + "items");
            WriteUInt16((ushort)list.Count);
            for (int i = 0; i < count; i++)
            {
                list[i].Write(this);
            }
        }

        public void WriteList<T>(List<T> list, Action<MyMessageWriter, T> writer)
            where T : struct
        {
            int count = list != null ? list.Count : 0;
            if (count > ushort.MaxValue) throw new InvalidOperationException("List can have only " + ushort.MaxValue + "items");
            WriteUInt16((ushort)list.Count);
            for (int i = 0; i < count; i++)
            {
                writer(this, list[i]);
            }
        }
        #endregion
    }
}
