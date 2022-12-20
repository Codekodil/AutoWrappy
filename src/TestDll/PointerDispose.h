#pragma once

//WRAPPY_POINTER
//WRAPPY_DISPOSE

#include<span>
#include<glm/glm.hpp>
#include<glm/gtc/quaternion.hpp>

namespace TestDll
{
	struct DisposeBase
	{
		int Ten();
	};

	struct PointerDispose :public DisposeBase
	{
		int Five();
		void* ThisPointer();
		int PointerValue(void* pointer);
		void(__stdcall* OnFive)();
		float Sum(glm::vec3 vec);
		void Normalice(std::span<glm::vec2> vecs);
		void Transform(std::span<glm::vec4> vecs, glm::mat4 transform);
	};
}

