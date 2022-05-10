using System;
using BulletSharp;
using BulletSharp.Math;
using BulletSharpExamples;
using DemoFramework;

namespace CollisionInterfaceDemo
{
	internal class DrawingResult : ContactResultCallback
	{
		private DynamicsWorld world;
		public DrawingResult(DynamicsWorld world)
		{
			this.world = world;
		}

		public override float AddSingleResult(ManifoldPoint cp,
			CollisionObjectWrapper colObj0Wrap, int partId0, int index0,
			CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
		{
			var ptA = cp.PositionWorldOnA;
			var ptB = cp.PositionWorldOnB;
			UnityEngine.Debug.Log("Contact!");
			//world.DebugDrawer.DrawLine(ref ptA, ref ptB, ref ptA);
			UnityEngine.Debug.LogFormat("{0}, {1} {2}", ptA, ptB, world);
			return 0;
		}
	};

	internal class CollisionInterfaceDemo : Demo
	{
		private Vector3 eye = new Vector3(6, 4, 1);
		private Vector3 target = new Vector3(0, 3, 0);
		private CollisionObject[] objects = new CollisionObject[2];
		private DrawingResult renderCallback;

		//Vector3 boxMin = new Vector3(-1, -1, -1);
		//Vector3 boxMax = new Vector3(1, 1, 1);
		//Vector3 white = new Vector3(1, 1, 1);

		protected override void OnInitialize()
		{
			Freelook.SetEyeTarget(eye, target);

			Graphics.SetFormText("BulletSharp - Collision Interface Demo");
			Graphics.SetInfoText("Move using mouse and WASD+shift\n" +
				"F3 - Toggle debug\n" +
				//"F11 - Toggle fullscreen\n" +
				"Space - Shoot box");

			IsDebugDrawEnabled = false;
		}

		protected override void OnInitializePhysics()
		{
			// collision configuration contains default setup for memory, collision setup
			CollisionConf = new DefaultCollisionConfiguration();
			Dispatcher = new CollisionDispatcher(CollisionConf);

			Broadphase = new AxisSweep3(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000));

			World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf)
			{
				Gravity = new Vector3(0, -10, 0)
			};

			renderCallback = new DrawingResult(World);


			var boxA = new BoxShape(new Vector3(1, 1, 1))
			{
				Margin = 0
			};

			var boxB = new BoxShape(new Vector3(0.5f, 0.5f, 0.5f))
			{
				Margin = 0
			};

			CollisionShapes.Add(boxA);
			CollisionShapes.Add(boxB);

			objects[0] = new CollisionObject();
			objects[1] = new CollisionObject();

			objects[0].CollisionShape = boxA;
			objects[1].CollisionShape = boxB;

			World.AddCollisionObject(objects[0]);
			World.AddCollisionObject(objects[1]);

			var rotA = new Quaternion(0.739f, -0.204f, 0.587f, 0.257f);
			rotA.Normalize();

			objects[0].WorldTransform = Matrix.RotationQuaternion(rotA) * Matrix.Translation(0, 3, 0);
			objects[1].WorldTransform = Matrix.Translation(0, 4.248f, 0);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			var t = objects[0].WorldTransform;
			var pos = t.Row4;
			t.Row4 = new Vector4(0, 0, 0, 1);
			t *= Matrix.RotationYawPitchRoll(0.1f * FrameDelta, 0.05f * FrameDelta, 0);
			t.Row4 = pos;
			objects[0].WorldTransform = t;
			World.ContactTest(objects[0], renderCallback);

			if (IsDebugDrawEnabled)
			{
				//World.DebugDrawer.DrawBox(ref boxMin, ref boxMax, ref t, ref white);
				World.ContactTest(objects[0], renderCallback);
			}
		}
	}

	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			using (Demo demo = new CollisionInterfaceDemo())
			{
				GraphicsLibraryManager.Run(demo);
			}
		}
	}
}
