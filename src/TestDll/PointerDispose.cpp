#include "PointerDispose.h"

#include <iostream>

using namespace TestDll;
using namespace std;

int PointerDispose::Five()
{
	auto callback = OnFive;
	if (callback)
		callback();
	return 5;
}

void* PointerDispose::ThisPointer()
{
	return this;
}

int PointerDispose::PointerValue(void* pointer)
{
	return *reinterpret_cast<int*>(pointer);
}
