#pragma once

//WRAPPY_SHARED
//WRAPPY_DELETE

namespace TestDll
{
	class SelfPointer
	{
	public:
		void* GetThis() { return this; }
	};
}

