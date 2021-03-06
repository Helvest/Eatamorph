using BulletSharp.SoftBody;
using UnityEngine;

namespace BulletUnity
{
	/// <summary>
	/// Used base for any(most) softbodies needing a mesh and meshrenderer.
	/// </summary>
	public class BSoftBodyWMesh : BSoftBody
	{
		public BUserMeshSettings meshSettings = new BUserMeshSettings();

		private MeshFilter _meshFilter;
		protected MeshFilter meshFilter => _meshFilter = _meshFilter ?? GetComponent<MeshFilter>();

		internal override bool _BuildCollisionObject()
		{
			var mesh = meshSettings.Build();

			if (mesh == null)
			{
				Debug.LogError("Could not build mesh from meshSettings for " + this, transform);
				return false;
			}

			GetComponent<MeshFilter>().sharedMesh = mesh;

#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				return false;
			}
#endif

			if (World == null)
			{
				return false;
			}

			//convert the mesh data to Bullet data and create SoftBody
			var bVerts = new BulletSharp.Math.Vector3[mesh.vertexCount];
			var verts = mesh.vertices;
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				bVerts[i] = verts[i].ToBullet();
			}

			softBody = SoftBodyHelpers.CreateFromTriMesh(World.WorldInfo, bVerts, mesh.triangles);
			collisionObject = softBody;
			SoftBodySettings.ConfigureSoftBody(softBody);//Set SB settings

			//Set SB position to GO position
			softBody.Rotate(transform.rotation.ToBullet());
			softBody.Translate(transform.position.ToBullet());
			softBody.Scale(transform.localScale.ToBullet());

			return true;
		}

		private const string stringName = "BSoftBodyWMesh";

		/// <summary>
		/// Create new SoftBody object using a Mesh
		/// </summary>
		/// <param name="position">World position</param>
		/// <param name="rotation">rotation</param>
		/// <param name="mesh">Need to provide a mesh</param>
		/// <param name="buildNow">Build now or configure properties and call BuildSoftBody() after</param>
		/// <param name="sBpresetSelect">Use a particular softBody configuration pre select values</param>
		/// <returns></returns>
		public static GameObject CreateNew(Vector3 position, Quaternion rotation, Mesh mesh, bool buildNow, SBSettingsPresets sBpresetSelect)
		{
			var go = new GameObject(stringName);
			go.transform.position = position;
			go.transform.rotation = rotation;
			var BSoft = go.AddComponent<BSoftBodyWMesh>();
			go.AddComponent<MeshFilter>();
			var meshRenderer = go.AddComponent<MeshRenderer>();
			BSoft.meshSettings.UserMesh = mesh;
			var material = new UnityEngine.Material(Shader.Find("Standard"));
			meshRenderer.material = material;

			BSoft.SoftBodySettings.ResetToSoftBodyPresets(sBpresetSelect); //Apply SoftBody settings presets

			if (buildNow)
			{
				BSoft._BuildCollisionObject();  //Build the SoftBody
			}

			return go;
		}

		/// <summary>
		/// Update Mesh (or line renderer) at runtime, call from Update 
		/// </summary>
		public override void UpdateMesh()
		{
			var mesh = meshFilter.sharedMesh;
			if (verts != null && verts.Length > 0)
			{
				mesh.vertices = verts;
				mesh.normals = norms;
				mesh.RecalculateBounds();
				transform.SetTransformationFromBulletMatrix(collisionObject.WorldTransform);  //Set SoftBody position, No motionstate    
			}
		}
	}
}