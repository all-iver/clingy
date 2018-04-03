namespace SubC.Attachments.ClingyEditor {

	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Attacher))]
	public class AttacherEditor : Editor {
		Attacher attacher;
        AttachObjectListEditor objectListEditor;

		void OnEnable() {
			attacher = (Attacher) target;
            objectListEditor = new AttachObjectListEditor(serializedObject, "_objects", "_strategy");
            objectListEditor.OnEnable();
		}

		// void OnSceneGUI() {
        //     // objectListEditor.OnSceneGUI(attacher.objects, attacher);
		// 	serializedObject.ApplyModifiedProperties();
		// }

		public override void OnInspectorGUI() {
			serializedObject.Update();

            SerializedProperty prop;

            prop = serializedObject.FindProperty("_request"); 
            objectListEditor.OnInspectorGUI(prop, attacher.isActive);

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("attachOnStart"));

            EditorGUI.BeginChangeCheck();
            int executionOrder = EditorGUILayout.IntField("Execution Order", ((Attacher) target).executionOrder);
            if (EditorGUI.EndChangeCheck())
                ((Attacher) target).executionOrder = executionOrder;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));

			EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
			if(GUILayout.Button("Attach", GUILayout.Width(100)))
				attacher.Attach();
			if(GUILayout.Button("Detach", GUILayout.Width(100)))
				attacher.Detach();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

	}

}