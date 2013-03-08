using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace KeenSoftwareHouse.Library.Cloning
{
    /// <summary>
    /// Used to duplicate objects either by the ISerializable interface or by field-level duplication.
    /// </summary>
    public class SerializerClone : MemberwiseClone
    {
        readonly IDictionary<Type, MemberInfo[]> fieldsCache;
        readonly IList<IDeserializationCallback> callbacks;
        static readonly StreamingContext sCtx = new StreamingContext(StreamingContextStates.Persistence);

        /// <summary>
        /// Used to duplicate objects either by the ISerializable interface or by field-level duplication.
        /// </summary>
        public SerializerClone()
        {
            fieldsCache = new Dictionary<Type, MemberInfo[]>();
            callbacks = new List<IDeserializationCallback>();
        }

        /// <summary>
        /// Public entry point to begin duplication of the object graph.  If your using this instance multiple
        /// times you should call Clear() between the object graphs or the copies previously made will be used.
        /// </summary>
        public override T Clone<T>(T instance)
        {
            T result;
            try
            {
                result = base.Clone<T>(instance);

                foreach (IDeserializationCallback cb in callbacks)
                    cb.OnDeserialization(this);
            }
            finally
            {
                if (callbacks.Count > 0)
                    callbacks.Clear();
            }
            return result;
        }

        /// <summary>
        /// If the object provided is [Serializable] a simulated serialization routine is used to duplicate 
        /// the object, if it's not serializable then the MemberwiseClone base class will perform the copy.
        /// </summary>
        protected override object CloneDefault(object instance, object def)
        {
            Type type = instance.GetType();

            if (!type.IsSerializable)
                return base.CloneDefault(instance);

            object copy;
            if (instance is ISerializable)
                copy = CloneWithISerializable(instance);
            else
                copy = CloneSerializableFields(instance);

            if (copy is IDeserializationCallback)
                callbacks.Add((IDeserializationCallback)copy);

            return copy;
        }

        private object CloneSerializableFields(object instance)
        {
            object copy;
            Type type = instance.GetType();
            MemberInfo[] fields = SerializedFields(type);
            object[] values = FormatterServices.GetObjectData(instance, fields);

            copy = FormatterServices.GetUninitializedObject(type);
            Graph.Add(instance, copy);

            for (int i = 0; i < values.Length; i++)
                values[i] = this.CloneObject(values[i], null);

            FormatterServices.PopulateObjectMembers(copy, fields, values);
            return copy;
        }

        private object CloneWithISerializable(object instance)
        {
            object copy;
            Type type = instance.GetType();
            SerializationInfo sInfo = new SerializationInfo(type, new FormatterConverter());
            ((ISerializable)instance).GetObjectData(sInfo, sCtx);

            Type tcopy = Type.GetType(String.Format("{0}, {1}", sInfo.FullTypeName, sInfo.AssemblyName), true, false);

            copy = FormatterServices.GetUninitializedObject(tcopy);
            Graph.Add(instance, copy);

            SerializationInfo sCopy = new SerializationInfo(tcopy, new FormatterConverter());
            foreach (SerializationEntry se in sInfo)
                sCopy.AddValue(se.Name, this.CloneObject(se.Value, null), se.ObjectType);

            if (type == tcopy || typeof(ISerializable).IsAssignableFrom(tcopy))
            {
                ConstructorInfo ci = tcopy.GetConstructor(
                    BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
                if (ci == null)
                    throw new SerializationException("Serialization constructor not found.");
                ci.Invoke(copy, new object[] { sCopy, sCtx });
            }
            else
            {
                List<MemberInfo> fields = new List<MemberInfo>();
                List<object> values = new List<object>();

                foreach(FieldInfo fi in SerializedFields(tcopy))
                {
                    try
                    {
                        object value = sCopy.GetValue(fi.Name, fi.FieldType);
                        fields.Add(fi);
                        values.Add(value);
                    }
                    catch (SerializationException) { }
                }

                FormatterServices.PopulateObjectMembers(copy, fields.ToArray(), values.ToArray());
            }

            if (copy is IObjectReference)
            {
                copy = ((IObjectReference)copy).GetRealObject(sCtx);
                Graph[instance] = copy;
            }
            return copy;
        }

        private MemberInfo[] SerializedFields(Type t)
        {
            MemberInfo[] result;
            if (fieldsCache.TryGetValue(t, out result))
                return result;
            return fieldsCache[t] = FormatterServices.GetSerializableMembers(t);
        }
    }
}
