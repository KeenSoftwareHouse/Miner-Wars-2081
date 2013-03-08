using System;
using System.Collections.Generic;

namespace KeenSoftwareHouse.Library.Cloning
{
    /// <summary>
    /// A class that performs duplication of an entire object graph
    /// </summary>
    public abstract class ObjectCloner : IDisposable
    {
        readonly IDictionary<object, object> graph;

        /// <summary>
        /// Creates the cloner
        /// </summary>
        protected ObjectCloner()
        {
            graph = new Dictionary<object, object>(new ReferenceEqualityComparer());
        }

        /// <summary>
        /// Disposes of the instance and it's references to objects that have been duplicated
        /// </summary>
        public void Dispose() { Clear(); }
        /// <summary>
        /// Removes all instances from the object graph
        /// </summary>
        public void Clear() { graph.Clear(); }
        /// <summary>
        /// Add or Remove instances from the object graph, by adding this.Graph[o] = o; the instance 'o' will 
        /// not be duplicated.
        /// </summary>
        public IDictionary<object, object> Graph { [System.Diagnostics.DebuggerStepThrough] get { return graph; } }

        /// <summary>
        /// Public entry point to begin duplication of the object graph.
        /// </summary>
        public virtual T Clone<T>(T instance)
        {
            try
            {
                return CloneObject<T>(instance, default(T));
            }
            finally { Clear(); }
        }

        /// <summary>
        /// Internal duplicate an object graph
        /// </summary>
        protected T CloneObject<T>(T instance, T original)
        {
            object copy;

            if (ReferenceEquals(instance, null) || instance is string || instance is DateTime ||
                instance is DateTimeOffset || instance is TimeSpan || instance is Boolean || instance is Byte ||
                instance is SByte || instance is Int16 || instance is UInt16 || instance is Int32 ||
                instance is UInt32 || instance is Int64 || instance is UInt64 || instance is IntPtr ||
                instance is Char || instance is Double || instance is Single || instance is MarshalByRefObject)
                return instance;

            if (graph.TryGetValue(instance, out copy))
            {
                return (T) copy;
            }

            if (instance is Array)
            {
                return (T) (object) CloneArray((Array) (object) instance);
            }

            if (instance is Delegate)
            {
                return (T) (object) CloneDelegate((Delegate) (object) instance);
            }

            return (T)CloneDefault(instance, original);
        }

        /// <summary>
        /// Provides the default behavior for duplicating an object and recording the
        /// duplication into the graph.
        /// </summary>
        /// <param name="inst">The inst.</param>
        /// <param name="def">The default instance value of it, if not null default instance is used insted of creating new one.</param>
        /// <returns></returns>
        protected abstract object CloneDefault(object inst, object def = null);

        private Array CloneArray(Array instance)
        {
            Array copy = (Array)((ICloneable)instance).Clone();
            graph.Add(instance, copy);

            int[] indexes = new int[copy.Rank];
            CloneArray(copy, indexes, 0);
            return copy;
        }

        private void CloneArray(Array copy, int[] indexes, int rank)
        {
            int stop = copy.GetUpperBound(rank);
            for (indexes[rank] = copy.GetLowerBound(rank); indexes[rank] <= stop; indexes[rank]++)
            {
                if ((rank + 1) < copy.Rank)
                    CloneArray(copy, indexes, rank + 1);
                else
                    copy.SetValue(this.CloneObject(copy.GetValue(indexes), null), indexes);
            }
        }

        private Delegate CloneDelegate(Delegate method)
        {
            Delegate result = null;
            foreach (Delegate del in method.GetInvocationList())
            {
                Delegate delCopy = Delegate.CreateDelegate(del.GetType(), CloneObject(del.Target, null), del.Method, true);
                result = Delegate.Combine(result, delCopy);
            }

            graph[method] = result;
            return result;
        }
    }
}
