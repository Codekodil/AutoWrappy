#pragma once

#include<string>

//WRAPPY_SHARED
//WRAPPY_DELETE
//WRAPPY_DISPOSE
//WRAPPY_OWNER

namespace TestDll
{
	class SharedAll
	{
	public:
		SharedAll(int i);
		void Print();
		void Write(char* l);
		void WriteString(std::string s);
		~SharedAll();
	private:
		int _i;
	};
}

