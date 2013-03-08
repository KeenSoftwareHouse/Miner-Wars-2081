using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.Generics
{
    /// <summary>
    /// Dynamic object pool. It's allocate new instance when necessary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyDynamicObjectPool<T> where T : class
    {
        private Stack<T> m_poolStack;

        public MyDynamicObjectPool(int capacity)
        {
            m_poolStack = new Stack<T>(capacity);
            Preallocate(capacity);
        }

        private void Preallocate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T instance = Activator.CreateInstance<T>();
                m_poolStack.Push(instance);
            }
        }

        public T Allocate()
        {
            if (m_poolStack.Count == 0)
            {
                Preallocate(1);
            }
            T item = m_poolStack.Pop();
            return item;
        }

        public void Deallocate(T item)
        {
            m_poolStack.Push(item);
        }
    }
}
