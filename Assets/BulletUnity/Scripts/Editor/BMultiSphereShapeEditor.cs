using BulletUnity;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BMultiSphereShape))]
public class BMultiSphereShapeEditor : Editor
{
	private BMultiSphereShape script;
	private SerializedProperty spheres;
	private SerializedProperty localScale;

	private void OnEnable()
	{
		script = (BMultiSphereShape)target;
		GetSerializedProperties();
	}

	private void GetSerializedProperties()
	{
		spheres = serializedObject.FindProperty("spheres");
	}

	public override void OnInspectorGUI()
	{
		if (script.transform.localScale != Vector3.one)
		{
			EditorGUILayout.HelpBox("This shape doesn't support scale of the object.\nThe scale must be one", MessageType.Warning);
		}

		EditorGUIUtility.wideMode = false;
		EditorGUILayout.PropertyField(spheres, true);
		EditorGUIUtility.wideMode = true;
		script.LocalScaling = EditorGUILayout.Vector3Field("Local Scaling", script.LocalScaling);
		serializedObject.ApplyModifiedProperties();
	}
}
