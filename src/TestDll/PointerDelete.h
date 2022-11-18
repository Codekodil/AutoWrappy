#pragma once

#include <span>

//WRAPPY_POINTER
//WRAPPY_DELETE

namespace TestDll
{
	class PointerDelete
	{
	public:
		void Nothing();
		int Nine();
		float Half(float a);
		double Add(int l, double r);
		int Sum(std::span<int> n);
	};
}