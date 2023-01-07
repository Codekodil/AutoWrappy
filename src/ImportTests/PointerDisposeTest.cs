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
				var vecs = new TestVec2[3];
				vecs[0].A = 2;
				vecs[1].B = .5f;
				vecs[2].A = -1;
				vecs[2].B = -1;
				obj.Normalice(vecs);
				Assert.AreEqual(1, vecs[0].A);
				Assert.AreEqual(0, vecs[0].B);
				Assert.AreEqual(0, vecs[1].A);
				Assert.AreEqual(1, vecs[1].B);
				Assert.AreEqual(-.7, vecs[2].A, .1);
				Assert.AreEqual(-.7, vecs[2].B, .1);
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
		public void GlmQuaternion()
		{
			using (var obj = new PointerDispose())
			{
				var vecs = new Vector3[4];
				vecs[1].X = 1;
				vecs[2].Y = 2;
				vecs[3].Z = 3;

				var rotation = Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1, 1, 1)), MathF.PI * 2f / 3f);
				var mappedRotation = new Vector4(rotation.X, rotation.Y, rotation.Z, rotation.W);

				obj.Rotate(vecs, mappedRotation);

				Assert.AreEqual(new Vector3(0), vecs[0]);
				Assert.AreEqual(0, (vecs[1] - new Vector3(0, 1, 0)).Length(), .1);
				Assert.AreEqual(0, (vecs[2] - new Vector3(0, 0, 2)).Length(), .1);
				Assert.AreEqual(0, (vecs[3] - new Vector3(3, 0, 0)).Length(), .1);
			}
		}

		[TestMethod]
		public void BaseMethodInvoke()
		{
			using (var obj = new PointerDispose())
			{
				var objBase = obj.AsDisposeBase();

				Assert.AreEqual(1, objBase.Ten());
			}
		}
	}
}