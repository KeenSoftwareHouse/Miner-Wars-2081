using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace KeenSoftwareHouse.Library.Cloning
{
    /// <summary>
    /// Provides a deep-copy, field-level duplication of any object
    /// </summary>
    public class MemberwiseClone : ObjectCloner
    {
        readonly IDictionary<Type, MemberInfo[]> fieldsCache;

        /// <summary>
        /// Provides a deep-copy, field-level duplication of any object
        /// </summary>
        public MemberwiseClone()
        {
            fieldsCache = new Dictionary<Type, MemberInfo[]>();
        }

        /// <summary>
        /// Routine to clone an objects fields and their contents
        /// </summary>
        protected override object CloneDefault(object instance, object def = null)
        {
            Type type = instance.GetType();
            object copy = FormatterServices.GetUninitializedObject(type);
            Graph.Add(instance, copy);

            MemberInfo[] fields = ClonableFields(type);
            object[] values = FormatterServices.GetObjectData(instance, fields);

            for (int i = 0; i < values.Length; i++)
                values[i] = this.CloneObject(values[i], null);

            FormatterServices.PopulateObjectMembers(copy, fields, values);
            return copy;
        }

        protected virtual MemberInfo[] ClonableFields(Type type)
        {
            MemberInfo[] result;
            if (fieldsCache.TryGetValue(type, out result))
                return result;

            List<MemberInfo> fields = new List<MemberInfo>();
            Type t = type;
            while (t != null && t != typeof(Object))
            {
                fields.AddRange(
                    t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                t = t.BaseType;
            }

            return fieldsCache[type] = fields.ToArray();
        }
    }
}
