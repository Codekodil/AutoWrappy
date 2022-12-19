using System.Numerics;
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

		[TestMethod]
		public void GlmVector()
		{
			using (var obj = new PointerDispose())
			{
				var vec = new Vector3(1, 2, 3);
				Assert.AreEqual(6, obj.Sum(vec));
			}
		}

		[TestMethod]
		public void GlmVectorSpan()
		{
			using (var obj = new PointerDispose())
			{
				var vecs = new Vector2[3];
				vecs[0].X = 2;
				vecs[1].Y = .5f;
				vecs[2].X = -1;
				vecs[2].Y = -1;
				obj.Normalice(vecs);
				///Assert.AreEqual(new Vector2(1, 0), vecs[0]);
				///Assert.AreEqual(new Vector2(0, 1), vecs[1]);
				///Assert.AreEqual(new Vector2(1, 1), vecs[2]);
			}
		}
	}
}