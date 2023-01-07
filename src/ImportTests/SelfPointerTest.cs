using TestDll;

namespace ImportTests
{
	[TestClass]
	public class SelfPointerTest
	{
		[TestMethod]
		public void Pointer()
		{
			var obj = new PointerDelete();
			Assert.AreEqual(obj.Native!.Value, obj.GetAddress());
		}
		[TestMethod]
		public void Shared()
		{
			var obj = new SelfPointer();
			Assert.AreEqual(obj.GetThis(), obj.GetAddress());
		}
	}
}