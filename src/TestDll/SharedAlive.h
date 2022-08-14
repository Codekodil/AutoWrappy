#pragma once

#include "PointerDispose.h"
#include "PointerDelete.h"
#include "SharedAll.h"
#include <memory>

//WRAPPY_SHARED

namespace TestDll
{
	class SharedAlive
	{
	public:
		SharedAlive(double a);
		int Two();
		PointerDispose* MakeDispose();
		std::shared_ptr<SharedAll> MakePrint(int i);
		void PrintTwice(std::shared_ptr<SharedAll> printer);
		double HalfNine(PointerDelete* pDelete);
	private:
		void Hidden() {}
		double _a;
	};
}