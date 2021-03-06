using System.Collections.Generic;
using BulletSharp;
using UnityEngine;

namespace BulletUnity
{
	public class BCollisionCallbacksDefault : BCollisionCallbacks
	{
		public class PersistentManifoldList
		{
			public List<PersistentManifold> manifolds = new List<PersistentManifold>();
		}

		private CollisionObject myCollisionObject;
		private Dictionary<CollisionObject, PersistentManifoldList> otherObjs2ManifoldMap = new Dictionary<CollisionObject, PersistentManifoldList>();
		private List<PersistentManifoldList> newContacts = new List<PersistentManifoldList>();
		private List<CollisionObject> objectsToRemove = new List<CollisionObject>();

		private void Start()
		{
			var co = GetComponent<BCollisionObject>();

			if (co == null)
			{
				Debug.LogError("BCollisionCallbacksDefault must be attached to an object with a BCollisionObject.");
				return;
			}

			myCollisionObject = co.GetCollisionObject();
		}

		public override void OnVisitPersistentManifold(PersistentManifold pm)
		{
			CollisionObject other;
			if (pm.NumContacts > 0)
			{
				if (pm.Body0 == myCollisionObject)
				{
					other = pm.Body1;
				}
				else
				{
					other = pm.Body0;
				}

				if (!otherObjs2ManifoldMap.TryGetValue(other, out var pml))
				{
					//todo get PersistentManifoldList from object pool
					//this is first contact with this other object
					//might have multiple new contacts with same object stored in separate persistent manifolds
					//don't add two different lists to new contacts
					bool foundExisting = false;
					for (int i = 0; i < newContacts.Count; i++)
					{
						if (newContacts[i].manifolds[0].Body0 == other ||
							newContacts[i].manifolds[0].Body1 == other)
						{
							foundExisting = true;
							newContacts[i].manifolds.Add(pm);
						}
					}

					if (!foundExisting)
					{
						pml = new PersistentManifoldList();
						newContacts.Add(pml);
						pml.manifolds.Add(pm);
						//don't add to otherObjs2ManifoldMap here. It messes up onStay do it after all pm's have been visited.
					}
				}
				else
				{
					pml.manifolds.Add(pm);
				}
			}
		}

		public override void OnFinishedVisitingManifolds()
		{
			objectsToRemove.Clear();
			foreach (var co in otherObjs2ManifoldMap.Keys)
			{
				var pml = otherObjs2ManifoldMap[co];
				if (pml.manifolds.Count > 0)
				{
					BOnCollisionStay(co, pml);
				}
				else
				{
					BOnCollisionExit(co);
					objectsToRemove.Add(co);
				}
			}

			for (int i = 0; i < objectsToRemove.Count; i++)
			{
				otherObjs2ManifoldMap.Remove(objectsToRemove[i]);
			}

			objectsToRemove.Clear();

			for (int i = 0; i < newContacts.Count; i++)
			{
				var pml = newContacts[i];
				CollisionObject other;
				if (pml.manifolds[0].Body0 == myCollisionObject)
				{
					other = pml.manifolds[0].Body1;
				}
				else
				{
					other = pml.manifolds[0].Body0;
				}
				otherObjs2ManifoldMap.Add(other, pml);
				BOnCollisionEnter(other, pml);
			}

			newContacts.Clear();

			foreach (var co in otherObjs2ManifoldMap.Keys)
			{
				var pml = otherObjs2ManifoldMap[co];
				pml.manifolds.Clear();
			}
		}

		/// <summary>
		///Beware of creating, destroying, adding or removing bullet objects inside these functions. Doing so can alter the list of collisions and ContactManifolds 
		///that are being iteratated over
		///(comodification). This can result in infinite loops, null pointer exceptions, out of sequence Enter,Stay,Exit, etc... A good way to handle this sitution is 
		///to collect the information in these callbacks then override "OnFinishedVisitingManifolds" like:
		///
		/// public override void OnFinishedVisitingManifolds()
		/// {
		///     base.OnFinishedVistingManifolds(); //don't omit this it does the callbacks
		///     do my Instantiation and deletion here.
		/// }
		/// </summary>

		public delegate void theEvent(BCollisionObject other);

		public theEvent OnCollisionEnter;
		public theEvent OnCollisionStay;
		public theEvent OnCollisionExit;

		protected virtual void BOnCollisionEnter(CollisionObject other, PersistentManifoldList manifoldList)
		{
			OnCollisionEnter?.Invoke((BCollisionObject)other.UserObject);
		}

		protected virtual void BOnCollisionStay(CollisionObject other, PersistentManifoldList manifoldList)
		{
			OnCollisionStay?.Invoke((BCollisionObject)other.UserObject);
		}

		protected virtual void BOnCollisionExit(CollisionObject other)
		{
			OnCollisionExit?.Invoke((BCollisionObject)other.UserObject);
		}
	}
}
