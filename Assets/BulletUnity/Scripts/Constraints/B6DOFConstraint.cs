using BulletSharp;
using UnityEngine;
using BM = BulletSharp.Math;

namespace BulletUnity
{
	[AddComponentMenu("Physics Bullet/Constraints/6 Degree Of Freedom")]
	public class B6DOFConstraint : BTypedConstraint
	{
		//Todo not sure if this is working
		//todo should be properties so can capture changes and propagate to scene
		public static string HelpMessage = "\n" +
											"\nTIP: To see constraint limits:\n" +
											"  - In BulletPhysicsWorld turn on 'Do Debug Draw' and set 'Debug Draw Mode' flags\n" +
											"  - On Constraint set 'Debug Draw Size'\n" +
											"  - Press play";

		[Header("Limits")]
		[SerializeField]
		protected Vector3 m_linearLimitLower;
		public Vector3 linearLimitLower
		{
			get => m_linearLimitLower;
			set
			{
				m_linearLimitLower = value;
				if (m_constraintPtr != null)
				{
					((Generic6DofConstraint)m_constraintPtr).LinearLowerLimit = m_linearLimitLower.ToBullet();
				}
			}
		}

		[SerializeField]
		protected Vector3 m_linearLimitUpper;
		public Vector3 linearLimitUpper
		{
			get => m_linearLimitUpper;
			set
			{
				m_linearLimitUpper = value;
				if (m_constraintPtr != null)
				{
					((Generic6DofConstraint)m_constraintPtr).LinearUpperLimit = m_linearLimitUpper.ToBullet();
				}
			}
		}

		[SerializeField]
		protected Vector3 m_angularLimitLowerRadians;
		public Vector3 angularLimitLowerRadians
		{
			get => m_angularLimitLowerRadians;
			set
			{
				m_angularLimitLowerRadians = value;
				if (m_constraintPtr != null)
				{
					((Generic6DofConstraint)m_constraintPtr).AngularLowerLimit = m_angularLimitLowerRadians.ToBullet();
				}
			}
		}

		[SerializeField]
		protected Vector3 m_angularLimitUpperRadians;
		public Vector3 angularLimitUpperRadians
		{
			get => m_angularLimitUpperRadians;
			set
			{
				m_angularLimitUpperRadians = value;
				if (m_constraintPtr != null)
				{
					((Generic6DofConstraint)m_constraintPtr).AngularUpperLimit = m_angularLimitUpperRadians.ToBullet();
				}
			}
		}

		[Header("Motor")]
		[SerializeField]
		protected Vector3 m_motorLinearTargetVelocity;
		public Vector3 motorLinearTargetVelocity
		{
			get => m_motorLinearTargetVelocity;
			set
			{
				m_motorLinearTargetVelocity = value;
				if (m_constraintPtr != null)
				{
					((Generic6DofConstraint)m_constraintPtr).TranslationalLimitMotor.TargetVelocity = m_motorLinearTargetVelocity.ToBullet();
				}
			}
		}

		[SerializeField]
		protected Vector3 m_motorLinearMaxMotorForce;
		public Vector3 motorLinearMaxMotorForce
		{
			get => m_motorLinearMaxMotorForce;
			set
			{
				m_motorLinearMaxMotorForce = value;
				if (m_constraintPtr != null)
				{
					((Generic6DofConstraint)m_constraintPtr).TranslationalLimitMotor.MaxMotorForce = m_motorLinearMaxMotorForce.ToBullet();
				}
			}
		}

		//called by Physics World just before constraint is added to world.
		//the current constraint properties are used to rebuild the constraint.
		internal override bool _BuildConstraint()
		{
			var world = GetWorld();
			if (m_constraintPtr != null)
			{
				if (m_isInWorld && world != null)
				{
					m_isInWorld = false;
					world.RemoveConstraint(m_constraintPtr);
				}
			}

			var targetRigidBodyA = GetComponent<BRigidBody>();
			if (targetRigidBodyA == null)
			{
				Debug.LogError("B6DOFConstraint needs to be added to a component with a BRigidBody.");
				return false;
			}

			if (!targetRigidBodyA.isInWorld)
			{
				world.AddRigidBody(targetRigidBodyA);
			}

			var rba = (RigidBody)targetRigidBodyA.GetCollisionObject();
			if (rba == null)
			{
				Debug.LogError("Constraint could not get bullet RigidBody from target rigid body");
				return false;
			}

			if (m_constraintType == ConstraintType.constrainToAnotherBody)
			{
				if (m_otherRigidBody == null)
				{
					Debug.LogError("Other Rigid Body is not set.");
					return false;
				}

				if (!m_otherRigidBody.isInWorld)
				{
					world.AddRigidBody(m_otherRigidBody);
					return false;
				}

				var rbb = (RigidBody)m_otherRigidBody.GetCollisionObject();
				if (rbb == null)
				{
					Debug.LogError("Constraint could not get bullet RigidBody from target rigid body");
					return false;
				}

				string errormsg = string.Empty;
				if (CreateFramesA_B(m_localConstraintAxisX, m_localConstraintAxisY, m_localConstraintPoint, out var frameInA, out var frameInOther, ref errormsg))
				{
					m_constraintPtr = new Generic6DofConstraint(rbb, rba, frameInOther, frameInA, true);
				}
				else
				{
					Debug.LogError(errormsg);
					return false;
				}
			}
			else
			{
				//TODO think about this
				//BM.Matrix frameInA = BM.Matrix.Identity;
				m_constraintPtr = new Generic6DofConstraint(rba, BM.Matrix.Identity, false);
			}

			m_constraintPtr.Userobject = this;
			var sl = (Generic6DofConstraint)m_constraintPtr;
			sl.LinearLowerLimit = m_linearLimitLower.ToBullet();
			sl.LinearUpperLimit = m_linearLimitUpper.ToBullet();
			sl.AngularLowerLimit = m_angularLimitLowerRadians.ToBullet();
			sl.AngularUpperLimit = m_angularLimitUpperRadians.ToBullet();
			sl.TranslationalLimitMotor.TargetVelocity = m_motorLinearTargetVelocity.ToBullet();
			sl.TranslationalLimitMotor.MaxMotorForce = m_motorLinearMaxMotorForce.ToBullet();
			sl.BreakingImpulseThreshold = m_breakingImpulseThreshold;
			sl.DebugDrawSize = m_debugDrawSize;
			sl.OverrideNumSolverIterations = m_overrideNumSolverIterations;

			return true;
		}
	}
}
