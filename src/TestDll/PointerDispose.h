#pragma once

//WRAPPY_POINTER
//WRAPPY_DISPOSE

namespace TestDll
{
	struct PointerDispose
	{
		int Five();
		void* ThisPointer();
		int PointerValue(void* pointer);
		void(__stdcall* OnFive)();
	};
}

