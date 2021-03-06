#define ROLLING_INFLUENCE_FIX

using System;
using System.Collections.Generic;
using System.Diagnostics;
using BulletSharp;
using BulletSharp.Math;

namespace VehicleDemo
{
	// This class is equivalent to RaycastVehicle, but is used to test the IAction interface
	public class CustomVehicle : IAction
	{
		private List<WheelInfo> wheelInfo = new List<WheelInfo>();
		private Vector3[] forwardWS = new Vector3[0];
		private Vector3[] axle = new Vector3[0];
		private float[] forwardImpulse = new float[0];
		private float[] sideImpulse = new float[0];

		public Matrix ChassisWorldTransform =>
				/*if (RigidBody.MotionState != null)
{
return RigidBody.MotionState.WorldTransform;
}*/
				RigidBody.CenterOfMassTransform;

		private float currentVehicleSpeedKmHour;

		public int NumWheels => wheelInfo.Count;

		private int indexRightAxis = 0;
		public int RightAxis => indexRightAxis;

		private int indexUpAxis = 2;
		private int indexForwardAxis = 1;
		private RigidBody chassisBody;
		public RigidBody RigidBody => chassisBody;

		private IVehicleRaycaster vehicleRaycaster;
		private static RigidBody fixedBody;

		public void SetBrake(float brake, int wheelIndex)
		{
			Debug.Assert((wheelIndex >= 0) && (wheelIndex < NumWheels));
			GetWheelInfo(wheelIndex).Brake = brake;
		}

		public float GetSteeringValue(int wheel)
		{
			return GetWheelInfo(wheel).Steering;
		}

		public void SetSteeringValue(float steering, int wheel)
		{
			Debug.Assert(wheel >= 0 && wheel < NumWheels);

			var wheelInfo = GetWheelInfo(wheel);
			wheelInfo.Steering = steering;
		}

		public void SetCoordinateSystem(int rightIndex, int upIndex, int forwardIndex)
		{
			indexRightAxis = rightIndex;
			indexUpAxis = upIndex;
			indexForwardAxis = forwardIndex;
		}

		public Matrix GetWheelTransformWS(int wheelIndex)
		{
			Debug.Assert(wheelIndex < NumWheels);
			return wheelInfo[wheelIndex].WorldTransform;
		}

		static CustomVehicle()
		{
			using (var ci = new RigidBodyConstructionInfo(0, null, null))
			{
				fixedBody = new RigidBody(ci);
				fixedBody.SetMassProps(0, Vector3.Zero);
			}
		}

		public CustomVehicle(VehicleTuning tuning, RigidBody chassis, IVehicleRaycaster raycaster)
		{
			chassisBody = chassis;
			vehicleRaycaster = raycaster;
		}

		public WheelInfo AddWheel(Vector3 connectionPointCS, Vector3 wheelDirectionCS0, Vector3 wheelAxleCS, float suspensionRestLength, float wheelRadius, VehicleTuning tuning, bool isFrontWheel)
		{
			var ci = new WheelInfoConstructionInfo
			{
				ChassisConnectionCS = connectionPointCS,
				WheelDirectionCS = wheelDirectionCS0,
				WheelAxleCS = wheelAxleCS,
				SuspensionRestLength = suspensionRestLength,
				WheelRadius = wheelRadius,
				SuspensionStiffness = tuning.SuspensionStiffness,
				WheelsDampingCompression = tuning.SuspensionCompression,
				WheelsDampingRelaxation = tuning.SuspensionDamping,
				FrictionSlip = tuning.FrictionSlip,
				IsFrontWheel = isFrontWheel,
				MaxSuspensionTravelCm = tuning.MaxSuspensionTravelCm,
				MaxSuspensionForce = tuning.MaxSuspensionForce
			};

			var wheel = new WheelInfo(ci);
			wheelInfo.Add(wheel);

			UpdateWheelTransformsWS(wheel, false);
			UpdateWheelTransform(NumWheels - 1, false);
			return wheel;
		}

		public void ApplyEngineForce(float force, int wheel)
		{
			Debug.Assert(wheel >= 0 && wheel < NumWheels);
			var wheelInfo = GetWheelInfo(wheel);
			wheelInfo.EngineForce = force;
		}

		private float CalcRollingFriction(RigidBody body0, RigidBody body1, Vector3 contactPosWorld, Vector3 frictionDirectionWorld, float maxImpulse)
		{
			float denom0 = body0.ComputeImpulseDenominator(contactPosWorld, frictionDirectionWorld);
			float denom1 = body1.ComputeImpulseDenominator(contactPosWorld, frictionDirectionWorld);
			const float relaxation = 1.0f;
			float jacDiagABInv = relaxation / (denom0 + denom1);

			float j1;

			var rel_pos1 = contactPosWorld - body0.CenterOfMassPosition;
			var rel_pos2 = contactPosWorld - body1.CenterOfMassPosition;

			var vel1 = body0.GetVelocityInLocalPoint(rel_pos1);
			var vel2 = body1.GetVelocityInLocalPoint(rel_pos2);
			var vel = vel1 - vel2;

			Vector3.Dot(ref frictionDirectionWorld, ref vel, out float vrel);

			// calculate j that moves us to zero relative velocity
			j1 = -vrel * jacDiagABInv;
			j1 = Math.Min(j1, maxImpulse);
			j1 = Math.Max(j1, -maxImpulse);

			return j1;
		}

		private Vector3 blue = new Vector3(0, 0, 1);
		private Vector3 magenta = new Vector3(1, 0, 1);
		public void DebugDraw(IDebugDraw debugDrawer)
		{
			for (int v = 0; v < NumWheels; v++)
			{
				var wheelInfo = GetWheelInfo(v);

				Vector3 wheelColor;
				if (wheelInfo.RaycastInfo.IsInContact)
				{
					wheelColor = blue;
				}
				else
				{
					wheelColor = magenta;
				}

				var transform = wheelInfo.WorldTransform;
				var wheelPosWS = transform.Origin;

				var axle = new Vector3(
					transform[0, RightAxis],
					transform[1, RightAxis],
					transform[2, RightAxis]);

				var to1 = wheelPosWS + axle;
				var to2 = GetWheelInfo(v).RaycastInfo.ContactPointWS;

				//debug wheels (cylinders)
				debugDrawer.DrawLine(ref wheelPosWS, ref to1, ref wheelColor);
				debugDrawer.DrawLine(ref wheelPosWS, ref to2, ref wheelColor);

			}
		}

		public WheelInfo GetWheelInfo(int index)
		{
			Debug.Assert((index >= 0) && (index < NumWheels));

			return wheelInfo[index];
		}

		private float RayCast(WheelInfo wheel)
		{
			UpdateWheelTransformsWS(wheel, false);

			float depth = -1;
			float raylen = wheel.SuspensionRestLength + wheel.WheelsRadius;

			var rayvector = wheel.RaycastInfo.WheelDirectionWS * raylen;
			var source = wheel.RaycastInfo.HardPointWS;
			wheel.RaycastInfo.ContactPointWS = source + rayvector;
			var target = wheel.RaycastInfo.ContactPointWS;

			float param = 0;
			var rayResults = new VehicleRaycasterResult();

			Debug.Assert(vehicleRaycaster != null);
			object obj = vehicleRaycaster.CastRay(ref source, ref target, rayResults);

			wheel.RaycastInfo.GroundObject = null;

			if (obj != null)
			{
				param = rayResults.DistFraction;
				depth = raylen * rayResults.DistFraction;
				wheel.RaycastInfo.ContactNormalWS = rayResults.HitNormalInWorld;
				wheel.RaycastInfo.IsInContact = true;

				wheel.RaycastInfo.GroundObject = fixedBody;///@todo for driving on dynamic/movable objects!;
				/////wheel.RaycastInfo.GroundObject = object;

				float hitDistance = param * raylen;
				wheel.RaycastInfo.SuspensionLength = hitDistance - wheel.WheelsRadius;
				//clamp on max suspension travel

				float minSuspensionLength = wheel.SuspensionRestLength - wheel.MaxSuspensionTravelCm * 0.01f;
				float maxSuspensionLength = wheel.SuspensionRestLength + wheel.MaxSuspensionTravelCm * 0.01f;
				if (wheel.RaycastInfo.SuspensionLength < minSuspensionLength)
				{
					wheel.RaycastInfo.SuspensionLength = minSuspensionLength;
				}
				if (wheel.RaycastInfo.SuspensionLength > maxSuspensionLength)
				{
					wheel.RaycastInfo.SuspensionLength = maxSuspensionLength;
				}

				wheel.RaycastInfo.ContactPointWS = rayResults.HitPointInWorld;

				float denominator = Vector3.Dot(wheel.RaycastInfo.ContactNormalWS, wheel.RaycastInfo.WheelDirectionWS);

				Vector3 chassis_velocity_at_contactPoint;
				var relpos = wheel.RaycastInfo.ContactPointWS - RigidBody.CenterOfMassPosition;

				chassis_velocity_at_contactPoint = RigidBody.GetVelocityInLocalPoint(relpos);

				float projVel = Vector3.Dot(wheel.RaycastInfo.ContactNormalWS, chassis_velocity_at_contactPoint);

				if (denominator >= -0.1f)
				{
					wheel.SuspensionRelativeVelocity = 0;
					wheel.ClippedInvContactDotSuspension = 1.0f / 0.1f;
				}
				else
				{
					float inv = -1.0f / denominator;
					wheel.SuspensionRelativeVelocity = projVel * inv;
					wheel.ClippedInvContactDotSuspension = inv;
				}
			}
			else
			{
				//put wheel info as in rest position
				wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
				wheel.SuspensionRelativeVelocity = 0.0f;
				wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
				wheel.ClippedInvContactDotSuspension = 1.0f;
			}

			return depth;
		}

		private void ResetSuspension()
		{
			for (int i = 0; i < NumWheels; i++)
			{
				var wheel = GetWheelInfo(i);
				wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
				wheel.SuspensionRelativeVelocity = 0;

				wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
				//wheel.ContactFriction = 0;
				wheel.ClippedInvContactDotSuspension = 1;
			}
		}

		private void ResolveSingleBilateral(RigidBody body1, Vector3 pos1, RigidBody body2, Vector3 pos2, float distance, Vector3 normal, ref float impulse, float timeStep)
		{
			float normalLenSqr = normal.LengthSquared;
			Debug.Assert(Math.Abs(normalLenSqr) < 1.1f);
			if (normalLenSqr > 1.1f)
			{
				impulse = 0;
				return;
			}
			var rel_pos1 = pos1 - body1.CenterOfMassPosition;
			var rel_pos2 = pos2 - body2.CenterOfMassPosition;

			var vel1 = body1.GetVelocityInLocalPoint(rel_pos1);
			var vel2 = body2.GetVelocityInLocalPoint(rel_pos2);
			var vel = vel1 - vel2;

			var world2A = Matrix.Transpose(body1.CenterOfMassTransform.Basis);
			var world2B = Matrix.Transpose(body2.CenterOfMassTransform.Basis);
			var m_aJ = Vector3.TransformCoordinate(Vector3.Cross(rel_pos1, normal), world2A);
			var m_bJ = Vector3.TransformCoordinate(Vector3.Cross(rel_pos2, -normal), world2B);
			var m_0MinvJt = body1.InvInertiaDiagLocal * m_aJ;
			var m_1MinvJt = body2.InvInertiaDiagLocal * m_bJ;
			Vector3.Dot(ref m_0MinvJt, ref m_aJ, out float dot0);
			Vector3.Dot(ref m_1MinvJt, ref m_bJ, out float dot1);
			float jacDiagAB = body1.InvMass + dot0 + body2.InvMass + dot1;
			float jacDiagABInv = 1.0f / jacDiagAB;

			Vector3.Dot(ref normal, ref vel, out float rel_vel);

			//todo: move this into proper structure
			const float contactDamping = 0.2f;

#if ONLY_USE_LINEAR_MASS
	        float massTerm = 1.0f / (body1.InvMass + body2.InvMass);
	        impulse = - contactDamping * rel_vel * massTerm;
#else
			float velocityImpulse = -contactDamping * rel_vel * jacDiagABInv;
			impulse = velocityImpulse;
#endif
		}

		public void UpdateAction(CollisionWorld collisionWorld, float deltaTimeStep)
		{
			UpdateVehicle(deltaTimeStep);
		}

		private const float sideFrictionStiffness2 = 1.0f;
		public void UpdateFriction(float timeStep)
		{
			//calculate the impulse, so that the wheels don't move sidewards
			int numWheel = NumWheels;
			if (numWheel == 0)
			{
				return;
			}

			Array.Resize(ref forwardWS, numWheel);
			Array.Resize(ref axle, numWheel);
			Array.Resize(ref forwardImpulse, numWheel);
			Array.Resize(ref sideImpulse, numWheel);

			int numWheelsOnGround = 0;

			//collapse all those loops into one!
			for (int i = 0; i < NumWheels; i++)
			{
				var groundObject = wheelInfo[i].RaycastInfo.GroundObject as RigidBody;
				if (groundObject != null)
				{
					numWheelsOnGround++;
				}

				sideImpulse[i] = 0;
				forwardImpulse[i] = 0;
			}

			for (int i = 0; i < NumWheels; i++)
			{
				var wheel = wheelInfo[i];

				var groundObject = wheel.RaycastInfo.GroundObject as RigidBody;
				if (groundObject != null)
				{
					var wheelTrans = GetWheelTransformWS(i);

					axle[i] = new Vector3(
						wheelTrans[0, indexRightAxis],
						wheelTrans[1, indexRightAxis],
						wheelTrans[2, indexRightAxis]);

					var surfNormalWS = wheel.RaycastInfo.ContactNormalWS;
					Vector3.Dot(ref axle[i], ref surfNormalWS, out float proj);
					axle[i] -= surfNormalWS * proj;
					axle[i].Normalize();

					Vector3.Cross(ref surfNormalWS, ref axle[i], out forwardWS[i]);
					forwardWS[i].Normalize();

					ResolveSingleBilateral(chassisBody, wheel.RaycastInfo.ContactPointWS,
							  groundObject, wheel.RaycastInfo.ContactPointWS,
							  0, axle[i], ref sideImpulse[i], timeStep);

					sideImpulse[i] *= sideFrictionStiffness2;
				}
			}

			const float sideFactor = 1.0f;
			const float fwdFactor = 0.5f;

			bool sliding = false;

			for (int i = 0; i < NumWheels; i++)
			{
				var wheel = wheelInfo[i];
				var groundObject = wheel.RaycastInfo.GroundObject as RigidBody;

				float rollingFriction = 0.0f;

				if (groundObject != null)
				{
					if (wheel.EngineForce != 0.0f)
					{
						rollingFriction = wheel.EngineForce * timeStep;
					}
					else
					{
						float defaultRollingFrictionImpulse = 0.0f;
						float maxImpulse = (wheel.Brake != 0) ? wheel.Brake : defaultRollingFrictionImpulse;
						rollingFriction = CalcRollingFriction(chassisBody, groundObject, wheel.RaycastInfo.ContactPointWS, forwardWS[i], maxImpulse);
					}
				}

				//switch between active rolling (throttle), braking and non-active rolling friction (no throttle/break)

				forwardImpulse[i] = 0;
				wheelInfo[i].SkidInfo = 1.0f;

				if (groundObject != null)
				{
					wheelInfo[i].SkidInfo = 1.0f;

					float maximp = wheel.WheelsSuspensionForce * timeStep * wheel.FrictionSlip;
					float maximpSide = maximp;

					float maximpSquared = maximp * maximpSide;


					forwardImpulse[i] = rollingFriction;//wheel.EngineForce* timeStep;

					float x = forwardImpulse[i] * fwdFactor;
					float y = sideImpulse[i] * sideFactor;

					float impulseSquared = (x * x + y * y);

					if (impulseSquared > maximpSquared)
					{
						sliding = true;

						float factor = maximp / (float)Math.Sqrt(impulseSquared);

						wheelInfo[i].SkidInfo *= factor;
					}
				}
			}

			if (sliding)
			{
				for (int wheel = 0; wheel < NumWheels; wheel++)
				{
					if (sideImpulse[wheel] != 0)
					{
						if (wheelInfo[wheel].SkidInfo < 1.0f)
						{
							forwardImpulse[wheel] *= wheelInfo[wheel].SkidInfo;
							sideImpulse[wheel] *= wheelInfo[wheel].SkidInfo;
						}
					}
				}
			}

			// apply the impulses
			for (int i = 0; i < NumWheels; i++)
			{
				var wheel = wheelInfo[i];

				var rel_pos = wheel.RaycastInfo.ContactPointWS -
						chassisBody.CenterOfMassPosition;

				if (forwardImpulse[i] != 0)
				{
					chassisBody.ApplyImpulse(forwardWS[i] * forwardImpulse[i], rel_pos);
				}
				if (sideImpulse[i] != 0)
				{
					var groundObject = wheel.RaycastInfo.GroundObject as RigidBody;

					var rel_pos2 = wheel.RaycastInfo.ContactPointWS -
						groundObject.CenterOfMassPosition;


					var sideImp = axle[i] * sideImpulse[i];

#if ROLLING_INFLUENCE_FIX // fix. It only worked if car's up was along Y - VT.
					//Vector4 vChassisWorldUp = RigidBody.CenterOfMassTransform.get_Columns(indexUpAxis);
					var vChassisWorldUp = new Vector3(
						RigidBody.CenterOfMassTransform.Row1[indexUpAxis],
						RigidBody.CenterOfMassTransform.Row2[indexUpAxis],
						RigidBody.CenterOfMassTransform.Row3[indexUpAxis]);
					Vector3.Dot(ref vChassisWorldUp, ref rel_pos, out float dot);
					rel_pos -= vChassisWorldUp * (dot * (1.0f - wheel.RollInfluence));
#else
                    rel_pos[indexUpAxis] *= wheel.RollInfluence;
#endif
					chassisBody.ApplyImpulse(sideImp, rel_pos);

					//apply friction impulse on the ground
					groundObject.ApplyImpulse(-sideImp, rel_pos2);
				}
			}
		}

		public void UpdateSuspension(float step)
		{
			float chassisMass = 1.0f / chassisBody.InvMass;

			for (int w_it = 0; w_it < NumWheels; w_it++)
			{
				var wheel_info = wheelInfo[w_it];

				if (wheel_info.RaycastInfo.IsInContact)
				{
					float force;
					//	Spring
					{
						float susp_length = wheel_info.SuspensionRestLength;
						float current_length = wheel_info.RaycastInfo.SuspensionLength;

						float length_diff = (susp_length - current_length);

						force = wheel_info.SuspensionStiffness
							* length_diff * wheel_info.ClippedInvContactDotSuspension;
					}

					// Damper
					{
						float projected_rel_vel = wheel_info.SuspensionRelativeVelocity;
						{
							float susp_damping;
							if (projected_rel_vel < 0.0f)
							{
								susp_damping = wheel_info.WheelsDampingCompression;
							}
							else
							{
								susp_damping = wheel_info.WheelsDampingRelaxation;
							}
							force -= susp_damping * projected_rel_vel;
						}
					}

					// RESULT
					wheel_info.WheelsSuspensionForce = force * chassisMass;
					if (wheel_info.WheelsSuspensionForce < 0)
					{
						wheel_info.WheelsSuspensionForce = 0;
					}
				}
				else
				{
					wheel_info.WheelsSuspensionForce = 0;
				}
			}
		}

		public void UpdateVehicle(float step)
		{
			for (int i = 0; i < NumWheels; i++)
			{
				UpdateWheelTransform(i, false);
			}

			currentVehicleSpeedKmHour = 3.6f * RigidBody.LinearVelocity.Length;

			var chassisTrans = ChassisWorldTransform;

			var forwardW = new Vector3(
				chassisTrans[0, indexForwardAxis],
				chassisTrans[1, indexForwardAxis],
				chassisTrans[2, indexForwardAxis]);

			if (Vector3.Dot(forwardW, RigidBody.LinearVelocity) < 0)
			{
				currentVehicleSpeedKmHour *= -1.0f;
			}

			// Simulate suspension
			/*
            for (int i = 0; i < wheelInfo.Count; i++)
            {
                float depth = RayCast(wheelInfo[i]);
            }
            */


			UpdateSuspension(step);

			for (int i = 0; i < wheelInfo.Count; i++)
			{
				//apply suspension force
				var wheel = wheelInfo[i];

				float suspensionForce = wheel.WheelsSuspensionForce;

				if (suspensionForce > wheel.MaxSuspensionForce)
				{
					suspensionForce = wheel.MaxSuspensionForce;
				}
				var impulse = wheel.RaycastInfo.ContactNormalWS * suspensionForce * step;
				var relpos = wheel.RaycastInfo.ContactPointWS - RigidBody.CenterOfMassPosition;

				RigidBody.ApplyImpulse(impulse, relpos);
			}


			UpdateFriction(step);

			for (int i = 0; i < wheelInfo.Count; i++)
			{
				var wheel = wheelInfo[i];
				var relpos = wheel.RaycastInfo.HardPointWS - RigidBody.CenterOfMassPosition;
				var vel = RigidBody.GetVelocityInLocalPoint(relpos);

				if (wheel.RaycastInfo.IsInContact)
				{
					var chassisWorldTransform = ChassisWorldTransform;

					var fwd = new Vector3(
						chassisWorldTransform[0, indexForwardAxis],
						chassisWorldTransform[1, indexForwardAxis],
						chassisWorldTransform[2, indexForwardAxis]);

					float proj = Vector3.Dot(fwd, wheel.RaycastInfo.ContactNormalWS);
					fwd -= wheel.RaycastInfo.ContactNormalWS * proj;

					Vector3.Dot(ref fwd, ref vel, out float proj2);

					wheel.DeltaRotation = (proj2 * step) / (wheel.WheelsRadius);
					wheel.Rotation += wheel.DeltaRotation;
				}
				else
				{
					wheel.Rotation += wheel.DeltaRotation;
				}

				wheel.DeltaRotation *= 0.99f;//damping of rotation when not in contact
			}
		}

		public void UpdateWheelTransform(int wheelIndex, bool interpolatedTransform)
		{
			var wheel = wheelInfo[wheelIndex];
			UpdateWheelTransformsWS(wheel, interpolatedTransform);
			var up = -wheel.RaycastInfo.WheelDirectionWS;
			var right = wheel.RaycastInfo.WheelAxleWS;
			var fwd = Vector3.Cross(up, right);
			fwd.Normalize();
			//up = Vector3.Cross(right, fwd);
			//up.Normalize();

			//rotate around steering over the wheelAxleWS
			var steeringMat = Matrix.RotationAxis(up, wheel.Steering);
			var rotatingMat = Matrix.RotationAxis(right, -wheel.Rotation);

			var basis2 = new Matrix
			{
				M11 = right[0],
				M12 = fwd[0],
				M13 = up[0],
				M21 = right[1],
				M22 = fwd[1],
				M23 = up[1],
				M31 = right[2],
				M32 = fwd[2]
			};
			basis2.M13 = up[2];

			var transform = steeringMat * rotatingMat * basis2;
			transform.Origin = wheel.RaycastInfo.HardPointWS + wheel.RaycastInfo.WheelDirectionWS * wheel.RaycastInfo.SuspensionLength;
			wheel.WorldTransform = transform;
		}

		private void UpdateWheelTransformsWS(WheelInfo wheel, bool interpolatedTransform)
		{
			wheel.RaycastInfo.IsInContact = false;

			var chassisTrans = ChassisWorldTransform;
			if (interpolatedTransform && RigidBody.MotionState != null)
			{
				chassisTrans = RigidBody.MotionState.WorldTransform;
			}

			wheel.RaycastInfo.HardPointWS = Vector3.TransformCoordinate(wheel.ChassisConnectionPointCS, chassisTrans);
			var chassisTransBasis = chassisTrans.Basis;
			wheel.RaycastInfo.WheelDirectionWS = Vector3.TransformCoordinate(wheel.WheelDirectionCS, chassisTransBasis);
			wheel.RaycastInfo.WheelAxleWS = Vector3.TransformCoordinate(wheel.WheelAxleCS, chassisTransBasis);
		}
	}
}
