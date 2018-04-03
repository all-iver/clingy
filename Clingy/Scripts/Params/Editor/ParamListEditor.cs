namespace SubC.Attachments.ClingyEditor {

    using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

    public class ParamListEditor {

        public static ReorderableList CreateReorderableList(SerializedObject serializedObject, SerializedProperty prop, 
                string header) {
			ReorderableList rl = new ReorderableList(serializedObject, prop, true, true, true, true);
            rl.elementHeightCallback = (int index) => {
                return EditorGUI.GetPropertyHeight(prop.GetArrayElementAtIndex(index));
            };
			rl.drawHeaderCallback = (Rect rect) => {  
    			EditorGUI.LabelField(rect, header);
			};
			rl.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				SerializedProperty paramProp = prop.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, paramProp, true);
			};
			rl.onAddCallback = (ReorderableList _rl) => {
                if (_rl.serializedProperty.hasMultipleDifferentValues) {
                    if (!EditorUtility.DisplayDialog("Adding an array element will copy the complete array to all other selected objects.", 
                            "Unique values in the different selected objects will be lost", "Add", "Cancel"))
                        return;
                }
				int index = _rl.serializedProperty.arraySize;
				_rl.serializedProperty.arraySize ++;
				_rl.index = index;
				SerializedProperty paramProp = _rl.serializedProperty.GetArrayElementAtIndex(index);
                paramProp.FindPropertyRelative("type").intValue = (int) ParamType.Vector3;
                paramProp.FindPropertyRelative("name").stringValue = Param.defaultNameForType[ParamType.Vector3];
                // paramProp.FindPropertyRelative("relativeTo").enumValueIndex = (int) ParamRelativeTo.Local;
				paramProp.FindPropertyRelative("quaternionValue").quaternionValue = Quaternion.identity;
				paramProp.FindPropertyRelative("colorValue").colorValue = Color.white;
				paramProp.serializedObject.ApplyModifiedProperties();
			};
			return rl;
		}

    }
}