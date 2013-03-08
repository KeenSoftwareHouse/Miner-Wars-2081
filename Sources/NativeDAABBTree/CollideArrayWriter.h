#pragma once
#include "btDbvt.h"

struct CollideArrayWriter: public btDbvt::ICollide
{
private:
	int* nodePointerArray;
	int offset;
	int offsetEnd;
	int count;

public:
	CollideArrayWriter(int* nodePointerArray, int offset, int count)
	{
		this->nodePointerArray = nodePointerArray;
		this->offset = offset;
		this->offsetEnd = offset + count;
		this->count = 0;
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
};

