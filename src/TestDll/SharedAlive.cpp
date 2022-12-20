#include "SharedAlive.h"

using namespace TestDll;
using namespace std;

int SharedAliveBase::One()
{
	return 1;
}

SharedAlive::SharedAlive(double a) { _a = a; }

int SharedAlive::Two()
{
	auto value = static_cast<int>((One() + One()) * _a);
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

void SharedAlive::PrintTwice(std::shared_ptr<SharedAll> printer)
{
	auto callback = PrintTwiceCallback;
	if (callback)
		printer = callback(printer);
	printer->Print();
	printer->Print();
}

double SharedAlive::HalfNine(PointerDelete* pDelete) { return pDelete->Half(static_cast<float>(pDelete->Nine())); }

void SharedAlive::FillWithDispose(span<PointerDispose*> disposes)
{
	bool first = true;
	for (auto& dispose : disposes)
	{
		if (first)
		{
			first = false;
			dispose = nullptr;
			continue;
		}
		if (!dispose)
			dispose = new PointerDispose();
	}
}

void SharedAlive::FillWithPrint(span<shared_ptr<SharedAll>> printers)
{
	bool first = true;
	for (auto& printer : printers)
	{
		if (first)
		{
			first = false;
			printer = nullptr;
			continue;
		}
		if (!printer)
			printer = make_shared<SharedAll>(69);
	}
}