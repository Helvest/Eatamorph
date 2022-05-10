using System;
using System.Runtime.InteropServices;
using System.Security;
using BulletSharp.Math;

namespace BulletSharp
{
	[Flags]
	public enum SliderFlags
	{
		None = 0,
		CfmDirLinear = 1,
		ErpDirLinear = 2,
		CfmDirAngular = 4,
		ErpDirAngular = 8,
		CfmOrthoLinear = 16,
		ErpOrthoLinear = 32,
		CfmOrthoAngular = 64,
		ErpOrthoAngular = 128,
		CfmLimitLinear = 512,
		ErpLimitLinear = 1024,
		CfmLimitAngular = 2048,
		ErpLimitAngular = 4096
	}

	public class SliderConstraint : TypedConstraint
	{
		public SliderConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, Matrix frameInA, Matrix frameInB, bool useLinearReferenceFrameA)
			: base(btSliderConstraint_new(rigidBodyA._native, rigidBodyB._native, ref frameInA, ref frameInB, useLinearReferenceFrameA))
		{
			_rigidBodyA = rigidBodyA;
			_rigidBodyB = rigidBodyB;
		}

		public SliderConstraint(RigidBody rigidBodyB, Matrix frameInB, bool useLinearReferenceFrameA)
			: base(btSliderConstraint_new2(rigidBodyB._native, ref frameInB, useLinearReferenceFrameA))
		{
			_rigidBodyA = GetFixedBody();
			_rigidBodyB = rigidBodyB;
		}

		public void CalculateTransformsRef(ref Matrix transA, ref Matrix transB)
		{
			btSliderConstraint_calculateTransforms(_native, ref transA, ref transB);
		}

		public void CalculateTransforms(Matrix transA, Matrix transB)
		{
			btSliderConstraint_calculateTransforms(_native, ref transA, ref transB);
		}

		public void GetInfo1NonVirtual(ConstraintInfo1 info)
		{
			btSliderConstraint_getInfo1NonVirtual(_native, info._native);
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, Matrix transA, Matrix transB, Vector3 linVelA, Vector3 linVelB, float rbAinvMass, float rbBinvMass)
		{
			btSliderConstraint_getInfo2NonVirtual(_native, info._native, ref transA, ref transB, ref linVelA, ref linVelB, rbAinvMass, rbBinvMass);
		}

		public void SetFramesRef(ref Matrix frameA, ref Matrix frameB)
		{
			btSliderConstraint_setFrames(_native, ref frameA, ref frameB);
		}

		public void SetFrames(Matrix frameA, Matrix frameB)
		{
			btSliderConstraint_setFrames(_native, ref frameA, ref frameB);
		}

		public void TestAngularLimits()
		{
			btSliderConstraint_testAngLimits(_native);
		}

		public void TestLinearLimits()
		{
			btSliderConstraint_testLinLimits(_native);
		}

		public Vector3 AncorInA
		{
			get
			{
				btSliderConstraint_getAncorInA(_native, out var value);
				return value;
			}
		}

		public Vector3 AncorInB
		{
			get
			{
				btSliderConstraint_getAncorInB(_native, out var value);
				return value;
			}
		}

		public float AngularDepth => btSliderConstraint_getAngDepth(_native);

		public float AngularPosition => btSliderConstraint_getAngularPos(_native);

		public Matrix CalculatedTransformA
		{
			get
			{
				btSliderConstraint_getCalculatedTransformA(_native, out var value);
				return value;
			}
		}

		public Matrix CalculatedTransformB
		{
			get
			{
				btSliderConstraint_getCalculatedTransformB(_native, out var value);
				return value;
			}
		}

		public float DampingDirAngular
		{
			get => btSliderConstraint_getDampingDirAng(_native);
			set => btSliderConstraint_setDampingDirAng(_native, value);
		}

		public float DampingDirLinear
		{
			get => btSliderConstraint_getDampingDirLin(_native);
			set => btSliderConstraint_setDampingDirLin(_native, value);
		}

		public float DampingLimAngular
		{
			get => btSliderConstraint_getDampingLimAng(_native);
			set => btSliderConstraint_setDampingLimAng(_native, value);
		}

		public float DampingLimLinear
		{
			get => btSliderConstraint_getDampingLimLin(_native);
			set => btSliderConstraint_setDampingLimLin(_native, value);
		}

		public float DampingOrthoAngular
		{
			get => btSliderConstraint_getDampingOrthoAng(_native);
			set => btSliderConstraint_setDampingOrthoAng(_native, value);
		}

		public float DampingOrthoLinear
		{
			get => btSliderConstraint_getDampingOrthoLin(_native);
			set => btSliderConstraint_setDampingOrthoLin(_native, value);
		}

		public SliderFlags Flags => btSliderConstraint_getFlags(_native);

		public Matrix FrameOffsetA
		{
			get
			{
				btSliderConstraint_getFrameOffsetA(_native, out var value);
				return value;
			}
		}

		public Matrix FrameOffsetB
		{
			get
			{
				btSliderConstraint_getFrameOffsetB(_native, out var value);
				return value;
			}
		}

		public float LinearDepth => btSliderConstraint_getLinDepth(_native);

		public float LinearPos => btSliderConstraint_getLinearPos(_native);

		public float LowerAngularLimit
		{
			get => btSliderConstraint_getLowerAngLimit(_native);
			set => btSliderConstraint_setLowerAngLimit(_native, value);
		}

		public float LowerLinearLimit
		{
			get => btSliderConstraint_getLowerLinLimit(_native);
			set => btSliderConstraint_setLowerLinLimit(_native, value);
		}

		public float MaxAngMotorForce
		{
			get => btSliderConstraint_getMaxAngMotorForce(_native);
			set => btSliderConstraint_setMaxAngMotorForce(_native, value);
		}

		public float MaxLinearMotorForce
		{
			get => btSliderConstraint_getMaxLinMotorForce(_native);
			set => btSliderConstraint_setMaxLinMotorForce(_native, value);
		}

		public bool PoweredAngularMotor
		{
			get => btSliderConstraint_getPoweredAngMotor(_native);
			set => btSliderConstraint_setPoweredAngMotor(_native, value);
		}

		public bool PoweredLinearMotor
		{
			get => btSliderConstraint_getPoweredLinMotor(_native);
			set => btSliderConstraint_setPoweredLinMotor(_native, value);
		}

		public float RestitutionDirAngular
		{
			get => btSliderConstraint_getRestitutionDirAng(_native);
			set => btSliderConstraint_setRestitutionDirAng(_native, value);
		}

		public float RestitutionDirLinear
		{
			get => btSliderConstraint_getRestitutionDirLin(_native);
			set => btSliderConstraint_setRestitutionDirLin(_native, value);
		}

		public float RestitutionLimAngular
		{
			get => btSliderConstraint_getRestitutionLimAng(_native);
			set => btSliderConstraint_setRestitutionLimAng(_native, value);
		}

		public float RestitutionLimLinear
		{
			get => btSliderConstraint_getRestitutionLimLin(_native);
			set => btSliderConstraint_setRestitutionLimLin(_native, value);
		}

		public float RestitutionOrthoAngular
		{
			get => btSliderConstraint_getRestitutionOrthoAng(_native);
			set => btSliderConstraint_setRestitutionOrthoAng(_native, value);
		}

		public float RestitutionOrthoLinear
		{
			get => btSliderConstraint_getRestitutionOrthoLin(_native);
			set => btSliderConstraint_setRestitutionOrthoLin(_native, value);
		}

		public float SoftnessDirAngular
		{
			get => btSliderConstraint_getSoftnessDirAng(_native);
			set => btSliderConstraint_setSoftnessDirAng(_native, value);
		}

		public float SoftnessDirLinear
		{
			get => btSliderConstraint_getSoftnessDirLin(_native);
			set => btSliderConstraint_setSoftnessDirLin(_native, value);
		}

		public float SoftnessLimAngular
		{
			get => btSliderConstraint_getSoftnessLimAng(_native);
			set => btSliderConstraint_setSoftnessLimAng(_native, value);
		}

		public float SoftnessLimLinear
		{
			get => btSliderConstraint_getSoftnessLimLin(_native);
			set => btSliderConstraint_setSoftnessLimLin(_native, value);
		}

		public float SoftnessOrthoAngular
		{
			get => btSliderConstraint_getSoftnessOrthoAng(_native);
			set => btSliderConstraint_setSoftnessOrthoAng(_native, value);
		}

		public float SoftnessOrthoLinear
		{
			get => btSliderConstraint_getSoftnessOrthoLin(_native);
			set => btSliderConstraint_setSoftnessOrthoLin(_native, value);
		}

		public bool SolveAngularLimit => btSliderConstraint_getSolveAngLimit(_native);

		public bool SolveLinearLimit => btSliderConstraint_getSolveLinLimit(_native);

		public float TargetAngularMotorVelocity
		{
			get => btSliderConstraint_getTargetAngMotorVelocity(_native);
			set => btSliderConstraint_setTargetAngMotorVelocity(_native, value);
		}

		public float TargetLinearMotorVelocity
		{
			get => btSliderConstraint_getTargetLinMotorVelocity(_native);
			set => btSliderConstraint_setTargetLinMotorVelocity(_native, value);
		}

		public float UpperAngularLimit
		{
			get => btSliderConstraint_getUpperAngLimit(_native);
			set => btSliderConstraint_setUpperAngLimit(_native, value);
		}

		public float UpperLinearLimit
		{
			get => btSliderConstraint_getUpperLinLimit(_native);
			set => btSliderConstraint_setUpperLinLimit(_native, value);
		}

		public bool UseFrameOffset
		{
			get => btSliderConstraint_getUseFrameOffset(_native);
			set => btSliderConstraint_setUseFrameOffset(_native, value);
		}

		public bool UseLinearReferenceFrameA => btSliderConstraint_getUseLinearReferenceFrameA(_native);

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btSliderConstraint_new(IntPtr rbA, IntPtr rbB, [In] ref Matrix frameInA, [In] ref Matrix frameInB, bool useLinearReferenceFrameA);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btSliderConstraint_new2(IntPtr rbB, [In] ref Matrix frameInB, bool useLinearReferenceFrameA);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_calculateTransforms(IntPtr obj, [In] ref Matrix transA, [In] ref Matrix transB);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getAncorInA(IntPtr obj, [Out] out Vector3 value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getAncorInB(IntPtr obj, [Out] out Vector3 value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getAngDepth(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getAngularPos(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getCalculatedTransformA(IntPtr obj, [Out] out Matrix value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getCalculatedTransformB(IntPtr obj, [Out] out Matrix value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getDampingDirAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getDampingDirLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getDampingLimAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getDampingLimLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getDampingOrthoAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getDampingOrthoLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern SliderFlags btSliderConstraint_getFlags(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getFrameOffsetA(IntPtr obj, [Out] out Matrix value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getFrameOffsetB(IntPtr obj, [Out] out Matrix value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getInfo1NonVirtual(IntPtr obj, IntPtr info);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_getInfo2NonVirtual(IntPtr obj, IntPtr info, [In] ref Matrix transA, [In] ref Matrix transB, [In] ref Vector3 linVelA, [In] ref Vector3 linVelB, float rbAinvMass, float rbBinvMass);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getLinDepth(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getLinearPos(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getLowerAngLimit(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getLowerLinLimit(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getMaxAngMotorForce(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getMaxLinMotorForce(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btSliderConstraint_getPoweredAngMotor(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btSliderConstraint_getPoweredLinMotor(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getRestitutionDirAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getRestitutionDirLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getRestitutionLimAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getRestitutionLimLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getRestitutionOrthoAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getRestitutionOrthoLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getSoftnessDirAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getSoftnessDirLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getSoftnessLimAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getSoftnessLimLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getSoftnessOrthoAng(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getSoftnessOrthoLin(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btSliderConstraint_getSolveAngLimit(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btSliderConstraint_getSolveLinLimit(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getTargetAngMotorVelocity(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getTargetLinMotorVelocity(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getUpperAngLimit(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern float btSliderConstraint_getUpperLinLimit(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btSliderConstraint_getUseFrameOffset(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool btSliderConstraint_getUseLinearReferenceFrameA(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setDampingDirAng(IntPtr obj, float dampingDirAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setDampingDirLin(IntPtr obj, float dampingDirLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setDampingLimAng(IntPtr obj, float dampingLimAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setDampingLimLin(IntPtr obj, float dampingLimLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setDampingOrthoAng(IntPtr obj, float dampingOrthoAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setDampingOrthoLin(IntPtr obj, float dampingOrthoLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setFrames(IntPtr obj, [In] ref Matrix frameA, [In] ref Matrix frameB);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setLowerAngLimit(IntPtr obj, float lowerLimit);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setLowerLinLimit(IntPtr obj, float lowerLimit);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setMaxAngMotorForce(IntPtr obj, float maxAngMotorForce);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setMaxLinMotorForce(IntPtr obj, float maxLinMotorForce);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setPoweredAngMotor(IntPtr obj, bool onOff);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setPoweredLinMotor(IntPtr obj, bool onOff);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setRestitutionDirAng(IntPtr obj, float restitutionDirAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setRestitutionDirLin(IntPtr obj, float restitutionDirLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setRestitutionLimAng(IntPtr obj, float restitutionLimAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setRestitutionLimLin(IntPtr obj, float restitutionLimLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setRestitutionOrthoAng(IntPtr obj, float restitutionOrthoAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setRestitutionOrthoLin(IntPtr obj, float restitutionOrthoLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setSoftnessDirAng(IntPtr obj, float softnessDirAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setSoftnessDirLin(IntPtr obj, float softnessDirLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setSoftnessLimAng(IntPtr obj, float softnessLimAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setSoftnessLimLin(IntPtr obj, float softnessLimLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setSoftnessOrthoAng(IntPtr obj, float softnessOrthoAng);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setSoftnessOrthoLin(IntPtr obj, float softnessOrthoLin);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setTargetAngMotorVelocity(IntPtr obj, float targetAngMotorVelocity);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setTargetLinMotorVelocity(IntPtr obj, float targetLinMotorVelocity);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setUpperAngLimit(IntPtr obj, float upperLimit);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setUpperLinLimit(IntPtr obj, float upperLimit);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_setUseFrameOffset(IntPtr obj, bool frameOffsetOnOff);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_testAngLimits(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btSliderConstraint_testLinLimits(IntPtr obj);
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SliderConstraintFloatData
	{
		public TypedConstraintFloatData TypedConstraintData;
		public TransformFloatData RigidBodyAFrame;
		public TransformFloatData RigidBodyBFrame;
		public float LinearUpperLimit;
		public float LinearLowerLimit;
		public float AngularUpperLimit;
		public float AngularLowerLimit;
		public int UseLinearReferenceFrameA;
		public int UseOffsetForConstraintFrame;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(SliderConstraintFloatData), fieldName).ToInt32(); }
	}
}
