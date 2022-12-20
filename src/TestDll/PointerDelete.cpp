#include "PointerDelete.h"

using namespace TestDll;
using namespace std;

void PointerDelete::Nothing() {}

int PointerDelete::Nine()
{
	return 9;
}

float PointerDelete::Half(float a)
{
	return a * 0.5f;
}

double PointerDelete::Add(int l, double r)
{
	return l + r;
}

int PointerDelete::Sum(span<int> n)
{
	int r = 0;
	for (auto& i : n)
		r += i;
	return r;
}