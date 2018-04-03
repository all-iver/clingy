namespace SubC.Attachments.ClingyEditor {

	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(AttacherGroup))]
	public class AttacherGroupEditor : Editor {
		AttacherGroup attacherGroup;

		void OnEnable() {
			attacherGroup = (AttacherGroup) target;
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
			if(GUILayout.Button("Attach All", GUILayout.Width(200)))
				attacherGroup.Attach();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
			if(GUILayout.Button("Detach All", GUILayout.Width(200)))
				attacherGroup.Detach();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

	}

}