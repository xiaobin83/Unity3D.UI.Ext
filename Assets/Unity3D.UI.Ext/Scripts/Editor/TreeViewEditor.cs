using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ui
{
	public class TreeViewEditor : Editor
	{
		SerializedProperty propContent;
		SerializedProperty propIndent;
		SerializedProperty propPrefabEntry;

		void OnEnable()
		{
			propContent = serializedObject.FindProperty("m_Content");
			propIndent = serializedObject.FindProperty("m_PropIndent");
			propPrefabEntry = serializedObject.FindProperty("m_PrefabEntry");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(propContent);
			EditorGUILayout.PropertyField(propIndent);
			EditorGUILayout.PropertyField(propPrefabEntry);
		}
	}
}
