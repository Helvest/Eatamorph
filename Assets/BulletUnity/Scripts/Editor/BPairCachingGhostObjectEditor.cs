using BulletUnity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BPairCachingGhostObject))]
public class BPairCachingGhostObjectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var obj = (BPairCachingGhostObject)target;

		obj.collisionFlags = BCollisionObjectEditor.RenderEnumMaskCollisionFlagsField(BCollisionObjectEditor.gcCollisionFlags, obj.collisionFlags);
		obj.groupsIBelongTo = BCollisionObjectEditor.RenderEnumMaskCollisionFilterGroupsField(BCollisionObjectEditor.gcGroupsIBelongTo, obj.groupsIBelongTo);
		obj.collisionMask = BCollisionObjectEditor.RenderEnumMaskCollisionFilterGroupsField(BCollisionObjectEditor.gcCollisionMask, obj.collisionMask);


		if (GUI.changed)
		{
			EditorUtility.SetDirty(obj);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			Undo.RecordObject(obj, "Undo Rigid Body");
		}
	}
}
