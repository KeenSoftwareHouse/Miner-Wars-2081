using System;
using System.Collections.Generic;

namespace KeenSoftwareHouse.Library.Memory
{
    /// <summary>
    /// Represent list that is pooled in pool and can be used as temporary.
    /// <remarks>
    /// Always follow this pattern:
    ///     using (var list = PoolList.Get())
    ///     {
    ///         // Do some work with list...
    ///     }
    /// 
    /// This will ensure fastest allocation and immediate realease back to pool. 
    ///  </remarks>
    /// </summary>
    /// <typeparam name="T">Type of list elements.</typeparam>
    public class PoolList<T>: List<T>, IDisposable
    {
        #region Constants

        /// <summary>
        /// Maximum concrete number of pooled lists per type.
        /// </summary>
        private const int MaximumNumLists = 32;

        #endregion

        #region Fields

        /// <summary>
        /// Indicates if finalizer is registered for this instance.
        /// </summary>
        private bool finalizerRegistered;

        /// <summary>
        /// Multi-threaded list pool
        /// </summary>
        private static readonly Pool<PoolList<T>> ListPool;

        #endregion

        #region Methods

        /// <summary>
        /// Cunstruct static pool of lists.
        /// </summary>
        static PoolList()
        {
            ListPool = new Pool<PoolList<T>>(MaximumNumLists);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that is empty and has the default initial capacity.
        /// </summary>
        public PoolList()
        {
            this.finalizerRegistered = true;
        }

        /// <summary>
        /// Destructor that enforce releasing poollist back to the pool.
        /// </summary>
        ~PoolList()
        {
            Dispose(false);
        }

        /// <summary>
        /// Returns empty list for temporary work.
        /// Can be called multiple time up to maximum number allowed lists per type.
        /// </summary>
        /// <returns></returns>
        public static PoolList<T> Get()
        {
            var list = ListPool.Acquire();

            if (!list.finalizerRegistered)
            {
                GC.ReRegisterForFinalize(list);
                list.finalizerRegistered = true;
            }

            return list;
        }

        /// <summary>
        /// Return poollist back to pool.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // No managed resources
            }

            this.Clear();
            ListPool.Release(this);
        }

        #endregion

        /// <summary>
        /// Dispose pool list.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);        
            GC.SuppressFinalize(this);

            this.finalizerRegistered = false;
        }    
    }
}