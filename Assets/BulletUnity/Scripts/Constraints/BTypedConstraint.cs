using System;
using BulletSharp;
using UnityEngine;
using BM = BulletSharp.Math;

namespace BulletUnity
{
	[Serializable]
	public abstract class BTypedConstraint : MonoBehaviour, IDisposable
	{
		public int WorldID { get; protected set; }

		public WorldController GetWorld()
		{
			return WorldsManager.GetWorldController(WorldID);
		}

		protected bool m_startWasCalled = false;

		public enum ConstraintType
		{
			constrainToPointInSpace,
			constrainToAnotherBody
		}

		[Header("Reference Frame Local To This Object")]
		[SerializeField]
		protected Vector3 m_localConstraintPoint = Vector3.zero;
		public Vector3 localConstraintPoint
		{
			get => m_localConstraintPoint;
			set
			{
				if (m_constraintPtr != null && m_localConstraintPoint != value)
				{
					Debug.LogError("Cannot change the local constraint point once the constraint has been created");
				}
				else
				{
					m_localConstraintPoint = value;
				}
			}
		}

		[SerializeField]
		protected Vector3 m_localConstraintAxisX = Vector3.forward;
		public Vector3 localConstraintAxisX
		{
			get => m_localConstraintAxisX;
			set
			{
				if (m_constraintPtr != null && m_localConstraintAxisX != value)
				{
					Debug.LogError("Cannot change the local constraint axis once the constraint has been created");
				}
				else
				{
					m_localConstraintAxisX = value;
				}
			}
		}

		[SerializeField]
		protected Vector3 m_localConstraintAxisY = Vector3.up;
		public Vector3 localConstraintAxisY
		{
			get => m_localConstraintAxisY;
			set
			{
				if (m_constraintPtr != null && m_localConstraintAxisY != value)
				{
					Debug.LogError("Cannot change the local constraint axis once the constraint has been created");
				}
				else
				{
					m_localConstraintAxisY = value;
				}
			}
		}

		[SerializeField]
		protected float m_breakingImpulseThreshold = Mathf.Infinity;
		public float breakingImpulseThreshold
		{
			get => m_breakingImpulseThreshold;
			set
			{
				if (m_constraintPtr != null)
				{
					m_constraintPtr.BreakingImpulseThreshold = value;
				}

				m_breakingImpulseThreshold = value;
			}
		}

		[SerializeField]
		protected bool m_disableCollisionsBetweenConstrainedBodies = true;
		public bool disableCollisionsBetweenConstrainedBodies
		{
			get => m_disableCollisionsBetweenConstrainedBodies;
			set
			{
				if (m_constraintPtr != null && m_disableCollisionsBetweenConstrainedBodies != value)
				{
					Debug.LogError("Cannot only set disableCollisionsBetweenConstrainedBodies before the constraint has been created");
				}
				else
				{
					m_disableCollisionsBetweenConstrainedBodies = value;
				}
			}
		}

		[SerializeField]
		protected ConstraintType m_constraintType;
		public ConstraintType constraintType
		{
			get => m_constraintType;
			set
			{
				if (m_constraintPtr != null && m_constraintType != value)
				{
					Debug.LogError("Cannot set the constraint type once the constraint has been created");
				}
				else
				{
					m_constraintType = value;
				}
			}
		}

		[HideInInspector]
		[SerializeField]
		protected BRigidBody m_thisRigidBody;
		public BRigidBody thisRigidBody
		{
			get => m_thisRigidBody;
			set
			{
				if (m_constraintPtr != null && m_thisRigidBody != value)
				{
					Debug.LogError("Cannot change the rigidbody once once the constraint has been created");
				}
				else
				{
					m_thisRigidBody = value;
				}
			}
		}

		[SerializeField]
		protected BRigidBody m_otherRigidBody;
		public BRigidBody otherRigidBody
		{
			get => m_otherRigidBody;
			set
			{
				if (m_constraintPtr != null && m_otherRigidBody != value)
				{
					Debug.LogError("Cannot change the rigidbody once once the constraint has been created");
				}
				else
				{
					m_otherRigidBody = value;
				}
			}
		}

		[SerializeField]
		protected float m_debugDrawSize;
		public float debugDrawSize
		{
			get => m_debugDrawSize;
			set
			{
				if (m_constraintPtr != null)
				{
					m_constraintPtr.DebugDrawSize = value;
				}
				else
				{
					m_debugDrawSize = value;
				}
			}
		}

		[SerializeField]
		protected int m_overrideNumSolverIterations = 20;
		public int overrideNumSolverIterations
		{
			get => m_overrideNumSolverIterations;
			set
			{
				if (value < 1)
				{
					value = 1;
				}

				if (m_constraintPtr != null)
				{
					m_constraintPtr.OverrideNumSolverIterations = value;
				}
				else
				{
					m_overrideNumSolverIterations = value;
				}
			}
		}

		internal TypedConstraint m_constraintPtr = null;
		internal bool m_isInWorld = false;

		public void DrawTransformGizmos(Transform t, Vector3 pivotPoint, Vector3 forward, Vector3 up)
		{
			var pivotWorld = t.TransformPoint(pivotPoint);
			var xWorld = t.TransformDirection(forward).normalized;
			var yWorld = t.TransformDirection(up).normalized;
			var zWorld = Vector3.Cross(xWorld, yWorld);
			yWorld = Vector3.Cross(zWorld, xWorld);
			yWorld.Normalize();
			xWorld.Normalize();
			Gizmos.color = Color.red;
			Gizmos.DrawLine(pivotWorld, pivotWorld + xWorld);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(pivotWorld, pivotWorld + yWorld);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(pivotWorld, pivotWorld + zWorld);
		}

		public bool CreateFrame(Vector3 forward, Vector3 up, Vector3 constraintPoint, ref BM.Matrix m, ref string errorMsg)
		{
			BM.Vector4 x;
			BM.Vector4 y;
			BM.Vector4 z;
			if (forward == Vector3.zero)
			{
				errorMsg = "forward vector must not be zero";
				return false;
			}

			forward.Normalize();
			if (up == Vector3.zero)
			{
				errorMsg = "up vector must not be zero";
				return false;
			}

			var right = Vector3.Cross(forward, up);
			if (right == Vector3.zero)
			{
				errorMsg = "forward and up vector must not be colinear";
				return false;
			}

			up = Vector3.Cross(right, forward);
			right.Normalize();
			up.Normalize();
			x.X = forward.x;
			x.Y = forward.y;
			x.Z = forward.z;
			x.W = 0f;
			y.X = up.x;
			y.Y = up.y;
			y.Z = up.z;
			y.W = 0f;
			z.X = right.x;
			z.Y = right.y;
			z.Z = right.z;
			z.W = 0f;
			m.Row1 = x;
			m.Row2 = y;
			m.Row3 = z;
			m.Origin = constraintPoint.ToBullet();

			return true;
		}

		// Object A has the constraint compontent and is constrainted to object B
		// The constraint frame is defined relative to A by three vectors.
		// This method calculates the frames A and B that need to be passed to bullet
		public bool CreateFramesA_B(Vector3 forwardInA, Vector3 upInA, Vector3 constraintPivotInA, out BM.Matrix frameInA, out BM.Matrix frameInB, ref string errorMsg)
		{
			frameInA = BM.Matrix.Identity;
			if (!CreateFrame(forwardInA, upInA, constraintPivotInA, ref frameInA, ref errorMsg))
			{
				frameInB = BM.Matrix.Identity;
				return false;
			}

			var x = frameInA.Row1;
			var y = frameInA.Row2;
			var z = frameInA.Row3;
			var xx = new Vector3(x.X, x.Y, x.Z);
			var yy = new Vector3(y.X, y.Y, y.Z);
			var zz = new Vector3(z.X, z.Y, z.Z);
			var q = transform.localRotation * Quaternion.Inverse(m_otherRigidBody.transform.localRotation);
			xx = q * xx;
			yy = q * yy;
			zz = q * zz;
			frameInB = BM.Matrix.Identity;
			frameInB.Row1 = new BM.Vector4(xx.ToBullet(), 0f);
			frameInB.Row2 = new BM.Vector4(yy.ToBullet(), 0f);
			frameInB.Row3 = new BM.Vector4(zz.ToBullet(), 0f);
			frameInB.Origin = m_otherRigidBody.transform.InverseTransformPoint(transform.TransformPoint(constraintPivotInA)).ToBullet();

			return true;
		}

		public virtual void OnDrawGizmosSelected()
		{
			DrawTransformGizmos(transform, m_localConstraintPoint, m_localConstraintAxisX, m_localConstraintAxisY);
		}

		protected virtual void AddToBulletWorld()
		{
			if (!m_isInWorld)
			{
				Debug.Assert(m_thisRigidBody.isInWorld, "Constrained bodies must be added to world before constraints " + this);

				if (m_constraintType == ConstraintType.constrainToAnotherBody)
				{
					Debug.Assert(m_otherRigidBody.isInWorld, "Constrained bodies must be added to world before constraints " + this);
				}

				GetWorld().AddConstraint(this);
			}
		}

		protected virtual void RemoveFromBulletWorld()
		{
			if (m_isInWorld)
			{
				GetWorld().RemoveConstraint(m_constraintPtr);
			}
		}

		protected virtual void Awake()
		{
			WorldID = WorldsManager.ActualWorldID;
		}

		protected virtual void Start()
		{
			if (m_startWasCalled == false)
			{
				m_startWasCalled = true;

				//deal with nasty script order of execution bug. Order of Start call is random so force rigid bodies to be in the world before constraint is added
				if (!m_thisRigidBody.isInWorld)
				{
					m_thisRigidBody.Start();
				}

				if (m_constraintType == ConstraintType.constrainToAnotherBody && !m_otherRigidBody.isInWorld)
				{
					m_otherRigidBody.Start();
				}

				AddToBulletWorld();
			}
		}

		private void OnDestroy()
		{
			Dispose(false);
		}

		public void OnEnable()
		{
			m_thisRigidBody = GetComponent<BRigidBody>();
			if (m_thisRigidBody == null)
			{
				Debug.LogError("Constraint must be added to a game object with a BRigidBody.");
			}

			if (m_startWasCalled)
			{
				AddToBulletWorld();
			}
		}

		public void OnDisable()
		{
			RemoveFromBulletWorld();
		}

		//do not override this
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		//override this one
		protected virtual void Dispose(bool isdisposing)
		{
			if (m_constraintPtr != null)
			{
				m_constraintPtr.Dispose();
				m_constraintPtr = null;
			}
		}

		//called by Physics World just before constraint is added to world.
		//the current constraint properties are used to rebuild the constraint.
		internal abstract bool _BuildConstraint();

		public virtual TypedConstraint GetConstraint()
		{
			if (m_constraintPtr == null)
			{
				_BuildConstraint();
			}

			return m_constraintPtr;
		}
	}
}
