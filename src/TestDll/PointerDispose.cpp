#include "PointerDispose.h"

#include <iostream>

using namespace TestDll;
using namespace std;
using namespace glm;

int DisposeBase::Ten()
{
	return TenValue;
}

int PointerDispose::Five()
{
	TenValue = 10;
	auto callback = OnFive;
	if (callback)
		callback();
	return Ten() / 2;
}

void* PointerDispose::ThisPointer()
{
	return &OnFive;
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

void PointerDispose::Rotate(span<vec3> points, quat rotation)
{
	auto transform = glm::toMat3(rotation);
	for (auto& point : points)
		point = transform * point;
}