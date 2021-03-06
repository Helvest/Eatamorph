using System;
using System.Runtime.InteropServices;
using System.Security;
using BulletSharp.Math;

namespace BulletSharp
{
	public abstract class TriangleCallback : IDisposable
	{
		internal IntPtr _native;

		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		private delegate void ProcessTriangleDelegate(IntPtr triangle, int partId, int triangleIndex);

		private ProcessTriangleDelegate _processTriangle;

		public TriangleCallback()
		{
			_processTriangle = new ProcessTriangleDelegate(ProcessTriangleUnmanaged);

			_native = btTriangleCallbackWrapper_new(
				Marshal.GetFunctionPointerForDelegate(_processTriangle));
		}

		private void ProcessTriangleUnmanaged(IntPtr triangle, int partId, int triangleIndex)
		{
			float[] triangleData = new float[11];
			Marshal.Copy(triangle, triangleData, 0, 11);
			var p0 = new Vector3(triangleData[0], triangleData[1], triangleData[2]);
			var p1 = new Vector3(triangleData[4], triangleData[5], triangleData[6]);
			var p2 = new Vector3(triangleData[8], triangleData[9], triangleData[10]);
			ProcessTriangle(ref p0, ref p1, ref p2, partId, triangleIndex);
		}

		public abstract void ProcessTriangle(ref Vector3 point0, ref Vector3 point1, ref Vector3 point2, int partId, int triangleIndex);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_native != IntPtr.Zero)
			{
				btTriangleCallback_delete(_native);
				_native = IntPtr.Zero;
			}
		}

		~TriangleCallback()
		{
			Dispose(false);
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btTriangleCallbackWrapper_new(IntPtr internalProcessTriangleIndexCallback);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btTriangleCallback_delete(IntPtr obj);
	}

	public abstract class InternalTriangleIndexCallback : IDisposable
	{
		internal IntPtr _native;

		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		private delegate void InternalProcessTriangleIndexDelegate(IntPtr triangle, int partId, int triangleIndex);

		private InternalProcessTriangleIndexDelegate _internalProcessTriangleIndex;

		internal InternalTriangleIndexCallback()
		{
			_internalProcessTriangleIndex = new InternalProcessTriangleIndexDelegate(InternalProcessTriangleIndexUnmanaged);

			_native = btInternalTriangleIndexCallbackWrapper_new(
				Marshal.GetFunctionPointerForDelegate(_internalProcessTriangleIndex));
		}

		private void InternalProcessTriangleIndexUnmanaged(IntPtr triangle, int partId, int triangleIndex)
		{
			float[] triangleData = new float[11];
			Marshal.Copy(triangle, triangleData, 0, 11);
			var p0 = new Vector3(triangleData[0], triangleData[1], triangleData[2]);
			var p1 = new Vector3(triangleData[4], triangleData[5], triangleData[6]);
			var p2 = new Vector3(triangleData[8], triangleData[9], triangleData[10]);
			InternalProcessTriangleIndex(ref p0, ref p1, ref p2, partId, triangleIndex);
		}

		public abstract void InternalProcessTriangleIndex(ref Vector3 point0, ref Vector3 point1, ref Vector3 point2, int partId, int triangleIndex);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_native != IntPtr.Zero)
			{
				btInternalTriangleIndexCallback_delete(_native);
				_native = IntPtr.Zero;
			}
		}

		~InternalTriangleIndexCallback()
		{
			Dispose(false);
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btInternalTriangleIndexCallbackWrapper_new(IntPtr internalProcessTriangleIndexCallback);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btInternalTriangleIndexCallback_delete(IntPtr obj);
	}
}
