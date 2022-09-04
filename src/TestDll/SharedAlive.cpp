#include "SharedAlive.h"

using namespace TestDll;
using namespace std;

SharedAlive::SharedAlive(double a) { _a = a; }

int SharedAlive::Two() { return static_cast<int>(2 * _a); }

PointerDispose* TestDll::SharedAlive::MakeDispose() { return new PointerDispose(); }

shared_ptr<SharedAll> TestDll::SharedAlive::MakePrint(int i) { return make_shared<SharedAll>(i); }

void TestDll::SharedAlive::PrintTwice(std::shared_ptr<SharedAll> printer)
{
	printer->Print();
	printer->Print();
}

double TestDll::SharedAlive::HalfNine(PointerDelete* pDelete) { return pDelete->Half(static_cast<int>(pDelete->Nine())); }