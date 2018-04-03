namespace SubC.Attachments.ClingyEditor {

	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	[CustomEditor(typeof(AttachPoint), true)]
    [CanEditMultipleObjects]
	public class AttachPointEditor : Editor {
		ReorderableList paramsRL;
		// AttachPoint attachPoint;

		void OnSceneGUI() {
			// if (attachPoint)
			// 	ClingyAnchorListEditor.DoAnchorHandles(attachPoint.anchors, attachPoint, attachPoint.transform);
			// serializedObject.ApplyModifiedProperties();
		}

		void OnEnable() {
			// attachPoint = (AttachPoint) target;
			paramsRL = ParamListEditor.CreateReorderableList(serializedObject, 
                    serializedObject.FindProperty("baseParams._params"), "Params");
		}

        public virtual void DoInspectorGUI() {
            SerializedProperty p = serializedObject.GetIterator();
            if (p.NextVisible(true)) {
                if (!p.NextVisible(true)) // skip the "Script" property
                    return;
                do {
                    EditorGUILayout.PropertyField(p, true);
                } while(p.NextVisible(false));
            }
        }

        void DoParamsList() {
			SerializedProperty prop;
            prop = serializedObject.FindProperty("expandParamsInInspector");
            prop.boolValue = EditorGUILayout.Foldout(prop.boolValue, "Params");
            if (prop.boolValue)
    			paramsRL.DoLayoutList();
        }

		public override void OnInspectorGUI() {
			serializedObject.Update();

            // DoBaseInspectorGUI();
            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });
            // base.DrawDefaultInspector();
            // DoInspectorGUI();
            DoParamsList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));

			// prop = serializedObject.FindProperty("events");
			// EditorGUILayout.PropertyField(prop);

			// EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying || !spriteAttachPoint.isAttached);
            // GUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();
			// if(GUILayout.Button("Detach All", GUILayout.Width(200)))
			// 	spriteAttachPoint.DetachAll();
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
			// EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();
		}

	}

}