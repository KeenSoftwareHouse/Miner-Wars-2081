/*
 * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
 *
 * Bullet Continuous Collision Detection and Physics Library
 * Copyright (c) 2003-2008 Erwin Coumans  http://www.bulletphysics.com/
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose, 
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace BulletXNA.LinearMath
{
	//This is an implementation of the ObjectArray class which attempts to grow itself to a certain size if it's indexed out of bounds?
	public class ObjectArray<T> : IList<T>,
	ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable where T:new()
	{

		// Fields
		private const int _defaultCapacity = 4;
		private static T[] _emptyArray;
		private T[] _items;
		private int _size;
		private object _syncRoot;
		private int _version;

		// Methods
		static ObjectArray()
		{
			ObjectArray<T>._emptyArray = new T[0];
		}

		public ObjectArray()
		{
			this._items = ObjectArray<T>._emptyArray;
		}

        public T[] GetRawArray()
        {
            return _items;
        }

		public ObjectArray(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection");
			}
			ICollection<T> is2 = collection as ICollection<T>;
			if (is2 != null)
			{
				int count = is2.Count;
				this._items = new T[count];
				is2.CopyTo(this._items, 0);
				this._size = count;
			}
			else
			{
				this._size = 0;
				this._items = new T[4];
				using (IEnumerator<T> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						this.Add(enumerator.Current);
					}
				}
			}
		}

		public ObjectArray(int capacity)
		{
			if (capacity < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_SmallCapacity");
			}
			this._items = new T[capacity];
		}

		public void Add(T item)
		{
			if (this._size == this._items.Length)
			{
				this.EnsureCapacity(this._size + 1);
			}
			this._items[this._size++] = item;
			this._version++;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			this.InsertRange(this._size, collection);
		}

		public ReadOnlyCollection<T> AsReadOnly()
		{
			return new ReadOnlyCollection<T>(this);
		}

        public void Swap(int index0, int index1)
        {
            T temp = _items[index0];
            _items[index0] = _items[index1];
            _items[index1] = temp;
        }

        public void Resize(int newsize)
        {
            Resize(newsize, true);
        }

        public void	Resize(int newsize,bool allocate)
		{
			int curSize = Count;

			if (newsize < curSize)
			{
                if (allocate)
                {
                    for (int i = newsize; i < curSize; i++)
                    {
                        this._items[i] = new T();
                    }
                }
                else
                {
                    for (int i = newsize; i < curSize; i++)
                    {
                        this._items[i] = default(T);
                    }
                }
			} else
			{
				if (newsize > Count)
				{
					Capacity = newsize;
				}
                if(allocate)
                {
				    for (int i=curSize;i<newsize;i++)
				    {
					    this._items[i] = new T();
				    }
                }

			}

            this._size = newsize;
		}
	


		public int BinarySearch(T item)
		{
			return this.BinarySearch(0, this.Count, item, null);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return this.BinarySearch(0, this.Count, item, comparer);
		}

		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			if ((index < 0) || (count < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - index) < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			return Array.BinarySearch<T>(this._items, index, count, item, comparer);
		}

		public void Clear()
		{
			if (this._size > 0)
			{
				Array.Clear(this._items, 0, this._size);
				this._size = 0;
			}
			this._version++;
		}

		public bool Contains(T item)
		{
			if (item == null)
			{
				for (int j = 0; j < this._size; j++)
				{
					if (this._items[j] == null)
					{
						return true;
					}
				}
				return false;
			}
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < this._size; i++)
			{
				if (comparer.Equals(this._items[i], item))
				{
					return true;
				}
			}
			return false;
		}

		public ObjectArray<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) where TOutput : new()
		{
			if (converter == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter");
			}
			ObjectArray<TOutput> list = new ObjectArray<TOutput>(this._size);
			for (int i = 0; i < this._size; i++)
			{
				list._items[i] = converter(this._items[i]);
			}
			list._size = this._size;
			return list;
		}

		public void CopyTo(T[] array)
		{
			this.CopyTo(array, 0);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this._items, 0, array, arrayIndex, this._size);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			if ((this._size - index) < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			Array.Copy(this._items, index, array, arrayIndex, count);
		}

		private void EnsureCapacity(int min)
		{
			if (this._items.Length < min)
			{
				int num = (this._items.Length == 0) ? 4 : (this._items.Length * 2);
				if (num < min)
				{
					num = min;
				}
				this.Capacity = num;
			}
		}

		public bool Exists(Predicate<T> match)
		{
			return (this.FindIndex(match) != -1);
		}

		public T Find(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = 0; i < this._size; i++)
			{
				if (match(this._items[i]))
				{
					return this._items[i];
				}
			}
			return default(T);
		}

		public ObjectArray<T> FindAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			ObjectArray<T> list = new ObjectArray<T>();
			for (int i = 0; i < this._size; i++)
			{
				if (match(this._items[i]))
				{
					list.Add(this._items[i]);
				}
			}
			return list;
		}

		public int FindIndex(Predicate<T> match)
		{
			return this.FindIndex(0, this._size, match);
		}

		public int FindIndex(int startIndex, Predicate<T> match)
		{
			return this.FindIndex(startIndex, this._size - startIndex, match);
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			if (startIndex > this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index");
			}
			if ((count < 0) || (startIndex > (this._size - count)))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count");
			}
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (match(this._items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T FindLast(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = this._size - 1; i >= 0; i--)
			{
				if (match(this._items[i]))
				{
					return this._items[i];
				}
			}
			return default(T);
		}

		public int FindLastIndex(Predicate<T> match)
		{
			return this.FindLastIndex(this._size - 1, this._size, match);
		}

		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			return this.FindLastIndex(startIndex, startIndex + 1, match);
		}

		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			if (this._size == 0)
			{
				if (startIndex != -1)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index");
				}
			}
			else if (startIndex >= this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index");
			}
			if ((count < 0) || (((startIndex - count) + 1) < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count");
			}
			int num = startIndex - count;
			for (int i = startIndex; i > num; i--)
			{
				if (match(this._items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public void ForEach(Action<T> action)
		{
			if (action == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = 0; i < this._size; i++)
			{
				action(this._items[i]);
			}
		}

		//public Enumerator<T> GetEnumerator()
		//{
		//    return new Enumerator<T>((ObjectArray<T>)this);
		//}

		public ObjectArray<T> GetRange(int index, int count)
		{
			if ((index < 0) || (count < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - index) < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			ObjectArray<T> list = new ObjectArray<T>(count);
			Array.Copy(this._items, index, list._items, 0, count);
			list._size = count;
			return list;
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf<T>(this._items, item, 0, this._size);
		}

		public int IndexOf(T item, int index)
		{
			if (index > this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			return Array.IndexOf<T>(this._items, item, index, this._size - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if (index > this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			if ((count < 0) || (index > (this._size - count)))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count");
			}
			return Array.IndexOf<T>(this._items, item, index, count);
		}

		public void Insert(int index, T item)
		{
			if (index > this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert");
			}
			if (this._size == this._items.Length)
			{
				this.EnsureCapacity(this._size + 1);
			}
			if (index < this._size)
			{
				Array.Copy(this._items, index, this._items, index + 1, this._size - index);
			}
			this._items[index] = item;
			this._size++;
			this._version++;
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection");
			}
			if (index > this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			ICollection<T> is2 = collection as ICollection<T>;
			if (is2 != null)
			{
				int count = is2.Count;
				if (count > 0)
				{
					this.EnsureCapacity(this._size + count);
					if (index < this._size)
					{
						Array.Copy(this._items, index, this._items, index + count, this._size - index);
					}
					if (this == is2)
					{
						Array.Copy(this._items, 0, this._items, index, index);
						Array.Copy(this._items, (int)(index + count), this._items, (int)(index * 2), (int)(this._size - index));
					}
					else
					{
						T[] array = new T[count];
						is2.CopyTo(array, 0);
						array.CopyTo(this._items, index);
					}
					this._size += count;
				}
			}
			else
			{
				using (IEnumerator<T> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						this.Insert(index++, enumerator.Current);
					}
				}
			}
			this._version++;
		}

		private static bool IsCompatibleObject(object value)
		{
			if (!(value is T) && ((value != null) || typeof(T).IsValueType))
			{
				return false;
			}
			return true;
		}

		public int LastIndexOf(T item)
		{
			return this.LastIndexOf(item, this._size - 1, this._size);
		}

		public int LastIndexOf(T item, int index)
		{
			if (index >= this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index");
			}
			return this.LastIndexOf(item, index, index + 1);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			if (this._size == 0)
			{
				return -1;
			}
			if ((index < 0) || (count < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((index >= this._size) || (count > (index + 1)))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index >= this._size) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection");
			}
			return Array.LastIndexOf<T>(this._items, item, index, count);
		}

        // Remove the item if it exits, order of the data isn't preserved and last item is copied into it's place.
        public bool RemoveQuick(T item)
        {
			int index = this.IndexOf(item);
			if (index >= 0)
			{
                if (_size > 0)
                {
                    // copy the last item to this position
                    this._items[index] = this._items[_size - 1];
                }
                --_size;
                return true;
            }
			return false;
		}

        public bool RemoveAtQuick(int  index)
        {
            if (index >= 0)
            {
                if (_size > 0)
                {
                    // copy the last item to this position
                    this._items[index] = this._items[_size - 1];
                }
                --_size;
                return true;
            }
            return false;
        }

		public bool Remove(T item)
		{
			int index = this.IndexOf(item);
			if (index >= 0)
			{
				this.RemoveAt(index);
				return true;
			}
			return false;
		}

		public int RemoveAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			int index = 0;
			while ((index < this._size) && !match(this._items[index]))
			{
				index++;
			}
			if (index >= this._size)
			{
				return 0;
			}
			int num2 = index + 1;
			while (num2 < this._size)
			{
				while ((num2 < this._size) && match(this._items[num2]))
				{
					num2++;
				}
				if (num2 < this._size)
				{
					this._items[index++] = this._items[num2++];
				}
			}
			Array.Clear(this._items, index, this._size - index);
			int num3 = this._size - index;
			this._size = index;
			this._version++;
			return num3;
		}

		public void RemoveAt(int index)
		{
			if (index >= this._size)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException");
			}
			this._size--;
			if (index < this._size)
			{
				Array.Copy(this._items, index + 1, this._items, index, this._size - index);
			}
			this._items[this._size] = default(T);
			this._version++;
		}

		public void RemoveRange(int index, int count)
		{
			if ((index < 0) || (count < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - index) < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			if (count > 0)
			{
				this._size -= count;
				if (index < this._size)
				{
					Array.Copy(this._items, index + count, this._items, index, this._size - index);
				}
				Array.Clear(this._items, this._size, count);
				this._version++;
			}
		}

		public void Reverse()
		{
			this.Reverse(0, this.Count);
		}

		public void Reverse(int index, int count)
		{
			if ((index < 0) || (count < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - index) < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			Array.Reverse(this._items, index, count);
			this._version++;
		}

		public void Sort()
		{
			this.Sort(0, this.Count, null);
		}

		public void Sort(IComparer<T> comparer)
		{
			this.Sort(0, this.Count, comparer);
		}

		public void Sort(Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			if (this._size > 0)
			{
				IComparer<T> comparer = new ObjectArray<T>.FunctorComparer(comparison);
				Array.Sort<T>(this._items, 0, this._size, comparer);
			}
		}

		public void Sort(int index, int count, IComparer<T> comparer)
		{
			if ((index < 0) || (count < 0))
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException((index < 0) ? ExceptionArgument.index : ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum");
			}
			if ((this._size - index) < count)
			{
				throw new Exception("ExceptionResource  - Offlen");
			}
			Array.Sort<T>(this._items, index, count, comparer);
			this._version++;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator((ObjectArray<T>)this);
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if ((array != null) && (array.Rank != 1))
			{
				throw new Exception("ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported");
			}
			try
			{
				Array.Copy(this._items, 0, array, arrayIndex, this._size);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new Exception("ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType");
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator((ObjectArray<T>)this);
		}

		int IList.Add(object item)
		{
			ObjectArray<T>.VerifyValueType(item);
			this.Add((T)item);
			return (this.Count - 1);
		}

		bool IList.Contains(object item)
		{
			return (ObjectArray<T>.IsCompatibleObject(item) && this.Contains((T)item));
		}

		int IList.IndexOf(object item)
		{
			if (ObjectArray<T>.IsCompatibleObject(item))
			{
				return this.IndexOf((T)item);
			}
			return -1;
		}

		void IList.Insert(int index, object item)
		{
			ObjectArray<T>.VerifyValueType(item);
			this.Insert(index, (T)item);
		}

		void IList.Remove(object item)
		{
			if (ObjectArray<T>.IsCompatibleObject(item))
			{
				this.Remove((T)item);
			}
		}

		public T[] ToArray()
		{
			T[] destinationArray = new T[this._size];
			Array.Copy(this._items, 0, destinationArray, 0, this._size);
			return destinationArray;
		}

		public void TrimExcess()
		{
			int num = (int)(this._items.Length * 0.9);
			if (this._size < num)
			{
				this.Capacity = this._size;
			}
		}

		public bool TrueForAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match");
			}
			for (int i = 0; i < this._size; i++)
			{
				if (!match(this._items[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static void VerifyValueType(object value)
		{
			if (!ObjectArray<T>.IsCompatibleObject(value))
			{
				throw new Exception("ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T)");
			}
		}

		// Properties
		public int Capacity
		{
			get
			{
				return this._items.Length;
			}
			set
			{
				if (value != this._items.Length)
				{
					if (value < this._size)
					{
						throw new Exception("ExceptionResource ArgumentOutOfRange_SmallCapacity");
					}
					if (value > 0)
					{
						T[] destinationArray = new T[value];
						if (this._size > 0)
						{
							Array.Copy(this._items, 0, destinationArray, 0, this._size);
						}
						this._items = destinationArray;
					}
					else
					{
						this._items = ObjectArray<T>._emptyArray;
					}
				}
			}
		}

		public int Count
		{
			get
			{
				return this._size;
			}
		}

		public T this[int index]
		{
			get
			{
                //checkAndGrow(index);
                int diff = index + 1 - _size;
                for (int i = 0; i < diff; ++i)
                {
                    Add(new T());
                }

				if (index >= this._size)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException()");
				}
				return this._items[index];
			}
			set
			{
                int diff = index + 1 - _size;
                for (int i = 0; i < diff; ++i)
                {
                    Add(new T());
                }

                //checkAndGrow(index);
				if (index >= this._size)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException()");
				}
				this._items[index] = value;
				this._version++;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}


		object IList.this[int index]
		{
			get
			{
                //checkAndGrow(index);
                int diff = index + 1 - _size;
                for (int i = 0; i < diff; ++i)
                {
                    Add(new T());
                }

				return this[index];
			}
			set
			{
                //checkAndGrow(index);
                int diff = index + 1 - _size;
                for (int i = 0; i < diff; ++i)
                {
                    Add(new T());
                }

				ObjectArray<T>.VerifyValueType(value);
				this[index] = (T)value;
			}
		}

		private void checkAndGrow(int newSize)
		{
			int diff = newSize+1 - _size;
			for(int i=0;i<diff;++i)
			{
				Add(new T());
			}
		}

		// Nested Types
		[StructLayout(LayoutKind.Sequential)]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private ObjectArray<T> list;
			private int index;
			private int version;
			private T current;
			internal Enumerator(ObjectArray<T> list)
			{
				this.list = list;
				this.index = 0;
				this.version = list._version;
				this.current = default(T);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				ObjectArray<T> list = this.list;
				if ((this.version == list._version) && (this.index < list._size))
				{
					this.current = list._items[this.index];
					this.index++;
					return true;
				}
				return this.MoveNextRare();
			}

			private bool MoveNextRare()
			{
				if (this.version != this.list._version)
				{
					throw new Exception("InvalidOperation EnumFailedVersion");
				}
				this.index = this.list._size + 1;
				this.current = default(T);
				return false;
			}

			public T Current
			{
				get
				{
					return this.current;
				}
			}
			object IEnumerator.Current
			{
				get
				{
					if ((this.index == 0) || (this.index == (this.list._size + 1)))
					{
						throw new Exception("InvalidOperation EnumOpCantHappen");
					}
					return this.Current;
				}
			}
			void IEnumerator.Reset()
			{
				if (this.version != this.list._version)
				{
					throw new Exception("InvalidOperation EnumFailedVersion");
				}
				this.index = 0;
				this.current = default(T);
			}
		}

		internal sealed class FunctorComparer : IComparer<T>
		{
			// Fields
			private Comparer<T> c;
			private Comparison<T> comparison;

			// Methods
			public FunctorComparer(Comparison<T> comparison)
			{
				this.c = Comparer<T>.Default;
				this.comparison = comparison;
			}

			public int Compare(T x, T y)
			{
				return this.comparison(x, y);
			}
		}



	}
}

 
 
 
