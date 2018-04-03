namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(AngleLimits))]
    public class AngleLimitsPropertyDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30;

            Rect leftRect = new Rect(position.x, position.y, position.width, position.height);
            // Rect leftRect = new Rect(position.x, position.y, position.width - 105, position.height);
            float width = leftRect.width / 2 - 1;
            Rect rect = new Rect(leftRect.x, leftRect.y, width, leftRect.height);

            SerializedProperty prop = property.FindPropertyRelative("min");
            EditorGUI.BeginChangeCheck();
            float newMin = EditorGUI.FloatField(rect, "Min", prop.floatValue);
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newMin;

            rect.x += width + 2;
            prop = property.FindPropertyRelative("max");
            EditorGUI.BeginChangeCheck();
            float newMax = EditorGUI.FloatField(rect, "Max", prop.floatValue);
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newMax;

            // Rect rightRect = new Rect(leftRect.x + leftRect.width + 8, leftRect.y, 95, leftRect.height);
            // prop = property.FindPropertyRelative("relativeTo");
            // EditorGUI.PropertyField(rightRect, prop, GUIContent.none);

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
    	}

    }

}