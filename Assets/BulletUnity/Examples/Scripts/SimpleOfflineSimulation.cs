using System.Collections.Generic;
using BulletSharp;
using BulletSharp.Math;
using UnityEngine;

/* 
A simple physics simulation that is not connected in any way to the Unity scene 
*/
public class SimpleOfflineSimulation : MonoBehaviour
{
	private void Start()
	{
		//Create a World
		Debug.Log("Initialize physics");
		var CollisionShapes = new List<CollisionShape>();

		var CollisionConf = new DefaultCollisionConfiguration();
		var Dispatcher = new CollisionDispatcher(CollisionConf);

		var Broadphase = new DbvtBroadphase();

		var World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf)
		{
			Gravity = new BulletSharp.Math.Vector3(0, -9.80665f, 0)
		};

		// create a few dynamic rigidbodies
		const float mass = 1.0f;

		//Add a single cube
		RigidBody fallRigidBody;
		var shape = new BoxShape(1f, 1f, 1f);

		var localInertia = BulletSharp.Math.Vector3.Zero;
		shape.CalculateLocalInertia(mass, out localInertia);

		var rbInfo = new RigidBodyConstructionInfo(mass, null, shape, localInertia);

		fallRigidBody = new RigidBody(rbInfo);
		rbInfo.Dispose();

		var st = Matrix.Translation(new BulletSharp.Math.Vector3(0f, 10f, 0f));

		fallRigidBody.WorldTransform = st;

		World.AddRigidBody(fallRigidBody);

		//Step the simulation 300 steps
		for (int i = 0; i < 300; i++)
		{
			World.StepSimulation(1f / 60f, 10);

			fallRigidBody.GetWorldTransform(out var trans);

			Debug.Log("box height: " + trans.Origin);
		}

		//Clean up.
		World.RemoveRigidBody(fallRigidBody);
		fallRigidBody.Dispose();

		Debug.Log("ExitPhysics");

		if (World != null)
		{
			//remove/dispose constraints
			int i;

			for (i = World.NumConstraints - 1; i >= 0; i--)
			{
				var constraint = World.GetConstraint(i);
				World.RemoveConstraint(constraint);
				constraint.Dispose();
			}

			//remove the rigidbodies from the dynamics world and delete them
			for (i = World.NumCollisionObjects - 1; i >= 0; i--)
			{
				var obj = World.CollisionObjectArray[i];
				var body = obj as RigidBody;

				if (body != null && body.MotionState != null)
				{
					body.MotionState.Dispose();
				}

				World.RemoveCollisionObject(obj);
				obj.Dispose();
			}

			//delete collision shapes
			foreach (var ss in CollisionShapes)
			{
				ss.Dispose();
			}

			CollisionShapes.Clear();

			World.Dispose();
			Broadphase.Dispose();
			Dispatcher.Dispose();
			CollisionConf.Dispose();
		}

		if (Broadphase != null)
		{
			Broadphase.Dispose();
		}

		if (Dispatcher != null)
		{
			Dispatcher.Dispose();
		}

		if (CollisionConf != null)
		{
			CollisionConf.Dispose();
		}
	}
}
