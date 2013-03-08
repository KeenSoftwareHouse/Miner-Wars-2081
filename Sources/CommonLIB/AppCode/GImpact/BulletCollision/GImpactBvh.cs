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

namespace BulletXNA.BulletCollision
{
    public class GImpactBvh
    {
        protected BvhTree m_box_tree;
        protected IPrimitiveManagerBase m_primitive_manager;

        //stackless refit
        protected void Refit()
        {
            int nodecount = GetNodeCount();
            while (nodecount-- != 0)
            {
                if (IsLeafNode(nodecount))
                {
                    AABB leafbox;
                    m_primitive_manager.GetPrimitiveBox(GetNodeData(nodecount), out leafbox);
                    SetNodeBound(nodecount, ref leafbox);
                }
                else
                {
                    //const GIM_BVH_TREE_NODE * nodepointer = get_node_pointer(nodecount);
                    //get left bound
                    AABB bound = new AABB();
                    bound.Invalidate();

                    AABB temp_box;

                    int child_node = GetLeftNode(nodecount);
                    if (child_node != 0)
                    {
                        GetNodeBound(child_node, out temp_box);
                        bound.Merge(ref temp_box);
                    }

                    child_node = GetRightNode(nodecount);
                    if (child_node != 0)
                    {
                        GetNodeBound(child_node, out temp_box);
                        bound.Merge(ref temp_box);
                    }

                    SetNodeBound(nodecount, ref bound);
                }
            }

        }

        //! this constructor doesn't build the tree. you must call	buildSet
        public GImpactBvh()
        {
            m_primitive_manager = null;
        }

        //! this constructor doesn't build the tree. you must call	buildSet
        public GImpactBvh(IPrimitiveManagerBase primitive_manager)
        {
            m_primitive_manager = primitive_manager;
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
        public void Update()
        {
            Refit();
        }

        //! this rebuild the entire set
        public void BuildSet()
        {
            //obtain primitive boxes
            GIM_BVH_DATA_ARRAY primitive_boxes = new GIM_BVH_DATA_ARRAY();
            //primitive_boxes.resize(m_primitive_manager.get_primitive_count());
            primitive_boxes.Capacity = m_primitive_manager.GetPrimitiveCount();
            for (int i = 0; i < primitive_boxes.Count; i++)
            {
                m_primitive_manager.GetPrimitiveBox(i, out primitive_boxes.GetRawArray()[i].m_bound);
                primitive_boxes.GetRawArray()[i].m_data = i;
            }

            m_box_tree.BuildTree(primitive_boxes);

        }

        //! returns the indices of the primitives in the m_primitive_manager
        public bool BoxQuery(ref AABB box, ObjectArray<int> collided_results)
        {
            int curIndex = 0;
            int numNodes = GetNodeCount();

            while (curIndex < numNodes)
            {
                AABB bound = new AABB();
                GetNodeBound(curIndex, out bound);

                //catch bugs in tree data

                bool aabbOverlap = bound.HasCollision(ref box);
                bool isleafnode = IsLeafNode(curIndex);

                if (isleafnode && aabbOverlap)
                {
                    collided_results.Add(GetNodeData(curIndex));
                }

                if (aabbOverlap || isleafnode)
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

        //! returns the indices of the primitives in the m_primitive_manager
        public bool RayQuery(ref IndexedVector3 ray_dir, ref IndexedVector3 ray_origin,
            ObjectArray<int> collided_results)
        {
            int curIndex = 0;
            int numNodes = GetNodeCount();

            while (curIndex < numNodes)
            {
                AABB bound = new AABB();
                GetNodeBound(curIndex, out bound);

                //catch bugs in tree data

                bool aabbOverlap = bound.CollideRay(ref ray_origin, ref ray_dir);
                bool isleafnode = IsLeafNode(curIndex);

                if (isleafnode && aabbOverlap)
                {
                    collided_results.Add(GetNodeData(curIndex));
                }

                if (aabbOverlap || isleafnode)
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

        public int GetNodeData(int nodeindex)
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

        public void GetNodeTriangle(int nodeindex, PrimitiveTriangle triangle)
        {
            m_primitive_manager.GetPrimitiveTriangle(GetNodeData(nodeindex), triangle);
        }


        //public const GIM_BVH_TREE_NODE * get_node_pointer(int index = 0)
        //{
        //    return m_box_tree.get_node_pointer(index);
        //}


        public static float GetAverageTreeCollisionTime()
        {
            // no implementation?
            return 0.0f;
        }


        public static void FindCollision(GImpactBvh boxset1, ref IndexedMatrix trans1,
            GImpactBvh boxset2, ref IndexedMatrix trans2,
            PairSet collision_pairs)
        {
            if (boxset1.GetNodeCount() == 0 || boxset2.GetNodeCount() == 0) return;

            BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0 = new BT_BOX_BOX_TRANSFORM_CACHE();

            trans_cache_1to0.CalcFromHomogenic(ref trans1, ref trans2);

#if TRI_COLLISION_PROFILING
	bt_begin_gim02_tree_time();
#endif //TRI_COLLISION_PROFILING

            FindCollisionPairsRecursive(boxset1, boxset2, collision_pairs, trans_cache_1to0, 0, 0, true);
#if TRI_COLLISION_PROFILING
	bt_end_gim02_tree_time();
#endif //TRI_COLLISION_PROFILING


        }

        public static bool NodeCollision(
            GImpactBvh boxset0, GImpactBvh boxset1,
            ref BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0,
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

        public static void FindCollisionPairsRecursive(
        GImpactBvh boxset0, GImpactBvh boxset1,
        PairSet collision_pairs,
        BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0,
        int node0, int node1, bool complete_primitive_tests)
        {



            if (NodeCollision(
                boxset0, boxset1, ref trans_cache_1to0,
                node0, node1, complete_primitive_tests) == false) return;//avoid colliding internal nodes

            if (boxset0.IsLeafNode(node0))
            {
                if (boxset1.IsLeafNode(node1))
                {
                    // collision result
                    collision_pairs.PushPair(
                        boxset0.GetNodeData(node0), boxset1.GetNodeData(node1));
                    return;
                }
                else
                {

                    //collide left recursive

                    FindCollisionPairsRecursive(
                                        boxset0, boxset1,
                                        collision_pairs, trans_cache_1to0,
                                        node0, boxset1.GetLeftNode(node1), false);

                    //collide right recursive
                    FindCollisionPairsRecursive(
                                        boxset0, boxset1,
                                        collision_pairs, trans_cache_1to0,
                                        node0, boxset1.GetRightNode(node1), false);


                }
            }
            else
            {
                if (boxset1.IsLeafNode(node1))
                {

                    //collide left recursive
                    FindCollisionPairsRecursive(
                                        boxset0, boxset1,
                                        collision_pairs, trans_cache_1to0,
                                        boxset0.GetLeftNode(node0), node1, false);


                    //collide right recursive

                    FindCollisionPairsRecursive(
                                        boxset0, boxset1,
                                        collision_pairs, trans_cache_1to0,
                                        boxset0.GetRightNode(node0), node1, false);


                }
                else
                {
                    //collide left0 left1



                    FindCollisionPairsRecursive(
                        boxset0, boxset1,
                        collision_pairs, trans_cache_1to0,
                        boxset0.GetLeftNode(node0), boxset1.GetLeftNode(node1), false);

                    //collide left0 right1

                    FindCollisionPairsRecursive(
                        boxset0, boxset1,
                        collision_pairs, trans_cache_1to0,
                        boxset0.GetLeftNode(node0), boxset1.GetRightNode(node1), false);


                    //collide right0 left1

                    FindCollisionPairsRecursive(
                        boxset0, boxset1,
                        collision_pairs, trans_cache_1to0,
                        boxset0.GetRightNode(node0), boxset1.GetLeftNode(node1), false);

                    //collide right0 right1

                    FindCollisionPairsRecursive(
                        boxset0, boxset1,
                        collision_pairs, trans_cache_1to0,
                        boxset0.GetRightNode(node0), boxset1.GetRightNode(node1), false);

                }// else if node1 is not a leaf
            }// else if node0 is not a leaf
        }

    }

    public struct GIM_PAIR
    {
        public int m_index1;
        public int m_index2;

        public GIM_PAIR(ref GIM_PAIR p)
        {
            m_index1 = p.m_index1;
            m_index2 = p.m_index2;
        }

        public GIM_PAIR(int index1, int index2)
        {
            m_index1 = index1;
            m_index2 = index2;
        }
    }

    //! A pairset array
    public class PairSet : ObjectArray<GIM_PAIR>
    {
        public PairSet()
            : base(32)
        {

        }

        public void PushPair(int index1, int index2)
        {
            Add(new GIM_PAIR(index1, index2));
        }

        public void PushPairInv(int index1, int index2)
        {
            Add(new GIM_PAIR(index2, index1));
        }
    }

    public struct GIM_BVH_DATA
    {
        public AABB m_bound;
        public int m_data;
    }


    public class GIM_BVH_TREE_NODE
    {
        public AABB m_bound = new AABB();
        protected int m_escapeIndexOrDataIndex;

        public GIM_BVH_TREE_NODE()
        {
            m_escapeIndexOrDataIndex = 0;
        }

        public bool IsLeafNode()
        {
            //skipindex is negative (internal node), triangleindex >=0 (leafnode)
            return (m_escapeIndexOrDataIndex >= 0);
        }

        public int GetEscapeIndex()
        {
            //btAssert(m_escapeIndexOrDataIndex < 0);
            return -m_escapeIndexOrDataIndex;
        }

        public void SetEscapeIndex(int index)
        {
            m_escapeIndexOrDataIndex = -index;
        }

        public int GetDataIndex()
        {
            //btAssert(m_escapeIndexOrDataIndex >= 0);

            return m_escapeIndexOrDataIndex;
        }

        public void SetDataIndex(int index)
        {
            m_escapeIndexOrDataIndex = index;
        }

    }

    public class GIM_BVH_DATA_ARRAY : ObjectArray<GIM_BVH_DATA>
    {
        public GIM_BVH_DATA_ARRAY()
        { }

        public GIM_BVH_DATA_ARRAY(int reserve)
            : base(reserve)
        {

        }
    }


    public class GIM_BVH_TREE_NODE_ARRAY : ObjectArray<GIM_BVH_TREE_NODE>
    {
    }


    public class BvhTree
    {
        protected int m_num_nodes;
        protected GIM_BVH_TREE_NODE_ARRAY m_node_array = new GIM_BVH_TREE_NODE_ARRAY();
        protected int SortAndCalcSplittingIndex(
            GIM_BVH_DATA_ARRAY primitive_boxes,
             int startIndex, int endIndex, int splitAxis)
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
            means *= (1.0f) / (float)numIndices;

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
            means *= (1.0f) / (float)numIndices;

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

            if ((endIndex - startIndex) == 1)
            {
                //We have a leaf node
                SetNodeBound(curIndex, ref primitive_boxes.GetRawArray()[startIndex].m_bound);
                m_node_array[curIndex].SetDataIndex(primitive_boxes[startIndex].m_data);

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

            m_node_array[curIndex].SetEscapeIndex(m_num_nodes - curIndex);
        }

        public BvhTree()
        {
            m_num_nodes = 0;
        }

        //! prototype functions for box tree management
        //!@{
        public void BuildTree(GIM_BVH_DATA_ARRAY primitive_boxes)
        {
            // initialize node count to 0
            m_num_nodes = 0;
            // allocate nodes
            m_node_array.Capacity = (primitive_boxes.Count * 2);

            // check this- should be capacity or count??
            BuildSubTree(primitive_boxes, 0, primitive_boxes.Count);

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

        public int GetNodeData(int nodeindex)
        {
            return m_node_array[nodeindex].GetDataIndex();
        }

        public void GetNodeBound(int nodeindex, out AABB bound)
        {
            bound = m_node_array[nodeindex].m_bound;
        }

        public void SetNodeBound(int nodeindex, ref AABB bound)
        {
            m_node_array[nodeindex].m_bound = bound;
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

        //public const GIM_BVH_TREE_NODE * get_node_pointer(int index = 0)
        //{
        //    return &m_node_array[index];
        //}

        //!@}
    }

    public interface IPrimitiveManagerBase
    {
        void Cleanup();
        //! determines if this manager consist on only triangles, which special case will be optimized
        bool IsTrimesh();
        int GetPrimitiveCount();
        void GetPrimitiveBox(int prim_index, out AABB primbox);
        //! retrieves only the points of the triangle, and the collision margin
        void GetPrimitiveTriangle(int prim_index, PrimitiveTriangle triangle);
    }

}
