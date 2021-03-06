using System;
using BulletSharp.SoftBody;
using UnityEngine;

namespace BulletUnity
{
	[RequireComponent(typeof(LineRenderer))]
	public class BSoftBodyRope : BSoftBody
	{
		[Serializable]
		public class RopeSettings
		{
			public int numPointsInRope = 10;
			[Tooltip("Rope start position in world position")]
			public Vector3 startPoint;
			[Tooltip("Rope end position in world position")]
			public Vector3 endPoint;

			public float width = .25f;
			public Color startColor = Color.white;
			public Color endColor = Color.white;
		}

		public RopeSettings meshSettings = new RopeSettings();

		[Tooltip("Rope anchors, if any")]
		public RopeAnchor[] ropeAnchors;

		private int lrVertexCount = 0;

		private LineRenderer _lr;
		private LineRenderer lr => _lr = _lr ?? GetComponent<LineRenderer>();

		internal override bool _BuildCollisionObject()
		{
			if (World == null)
			{
				return false;
			}

			if (meshSettings.numPointsInRope < 2)
			{
				Debug.LogError("There must be at least two points in the rope");
				return false;
			}

			if (SoftBodySettings.totalMass <= 0f)
			{
				Debug.LogError("The rope must have a positive mass");
				return false;
			}

			softBody = SoftBodyHelpers.CreateRope
			(
				World.WorldInfo,
				meshSettings.startPoint.ToBullet(),
				meshSettings.endPoint.ToBullet(),
				meshSettings.numPointsInRope,
				0
			);

			collisionObject = softBody;

			verts = new Vector3[softBody.Nodes.Count];
			norms = new Vector3[softBody.Nodes.Count];

			for (int i = 0; i < softBody.Nodes.Count; i++)
			{
				verts[i] = softBody.Nodes[i].Position.ToUnity();
				norms[i] = softBody.Nodes[i].Normal.ToUnity();
			}

			//Set SB settings
			SoftBodySettings.ConfigureSoftBody(softBody);

			foreach (var anchor in ropeAnchors)
			{
				//anchorNode point 0 to 1, rounds to node # 
				int node = (int)Mathf.Floor(Mathf.Lerp(0, softBody.Nodes.Count - 1, anchor.anchorNodePoint));

				if (anchor.body != null)
				{
					softBody.AppendAnchor(node, (BulletSharp.RigidBody)anchor.body.GetCollisionObject());
				}
				else
				{
					softBody.SetMass(node, 0);  //setting node mass to 0 fixes it in space apparently
				}
			}

			//TODO: lr, Doesnt always work in editor
			var lr = GetComponent<LineRenderer>();

			lr.useWorldSpace = false;

			lr.positionCount = verts.Length;
			lr.startWidth = meshSettings.width;
			lr.endWidth = meshSettings.width;
			lr.startColor = meshSettings.startColor;
			lr.endColor = meshSettings.endColor;

			UpdateMesh();

			return true;
		}

		private void OnDrawGizmosSelected()
		{
			BUtility.DebugDrawRope
			(
				transform.position,
				transform.rotation,
				SoftBodySettings.scale,
				meshSettings.startPoint,
				meshSettings.endPoint,
				meshSettings.numPointsInRope,
				Color.green
			);
		}

		private const string stringName = "BSoftBodyRope";

		/// <summary>
		/// Create new SoftBody object
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="buildNow">Build now or configure properties and call BuildSoftBody() after</param>
		/// <returns></returns>
		public static GameObject CreateNew(Vector3 position, Quaternion rotation, bool buildNow = true)
		{
			var go = new GameObject(stringName);

			go.transform.position = position;
			go.transform.rotation = rotation;

			var bRope = go.AddComponent<BSoftBodyRope>();

			var material = new UnityEngine.Material(Shader.Find("LineRenderFix"));

			bRope.lr.sharedMaterial = material;

			bRope.SoftBodySettings.ResetToSoftBodyPresets(SBSettingsPresets.Rope);

			if (buildNow)
			{
				bRope._BuildCollisionObject();
			}

			return go;
		}

		/// <summary>
		/// Update Rope line renderer at runtime, called from Update 
		/// </summary>
		public override void UpdateMesh()
		{
			if (lr == null)
			{
				return;
			}

			if (lr.enabled == false)
			{
				lr.enabled = true;
			}

			if (lrVertexCount != verts.Length)
			{
				lrVertexCount = verts.Length;

				lr.positionCount = verts.Length;
				lr.startWidth = meshSettings.width;
				lr.endWidth = meshSettings.width;
				lr.startColor = meshSettings.startColor;
				lr.endColor = meshSettings.endColor;
			}

			for (int i = 0; i < verts.Length; i++)
			{
				lr.SetPosition(i, verts[i]);
			}
		}
	}

	[Serializable]
	public class RopeAnchor
	{
		[Tooltip("Anchor to body.  null = anchor to current rope node world position")]
		public BRigidBody body;

		[Range(0, 1)]
		[Tooltip("Anchor point location calulated from total rope lenghth.  Anchor point inserted at ((startPoint - endPoint) * anchorNodePoint; (0 to 1) (0 to 100%)")]
		public float anchorNodePoint;
	}
}
