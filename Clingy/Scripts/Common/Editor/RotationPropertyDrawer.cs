namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(RotationAttribute))]
    public class RotationPropertyDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Quaternion rotation = property.quaternionValue;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            Vector3 eulers = EditorGUI.Vector3Field(position, label, rotation.eulerAngles);
            if (EditorGUI.EndChangeCheck())
                property.quaternionValue = Quaternion.Euler(eulers);
            EditorGUI.EndProperty();
    	}

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            if (!EditorGUIUtility.wideMode)
                return base.GetPropertyHeight(prop, label) + EditorGUIUtility.singleLineHeight;
            return base.GetPropertyHeight(prop, label);
        }

    }

}