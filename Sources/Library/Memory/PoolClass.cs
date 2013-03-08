using System;

namespace KeenSoftwareHouse.Library.Memory
{
    /// <summary>
    /// Represent an object from pool that can be getted and by dispose pattern returnd back when no-needed.
    /// PoolClass use its internal pool of T or can be initialized with custom: 
    ///      PoolClass<T>.Init(externalPool);
    /// <remarks>
    /// Always follows this pattern:
    ///     using (var wrapper = PoolClass<T>.New(p0, p1, ...))
    ///     {
    ///         T item = wrapper;
    /// 
    ///         item.DoSomeWork()
    ///     }
    /// 
    /// This will ensure fastest immediate realease back to pool. 
    /// </remarks>
    /// </summary>
    /// <typeparam name="T">Class</typeparam>
    public static class PoolClass<T> where T: class
    {
        #region Constants

        /// <summary>
        /// Maximum concrete number of pooled objects.
        /// </summary>
        private const int MaximumNumClass = 32;

        #endregion

        #region Fields

        /// <summary>
        /// Multi-threaded list pool
        /// </summary>
        private static Pool<T> ListPool;

        #endregion

        #region Methods

        /// <summary>
        /// Inits the specified external pool.
        /// </summary>
        /// <param name="externalPool">The external pool.</param>
        public static void Init(Pool<T> externalPool = null)
        {
            if (ListPool != null)
            {
                return;
            }

            ListPool = externalPool ?? new Pool<T>(MaximumNumClass);
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <returns></returns>
        public static ObjectDisposer<T> New()
        {
            Init();

            var item = ListPool.New();

            return new ObjectDisposer<T>(item, ReturnMode.Delete);
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <returns>New object.</returns>
        public static ObjectDisposer<T> New<T0>(T0 p0)
        {
            Init();

            var item = ListPool.New(p0);

            return new ObjectDisposer<T>(item, ReturnMode.Delete);
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <returns>New object.</returns>
        public static ObjectDisposer<T> New<T0, T1>(T0 p0, T1 p1)
        {
            Init();

            var item = ListPool.New(p0, p1);

            return new ObjectDisposer<T>(item, ReturnMode.Delete);
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>New object.</returns>
        public static ObjectDisposer<T> New<T0, T1, T2>(T0 p0, T1 p1, T2 p2)
        {
            Init();

            var item = ListPool.New(p0, p1, p2);

            return new ObjectDisposer<T>(item, ReturnMode.Delete);
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns>New object.</returns>
        public static ObjectDisposer<T> New<T0, T1, T2, T3>(T0 p0, T1 p1, T2 p2, T3 p3)
        {
            Init();

            var item = ListPool.New(p0, p1, p2, p3);

            return new ObjectDisposer<T>(item, ReturnMode.Delete);
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <returns>New object.</returns>
        public static ObjectDisposer<T> New<T0, T1, T2, T3, T4>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Init();

            var item = ListPool.New(p0, p1, p2, p3, p4);

            return new ObjectDisposer<T>(item, ReturnMode.Delete);
        }

        /// <summary>
        /// Release object back to pool and allows all data in it to be reclaimed by GC.
        /// </summary>
        /// <param name="item">The item.</param>
        public static void Delete(T item)
        {
            ListPool.Delete(item);
        }

        /// <summary>
        /// Acquires object.
        /// </summary>
        /// <returns></returns>
        public static ObjectDisposer<T> Acquire()
        {
            Init();

            var item = ListPool.Acquire();

            return new ObjectDisposer<T>(item, ReturnMode.Release);
        }

        /// <summary>
        /// Releases the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public static void Release(T item)
        {
            ListPool.Release(item);
        }

        #endregion
    }

    /// <summary>
    /// Pool return mode.
    /// </summary>
    internal enum ReturnMode : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Delete,

        /// <summary>
        /// 
        /// </summary>
        Release
    };

    /// <summary>
    /// ObjectDisposer is wrapper around object from pool and on dispose release it back to pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ObjectDisposer<T> : IDisposable where T : class
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly T item;

        /// <summary>
        /// 
        /// </summary>
        private readonly ReturnMode returnMode;
        
        #endregion

        #region Operators

        /// <summary>
        /// Implicit conversion to wrapped type.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(ObjectDisposer<T> right)
        {
            return right.item;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="returnMode">The return mode.</param>
        internal ObjectDisposer(T item, ReturnMode returnMode)
        {
            this.item = item;
            this.returnMode = returnMode;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            switch (this.returnMode)
            {
                case ReturnMode.Delete:
                    {
                        PoolClass<T>.Delete(this.item);

                        break;
                    }
                case ReturnMode.Release:
                    {
                        PoolClass<T>.Release(this.item);

                        break;
                    }
            }
        }

        #endregion
    }
}