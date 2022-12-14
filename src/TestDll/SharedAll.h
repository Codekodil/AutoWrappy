#pragma once

#include<string>
#include<span>

//WRAPPY_SHARED
//WRAPPY_DELETE
//WRAPPY_DISPOSE
//WRAPPY_OWNER

namespace TestDll
{
	struct Ignore //WRAPPY_IGNORE
	{};
	template<typename T>
	struct Base //WRAPPY_IGNORE
	{};

	class SharedAll :public Base<Ignore>
	{
	public:
		SharedAll(int i);
		void Print();
		void Write(std::span<char> l);
		void WriteString(std::string s);
		~SharedAll();
	private:
		int _i;
	};
}

