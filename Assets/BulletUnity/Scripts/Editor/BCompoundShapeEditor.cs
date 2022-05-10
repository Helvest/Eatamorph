using System.Collections;
using BulletUnity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BCompoundShape))]
public class BCompoundShapeEditor : Editor
{
	private BCompoundShape script;
	private SerializedProperty shapes;

	private void OnEnable()
	{
		script = (BCompoundShape)target;
		GetSerializedProperties();
	}

	private void GetSerializedProperties()
	{
		shapes = serializedObject.FindProperty("colliders");
	}

	public override void OnInspectorGUI()
	{
		if (script.transform.localScale != Vector3.one)
		{
			EditorGUILayout.HelpBox("This shape doesn't support scale of the object.\nThe scale must be one", MessageType.Warning);
		}
		EditorGUILayout.PropertyField(shapes, true);
		script.LocalScaling = EditorGUILayout.Vector3Field("Local Scaling", script.LocalScaling);
		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(script);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			Repaint();
		}
	}
}
