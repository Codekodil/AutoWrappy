using TestDll;

namespace ImportTests
{
	[TestClass]
	public class SharedAliveTest
	{
		[TestMethod]
		public void Double()
		{
			var obj = new SharedAlive(3);
			Assert.AreEqual(6, obj.Two());
		}

		[TestMethod]
		public void DoubleCallback()
		{
			var callbackCount = 0;

			var obj = new SharedAlive(10);
			obj.TwoCallback += a =>
			{
				callbackCount++;
				return a + 1;
			};
			Assert.AreEqual(21, obj.Two());
			Assert.AreEqual(1, callbackCount);
		}

		[TestMethod]
		public void MakeDisposeCallback()
		{
			var callbackCount = 0;

			var dispose = new PointerDispose();
			var obj = new SharedAlive(10);
			obj.MakeDisposeCallback += d =>
			{
				callbackCount++;
				d.Dispose();
				return dispose;
			};
			using (var subObj = obj.MakeDispose())
			{
				subObj.OnFive += () =>
				{
					callbackCount++;
				};
				subObj.Five();
				subObj.Five();

				Assert.AreEqual(dispose.Native, subObj.Native);
			}
			Assert.AreEqual(3, callbackCount);
		}
	}
}