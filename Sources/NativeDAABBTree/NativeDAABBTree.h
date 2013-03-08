// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the NATIVEDAABBTREE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// NATIVEDAABBTREE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef NATIVEDAABBTREE_EXPORTS
#define NATIVEDAABBTREE_API __declspec(dllexport)
#else
#define NATIVEDAABBTREE_API __declspec(dllimport)
#endif

#include "btDbvt.h"
#include "CollideArrayWriter.h"
#include "CollideArrayWriterSphere.h"

struct vector
{
	float x,y,z;
};

struct aabb
{
	vector min, max;
};

__forceinline btDbvtVolume makeVolume(const aabb& box)
{
	return btDbvtVolume::FromMM(btVector3(box.min.x, box.min.y, box.min.z), btVector3(box.max.x, box.max.y, box.max.z));
}

extern "C" NATIVEDAABBTREE_API btDbvt* Create();
extern "C" NATIVEDAABBTREE_API void Destroy(btDbvt* tree);
extern "C" NATIVEDAABBTREE_API void Clear(btDbvt* tree);
extern "C" NATIVEDAABBTREE_API btDbvtNode* Insert(btDbvt* tree, aabb& box, int data);
extern "C" NATIVEDAABBTREE_API void Remove(btDbvt* tree, btDbvtNode* node);
extern "C" NATIVEDAABBTREE_API void ChangeData(btDbvt* tree, btDbvtNode* node, int newData);
extern "C" NATIVEDAABBTREE_API int RemoveAndChangeData(btDbvt* tree, btDbvtNode* removedNode, btDbvtNode* changeDataNode);
extern "C" NATIVEDAABBTREE_API void Move(btDbvt* tree, btDbvtNode* node, aabb& box);
extern "C" NATIVEDAABBTREE_API void Move2(btDbvt* tree, btDbvtNode* node, aabb& box, vector& velocity);
extern "C" NATIVEDAABBTREE_API void Move3(btDbvt* tree, btDbvtNode* node, aabb& box, float margin);
extern "C" NATIVEDAABBTREE_API void Move4(btDbvt* tree, btDbvtNode* node, aabb& box, vector& velocity, float margin);
extern "C" NATIVEDAABBTREE_API int MaxDepth(btDbvt* tree, btDbvtNode* node);
extern "C" NATIVEDAABBTREE_API int CountLeaves(btDbvt* tree, btDbvtNode* node);
extern "C" NATIVEDAABBTREE_API bool IsEmpty(btDbvt* tree);
extern "C" NATIVEDAABBTREE_API void OptimizeBottomUp(btDbvt* tree);
extern "C" NATIVEDAABBTREE_API void OptimizeTopDown(btDbvt* tree, int bu_treshold);
extern "C" NATIVEDAABBTREE_API void OptimizeIncremental(btDbvt* tree, int passes);

// Gets all nodes, node pointers are written to [nodePointerArray] at [offset], no more than [count] nodes are written
// Return true on success, when array is too small, returns false
extern "C" NATIVEDAABBTREE_API int QueryAll(btDbvt* tree, int* nodePointerArray, int offset, int count);
extern "C" NATIVEDAABBTREE_API int QueryAABB(btDbvt* tree, aabb& box, int* nodePointerArray, int offset, int count);
extern "C" NATIVEDAABBTREE_API int QuerySixPlanes(btDbvt* tree, float* x, float* y, float* z, float* d, int* nodePointerArray, int offset, int count);
extern "C" NATIVEDAABBTREE_API int QuerySphere(btDbvt* tree, vector& sphereCenter, float sphereRadius, int* nodePointerArray, int offset, int count);
extern "C" NATIVEDAABBTREE_API int QueryRay(btDbvt* tree, vector& start, vector& dir, int* nodePointerArray, int offset, int count);
extern "C" NATIVEDAABBTREE_API int QueryRayNotThreadSafe(btDbvt* tree, vector& start, vector& dir, int* nodePointerArray, int offset, int count);
