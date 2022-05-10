using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletSharp
{
	public class UniformScalingShape : ConvexShape
	{
		private ConvexShape _convexChildShape;

		public UniformScalingShape(ConvexShape convexChildShape, float uniformScalingFactor)
			: base(btUniformScalingShape_new(convexChildShape._native, uniformScalingFactor))
		{
			_convexChildShape = convexChildShape;
		}

		public ConvexShape ChildShape => _convexChildShape;

		public float UniformScalingFactor => btUniformScalingShape_getUniformScalingFactor(_native);

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btUniformScalingShape_new(IntPtr convexChildShape, float uniformScalingFactor);
		//[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		//static extern IntPtr btUniformScalingShape_getChildShape(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btUniformScalingShape_getUniformScalingFactor(IntPtr obj);
	}
}
