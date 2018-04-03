namespace SubC.Attachments.ClingyEditor {

    using UnityEngine;
    using UnityEditor;

    public class TransitionerEditor {

        public static void DoInspectorGUI(SerializedObject obj, AttachStrategy attachStrategy) {
            SerializedProperty p = obj.GetIterator();
            if (p.NextVisible(true)) {
                if (!p.NextVisible(true)) // skip the "Script" property
                    return;
                EditorGUILayout.Space();
                do {
                    EditorGUILayout.PropertyField(p, true);
                } while(p.NextVisible(false));
            }
        }

    }

}