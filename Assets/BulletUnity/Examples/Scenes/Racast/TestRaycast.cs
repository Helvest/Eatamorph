using BulletSharp;
using BulletUnity;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
	public void Update()
	{
		if (Time.frameCount == 10)
		{
			var fromP = transform.position.ToBullet();
			var toP = (transform.position + Vector3.down * 10f).ToBullet();
			var callback = new ClosestRayResultCallback(ref fromP, ref toP);
			var world = WorldsManager.WorldController;
			world.World.RayTest(fromP, toP, callback);

			if (callback.HasHit)
			{
				Debug.LogFormat("Hit p={0} n={1} obj{2}", callback.HitPointWorld, callback.HitNormalWorld, callback.CollisionObject.UserObject);
			}
		}
	}
}
