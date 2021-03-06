using BulletSharp;
using UnityEngine;

namespace BulletUnity
{
	/* 
    Custom verson of the collision object for handling heightfields to deal with some issues matching terrains to heighfields
    1) Unity heitfiels have pivot at corner. Bullet heightfields have pivot at center
    2) Can't rotate unity heightfields        
    */
	public class BTerrainCollisionObject : BCollisionObject
	{
		private TerrainData td;

		protected override void Awake()
		{
			base.Awake();

			var t = GetComponent<Terrain>();
			if (t != null)
			{
				td = t.terrainData;
			}
		}

		//called by Physics World just before rigid body is added to world.
		//the current rigid body properties are used to rebuild the rigid body.
		internal override bool _BuildCollisionObject()
		{
			if (td == null)
			{
				Debug.LogError("Must be attached to an object with a terrain ");
				return false;
			}

			var world = GetWorld();
			if (collisionObject != null)
			{
				if (isInWorld && world != null)
				{
					isInWorld = false;
					world.RemoveCollisionObject(this);
				}
			}

			if (transform.localScale != Vector3.one)
			{
				Debug.LogError("The local scale on this collision shape is not one. Bullet physics does not support scaling on a rigid body world transform. Instead alter the dimensions of the CollisionShape.");
			}

			m_collisionShape = GetComponent<BCollisionShape>();
			if (m_collisionShape == null)
			{
				Debug.LogError("There was no collision shape component attached to this BRigidBody. " + name);
				return false;
			}

			if (!(m_collisionShape is BHeightfieldTerrainShape))
			{
				Debug.LogError("The collision shape needs to be a BHeightfieldTerrainShape. " + name);
				return false;
			}

			var cs = m_collisionShape.GetCollisionShape();
			//rigidbody is dynamic if and only if mass is non zero, otherwise static

			if (collisionObject == null)
			{
				collisionObject = new CollisionObject
				{
					CollisionShape = cs,
					UserObject = this
				};

				var worldTrans = BulletSharp.Math.Matrix.Identity;
				var pos = transform.position + new Vector3(td.size.x * .5f, td.size.y * .5f, td.size.z * .5f);
				worldTrans.Origin = pos.ToBullet();
				collisionObject.WorldTransform = worldTrans;
				collisionObject.CollisionFlags = m_collisionFlags;
			}
			else
			{
				collisionObject.CollisionShape = cs;
				var worldTrans = BulletSharp.Math.Matrix.Identity;
				var pos = transform.position + new Vector3(td.size.x * .5f, td.size.y * .5f, td.size.z * .5f);
				worldTrans.Origin = pos.ToBullet();
				collisionObject.WorldTransform = worldTrans;
				collisionObject.CollisionFlags = m_collisionFlags;
			}

			return true;
		}

		public override void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			if (isInWorld)
			{
				var newTrans = collisionObject.WorldTransform;
				newTrans.Origin = transform.position.ToBullet();
				collisionObject.WorldTransform = newTrans;
			}

			transform.SetPositionAndRotation(position, rotation);
		}
	}
}
