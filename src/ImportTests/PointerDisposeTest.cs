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
				Assert.AreEqual(new Vector2(1, 0), vecs[0]);
				Assert.AreEqual(new Vector2(0, 1), vecs[1]);
				Assert.AreEqual(-.7, vecs[2].X, .1);
				Assert.AreEqual(-.7, vecs[2].Y, .1);
			}
		}

		[TestMethod]
		public void GlmMatrix()
		{
			using (var obj = new PointerDispose())
			{
				var vecs = new Vector4[4];
				vecs[1].X = 2;
				vecs[2].Y = 2;
				vecs[3].Z = 2;
				for (int i = 0; i < vecs.Length; i++)
					vecs[i].W = 1;

				var transform = Matrix4x4.CreateRotationY(MathF.PI * .5f) * Matrix4x4.CreateScale(.5f, 1, 2) * Matrix4x4.CreateTranslation(10, 20, 30);

				obj.Transform(vecs, transform);

				Assert.AreEqual(new Vector4(10, 20, 30, 1), vecs[0]);
				Assert.AreEqual(new Vector4(10, 20, 26, 1), vecs[1]);
				Assert.AreEqual(new Vector4(10, 22, 30, 1), vecs[2]);
				Assert.AreEqual(new Vector4(11, 20, 30, 1), vecs[3]);
			}
		}

		[TestMethod]
		public void BaseSameAddress()
		{
			using (var obj = new PointerDispose())
			{
				var objBase = obj.AsDisposeBase();

				Assert.AreEqual(obj.Native, objBase.Native);
			}
		}
	}
}