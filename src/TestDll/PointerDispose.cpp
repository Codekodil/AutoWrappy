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

float TestDll::PointerDispose::Sum(glm::vec3 vec)
{
	return glm::dot(vec, glm::vec3(1.0));
}

void TestDll::PointerDispose::Normalice(span<glm::vec2> vecs)
{
	for (auto& vec : vecs)
		vec = glm::normalize(vec);
}