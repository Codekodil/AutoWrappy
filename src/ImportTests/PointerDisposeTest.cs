using System.Text;
using TestDll;

namespace ImportTests
{
	[TestClass]
	public class PointerDisposeTest
	{
		[TestMethod]
		public void Five()
		{
			var obj = new PointerDispose();
			using (obj)
			{
				Assert.AreEqual(5, obj.Five());
				Assert.AreNotEqual(null, obj.Native);
			}
			Assert.AreEqual(null, obj.Native);
			Assert.ThrowsException<ObjectDisposedException>(() => obj.Five());
		}

		[TestMethod]
		public void Pointer()
		{
			using (var obj = new PointerDispose())
			{
				var pointer = obj.ThisPointer();
				Assert.AreEqual(0, obj.PointerValue(pointer));
				obj.OnFive += () => { };
				Assert.AreNotEqual(0, obj.PointerValue(pointer));
			}
		}
	}
}