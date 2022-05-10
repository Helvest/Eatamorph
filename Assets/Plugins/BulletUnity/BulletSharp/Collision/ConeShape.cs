using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletSharp
{
	public class ConeShape : ConvexInternalShape
	{
		internal ConeShape(IntPtr native)
			: base(native)
		{
		}

		public ConeShape(float radius, float height)
			: base(btConeShape_new(radius, height))
		{
		}

		public int ConeUpIndex
		{
			get => btConeShape_getConeUpIndex(_native);
			set => btConeShape_setConeUpIndex(_native, value);
		}

		public float Height => btConeShape_getHeight(_native);

		public float Radius => btConeShape_getRadius(_native);

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btConeShape_new(float radius, float height);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btConeShape_getConeUpIndex(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btConeShape_getHeight(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btConeShape_getRadius(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btConeShape_setConeUpIndex(IntPtr obj, int upIndex);
	}

	public class ConeShapeX : ConeShape
	{
		public ConeShapeX(float radius, float height)
			: base(btConeShapeX_new(radius, height))
		{
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btConeShapeX_new(float radius, float height);
	}

	public class ConeShapeZ : ConeShape
	{
		public ConeShapeZ(float radius, float height)
			: base(btConeShapeZ_new(radius, height))
		{
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btConeShapeZ_new(float radius, float height);
	}
}
