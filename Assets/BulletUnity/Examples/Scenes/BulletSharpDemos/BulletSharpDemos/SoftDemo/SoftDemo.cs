using System;
using System.Linq;
using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using BulletSharpExamples;
using DemoFramework;
using CollisionFlags = BulletSharp.SoftBody.CollisionFlags;

namespace SoftDemo
{
	internal class ImplicitSphere : ImplicitFn
	{
		private Vector3 _center;
		private float _sqRadius;

		public ImplicitSphere(ref Vector3 center, float radius)
		{
			_center = center;
			_sqRadius = radius * radius;
		}

		public override float Eval(ref Vector3 x)
		{
			return (x - _center).LengthSquared - _sqRadius;
		}
	};

	internal class MotorControl : AngularJoint.IControl
	{
		private float goal = 0;
		private float maxTorque = 0;

		public float Goal
		{
			get => goal;
			set => goal = value;
		}

		public float MaxTorque
		{
			get => maxTorque;
			set => maxTorque = value;
		}

		public override float Speed(AngularJoint joint, float current)
		{
			return current + Math.Min(maxTorque, Math.Max(-maxTorque, goal - current));
		}
	}

	internal class SteerControl : AngularJoint.IControl
	{
		private float sign;
		private MotorControl _motorControl;

		public float Angle { get; set; }

		public SteerControl(float sign, MotorControl motorControl)
		{
			this.sign = sign;
			_motorControl = motorControl;
		}

		public override void Prepare(AngularJoint joint)
		{
			joint.Refs[0] = new Vector3((float)Math.Cos(Angle * sign), 0, (float)Math.Sin(Angle * sign));
		}

		public override float Speed(AngularJoint joint, float current)
		{
			return _motorControl.Speed(joint, current);
		}
	}

	internal class SoftDemo : Demo
	{
		private Vector3 eye = new Vector3(20, 20, 80);
		private Vector3 target = new Vector3(0, 0, 10);
		private Point lastMousePos;
		private Vector3 impact;
		private SRayCast results = new SRayCast();
		private Node node;
		private Vector3 goal;
		private bool drag;

		public int demo = 27;
		private SoftBodyWorldInfo softBodyWorldInfo;

		public bool cutting;
		private const int maxProxies = 32766;
		private static MotorControl motorControl = new MotorControl();
		private static SteerControl steerControlF = new SteerControl(1, motorControl);
		private static SteerControl steerControlR = new SteerControl(-1, motorControl);

		private SoftRigidDynamicsWorld SoftWorld => World as SoftRigidDynamicsWorld;

		public delegate void DemoConstructor();

		public DemoConstructor[] demos;

		public SoftDemo()
		{
			demos = new DemoConstructor[] { Init_Cloth, Init_Pressure, Init_Volume, Init_Ropes, Init_RopeAttach,
				Init_ClothAttach, Init_Sticks, Init_CapsuleCollision, Init_Collide, Init_Collide2, Init_Collide3, Init_Impact, Init_Aero,
				Init_Aero2, Init_Friction, Init_Torus, Init_TorusMatch, Init_Bunny, Init_BunnyMatch, Init_Cutting1,
				Init_ClusterDeform, Init_ClusterCollide1, Init_ClusterCollide2, Init_ClusterSocket, Init_ClusterHinge,
				Init_ClusterCombine, Init_ClusterCar, Init_ClusterRobot, Init_ClusterStackSoft, Init_ClusterStackMixed,
				Init_TetraCube, Init_TetraBunny, Init_Bending
			};
		}

		protected override void OnInitialize()
		{
			Freelook.SetEyeTarget(eye, target);

			Graphics.SetFormText("BulletSharp - SoftBody Demo");
		}

		private void NextDemo()
		{
			demo++;
			if (demo >= demos.Length)
			{
				demo = 0;
			}

			ClientResetScene();
		}

		private void PreviousDemo()
		{
			demo--;
			if (demo < 0)
			{
				demo = demos.Length - 1;
			}

			ClientResetScene();
		}

		private void InitializeDemo()
		{
			motorControl.Goal = 0;
			motorControl.MaxTorque = 0;

			CollisionShape groundShape = new BoxShape(50, 50, 50);
			CollisionShapes.Add(groundShape);
			var body = LocalCreateRigidBody(0, Matrix.Translation(0, -62, 0), groundShape);
			body.UserObject = "Ground";

			softBodyWorldInfo.SparseSdf.Reset();

			softBodyWorldInfo.AirDensity = 1.2f;
			softBodyWorldInfo.WaterDensity = 0;
			softBodyWorldInfo.WaterOffset = 0;
			softBodyWorldInfo.WaterNormal = Vector3.Zero;
			softBodyWorldInfo.Gravity = new Vector3(0, -10, 0);

			Graphics.CullingEnabled = true;
			demos[demo]();
		}

		private static Vector3 GetRandomVector(Random random)
		{
			return new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
		}

		private SoftBody Create_SoftBox(Vector3 p, Vector3 s)
		{
			var h = s * 0.5f;
			var c = new Vector3[]{
				h * new Vector3(-1,-1,-1),
				h * new Vector3(+1,-1,-1),
				h * new Vector3(-1,+1,-1),
				h * new Vector3(+1,+1,-1),
				h * new Vector3(-1,-1,+1),
				h * new Vector3(+1,-1,+1),
				h * new Vector3(-1,+1,+1),
				h * new Vector3(+1,+1,+1)};
			var psb = SoftBodyHelpers.CreateFromConvexHull(softBodyWorldInfo, c);
			psb.GenerateBendingConstraints(2);
			psb.Translate(p);
			SoftWorld.AddSoftBody(psb);

			return psb;
		}

		private SoftBody Create_SoftBoulder(Vector3 p, Vector3 s, int np)
		{
			var random = new Random();
			var pts = new Vector3[np];
			for (int i = 0; i < np; ++i)
			{
				pts[i] = GetRandomVector(random) * s;
			}

			var psb = SoftBodyHelpers.CreateFromConvexHull(softBodyWorldInfo, pts);
			psb.GenerateBendingConstraints(2);
			psb.Translate(p);
			SoftWorld.AddSoftBody(psb);

			return psb;
		}

		private void Create_RbUpStack(int count)
		{
			const float mass = 10.0f;

			var cylinderCompound = new CompoundShape();
			CollisionShape cylinderShape = new CylinderShapeX(4, 1, 1);
			CollisionShape boxShape = new BoxShape(4, 1, 1);
			cylinderCompound.AddChildShape(Matrix.Identity, boxShape);
			var orn = Quaternion.RotationYawPitchRoll((float)Math.PI / 2.0f, 0.0f, 0.0f);
			var localTransform = Matrix.RotationQuaternion(orn);
			//localTransform *= Matrix.Translation(new Vector3(1,1,1));
			cylinderCompound.AddChildShape(localTransform, cylinderShape);

			var shape = new CollisionShape[]{cylinderCompound,
				new BoxShape(new Vector3(1,1,1)),
				new SphereShape(1.5f)};

			for (int i = 0; i < count; ++i)
			{
				LocalCreateRigidBody(mass, Matrix.Translation(0, 2 + 6 * i, 0), shape[i % shape.Length]);
			}
		}

		private void Create_BigBall(Vector3 position)
		{
			LocalCreateRigidBody(10.0f, Matrix.Translation(position), new SphereShape(1.5f));
		}

		private RigidBody Create_BigPlate(float mass, float height)
		{
			var body = LocalCreateRigidBody(mass, Matrix.Translation(0, height, 0.5f), new BoxShape(5, 1, 5));
			body.Friction = 1;
			return body;
		}

		private RigidBody Create_BigPlate()
		{
			return Create_BigPlate(15, 4);
		}

		private void Init_Pressure()
		{
			var psb = SoftBodyHelpers.CreateEllipsoid(softBodyWorldInfo, new Vector3(35, 25, 0),
				new Vector3(3, 3, 3), 512);
			psb.Materials[0].LinearStiffness = 0.1f;
			psb.config.DynamicFriction = 1;
			psb.config.Damping = 0.001f; // fun factor...
			psb.config.Pressure = 2500;
			psb.SetTotalMass(30, true);
			SoftWorld.AddSoftBody(psb);

			Create_BigPlate();
			Create_LinearStair(10, Vector3.Zero, new Vector3(2, 1, 5));
		}

		private void Init_Ropes()
		{
			const int n = 15;
			for (int i = 0; i < n; i++)
			{
				var psb = SoftBodyHelpers.CreateRope(softBodyWorldInfo,
					new Vector3(-10, 0, i * 0.25f),
					new Vector3(10, 0, i * 0.25f), 16, 1 + 2);
				psb.config.PositionIterations = 4;
				psb.Materials[0].LinearStiffness = 0.1f + (i / (float)(n - 1)) * 0.9f;
				psb.TotalMass = 20;
				SoftWorld.AddSoftBody(psb);
			}
		}

		private SoftBody Create_Rope(Vector3 p)
		{
			var psb = SoftBodyHelpers.CreateRope(softBodyWorldInfo, p, p + new Vector3(10, 0, 0), 8, 1);
			psb.TotalMass = 50;
			SoftWorld.AddSoftBody(psb);
			return psb;
		}

		private void Init_RopeAttach()
		{
			softBodyWorldInfo.SparseSdf.RemoveReferences(null);
			var body = LocalCreateRigidBody(50, Matrix.Translation(12, 8, 0), new BoxShape(2, 6, 2));
			var psb0 = Create_Rope(new Vector3(0, 8, -1));
			var psb1 = Create_Rope(new Vector3(0, 8, +1));
			psb0.AppendAnchor(psb0.Nodes.Count - 1, body);
			psb1.AppendAnchor(psb1.Nodes.Count - 1, body);
		}

		private void Init_ClothAttach()
		{
			const float s = 4;
			const float h = 6;
			int r = 9;
			var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, new Vector3(-s, h, -s),
				new Vector3(+s, h, -s),
				new Vector3(-s, h, +s),
				new Vector3(+s, h, +s), r, r, 4 + 8, true);
			SoftWorld.AddSoftBody(psb);

			var body = LocalCreateRigidBody(20, Matrix.Translation(0, h, -(s + 3.5f)), new BoxShape(s, 1, 3));
			psb.AppendAnchor(0, body);
			psb.AppendAnchor(r - 1, body);
			body.UserObject = "LargeBox";
			cutting = true;

			Graphics.CullingEnabled = false;
		}

		private void Create_LinearStair(int count, Vector3 origin, Vector3 sizes)
		{
			var shape = new BoxShape(sizes);
			for (int i = 0; i < count; i++)
			{
				var body = LocalCreateRigidBody(0,
					Matrix.Translation(origin + new Vector3(sizes.X * 2 * i, sizes.Y * 2 * i, 0)), shape);
				body.Friction = 1;
			}
		}

		private void Init_Impact()
		{
			var psb = SoftBodyHelpers.CreateRope(softBodyWorldInfo,
				Vector3.Zero, new Vector3(0, -1, 0), 0, 1);
			SoftWorld.AddSoftBody(psb);
			psb.config.RigidContactHardness = 0.5f;
			LocalCreateRigidBody(10, Matrix.Translation(0, 20, 0), new BoxShape(2));
		}

		private void Init_CapsuleCollision()
		{
			float s = 4;
			float h = 6;
			int r = 20;

			var startTransform = Matrix.Translation(0, h - 2, 0);

			CollisionShape capsuleShape = new CapsuleShapeX(1, 5)
			{
				Margin = 0.5f
			};

			//capsuleShape.LocalScaling = new Vector3(5, 1, 1);
			//RigidBody body = LocalCreateRigidBody(20, startTransform, capsuleShape);
			var body = LocalCreateRigidBody(0, startTransform, capsuleShape);
			body.Friction = 0.8f;

			const int fixeds = 0; //4+8;
			var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, new Vector3(-s, h, -s),
				new Vector3(+s, h, -s),
				new Vector3(-s, h, +s),
				new Vector3(+s, h, +s), r, r, fixeds, true);
			SoftWorld.AddSoftBody(psb);
			psb.TotalMass = 0.1f;

			psb.config.PositionIterations = 10;
			psb.config.ClusterIterations = 10;
			psb.config.DriftIterations = 10;
			//psb.Cfg.VelocityIterations = 10;


			//psb.AppendAnchor(0, body);
			//psb.AppendAnchor(r-1, body);
			// cutting=true;

			Graphics.CullingEnabled = false;
		}

		private void Init_Collide()
		{
			for (int i = 0; i < 3; ++i)
			{
				var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, TorusMesh.Vertices, TorusMesh.Indices);
				psb.GenerateBendingConstraints(2);
				psb.config.PositionIterations = 2;
				psb.config.Collisions |= CollisionFlags.VertexFaceSoftSoft;
				psb.RandomizeConstraints();
				var m = Matrix.RotationYawPitchRoll((float)Math.PI / 2 * (i & 1), (float)Math.PI / 2 * (1 - (i & 1)), 0) *
					Matrix.Translation(3 * i, 2, 0);
				psb.Transform(m);
				psb.Scale(new Vector3(2, 2, 2));
				psb.SetTotalMass(50, true);
				SoftWorld.AddSoftBody(psb);
			}
			cutting = true;
		}

		private void Init_Collide2()
		{
			for (int i = 0; i < 3; ++i)
			{
				var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, BunnyMesh.Vertices, BunnyMesh.Indices);
				var pm = psb.AppendMaterial();
				pm.LinearStiffness = 0.5f;
				pm.Flags -= MaterialFlags.DebugDraw;
				psb.GenerateBendingConstraints(2, pm);
				psb.config.PositionIterations = 2;
				psb.config.DynamicFriction = 0.5f;
				psb.config.Collisions |= CollisionFlags.VertexFaceSoftSoft;
				psb.RandomizeConstraints();
				var m = Matrix.RotationYawPitchRoll((float)Math.PI / 2 * (i & 1), 0, 0) *
					Matrix.Translation(0, -1 + 5 * i, 0);
				psb.Transform(m);
				psb.Scale(new Vector3(6, 6, 6));
				psb.SetTotalMass(100, true);
				SoftWorld.AddSoftBody(psb);
			}
			cutting = true;
		}

		private void Init_Collide3()
		{
			float s = 8;
			var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, new Vector3(-s, 0, -s),
				new Vector3(+s, 0, -s),
				new Vector3(-s, 0, +s),
				new Vector3(+s, 0, +s),
				15, 15, 1 + 2 + 4 + 8, true);
			psb.Materials[0].LinearStiffness = 0.4f;
			psb.config.Collisions |= CollisionFlags.VertexFaceSoftSoft;
			psb.TotalMass = 150;
			SoftWorld.AddSoftBody(psb);

			s = 4;
			var o = new Vector3(5, 10, 0);
			psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo,
				new Vector3(-s, 0, -s) + o,
				new Vector3(+s, 0, -s) + o,
				new Vector3(-s, 0, +s) + o,
				new Vector3(+s, 0, +s) + o,
				7, 7, 0, true);
			var pm = psb.AppendMaterial();
			pm.LinearStiffness = 0.1f;
			pm.Flags -= MaterialFlags.DebugDraw;
			psb.GenerateBendingConstraints(2, pm);
			psb.Materials[0].LinearStiffness = 0.5f;
			psb.config.Collisions |= CollisionFlags.VertexFaceSoftSoft;
			psb.TotalMass = 150;
			SoftWorld.AddSoftBody(psb);
			cutting = true;

			Graphics.CullingEnabled = false;
		}

		// Aerodynamic forces, 50x1g flyers
		private void Init_Aero()
		{
			const float s = 2;
			const int segments = 6;
			const int count = 50;
			var random = new Random();
			for (int i = 0; i < count; ++i)
			{
				var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo,
					new Vector3(-s, 0, -s), new Vector3(+s, 0, -s),
					new Vector3(-s, 0, +s), new Vector3(+s, 0, +s),
					segments, segments, 0, true);
				var pm = psb.AppendMaterial();
				pm.Flags -= MaterialFlags.DebugDraw;
				psb.GenerateBendingConstraints(2, pm);
				psb.config.Lift = 0.004f;
				psb.config.Drag = 0.0003f;
				psb.config.AeroModel = AeroModel.VertexTwoSided;
				var trans = Matrix.Identity;
				var ra = 0.1f * GetRandomVector(random);
				var rp = 75 * GetRandomVector(random) + new Vector3(-50, 15, 0);
				var rot = Quaternion.RotationYawPitchRoll(
					(float)Math.PI / 8 + ra.X, (float)-Math.PI / 7 + ra.Y, ra.Z);
				trans *= Matrix.RotationQuaternion(rot);
				trans *= Matrix.Translation(rp);
				psb.Transform(trans);
				psb.TotalMass = 0.1f;
				psb.AddForce(new Vector3(0, (float)random.NextDouble(), 0), 0);
				SoftWorld.AddSoftBody(psb);
			}

			Graphics.CullingEnabled = false;
		}

		private void Init_Aero2()
		{
			const float s = 5;
			const int segments = 10;
			const int count = 5;
			var pos = new Vector3(-s * segments, 0, 0);
			float gap = 0.5f;

			for (int i = 0; i < count; ++i)
			{
				var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, new Vector3(-s, 0, -s * 3),
					new Vector3(+s, 0, -s * 3),
					new Vector3(-s, 0, +s),
					new Vector3(+s, 0, +s),
					segments, segments * 3,
					1 + 2, true);

				psb.CollisionShape.Margin = 0.5f;
				var pm = psb.AppendMaterial();
				pm.LinearStiffness = 0.0004f;
				pm.Flags -= MaterialFlags.DebugDraw;
				psb.GenerateBendingConstraints(2, pm);

				psb.config.Lift = 0.05f;
				psb.config.Drag = 0.01f;

				//psb.Cfg.LF = 0.004f;
				//psb.Cfg.DG = 0.0003f;

				psb.config.PositionIterations = 2;
				psb.config.AeroModel = AeroModel.VertexTwoSidedLiftDrag;


				psb.WindVelocity = new Vector3(4, -12.0f, -25.0f);

				pos += new Vector3(s * 2 + gap, 0, 0);
				var trs = Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(pos);
				psb.Transform(trs);
				psb.TotalMass = 2.0f;

				SoftBodyHelpers.ReoptimizeLinkOrder(psb);

				SoftWorld.AddSoftBody(psb);
			}

			Graphics.CullingEnabled = false;
		}

		private void Init_Friction()
		{
			const float bs = 2;
			const float ts = bs + bs / 4;
			for (int i = 0, ni = 20; i < ni; ++i)
			{
				var p = new Vector3(-ni * ts / 2 + i * ts, bs, 40);
				var psb = Create_SoftBox(p, new Vector3(bs, bs, bs));
				psb.config.DynamicFriction = 0.1f * ((i + 1) / (float)ni);
				psb.AddVelocity(new Vector3(0, 0, -10));
			}
		}

		private void Init_TetraBunny()
		{
			var psb = SoftBodyHelpers.CreateFromTetGenData(softBodyWorldInfo,
				Bunny.GetElements(), null, Bunny.GetNodes(), false, true, true);
			SoftWorld.AddSoftBody(psb);
			psb.Rotate(Quaternion.RotationYawPitchRoll((float)Math.PI / 2, 0, 0));
			psb.SetVolumeMass(150);
			psb.config.PositionIterations = 2;
			//psb.Cfg.PIterations = 1;
			cutting = false;
			//psb.CollisionShape.Margin = 0.01f;
			psb.config.Collisions = CollisionFlags.ClusterClusterSoftSoft | CollisionFlags.ClusterConvexRigidSoft; //| CollisionFlags.ClusterSelf;

			///pass zero in generateClusters to create  cluster for each tetrahedron or triangle
			psb.GenerateClusters(0);
			//psb.Materials[0].Lst = 0.2f;
			psb.config.DynamicFriction = 10;

			Graphics.CullingEnabled = false;
		}

		private void Init_TetraCube()
		{
			var ele = (UnityEngine.TextAsset)UnityEngine.Resources.Load("Cube.ele");  //(elementFilename != null) ? File.ReadAllText(elementFilename) : null;
			var face = (UnityEngine.TextAsset)UnityEngine.Resources.Load("Cube.ply"); // (faceFilename != null) ? File.ReadAllText(faceFilename) : null;
			var node = (UnityEngine.TextAsset)UnityEngine.Resources.Load("Cube.node");  // File.ReadAllText(nodeFilename)
			var psb = SoftBodyHelpers.CreateFromTetGenData(softBodyWorldInfo, ele.text, face.text, node.text, false, true, true);

			SoftWorld.AddSoftBody(psb);
			psb.Scale(new Vector3(4, 4, 4));
			psb.Translate(0, 5, 0);
			psb.SetVolumeMass(300);

			// fix one vertex
			//psb.SetMass(0,0);
			//psb.SetMass(10,0);
			//psb.SetMass(20,0);
			psb.config.PositionIterations = 1;
			//psb.GenerateClusters(128);
			psb.GenerateClusters(16);
			//psb.CollisionShape.Margin = 0.5f;

			psb.CollisionShape.Margin = 0.01f;
			psb.config.Collisions = CollisionFlags.ClusterClusterSoftSoft | CollisionFlags.ClusterConvexRigidSoft;
			// | Collision.ClusterSelf;
			psb.Materials[0].LinearStiffness = 0.8f;
			cutting = false;

			Graphics.CullingEnabled = false;
		}

		private void Init_Volume()
		{
			var psb = SoftBodyHelpers.CreateEllipsoid(softBodyWorldInfo, new Vector3(35, 25, 0),
				new Vector3(1, 1, 1) * 3, 512);
			psb.Materials[0].LinearStiffness = 0.45f;
			psb.config.VolumeConversation = 20;
			psb.SetTotalMass(50, true);
			psb.SetPose(true, false);
			SoftWorld.AddSoftBody(psb);

			Create_BigPlate();
			Create_LinearStair(10, Vector3.Zero, new Vector3(2, 1, 5));
		}

		private void Init_Sticks()
		{
			const int n = 16;
			const int sg = 4;
			const float sz = 5;
			const float hg = 4;
			const float inf = 1 / (float)(n - 1);
			for (int y = 0; y < n; ++y)
			{
				for (int x = 0; x < n; ++x)
				{
					var org = new Vector3(-sz + sz * 2 * x * inf,
						-10, -sz + sz * 2 * y * inf);

					var psb = SoftBodyHelpers.CreateRope(softBodyWorldInfo, org,
						org + new Vector3(hg * 0.001f, hg, 0), sg, 1);

					psb.config.Damping = 0.005f;
					psb.config.RigidContactHardness = 0.1f;
					for (int i = 0; i < 3; ++i)
					{
						psb.GenerateBendingConstraints(2 + i);
					}
					psb.SetMass(1, 0);
					psb.TotalMass = 0.01f;
					SoftWorld.AddSoftBody(psb);
				}
			}
			Create_BigBall(new Vector3(0, 13, 0));
		}

		private void Init_Bending()
		{
			const float s = 4;
			var x = new Vector3[]{new Vector3(-s,0,-s),
				new Vector3(+s,0,-s),
				new Vector3(+s,0,+s),
				new Vector3(-s,0,+s)};
			float[] m = new float[] { 0, 0, 0, 1 };
			var psb = new SoftBody(softBodyWorldInfo, 4, x, m);
			psb.AppendLink(0, 1);
			psb.AppendLink(1, 2);
			psb.AppendLink(2, 3);
			psb.AppendLink(3, 0);
			psb.AppendLink(0, 2);

			SoftWorld.AddSoftBody(psb);
		}

		private void Init_Cloth()
		{
			float s = 8;
			var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, new Vector3(-s, 0, -s),
				new Vector3(+s, 0, -s),
				new Vector3(-s, 0, +s),
				new Vector3(+s, 0, +s),
				31, 31,
				1 + 2 + 4 + 8, true);

			psb.CollisionShape.Margin = 0.5f;
			var pm = psb.AppendMaterial();
			pm.LinearStiffness = 0.4f;
			pm.Flags -= MaterialFlags.DebugDraw;
			psb.GenerateBendingConstraints(2, pm);
			psb.TotalMass = 150;
			SoftWorld.AddSoftBody(psb);

			Create_RbUpStack(10);
			cutting = true;

			Graphics.CullingEnabled = false;
		}

		private void Init_Bunny()
		{
			var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, BunnyMesh.Vertices, BunnyMesh.Indices);
			var pm = psb.AppendMaterial();
			pm.LinearStiffness = 0.5f;
			pm.Flags -= MaterialFlags.DebugDraw;
			psb.GenerateBendingConstraints(2, pm);
			psb.config.PositionIterations = 2;
			psb.config.DynamicFriction = 0.5f;
			psb.RandomizeConstraints();
			var m = Matrix.RotationYawPitchRoll(0, (float)Math.PI / 2, 0) *
				Matrix.Translation(0, 4, 0);
			psb.Transform(m);
			psb.Scale(new Vector3(6, 6, 6));
			psb.SetTotalMass(100, true);
			SoftWorld.AddSoftBody(psb);
			cutting = true;
		}

		private void Init_BunnyMatch()
		{
			var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, BunnyMesh.Vertices, BunnyMesh.Indices);
			psb.config.DynamicFriction = 0.5f;
			psb.config.PoseMatching = 0.05f;
			psb.config.PositionIterations = 5;
			psb.RandomizeConstraints();
			psb.Scale(new Vector3(6, 6, 6));
			psb.SetTotalMass(100, true);
			psb.SetPose(false, true);
			SoftWorld.AddSoftBody(psb);
		}

		private void Init_Torus()
		{
			var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, TorusMesh.Vertices, TorusMesh.Indices);
			psb.GenerateBendingConstraints(2);
			psb.config.PositionIterations = 2;
			psb.RandomizeConstraints();
			var m = Matrix.RotationYawPitchRoll(0, (float)Math.PI / 2, 0) *
				Matrix.Translation(0, 4, 0);
			psb.Transform(m);
			psb.Scale(new Vector3(2, 2, 2));
			psb.SetTotalMass(50, true);
			SoftWorld.AddSoftBody(psb);
			cutting = true;
		}

		private void Init_TorusMatch()
		{
			var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, TorusMesh.Vertices, TorusMesh.Indices);
			psb.Materials[0].LinearStiffness = 0.1f;
			psb.config.PoseMatching = 0.05f;
			psb.RandomizeConstraints();
			var m = Matrix.RotationYawPitchRoll(0, (float)Math.PI / 2, 0) *
				Matrix.Translation(0, 4, 0);
			psb.Transform(m);
			psb.Scale(new Vector3(2, 2, 2));
			psb.SetTotalMass(50, true);
			psb.SetPose(false, true);
			SoftWorld.AddSoftBody(psb);
		}

		private void Init_Cutting1()
		{
			const float s = 6;
			const float h = 2;
			const int r = 16;
			var p = new Vector3[]{new Vector3(+s,h,-s),
				new Vector3(-s,h,-s),
				new Vector3(+s,h,+s),
				new Vector3(-s,h,+s)};
			var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, p[0], p[1], p[2], p[3], r, r, 1 + 2 + 4 + 8, true);
			SoftWorld.AddSoftBody(psb);
			psb.config.PositionIterations = 1;
			cutting = true;

			Graphics.CullingEnabled = false;
		}

		private void CreateGear(Vector3 pos, float speed)
		{
			var startTransform = Matrix.Translation(pos);
			var shape = new CompoundShape();
#if true
			shape.AddChildShape(Matrix.Identity, new BoxShape(5, 1, 6));
			shape.AddChildShape(Matrix.RotationZ((float)Math.PI), new BoxShape(5, 1, 6));
#else
            shape.AddChildShape(Matrix.Identity, new CylinderShapeZ(5,1,7));
            shape.AddChildShape(Matrix.RotationZ((float)Math.PI), new BoxShape(4,1,8));
#endif
			var body = LocalCreateRigidBody(10, startTransform, shape);
			body.Friction = 1;
			var hinge = new HingeConstraint(body, Matrix.Identity);
			if (speed != 0)
			{
				hinge.EnableAngularMotor(true, speed, 3);
			}

			World.AddConstraint(hinge);
		}

		private SoftBody Create_ClusterBunny(Vector3 x, Vector3 a)
		{
			var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, BunnyMesh.Vertices, BunnyMesh.Indices);
			var pm = psb.AppendMaterial();
			pm.LinearStiffness = 1;
			pm.Flags -= MaterialFlags.DebugDraw;
			psb.GenerateBendingConstraints(2, pm);
			psb.config.PositionIterations = 2;
			psb.config.DynamicFriction = 1;
			psb.config.Collisions = CollisionFlags.ClusterClusterSoftSoft | CollisionFlags.ClusterConvexRigidSoft;
			psb.RandomizeConstraints();
			var m = Matrix.RotationYawPitchRoll(a.X, a.Y, a.Z) * Matrix.Translation(x);
			psb.Transform(m);
			psb.Scale(new Vector3(8, 8, 8));
			psb.SetTotalMass(150, true);
			psb.GenerateClusters(1);
			SoftWorld.AddSoftBody(psb);
			return psb;
		}

		private SoftBody Create_ClusterTorus(Vector3 x, Vector3 a, Vector3 s)
		{
			var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, TorusMesh.Vertices, TorusMesh.Indices);
			var pm = psb.AppendMaterial();
			pm.LinearStiffness = 1;
			pm.Flags -= MaterialFlags.DebugDraw;
			psb.GenerateBendingConstraints(2, pm);
			psb.config.PositionIterations = 2;
			psb.config.Collisions = CollisionFlags.ClusterClusterSoftSoft | CollisionFlags.ClusterConvexRigidSoft;
			psb.RandomizeConstraints();
			psb.Scale(s);
			var m = Matrix.RotationYawPitchRoll(a.X, a.Y, a.Z) * Matrix.Translation(x);
			psb.Transform(m);
			psb.SetTotalMass(50, true);
			psb.GenerateClusters(64);
			SoftWorld.AddSoftBody(psb);
			return psb;
		}

		private SoftBody Create_ClusterTorus(Vector3 x, Vector3 a)
		{
			return Create_ClusterTorus(x, a, new Vector3(2));
		}

		private void Init_ClusterDeform()
		{
			var psb = Create_ClusterTorus(Vector3.Zero, new Vector3((float)Math.PI / 2, 0, (float)Math.PI / 2));
			psb.GenerateClusters(8);
			psb.config.DynamicFriction = 1;
		}

		private void Init_ClusterCollide1()
		{
			const float s = 8;
			var psb = SoftBodyHelpers.CreatePatch(softBodyWorldInfo, new Vector3(-s, 0, -s),
				new Vector3(+s, 0, -s),
				new Vector3(-s, 0, +s),
				new Vector3(+s, 0, +s),
				17, 17,//9,9,//31,31,
				1 + 2 + 4 + 8,
				true);
			var pm = psb.AppendMaterial();
			pm.LinearStiffness = 0.4f;
			pm.Flags -= MaterialFlags.DebugDraw;
			psb.config.DynamicFriction = 1;
			psb.config.SoftRigidHardness = 1;
			psb.config.SoftRigidImpulseSplit = 0;
			psb.config.Collisions = CollisionFlags.ClusterClusterSoftSoft | CollisionFlags.ClusterConvexRigidSoft;
			psb.GenerateBendingConstraints(2, pm);

			psb.CollisionShape.Margin = 0.05f;
			psb.TotalMass = 50;

			// pass zero in generateClusters to create  cluster for each tetrahedron or triangle
			psb.GenerateClusters(0);
			//psb.GenerateClusters(64);

			SoftWorld.AddSoftBody(psb);

			Create_RbUpStack(10);

			Graphics.CullingEnabled = false;
		}

		private void Init_ClusterCollide2()
		{
			for (int i = 0; i < 3; ++i)
			{
				var psb = SoftBodyHelpers.CreateFromTriMesh(softBodyWorldInfo, TorusMesh.Vertices, TorusMesh.Indices);
				var pm = psb.AppendMaterial();
				pm.Flags -= MaterialFlags.DebugDraw;
				psb.GenerateBendingConstraints(2, pm);
				psb.config.PositionIterations = 2;
				psb.config.DynamicFriction = 1;
				psb.config.SoftSoftHardness = 1;
				psb.config.SoftSoftImpulseSplit = 0;
				psb.config.SoftKineticHardness = 0.1f;
				psb.config.SoftKineticImpulseSplit = 1;
				psb.config.Collisions = CollisionFlags.ClusterClusterSoftSoft | CollisionFlags.ClusterConvexRigidSoft;
				psb.RandomizeConstraints();
				var m = Matrix.RotationYawPitchRoll((float)Math.PI / 2 * (i & 1), (float)Math.PI / 2 * (1 - (i & 1)), 0)
					* Matrix.Translation(3 * i, 2, 0);
				psb.Transform(m);
				psb.Scale(new Vector3(2, 2, 2));
				psb.SetTotalMass(50, true);
				psb.GenerateClusters(16);
				SoftWorld.AddSoftBody(psb);
			}
		}

		private void Init_ClusterSocket()
		{
			var psb = Create_ClusterTorus(Vector3.Zero, new Vector3((float)Math.PI / 2, 0, (float)Math.PI / 2));
			var prb = Create_BigPlate(50, 8);
			psb.config.DynamicFriction = 1;
			var lj = new LinearJoint.Specs
			{
				Position = new Vector3(0, 5, 0)
			};
			psb.AppendLinearJoint(lj, new Body(prb));
		}

		private void Init_ClusterHinge()
		{
			var psb = Create_ClusterTorus(Vector3.Zero, new Vector3((float)Math.PI / 2, 0, (float)Math.PI / 2));
			var prb = Create_BigPlate(50, 8);
			psb.config.DynamicFriction = 1;
			var aj = new AngularJoint.Specs
			{
				Axis = new Vector3(0, 0, 1)
			};
			psb.AppendAngularJoint(aj, new Body(prb));
		}

		private void Init_ClusterCombine()
		{
			var sz = new Vector3(2, 4, 2);
			var psb0 = Create_ClusterTorus(new Vector3(0, 8, 0), new Vector3((float)Math.PI / 2, 0, (float)Math.PI / 2), sz);
			var psb1 = Create_ClusterTorus(new Vector3(0, 8, 10), new Vector3((float)Math.PI / 2, 0, (float)Math.PI / 2), sz);
			var psbs = new SoftBody[] { psb0, psb1 };
			for (int j = 0; j < 2; ++j)
			{
				psbs[j].config.DynamicFriction = 1;
				psbs[j].config.DynamicFriction = 0;
				psbs[j].config.PositionIterations = 1;
				psbs[j].Clusters[0].Matching = 0.05f;
				psbs[j].Clusters[0].NodeDamping = 0.05f;
			}
			var aj = new AngularJoint.Specs
			{
				Axis = new Vector3(0, 0, 1),
				Control = motorControl
			};
			psb0.AppendAngularJoint(aj, psb1);

			var lj = new LinearJoint.Specs
			{
				Position = new Vector3(0, 8, 5)
			};
			psb0.AppendLinearJoint(lj, psb1);
		}

		private void Init_ClusterCar()
		{
			//SetAzi(180);
			var origin = new Vector3(100, 80, 0);
			var orientation = Quaternion.RotationYawPitchRoll(-(float)Math.PI / 2, 0, 0);
			const float widthf = 8;
			const float widthr = 9;
			const float length = 8;
			const float height = 4;
			Vector3[] wheels = {
				new Vector3(+widthf,-height,+length), // Front left
                new Vector3(-widthf,-height,+length), // Front right
                new Vector3(+widthr,-height,-length), // Rear left
                new Vector3(-widthr,-height,-length), // Rear right
            };
			var pa = Create_ClusterBunny(Vector3.Zero, Vector3.Zero);
			var pfl = Create_ClusterTorus(wheels[0], new Vector3(0, 0, (float)Math.PI / 2), new Vector3(2, 4, 2));
			var pfr = Create_ClusterTorus(wheels[1], new Vector3(0, 0, (float)Math.PI / 2), new Vector3(2, 4, 2));
			var prl = Create_ClusterTorus(wheels[2], new Vector3(0, 0, (float)Math.PI / 2), new Vector3(2, 5, 2));
			var prr = Create_ClusterTorus(wheels[3], new Vector3(0, 0, (float)Math.PI / 2), new Vector3(2, 5, 2));

			pfl.config.DynamicFriction =
				pfr.config.DynamicFriction =
				prl.config.DynamicFriction =
				prr.config.DynamicFriction = 1;

			var lspecs = new LinearJoint.Specs
			{
				ConstraintForceMixing = 1,
				ErrorReductionParameter = 1,
				Position = Vector3.Zero
			};

			lspecs.Position = wheels[0];
			pa.AppendLinearJoint(lspecs, pfl);
			lspecs.Position = wheels[1];
			pa.AppendLinearJoint(lspecs, pfr);
			lspecs.Position = wheels[2];
			pa.AppendLinearJoint(lspecs, prl);
			lspecs.Position = wheels[3];
			pa.AppendLinearJoint(lspecs, prr);

			var aspecs = new AngularJoint.Specs
			{
				ConstraintForceMixing = 1,
				ErrorReductionParameter = 1,
				Axis = new Vector3(1, 0, 0),

				Control = steerControlF
			};
			pa.AppendAngularJoint(aspecs, pfl);
			pa.AppendAngularJoint(aspecs, pfr);

			aspecs.Control = motorControl;
			pa.AppendAngularJoint(aspecs, prl);
			pa.AppendAngularJoint(aspecs, prr);

			pa.Rotate(orientation);
			pfl.Rotate(orientation);
			pfr.Rotate(orientation);
			prl.Rotate(orientation);
			prr.Rotate(orientation);
			pa.Translate(origin);
			pfl.Translate(origin);
			pfr.Translate(origin);
			prl.Translate(origin);
			prr.Translate(origin);
			pfl.config.PositionIterations =
				pfr.config.PositionIterations =
				prl.config.PositionIterations =
				prr.config.PositionIterations = 1;
			pfl.Clusters[0].Matching =
				pfr.Clusters[0].Matching =
				prl.Clusters[0].Matching =
				prr.Clusters[0].Matching = 0.05f;
			pfl.Clusters[0].NodeDamping =
				pfr.Clusters[0].NodeDamping =
				prl.Clusters[0].NodeDamping =
				prr.Clusters[0].NodeDamping = 0.05f;

			Create_LinearStair(20, new Vector3(0, -8, 0), new Vector3(3, 2, 40));
			Create_RbUpStack(50);
			//autocam=true;
		}

		private SoftBody Init_ClusterRobot_CreateBall(Vector3 pos)
		{
			var psb = SoftBodyHelpers.CreateEllipsoid(softBodyWorldInfo, pos, new Vector3(1, 1, 1) * 3, 512);
			psb.Materials[0].LinearStiffness = 0.45f;
			psb.config.VolumeConversation = 20;
			psb.SetTotalMass(50, true);
			psb.SetPose(true, false);
			psb.GenerateClusters(1);
			SoftWorld.AddSoftBody(psb);
			return psb;
		}

		private void Init_ClusterRobot()
		{
			var basePos = new Vector3(0, 25, 8);
			var psb0 = Init_ClusterRobot_CreateBall(basePos + new Vector3(-8, 0, 0));
			var psb1 = Init_ClusterRobot_CreateBall(basePos + new Vector3(+8, 0, 0));
			var psb2 = Init_ClusterRobot_CreateBall(basePos + new Vector3(0, 0, +8 * (float)Math.Sqrt(2)));
			var ctr = (psb0.ClusterCom(0) + psb1.ClusterCom(0) + psb2.ClusterCom(0)) / 3;
			var pshp = new CylinderShape(new Vector3(8, 1, 8));
			var prb = LocalCreateRigidBody(50, Matrix.Translation(ctr + new Vector3(0, 5, 0)), pshp);
			var ls = new LinearJoint.Specs
			{
				ErrorReductionParameter = 0.5f
			};
			var prbBody = new Body(prb);
			ls.Position = psb0.ClusterCom(0);
			psb0.AppendLinearJoint(ls, prbBody);
			ls.Position = psb1.ClusterCom(0);
			psb1.AppendLinearJoint(ls, prbBody);
			ls.Position = psb2.ClusterCom(0);
			psb2.AppendLinearJoint(ls, prbBody);

			var pbox = new BoxShape(20, 1, 40);
			LocalCreateRigidBody(0, Matrix.RotationZ(-(float)Math.PI / 4), pbox);
		}

		private void Init_ClusterStackSoft()
		{
			for (int i = 0; i < 10; ++i)
			{
				var psb = Create_ClusterTorus(new Vector3(0, -9 + 8.25f * i, 0), Vector3.Zero);
				psb.config.DynamicFriction = 1;
			}
		}

		//
		private void Init_ClusterStackMixed()
		{
			for (int i = 0; i < 10; ++i)
			{
				if (((i + 1) & 1) == 1)
				{
					Create_BigPlate(50, -9 + 4.25f * i);
				}
				else
				{
					var psb = Create_ClusterTorus(new Vector3(0, -9 + 4.25f * i, 0), Vector3.Zero);
					psb.config.DynamicFriction = 1;
				}
			}
		}

		protected override void OnInitializePhysics()
		{
			// collision configuration contains default setup for memory, collision setup
			CollisionConf = new SoftBodyRigidBodyCollisionConfiguration();
			Dispatcher = new CollisionDispatcher(CollisionConf);

			Broadphase = new AxisSweep3(new Vector3(-1000f, -1000f, -1000f), new Vector3(1000f, 1000f, 1000f), maxProxies);

			// the default constraint solver.
			Solver = new SequentialImpulseConstraintSolver();

			softBodyWorldInfo = new SoftBodyWorldInfo
			{
				AirDensity = 1.2f,
				WaterDensity = 0,
				WaterOffset = 0,
				WaterNormal = Vector3.Zero,
				Gravity = new Vector3(0, -10, 0),
				Dispatcher = Dispatcher,
				Broadphase = Broadphase
			};

			softBodyWorldInfo.SparseSdf.Initialize();

			World = new SoftRigidDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf)
			{
				Gravity = new Vector3(0, -10, 0)
			};
			World.DispatchInfo.EnableSpu = true;

			World.SetInternalTickCallback(PickingPreTickCallback, this, true);

			InitializeDemo();
		}

		public override void OnUpdate()
		{
			softBodyWorldInfo.SparseSdf.GarbageCollect();
			base.OnUpdate();
		}

		private void PickingPreTickCallback(DynamicsWorld world, float timeStep)
		{
			if (!drag)
			{
				return;
			}

			var rayFrom = Freelook.Eye;
			var rayTo = GetRayTo(lastMousePos, Freelook.Eye, Freelook.Target, Graphics.FieldOfView);
			var rayDir = rayTo - rayFrom;
			rayDir.Normalize();
			var N = Freelook.Target - rayFrom;
			N.Normalize();
			float O = Vector3.Dot(impact, N);
			float den = Vector3.Dot(N, rayDir);
			if ((den * den) > 0)
			{
				float num = O - Vector3.Dot(N, rayFrom);
				float hit = num / den;
				if (hit > 0 && hit < 1500)
				{
					goal = rayFrom + rayDir * hit;
				}
			}
			var delta = goal - node.Position;
			float maxDrag = 10;
			if (delta.LengthSquared > (maxDrag * maxDrag))
			{
				delta.Normalize();
				delta *= maxDrag;
			}
			node.Velocity += delta / timeStep;
		}

		public override void OnHandleInput()
		{
			if (Input.KeysPressed.Contains(Keys.B))
			{
				PreviousDemo();
			}
			else if (Input.KeysPressed.Contains(Keys.N))
			{
				NextDemo();
			}

			if (Input.KeysDown.Count != 0)
			{
				if (demos[demo] == Init_ClusterCombine || demos[demo] == Init_ClusterCar)
				{
					if (Input.KeysDown.Contains(Keys.Up))
					{
						motorControl.MaxTorque = 1;
						motorControl.Goal += FrameDelta * 2;
					}
					else if (Input.KeysDown.Contains(Keys.Down))
					{
						motorControl.MaxTorque = 1;
						motorControl.Goal -= FrameDelta * 2;
					}
					else if (Input.KeysDown.Contains(Keys.Left))
					{
						steerControlF.Angle += FrameDelta;
						steerControlR.Angle += FrameDelta;
					}
					else if (Input.KeysDown.Contains(Keys.Right))
					{
						steerControlF.Angle -= FrameDelta;
						steerControlR.Angle -= FrameDelta;
					}
				}
			}

			if (Input.MousePressed == MouseButtons.Right)
			{
				results.Fraction = 1;
				if (pickConstraint == null)
				{
					var rayFrom = Freelook.Eye;
					var rayTo = GetRayTo(Input.MousePoint, Freelook.Eye, Freelook.Target, Graphics.FieldOfView);
					var rayDir = rayTo - rayFrom;
					rayDir.Normalize();

					var res = new SRayCast();
					if (SoftWorld.SoftBodyArray.Any(b => b.RayTest(ref rayFrom, ref rayTo, res)))
					{
						results = res;
						impact = rayFrom + (rayTo - rayFrom) * results.Fraction;
						drag = !cutting;
						lastMousePos = Input.MousePoint;

						NodePtrArray nodes;
						switch (results.Feature)
						{
							case FeatureType.Face:
								nodes = results.Body.Faces[results.Index].Nodes;
								break;
							case FeatureType.Tetra:
								nodes = results.Body.Tetras[results.Index].Nodes;
								break;
							default:
								nodes = null;
								break;
						}
						if (nodes != null)
						{
							node = nodes.Aggregate((min, n) =>
								(n.Position - impact).LengthSquared <
								(min.Position - impact).LengthSquared ? n : min
							);
							goal = node.Position;
						}
						else
						{
							node = null;
						}
					}
				}
			}
			else if (Input.MouseReleased == MouseButtons.Right)
			{
				if (!drag && cutting && results.Fraction < 1)
				{
					using (var isphere = new ImplicitSphere(ref impact, 1))
					{
						results.Body.Refine(isphere, 0.0001f, true);
					}
				}
				results.Fraction = 1;
				drag = false;
			}

			// Mouse movement
			if (Input.MouseDown == MouseButtons.Right)
			{
				if (node != null && results.Fraction < 1)
				{
					if (!drag)
					{
						int x = Input.MousePoint.X - lastMousePos.X;
						int y = Input.MousePoint.Y - lastMousePos.Y;
						if ((x * x) + (y * y) > 6)
						{
							drag = true;
						}
					}
					if (drag)
					{
						lastMousePos = Input.MousePoint;
					}
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
			using (Demo demo = new SoftDemo())
			{
				GraphicsLibraryManager.Run(demo);
			}
		}
	}
}
