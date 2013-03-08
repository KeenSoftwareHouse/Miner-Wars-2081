using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KeenSoftwareHouse.Library.Collections
{
    /// <summary>
    /// IndexedLinkedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class IndexedLinkedList<T>
    {
        readonly LinkedList<T> data = new LinkedList<T>();
        readonly Dictionary<T, LinkedListNode<T>> index = new Dictionary<T, LinkedListNode<T>>();

        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        public T First
        {
            get
            {
                return data.First.Value;
            }
        }

        public void Add(T value)
        {
            index[value] = data.AddLast(value);
        }

        public void RemoveFirst()
        {
            index.Remove(data.First.Value);
            data.RemoveFirst();
        }

        public LinkedListNode<T> Remove(T value)
        {
            LinkedListNode<T> node;
            if (index.TryGetValue(value, out node))
            {
                data.Remove(node);
                index.Remove(value);

                return node;
            }

            return null;
        }

        public void Clear()
        {
            data.Clear();
            index.Clear();
        }

        public void Promote(T value)
        {
            LinkedListNode<T> node;
            if (index.TryGetValue(value, out node))
            {
                data.Remove(node);
                data.AddLast(node);
            }
        }
    }

    /// <summary>
    /// Least recently used cache of objects.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class LRUCache<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<TKey, TValue> data;
        
        /// <summary>
        /// 
        /// </summary>
        private readonly IndexedLinkedList<TKey> lruList = new IndexedLinkedList<TKey>();
       
        /// <summary>
        /// 
        /// </summary>
        private readonly ICollection<KeyValuePair<TKey, TValue>> dataAsCollection;
        
        /// <summary>
        /// 
        /// </summary>
        private readonly int capacity;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return data.Keys;
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return data.Values; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { return data.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LRUCache&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public LRUCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("capacity should always be bigger than 0");
            }

            this.data = new Dictionary<TKey, TValue>(capacity);
            this.dataAsCollection = data;
            this.capacity = capacity;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                throw new ArgumentException("An attempt was made to insert a duplicate key in the cache.");
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            bool existed = data.Remove(key);
            lruList.Remove(key);
            return existed;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {

            bool removed = dataAsCollection.Remove(item);
            if (removed)
            {
                lruList.Remove(item.Key);
            }
            return removed;
        }

        /// <summary>
        /// Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                var value = data[key];
                
                lruList.Promote(key);

                return value;
            }
            set
            {
                data[key] = value;
                lruList.Remove(key);
                lruList.Add(key);

                if (data.Count > capacity)
                {
                    data.Remove(lruList.First);
                    lruList.RemoveFirst();
                }
            }
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            bool success = data.TryGetValue(key, out value);

            if (success)
            {
                lruList.Promote(key); 
            }

            return success;
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return data.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dataAsCollection.Contains(item);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            data.Clear();
            lruList.Clear();
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dataAsCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dataAsCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)data).GetEnumerator();
        }

        #endregion
    }
}
