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
				d!.Dispose();
				return dispose;
			};
			using (var subObj = obj.MakeDispose()!)
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

		[TestMethod]
		public void FillWithDispose()
		{
			var obj = new SharedAlive(10);

			var d1 = new PointerDispose();
			var d2 = new PointerDispose();

			var disposes = new[] { d1, d2, null, null };

			obj.FillWithDispose(disposes);

			Assert.AreEqual(null, disposes[0]);
			Assert.AreEqual(d2, disposes[1]);
			Assert.AreNotEqual(null, disposes[2]);
			Assert.AreNotEqual(null, disposes[3]);

			foreach (var d in disposes.Concat(new[] { d1, d2 }).Distinct())
				d?.Dispose();
		}

		[TestMethod]
		public void FillWithPrint()
		{
			var obj = new SharedAlive(10);

			var p1 = new SharedAll(1);
			var p2 = new SharedAll(2);

			var printers = new[] { p1, p2, null, null };

			obj.FillWithPrint(printers);

			Assert.AreEqual(null, printers[0]);
			Assert.AreEqual(p2, printers[1]);
			Assert.AreNotEqual(null, printers[2]);
			Assert.AreNotEqual(null, printers[3]);

			foreach (var d in printers.Concat(new[] { p1, p2 }).Distinct())
				d?.Dispose();
		}
	}
}