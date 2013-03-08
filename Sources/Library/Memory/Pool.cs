using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Linq.Expressions;

namespace KeenSoftwareHouse.Library.Memory
{    
    using Debugging;
    using Reflection;

    /// <summary>
    /// Loading mode.
    /// </summary>
    public enum LoadingMode
    {
        /// <summary>
        /// Precreate pooled object.
        /// </summary>
        Eager, 
        /// <summary>
        /// Object will be created on-demand.
        /// </summary>
        Lazy, 
        /// <summary>
        /// 
        /// </summary>
        LazyExpanding
    };

    /// <summary>
    /// Access mode of pool.
    /// </summary>
    public enum AccessMode
    {
        /// <summary>
        /// FIFO pool.
        /// </summary>
        FIFO, 
        /// <summary>
        /// LIFO pool.
        /// </summary>
        LIFO, 
        /// <summary>
        /// Circular pool.
        /// </summary>
        Circular
    };

    /// <summary>
    /// Represent a pool of objects.
    /// Support Acquire/Release and also New/Delete pattern.
    /// </summary>
    /// <typeparam name="T">Type of pooled objects.</typeparam>
    [DebuggerStepThrough]
    public class Pool<T> : IDisposable
    {
        #region Constatns

        /// <summary>
        /// Define maximum number of constructor parameters that are supported by this pool.
        /// </summary>
        public const int MaximumConstructorParams = 5;

        #endregion

        #region Fields

        /// <summary>
        /// Factory function.
        /// </summary>
        private readonly Func<Pool<T>, T> factory;

        /// <summary>
        /// Internal iterm store.
        /// </summary>
        private readonly IItemStore itemStore;

        /// <summary>
        /// Pool size.
        /// </summary>
        private readonly int size;

        /// <summary>
        /// Actual count of created objects.
        /// </summary>
        private int count;

        /// <summary>
        /// Sync primitive.
        /// </summary>
        private readonly Semaphore sync;

        /// <summary>
        /// Contructors used for objects reinicialization.
        /// </summary>
        private static Delegate[] Constructors;

        /// <summary>
        /// Destructor that is used for zero object references.
        /// </summary>
        private static Action<T> Destructor;

        #endregion

        #region Properties

        /// <summary>
        /// Loading mode.
        /// </summary>
        public LoadingMode LoadingMode 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Returns true if pool was already disposed.
        /// </summary>
        public bool IsDisposed 
        { 
            get; 
            private set; 
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes a new instance of the Pool class.
        /// </summary>
        /// <param name="size">Maximum pool size.</param>
        /// <param name="factory">Factory function(creator) for pooled objects.</param>
        /// <param name="loadingMode">Loading mode.</param>
        /// <param name="accessMode">Acess mode</param>
        public Pool(int size, Func<Pool<T>, T> factory = null, LoadingMode loadingMode = LoadingMode.Lazy, AccessMode accessMode = AccessMode.LIFO)
        {
            Exceptions.ThrowIf<ArgumentOutOfRangeException>(size == 0, "Argument 'size' must be greater than zero.");

            this.size = size;
            this.factory = factory;
            this.sync = new Semaphore(size, size);
            this.LoadingMode = loadingMode;

            // Create pool
            switch (accessMode)
            {
                case AccessMode.FIFO:
                    {
                        this.itemStore = new QueueStore(size);
                        break;
                    }
                case AccessMode.LIFO:
                    {
                        this.itemStore = new StackStore(size);
                        break;
                    }
                case AccessMode.Circular:
                    {
                        this.itemStore = new CircularStore(size);
                        break;
                    }
            }

            // Default factory
            if (this.factory == null)
            {
                this.factory = p => (T)Activator.CreateInstance(typeof(T), true);
            }

            // Preaload
            if (loadingMode == LoadingMode.Eager)
            {
                PreloadItems();
            }
        }

        /// <summary>
        /// Acquire object from pool but does not initialize it.
        /// </summary>
        /// <returns>New object.</returns>
        public T Acquire()
        {
            sync.WaitOne();
            switch (LoadingMode)
            {
                case LoadingMode.Eager:
                    {
                        return AcquireEager();
                    }
                case LoadingMode.Lazy:
                    {
                        return AcquireLazy();
                    }
                default:
                    {
                        Debug.Assert(LoadingMode == LoadingMode.LazyExpanding,
                                     "Unknown LoadingMode encountered in Acquire method.");
                        return AcquireLazyExpanding();
                    }
            }
        }

        /// <summary>
        /// Release object back to pool.
        /// </summary>
        /// <param name="item">Object to release.</param>
        public void Release(T item)
        {
            lock (itemStore)
            {
                itemStore.Store(item);
            }
            sync.Release();
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <returns></returns>
        public T New()
        {
            T obj = Acquire();

            var constructor = (Action<T>)GetConstructor(0, typeof(Action<T>));

            constructor(obj);

            return obj;
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <returns>New object.</returns>
        public T New<T0>(T0 p0)
        {
            T obj = Acquire();

            var constructor = (Action<T, T0>)GetConstructor(1, typeof(Action<T, T0>));

            constructor(obj, p0);

            return obj;
        }

        /// <summary>
        /// Acquires object from pool and call constructor.
        /// </summary>
        /// <typeparam name="T0">The type of the 0.</typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <returns>New object.</returns>
        public T New<T0, T1>(T0 p0, T1 p1)
        {
            T obj = Acquire();

            var constructor = (Action<T, T0, T1>)GetConstructor(2, typeof(Action<T, T0, T1>));

            constructor(obj, p0, p1);

            return obj;
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
        public T New<T0, T1, T2>(T0 p0, T1 p1, T2 p2)
        {
            T obj = Acquire();

            var constructor = (Action<T, T0, T1, T2>)GetConstructor(3, typeof(Action<T, T0, T1, T2>));

            constructor(obj, p0, p1, p2);

            return obj;
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
        public T New<T0, T1, T2, T3>(T0 p0, T1 p1, T2 p2, T3 p3)
        {
            T obj = Acquire();

            var constructor = (Action<T, T0, T1, T2, T3>)GetConstructor(4, typeof(Action<T, T0, T1, T2, T3>));

            constructor(obj, p0, p1, p2, p3);

            return obj;
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
        public T New<T0, T1, T2, T3, T4>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            T obj = Acquire();

            var constructor = (Action<T, T0, T1, T2, T3, T4>)GetConstructor(5, typeof(Action<T, T0, T1, T2, T3, T4>));

            constructor(obj, p0, p1, p2, p3, p4);

            return obj;
        }

        /// <summary>
        /// Release object back to pool and allows all data in it to be reclaimed by GC.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(T item)
        {
            var destructor = GetDestructor();

            destructor(item);

            Release(item);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                lock (itemStore)
                {
                    while (itemStore.Count > 0)
                    {
                        var disposable = (IDisposable)itemStore.Fetch();
                        disposable.Dispose();
                    }
                }
            }

            sync.Close();
        } 
        
        #endregion

        #region Private Methods

        private T AcquireEager()
        {
            lock (itemStore)
            {
                return itemStore.Fetch();
            }
        }

        private T AcquireLazy()
        {
            lock (itemStore)
            {
                if (itemStore.Count > 0)
                {
                    return itemStore.Fetch();
                }
            }

            Interlocked.Increment(ref count);
            return factory(this);
        }

        private T AcquireLazyExpanding()
        {
            bool shouldExpand = false;
            if (count < size)
            {
                int newCount = Interlocked.Increment(ref count);
                if (newCount <= size)
                {
                    shouldExpand = true;
                }
                else
                {
                    // Another thread took the last spot - use the store instead
                    Interlocked.Decrement(ref count);
                }
            }

            if (shouldExpand)
            {
                return factory(this);
            }

            lock (itemStore)
            {
                return itemStore.Fetch();
            }
        }

        /// <summary>
        /// Get or creates in place constructor delegate.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="inplaceCtorType"></param>
        /// <returns></returns>
        private static Delegate GetConstructor(int i, Type inplaceCtorType)
        {
            Debug.Assert(i < MaximumConstructorParams);

            if (Constructors == null)
            {
                Constructors = new Delegate[MaximumConstructorParams];
            }

            return Constructors[i] ?? (Constructors[i] = ConstructorHelper<T>.CreateInPlaceConstructor(inplaceCtorType));
        }

        /// <summary>
        /// Gets or creates zeroing destructor action.
        /// </summary>
        /// <returns></returns>
        private static Action<T> GetDestructor()
        {
            if (Destructor == null)
            {
                // Param1
                var objExp = Expression.Parameter(typeof (T), "obj");

                // Assignment block
                var blockExp = Expression.Block(from field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                            select Expression.Assign(Expression.Field(objExp, field), 
                                                                     Expression.Default(field.FieldType)));

                Destructor = Expression.Lambda<Action<T>>(blockExp, objExp).Compile();
            }

            return Destructor;
        }

        private void PreloadItems()
        {
            for (int i = 0; i < size; i++)
            {
                T item = factory(this);
                itemStore.Store(item);
            }

            count = size;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// Item sorage interface.
        /// </summary>
        interface IItemStore
        {
            T Fetch();
            void Store(T item);
            int Count { get; }
        }

        /// <summary>
        /// FIFO pool wrapper.
        /// </summary>
        [DebuggerStepThrough]
        class QueueStore : Queue<T>, IItemStore
        {
            public QueueStore(int capacity)
                : base(capacity)
            {
            }

            public T Fetch()
            {
                return Dequeue();
            }

            public void Store(T item)
            {
                Enqueue(item);
            }
        }

        /// <summary>
        /// LIFO pool wrapper.
        /// </summary>
        [DebuggerStepThrough]
        class StackStore : Stack<T>, IItemStore
        {
            public StackStore(int capacity)
                : base(capacity)
            {
            }

            public T Fetch()
            {
                return Pop();
            }

            public void Store(T item)
            {
                Push(item);
            }
        }

        /// <summary>
        /// Cicrcular pool wraper
        /// </summary>
        [DebuggerStepThrough]
        class CircularStore : IItemStore
        {
            private readonly List<Slot> slots;
            private int freeSlotCount;
            private int position = -1;

            public int Count
            {
                get
                {
                    return this.freeSlotCount;
                }
            }

            #region Methods

            public CircularStore(int capacity)
            {
                this.slots = new List<Slot>(capacity);
            }

            public T Fetch()
            {
                Exceptions.ThrowIf<InvalidOperationException>(this.Count == 0, "The buffer is empty.");

                int startPosition = this.position;

                do
                {
                    Advance();
                    Slot slot = this.slots[this.position];

                    if (!slot.IsInUse)
                    {
                        slot.IsInUse = true;
                        --this.freeSlotCount;
                        return slot.Item;
                    }

                } while (startPosition != position);

                throw new InvalidOperationException("No free slots.");
            }

            public void Store(T item)
            {
                Slot slot = this.slots.Find(s => Equals(s.Item, item));

                if (slot == null)
                {
                    slot = new Slot(item);
                    this.slots.Add(slot);
                }

                slot.IsInUse = false;
                ++this.freeSlotCount;
            }

            private void Advance()
            {
                position = (position + 1)%this.slots.Count;
            }

            private class Slot
            {
                public Slot(T item)
                {
                    this.Item = item;
                }

                public T Item { get; private set; }
                public bool IsInUse { get; set; }
            }

            #endregion
        }

        #endregion
    }
}