using System.Collections;
using BulletUnity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BBvhTriangleMeshShape))]
public class BBvhTriangleMeshShapeEditor : Editor
{
	private BBvhTriangleMeshShape script;

	//SerializedProperty hullMesh;

	private void OnEnable()
	{
		script = (BBvhTriangleMeshShape)target;
		//GetSerializedProperties();
	}
	/*
    void GetSerializedProperties()
    {
        hullMesh = serializedObject.FindProperty("hullMesh");
    }
    */

	public override void OnInspectorGUI()
	{
		if (script.transform.localScale != Vector3.one)
		{
			EditorGUILayout.HelpBox("This shape doesn't support scale of the object.\nThe scale must be one", MessageType.Warning);
		}
		//EditorGUILayout.PropertyField(hullMesh);
		script.HullMesh = (Mesh)EditorGUILayout.ObjectField("Hull Mesh", script.HullMesh, typeof(Mesh), true);
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
