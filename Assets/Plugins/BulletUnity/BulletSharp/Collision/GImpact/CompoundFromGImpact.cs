using BulletSharp.Math;

namespace BulletSharp
{
	internal class MyCallback : TriangleRaycastCallback
	{
		private int _ignorePart;
		private int _ignoreTriangleIndex;

		public MyCallback(ref Vector3 from, ref Vector3 to, int ignorePart, int ignoreTriangleIndex)
			: base(ref from, ref to)
		{
			_ignorePart = ignorePart;
			_ignoreTriangleIndex = ignoreTriangleIndex;
		}

		public override float ReportHit(ref Vector3 hitNormalLocal, float hitFraction, int partId, int triangleIndex)
		{
			if (partId != _ignorePart || triangleIndex != _ignoreTriangleIndex)
			{
				if (hitFraction < HitFraction)
				{
					return hitFraction;
				}
			}

			return HitFraction;
		}
	}

	internal class MyInternalTriangleIndexCallback : InternalTriangleIndexCallback
	{
		private CompoundShape _colShape;
		private float _depth;
		private GImpactMeshShape _meshShape;

		public MyInternalTriangleIndexCallback(CompoundShape colShape, GImpactMeshShape meshShape, float depth)
		{
			_colShape = colShape;
			_depth = depth;
			_meshShape = meshShape;
		}

		public override void InternalProcessTriangleIndex(ref Vector3 vertex0, ref Vector3 vertex1, ref Vector3 vertex2, int partId, int triangleIndex)
		{
			var scale = _meshShape.LocalScaling;
			var v0 = vertex0 * scale;
			var v1 = vertex1 * scale;
			var v2 = vertex2 * scale;

			var centroid = (v0 + v1 + v2) / 3;
			var normal = (v1 - v0).Cross(v2 - v0);
			normal.Normalize();
			var rayFrom = centroid;
			var rayTo = centroid - normal * _depth;

			var cb = new MyCallback(ref rayFrom, ref rayTo, partId, triangleIndex);

			_meshShape.ProcessAllTrianglesRay(cb, ref rayFrom, ref rayTo);
			if (cb.HitFraction < 1)
			{
				rayTo = Vector3.Lerp(cb.From, cb.To, cb.HitFraction);
				//rayTo = cb.From;
				//gDebugDraw.drawLine(tr(centroid),tr(centroid+normal),btVector3(1,0,0));
			}

			var tet = new BuSimplex1To4(v0, v1, v2, rayTo);
			_colShape.AddChildShape(Matrix.Identity, tet);
		}
	}

	public sealed class CompoundFromGImpact
	{
		private CompoundFromGImpact()
		{
		}

		public static CompoundShape Create(GImpactMeshShape gImpactMesh, float depth)
		{
			var colShape = new CompoundShape();
			using (var cb = new MyInternalTriangleIndexCallback(colShape, gImpactMesh, depth))
			{
				gImpactMesh.GetAabb(Matrix.Identity, out var aabbMin, out var aabbMax);
				gImpactMesh.MeshInterface.InternalProcessAllTriangles(cb, aabbMin, aabbMax);
			}
			return colShape;
		}
	}
}
