using UnityEngine;

namespace BulletUnity.Primitives
{
	/// <summary>
	/// Basic BBox
	/// </summary>
	[RequireComponent(typeof(BRigidBody))]
	[RequireComponent(typeof(BConvexTriangleMeshShape))]
	public class BConvexTriMesh : BPrimitive
	{
		public BUserMeshSettings meshSettings = new BUserMeshSettings();

		public static GameObject CreateNew(Vector3 position, Quaternion rotation)
		{
			var go = new GameObject();
			var bConvexHull = go.AddComponent<BConvexHull>();
			CreateNewBase(go, position, rotation);
			bConvexHull.BuildMesh();
			go.name = "BConvexTriMesh";

			return go;
		}

		public override void BuildMesh()
		{
			var mesh = meshSettings.Build();
			GetComponent<MeshFilter>().sharedMesh = mesh;
			GetComponent<BConvexTriangleMeshShape>().HullMesh = mesh;
		}
	}
}
