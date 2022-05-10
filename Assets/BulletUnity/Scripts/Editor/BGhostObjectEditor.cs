﻿using BulletUnity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BGhostObject))]
public class BGhostObjectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var obj = (BGhostObject)target;

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
