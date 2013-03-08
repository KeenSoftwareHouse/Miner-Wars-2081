/*
 * 
 * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
 *
This source file is part of GIMPACT Library.

For the latest info, see http://gimpact.sourceforge.net/

Copyright (c) 2007 Francisco Leon Najera. C.C. 80087371.
email: projectileman@yahoo.com


This software is provided 'as-is', without any express or implied warranty.
In no event will the authors be held liable for any damages arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it freely,
subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using System.Diagnostics;
using BulletXNA.LinearMath;
using System.IO;
using System;

namespace BulletXNA.BulletCollision
{
    public delegate float? ProcessCollisionHandler(int triangleIndex);

    public class GImpactQuantizedBvh
    {

        protected QuantizedBvhTree m_box_tree;
        protected IPrimitiveManagerBase m_primitive_manager;
        protected int m_size = 0;

        static bool QuantizedNodeCollision(
            GImpactQuantizedBvh boxset0, GImpactQuantizedBvh boxset1,
            BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0,
            int node0, int node1, bool complete_primitive_tests)
        {
            AABB box0;
            boxset0.GetNodeBound(node0, out box0);
            AABB box1;
            boxset1.GetNodeBound(node1, out box1);

            return box0.OverlappingTransCache(ref box1, ref trans_cache_1to0, complete_primitive_tests);
            //	box1.appy_transform_trans_cache(trans_cache_1to0);
            //	return box0.has_collision(box1);

        }

        public byte[] Save()
        {
            return m_box_tree.Save();
        }

        public int Size
        {
            get { return m_size; }
        }


        public void Load(byte[] byteArray)
        {
            m_box_tree.Load(byteArray);
            m_size = byteArray.Length;
        }

        //stackless refit
        //protected void Refit()
        //{
        //    int nodecount = GetNodeCount();
        //    while (nodecount-- != 0)
        //    {
        //        if (IsLeafNode(nodecount))
        //        {
        //            AABB leafbox;
        //            m_primitive_manager.GetPrimitiveBox(GetNodeData(nodecount), out leafbox);
        //            SetNodeBound(nodecount, ref leafbox);
        //        }
        //        else
        //        {
        //            //const GIM_BVH_TREE_NODE * nodepointer = get_node_pointer(nodecount);
        //            //get left bound
        //            AABB bound = new AABB();
        //            bound.Invalidate();

        //            AABB temp_box;

        //            int child_node = GetLeftNode(nodecount);
        //            if (child_node != 0)
        //            {
        //                GetNodeBound(child_node, out temp_box);
        //                bound.Merge(ref temp_box);
        //            }

        //            child_node = GetRightNode(nodecount);
        //            if (child_node != 0)
        //            {
        //                GetNodeBound(child_node, out temp_box);
        //                bound.Merge(ref temp_box);
        //            }

        //            SetNodeBound(nodecount, ref bound);
        //        }
        //    }
        //}

        //! this constructor doesn't build the tree. you must call	buildSet
        public GImpactQuantizedBvh()
        {
            m_box_tree = new QuantizedBvhTree();
        }

        //! this constructor doesn't build the tree. you must call	buildSet
        public GImpactQuantizedBvh(IPrimitiveManagerBase primitive_manager)
        {
            m_primitive_manager = primitive_manager;
            m_box_tree = new QuantizedBvhTree();
        }

        public AABB GetGlobalBox()
        {
            AABB totalbox;
            GetNodeBound(0, out totalbox);
            return totalbox;
        }

        public void SetPrimitiveManager(IPrimitiveManagerBase primitive_manager)
        {
            m_primitive_manager = primitive_manager;
        }

        public IPrimitiveManagerBase GetPrimitiveManager()
        {
            return m_primitive_manager;
        }


        //! node manager prototype functions
        ///@{

        //! this attemps to refit the box set.
        //public void Update()
        //{
        //    Refit();
        //}

        //! this rebuild the entire set
        public void BuildSet()
        {
            //obtain primitive boxes
            int listSize = m_primitive_manager.GetPrimitiveCount();
            GIM_BVH_DATA_ARRAY primitive_boxes = new GIM_BVH_DATA_ARRAY(listSize);
            // forces boxes to be allocated
            primitive_boxes.Resize(listSize);

            GIM_BVH_DATA[] rawArray = primitive_boxes.GetRawArray();
            for (int i = 0; i < listSize; i++)
            {
                m_primitive_manager.GetPrimitiveBox(i, out rawArray[i].m_bound);
                rawArray[i].m_data = i;
            }

            m_box_tree.BuildTree(primitive_boxes);

        }

        //! returns the indices of the primitives in the m_primitive_manager
        public bool BoxQuery(ref AABB box, ObjectArray<int> collided_results)
        {
            return BoxQuery(ref box, collided_results, false);
        }
        public bool BoxQuery(ref AABB box, ObjectArray<int> collided_results, bool graphics)
        {
            int curIndex = 0;
            int numNodes = GetNodeCount();

            if (BulletGlobals.g_streamWriter != null && BulletGlobals.debugGimpactBVH && !graphics)
            {
                BulletGlobals.g_streamWriter.WriteLine("QIQBVH BoxQuery [{0}]", numNodes);
            }

            //quantize box

            UShortVector3 quantizedMin;
            UShortVector3 quantizedMax;

            m_box_tree.QuantizePoint(out quantizedMin, ref box.m_min);
            m_box_tree.QuantizePoint(out quantizedMax, ref box.m_max);


            while (curIndex < numNodes)
            {

                //catch bugs in tree data

                bool aabbOverlap = m_box_tree.TestQuantizedBoxOverlap(curIndex, ref quantizedMin, ref quantizedMax);
                bool isLeafNode = IsLeafNode(curIndex);


                if (BulletGlobals.g_streamWriter != null && BulletGlobals.debugGimpactBVH && !graphics)
                {
                    BulletGlobals.g_streamWriter.WriteLine("QIQBVH BoxQuery [{0}] o[{1}] l[{2}]", curIndex, aabbOverlap ? 1 : 0, isLeafNode ? 1 : 0);
                }

                if (isLeafNode && aabbOverlap)
                {
                    foreach (var i in GetNodeData(curIndex))
                    {
                        collided_results.Add(i);
                    }
                }

                if (aabbOverlap || isLeafNode)
                {
                    //next subnode
                    curIndex++;
                }
                else
                {
                    //skip node
                    curIndex += GetEscapeNodeIndex(curIndex);
                }
            }
            if (collided_results.Count > 0) return true;
            return false;
        }

        //! returns the indices of the primitives in the m_primitive_manager
        public bool BoxQueryTrans(ref AABB box,
             ref IndexedMatrix transform, ObjectArray<int> collided_results)
        {
            AABB transbox = box;
            transbox.ApplyTransform(ref transform);
            return BoxQuery(ref transbox, collided_results);
        }

        public bool RayQueryClosest(ref IndexedVector3 ray_dir, ref IndexedVector3 ray_origin, ProcessCollisionHandler handler)
        {
            int curIndex = 0;
            int numNodes = GetNodeCount();
            float distance = float.PositiveInfinity;

            while (curIndex < numNodes)
            {
                AABB bound;
                GetNodeBound(curIndex, out bound);

                //catch bugs in tree data

                float? aabbDist = bound.CollideRayDistance(ref ray_origin, ref ray_dir);
                bool isLeafNode = IsLeafNode(curIndex);
                bool aabbOverlapSignificant = aabbDist.HasValue && aabbDist.Value < distance;

                if (aabbOverlapSignificant && isLeafNode)
                {
                    foreach (var i in GetNodeData(curIndex))
                    {
                        float? newDist = handler(i);
                        if (newDist.HasValue && newDist.Value < distance)
                        {
                            distance = newDist.Value;
                        }
                    }
                }

                if (aabbOverlapSignificant || isLeafNode)
                {
                    //next subnode
                    curIndex++;
                }
                else
                {
                    //skip node
                    curIndex += GetEscapeNodeIndex(curIndex);
                }
            }
            return distance != float.PositiveInfinity;
        }

        //! returns the indices of the primitives in the m_primitive_manager
        public bool RayQuery(ref IndexedVector3 ray_dir, ref IndexedVector3 ray_origin,
            ObjectArray<int> collided_results)
        {
            int curIndex = 0;
            int numNodes = GetNodeCount();

            while (curIndex < numNodes)
            {
                AABB bound;
                GetNodeBound(curIndex, out bound);

                //catch bugs in tree data

                bool aabbOverlap = bound.CollideRay(ref ray_origin, ref ray_dir);
                bool isLeafNode = IsLeafNode(curIndex);

                if (isLeafNode && aabbOverlap)
                {
                    foreach (var i in GetNodeData(curIndex))
                    {
                        collided_results.Add(i);
                    }
                }

                if (aabbOverlap || isLeafNode)
                {
                    //next subnode
                    curIndex++;
                }
                else
                {
                    //skip node
                    curIndex += GetEscapeNodeIndex(curIndex);
                }
            }
            if (collided_results.Count > 0) return true;
            return false;
        }

        //! tells if this set has hierarcht
        public bool HasHierarchy()
        {
            return true;
        }

        //! tells if this set is a trimesh
        public bool IsTrimesh()
        {
            return m_primitive_manager.IsTrimesh();
        }

        //! node count
        public int GetNodeCount()
        {
            return m_box_tree.GetNodeCount();
        }

        //! tells if the node is a leaf
        public bool IsLeafNode(int nodeindex)
        {
            return m_box_tree.IsLeafNode(nodeindex);
        }

        public int[] GetNodeData(int nodeindex)
        {
            return m_box_tree.GetNodeData(nodeindex);
        }

        public void GetNodeBound(int nodeindex, out AABB bound)
        {
            m_box_tree.GetNodeBound(nodeindex, out bound);
        }

        public void SetNodeBound(int nodeindex, ref AABB bound)
        {
            m_box_tree.SetNodeBound(nodeindex, ref bound);
        }


        public int GetLeftNode(int nodeindex)
        {
            return m_box_tree.GetLeftNode(nodeindex);
        }

        public int GetRightNode(int nodeindex)
        {
            return m_box_tree.GetRightNode(nodeindex);
        }

        public int GetEscapeNodeIndex(int nodeindex)
        {
            return m_box_tree.GetEscapeNodeIndex(nodeindex);
        }

        //public void GetNodeTriangle(int nodeindex, PrimitiveTriangle triangle)
        //{
        //    m_primitive_manager.GetPrimitiveTriangle(GetNodeData(nodeindex), triangle);
        //}


        //SIMD_FORCE_INLINE const BT_QUANTIZED_BVH_NODE * get_node_pointer(int index = 0) const
        //{
        //    return m_box_tree.get_node_pointer(index);
        //}


        public static float GetAverageTreeCollisionTime()
        {
            return 1.0f;
        }


        //public static void FindQuantizedCollisionPairsRecursive(
        //    GImpactQuantizedBvh boxset0, GImpactQuantizedBvh boxset1,
        //    PairSet collision_pairs,
        //    ref BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0,
        //    int node0, int node1, bool complete_primitive_tests)
        //{



        //    if (QuantizedNodeCollision(
        //        boxset0, boxset1, trans_cache_1to0,
        //        node0, node1, complete_primitive_tests) == false) return;//avoid colliding internal nodes

        //    if (boxset0.IsLeafNode(node0))
        //    {
        //        if (boxset1.IsLeafNode(node1))
        //        {
        //            // collision result
        //            collision_pairs.PushPair(boxset0.GetNodeData(node0), boxset1.GetNodeData(node1));
        //            return;
        //        }
        //        else
        //        {

        //            //collide left recursive

        //            FindQuantizedCollisionPairsRecursive(
        //                                boxset0, boxset1,
        //                                collision_pairs, ref trans_cache_1to0,
        //                                node0, boxset1.GetLeftNode(node1), false);

        //            //collide right recursive
        //            FindQuantizedCollisionPairsRecursive(
        //                                boxset0, boxset1,
        //                                collision_pairs, ref trans_cache_1to0,
        //                                node0, boxset1.GetRightNode(node1), false);


        //        }
        //    }
        //    else
        //    {
        //        if (boxset1.IsLeafNode(node1))
        //        {

        //            //collide left recursive
        //            FindQuantizedCollisionPairsRecursive(
        //                                boxset0, boxset1,
        //                                collision_pairs, ref trans_cache_1to0,
        //                                boxset0.GetLeftNode(node0), node1, false);


        //            //collide right recursive

        //            FindQuantizedCollisionPairsRecursive(
        //                                boxset0, boxset1,
        //                                collision_pairs, ref trans_cache_1to0,
        //                                boxset0.GetRightNode(node0), node1, false);


        //        }
        //        else
        //        {
        //            //collide left0 left1



        //            FindQuantizedCollisionPairsRecursive(
        //                boxset0, boxset1,
        //                collision_pairs, ref trans_cache_1to0,
        //                boxset0.GetLeftNode(node0), boxset1.GetLeftNode(node1), false);

        //            //collide left0 right1

        //            FindQuantizedCollisionPairsRecursive(
        //                boxset0, boxset1,
        //                collision_pairs, ref trans_cache_1to0,
        //                boxset0.GetLeftNode(node0), boxset1.GetRightNode(node1), false);


        //            //collide right0 left1

        //            FindQuantizedCollisionPairsRecursive(
        //                boxset0, boxset1,
        //                collision_pairs, ref trans_cache_1to0,
        //                boxset0.GetRightNode(node0), boxset1.GetLeftNode(node1), false);

        //            //collide right0 right1

        //            FindQuantizedCollisionPairsRecursive(
        //                boxset0, boxset1,
        //                collision_pairs, ref trans_cache_1to0,
        //                boxset0.GetRightNode(node0), boxset1.GetRightNode(node1), false);

        //        }// else if node1 is not a leaf
        //    }// else if node0 is not a leaf
        //}


//        public static void FindCollision(GImpactQuantizedBvh boxset0, ref IndexedMatrix trans0,
//        GImpactQuantizedBvh boxset1, ref IndexedMatrix trans1,
//        PairSet collision_pairs)
//        {
//            if (boxset0.GetNodeCount() == 0 || boxset1.GetNodeCount() == 0)
//            {
//                return;
//            }

//            BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0 = new BT_BOX_BOX_TRANSFORM_CACHE();

//            trans_cache_1to0.CalcFromHomogenic(ref trans0, ref trans1);

//#if TRI_COLLISION_PROFILING
//    BulletGlobals.StartProfile("GIMPACT-TRIMESH");
//#endif //TRI_COLLISION_PROFILING

//            FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, 0, 0, true);
//#if TRI_COLLISION_PROFILING
//    BulletGlobals.StopProfile();
//#endif //TRI_COLLISION_PROFILING

//        }
    }

    public class BT_QUANTIZED_BVH_NODE
    {
        //12 bytes
        public UShortVector3 m_quantizedAabbMin;
        public UShortVector3 m_quantizedAabbMax;
        //4 bytes
        public int[] m_escapeIndexOrDataIndex;
        //public int[] m_indices;

        public bool IsLeafNode()
        {
            //skipindex is negative (internal node), triangleindex >=0 (leafnode)
            return (m_escapeIndexOrDataIndex[0] >= 0);
        }

        public int GetEscapeIndex()
        {
            //btAssert(m_escapeIndexOrDataIndex < 0);
            return m_escapeIndexOrDataIndex == null ? 0 : - m_escapeIndexOrDataIndex[0];
        }

        public void SetEscapeIndex(int index)
        {
            m_escapeIndexOrDataIndex = new int[] { -index };
        }

        public void SetDataIndices(int[] indices)
        {
            m_escapeIndexOrDataIndex = indices;
        }

        public int[] GetDataIndices()
        {
            return m_escapeIndexOrDataIndex;
        }

        //public int GetDataIndex()
        //{
        //    //btAssert(m_escapeIndexOrDataIndex >= 0);

        //    return m_escapeIndexOrDataIndex[0];
        //}

        //public void SetDataIndex(int index)
        //{
        //    m_escapeIndexOrDataIndex[0] = index;
        //}

        public bool TestQuantizedBoxOverlapp(ref UShortVector3 quantizedMin, ref UShortVector3 quantizedMax)
        {
            if (m_quantizedAabbMin.X > quantizedMax.X ||
               m_quantizedAabbMax.X < quantizedMin.X ||
               m_quantizedAabbMin.Y > quantizedMax.Y ||
               m_quantizedAabbMax.Y < quantizedMin.Y ||
               m_quantizedAabbMin.Z > quantizedMax.Z ||
               m_quantizedAabbMax.Z < quantizedMin.Z)
            {
                return false;
            }
            return true;
        }

    }



    public class GIM_QUANTIZED_BVH_NODE_ARRAY : ObjectArray<BT_QUANTIZED_BVH_NODE>
    {
        public GIM_QUANTIZED_BVH_NODE_ARRAY()
        {
        }

        public GIM_QUANTIZED_BVH_NODE_ARRAY(int capacity)
            : base(capacity)
        {
        }
    }

    //! Basic Box tree structure
    public class QuantizedBvhTree
    {
        public const int MAX_INDICES_PER_NODE = 6;

        protected int m_num_nodes;
        protected GIM_QUANTIZED_BVH_NODE_ARRAY m_node_array;
        protected AABB m_global_bound;
        protected IndexedVector3 m_bvhQuantization;

        static void WriteIndexedVector3(IndexedVector3 vector, BinaryWriter bw)
        {
            bw.Write(vector.X);
            bw.Write(vector.Y);
            bw.Write(vector.Z);
        }

        static IndexedVector3 ReadIndexedVector3(BinaryReader br)
        {
            return new IndexedVector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        static void WriteUShortVector3(UShortVector3 vector, BinaryWriter bw)
        {
            bw.Write(vector.X);
            bw.Write(vector.Y);
            bw.Write(vector.Z);
        }

        static UShortVector3 ReadUShortVector3(BinaryReader br)
        {
            var vec = new UShortVector3();
            vec.X = br.ReadUInt16();
            vec.Y = br.ReadUInt16();
            vec.Z = br.ReadUInt16();
            return vec;
        }

        public byte[] Save()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(m_num_nodes);
                WriteIndexedVector3(m_global_bound.m_min, bw);
                WriteIndexedVector3(m_global_bound.m_max, bw);
                WriteIndexedVector3(m_bvhQuantization, bw);

                for (int i = 0; i < m_num_nodes; i++)
                {
                    bw.Write((Int32)m_node_array[i].m_escapeIndexOrDataIndex.Length);
                    for (int j = 0; j < m_node_array[i].m_escapeIndexOrDataIndex.Length; j++)
                    {
                        bw.Write((Int32)m_node_array[i].m_escapeIndexOrDataIndex[j]);
                    }
                    WriteUShortVector3(m_node_array[i].m_quantizedAabbMin, bw);
                    WriteUShortVector3(m_node_array[i].m_quantizedAabbMax, bw);
                }

                return ms.ToArray();
            }
        }

        public void Load(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            using (BinaryReader br = new BinaryReader(ms))
            {
                m_num_nodes = br.ReadInt32();
                var min = ReadIndexedVector3(br);
                var max = ReadIndexedVector3(br);
                m_global_bound = new AABB(ref min, ref max);
                m_bvhQuantization = ReadIndexedVector3(br);

                m_node_array = new GIM_QUANTIZED_BVH_NODE_ARRAY(m_num_nodes);
                for (int i = 0; i < m_num_nodes; i++)
                {
                    int count = br.ReadInt32();

                    BT_QUANTIZED_BVH_NODE node = new BT_QUANTIZED_BVH_NODE();
                    node.m_escapeIndexOrDataIndex = new int[count];
                    for (int j = 0; j < count; j++)
                    {
                        node.m_escapeIndexOrDataIndex[j] = br.ReadInt32();
                    }
                    node.m_quantizedAabbMin = ReadUShortVector3(br);
                    node.m_quantizedAabbMax = ReadUShortVector3(br);
                    m_node_array.Add(node);
                }
            }
        }

        protected void CalcQuantization(GIM_BVH_DATA_ARRAY primitive_boxes)
        {
            CalcQuantization(primitive_boxes, 1.0f);
        }
        protected void CalcQuantization(GIM_BVH_DATA_ARRAY primitive_boxes, float boundMargin)
        {
            //calc globa box
            AABB global_bound = new AABB();
            global_bound.Invalidate();

            int count = primitive_boxes.Count;
            for (int i = 0; i < count; i++)
            {
                global_bound.Merge(ref primitive_boxes.GetRawArray()[i].m_bound);
            }

            GImpactQuantization.CalcQuantizationParameters(out m_global_bound.m_min, out m_global_bound.m_max, out m_bvhQuantization, ref global_bound.m_min, ref global_bound.m_max, boundMargin);

        }

        protected int SortAndCalcSplittingIndex(GIM_BVH_DATA_ARRAY primitive_boxes, int startIndex, int endIndex, int splitAxis)
        {
            int i;
            int splitIndex = startIndex;
            int numIndices = endIndex - startIndex;

            // average of centers
            float splitValue = 0.0f;

            IndexedVector3 means = IndexedVector3.Zero;
            for (i = startIndex; i < endIndex; i++)
            {
                IndexedVector3 center = 0.5f * (primitive_boxes[i].m_bound.m_max +
                             primitive_boxes[i].m_bound.m_min);
                means += center;
            }
            means *= ((1.0f) / (float)numIndices);

            splitValue = means[splitAxis];


            //sort leafNodes so all values larger then splitValue comes first, and smaller values start from 'splitIndex'.
            for (i = startIndex; i < endIndex; i++)
            {
                IndexedVector3 center = 0.5f * (primitive_boxes[i].m_bound.m_max +
                             primitive_boxes[i].m_bound.m_min);
                if (center[splitAxis] > splitValue)
                {
                    //swap
                    primitive_boxes.Swap(i, splitIndex);
                    //swapLeafNodes(i,splitIndex);
                    splitIndex++;
                }
            }

            //if the splitIndex causes unbalanced trees, fix this by using the center in between startIndex and endIndex
            //otherwise the tree-building might fail due to stack-overflows in certain cases.
            //unbalanced1 is unsafe: it can cause stack overflows
            //bool unbalanced1 = ((splitIndex==startIndex) || (splitIndex == (endIndex-1)));

            //unbalanced2 should work too: always use center (perfect balanced trees)
            //bool unbalanced2 = true;

            //this should be safe too:
            int rangeBalancedIndices = numIndices / 3;
            bool unbalanced = ((splitIndex <= (startIndex + rangeBalancedIndices)) || (splitIndex >= (endIndex - 1 - rangeBalancedIndices)));

            if (unbalanced)
            {
                splitIndex = startIndex + (numIndices >> 1);
            }

            Debug.Assert(!((splitIndex == startIndex) || (splitIndex == (endIndex))));

            return splitIndex;


        }

        protected int CalcSplittingAxis(GIM_BVH_DATA_ARRAY primitive_boxes, int startIndex, int endIndex)
        {
            int i;

            IndexedVector3 means = IndexedVector3.Zero;
            IndexedVector3 variance = IndexedVector3.Zero;
            int numIndices = endIndex - startIndex;

            for (i = startIndex; i < endIndex; i++)
            {
                IndexedVector3 center = 0.5f * (primitive_boxes[i].m_bound.m_max +
                             primitive_boxes[i].m_bound.m_min);
                means += center;
            }
            means *= (1.0f / (float)numIndices);

            for (i = startIndex; i < endIndex; i++)
            {
                IndexedVector3 center = 0.5f * (primitive_boxes[i].m_bound.m_max +
                             primitive_boxes[i].m_bound.m_min);
                IndexedVector3 diff2 = center - means;
                diff2 = diff2 * diff2;
                variance += diff2;
            }
            variance *= (1.0f) / ((float)numIndices - 1);

            return MathUtil.MaxAxis(ref variance);
        }

        protected void BuildSubTree(GIM_BVH_DATA_ARRAY primitive_boxes, int startIndex, int endIndex)
        {
            int curIndex = m_num_nodes;
            m_num_nodes++;

            Debug.Assert((endIndex - startIndex) > 0);

            if ((endIndex - startIndex) <= MAX_INDICES_PER_NODE)
            {
                //We have a leaf node
                int count = endIndex - startIndex;
                int[] indices = new int[count];
                AABB bounds = new AABB();
                bounds.Invalidate();

                for (int i = 0; i < count; i++)
                {
                    indices[i] = primitive_boxes[startIndex + i].m_data;
                    bounds.Merge(primitive_boxes.GetRawArray()[startIndex + i].m_bound);
                }
                SetNodeBound(curIndex, ref bounds);
                m_node_array[curIndex].SetDataIndices(indices);

                if (BulletGlobals.g_streamWriter != null && BulletGlobals.debugGimpactBVH)
                {
                    BulletGlobals.g_streamWriter.WriteLine("bst curIndex[{0}] dataIndex[{1}]", curIndex, primitive_boxes[startIndex].m_data);
                    MathUtil.PrintVector3(BulletGlobals.g_streamWriter, "bst min", primitive_boxes[startIndex].m_bound.m_min);
                    MathUtil.PrintVector3(BulletGlobals.g_streamWriter, "bst max", primitive_boxes[startIndex].m_bound.m_max);
                }

                return;
            }
            //calculate Best Splitting Axis and where to split it. Sort the incoming 'leafNodes' array within range 'startIndex/endIndex'.

            //split axis
            int splitIndex = CalcSplittingAxis(primitive_boxes, startIndex, endIndex);

            splitIndex = SortAndCalcSplittingIndex(
                    primitive_boxes, startIndex, endIndex,
                    splitIndex//split axis
                    );


            //calc this node bounding box

            AABB node_bound = new AABB();
            node_bound.Invalidate();

            for (int i = startIndex; i < endIndex; i++)
            {
                node_bound.Merge(ref primitive_boxes.GetRawArray()[i].m_bound);
            }

            SetNodeBound(curIndex, ref node_bound);


            //build left branch
            BuildSubTree(primitive_boxes, startIndex, splitIndex);


            //build right branch
            BuildSubTree(primitive_boxes, splitIndex, endIndex);

            m_node_array.GetRawArray()[curIndex].SetEscapeIndex(m_num_nodes - curIndex);

            if (BulletGlobals.g_streamWriter != null && BulletGlobals.debugGimpactBVH)
            {
                BulletGlobals.g_streamWriter.WriteLine("bst curIndex[{0}] escapeIndex[{1}]", curIndex, m_node_array.GetRawArray()[curIndex].GetEscapeIndex());
                MathUtil.PrintVector3(BulletGlobals.g_streamWriter, "bst node min", node_bound.m_min);
                MathUtil.PrintVector3(BulletGlobals.g_streamWriter, "bst node max", node_bound.m_max);
            }


        }

        public QuantizedBvhTree()
        {
            m_num_nodes = 0;
            m_node_array = new GIM_QUANTIZED_BVH_NODE_ARRAY();
        }

        public QuantizedBvhTree(int defaultCapacity)
        {
            m_num_nodes = 0;
            m_node_array = new GIM_QUANTIZED_BVH_NODE_ARRAY(defaultCapacity);
        }

        //! prototype functions for box tree management
        //!@{
        public void BuildTree(GIM_BVH_DATA_ARRAY primitive_boxes)
        {
            CalcQuantization(primitive_boxes);
            // initialize node count to 0
            m_num_nodes = 0;
            // allocate nodes
            m_node_array.Resize(primitive_boxes.Count * 2);

            BuildSubTree(primitive_boxes, 0, primitive_boxes.Count);
        }

        public void QuantizePoint(out UShortVector3 quantizedpoint, ref IndexedVector3 point)
        {
            GImpactQuantization.QuantizeClamp(out quantizedpoint, ref point, ref m_global_bound.m_min, ref m_global_bound.m_max, ref m_bvhQuantization);
        }


        public bool TestQuantizedBoxOverlap(int node_index, ref UShortVector3 quantizedMin, ref UShortVector3 quantizedMax)
        {
            return m_node_array[node_index].TestQuantizedBoxOverlapp(ref quantizedMin, ref quantizedMax);
        }

        public void ClearNodes()
        {
            m_node_array.Clear();
            m_num_nodes = 0;
        }

        //! node count
        public int GetNodeCount()
        {
            return m_num_nodes;
        }

        //! tells if the node is a leaf
        public bool IsLeafNode(int nodeindex)
        {
            return m_node_array[nodeindex].IsLeafNode();
        }

        public int[] GetNodeData(int nodeindex)
        {
            return m_node_array[nodeindex].GetDataIndices();
        }

        public void GetNodeBound(int nodeindex, out AABB bound)
        {
            bound.m_min = GImpactQuantization.Unquantize(
                ref m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMin,
                ref m_global_bound.m_min, ref m_bvhQuantization);

            bound.m_max = GImpactQuantization.Unquantize(
                ref m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMax,
                ref m_global_bound.m_min, ref m_bvhQuantization);
        }

        public void SetNodeBound(int nodeindex, ref AABB bound)
        {
            GImpactQuantization.QuantizeClamp(out m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMin,
                                ref bound.m_min,
                                ref m_global_bound.m_min,
                                ref m_global_bound.m_max,
                                ref m_bvhQuantization);

            GImpactQuantization.QuantizeClamp(out m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMax,
                                ref bound.m_max,
                                ref m_global_bound.m_min,
                                ref m_global_bound.m_max,
                                ref m_bvhQuantization);
        }

        public int GetLeftNode(int nodeindex)
        {
            return nodeindex + 1;
        }

        public int GetRightNode(int nodeindex)
        {
            if (m_node_array[nodeindex + 1].IsLeafNode()) return nodeindex + 2;
            return nodeindex + 1 + m_node_array[nodeindex + 1].GetEscapeIndex();
        }

        public int GetEscapeNodeIndex(int nodeindex)
        {
            return m_node_array[nodeindex].GetEscapeIndex();
        }

        //public const BT_QUANTIZED_BVH_NODE * get_node_pointer(int index = 0)
        //{
        //    return &m_node_array[index];
        //}

        //!@}
    }


}
