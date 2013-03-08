#region Using Statements

using System.Collections.Generic;
using System.Threading;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;

#endregion

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Cache for triangles that are used during the intraction processing
    /// </summary>
    class MyTriangleCache
    {
        static MyTriangleCache()
        {
            m_Cache = new Stack<List<MyTriangle_Vertex_Normal>>(CACHE_SIZE);
            while (m_Cache.Count < CACHE_SIZE)
            {
                m_Cache.Push(new List<MyTriangle_Vertex_Normal>(TRIANGLES_SIZE));
            }
        }

        public MyTriangleCache()
        {            
            while (m_Cache.Count < CACHE_SIZE)
            {
                m_Cache.Push(new List<MyTriangle_Vertex_Normal>(TRIANGLES_SIZE));
            }            
        }

        public List<MyTriangle_Vertex_Normal> GetFreeTriangleList(MyRBElementInteraction itr)
        {
            List<MyTriangle_Vertex_Normal> retVal = null;
            lock (m_Locker)
            {
                if (m_Cache.Count == 0)
                {
                    m_Cache.Push(new List<MyTriangle_Vertex_Normal>(TRIANGLES_SIZE));
                }
                retVal = m_Cache.Pop();             
            }
            return retVal;
        }

        public void PushBackTriangleList(List<MyTriangle_Vertex_Normal> list)
        {
            lock (m_Locker)
            {
                list.Clear();
                m_Cache.Push(list);
            }            
        }

        private const int CACHE_SIZE = 16;
        private const int TRIANGLES_SIZE = 8096;
        private static Stack<List<MyTriangle_Vertex_Normal>> m_Cache;
        private readonly object m_Locker = new object();
    }
}