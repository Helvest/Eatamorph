using UnityEngine;

namespace BulletUnity.Primitives
{
	/// <summary>
	/// Basic BBox
	/// </summary>
	[RequireComponent(typeof(BRigidBody))]
	[RequireComponent(typeof(BConvexHullShape))]
	public class BConvexHull : BPrimitive
	{
		public BUserMeshSettings meshSettings = new BUserMeshSettings();

		public static GameObject CreateNew(Vector3 position, Quaternion rotation)
		{
			var go = new GameObject();
			var bConvexHull = go.AddComponent<BConvexHull>();
			CreateNewBase(go, position, rotation);
			bConvexHull.BuildMesh();
			go.name = "BConvexHull";

			return go;
		}

		public override void BuildMesh()
		{
			var mesh = meshSettings.Build();
			GetComponent<MeshFilter>().sharedMesh = mesh;
			GetComponent<BConvexHullShape>().HullMesh = mesh;
		}
	}
}
