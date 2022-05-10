using BulletSharp;
using BulletUnity;
using UnityEngine;

public class BCamera
{
	private static CollisionObject closestRayResultReturnObj;

	public static CollisionObject ScreenPointToRay(Camera cam, Vector3 inputScreenCoords, CollisionFilterGroups rayFilterGroup, CollisionFilterGroups rayFilterMask)
	{
		/* Returns the first CollisionObject the ray hits.
         * Requires as Input: 
         * - Camera cam, UnityCamera from which the ray should be cast, e.g. Camera.main
         * - Vector3 inputScreenCoords, the screenpoint through which the ray should be cast. E.g. mousepointer Input.mousePosition
         * - CollisionFilterGroups rayFilterGroup, of which FilterGroup the ray belongs to
         * - CollisionFilterGroups rayFilterMask, which FilterMask the ray has (to map to other Objects FilterGroups
         * Be aware the Bulletphysics probably needs Group/Mask to match in both ways, i.e. Ray needs to collide with otherObj, as otherObj needs to collide with Ray.
         * To get all Ray hits, see commented out Callback AllHitsRayResultCallback below.
         * First version. Adapt to your needs. Refer to Bulletphysics.org for info. Make Pull Request to Phong BulletUnity with improvements.*/

		var collisionWorld = BPhysicsWorld.Get().world;
		var rayFrom = cam.transform.position.ToBullet();
		var rayTo = cam.ScreenToWorldPoint(new Vector3(inputScreenCoords.x, inputScreenCoords.y, cam.farClipPlane)).ToBullet();

		var rayCallBack = new ClosestRayResultCallback(ref rayFrom, ref rayTo)
		{
			CollisionFilterGroup = (short)rayFilterGroup,
			CollisionFilterMask = (short)rayFilterMask
		};
		//BulletSharp.AllHitsRayResultCallback rayCallBack = new BulletSharp.AllHitsRayResultCallback(rayFrom, rayTo);

		//Debug.Log("Casting ray from: " + rayFrom + " to: " + rayTo);
		collisionWorld.RayTest(rayFrom, rayTo, rayCallBack);

		closestRayResultReturnObj = null;
		if (rayCallBack.HasHit)
		{
			//Debug.Log("rayCallBack " + rayCallBack.GetType() + " had a hit: " + rayCallBack.CollisionObject.UserObject + " / of type: " + rayCallBack.CollisionObject.UserObject.GetType());
			closestRayResultReturnObj = rayCallBack.CollisionObject;
		}

		rayCallBack.Dispose();
		return closestRayResultReturnObj;
	}
}
