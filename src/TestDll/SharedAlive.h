#pragma once

#include "PointerDispose.h"
#include "PointerDelete.h"
#include "SharedAll.h"
#include <memory>

//WRAPPY_SHARED
//WRAPPY_DELETE

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
		int(__stdcall* TwoCallback)(int two) = nullptr;
		PointerDispose* (__stdcall* MakeDisposeCallback)(PointerDispose* dispose) = nullptr;
		std::shared_ptr<SharedAll>(__stdcall* PrintTwiceCallback)(std::shared_ptr<SharedAll> printer) = nullptr;
	private:
		void Hidden() {}
		double _a;
	};
}