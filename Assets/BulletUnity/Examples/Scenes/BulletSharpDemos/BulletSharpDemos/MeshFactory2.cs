using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using BulletSharp;
using BulletSharp.Math;
using BulletUnity;
using UnityEngine;

namespace DemoFramework
{
	public class MeshFactory2
	{
		public static void CreateShape(CollisionShape shape, Mesh mesh)
		{
			//Debug.Log("Creating Shape " + shape);
			switch (shape.ShapeType)
			{
				case BroadphaseNativeType.BoxShape:
					CreateCube(shape as BoxShape, mesh);
					return;

				case BroadphaseNativeType.Box2DShape:
					CreateBox2DShape(shape as Box2DShape, mesh);
					return;

				case BroadphaseNativeType.CapsuleShape:
					Debug.LogError("Not Implemented " + shape);
					return;

				case BroadphaseNativeType.Convex2DShape:
					CreateShape((shape as Convex2DShape).ChildShape, mesh);
					return;

				case BroadphaseNativeType.ConvexHullShape:
					CreateConvexHull(shape as ConvexHullShape, mesh);
					return;

				case BroadphaseNativeType.ConeShape:
					CreateConeShape((shape as ConeShape), mesh);
					return;

				case BroadphaseNativeType.CylinderShape:
					CreateCylinder(shape as CylinderShape, mesh);
					return;

				case BroadphaseNativeType.GImpactShape:
					CreateTriangleMeshShape((shape as GImpactMeshShape).MeshInterface, mesh);
					return;

				case BroadphaseNativeType.MultiSphereShape:
					Debug.LogError("Not Implemented " + shape);
					return;

				case BroadphaseNativeType.SphereShape:
					CreateSphere(shape as SphereShape, mesh);
					return;

				case BroadphaseNativeType.StaticPlaneShape:
					CreateStaticPlane((shape as StaticPlaneShape), mesh);
					return;

				case BroadphaseNativeType.TriangleMeshShape:
					CreateTriangleMeshShape((shape as TriangleMeshShape).MeshInterface, mesh);
					return;
			}

			if (shape is PolyhedralConvexShape)
			{
				return;
			}

			Debug.LogError("Not Implemented " + shape);

			throw new NotImplementedException();
		}

		public static void CreateConeShape(ConeShape shape, Mesh mesh)
		{
			ProceduralPrimitives.CreateMeshCone(mesh, shape.Height, shape.Radius, 0f, 10);
		}

		public static void CreateBox2DShape(Box2DShape shape, Mesh mesh)
		{
			var v = shape.Vertices;
			MakeUnityCubeMesh(v[0].ToUnity(), v[1].ToUnity(), v[2].ToUnity(), v[3].ToUnity(), v[4].ToUnity(), v[5].ToUnity(), v[6].ToUnity(), v[7].ToUnity(), mesh);
		}

		public static void CreateConvexHull(ConvexHullShape shape, Mesh mesh)
		{
			var hull = new ShapeHull(shape);
			hull.BuildHull(shape.Margin);

			var verts = new List<UnityEngine.Vector3>();
			var tris = new List<int>();

			//int vertexCount = hull.NumVertices;
			var indices = hull.Indices;
			var points = hull.Vertices;

			for (int i = 0; i < indices.Count; i += 3)
			{
				verts.Add(points[(int)indices[i]].ToUnity());
				verts.Add(points[(int)indices[i + 1]].ToUnity());
				verts.Add(points[(int)indices[i + 2]].ToUnity());
				tris.Add(i);
				tris.Add(i + 1);
				tris.Add(i + 2);
			}

			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
		}

		private static void PlaneSpace1(UnityEngine.Vector3 n, out UnityEngine.Vector3 p, out UnityEngine.Vector3 q)
		{
			if (Math.Abs(n[2]) > (Math.Sqrt(2) / 2))
			{
				// choose p in y-z plane
				float a = n[1] * n[1] + n[2] * n[2];
				float k = 1.0f / (float)Math.Sqrt(a);
				p = new UnityEngine.Vector3(0, -n[2] * k, n[1] * k);
				// set q = n x p
				q = UnityEngine.Vector3.Cross(n, p);
			}
			else
			{
				// choose p in x-y plane
				float a = n[0] * n[0] + n[1] * n[1];
				float k = 1.0f / (float)Math.Sqrt(a);
				p = new UnityEngine.Vector3(-n[1] * k, n[0] * k, 0);
				// set q = n x p
				q = UnityEngine.Vector3.Cross(n, p);
			}
		}

		public static void CreateStaticPlane(StaticPlaneShape shape, Mesh m)
		{
			var planeOrigin = shape.PlaneNormal.ToUnity() * shape.PlaneConstant;
			PlaneSpace1(shape.PlaneNormal.ToUnity(), out var vec0, out var vec1);
			const float size = 1000f;

			int[] indices = new int[] { 0, 2, 1, 0, 1, 3 };

			var verts = new UnityEngine.Vector3[]
			{
				planeOrigin + vec0*size,
				planeOrigin - vec0*size,
				planeOrigin + vec1*size,
				planeOrigin - vec1*size,
			};

			m.Clear();
			m.vertices = verts;
			m.triangles = indices;
			m.RecalculateBounds();
			m.RecalculateNormals();
		}

		public static void CreateTriangleMeshShape(StridingMeshInterface meshInterface, Mesh m)
		{
			// StridingMeshInterface can only be TriangleIndexVertexArray
			var meshes = (meshInterface as TriangleIndexVertexArray).IndexedMeshArray;
			int numTriangles = 0;
			foreach (var mesh in meshes)
			{
				numTriangles += mesh.NumTriangles;
			}
			int numVertices = numTriangles * 3;
			var vertices = new UnityEngine.Vector3[numVertices * 2];

			int v = 0;
			var triangles = new List<int>();
			for (int part = 0; part < meshInterface.NumSubParts; part++)
			{
				var mesh = meshes[part];

				var indexStream = mesh.GetTriangleStream();
				var vertexStream = mesh.GetVertexStream();
				var indexReader = new BinaryReader(indexStream);
				var vertexReader = new BinaryReader(vertexStream);

				int vertexStride = mesh.VertexStride;
				int triangleStrideDelta = mesh.TriangleIndexStride - 3 * sizeof(int);

				while (indexStream.Position < indexStream.Length)
				{
					uint i = indexReader.ReadUInt32();
					vertexStream.Position = vertexStride * i;
					float f1 = vertexReader.ReadSingle();
					float f2 = vertexReader.ReadSingle();
					float f3 = vertexReader.ReadSingle();
					var v0 = new UnityEngine.Vector3(f1, f2, f3);
					i = indexReader.ReadUInt32();
					vertexStream.Position = vertexStride * i;
					f1 = vertexReader.ReadSingle();
					f2 = vertexReader.ReadSingle();
					f3 = vertexReader.ReadSingle();
					var v1 = new UnityEngine.Vector3(f1, f2, f3);
					i = indexReader.ReadUInt32();
					vertexStream.Position = vertexStride * i;
					f1 = vertexReader.ReadSingle();
					f2 = vertexReader.ReadSingle();
					f3 = vertexReader.ReadSingle();
					var v2 = new UnityEngine.Vector3(f1, f2, f3);

					var v01 = v0 - v1;
					var v02 = v0 - v2;
					var normal = UnityEngine.Vector3.Cross(v01, v02);
					normal.Normalize();

					triangles.Add(v);
					triangles.Add(v + 1);
					triangles.Add(v + 2);
					vertices[v++] = v0;
					vertices[v++] = v1;
					vertices[v++] = v2;

					indexStream.Position += triangleStrideDelta;
				}

				indexStream.Dispose();
				vertexStream.Dispose();
			}

			m.Clear();
			m.vertices = vertices;
			m.triangles = triangles.ToArray();
			m.RecalculateBounds();
			m.RecalculateNormals();
		}

		public static void CreateCylinder(CylinderShape cs, Mesh mesh)
		{
			mesh.Clear();
			//float r = cs.Radius;
			//todo this is a cube
			mesh.Clear();
			var ext = cs.HalfExtentsWithMargin;
			float length = ext.X * 2f;
			float width = ext.Y * 2f;
			float height = ext.Z * 2f;


			var p0 = new UnityEngine.Vector3(-length * .5f, -width * .5f, height * .5f);
			var p1 = new UnityEngine.Vector3(length * .5f, -width * .5f, height * .5f);
			var p2 = new UnityEngine.Vector3(length * .5f, -width * .5f, -height * .5f);
			var p3 = new UnityEngine.Vector3(-length * .5f, -width * .5f, -height * .5f);

			var p4 = new UnityEngine.Vector3(-length * .5f, width * .5f, height * .5f);
			var p5 = new UnityEngine.Vector3(length * .5f, width * .5f, height * .5f);
			var p6 = new UnityEngine.Vector3(length * .5f, width * .5f, -height * .5f);
			var p7 = new UnityEngine.Vector3(-length * .5f, width * .5f, -height * .5f);

			var vertices = new UnityEngine.Vector3[]
			{
				// Bottom
				p0, p1, p2, p3,
 
				// Left
				p7, p4, p0, p3,
 
				// Front
				p4, p5, p1, p0,
 
				// Back
				p6, p7, p3, p2,
 
				// Right
				p5, p6, p2, p1,
 
				// Top
				p7, p6, p5, p4
			};



			var up = UnityEngine.Vector3.up;
			var down = UnityEngine.Vector3.down;
			var front = UnityEngine.Vector3.forward;
			var back = UnityEngine.Vector3.back;
			var left = UnityEngine.Vector3.left;
			var right = UnityEngine.Vector3.right;

			var normales = new UnityEngine.Vector3[]
			{
				// Bottom
				down, down, down, down,
 
				// Left
				left, left, left, left,
 
				// Front
				front, front, front, front,
 
				// Back
				back, back, back, back,
 
				// Right
				right, right, right, right,
 
				// Top
				up, up, up, up
			};

			var _00 = new Vector2(0f, 0f);
			var _10 = new Vector2(1f, 0f);
			var _01 = new Vector2(0f, 1f);
			var _11 = new Vector2(1f, 1f);

			var uvs = new Vector2[]
			{
				// Bottom
				_11, _01, _00, _10,
 
				// Left
				_11, _01, _00, _10,
 
				// Front
				_11, _01, _00, _10,
 
				// Back
				_11, _01, _00, _10,
 
				// Right
				_11, _01, _00, _10,
 
				// Top
				_11, _01, _00, _10,
			};

			int[] triangles = new int[]
			{
				// Bottom
				3, 1, 0,
				3, 2, 1,			
 
				// Left
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
				// Front
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
				// Back
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
				// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
				// Top
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
			};

			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
		}

		public static void CreateCube(CollisionShape cs, Mesh mesh)
		{
			var ext = ((BoxShape)cs).HalfExtentsWithMargin;

			float length = ext.X * 2f;
			float width = ext.Y * 2f;
			float height = ext.Z * 2f;

			var p0 = new UnityEngine.Vector3(-length * .5f, -width * .5f, height * .5f);
			var p1 = new UnityEngine.Vector3(length * .5f, -width * .5f, height * .5f);
			var p2 = new UnityEngine.Vector3(length * .5f, -width * .5f, -height * .5f);
			var p3 = new UnityEngine.Vector3(-length * .5f, -width * .5f, -height * .5f);

			var p4 = new UnityEngine.Vector3(-length * .5f, width * .5f, height * .5f);
			var p5 = new UnityEngine.Vector3(length * .5f, width * .5f, height * .5f);
			var p6 = new UnityEngine.Vector3(length * .5f, width * .5f, -height * .5f);
			var p7 = new UnityEngine.Vector3(-length * .5f, width * .5f, -height * .5f);

			MakeUnityCubeMesh(p0, p1, p2, p3, p4, p5, p6, p7, mesh);
		}

		private static void MakeUnityCubeMesh(
			UnityEngine.Vector3 p0, UnityEngine.Vector3 p1, UnityEngine.Vector3 p2, UnityEngine.Vector3 p3,
			UnityEngine.Vector3 p4, UnityEngine.Vector3 p5, UnityEngine.Vector3 p6, UnityEngine.Vector3 p7,
			Mesh mesh)
		{
			mesh.Clear();
			var vertices = new UnityEngine.Vector3[]
			{
				// Bottom
				p0, p1, p2, p3,
 
				// Left
				p7, p4, p0, p3,
 
				// Front
				p4, p5, p1, p0,
 
				// Back
				p6, p7, p3, p2,
 
				// Right
				p5, p6, p2, p1,
 
				// Top
				p7, p6, p5, p4
			};

			var up = UnityEngine.Vector3.up;
			var down = UnityEngine.Vector3.down;
			var front = UnityEngine.Vector3.forward;
			var back = UnityEngine.Vector3.back;
			var left = UnityEngine.Vector3.left;
			var right = UnityEngine.Vector3.right;

			var normales = new UnityEngine.Vector3[]
			{
				// Bottom
				down, down, down, down,
 
				// Left
				left, left, left, left,
 
				// Front
				front, front, front, front,
 
				// Back
				back, back, back, back,
 
				// Right
				right, right, right, right,
 
				// Top
				up, up, up, up
			};

			var _00 = new Vector2(0f, 0f);
			var _10 = new Vector2(1f, 0f);
			var _01 = new Vector2(0f, 1f);
			var _11 = new Vector2(1f, 1f);

			var uvs = new Vector2[]
			{
				// Bottom
				_11, _01, _00, _10,
 
				// Left
				_11, _01, _00, _10,
 
				// Front
				_11, _01, _00, _10,
 
				// Back
				_11, _01, _00, _10,
 
				// Right
				_11, _01, _00, _10,
 
				// Top
				_11, _01, _00, _10,
			};

			int[] triangles = new int[]
			{
				// Bottom
				3, 1, 0,
				3, 2, 1,			
 
				// Left
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
				// Front
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
				// Back
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
				// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
				// Top
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
			};

			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
		}

		public static void CreateSphere(SphereShape shape, Mesh mesh)
		{
			mesh.Clear();

			float radius = shape.Radius;
			// Longitude |||
			int nbLong = 24;
			// Latitude ---
			int nbLat = 16;

			#region Vertices
			var vertices = new UnityEngine.Vector3[(nbLong + 1) * nbLat + 2];
			float _pi = Mathf.PI;
			float _2pi = _pi * 2f;

			vertices[0] = UnityEngine.Vector3.up * radius;
			for (int lat = 0; lat < nbLat; lat++)
			{
				float a1 = _pi * (lat + 1) / (nbLat + 1);
				float sin1 = Mathf.Sin(a1);
				float cos1 = Mathf.Cos(a1);

				for (int lon = 0; lon <= nbLong; lon++)
				{
					float a2 = _2pi * (lon == nbLong ? 0 : lon) / nbLong;
					float sin2 = Mathf.Sin(a2);
					float cos2 = Mathf.Cos(a2);

					vertices[lon + lat * (nbLong + 1) + 1] = new UnityEngine.Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
				}
			}
			vertices[vertices.Length - 1] = UnityEngine.Vector3.up * -radius;
			#endregion

			#region Normales		
			var normales = new UnityEngine.Vector3[vertices.Length];
			for (int n = 0; n < vertices.Length; n++)
			{
				normales[n] = vertices[n].normalized;
			}
			#endregion

			#region UVs
			var uvs = new Vector2[vertices.Length];
			uvs[0] = Vector2.up;
			uvs[uvs.Length - 1] = Vector2.zero;
			for (int lat = 0; lat < nbLat; lat++)
			{
				for (int lon = 0; lon <= nbLong; lon++)
				{
					uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
				}
			}
			#endregion

			#region Triangles
			int nbFaces = vertices.Length;
			int nbTriangles = nbFaces * 2;
			int nbIndexes = nbTriangles * 3;
			int[] triangles = new int[nbIndexes];

			//Top Cap
			int i = 0;
			for (int lon = 0; lon < nbLong; lon++)
			{
				triangles[i++] = lon + 2;
				triangles[i++] = lon + 1;
				triangles[i++] = 0;
			}

			//Middle
			for (int lat = 0; lat < nbLat - 1; lat++)
			{
				for (int lon = 0; lon < nbLong; lon++)
				{
					int current = lon + lat * (nbLong + 1) + 1;
					int next = current + nbLong + 1;

					triangles[i++] = current;
					triangles[i++] = current + 1;
					triangles[i++] = next + 1;

					triangles[i++] = current;
					triangles[i++] = next + 1;
					triangles[i++] = next;
				}
			}

			//Bottom Cap
			for (int lon = 0; lon < nbLong; lon++)
			{
				triangles[i++] = vertices.Length - 1;
				triangles[i++] = vertices.Length - (lon + 2) - 1;
				triangles[i++] = vertices.Length - (lon + 1) - 1;
			}
			#endregion

			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
		}
	}
}
