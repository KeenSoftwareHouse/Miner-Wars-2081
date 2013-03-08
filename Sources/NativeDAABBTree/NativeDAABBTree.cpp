// NativeDAABBTree.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "NativeDAABBTree.h"


NATIVEDAABBTREE_API btDbvt* Create()
{
	return new btDbvt();
}

NATIVEDAABBTREE_API void Destroy(btDbvt* tree)
{
	delete tree;
}

NATIVEDAABBTREE_API void Clear(btDbvt* tree)
{
	tree->clear();
}

NATIVEDAABBTREE_API btDbvtNode* Insert(btDbvt* tree, aabb& box, int data)
{
	return tree->insert(makeVolume(box), (void*)data);
}

NATIVEDAABBTREE_API void Remove(btDbvt* tree, btDbvtNode* node)
{
	tree->remove(node);
}

NATIVEDAABBTREE_API void ChangeData(btDbvt* tree, btDbvtNode* node, int newData)
{
	node->dataAsInt = newData;
}

NATIVEDAABBTREE_API int RemoveAndChangeData(btDbvt* tree, btDbvtNode* removedNode, btDbvtNode* changeDataNode)
{
	changeDataNode->dataAsInt = removedNode->dataAsInt;
	tree->remove(removedNode);
	return changeDataNode->dataAsInt;
}

NATIVEDAABBTREE_API void Move(btDbvt* tree, btDbvtNode* node, aabb& box)
{
	tree->update(node, makeVolume(box));
}

NATIVEDAABBTREE_API void Move2(btDbvt* tree, btDbvtNode* node, aabb& box, vector& velocity)
{
	btVector3 vel(velocity.x, velocity.y, velocity.z);
	tree->update(node, makeVolume(box), vel);
}

NATIVEDAABBTREE_API void Move3(btDbvt* tree, btDbvtNode* node, aabb& box, float margin)
{
	tree->update(node, makeVolume(box), margin);
}

NATIVEDAABBTREE_API void Move4(btDbvt* tree, btDbvtNode* node, aabb& box, vector& velocity, float margin)
{
	btVector3 vel(velocity.x, velocity.y, velocity.z);
	tree->update(node, makeVolume(box), vel, margin);
}

NATIVEDAABBTREE_API int MaxDepth(btDbvt* tree, btDbvtNode* node)
{
	return tree->maxdepth(node);
}

NATIVEDAABBTREE_API int CountLeaves(btDbvt* tree, btDbvtNode* node)
{
	return tree->countLeaves(node);
}

NATIVEDAABBTREE_API bool IsEmpty(btDbvt* tree)
{
	return tree->empty();
}

NATIVEDAABBTREE_API void OptimizeBottomUp(btDbvt* tree)
{
	tree->optimizeBottomUp();
}

NATIVEDAABBTREE_API void OptimizeTopDown(btDbvt* tree, int bu_treshold)
{
	tree->optimizeTopDown(bu_treshold);
}

NATIVEDAABBTREE_API void OptimizeIncremental(btDbvt* tree, int passes)
{
	tree->optimizeIncremental(passes);
}

// Gets all nodes, node pointers are written to [nodePointerArray] at [offset], no more than [count] nodes are written
// Return true on success, when array is too small, returns false
NATIVEDAABBTREE_API int QueryAll(btDbvt* tree, int* nodePointerArray, int offset, int count)
{
	CollideArrayWriter result(nodePointerArray, offset, count);
	tree->enumLeaves(tree->m_root, result);
	return result.GetResult();
}

NATIVEDAABBTREE_API int QueryAABB(btDbvt* tree, aabb& box, int* nodePointerArray, int offset, int count)
{
	CollideArrayWriter result(nodePointerArray, offset, count);
	tree->collideTV(tree->m_root, makeVolume(box), result);
	return result.GetResult();
}

NATIVEDAABBTREE_API int QuerySixPlanes(btDbvt* tree, float* x, float* y, float* z, float* offsets, int* nodePointerArray, int offset, int count)
{
	btVector3 normals[6];
	for(int i = 0; i < 6; i++)
	{
		normals[i] = btVector3(x[i], y[i], z[i]);
	}

	CollideArrayWriter result(nodePointerArray, offset, count);
	tree->collideKDOP(tree->m_root, normals, offsets, 6, result);

	return result.GetResult();
}

NATIVEDAABBTREE_API int QuerySphere(btDbvt* tree, vector& sphereCenter, float sphereRadius, int* nodePointerArray, int offset, int count)
{
	btVector3 center(sphereCenter.x, sphereCenter.y, sphereCenter.z);
	CollideArrayWriterSphere result(nodePointerArray, offset, count, center, sphereRadius);
	tree->collideTU(tree->m_root, result);
	return result.GetResult();
}

NATIVEDAABBTREE_API int QueryRay(btDbvt* tree, vector& from, vector& to, int* nodePointerArray, int offset, int count)
{
	btVector3 btFrom(from.x, from.y, from.z);
	btVector3 btTo(to.x, to.y, to.z);

	CollideArrayWriter result(nodePointerArray, offset, count);
	tree->rayTest(tree->m_root, btFrom, btTo, result);
	return result.GetResult();
}

NATIVEDAABBTREE_API int QueryRayNotThreadSafe(btDbvt* tree, vector& from, vector& to, int* nodePointerArray, int offset, int count)
{
	btVector3 rayFrom(from.x, from.y, from.z);
	btVector3 rayTo(to.x, to.y, to.z);

	btVector3 rayDir = (rayTo-rayFrom);
	rayDir.normalize ();

	btVector3 zero(0,0,0);

	btVector3 rayDirectionInverse;
	rayDirectionInverse[0] = rayDir[0] == btScalar(0.0) ? btScalar(BT_LARGE_FLOAT) : btScalar(1.0) / rayDir[0];
	rayDirectionInverse[1] = rayDir[1] == btScalar(0.0) ? btScalar(BT_LARGE_FLOAT) : btScalar(1.0) / rayDir[1];
	rayDirectionInverse[2] = rayDir[2] == btScalar(0.0) ? btScalar(BT_LARGE_FLOAT) : btScalar(1.0) / rayDir[2];
	unsigned int signs[3] = { rayDirectionInverse[0] < 0.0, rayDirectionInverse[1] < 0.0, rayDirectionInverse[2] < 0.0};

	btScalar lambda_max = rayDir.dot(rayTo-rayFrom);

	CollideArrayWriter result(nodePointerArray, offset, count);
	tree->rayTestInternal(tree->m_root, rayFrom, rayTo, rayDirectionInverse, signs, lambda_max, zero, zero, result);
	return result.GetResult();
}