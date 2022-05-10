using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BulletSharp;
using BulletSharp.Math;
using BulletSharpExamples;

namespace DemoFramework
{
	public abstract class Demo : IDisposable
	{
		protected Graphics Graphics { get; set; }
		public FreeLook Freelook { get; set; }
		public Input Input { get; set; }

		// Frame counting
		private Clock clock = new Clock();
		private float frameAccumulator;
		private int frameCount;
		private float _frameDelta;
		public float FrameDelta => _frameDelta;

		public float FramesPerSecond { get; private set; }

		// Physics
		protected DynamicsWorld _world;
		public DynamicsWorld World
		{
			get => _world;
			protected set => _world = value;
		}

		protected CollisionConfiguration CollisionConf;
		protected CollisionDispatcher Dispatcher;
		protected BroadphaseInterface Broadphase;
		protected ConstraintSolver Solver;
		public List<CollisionShape> CollisionShapes { get; private set; }

		protected BoxShape shootBoxShape;
		protected float shootBoxInitialSpeed = 40;
		private RigidBody pickedBody;
		protected TypedConstraint pickConstraint;
		private float oldPickingDist;

		// Debug drawing
		private bool _isDebugDrawEnabled;
		private DebugDrawModes _debugDrawMode = DebugDrawModes.DrawWireframe;
		private IDebugDraw _debugDrawer;

		public DebugDrawModes DebugDrawMode
		{
			get => _debugDrawMode;
			set
			{
				_debugDrawMode = value;
				if (_debugDrawer != null)
				{
					_debugDrawer.DebugMode = value;
				}
			}
		}

		public bool IsDebugDrawEnabled
		{
			get => _isDebugDrawEnabled;
			set
			{
				if (value)
				{
					if (_debugDrawer == null)
					{
						_debugDrawer = Graphics.GetPhysicsDebugDrawer();
						_debugDrawer.DebugMode = _debugDrawMode;
						if (_world != null)
						{
							_world.DebugDrawer = _debugDrawer;
						}
					}
				}
				else
				{
					if (_debugDrawer != null)
					{
						if (_world != null)
						{
							_world.DebugDrawer = null;
						}
						if (_debugDrawer is IDisposable)
						{
							(_debugDrawer as IDisposable).Dispose();
						}
						_debugDrawer = null;
					}
				}
				_isDebugDrawEnabled = value;
			}
		}

		private bool isCullingEnabled = true;
		public bool CullingEnabled
		{
			get => isCullingEnabled;
			set
			{
				Graphics.CullingEnabled = value;
				isCullingEnabled = value;
			}
		}

		public Demo()
		{
			CollisionShapes = new List<CollisionShape>();
		}

		public void Run()
		{
			using (Graphics = GraphicsLibraryManager.GetGraphics(this))
			{
				Input = new Input(Graphics.Form);
				Freelook = new FreeLook(Input);

				Graphics.Initialize();
				Graphics.CullingEnabled = isCullingEnabled;
				OnInitialize();

				if (World == null)
				{
					OnInitializePhysics();
					BulletExampleRunner.Get().PostOnInitializePhysics();
				}

				if (_isDebugDrawEnabled)
				{
					if (_debugDrawer == null)
					{
						_debugDrawer = Graphics.GetPhysicsDebugDrawer();
						_debugDrawer.DebugMode = DebugDrawMode;
					}
					if (World != null)
					{
						World.DebugDrawer = _debugDrawer;
					}
				}

				Graphics.UpdateView();

				clock.Start();

				Graphics.Run();

				/*
                if (_debugDrawer != null)
                {
                    if (World != null)
                    {
                        World.DebugDrawer = null;
                    }
                    if (_debugDrawer is IDisposable)
                    {
                        (_debugDrawer as IDisposable).Dispose();
                    }
                    _debugDrawer = null;
                }
                */
			}
			//Graphics = null;
		}

		protected virtual void OnInitialize()
		{
			UnityEngine.Debug.Log("OnInitialize");
		}

		protected virtual void OnInitializePhysics()
		{
			UnityEngine.Debug.Log("OnInitializePhysics");
		}

		public virtual void ClientResetScene()
		{
			RemovePickingConstraint();
			ExitPhysics();
			OnInitializePhysics();
			BulletExampleRunner.Get().PostOnInitializePhysics();
			if (World != null && _debugDrawer != null)
			{
				World.DebugDrawer = _debugDrawer;
			}
		}

		public virtual void ExitPhysics()
		{
			BulletExampleRunner.Get().ExitPhysics();
			UnityEngine.Debug.Log("ExitPhysics");
			if (_world != null)
			{
				//remove/dispose constraints
				int i;
				for (i = _world.NumConstraints - 1; i >= 0; i--)
				{
					var constraint = _world.GetConstraint(i);
					_world.RemoveConstraint(constraint);
					constraint.Dispose();
				}

				//remove the rigidbodies from the dynamics world and delete them
				for (i = _world.NumCollisionObjects - 1; i >= 0; i--)
				{
					var obj = _world.CollisionObjectArray[i];
					var body = obj as RigidBody;
					if (body != null && body.MotionState != null)
					{
						body.MotionState.Dispose();
					}
					_world.RemoveCollisionObject(obj);
					obj.Dispose();
				}

				//delete collision shapes
				foreach (var shape in CollisionShapes)
				{
					shape.Dispose();
				}

				CollisionShapes.Clear();

				_world.Dispose();
				Broadphase.Dispose();
				Dispatcher.Dispose();
				CollisionConf.Dispose();
			}

			if (Broadphase != null)
			{
				Broadphase.Dispose();
			}
			if (Dispatcher != null)
			{
				Dispatcher.Dispose();
			}
			if (CollisionConf != null)
			{
				CollisionConf.Dispose();
			}
		}

		public virtual void OnUpdate()
		{
			_frameDelta = clock.Update();
			frameAccumulator += _frameDelta;
			++frameCount;
			if (frameAccumulator >= 1.0f)
			{
				FramesPerSecond = frameCount / frameAccumulator;

				frameAccumulator = 0.0f;
				frameCount = 0;
			}

			if (_world != null)
			{
				_world.StepSimulation(_frameDelta);
			}

			if (Freelook.Update(_frameDelta))
			{
				Graphics.UpdateView();
			}

			Input.ClearKeyCache();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ExitPhysics();
			}
		}


		public virtual void OnHandleInput()
		{
			if (Input.KeysPressed.Count != 0)
			{
				switch (Input.KeysPressed[0])
				{
					case Keys.Escape:
					case Keys.Q:
						Graphics.Form.Close();
						return;
					case Keys.F3:
						IsDebugDrawEnabled = !IsDebugDrawEnabled;
						break;
					case Keys.F8:
						Input.ClearKeyCache();
						GraphicsLibraryManager.ExitWithReload = true;
						Graphics.Form.Close();
						break;
					case Keys.F11:
						Graphics.IsFullScreen = !Graphics.IsFullScreen;
						break;
					case (Keys.Control | Keys.F):
						const int maxSerializeBufferSize = 1024 * 1024 * 5;
						var serializer = new DefaultSerializer(maxSerializeBufferSize);
						World.Serialize(serializer);

						byte[] dataBytes = new byte[serializer.CurrentBufferSize];
						Marshal.Copy(serializer.BufferPointer, dataBytes, 0, dataBytes.Length);

						var file = new System.IO.FileStream("world.bullet", System.IO.FileMode.Create);
						file.Write(dataBytes, 0, dataBytes.Length);
						file.Dispose();
						break;
					case Keys.G:
						//shadowsEnabled = !shadowsEnabled;
						break;
					case Keys.Space:
						ShootBox(Freelook.Eye, GetRayTo(Input.MousePoint, Freelook.Eye, Freelook.Target, Graphics.FieldOfView));
						break;
					case Keys.Return:
						ClientResetScene();
						break;
				}
			}

			if (Input.MousePressed != MouseButtons.None)
			{
				var rayTo = GetRayTo(Input.MousePoint, Freelook.Eye, Freelook.Target, Graphics.FieldOfView);

				if (Input.MousePressed == MouseButtons.Right)
				{
					if (_world != null)
					{
						var rayFrom = Freelook.Eye;

						var rayCallback = new ClosestRayResultCallback(ref rayFrom, ref rayTo);
						_world.RayTestRef(ref rayFrom, ref rayTo, rayCallback);
						if (rayCallback.HasHit)
						{
							var body = rayCallback.CollisionObject as RigidBody;
							if (body != null)
							{
								if (!(body.IsStaticObject || body.IsKinematicObject))
								{
									pickedBody = body;
									pickedBody.ActivationState = ActivationState.DisableDeactivation;

									var pickPos = rayCallback.HitPointWorld;
									var localPivot = Vector3.TransformCoordinate(pickPos, Matrix.Invert(body.CenterOfMassTransform));

									if (Input.KeysDown.Contains(Keys.ShiftKey))
									{
										var dof6 = new Generic6DofConstraint(body, Matrix.Translation(localPivot), false)
										{
											LinearLowerLimit = Vector3.Zero,
											LinearUpperLimit = Vector3.Zero,
											AngularLowerLimit = Vector3.Zero,
											AngularUpperLimit = Vector3.Zero
										};

										_world.AddConstraint(dof6);
										pickConstraint = dof6;

										dof6.SetParam(ConstraintParam.StopCfm, 0.8f, 0);
										dof6.SetParam(ConstraintParam.StopCfm, 0.8f, 1);
										dof6.SetParam(ConstraintParam.StopCfm, 0.8f, 2);
										dof6.SetParam(ConstraintParam.StopCfm, 0.8f, 3);
										dof6.SetParam(ConstraintParam.StopCfm, 0.8f, 4);
										dof6.SetParam(ConstraintParam.StopCfm, 0.8f, 5);

										dof6.SetParam(ConstraintParam.StopErp, 0.1f, 0);
										dof6.SetParam(ConstraintParam.StopErp, 0.1f, 1);
										dof6.SetParam(ConstraintParam.StopErp, 0.1f, 2);
										dof6.SetParam(ConstraintParam.StopErp, 0.1f, 3);
										dof6.SetParam(ConstraintParam.StopErp, 0.1f, 4);
										dof6.SetParam(ConstraintParam.StopErp, 0.1f, 5);
									}
									else
									{
										var p2p = new Point2PointConstraint(body, localPivot);
										_world.AddConstraint(p2p);
										pickConstraint = p2p;
										p2p.Setting.ImpulseClamp = 30;
										//very weak constraint for picking
										p2p.Setting.Tau = 0.001f;
										/*
                                        p2p.SetParam(ConstraintParams.Cfm, 0.8f, 0);
                                        p2p.SetParam(ConstraintParams.Cfm, 0.8f, 1);
                                        p2p.SetParam(ConstraintParams.Cfm, 0.8f, 2);
                                        p2p.SetParam(ConstraintParams.Erp, 0.1f, 0);
                                        p2p.SetParam(ConstraintParams.Erp, 0.1f, 1);
                                        p2p.SetParam(ConstraintParams.Erp, 0.1f, 2);
                                        */
									}

									oldPickingDist = (pickPos - rayFrom).Length;
								}
							}
						}
						rayCallback.Dispose();
					}
				}
			}
			else if (Input.MouseReleased == MouseButtons.Right)
			{
				RemovePickingConstraint();
			}

			// Mouse movement
			if (Input.MouseDown == MouseButtons.Right)
			{
				if (pickConstraint != null)
				{
					var newRayTo = GetRayTo(Input.MousePoint, Freelook.Eye, Freelook.Target, Graphics.FieldOfView);

					if (pickConstraint.ConstraintType == TypedConstraintType.D6)
					{
						var pickCon = pickConstraint as Generic6DofConstraint;

						//keep it at the same picking distance
						var rayFrom = Freelook.Eye;
						var dir = newRayTo - rayFrom;
						dir.Normalize();
						dir *= oldPickingDist;
						var newPivotB = rayFrom + dir;

						var tempFrameOffsetA = pickCon.FrameOffsetA;
						tempFrameOffsetA.M41 = newPivotB.X;
						tempFrameOffsetA.M42 = newPivotB.Y;
						tempFrameOffsetA.M43 = newPivotB.Z;
						pickCon.SetFrames(tempFrameOffsetA, pickCon.FrameOffsetB);
					}
					else
					{
						var pickCon = pickConstraint as Point2PointConstraint;

						//keep it at the same picking distance
						var rayFrom = Freelook.Eye;
						var dir = newRayTo - rayFrom;
						dir.Normalize();
						dir *= oldPickingDist;
						pickCon.PivotInB = rayFrom + dir;
					}
				}
			}
		}

		private void RemovePickingConstraint()
		{
			if (pickConstraint != null && _world != null)
			{
				_world.RemoveConstraint(pickConstraint);
				pickConstraint.Dispose();
				pickConstraint = null;
				pickedBody.ForceActivationState(ActivationState.ActiveTag);
				pickedBody.DeactivationTime = 0;
				pickedBody = null;
			}
		}

		protected Vector3 GetRayTo(Point point, Vector3 eye, Vector3 target, float fov)
		{
			float aspect;

			var rayForward = target - eye;
			rayForward.Normalize();
			const float farPlane = 10000.0f;
			rayForward *= farPlane;

			var vertical = Freelook.Up;

			var hor = Vector3.Cross(rayForward, vertical);
			hor.Normalize();
			vertical = Vector3.Cross(hor, rayForward);
			vertical.Normalize();

			float tanFov = (float)Math.Tan(fov / 2);
			hor *= 2.0f * farPlane * tanFov;
			vertical *= 2.0f * farPlane * tanFov;

			var clientSize = Graphics.Form.ClientSize;
			if (clientSize.Width > clientSize.Height)
			{
				aspect = clientSize.Width / (float)clientSize.Height;
				hor *= aspect;
			}
			else
			{
				aspect = clientSize.Height / (float)clientSize.Width;
				vertical *= aspect;
			}

			var rayToCenter = eye + rayForward;
			var dHor = hor / clientSize.Width;
			var dVert = vertical / clientSize.Height;

			var rayTo = rayToCenter - 0.5f * hor + 0.5f * vertical;
			rayTo += (clientSize.Width - point.X) * dHor;
			rayTo -= point.Y * dVert;
			return rayTo;
		}

		public virtual void ShootBox(Vector3 camPos, Vector3 destination)
		{
			AllocConsole();
			if (_world == null)
			{
				return;
			}

			const float mass = 1.0f;

			if (shootBoxShape == null)
			{
				shootBoxShape = new BoxShape(1.0f);
				//shootBoxShape.InitializePolyhedralFeatures();
			}

			var body = LocalCreateRigidBody(mass, Matrix.Translation(camPos), shootBoxShape);
			body.LinearFactor = new Vector3(1, 1, 1);
			//body.Restitution = 1;

			var linVel = destination - camPos;
			linVel.Normalize();

			body.LinearVelocity = linVel * shootBoxInitialSpeed;
			body.CcdMotionThreshold = 0.5f;
			body.CcdSweptSphereRadius = 0.9f;
		}

		public virtual RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape, bool isKinematic = false)
		{
			//rigidbody is dynamic if and only if mass is non zero, otherwise static
			bool isDynamic = (mass != 0.0f);

			var localInertia = Vector3.Zero;
			if (isDynamic)
			{
				shape.CalculateLocalInertia(mass, out localInertia);
			}

			//using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
			var myMotionState = new DefaultMotionState(startTransform);

			var rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
			var body = new RigidBody(rbInfo);
			if (isKinematic)
			{
				body.CollisionFlags = body.CollisionFlags | CollisionFlags.KinematicObject;
				body.ActivationState = ActivationState.DisableDeactivation;
			}
			rbInfo.Dispose();

			_world.AddRigidBody(body);

			return body;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AllocConsole();
	}
}
