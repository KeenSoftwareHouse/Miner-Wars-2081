using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MinerWarsMath
{
    public class NativeDAABBTree<T>
    {
        public struct Node
        {
            public readonly IntPtr Handle;
            public T UserData;

            public Node(IntPtr handle, T userData)
            {
                Handle = handle;
                UserData = userData;
            }
        }

        [ThreadStatic]
        private static int[] m_resultList;

        const int DEFAULT_SIZE = 1024;

        private List<Node> m_nodes = new List<Node>(DEFAULT_SIZE);
        private IntPtr m_handle;
        private float m_margin;

        public NativeDAABBTree(float extension)
        {
            m_handle = NativeDAABBTreeInterop.Create();
            m_margin = extension;
        }

        ~NativeDAABBTree()
        {
            NativeDAABBTreeInterop.Destroy(m_handle);
        }

        public void Clear()
        {
            NativeDAABBTreeInterop.Clear(m_handle);
            m_nodes.Clear();
        }

        public Node AddProxy(T data, ref BoundingBox aabb)
        {
            int nextIndex = m_nodes.Count;
            var handle = NativeDAABBTreeInterop.Insert(m_handle, ref aabb, nextIndex);
            var node = new Node(handle, data);
            m_nodes.Add(node);
            return node;
        }

        public void MoveProxy(Node node, ref BoundingBox aabb)
        {
            NativeDAABBTreeInterop.Move(m_handle, node.Handle, ref aabb, m_margin);
        }

        public void MoveProxy(Node node, ref BoundingBox aabb, Vector3 velocity, float margin)
        {
            NativeDAABBTreeInterop.Move(m_handle, node.Handle, ref aabb, ref velocity, margin);
        }

        public void RemoveProxy(Node node)
        {
            int lastIndex = m_nodes.Count - 1;
            int index = NativeDAABBTreeInterop.RemoveAndChangeData(m_handle, node.Handle, m_nodes[lastIndex].Handle);
            m_nodes[index] = m_nodes[lastIndex];
            m_nodes.RemoveAt(lastIndex);
        }

        public bool IsEmpty
        {
            get
            {
                return NativeDAABBTreeInterop.IsEmpty(m_handle);
            }
        }

        public void OptimizeBottomUp()
        {
            NativeDAABBTreeInterop.OptimizeBottomUp(m_handle);
        }

        public void OptimizeTopDown(int bu_threshold = 128)
        {
            NativeDAABBTreeInterop.OptimizeTopDown(m_handle, bu_threshold);
        }

        public void OptimizeIncremental(int passes)
        {
            NativeDAABBTreeInterop.OptimizeIncremental(m_handle, passes);
        }

        public void GetAll(List<T> addToList)
        {
            foreach (var node in m_nodes)
            {
                addToList.Add(node.UserData);
            }
        }

        public void OverlapAABB(List<T> addToList, ref BoundingBox aabb)
        {
            if (m_resultList == null)
                m_resultList = new int[DEFAULT_SIZE];

            int count;

            while ((count = NativeDAABBTreeInterop.QueryAABB(m_handle, ref aabb, m_resultList, 0, m_resultList.Length)) < 0)
            {
                m_resultList = new int[m_resultList.Length * 2];
            }

            for (int i = 0; i < count; i++)
            {
                int dataIndex = m_resultList[i];
                addToList.Add(m_nodes[dataIndex].UserData);
            }
        }

        public void OverlapFrustum(List<T> addToList, ref BoundingFrustum frustum)
        {
            if (m_resultList == null)
                m_resultList = new int[DEFAULT_SIZE];

            int count;            

            while ((count = NatDAABB.OverlapFrustum(m_resultList, ref frustum, m_handle)) < 0)
            {
                m_resultList = new int[m_resultList.Length * 2];
            }

            for (int i = 0; i < count; i++)
            {
                int dataIndex = m_resultList[i];
                addToList.Add(m_nodes[dataIndex].UserData);
            }
        }

        public void OverlapSphere(List<T> addToList, ref BoundingSphere sphere)
        {
            if (m_resultList == null)
                m_resultList = new int[DEFAULT_SIZE];

            int count;

            while ((count = NativeDAABBTreeInterop.QuerySphere(m_handle, ref sphere.Center, sphere.Radius, m_resultList, 0, m_resultList.Length)) < 0)
            {
                m_resultList = new int[m_resultList.Length * 2];
            }

            for (int i = 0; i < count; i++)
            {
                int dataIndex = m_resultList[i];
                addToList.Add(m_nodes[dataIndex].UserData);
            }
        }

        public void RayQuery(List<T> addToList, Vector3 from, Vector3 to)
        {
            if (m_resultList == null)
                m_resultList = new int[DEFAULT_SIZE];

            int count;

            while ((count = NativeDAABBTreeInterop.QueryRay(m_handle, ref from, ref to, m_resultList, 0, m_resultList.Length)) < 0)
            {
                m_resultList = new int[m_resultList.Length * 2];
            }

            for (int i = 0; i < count; i++)
            {
                int dataIndex = m_resultList[i];
                addToList.Add(m_nodes[dataIndex].UserData);
            }
        }

        public void RayQuery(List<T> addToList, ref Ray ray)
        {
            Vector3 to = ray.Position + ray.Direction * 10000;
            RayQuery(addToList, ray.Position, to);
        }
    }

    class NatDAABB
    {
        public static unsafe int OverlapFrustum(int[] resultList, ref BoundingFrustum frustum, IntPtr handle)
        {
            float* x = stackalloc float[6];
            float* y = stackalloc float[6];
            float* z = stackalloc float[6];
            float* d = stackalloc float[6];

            Over(ref frustum, x, y, z, d);

            return NativeDAABBTreeInterop.QuerySixPlanes(handle, x, y, z, d, resultList, 0, resultList.Length);
        }

        public static unsafe void Over(ref BoundingFrustum frustum, float* x, float* y, float* z, float* d)
        {
            for (int i = 0; i < 6; i++)
            {
                Plane p = frustum[i];
                x[i] = -p.Normal.X;
                y[i] = -p.Normal.Y;
                z[i] = -p.Normal.Z;
                d[i] = -p.D;
            }
        }
    }
}
