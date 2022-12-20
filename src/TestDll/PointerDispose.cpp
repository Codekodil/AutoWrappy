#include "PointerDispose.h"

#include <iostream>

using namespace TestDll;
using namespace std;
using namespace glm;

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

float PointerDispose::Sum(vec3 vec)
{
	return dot(vec, vec3(1.0));
}

void PointerDispose::Normalice(span<vec2> vecs)
{
	for (auto& vec : vecs)
		vec = normalize(vec);
}

void PointerDispose::Transform(span<vec4> vecs, mat4 transform)
{
	for (auto& vec : vecs)
		vec = transform * vec;
}