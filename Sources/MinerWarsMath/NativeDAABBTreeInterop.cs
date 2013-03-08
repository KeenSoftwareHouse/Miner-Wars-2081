using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace MinerWarsMath
{
    partial class NativeDAABBTreeInterop
    {
        /// Return Type: btDbvt*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Create", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create();


        /// Return Type: void
        ///tree: btDbvt*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy(IntPtr tree);


        /// Return Type: void
        ///tree: btDbvt*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Clear", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clear(IntPtr tree);

        /// Return Type: btDbvtNode*
        ///tree: btDbvt*
        ///volume: btDbvtVolume
        ///data: void*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Insert", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Insert(IntPtr tree, ref BoundingBox box, int data);

        /// Return Type: btDbvtNode*
        ///tree: btDbvt*
        ///volume: btDbvtVolume
        ///data: void*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Move", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Move(IntPtr tree, IntPtr node, ref BoundingBox box);

        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Move2", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Move(IntPtr tree, IntPtr node, ref BoundingBox box, ref Vector3 velocity);

        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Move3", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Move(IntPtr tree, IntPtr node, ref BoundingBox box, float margin);

        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Move4", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Move(IntPtr tree, IntPtr node, ref BoundingBox box, ref Vector3 velocity, float margin);

        /// Return Type: void
        ///tree: btDbvt*
        ///node: btDbvtNode*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "Remove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Remove(IntPtr tree, IntPtr node);

        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "ChangeData", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChangeData(IntPtr tree, IntPtr node, int newValue);

        /// <summary>
        /// For performance reasons, when removing node, almost always changing data
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "RemoveAndChangeData", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RemoveAndChangeData(IntPtr tree, IntPtr removeNode, IntPtr changeDataNode);

        /// Return Type: int
        ///tree: btDbvt*
        ///node: btDbvtNode*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "MaxDepth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MaxDepth(IntPtr tree, IntPtr node);


        /// Return Type: int
        ///tree: btDbvt*
        ///node: btDbvtNode*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "CountLeaves", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CountLeaves(IntPtr tree, IntPtr node);


        /// Return Type: boolean
        ///tree: btDbvt*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "IsEmpty", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        public static extern bool IsEmpty(IntPtr tree);


        /// Return Type: void
        ///tree: btDbvt*
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "OptimizeBottomUp", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptimizeBottomUp(IntPtr tree);


        /// Return Type: void
        ///tree: btDbvt*
        ///bu_treshold: int
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "OptimizeTopDown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptimizeTopDown(IntPtr tree, int bu_treshold);


        /// Return Type: void
        ///tree: btDbvt*
        ///passes: int
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "OptimizeIncremental", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OptimizeIncremental(IntPtr tree, int passes);


        /// Return Type: boolean
        ///tree: btDbvt*
        ///dataArray: btDbvtNode**
        ///offset: int
        ///count: int
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "QueryAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QueryAll(IntPtr tree, int[] dataArray, int offset, int count);


        /// Return Type: boolean
        ///tree: btDbvt*
        ///volume: btDbvtVolume
        ///dataArray: btDbvtNode*
        ///offset: int
        ///count: int
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "QueryAABB", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QueryAABB(IntPtr tree, ref BoundingBox box, int[] dataArray, int offset, int count);


        /// Return Type: boolean
        ///tree: btDbvt*
        ///sphereCenter: btVector3
        ///sphereRadius: float
        ///dataArray: btDbvtNode*
        ///offset: int
        ///count: int
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "QuerySphere", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QuerySphere(IntPtr tree, ref Vector3 center, float sphereRadius, int[] dataArray, int offset, int count);


        /// Return Type: boolean
        ///tree: btDbvt*
        ///normals: btVector3*
        ///offsets: float*
        ///planeCount: int
        ///dataArray: btDbvtNode**
        ///offset: int
        ///count: int
        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "QuerySixPlanes", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int QuerySixPlanes(IntPtr tree, float* normalX, float* normalY, float* normalZ, float* offsets, int[] dataArray, int offset, int count);

        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "QueryRay", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QueryRay(IntPtr tree, ref Vector3 from, ref Vector3 to, int[] dataArray, int offset, int count);

        [SuppressUnmanagedCodeSecurity]
        [DllImportAttribute("NativeDAABBTree.dll", EntryPoint = "QueryRayNotThreadSafe", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QueryRayNotThreadSafe(IntPtr tree, ref Vector3 from, ref Vector3 to, int[] dataArray, int offset, int count);
    }
}
