using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletSharp
{
	[Flags]
	public enum SolverModes
	{
		None = 0,
		RandomizeOrder = 1,
		FrictionSeparate = 2,
		UseWarmStarting = 4,
		Use2FrictionDirections = 16,
		EnableFrictionDirectionCaching = 32,
		DisableVelocityDependentFrictionDirection = 64,
		CacheFriendly = 128,
		Simd = 256,
		InterleaveContactAndFrictionConstraints = 512,
		AllowZeroLengthFrictionDirections = 1024
	}

	public class ContactSolverInfoData : IDisposable
	{
		internal IntPtr _native;
		private bool _preventDelete;

		internal ContactSolverInfoData(IntPtr native, bool preventDelete)
		{
			_native = native;
			_preventDelete = preventDelete;
		}

		public ContactSolverInfoData()
		{
			_native = btContactSolverInfoData_new();
		}

		public float Damping
		{
			get => btContactSolverInfoData_getDamping(_native);
			set => btContactSolverInfoData_setDamping(_native, value);
		}

		public float Erp
		{
			get => btContactSolverInfoData_getErp(_native);
			set => btContactSolverInfoData_setErp(_native, value);
		}

		public float Erp2
		{
			get => btContactSolverInfoData_getErp2(_native);
			set => btContactSolverInfoData_setErp2(_native, value);
		}

		public float Friction
		{
			get => btContactSolverInfoData_getFriction(_native);
			set => btContactSolverInfoData_setFriction(_native, value);
		}

		public float GlobalCfm
		{
			get => btContactSolverInfoData_getGlobalCfm(_native);
			set => btContactSolverInfoData_setGlobalCfm(_native, value);
		}

		public float LinearSlop
		{
			get => btContactSolverInfoData_getLinearSlop(_native);
			set => btContactSolverInfoData_setLinearSlop(_native, value);
		}

		public float MaxErrorReduction
		{
			get => btContactSolverInfoData_getMaxErrorReduction(_native);
			set => btContactSolverInfoData_setMaxErrorReduction(_native, value);
		}

		public float MaxGyroscopicForce
		{
			get => btContactSolverInfoData_getMaxGyroscopicForce(_native);
			set => btContactSolverInfoData_setMaxGyroscopicForce(_native, value);
		}

		public int MinimumSolverBatchSize
		{
			get => btContactSolverInfoData_getMinimumSolverBatchSize(_native);
			set => btContactSolverInfoData_setMinimumSolverBatchSize(_native, value);
		}

		public int NumIterations
		{
			get => btContactSolverInfoData_getNumIterations(_native);
			set => btContactSolverInfoData_setNumIterations(_native, value);
		}

		public int RestingContactRestitutionThreshold
		{
			get => btContactSolverInfoData_getRestingContactRestitutionThreshold(_native);
			set => btContactSolverInfoData_setRestingContactRestitutionThreshold(_native, value);
		}

		public float Restitution
		{
			get => btContactSolverInfoData_getRestitution(_native);
			set => btContactSolverInfoData_setRestitution(_native, value);
		}

		public float SingleAxisRollingFrictionThreshold
		{
			get => btContactSolverInfoData_getSingleAxisRollingFrictionThreshold(_native);
			set => btContactSolverInfoData_setSingleAxisRollingFrictionThreshold(_native, value);
		}

		public SolverModes SolverMode
		{
			get => btContactSolverInfoData_getSolverMode(_native);
			set => btContactSolverInfoData_setSolverMode(_native, value);
		}

		public float Sor
		{
			get => btContactSolverInfoData_getSor(_native);
			set => btContactSolverInfoData_setSor(_native, value);
		}

		public int SplitImpulse
		{
			get => btContactSolverInfoData_getSplitImpulse(_native);
			set => btContactSolverInfoData_setSplitImpulse(_native, value);
		}

		public float SplitImpulsePenetrationThreshold
		{
			get => btContactSolverInfoData_getSplitImpulsePenetrationThreshold(_native);
			set => btContactSolverInfoData_setSplitImpulsePenetrationThreshold(_native, value);
		}

		public float SplitImpulseTurnErp
		{
			get => btContactSolverInfoData_getSplitImpulseTurnErp(_native);
			set => btContactSolverInfoData_setSplitImpulseTurnErp(_native, value);
		}

		public float Tau
		{
			get => btContactSolverInfoData_getTau(_native);
			set => btContactSolverInfoData_setTau(_native, value);
		}

		public float TimeStep
		{
			get => btContactSolverInfoData_getTimeStep(_native);
			set => btContactSolverInfoData_setTimeStep(_native, value);
		}

		public float WarmStartingFactor
		{
			get => btContactSolverInfoData_getWarmstartingFactor(_native);
			set => btContactSolverInfoData_setWarmstartingFactor(_native, value);
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
					btContactSolverInfoData_delete(_native);
				}
				_native = IntPtr.Zero;
			}
		}

		~ContactSolverInfoData()
		{
			Dispose(false);
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btContactSolverInfoData_new();
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getDamping(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getErp(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getErp2(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getFriction(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getGlobalCfm(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getLinearSlop(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getMaxErrorReduction(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getMaxGyroscopicForce(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btContactSolverInfoData_getMinimumSolverBatchSize(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btContactSolverInfoData_getNumIterations(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btContactSolverInfoData_getRestingContactRestitutionThreshold(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getRestitution(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getSingleAxisRollingFrictionThreshold(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern SolverModes btContactSolverInfoData_getSolverMode(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getSor(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btContactSolverInfoData_getSplitImpulse(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getSplitImpulsePenetrationThreshold(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getSplitImpulseTurnErp(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getTau(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getTimeStep(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btContactSolverInfoData_getWarmstartingFactor(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setDamping(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setErp(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setErp2(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setFriction(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setGlobalCfm(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setLinearSlop(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setMaxErrorReduction(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setMaxGyroscopicForce(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setMinimumSolverBatchSize(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setNumIterations(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setRestingContactRestitutionThreshold(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setRestitution(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setSingleAxisRollingFrictionThreshold(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setSolverMode(IntPtr obj, SolverModes value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setSor(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setSplitImpulse(IntPtr obj, int value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setSplitImpulsePenetrationThreshold(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setSplitImpulseTurnErp(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setTau(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setTimeStep(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_setWarmstartingFactor(IntPtr obj, float value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btContactSolverInfoData_delete(IntPtr obj);
	}

	public class ContactSolverInfo : ContactSolverInfoData
	{
		internal ContactSolverInfo(IntPtr native, bool preventDelete)
			: base(native, preventDelete)
		{
		}

		public ContactSolverInfo()
			: base(btContactSolverInfo_new(), false)
		{
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btContactSolverInfo_new();
	}
}
