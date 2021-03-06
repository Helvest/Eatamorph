using System;
using System.Runtime.InteropServices;
using System.Security;
using BulletSharp.Math;

namespace BulletSharp
{
	public delegate void ContactDestroyedEventHandler(object userPersistantData);
	public delegate void ContactProcessedEventHandler(ManifoldPoint cp, CollisionObject body0, CollisionObject body1);

	public class PersistentManifold : IDisposable //: TypedObject
	{
		internal IntPtr _native;
		private bool _preventDelete;
		private static ContactDestroyedEventHandler _contactDestroyed;
		private static ContactProcessedEventHandler _contactProcessed;
		private static ContactDestroyedUnmanagedDelegate _contactDestroyedUnmanaged;
		private static ContactProcessedUnmanagedDelegate _contactProcessedUnmanaged;
		private static IntPtr _contactDestroyedUnmanagedPtr;
		private static IntPtr _contactProcessedUnmanagedPtr;

		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		private delegate bool ContactDestroyedUnmanagedDelegate(IntPtr userPersistantData);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		private delegate bool ContactProcessedUnmanagedDelegate(IntPtr cp, IntPtr body0, IntPtr body1);

		private static bool ContactDestroyedUnmanaged(IntPtr userPersistentData)
		{
			_contactDestroyed.Invoke(GCHandle.FromIntPtr(userPersistentData).Target);
			return false;
		}

		private static bool ContactProcessedUnmanaged(IntPtr cp, IntPtr body0, IntPtr body1)
		{
			_contactProcessed.Invoke(new ManifoldPoint(cp, true), CollisionObject.GetManaged(body0), CollisionObject.GetManaged(body1));
			return false;
		}

		public static event ContactDestroyedEventHandler ContactDestroyed
		{
			add
			{
				if (_contactDestroyedUnmanaged == null)
				{
					_contactDestroyedUnmanaged = new ContactDestroyedUnmanagedDelegate(ContactDestroyedUnmanaged);
					_contactDestroyedUnmanagedPtr = Marshal.GetFunctionPointerForDelegate(_contactDestroyedUnmanaged);
				}
				setGContactDestroyedCallback(_contactDestroyedUnmanagedPtr);
				_contactDestroyed += value;
			}
			remove
			{
				_contactDestroyed -= value;
				if (_contactDestroyed == null)
				{
					setGContactDestroyedCallback(IntPtr.Zero);
				}
			}
		}

		public static event ContactProcessedEventHandler ContactProcessed
		{
			add
			{
				if (_contactProcessedUnmanaged == null)
				{
					_contactProcessedUnmanaged = new ContactProcessedUnmanagedDelegate(ContactProcessedUnmanaged);
					_contactProcessedUnmanagedPtr = Marshal.GetFunctionPointerForDelegate(_contactProcessedUnmanaged);
				}
				setGContactProcessedCallback(_contactProcessedUnmanagedPtr);
				_contactProcessed += value;
			}
			remove
			{
				_contactProcessed -= value;
				if (_contactProcessed == null)
				{
					setGContactProcessedCallback(IntPtr.Zero);
				}
			}
		}

		internal PersistentManifold(IntPtr native, bool preventDelete)
		{
			_native = native;
			_preventDelete = preventDelete;
		}

		public PersistentManifold()
		{
			_native = btPersistentManifold_new();
		}

		public PersistentManifold(CollisionObject body0, CollisionObject body1, int __unnamed2, float contactBreakingThreshold, float contactProcessingThreshold)
		{
			_native = btPersistentManifold_new2(body0._native, body1._native, __unnamed2, contactBreakingThreshold, contactProcessingThreshold);
		}

		public int AddManifoldPoint(ManifoldPoint newPoint)
		{
			return btPersistentManifold_addManifoldPoint(_native, newPoint._native);
		}

		public int AddManifoldPoint(ManifoldPoint newPoint, bool isPredictive)
		{
			return btPersistentManifold_addManifoldPoint2(_native, newPoint._native, isPredictive);
		}

		public void ClearManifold()
		{
			btPersistentManifold_clearManifold(_native);
		}

		public void ClearUserCache(ManifoldPoint pt)
		{
			btPersistentManifold_clearUserCache(_native, pt._native);
		}

		public int GetCacheEntry(ManifoldPoint newPoint)
		{
			return btPersistentManifold_getCacheEntry(_native, newPoint._native);
		}

		public ManifoldPoint GetContactPoint(int index)
		{
			return new ManifoldPoint(btPersistentManifold_getContactPoint(_native, index), true);
		}

		public void RefreshContactPointsRef(ref Matrix trA, ref Matrix trB)
		{
			btPersistentManifold_refreshContactPoints(_native, ref trA, ref trB);
		}

		public void RefreshContactPoints(Matrix trA, Matrix trB)
		{
			btPersistentManifold_refreshContactPoints(_native, ref trA, ref trB);
		}

		public void RemoveContactPoint(int index)
		{
			btPersistentManifold_removeContactPoint(_native, index);
		}

		public void ReplaceContactPoint(ManifoldPoint newPoint, int insertIndex)
		{
			btPersistentManifold_replaceContactPoint(_native, newPoint._native, insertIndex);
		}

		public void SetBodies(CollisionObject body0, CollisionObject body1)
		{
			btPersistentManifold_setBodies(_native, body0._native, body1._native);
		}

		public bool ValidContactDistance(ManifoldPoint pt)
		{
			return btPersistentManifold_validContactDistance(_native, pt._native);
		}

		public CollisionObject Body0 => CollisionObject.GetManaged(btPersistentManifold_getBody0(_native));

		public CollisionObject Body1 => CollisionObject.GetManaged(btPersistentManifold_getBody1(_native));

		public int CompanionIdA
		{
			get => btPersistentManifold_getCompanionIdA(_native);
			set => btPersistentManifold_setCompanionIdA(_native, value);
		}

		public int CompanionIdB
		{
			get => btPersistentManifold_getCompanionIdB(_native);
			set => btPersistentManifold_setCompanionIdB(_native, value);
		}

		public float ContactBreakingThreshold
		{
			get => btPersistentManifold_getContactBreakingThreshold(_native);
			set => btPersistentManifold_setContactBreakingThreshold(_native, value);
		}

		public float ContactProcessingThreshold
		{
			get => btPersistentManifold_getContactProcessingThreshold(_native);
			set => btPersistentManifold_setContactProcessingThreshold(_native, value);
		}

		public int Index1A
		{
			get => btPersistentManifold_getIndex1a(_native);
			set => btPersistentManifold_setIndex1a(_native, value);
		}

		public int NumContacts
		{
			get => btPersistentManifold_getNumContacts(_native);
			set => btPersistentManifold_setNumContacts(_native, value);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_native != IntPtr.Zero)
			{
				if (!_preventDelete)
				{
					btPersistentManifold_delete(_native);
				}
				_native = IntPtr.Zero;
			}
		}

		~PersistentManifold()
		{
			Dispose(false);
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btPersistentManifold_new();
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btPersistentManifold_new2(IntPtr body0, IntPtr body1, int __unnamed2, float contactBreakingThreshold, float contactProcessingThreshold);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_addManifoldPoint(IntPtr obj, IntPtr newPoint);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_addManifoldPoint2(IntPtr obj, IntPtr newPoint, bool isPredictive);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_clearManifold(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_clearUserCache(IntPtr obj, IntPtr pt);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btPersistentManifold_getBody0(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btPersistentManifold_getBody1(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_getCacheEntry(IntPtr obj, IntPtr newPoint);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_getCompanionIdA(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_getCompanionIdB(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btPersistentManifold_getContactBreakingThreshold(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btPersistentManifold_getContactPoint(IntPtr obj, int index);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btPersistentManifold_getContactProcessingThreshold(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_getIndex1a(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btPersistentManifold_getNumContacts(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_refreshContactPoints(IntPtr obj, [In] ref Matrix trA, [In] ref Matrix trB);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_removeContactPoint(IntPtr obj, int index);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_replaceContactPoint(IntPtr obj, IntPtr newPoint, int insertIndex);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setBodies(IntPtr obj, IntPtr body0, IntPtr body1);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setCompanionIdA(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setCompanionIdB(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setContactBreakingThreshold(IntPtr obj, float contactBreakingThreshold);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setContactProcessingThreshold(IntPtr obj, float contactProcessingThreshold);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setIndex1a(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_setNumContacts(IntPtr obj, int cachedPoints);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btPersistentManifold_validContactDistance(IntPtr obj, IntPtr pt);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btPersistentManifold_delete(IntPtr obj);
		//[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		//static extern IntPtr getGContactDestroyedCallback();
		//[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		//static extern IntPtr getGContactProcessedCallback();
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void setGContactDestroyedCallback(IntPtr value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void setGContactProcessedCallback(IntPtr value);
	}
}
