using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//  Object pool generic class
//
//  Use as a container for objects when you can't allocate new objects using new (because of GC). E.g. missiles, bullets, etc.
//  All methods are O(1), including iterating over list of active objects.
//  Pool holds only classess (not structs). It can can be extended, maybe.
//  When you use only objects using this pool, no new objects will be initialized and GC can't happen. This was tested.
//  Using foreach doesn't create garbage, because enumerator is created only once, not for each 'foreach'.
//
//  Usage: initialize with known max capacity. Then when object is needed, allocate it from pool using Allocate().
//  If object isn't needed anymore (or died), mark it for deallocation using MarkForDeallocate(). This method doesn't
//  deallocate object immediately, just mark it. Reason is: we often need to deallocate object during while we are
//  iterating list of active objects.
//  When you are done, call DeallocateAllMarked(). Again, don't call it during iterating the pool.
//  Iteration is possible using 'foreach', and iterates only over active objects. It's fast, no enumerator is initialized
//  for every 'foreach'. Iteration is not over T, but over LinkedListNode<T>
//
//  Example:
//      MyObjectsPool<MyMissile> m_missiles;
//      foreach (LinkedListNode<MyMissile> item in m_missiles)
//      {
//          m_missiles.MarkForDeallocate(item);
//      }
//      foreach (LinkedListNode<MyMissile> item in m_missiles)
//      {
//          m_missiles.MarkForDeallocate(item);
//      }
//      m_missiles.DeallocateAllMarked();
//      for (int value = 0; value < MyFakes.MAX_MISSILES_COUNT; value++)
//      {
//          MyMissile temp = m_missiles.Allocate();
//      } 

  

namespace MinerWars.CommonLIB.AppCode.Generics
{
    public class MyObjectsPoolEnumerator<T> : IEnumerator<LinkedListNode<T>>
    {
        LinkedList<T> m_list;
        LinkedListNode<T> m_current = null;
        bool m_reset;

        public LinkedListNode<T> Current
        {
            get
            {
                return m_current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return m_current;
            }
        }

        public bool MoveNext()
        {
            if (m_reset == true)
            {
                m_reset = false;

                if (m_list.First == null)
                {
                    return false;
                }
                else
                {
                    m_current = m_list.First;
                    return true;
                }
            }
            else
            {
                if (m_current.Next == null)
                {
                    return false;
                }
                else
                {
                    m_current = m_current.Next;
                    return true;
                }
            }
        }

        public void Reset()
        {
            m_reset = true;
            m_current = null;
        }

        public void Dispose()
        { 
        }

        public MyObjectsPoolEnumerator(LinkedList<T> list)
        {
            m_list = list;
        }
    }
    
    public class MyObjectsPool<T> : IEnumerable<LinkedListNode<T>> where T : class, new()
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //  Preallocated array from which pool gives objects
        LinkedListNode<T>[] m_itemsArray;

        //  Linked list containing only active (allocated) items. So it doesn't contain all preallocated items.
        LinkedList<T> m_activeItems;

        //  References to items not-allocated yet to (they are only preallocated)
        Stack<LinkedListNode<T>> m_notAllocatedItems;
        
        //  References to items to delete
        Stack<LinkedListNode<T>> m_itemsToDelete;

        //  When we want to find "linked list node" by object reference - because maintaining those nodes isn't really funny
        Dictionary<T, LinkedListNode<T>> m_itemToListNode;
        
        //  Count of items allowed to store in this pool.
        int m_capacity;

        MyObjectsPoolEnumerator<T> m_enumerator;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
        //  Class doesn't support parameter-less constructor
        private MyObjectsPool()
        {
        }

        public MyObjectsPool(int capacity)
        {
            //  Pool should contain at least one preallocated item!
            MyCommonDebugUtils.AssertRelease(capacity > 0);

            m_capacity = capacity;
            m_notAllocatedItems = new Stack<LinkedListNode<T>>(m_capacity);
            m_itemsToDelete = new Stack<LinkedListNode<T>>(m_capacity);
            m_activeItems = new LinkedList<T>();
            m_itemToListNode = new Dictionary<T, LinkedListNode<T>>(m_capacity);

            m_enumerator = new MyObjectsPoolEnumerator<T>(m_activeItems);

            //  Preallocate items
            m_itemsArray = new LinkedListNode<T>[m_capacity];
            for (int i = 0; i < m_capacity; i++)
            {
                m_itemsArray[i] = new LinkedListNode<T>(new T());
                m_notAllocatedItems.Push(m_itemsArray[i]);
            }
        }

        //  Allocates new object in the pool and returns reference to it.
        //  If pool doesn't have free object (it's full), null is returned. But this shouldn't happen if capacity is chosen carefully.
        public T Allocate()
        {
            return Allocate(false);
        }

        //  Allocates new object in the pool and returns reference to it.
        //  If pool doesn't have free object (it's full), null is returned. But this shouldn't happen if capacity is chosen carefully.
        public T Allocate(bool nullAllowed)
        {
            LinkedListNode<T> ret = AllocateEx();
            System.Diagnostics.Debug.Assert(nullAllowed ? true : ret != null, "MyObjectsPool is full and cannot allocate any other item!");
            if (ret == null)
            {
                return null;
            }
            else
            {
                return ret.Value;
            }
        }

        //  Allocates new object in the pool and returns reference to its linked list node.
        //  If pool doesn't have free object (it's full), null is returned. But this shouldn't happen if capacity is chosen carefully.
        //  Difference between Allocate() and AllocateEx() is that first returns object and second returns linked list node that contains this object.
        public LinkedListNode<T> AllocateEx()
        {
            if (m_notAllocatedItems.Count <= 0)
            {
                return null;
            }

            //  Get free item from the stack
            LinkedListNode<T> allocatedItem = m_notAllocatedItems.Pop();

            //  Add it to active items linked list
            m_activeItems.AddLast(allocatedItem);

            //  When we want to find "linked list node" by object reference - because maintaining those nodes isn't really funny
            m_itemToListNode.Add(allocatedItem.Value, allocatedItem);

            //  Return reference to the caller
            return allocatedItem;
        }

        //  Return count of active items in the pool
        public int GetActiveCount()
        {
            return m_activeItems.Count();
        }

        //  Return max number of items in the pool
        public int GetCapacity()
        {
            return m_capacity;
        }

        //  Deallocates object imediatelly. This is the version that accepts object, and then it find its node.
        //  IMPORTANT: Don't call while iterating this object pool!
        public void Deallocate(T item)
        {
            LinkedListNode<T> node;
            try
            {
                node = m_itemToListNode[item];
            }
            catch (KeyNotFoundException ex) // This is just for making sure that the game will not crash
            {
                MyMwcLog.WriteLine(ex);
                System.Diagnostics.Debug.Fail(ex.ToString());
                return;
            }

            Deallocate(node);
        }

        //  Deallocates object imediatelly.
        //  IMPORTANT: Don't call while iterating this object pool!
        public void Deallocate(LinkedListNode<T> node)
        {
            //  Remove item from active items linked list
            m_activeItems.Remove(node);

            //  When we want to find "linked list node" by object reference - because maintaining those nodes isn't really funny
            m_itemToListNode.Remove(node.Value);

            //  Add item to stack of free items
            m_notAllocatedItems.Push(node);
        }

        //  Marks object for deallocation, but doesn't remove it immediately. Call it during iterating the pool.
        public void MarkForDeallocate(LinkedListNode<T> node)
        {
            m_itemsToDelete.Push(node);
        }

        //  Deallocates objects marked for deallocation. If same object was marked twice or more times for
        //  deallocation, this method will handle it and deallocate it only once (rest is ignored).
        //  IMPORTANT: Call only when not iterating the pool!!!
        public void DeallocateAllMarked()
        {
            while (m_itemsToDelete.Count > 0)
            {
                LinkedListNode<T> node = m_itemsToDelete.Pop();

                //  If a.List equals null, then object was already deallocated
                if (node.List != null)
                {
                    Deallocate(node);
                }
            }
        }
        
        //  Deallocates all objects
        //  IMPORTANT: Call only when not iterating the pool!!!
        public void DeallocateAll()
        {
            while (m_activeItems.Count > 0)
            {
                LinkedListNode<T> node = m_activeItems.First;

                //  If a.List equals null, then object was already deallocated
                if (node.List != null)
                {
                    Deallocate(node);
                }
            }
        }

        public IEnumerator<LinkedListNode<T>> GetEnumerator()
        {
            m_enumerator.Reset();
            return m_enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            m_enumerator.Reset();
            return m_enumerator;
        }

        //  Return reference to array of preallocated items. Use it very carefully, as it's revealing private member.
        //  Use it only if you need to initialize preallocated object (e.g. call Init() on phys-objects)
        public LinkedListNode<T>[] GetPreallocatedItemsArray()
        {
            return m_itemsArray;
        }
    }
}
