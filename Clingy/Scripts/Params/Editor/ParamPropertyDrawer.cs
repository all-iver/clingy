namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(Param))]
    public class ParamPropertyDrawer : PropertyDrawer {

        public static float GetPropertyHeight() {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return GetPropertyHeight();
        }

        public override void OnGUI(Rect initialRect, SerializedProperty paramProp, GUIContent label) {
            SerializedProperty typeProp = paramProp.FindPropertyRelative("type");
            ParamType type = (ParamType) typeProp.intValue;
            SerializedProperty nameProp = paramProp.FindPropertyRelative("name");
            // string valuePropName;
            try {
                // valuePropName = Param.GetValuePropName(type);
            } catch (System.NotImplementedException) {
                type = ParamType.Vector3;
                typeProp.intValue = (int) type;
                // valuePropName = Param.GetValuePropName(type);
            }
            Rect rect = initialRect;
            float width = rect.width;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.width = 90;
            EditorGUI.BeginProperty(rect, GUIContent.none, typeProp);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, typeProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) {
                if (nameProp.stringValue == Param.defaultNameForType[(ParamType) type]
                        || nameProp.stringValue == "")
                    nameProp.stringValue = Param.defaultNameForType[(ParamType) typeProp.intValue];
            }
            EditorGUI.EndProperty();

            if (typeProp.hasMultipleDifferentValues)
                return;

            rect.x += 100;
            rect.width = initialRect.width - 140;
            EditorGUI.LabelField(rect, "Name");
            rect.x += 40;
            EditorGUI.PropertyField(rect, nameProp, GUIContent.none);

            rect = initialRect;
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
            DrawValueProp(rect, paramProp);
    	}

        public static void DrawValueProp(Rect rect, SerializedProperty paramProp, string label = "Value") {
            SerializedProperty typeProp = paramProp.FindPropertyRelative("type");
            ParamType type = (ParamType) typeProp.intValue;
            string valuePropName = Param.GetValuePropName(type);
            Rect initialRect = rect;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float labelWidth = 0;
            if (!string.IsNullOrEmpty(label)) {
                labelWidth = 46;
                rect.width = labelWidth;
                EditorGUI.LabelField(rect, label);
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x += labelWidth;
            }
            // if (Param.GetRelativityType(type) == ParamRelativityType.Normal) {
            //     rect.width = initialRect.width - labelWidth - 56;
            // } else if (Param.GetRelativityType(type) == ParamRelativityType.Blend) {
            //     rect.width = initialRect.width - labelWidth - 66;
            // } else {
                rect.width = initialRect.width - labelWidth;
            // }
            SerializedProperty valueProp = paramProp.FindPropertyRelative(valuePropName);
            EditorGUI.PropertyField(rect, valueProp, GUIContent.none);
            
            // if (Param.GetRelativityType(type) == ParamRelativityType.Normal) {
            //     rect.x = initialRect.xMax - 48;
            //     rect.width = 48;
            //     EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("relativeTo"), GUIContent.none);
            // } else if (Param.GetRelativityType(type) == ParamRelativityType.Blend) {
            //     rect.x = initialRect.xMax - 58;
            //     rect.width = 58;
            //     EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("blendMode"), GUIContent.none);
            // }                
            EditorGUI.indentLevel = indentLevel;
        }

    }

}