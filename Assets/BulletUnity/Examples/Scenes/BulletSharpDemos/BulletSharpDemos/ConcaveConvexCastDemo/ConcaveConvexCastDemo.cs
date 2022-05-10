using System;
using System.IO;
using System.Runtime.InteropServices;
using BulletSharp;
using BulletSharp.Math;
using BulletSharpExamples;
using DemoFramework;

namespace ConcaveConvexCastDemo
{
	// Scrolls back and forth over terrain
	internal class ConvexcastBatch
	{
		private const int NUMRAYS_IN_BAR = 100;
		private Vector3[] source = new Vector3[NUMRAYS_IN_BAR];
		private Vector3[] dest = new Vector3[NUMRAYS_IN_BAR];
		private Vector3[] direction = new Vector3[NUMRAYS_IN_BAR];
		private Vector3[] hit_com = new Vector3[NUMRAYS_IN_BAR];
		private Vector3[] hit_surface = new Vector3[NUMRAYS_IN_BAR];
		private float[] hit_fraction = new float[NUMRAYS_IN_BAR];
		private Vector3[] normal = new Vector3[NUMRAYS_IN_BAR];
		private int frame_counter;
		private int ms;
		private int sum_ms;
		private int sum_ms_samples;
		private int min_ms;
		private int max_ms;
		private float dx;
		private float min_x;
		private float max_x;
		private float min_y;
		private float max_y;
		private float sign;
		private Vector3 boxShapeHalfExtents;
		private BoxShape boxShape;

		public ConvexcastBatch()
		{
			boxShape = new BoxShape(0);
			ms = 0;
			max_ms = 0;
			min_ms = 9999;
			sum_ms_samples = 0;
			sum_ms = 0;
		}

		public ConvexcastBatch(bool unused, float ray_length, float min_z, float max_z, float min_y, float max_y)
		{
			boxShapeHalfExtents = new Vector3(1.0f, 1.0f, 1.0f);
			boxShape = new BoxShape(boxShapeHalfExtents);
			frame_counter = 0;
			ms = 0;
			max_ms = 0;
			min_ms = 9999;
			sum_ms_samples = 0;
			sum_ms = 0;
			dx = 10.0f;
			min_x = -40;
			max_x = 20;
			this.min_y = min_y;
			this.max_y = max_y;
			sign = 1.0f;
			//const float dalpha = 4 * (float)Math.PI / NUMRAYS_IN_BAR;
			for (int i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				float z = (max_z - min_z) / NUMRAYS_IN_BAR * i + min_z;
				source[i] = new Vector3(min_x, this.max_y, z);
				dest[i] = new Vector3(min_x + ray_length, this.min_y, z);
				normal[i] = new Vector3(1.0f, 0.0f, 0.0f);
			}
		}

		public ConvexcastBatch(float ray_length, float z, float min_y = -1000, float max_y = 10)
		{
			boxShapeHalfExtents = new Vector3(1.0f, 1.0f, 1.0f);
			boxShape = new BoxShape(boxShapeHalfExtents);
			frame_counter = 0;
			ms = 0;
			max_ms = 0;
			min_ms = 9999;
			sum_ms_samples = 0;
			sum_ms = 0;
			dx = 10.0f;
			min_x = -40;
			max_x = 20;
			this.min_y = min_y;
			this.max_y = max_y;
			sign = 1.0f;
			const float dalpha = 4 * (float)Math.PI / NUMRAYS_IN_BAR;
			for (int i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				float alpha = dalpha * i;
				// rotate around by alpha degrees y
				var tr = Matrix.RotationQuaternion(Quaternion.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), alpha));
				direction[i] = new Vector3(1.0f, 0.0f, 0.0f);
				direction[i] = Vector3.TransformCoordinate(direction[i], tr);
				source[i] = new Vector3(min_x, this.max_y, z);
				dest[i] = source[i] + direction[i] * ray_length;
				dest[i][1] = this.min_y;
				normal[i] = new Vector3(1.0f, 0.0f, 0.0f);
			}
		}

		public void Move(float dt)
		{
			if (dt > (1.0f / 60.0f))
			{
				dt = 1.0f / 60.0f;
			}

			for (int i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				source[i][0] += dx * dt * sign;
				dest[i][0] += dx * dt * sign;
			}
			if (source[0][0] < min_x)
			{
				sign = 1.0f;
			}
			else if (source[0][0] > max_x)
			{
				sign = -1.0f;
			}
		}

		public void Cast(CollisionWorld cw)
		{
			var zero = Vector3.Zero;
			var cb = new ClosestConvexResultCallback(ref zero, ref zero);
			for (int i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				cb.ClosestHitFraction = 1.0f;
				cb.ConvexFromWorld = source[i];
				cb.ConvexToWorld = dest[i];

				var qFrom = Quaternion.RotationAxis(new Vector3(1.0f, 0.0f, 0.0f), 0.0f);
				var qTo = Quaternion.RotationAxis(new Vector3(1.0f, 0.0f, 0.0f), 0.7f);
				var from = Matrix.RotationQuaternion(qFrom) * Matrix.Translation(source[i]);
				var to = Matrix.RotationQuaternion(qTo) * Matrix.Translation(dest[i]);
				cw.ConvexSweepTest(boxShape, from, to, cb);
				if (cb.HasHit)
				{
					hit_surface[i] = cb.HitPointWorld;
					hit_com[i] = Vector3.Lerp(source[i], dest[i], cb.ClosestHitFraction);
					hit_fraction[i] = cb.ClosestHitFraction;
					normal[i] = cb.HitNormalWorld;
					normal[i].Normalize();
				}
				else
				{
					hit_com[i] = dest[i];
					hit_surface[i] = dest[i];
					hit_fraction[i] = 1.0f;
					normal[i] = new Vector3(1.0f, 0.0f, 0.0f);
				}
			}

			frame_counter++;
			if (frame_counter > 50)
			{
				min_ms = ms < min_ms ? ms : min_ms;
				max_ms = ms > max_ms ? ms : max_ms;
				sum_ms += ms;
				sum_ms_samples++;
				float mean_ms = sum_ms / (float)sum_ms_samples;
				Console.WriteLine("{0} rays in {1} ms {2} {3} {4}", NUMRAYS_IN_BAR * frame_counter, ms, min_ms, max_ms, mean_ms);
				ms = 0;
				frame_counter = 0;
			}
		}

		private static Vector3 green = new Vector3(0.0f, 1.0f, 0.0f);
		private static Vector3 white = new Vector3(1.0f, 1.0f, 1.0f);
		private static Vector3 cyan = new Vector3(0.0f, 1.0f, 1.0f);

		public void Draw(IDebugDraw drawer)
		{
			int i;
			for (i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				drawer.DrawLine(ref source[i], ref hit_com[i], ref green);
			}
			const float normalScale = 10.0f; // easier to see if this is big
			for (i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				var to = hit_surface[i] + normalScale * normal[i];
				drawer.DrawLine(ref hit_surface[i], ref to, ref white);
			}
			var qFrom = Quaternion.RotationAxis(new Vector3(1.0f, 0.0f, 0.0f), 0.0f);
			var qTo = Quaternion.RotationAxis(new Vector3(1.0f, 0.0f, 0.0f), 0.7f);
			for (i = 0; i < NUMRAYS_IN_BAR; i++)
			{
				var from = Matrix.RotationQuaternion(qFrom) * Matrix.Translation(source[i]);
				var to = Matrix.RotationQuaternion(qTo) * Matrix.Translation(dest[i]);

				TransformUtil.CalculateVelocity(ref from, ref to, 1.0f, out var linVel, out var angVel);
				TransformUtil.IntegrateTransform(ref from, ref linVel, ref angVel, hit_fraction[i], out var T);
				var box1 = boxShapeHalfExtents;
				var box2 = -boxShapeHalfExtents;
				drawer.DrawBox(ref box1, ref box2, ref T, ref cyan);
			}
		}
	}

	internal class ConcaveConvexCastDemo : Demo
	{
		private Vector3 eye = new Vector3(0, 15, 60);
		private Vector3 target = new Vector3(-5, 5, 0);
		private const DebugDrawModes debugMode = DebugDrawModes.None;
		private const float TriangleSize = 8.0f;
		private const int NumVertsX = 30;
		private const int NumVertsY = 30;
		private const float WaveHeight = 5.0f;
		private static float offset = 0.0f;
		private bool animatedMesh = true;
		private const int NumDynamicBoxesX = 30;
		private const int NumDynamicBoxesY = 30;
		private TriangleIndexVertexArray indexVertexArrays;
		private BvhTriangleMeshShape groundShape;
		private static ConvexcastBatch convexcastBatch;
		private RigidBody staticBody;

		protected override void OnInitialize()
		{
			Freelook.SetEyeTarget(eye, target);

			Graphics.SetFormText("BulletSharp - Concave Convexcast Demo");
			Graphics.SetInfoText("Move using mouse and WASD+shift\n" +
				"F3 - Toggle debug\n" +
				//"F11 - Toggle fullscreen\n" +
				"Space - Shoot box");

			IsDebugDrawEnabled = false;
			DebugDrawMode = debugMode;

			const int totalVerts = NumVertsX * NumVertsY;
			const int totalTriangles = 2 * (NumVertsX - 1) * (NumVertsY - 1);
			indexVertexArrays = new TriangleIndexVertexArray();

			var mesh = new IndexedMesh
			{
				NumTriangles = totalTriangles,
				NumVertices = totalVerts,
				TriangleIndexStride = 3 * sizeof(int),
				VertexStride = Vector3.SizeInBytes
			};
			mesh.TriangleIndexBase = Marshal.AllocHGlobal(mesh.TriangleIndexStride * totalTriangles);
			mesh.VertexBase = Marshal.AllocHGlobal(mesh.VertexStride * totalVerts);
			var indicesStream = mesh.GetTriangleStream();
			var indices = new BinaryWriter(indicesStream);
			for (int i = 0; i < NumVertsX - 1; i++)
			{
				for (int j = 0; j < NumVertsY - 1; j++)
				{
					indices.Write(j * NumVertsX + i);
					indices.Write(j * NumVertsX + i + 1);
					indices.Write((j + 1) * NumVertsX + i + 1);

					indices.Write(j * NumVertsX + i);
					indices.Write((j + 1) * NumVertsX + i + 1);
					indices.Write((j + 1) * NumVertsX + i);
				}
			}
			indices.Dispose();

			indexVertexArrays.AddIndexedMesh(mesh);

			convexcastBatch = new ConvexcastBatch(40.0f, 0.0f, -10.0f, 80.0f);
			//convexcastBatch = new ConvexcastBatch(true, 40.0f, -50.0f, 50.0f);
		}

		private void SetVertexPositions(float waveheight, float offset)
		{
			var vertexStream = indexVertexArrays.GetVertexStream();
			using (var vertexWriter = new BinaryWriter(vertexStream))
			{
				for (int i = 0; i < NumVertsX; i++)
				{
					for (int j = 0; j < NumVertsY; j++)
					{
						vertexWriter.Write((i - NumVertsX * 0.5f) * TriangleSize);
						vertexWriter.Write(waveheight * (float)Math.Sin(i + offset) * (float)Math.Cos(j + offset));
						vertexWriter.Write((j - NumVertsY * 0.5f) * TriangleSize);
					}
				}
			}
		}

		protected override void OnInitializePhysics()
		{
			// collision configuration contains default setup for memory, collision setup
			CollisionConf = new DefaultCollisionConfiguration();
			Dispatcher = new CollisionDispatcher(CollisionConf);

			var worldMin = new Vector3(-1000, -1000, -1000);
			var worldMax = new Vector3(1000, 1000, 1000);
			Broadphase = new AxisSweep3(worldMin, worldMax);
			Solver = new SequentialImpulseConstraintSolver();

			World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);
			World.SolverInfo.SplitImpulse = 1;
			World.Gravity = new Vector3(0, -10, 0);


			CollisionShape colShape = new BoxShape(1);
			CollisionShapes.Add(colShape);

			for (int j = 0; j < NumDynamicBoxesX; j++)
			{
				for (int i = 0; i < NumDynamicBoxesY; i++)
				{
					//CollisionShape colShape = new CapsuleShape(0.5f,2.0f);//boxShape = new SphereShape(1.0f);
					var startTransform = Matrix.Translation(5 * (i - NumDynamicBoxesX / 2), 10, 5 * (j - NumDynamicBoxesY / 2));
					LocalCreateRigidBody(1.0f, startTransform, colShape);
				}
			}

			SetVertexPositions(WaveHeight, 0.0f);

			const bool useQuantizedAabbCompression = true;
			groundShape = new BvhTriangleMeshShape(indexVertexArrays, useQuantizedAabbCompression);
			CollisionShapes.Add(groundShape);

			staticBody = LocalCreateRigidBody(0.0f, Matrix.Identity, groundShape);
			staticBody.CollisionFlags |= CollisionFlags.StaticObject;
			staticBody.UserObject = "Ground";
		}

		public override void OnUpdate()
		{
			if (animatedMesh)
			{
				offset += FrameDelta;
				SetVertexPositions(WaveHeight, offset);
				//Graphics.MeshFactory.RemoveShape(groundShape);

				var worldMin = new Vector3(-1000, -1000, -1000);
				var worldMax = new Vector3(1000, 1000, 1000);

				groundShape.RefitTree(ref worldMin, ref worldMax);

				//clear all contact points involving mesh proxy. Note: this is a slow/unoptimized operation.
				Broadphase.OverlappingPairCache.CleanProxyFromPairs(staticBody.BroadphaseHandle, Dispatcher);
			}

			convexcastBatch.Move(FrameDelta);
			convexcastBatch.Cast(World);
			if (IsDebugDrawEnabled)
			{
				//convexcastBatch.Draw(World.DebugDrawer);
			}

			base.OnUpdate();
		}

		public override void OnHandleInput()
		{
			if (Input.KeysPressed.Contains(Keys.G))
			{
				animatedMesh = !animatedMesh;
				if (animatedMesh)
				{
					staticBody.CollisionFlags |= CollisionFlags.KinematicObject;
					staticBody.ActivationState = ActivationState.DisableDeactivation;
				}
				else
				{
					staticBody.CollisionFlags &= ~CollisionFlags.KinematicObject;
					staticBody.ActivationState = ActivationState.ActiveTag;
				}
			}
			base.OnHandleInput();
		}
	}

	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			using (Demo demo = new ConcaveConvexCastDemo())
			{
				GraphicsLibraryManager.Run(demo);
			}
		}
	}
}
