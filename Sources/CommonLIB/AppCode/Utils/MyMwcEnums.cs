using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;

namespace MinerWars.CommonLIB.AppCode.Utils
{
    delegate void WriteHandler<T>(BinaryWriter writer, T value) where T : struct, IConvertible;
    delegate T ReadHandler<T>(BinaryReader reader) where T : struct, IConvertible;

    public static class MyMwcEnums
    {
        class EnumInfo
        {
            public bool IsFlag;
            public HashSet<int> AllowedValues;

            public bool IsAllowed<T>(T value)
                where T : struct, IConvertible
            {
                int testValue = value.ToInt32(null);
                if (IsFlag)
                {                    
                    foreach (var val in AllowedValues)
                    {
                        testValue &= ~val;
                    }
                    return testValue == 0; // When we removed all flags, it should be zero
                }
                else
                {
                    return AllowedValues.Contains(testValue);
                }
            }
        }

        class EnumInfo<T> : EnumInfo
            where T : struct, IConvertible
        {
            public ReadHandler<T> Reader;
            public WriteHandler<T> Writer;
        }

        static Dictionary<Type, EnumInfo> m_enumValues = new Dictionary<Type, EnumInfo>();

        public static bool IsValidValue<T>(T value)
            where T : struct, IConvertible
        {
            return GetInfo<T>().IsAllowed(value);
        }

        public static HashSet<int> GetAllowedValues<T>()
            where T : struct, IConvertible
        {
            return GetInfo<T>().AllowedValues;
        }

        public static void Prepare<T>()
            where T : struct, IConvertible
        {
            GetInfo<T>();
        }

        private static EnumInfo<T> GetInfo<T>()
            where T : struct, IConvertible
        {
            var enumType = typeof(T);
            EnumInfo enumInfo;
            if (!m_enumValues.TryGetValue(enumType, out enumInfo))
            {
                enumInfo = CreateInfo<T>();
                m_enumValues[enumType] = enumInfo;
            }
            return (EnumInfo<T>)enumInfo;
        }

        public static bool ReadEnum<T>(BinaryReader reader, out T value)
            where T : struct, IConvertible
        {
            var info = GetInfo<T>();
            value = info.Reader(reader);
            return info.IsAllowed<T>(value);
        }

        public static void WriteEnum<T>(BinaryWriter writer, T value)
            where T : struct, IConvertible
        {
            GetInfo<T>().Writer(writer, value);
        }

        private static EnumInfo CreateInfo<T>()
            where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            Debug.Assert(enumType.IsEnum, "T must be Enum");
            Debug.Assert(CheckUnderlyingType(enumType), "Enum underlying type must be sbyte, short, int, byte or ushort");
            return new EnumInfo<T>()
            {
                IsFlag = Attribute.IsDefined(enumType, typeof(FlagsAttribute)),
                AllowedValues = new HashSet<int>(Enum.GetValues(enumType).OfType<T>().Select(s => s.ToInt32(null))),
                Reader = CreateReader<T>(),
                Writer = CreateWriter<T>(),
            };
        }

        private static bool CheckUnderlyingType(Type enumType)
        {
            var underlyingType = enumType.GetEnumUnderlyingType();
            return underlyingType == typeof(byte) || underlyingType == typeof(short) || underlyingType == typeof(int) || underlyingType == typeof(sbyte) || underlyingType == typeof(ushort);
        }

        private static ReadHandler<T> CreateReader<T>()
            where T : struct, IConvertible
        {
            var enumType = typeof(T);
            var underlying = enumType.GetEnumUnderlyingType();
            if (underlying == typeof(sbyte))
            {
                var caster = CreateEnumCaster<sbyte, T>();
                return new ReadHandler<T>((s) => caster(s.ReadSByte()));
            }
            else if (underlying == typeof(short))
            {
                var caster = CreateEnumCaster<short, T>();
                return new ReadHandler<T>((s) => caster(s.ReadInt16()));
            }
            else if (underlying == typeof(int))
            {
                var caster = CreateEnumCaster<int, T>();
                return new ReadHandler<T>((s) => caster(s.ReadInt32()));
            }
            else if (underlying == typeof(long))
            {
                var caster = CreateEnumCaster<long, T>();
                return new ReadHandler<T>((s) => caster(s.ReadInt64()));
            }
            else if (underlying == typeof(byte))
            {
                var caster = CreateEnumCaster<byte, T>();
                return new ReadHandler<T>((s) => caster(s.ReadByte()));
            }
            else if (underlying == typeof(ushort))
            {
                var caster = CreateEnumCaster<ushort, T>();
                return new ReadHandler<T>((s) => caster(s.ReadUInt16()));
            }
            else if (underlying == typeof(uint))
            {
                var caster = CreateEnumCaster<uint, T>();
                return new ReadHandler<T>((s) => caster(s.ReadUInt32()));
            }
            else if (underlying == typeof(ulong))
            {
                var caster = CreateEnumCaster<ulong, T>();
                return new ReadHandler<T>((s) => caster(s.ReadUInt64()));
            }
            else
            {
                throw new InvalidOperationException("Unsupported enum underlying type, use sbyte, short, int, long, byte, ushort, uint or ulong");
            }
        }

        private static WriteHandler<T> CreateWriter<T>()
            where T : struct, IConvertible
        {
            var enumType = typeof(T);
            var underlying = enumType.GetEnumUnderlyingType();
            if (underlying == typeof(sbyte))
            {
                var caster = CreateNumberCaster<sbyte, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(short))
            {
                var caster = CreateNumberCaster<short, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(int))
            {
                var caster = CreateNumberCaster<int, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(long))
            {
                var caster = CreateNumberCaster<long, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(byte))
            {
                var caster = CreateNumberCaster<byte, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(ushort))
            {
                var caster = CreateNumberCaster<ushort, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(uint))
            {
                var caster = CreateNumberCaster<uint, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else if (underlying == typeof(ulong))
            {
                var caster = CreateNumberCaster<ulong, T>();
                return new WriteHandler<T>((s, val) => s.Write(caster(val)));
            }
            else
            {
                throw new InvalidOperationException("Unsupported enum underlying type, use sbyte, short, int, long, byte, ushort, uint or ulong");
            }
        }

        private static Func<TUnderlying, TEnum> CreateEnumCaster<TUnderlying, TEnum>()
            where TEnum : struct, IConvertible
            where TUnderlying : struct
        {
            var param = Expression.Parameter(typeof(TUnderlying));
            UnaryExpression caster = Expression.Convert(param, typeof(TEnum));
            return Expression.Lambda<Func<TUnderlying, TEnum>>(caster, param).Compile();
        }

        private static Func<TEnum, TUnderlying> CreateNumberCaster<TUnderlying, TEnum>()
            where TEnum : struct, IConvertible
            where TUnderlying : struct
        {
            var param = Expression.Parameter(typeof(TEnum));
            UnaryExpression caster = Expression.Convert(param, typeof(TUnderlying));
            return Expression.Lambda<Func<TEnum, TUnderlying>>(caster, param).Compile();
        }
    }
}
