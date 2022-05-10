using UnityEngine;

namespace BulletUnity.Primitives
{
	/// <summary>
	/// BCylinder
	/// </summary>
	[RequireComponent(typeof(BRigidBody))]
	[RequireComponent(typeof(BCapsuleShape))]
	public class BCapsule : BPrimitive
	{
		public BCapsuleMeshSettings meshSettings = new BCapsuleMeshSettings();

		public static GameObject CreateNew(Vector3 position, Quaternion rotation)
		{
			var go = new GameObject();
			var bCylinder = go.AddComponent<BCapsule>();
			CreateNewBase(go, position, rotation);
			bCylinder.BuildMesh();
			go.name = "BCapsule";

			return go;
		}

		public override void BuildMesh()
		{
			GetComponent<MeshFilter>().sharedMesh = meshSettings.Build();
			var cs = GetComponent<BCapsuleShape>();
			cs.Height = meshSettings.height;
			cs.Radius = meshSettings.radius;
			cs.UpAxis = meshSettings.upAxis;
		}
	}
}
