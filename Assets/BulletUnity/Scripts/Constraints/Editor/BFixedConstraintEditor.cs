using BulletUnity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BFixedConstraint))]
public class BFixedConstraintEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var hc = (BFixedConstraint)target;
		EditorGUILayout.HelpBox(BFixedConstraint.HelpMessage, MessageType.Info);
		BTypedConstraintEditor.DrawTypedConstraint(hc);
		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(hc);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			Repaint();
		}
	}
}
