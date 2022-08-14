#include "PointerDelete.h"

using namespace TestDll;

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

int TestDll::PointerDelete::Sum(int* n, int s)
{
	int r = 0;
	for (int i = 0; i < s; ++i)
		r += n[i];
	return r;
}