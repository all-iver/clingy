namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(AdoptOptionAttribute))]
    public class AdoptOptionPropertyDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty prop;

            EditorGUI.BeginProperty(position, label, property);

            // EditorGUI.BeginChangeCheck();
            prop = property.FindPropertyRelative("enabled");
            // EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, prop, new GUIContent(label.text.Replace(" Options", "")));
            // if (EditorGUI.EndChangeCheck())
            //     prop.enumValueIndex = (int) side;
            if (!prop.boolValue)
                return;

            EditorGUI.indentLevel ++;
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            prop = property.FindPropertyRelative("param");
            EditorGUI.PropertyField(position, prop);

            if (((AdoptOptionAttribute) attribute).keepUpdated) {
                position.y += EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;
                prop = property.FindPropertyRelative("keepUpdated");
                EditorGUI.PropertyField(position, prop);
            }
            EditorGUI.indentLevel --;

            EditorGUI.EndProperty();

            // EditorGUI.BeginChangeCheck();
            // prop = property.FindPropertyRelative("paramID");
            // EditorGUI.BeginChangeCheck();
            // ClingyPositionParamID paramID = (ClingyPositionParamID) EditorGUI.EnumPopup(right, 
            //         (ClingyPositionParamID) prop.enumValueIndex);
            // if (EditorGUI.EndChangeCheck())
            //     prop.enumValueIndex = (int) paramID;
    	}

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            if (!prop.FindPropertyRelative("enabled").boolValue)
                return EditorGUIUtility.singleLineHeight;
            SerializedProperty paramProp = prop.FindPropertyRelative("param");
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing 
                    + EditorGUI.GetPropertyHeight(paramProp);
            if (((AdoptOptionAttribute) attribute).keepUpdated)
                height += EditorGUIUtility.singleLineHeight;
            return height;
        }

    }

}