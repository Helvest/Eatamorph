using System;
using BulletSharp;
using BulletSharp.Math;
using BulletSharpExamples;
using BulletUnity;
using DemoFramework;

namespace ConstraintDemo
{
	internal class ConstraintDemo : Demo
	{
		private Vector3 eye = new Vector3(35, 10, 35);
		private Vector3 target = new Vector3(0, 5, 0);
		private const DebugDrawModes debugMode = DebugDrawModes.DrawConstraints | DebugDrawModes.DrawConstraintLimits;

		public const float CubeHalfExtents = 1.0f;
		private Vector3 lowerSliderLimit = new Vector3(-10, 0, 0);
		private Vector3 hiSliderLimit = new Vector3(10, 0, 0);

		public RigidBody d6body0;
		private Generic6DofConstraint spSlider6Dof;
		private HingeConstraint spDoorHinge;
		private HingeConstraint spHingeDynAB;
		private ConeTwistConstraint coneTwist;

		protected override void OnInitialize()
		{
			Freelook.SetEyeTarget(eye, target);

			Graphics.SetFormText("BulletSharp - Constraints Demo");
			Graphics.SetInfoText("Move using mouse and WASD+shift\n" +
				"F3 - Toggle debug\n" +
				//"F11 - Toggle fullscreen\n" +
				"Space - Shoot box");

			IsDebugDrawEnabled = false;
			DebugDrawMode = debugMode;
		}

		private void SetupEmptyDynamicsWorld()
		{
			CollisionConf = new DefaultCollisionConfiguration();
			Dispatcher = new CollisionDispatcher(CollisionConf);
			Broadphase = new DbvtBroadphase();
			Solver = new SequentialImpulseConstraintSolver();
			World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf)
			{
				Gravity = new Vector3(0, -10, 0)
			};
		}

		protected override void OnInitializePhysics()
		{
			SetupEmptyDynamicsWorld();

			CollisionShape groundShape = new BoxShape(50, 1, 50);
			//CollisionShape groundShape = new StaticPlaneShape(Vector3.UnitY, 40);
			CollisionShapes.Add(groundShape);
			var body = LocalCreateRigidBody(0, Matrix.Translation(0, -16, 0), groundShape);
			body.UserObject = "Ground";

			CollisionShape shape = new BoxShape(new Vector3(CubeHalfExtents));
			CollisionShapes.Add(shape);


			const float THETA = (float)Math.PI / 4.0f;
			float L_1 = 2 - (float)Math.Tan(THETA);
			float L_2 = 1 / (float)Math.Cos(THETA);
			float RATIO = L_2 / L_1;

			RigidBody bodyA;
			RigidBody bodyB;

			CollisionShape cylA = new CylinderShape(0.2f, 0.25f, 0.2f);
			CollisionShape cylB = new CylinderShape(L_1, 0.025f, L_1);
			var cyl0 = new CompoundShape();
			cyl0.AddChildShape(Matrix.Identity, cylA);
			cyl0.AddChildShape(Matrix.Identity, cylB);

			float mass = 6.28f;
			cyl0.CalculateLocalInertia(mass, out var localInertia);
			var ci = new RigidBodyConstructionInfo(mass, null, cyl0, localInertia)
			{
				StartWorldTransform = Matrix.Translation(-8, 1, -8)
			};

			body = new RigidBody(ci); //1,0,cyl0,localInertia);
			World.AddRigidBody(body);
			body.LinearFactor = Vector3.Zero;
			body.AngularFactor = new Vector3(0, 1, 0);
			bodyA = body;

			cylA = new CylinderShape(0.2f, 0.26f, 0.2f);
			cylB = new CylinderShape(L_2, 0.025f, L_2);
			cyl0 = new CompoundShape();
			cyl0.AddChildShape(Matrix.Identity, cylA);
			cyl0.AddChildShape(Matrix.Identity, cylB);

			mass = 6.28f;
			cyl0.CalculateLocalInertia(mass, out localInertia);
			ci = new RigidBodyConstructionInfo(mass, null, cyl0, localInertia);
			var orn = Quaternion.RotationAxis(new Vector3(0, 0, 1), -THETA);
			ci.StartWorldTransform = Matrix.RotationQuaternion(orn) * Matrix.Translation(-10, 2, -8);

			body = new RigidBody(ci)
			{
				LinearFactor = Vector3.Zero
			};//1,0,cyl0,localInertia);
			var hinge = new HingeConstraint(body, Vector3.Zero, new Vector3(0, 1, 0), true);
			World.AddConstraint(hinge);
			bodyB = body;
			body.AngularVelocity = new Vector3(0, 3, 0);

			World.AddRigidBody(body);

			var axisA = new Vector3(0, 1, 0);
			var axisB = new Vector3(0, 1, 0);
			orn = Quaternion.RotationAxis(new Vector3(0, 0, 1), -THETA);
			var mat = Matrix.RotationQuaternion(orn);
			axisB = new Vector3(mat.M21, mat.M22, mat.M23);

			var gear = new GearConstraint(bodyA, bodyB, axisA, axisB, RATIO);
			World.AddConstraint(gear, true);


			mass = 1.0f;

			var body0 = LocalCreateRigidBody(mass, Matrix.Translation(0, 20, 0), shape);

			//RigidBody body1 = null;//LocalCreateRigidBody(mass, Matrix.Translation(2*CUBE_HALF_EXTENTS,20,0), shape);
			//RigidBody body1 = LocalCreateRigidBody(0, Matrix.Translation(2*CUBE_HALF_EXTENTS,20,0), null);
			//body1.ActivationState = ActivationState.DisableDeactivation;
			//body1.SetDamping(0.3f, 0.3f);

			var pivotInA = new Vector3(CubeHalfExtents, -CubeHalfExtents, -CubeHalfExtents);
			var axisInA = new Vector3(0, 0, 1);

			/*
            Vector3 pivotInB;
            if (body1 != null)
            {
                Matrix transform = Matrix.Invert(body1.CenterOfMassTransform) * body0.CenterOfMassTransform;
                pivotInB = Vector3.TransformCoordinate(pivotInA, transform);
            }
            else
            {
                pivotInB = pivotInA;
            }
            */

			/*
            Vector3 axisInB;
            if (body1 != null)
            {
                Matrix transform = Matrix.Invert(body1.CenterOfMassTransform) * body1.CenterOfMassTransform;
                axisInB = Vector3.TransformCoordinate(axisInA, transform);
            }
            else
            {
                axisInB = Vector3.TransformCoordinate(axisInA, body0.CenterOfMassTransform);
            }
            */

#if P2P
            {
                TypedConstraint p2p = new Point2PointConstraint(body0, pivotInA);
                //TypedConstraint p2p = new Point2PointConstraint(body0, body1, pivotInA, pivotInB);
                //TypedConstraint hinge = new HingeConstraint(body0, body1, pivotInA, pivotInB, axisInA, axisInB);
                World.AddConstraint(p2p);
                p2p.DebugDrawSize = 5;
            }
#else
			{
				hinge = new HingeConstraint(body0, pivotInA, axisInA);

				//use zero targetVelocity and a small maxMotorImpulse to simulate joint friction
				//float	targetVelocity = 0.f;
				//float	maxMotorImpulse = 0.01;
				const float targetVelocity = 1.0f;
				const float maxMotorImpulse = 1.0f;
				hinge.EnableAngularMotor(true, targetVelocity, maxMotorImpulse);
				World.AddConstraint(hinge);
				hinge.DebugDrawSize = 5;
			}
#endif

			var pRbA1 = LocalCreateRigidBody(mass, Matrix.Translation(-20, 0, 30), shape);
			//RigidBody pRbA1 = LocalCreateRigidBody(0.0f, Matrix.Translation(-20, 0, 30), shape);
			pRbA1.ActivationState = ActivationState.DisableDeactivation;

			// add dynamic rigid body B1
			var pRbB1 = LocalCreateRigidBody(mass, Matrix.Translation(-20, 0, 30), shape);
			//RigidBody pRbB1 = LocalCreateRigidBody(0.0f, Matrix.Translation(-20, 0, 30), shape);
			pRbB1.ActivationState = ActivationState.DisableDeactivation;

			// create slider constraint between A1 and B1 and add it to world
			var spSlider1 = new SliderConstraint(pRbA1, pRbB1, Matrix.Identity, Matrix.Identity, true)
			{
				//spSlider1 = new SliderConstraint(pRbA1, pRbB1, Matrix.Identity, Matrix.Identity, false);
				LowerLinearLimit = -15.0f,
				UpperLinearLimit = -5.0f
			};
			spSlider1.LowerLinearLimit = 5.0f;
			spSlider1.UpperLinearLimit = 15.0f;
			spSlider1.LowerLinearLimit = -10.0f;
			spSlider1.UpperLinearLimit = -10.0f;

			spSlider1.LowerAngularLimit = -(float)Math.PI / 3.0f;
			spSlider1.UpperAngularLimit = (float)Math.PI / 3.0f;

			World.AddConstraint(spSlider1, true);
			spSlider1.DebugDrawSize = 5.0f;


			//create a slider, using the generic D6 constraint
			var sliderWorldPos = new Vector3(0, 10, 0);
			var sliderAxis = Vector3.UnitX;
			const float angle = 0; //SIMD_RADS_PER_DEG * 10.f;
			var trans = Matrix.RotationAxis(sliderAxis, angle) * Matrix.Translation(sliderWorldPos);
			d6body0 = LocalCreateRigidBody(mass, trans, shape);
			d6body0.ActivationState = ActivationState.DisableDeactivation;

			var fixedBody1 = LocalCreateRigidBody(0, trans, null);
			World.AddRigidBody(fixedBody1);

			var frameInA = Matrix.Translation(0, 5, 0);
			var frameInB = Matrix.Translation(0, 5, 0);

			//bool useLinearReferenceFrameA = false;//use fixed frame B for linear llimits
			const bool useLinearReferenceFrameA = true; //use fixed frame A for linear llimits
			spSlider6Dof = new Generic6DofConstraint(fixedBody1, d6body0, frameInA, frameInB, useLinearReferenceFrameA)
			{
				LinearLowerLimit = lowerSliderLimit,
				LinearUpperLimit = hiSliderLimit,

				//range should be small, otherwise singularities will 'explode' the constraint
				//AngularLowerLimit = new Vector3(-1.5f,0,0),
				//AngularUpperLimit = new Vector3(1.5f,0,0),
				//AngularLowerLimit = new Vector3(0,0,0),
				//AngularUpperLimit = new Vector3(0,0,0),
				AngularLowerLimit = new Vector3((float)-Math.PI, 0, 0),
				AngularUpperLimit = new Vector3(1.5f, 0, 0)
			};

			//spSlider6Dof.TranslationalLimitMotor.EnableMotor[0] = true;
			spSlider6Dof.TranslationalLimitMotor.TargetVelocity = new Vector3(-5.0f, 0, 0);
			spSlider6Dof.TranslationalLimitMotor.MaxMotorForce = new Vector3(0.1f, 0, 0);

			World.AddConstraint(spSlider6Dof);
			spSlider6Dof.DebugDrawSize = 5;



			// create a door using hinge constraint attached to the world

			CollisionShape pDoorShape = new BoxShape(2.0f, 5.0f, 0.2f);
			CollisionShapes.Add(pDoorShape);
			var pDoorBody = LocalCreateRigidBody(1.0f, Matrix.Translation(-5.0f, -2.0f, 0.0f), pDoorShape);
			pDoorBody.ActivationState = ActivationState.DisableDeactivation;
			var btPivotA = new Vector3(10.0f + 2.1f, -2.0f, 0.0f); // right next to the door slightly outside
			var btAxisA = Vector3.UnitY; // pointing upwards, aka Y-axis

			spDoorHinge = new HingeConstraint(pDoorBody, btPivotA, btAxisA);

			//spDoorHinge.SetLimit(0.0f, (float)Math.PI / 2);
			// test problem values
			//spDoorHinge.SetLimit(-(float)Math.PI, (float)Math.PI * 0.8f);

			//spDoorHinge.SetLimit(1, -1);
			//spDoorHinge.SetLimit(-(float)Math.PI * 0.8f, (float)Math.PI);
			//spDoorHinge.SetLimit(-(float)Math.PI * 0.8f, (float)Math.PI, 0.9f, 0.3f, 0.0f);
			//spDoorHinge.SetLimit(-(float)Math.PI * 0.8f, (float)Math.PI, 0.9f, 0.01f, 0.0f); // "sticky limits"
			spDoorHinge.SetLimit(-(float)Math.PI * 0.25f, (float)Math.PI * 0.25f);
			//spDoorHinge.SetLimit(0, 0);
			World.AddConstraint(spDoorHinge);
			spDoorHinge.DebugDrawSize = 5;

			/*RigidBody pDropBody =*/
			LocalCreateRigidBody(10.0f, Matrix.Translation(-5.0f, 2.0f, 0.0f), shape);



			// create a generic 6DOF constraint

			//RigidBody pBodyA = LocalCreateRigidBody(mass, Matrix.Translation(10.0f, 6.0f, 0), shape);
			var pBodyA = LocalCreateRigidBody(0, Matrix.Translation(10, 6, 0), shape);
			//RigidBody pBodyA = LocalCreateRigidBody(0, Matrix.Translation(10, 6, 0), null);
			pBodyA.ActivationState = ActivationState.DisableDeactivation;

			var pBodyB = LocalCreateRigidBody(mass, Matrix.Translation(0, 6, 0), shape);
			//RigidBody pBodyB = LocalCreateRigidBody(0, Matrix.Translation(0, 6, 0), shape);
			pBodyB.ActivationState = ActivationState.DisableDeactivation;

			frameInA = Matrix.Translation(-5, 0, 0);
			frameInB = Matrix.Translation(5, 0, 0);

			var pGen6DOF = new Generic6DofConstraint(pBodyA, pBodyB, frameInA, frameInB, true)
			{
				//Generic6DofConstraint pGen6DOF = new Generic6DofConstraint(pBodyA, pBodyB, frameInA, frameInB, false);
				LinearLowerLimit = new Vector3(-10, -2, -1),
				LinearUpperLimit = new Vector3(10, 2, 1),
				//pGen6DOF.LinearLowerLimit = new Vector3(-10, 0, 0);
				//pGen6DOF.LinearUpperLimit = new Vector3(10, 0, 0);
				//pGen6DOF.LinearLowerLimit = new Vector3(0, 0, 0);
				//pGen6DOF.LinearUpperLimit = new Vector3(0, 0, 0);

				//pGen6DOF.TranslationalLimitMotor.EnableMotor[0] = true;
				//pGen6DOF.TranslationalLimitMotor.TargetVelocity = new Vector3(5, 0, 0);
				//pGen6DOF.TranslationalLimitMotor.MaxMotorForce = new Vector3(0.1f, 0, 0);

				//pGen6DOF.AngularLowerLimit = new Vector3(0, (float)Math.PI * 0.9f, 0);
				//pGen6DOF.AngularUpperLimit = new Vector3(0, -(float)Math.PI * 0.9f, 0);
				//pGen6DOF.AngularLowerLimit = new Vector3(0, 0, -(float)Math.PI);
				//pGen6DOF.AngularUpperLimit = new Vector3(0, 0, (float)Math.PI);

				AngularLowerLimit = new Vector3(-(float)Math.PI / 4, -0.75f, -(float)Math.PI * 0.4f),
				AngularUpperLimit = new Vector3((float)Math.PI / 4, 0.75f, (float)Math.PI * 0.4f)
			};
			//pGen6DOF.AngularLowerLimit = new Vector3(0, -0.75f, (float)Math.PI * 0.8f);
			//pGen6DOF.AngularUpperLimit = new Vector3(0, 0.75f, -(float)Math.PI * 0.8f);
			//pGen6DOF.AngularLowerLimit = new Vector3(0, -(float)Math.PI * 0.8f, (float)Math.PI * 1.98f);
			//pGen6DOF.AngularUpperLimit = new Vector3(0, (float)Math.PI * 0.8f, -(float)Math.PI * 1.98f);

			//pGen6DOF.AngularLowerLimit = new Vector3(-0.75f, -0.5f, -0.5f);
			//pGen6DOF.AngularUpperLimit = new Vector3(0.75f, 0.5f, 0.5f);
			//pGen6DOF.AngularLowerLimit = new Vector3(-0.75f, 0, 0);
			//pGen6DOF.AngularUpperLimit = new Vector3(0.75f, 0, 0);
			//pGen6DOF.AngularLowerLimit = new Vector3(0, -0.7f, 0);
			//pGen6DOF.AngularUpperLimit = new Vector3(0, 0.7f, 0);
			//pGen6DOF.AngularLowerLimit = new Vector3(-1, 0, 0);
			//pGen6DOF.AngularUpperLimit = new Vector3(1, 0, 0);



			// create a ConeTwist constraint

			pBodyA = LocalCreateRigidBody(1.0f, Matrix.Translation(-10, 5, 0), shape);
			//pBodyA = LocalCreateRigidBody(0, Matrix.Translation(-10, 5, 0), shape);
			pBodyA.ActivationState = ActivationState.DisableDeactivation;

			pBodyB = LocalCreateRigidBody(0, Matrix.Translation(-10, -5, 0), shape);
			//pBodyB = LocalCreateRigidBody(1.0f, Matrix.Translation(-10, -5, 0), shape);

			frameInA = Matrix.RotationYawPitchRoll(0, 0, (float)Math.PI / 2);
			frameInA *= Matrix.Translation(0, -5, 0);
			frameInB = Matrix.RotationYawPitchRoll(0, 0, (float)Math.PI / 2);
			frameInB *= Matrix.Translation(0, 5, 0);

			coneTwist = new ConeTwistConstraint(pBodyA, pBodyB, frameInA, frameInB);
			//coneTwist.SetLimit((float)Math.PI / 4, (float)Math.PI / 4, (float)Math.PI * 0.8f);
			//coneTwist.SetLimit((((float)Math.PI / 4) * 0.6f), (float)Math.PI / 4, (float)Math.PI * 0.8f, 1.0f); // soft limit == hard limit
			coneTwist.SetLimit((((float)Math.PI / 4) * 0.6f), (float)Math.PI / 4, (float)Math.PI * 0.8f, 0.5f);
			World.AddConstraint(coneTwist, true);
			coneTwist.DebugDrawSize = 5;



			// Hinge connected to the world, with motor (to hinge motor with new and old constraint solver)

			var pBody = LocalCreateRigidBody(1.0f, Matrix.Identity, shape);
			pBody.ActivationState = ActivationState.DisableDeactivation;
			var pivotA = new Vector3(10.0f, 0.0f, 0.0f);
			btAxisA = new Vector3(0.0f, 0.0f, 1.0f);

			var pHinge = new HingeConstraint(pBody, pivotA, btAxisA);
			//pHinge.EnableAngularMotor(true, -1.0f, 0.165f); // use for the old solver
			pHinge.EnableAngularMotor(true, -1.0f, 1.65f); // use for the new SIMD solver
			World.AddConstraint(pHinge);
			pHinge.DebugDrawSize = 5;



			// create a universal joint using generic 6DOF constraint
			// create two rigid bodies
			// static bodyA (parent) on top:
			pBodyA = LocalCreateRigidBody(0, Matrix.Translation(20, 4, 0), shape);
			pBodyA.ActivationState = ActivationState.DisableDeactivation;
			// dynamic bodyB (child) below it :
			pBodyB = LocalCreateRigidBody(1.0f, Matrix.Translation(20, 0, 0), shape);
			pBodyB.ActivationState = ActivationState.DisableDeactivation;
			// add some (arbitrary) data to build constraint frames
			var parentAxis = new Vector3(1, 0, 0);
			var childAxis = new Vector3(0, 0, 1);
			var anchor = new Vector3(20, 2, 0);

			var pUniv = new UniversalConstraint(pBodyA, pBodyB, anchor, parentAxis, childAxis);
			pUniv.SetLowerLimit(-(float)Math.PI / 4, -(float)Math.PI / 4);
			pUniv.SetUpperLimit((float)Math.PI / 4, (float)Math.PI / 4);
			// add constraint to world
			World.AddConstraint(pUniv, true);
			// draw constraint frames and limits for debugging
			pUniv.DebugDrawSize = 5;

			World.AddConstraint(pGen6DOF, true);
			pGen6DOF.DebugDrawSize = 5;



			// create a generic 6DOF constraint with springs 

			pBodyA = LocalCreateRigidBody(0, Matrix.Translation(-20, 16, 0), shape);
			pBodyA.ActivationState = ActivationState.DisableDeactivation;

			pBodyB = LocalCreateRigidBody(1.0f, Matrix.Translation(-10, 16, 0), shape);
			pBodyB.ActivationState = ActivationState.DisableDeactivation;

			frameInA = Matrix.Translation(10, 0, 0);
			frameInB = Matrix.Identity;

			var pGen6DOFSpring = new Generic6DofSpringConstraint(pBodyA, pBodyB, frameInA, frameInB, true)
			{
				LinearUpperLimit = new Vector3(5, 0, 0),
				LinearLowerLimit = new Vector3(-5, 0, 0),
				AngularLowerLimit = new Vector3(0, 0, -1.5f),
				AngularUpperLimit = new Vector3(0, 0, 1.5f),
				DebugDrawSize = 5
			};
			World.AddConstraint(pGen6DOFSpring, true);

			pGen6DOFSpring.EnableSpring(0, true);
			pGen6DOFSpring.SetStiffness(0, 39.478f);
			pGen6DOFSpring.SetDamping(0, 0.5f);
			pGen6DOFSpring.EnableSpring(5, true);
			pGen6DOFSpring.SetStiffness(5, 39.478f);
			pGen6DOFSpring.SetDamping(0, 0.3f);
			pGen6DOFSpring.SetEquilibriumPoint();



			// create a Hinge2 joint
			// create two rigid bodies
			// static bodyA (parent) on top:
			pBodyA = LocalCreateRigidBody(0, Matrix.Translation(-20, 4, 0), shape);
			pBodyA.ActivationState = ActivationState.DisableDeactivation;
			// dynamic bodyB (child) below it :
			pBodyB = LocalCreateRigidBody(1.0f, Matrix.Translation(-20, 0, 0), shape);
			pBodyB.ActivationState = ActivationState.DisableDeactivation;
			// add some data to build constraint frames
			parentAxis = new Vector3(0, 1, 0);
			childAxis = new Vector3(1, 0, 0);
			anchor = new Vector3(-20, 0, 0);
			var pHinge2 = new Hinge2Constraint(pBodyA, pBodyB, anchor, parentAxis, childAxis);
			pHinge2.SetLowerLimit(-(float)Math.PI / 4);
			pHinge2.SetUpperLimit((float)Math.PI / 4);
			// add constraint to world
			World.AddConstraint(pHinge2, true);
			// draw constraint frames and limits for debugging
			pHinge2.DebugDrawSize = 5;



			// create a Hinge joint between two dynamic bodies
			// create two rigid bodies
			// static bodyA (parent) on top:
			pBodyA = LocalCreateRigidBody(1.0f, Matrix.Translation(-20, -2, 0), shape);
			pBodyA.ActivationState = ActivationState.DisableDeactivation;
			// dynamic bodyB:
			pBodyB = LocalCreateRigidBody(10.0f, Matrix.Translation(-30, -2, 0), shape);
			pBodyB.ActivationState = ActivationState.DisableDeactivation;
			// add some data to build constraint frames
			axisA = new Vector3(0, 1, 0);
			axisB = new Vector3(0, 1, 0);
			var pivotA2 = new Vector3(-5, 0, 0);
			var pivotB = new Vector3(5, 0, 0);
			spHingeDynAB = new HingeConstraint(pBodyA, pBodyB, pivotA2, pivotB, axisA, axisB);
			spHingeDynAB.SetLimit(-(float)Math.PI / 4, (float)Math.PI / 4);
			// add constraint to world
			World.AddConstraint(spHingeDynAB, true);
			// draw constraint frames and limits for debugging
			spHingeDynAB.DebugDrawSize = 5;
		}
	}

	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			using (Demo demo = new ConstraintDemo())
			{
				GraphicsLibraryManager.Run(demo);
			}
		}
	}
}
