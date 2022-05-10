
//http://answers.unity3d.com/questions/31784/changing-the-order-of-components.html?page=2&pageSize=5&sort=votes


using System;
using System.Collections.Generic;
using System.Linq;
using BulletUnity;
using BulletUnity.Primitives;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class BSoftBodyComponentOrderSorter : ScriptableObject
{
	private class ComponentComparer : IComparer<Component>
	{
		private static readonly Type[] TypesOrder =
		{
			 typeof (Transform), // Transform is always first (though that doesn't really matter, as we can't  move it anyway).

            // Add your types here in the order you want them to be in the inspector.
           typeof (BSoftBody),
		   typeof (BPrimitive), //if present
 
            typeof (MeshFilter),
			typeof (MeshRenderer),
			typeof (LineRenderer),


		 };

		private int GetIndex(Component Component)
		{
			var Type = Component.GetType();

			var BestMatch = typeof(UnityEngine.Object);
			int BestIndex = int.MaxValue;
			for (int Index = 0; Index < TypesOrder.Length; Index++)
			{
				// If we found the exact type in the list, then this is the right index.
				var TypeOrder = TypesOrder[Index];
				if (Type == TypeOrder)
				{
					return Index;
				}

				// If we found a parent, then we switch to its place if it is more
				// "recent" (in the inheritance tree) than previously found parents.
				if (Type.IsSubclassOf(TypeOrder))
				{
					if (TypeOrder.IsSubclassOf(BestMatch))
					{
						BestMatch = TypeOrder;
						BestIndex = Index;
					}
				}
			}

			return BestIndex;
		}

		public int Compare(Component First, Component Second)
		{
			return Comparer<int>.Default.Compare(GetIndex(First), GetIndex(Second));
		}
	}


	[MenuItem("BulletForUnity/BSoftBody/SortScriptsInOrder")]
	private static void SortComponents()
	{
		var gameObject = Selection.activeGameObject;
		SortComponents(gameObject);
	}


	// [MenuItem("Edit/Sort Components %&a")]
	public static void SortComponents(GameObject go)
	{
		//var GameObject = Selection.activeGameObject;
		var SortedComponents = go.GetComponents<Component>()
			.Where(Component => Component.GetType() != typeof(Transform)).ToList();
		SortedComponents.Sort(new ComponentComparer());

		for (int Index = 0; Index < SortedComponents.Count; Index++)
		{
			var SortedComponent = SortedComponents[Index];
			var Components = go.GetComponents<Component>()
				.Where(Component => Component.GetType() != typeof(Transform)).ToList();
			int CurrentIndex = Components.IndexOf(SortedComponent);
			if (CurrentIndex < Index)
			{
				for (int MoveIndex = CurrentIndex; MoveIndex < Index; MoveIndex++)
				{
					UnityEditorInternal.ComponentUtility.MoveComponentDown(SortedComponent);
				}
			}
			else
			{
				for (int MoveIndex = CurrentIndex; MoveIndex > Index; MoveIndex--)
				{
					UnityEditorInternal.ComponentUtility.MoveComponentUp(SortedComponent);
				}
			}
		}
	}
}