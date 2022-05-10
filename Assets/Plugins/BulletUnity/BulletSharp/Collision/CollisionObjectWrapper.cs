using System;
using System.Runtime.InteropServices;
using System.Security;
using BulletSharp.Math;

namespace BulletSharp
{
	public class CollisionObjectWrapper
	{
		internal IntPtr _native;

		internal CollisionObjectWrapper(IntPtr native)
		{
			_native = native;
		}

		public CollisionObject CollisionObject
		{
			get => CollisionObject.GetManaged(btCollisionObjectWrapper_getCollisionObject(_native));
			set => btCollisionObjectWrapper_setCollisionObject(_native, value._native);
		}

		public CollisionShape CollisionShape
		{
			get => CollisionShape.GetManaged(btCollisionObjectWrapper_getCollisionShape(_native));
			set => btCollisionObjectWrapper_setShape(_native, value._native);
		}

		public int Index
		{
			get => btCollisionObjectWrapper_getIndex(_native);
			set => btCollisionObjectWrapper_setIndex(_native, value);
		}

		public CollisionObjectWrapper Parent
		{
			get => new CollisionObjectWrapper(btCollisionObjectWrapper_getParent(_native));
			set => btCollisionObjectWrapper_setParent(_native, value._native);
		}

		public int PartId
		{
			get => btCollisionObjectWrapper_getPartId(_native);
			set => btCollisionObjectWrapper_setPartId(_native, value);
		}

		public Matrix WorldTransform
		{
			get
			{
				btCollisionObjectWrapper_getWorldTransform(_native, out var value);
				return value;
			}
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCollisionObjectWrapper_getCollisionObject(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCollisionObjectWrapper_getCollisionShape(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btCollisionObjectWrapper_getIndex(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCollisionObjectWrapper_getParent(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btCollisionObjectWrapper_getPartId(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCollisionObjectWrapper_getWorldTransform(IntPtr obj, [Out] out Matrix value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCollisionObjectWrapper_setCollisionObject(IntPtr obj, IntPtr value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCollisionObjectWrapper_setIndex(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCollisionObjectWrapper_setParent(IntPtr obj, IntPtr value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCollisionObjectWrapper_setPartId(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCollisionObjectWrapper_setShape(IntPtr obj, IntPtr value);
	}
}
