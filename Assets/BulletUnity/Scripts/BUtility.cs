using UnityEngine;

namespace BulletUnity
{
	public class BUtility
	{
		public const float Two_PI = 6.283185307179586232f;
		public const float RADS_PER_DEG = Two_PI / 360.0f;
		public const float SQRT12 = 0.7071067811865475244008443621048490f;

		public static void DebugDrawRope(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 begin, Vector3 end, int res, Color color)
		{
			Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);

			int r = res + 2;

			float t, tNext;
			Vector3 p, pNext;
			for (int i = 0; i < r; i++)
			{
				t = i * 1.0f / (r - 1);
				tNext = (i + 1) * 1.0f / (r - 1);

				p = Vector3.Lerp(begin, end, t);
				pNext = Vector3.Lerp(begin, end, tNext);

				if (i != r - 1)
				{
					Gizmos.color = color;
					Gizmos.DrawLine(p, pNext); // line
				}

				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(p, 0.045f);
			}
		}

		public static void DebugDrawSphere(Vector3 position, Quaternion rotation, Vector3 scale, float radius, Color color)
		{
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);

			Gizmos.DrawWireSphere(Vector3.zero, radius);
		}

		public static void DebugDrawPatch(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 c00, Vector3 c01, Vector3 c10, Vector3 c11, int resX, int resY, Color color)
		{
			if (resX < 2 || resY < 2)
			{
				return;
			}

			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);

			for (int iy = 0; iy < resY; ++iy)
			{
				for (int ix = 0; ix < resX; ++ix)
				{
					// point 00
					float tx_00 = ix * 1.0f / (resX - 1);
					float ty_00 = iy * 1.0f / (resY - 1);

					var py0_00 = Vector3.Lerp(c00, c01, ty_00);
					var py1_00 = Vector3.Lerp(c10, c11, ty_00);
					var pxy_00 = Vector3.Lerp(py0_00, py1_00, tx_00);

					// point 01
					float tx_01 = (ix + 1) * 1.0f / (resX - 1);
					float ty_01 = iy * 1.0f / (resY - 1);

					var py0_01 = Vector3.Lerp(c00, c01, ty_01);
					var py1_01 = Vector3.Lerp(c10, c11, ty_01);
					var pxy_01 = Vector3.Lerp(py0_01, py1_01, tx_01);

					//point 10
					float tx_10 = ix * 1.0f / (resX - 1);
					float ty_10 = (iy + 1) * 1.0f / (resY - 1);

					var py0_10 = Vector3.Lerp(c00, c01, ty_10);
					var py1_10 = Vector3.Lerp(c10, c11, ty_10);
					var pxy_10 = Vector3.Lerp(py0_10, py1_10, tx_10);

					//point 11
					float tx_11 = (ix + 1) * 1.0f / (resX - 1);
					float ty_11 = (iy + 1) * 1.0f / (resY - 1);

					var py0_11 = Vector3.Lerp(c00, c01, ty_11);
					var py1_11 = Vector3.Lerp(c10, c11, ty_11);
					var pxy_11 = Vector3.Lerp(py0_11, py1_11, tx_11);

					Gizmos.DrawLine(pxy_00, pxy_01);
					Gizmos.DrawLine(pxy_01, pxy_11);
					Gizmos.DrawLine(pxy_00, pxy_11);
					Gizmos.DrawLine(pxy_00, pxy_10);
					Gizmos.DrawLine(pxy_10, pxy_11);
				}
			}
		}

		/*	
            //it is very slow, so don't use it if you don't need it indeed..
            public static void DebugDrawPolyhedron(Vector3 position,Quaternion rotation,Vector3 scale,btPolyhedralConvexShape shape,Color color)
            {
                if( shape == null )
                    return;

                Matrix4x4 matrix = Matrix4x4.TRS(position,rotation,scale);
                Gizmos.color = color;
                btConvexPolyhedron poly = shape.getConvexPolyhedron();
                if( poly == null )
                    return;

                int faceSize = poly.m_faces.size();
                for (int i=0;i < faceSize;i++)
                {
                    Vector3 centroid = new Vector3(0,0,0);
                    btFace face = poly.m_faces.at(i);
                    int numVerts = face.m_indices.size();
                    if (numVerts > 0)
                    {
                        int lastV = face.m_indices.at(numVerts-1);
                        for (int v=0;v < numVerts;v++)
                        {
                            int curVert = face.m_indices.at(v);
                            BulletSharp.Math.Vector3 curVertObject = BulletSharp.Math.Vector3.GetObjectFromSwigPtr(poly.m_vertices.at(curVert));
                            centroid.x += curVertObject.x();
                            centroid.y += curVertObject.y();
                            centroid.z += curVertObject.z();
                            BulletSharp.Math.Vector3 btv1 = BulletSharp.Math.Vector3.GetObjectFromSwigPtr(poly.m_vertices.at(lastV));
                            BulletSharp.Math.Vector3 btv2 = BulletSharp.Math.Vector3.GetObjectFromSwigPtr(poly.m_vertices.at(curVert));
                            Vector3 v1 = new Vector3(btv1.x(),btv1.y(),btv1.z());
                            Vector3 v2 = new Vector3(btv2.x(),btv2.y(),btv2.z());
                            v1 = matrix.MultiplyPoint(v1);
                            v2 = matrix.MultiplyPoint(v2);
                            Gizmos.DrawLine(v1,v2);
                            lastV = curVert;
                        }
                    }
                    float s = 1.0f/numVerts;
                    centroid.x *= s;
                    centroid.y *= s;
                    centroid.z *= s;

                    //normal draw
        //            {
        //                Vector3 normalColor = new Vector3(1,1,0);
        //				 
        //                BulletSharp.Math.Vector3 faceNormal(face.m_plane[0],poly->m_faces[i].m_plane[1],poly->m_faces[i].m_plane[2]);
        //                getDebugDrawer()->drawLine(worldTransform*centroid,worldTransform*(centroid+faceNormal),normalColor);
        //            }

                }
            }
        */

		public static void DebugDrawBox(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 maxVec, Color color)
		{
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);

			Gizmos.DrawWireCube(Vector3.zero, maxVec * 2f);
		}

		public static void DebugDrawCapsule(Vector3 position, Quaternion rotation, Vector3 scale, float radius, float halfHeight, int upAxis, Color color)
		{
			switch (upAxis)
			{
				default:
				case 0:
					halfHeight *= scale.x;
					scale = Vector3.one * scale.z;
					break;

				case 1:
					halfHeight *= scale.y;
					scale = Vector3.one * scale.x;
					break;

				case 2:
					halfHeight *= scale.z;
					scale = Vector3.one * scale.y;
					break;
			}

			halfHeight /= scale.x;

			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);

			var capStart = Vector3.zero;
			capStart[upAxis] = -halfHeight;

			var capEnd = Vector3.zero;
			capEnd[upAxis] = halfHeight;

			Gizmos.DrawWireSphere(capStart, radius);
			Gizmos.DrawWireSphere(capEnd, radius);

			// Draw some additional lines

			capStart[(upAxis + 1) % 3] = radius;
			capEnd[(upAxis + 1) % 3] = radius;
			Gizmos.DrawLine(capStart, capEnd);

			capStart[(upAxis + 1) % 3] = -radius;
			capEnd[(upAxis + 1) % 3] = -radius;
			Gizmos.DrawLine(capStart, capEnd);

			capStart[(upAxis + 1) % 3] = 0f;
			capEnd[(upAxis + 1) % 3] = 0f;

			capStart[(upAxis + 2) % 3] = radius;
			capEnd[(upAxis + 2) % 3] = radius;
			Gizmos.DrawLine(capStart, capEnd);

			capStart[(upAxis + 2) % 3] = -radius;
			capEnd[(upAxis + 2) % 3] = -radius;
			Gizmos.DrawLine(capStart, capEnd);
		}

		public static void DebugDrawCylinder(Vector3 position, Quaternion rotation, Vector3 scale, float radius, float halfHeight, int upAxis, Color color)
		{
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.identity;

			var start = position;
			var offsetHeight = Vector3.zero;
			offsetHeight[upAxis] = halfHeight;
			var offsetRadius = Vector3.zero;
			offsetRadius[(upAxis + 1) % 3] = radius;

			offsetHeight.x *= scale.x;
			offsetHeight.y *= scale.y;
			offsetHeight.z *= scale.z;
			offsetRadius.x *= scale.x;
			offsetRadius.y *= scale.y;
			offsetRadius.z *= scale.z;

			Gizmos.DrawLine(start + rotation * (offsetHeight + offsetRadius), start + rotation * (-offsetHeight + offsetRadius));
			Gizmos.DrawLine(start + rotation * (offsetHeight - offsetRadius), start + rotation * (-offsetHeight - offsetRadius));

			// Drawing top and bottom caps of the cylinder
			var yaxis = new Vector3(0, 0, 0);
			yaxis[upAxis] = 1.0f;
			var xaxis = new Vector3(0, 0, 0);
			xaxis[(upAxis + 1) % 3] = 1.0f;

			float r = offsetRadius.magnitude;
			DebugDrawArc(start - rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, r, r, 0, Two_PI, color, false, 10.0f);
			DebugDrawArc(start + rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, r, r, 0, Two_PI, color, false, 10.0f);
		}

		public static void DebugDrawCone(Vector3 position, Quaternion rotation, Vector3 scale, float radius, float height, int upAxis, Color color)
		{
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.identity;

			var start = position;

			var offsetHeight = new Vector3(0, 0, 0);
			offsetHeight[upAxis] = height * 0.5f;
			var offsetRadius = new Vector3(0, 0, 0);
			offsetRadius[(upAxis + 1) % 3] = radius;
			var offset2Radius = new Vector3(0, 0, 0);
			offset2Radius[(upAxis + 2) % 3] = radius;

			offsetHeight.x *= scale.x;
			offsetHeight.y *= scale.y;
			offsetHeight.z *= scale.z;
			offsetRadius.x *= scale.x;
			offsetRadius.y *= scale.y;
			offsetRadius.z *= scale.z;
			offset2Radius.x *= scale.x;
			offset2Radius.y *= scale.y;
			offset2Radius.z *= scale.z;

			Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight + offsetRadius));
			Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight - offsetRadius));
			Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight + offset2Radius));
			Gizmos.DrawLine(start + rotation * (offsetHeight), start + rotation * (-offsetHeight - offset2Radius));

			// Drawing the base of the cone
			var yaxis = new Vector3(0, 0, 0);
			yaxis[upAxis] = 1.0f;
			var xaxis = new Vector3(0, 0, 0);
			xaxis[(upAxis + 1) % 3] = 1.0f;
			DebugDrawArc(start - rotation * (offsetHeight), rotation * yaxis, rotation * xaxis, offsetRadius.magnitude, offset2Radius.magnitude, 0, Two_PI, color, false, 10.0f);
		}

		public static void DebugDrawPlane(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 planeNormal, float planeConst, Color color)
		{
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

			var planeOrigin = planeNormal * planeConst;

			var vec0 = Vector3.zero;
			var vec1 = Vector3.zero;

			GetPlaneSpaceVector(planeNormal, ref vec0, ref vec1);

			float vecLen = 100.0f;
			var pt0 = planeOrigin + vec0 * vecLen;
			var pt1 = planeOrigin - vec0 * vecLen;
			var pt2 = planeOrigin + vec1 * vecLen;
			var pt3 = planeOrigin - vec1 * vecLen;

			Gizmos.DrawLine(pt0, pt1);
			Gizmos.DrawLine(pt2, pt3);
		}

		public static void GetPlaneSpaceVector(Vector3 planeNormal, ref Vector3 vec1, ref Vector3 vec2)
		{
			if (Mathf.Abs(planeNormal[2]) > SQRT12)
			{
				// choose p in y-z plane
				float a = planeNormal[1] * planeNormal[1] + planeNormal[2] * planeNormal[2];
				float k = 1.0f / Mathf.Sqrt(a);
				vec1[0] = 0;
				vec1[1] = -planeNormal[2] * k;
				vec1[2] = planeNormal[1] * k;
				// set q = n x p
				vec2[0] = a * k;
				vec2[1] = -planeNormal[0] * vec1[2];
				vec2[2] = planeNormal[0] * vec1[1];
			}
			else
			{
				// choose p in x-y plane
				float a = planeNormal[0] * planeNormal[0] + planeNormal[1] * planeNormal[1];
				float k = 1.0f / Mathf.Sqrt(a);
				vec1[0] = -planeNormal[1] * k;
				vec1[1] = planeNormal[0] * k;
				vec1[2] = 0;
				// set q = n x p
				vec2[0] = -planeNormal[2] * vec1[1];
				vec2[1] = planeNormal[2] * vec1[0];
				vec2[2] = a * k;
			}
		}

		public static void DebugDrawArc(Vector3 center, Vector3 normal, Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle,
					Color color, bool drawSect, float stepDegrees)
		{
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.identity;

			var vx = axis;
			var vy = Vector3.Cross(normal, axis);
			float step = stepDegrees * RADS_PER_DEG;
			int nSteps = (int)((maxAngle - minAngle) / step);

			if (nSteps == 0)
			{
				nSteps = 1;
			}

			var prev = center + radiusA * vx * Mathf.Cos(minAngle) + radiusB * vy * Mathf.Sin(minAngle);
			if (drawSect)
			{
				Gizmos.DrawLine(center, prev);
			}

			for (int i = 1; i <= nSteps; i++)
			{
				float angle = minAngle + (maxAngle - minAngle) * i * 1.0f / (nSteps * 1.0f);
				var next = center + radiusA * vx * Mathf.Cos(angle) + radiusB * vy * Mathf.Sin(angle);
				Gizmos.DrawLine(prev, next);
				prev = next;
			}

			if (drawSect)
			{
				Gizmos.DrawLine(center, prev);
			}
		}
	}
}
