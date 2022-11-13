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

void PointerDispose::PointerValue(void* pointer)
{
	cout << "Pointer value [" << pointer << "]: " << *static_cast<int*>(pointer) << endl;
}
