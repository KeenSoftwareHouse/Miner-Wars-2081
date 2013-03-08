#pragma once
#include "btdbvt.h"

class CollideArrayWriterSphere: public btDbvt::ICollide
{
private:
	int* nodePointerArray;
	int offset;
	int offsetEnd;
	int count;

	btVector3 center;
	btScalar radius2;

public:
	CollideArrayWriterSphere(int* nodePointerArray, int offset, int count, btVector3& sphereCenter, btScalar sphereRadius)
	{
		this->nodePointerArray = nodePointerArray;
		this->offset = offset;
		this->offsetEnd = offset + count;
		this->count = 0;
		this->center = sphereCenter;
		this->radius2 = sphereRadius * sphereRadius;
	}

	__forceinline int GetResult()
	{
		return count;
	}

	void Process(const btDbvtNode* left, const btDbvtNode* right)
	{
		// ??
	}

	void Process(const btDbvtNode* node)
	{
		if(offset < offsetEnd)
		{
			nodePointerArray[offset] = node->dataAsInt;
			offset++;
			count++;
		}
		else
		{
			this->count = -1;
		}
	}

	bool Descent(const btDbvtNode* node)
	{
		btVector3 clamp = center;
		clamp.setMin(node->volume.Mins());
		clamp.setMax(node->volume.Maxs());

		btScalar distance2 = clamp.distance2(center);
		return distance2 <= radius2;
	}
};

