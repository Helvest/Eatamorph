using System;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using BulletSharp.SoftBody;
using BulletUnity.Primitives;
using UnityEngine;

namespace BulletUnity
{
	public class RBRainTest : MonoBehaviour
	{

		public Rect startAreaOfRain = new Rect(new Vector2(0f, 0f), new Vector2(10f, 10f));
		[Range(0.5f, 300f)]
		public float fromHeight = 30f;
		[Range(0.1f, 20f)]
		public float rigidBodiesPerSecond = 1.0f;
		[Range(1f, 300f)]
		public float lifetime = 10f;

		public bool enableRain = true;

		// SelectMesh ProcedurealMesh = SelectMesh.Sphere;

		private float lastBunnyTime = 0f;

		private void Update()
		{

			if (!enableRain)
			{
				return;
			}

			if ((Time.time - lastBunnyTime) > (1 / rigidBodiesPerSecond))
			{
				var pos = new Vector3(0, 0, 0)
				{
					x = startAreaOfRain.center.x + UnityEngine.Random.Range(-startAreaOfRain.width / 2, startAreaOfRain.width / 2),
					z = startAreaOfRain.center.y + UnityEngine.Random.Range(-startAreaOfRain.height / 2, startAreaOfRain.height / 2),
					y = fromHeight
				};

				var go = BSphere.CreateNew(pos, UnityEngine.Random.rotation);

				var sphere = go.GetComponent<BSphere>();

				sphere.meshSettings.radius = 5.0f;
				sphere.BuildMesh();

				go.GetComponent<BRigidBody>().mass = 50;

				//randomize color for effect
				go.GetComponent<MeshRenderer>().material.color =
				  new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));

				Destroy(go, lifetime);

				lastBunnyTime = Time.time;
			}
		}

		private enum SelectMesh
		{
			//Random,
			Box,
			Sphere,
			Cylinder,
			Cone,
			Pyramid,
			// Plane,
		}




	}
}
