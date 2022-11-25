using TestDll;

namespace ImportTests
{
	[TestClass]
	public class PointerDeleteTest
	{
		[TestMethod]
		public void Nine()
		{
			var obj = new PointerDelete();
			Assert.AreEqual(9, obj.Nine());
		}

		[TestMethod]
		public void Add()
		{
			var obj = new PointerDelete();
			Assert.AreEqual(.01, obj.Add(-10000, 10000.01), 0.00001);
		}

		[TestMethod]
		public void Half()
		{
			var obj = new PointerDelete();
			Assert.AreEqual(49.5f, obj.Half(99f));
		}

		[DataRow(new[] { 123 }, 123)]
		[DataRow(new[] { 1, 2, 3, 4, 5 }, 15)]
		[DataRow(new int[0], 0)]
		[DataTestMethod]
		public void Sum(int[] a, int result)
		{
			var obj = new PointerDelete();
			Assert.AreEqual(result, obj.Sum(a));
		}
	}
}