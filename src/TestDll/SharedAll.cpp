#include "SharedAll.h"

#include <iostream>

using namespace TestDll;
using namespace std;

SharedAll::SharedAll(int i)
{
	_i = i;
}

void SharedAll::Print()
{
	cout << "Hoi " << _i << endl;
}

void SharedAll::Write(span<char> l)
{
	cout << l.data() << " " << l.size() << " " << _i << endl;
}

void SharedAll::WriteString(std::string s)
{
	cout << s << " " << _i << endl;
}

SharedAll::~SharedAll()
{
	cout << "Bye " << _i << endl;
}