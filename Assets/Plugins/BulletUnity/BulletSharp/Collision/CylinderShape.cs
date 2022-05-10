using System;
using System.Runtime.InteropServices;
using System.Security;
using BulletSharp.Math;

namespace BulletSharp
{
	public class CylinderShape : ConvexInternalShape
	{
		internal CylinderShape(IntPtr native)
			: base(native)
		{
		}

		public CylinderShape(Vector3 halfExtents)
			: base(btCylinderShape_new(ref halfExtents))
		{
		}

		public CylinderShape(float halfExtentX, float halfExtentY, float halfExtentZ)
			: base(btCylinderShape_new2(halfExtentX, halfExtentY, halfExtentZ))
		{
		}

		public Vector3 HalfExtentsWithMargin
		{
			get
			{
				btCylinderShape_getHalfExtentsWithMargin(_native, out var value);
				return value;
			}
		}

		public Vector3 HalfExtentsWithoutMargin
		{
			get
			{
				btCylinderShape_getHalfExtentsWithoutMargin(_native, out var value);
				return value;
			}
		}

		public float Radius => btCylinderShape_getRadius(_native);

		public int UpAxis => btCylinderShape_getUpAxis(_native);

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCylinderShape_new([In] ref Vector3 halfExtents);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCylinderShape_new2(float halfExtentX, float halfExtentY, float halfExtentZ);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCylinderShape_getHalfExtentsWithMargin(IntPtr obj, [Out] out Vector3 value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCylinderShape_getHalfExtentsWithoutMargin(IntPtr obj, [Out] out Vector3 value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btCylinderShape_getRadius(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btCylinderShape_getUpAxis(IntPtr obj);
	}

	public class CylinderShapeX : CylinderShape
	{
		public CylinderShapeX(Vector3 halfExtents)
			: base(btCylinderShapeX_new(ref halfExtents))
		{
		}

		public CylinderShapeX(float halfExtentX, float halfExtentY, float halfExtentZ)
			: base(btCylinderShapeX_new2(halfExtentX, halfExtentY, halfExtentZ))
		{
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCylinderShapeX_new([In] ref Vector3 halfExtents);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCylinderShapeX_new2(float halfExtentX, float halfExtentY, float halfExtentZ);
	}

	public class CylinderShapeZ : CylinderShape
	{
		public CylinderShapeZ(Vector3 halfExtents)
			: base(btCylinderShapeZ_new(ref halfExtents))
		{
		}

		public CylinderShapeZ(float halfExtentX, float halfExtentY, float halfExtentZ)
			: base(btCylinderShapeZ_new2(halfExtentX, halfExtentY, halfExtentZ))
		{
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCylinderShapeZ_new([In] ref Vector3 halfExtents);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCylinderShapeZ_new2(float halfExtentX, float halfExtentY, float halfExtentZ);
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CylinderShapeFloatData
	{
		public ConvexInternalShapeFloatData ConvexInternalShapeData;
		public int UpAxis;
		public int Padding;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(CylinderShapeFloatData), fieldName).ToInt32(); }
	}
}
