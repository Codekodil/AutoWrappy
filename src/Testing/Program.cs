using TestDll;

var pde = new PointerDelete();
Console.WriteLine($"4.55: " + pde.Half((float)pde.Add(pde.Nine(), .1)));

var a = new[] { 1, 2, 3, 4, 5 };
Console.WriteLine($"15: " + pde.Sum(a, a.Length));

using (var pdi = new PointerDispose())
{
	Console.WriteLine($"5: " + pdi.Five());
	pdi.PointerValue(pdi.ThisPointer());
}

var sa = new SharedAlive(4);
sa.TwoCallback += a => a + 7;
Console.WriteLine($"15: " + sa.Two());
using (var made = sa.MakeDispose())
{
	made.OnFive += () => Console.WriteLine($"OnFive");
	Console.WriteLine($"5: " + made.Five());
}
using (var made = sa.MakePrint(10))
{
	made.Print();
}

using (var printer = new SharedAll(4))
{
	sa.PrintTwice(printer);
	var bytes = new byte[] { (byte)'O', (byte)'h', 0 };
	printer.Write(bytes);
	printer.WriteString("Wubwub");
}
using (var printer = new SharedAll(2))
{
	printer.Owner = false;
	printer.Print();
}

Console.WriteLine($"4.5: " + sa.HalfNine(pde));