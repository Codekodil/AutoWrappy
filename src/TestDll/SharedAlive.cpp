#include "SharedAlive.h"

using namespace TestDll;
using namespace std;

SharedAlive::SharedAlive(double a) { _a = a; }

int SharedAlive::Two()
{
	auto value = static_cast<int>(2 * _a);
	auto callback = TwoCallback;
	return callback ? callback(value) : value;
}

PointerDispose* TestDll::SharedAlive::MakeDispose()
{
	auto value = new PointerDispose();
	auto callback = MakeDisposeCallback;
	return callback ? callback(value) : value;
}

shared_ptr<SharedAll> TestDll::SharedAlive::MakePrint(int i) { return make_shared<SharedAll>(i); }

void TestDll::SharedAlive::PrintTwice(std::shared_ptr<SharedAll> printer)
{
	auto callback = PrintTwiceCallback;
	if (callback)
		printer = callback(printer);
	printer->Print();
	printer->Print();
}

double TestDll::SharedAlive::HalfNine(PointerDelete* pDelete) { return pDelete->Half(static_cast<int>(pDelete->Nine())); }