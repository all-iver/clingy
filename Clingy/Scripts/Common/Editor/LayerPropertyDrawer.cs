namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerPropertyDrawer : PropertyDrawer {

        // note - for some reason LayerField does not seem to work with multi-object editing where the objects have
        // different values
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int layer = EditorGUI.LayerField(position, label, property.intValue);
            if (EditorGUI.EndChangeCheck())
                property.intValue = layer;
            EditorGUI.EndProperty();
    	}

    }

}