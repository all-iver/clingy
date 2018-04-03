namespace SubC.Attachments.ClingyEditor {

    using UnityEngine;
	using UnityEditor;
    
	public class FlexibleTransitionerEditor : TransitionerEditor {

        static void DoTweenOptions(SerializedProperty optionsProp) {
            SerializedProperty tweenMethodProp = optionsProp.FindPropertyRelative("tweenMethod");
            EditorGUILayout.PropertyField(tweenMethodProp);
            if (((TweenMethod) tweenMethodProp.intValue) == TweenMethod.Speed) {
                EditorGUI.indentLevel ++;
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("speed"));
                EditorGUI.indentLevel --;
            } else if (((TweenMethod) tweenMethodProp.intValue) == TweenMethod.Time) {
                EditorGUI.indentLevel ++;
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("duration"));
                EditorGUI.indentLevel --;
            }
            if (((TweenMethod) tweenMethodProp.intValue) != TweenMethod.None) {
                EditorGUI.indentLevel ++;
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("easing"));
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("delay"));
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("completeOnCancel"));
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("dynamicTarget"));
                EditorGUI.indentLevel --;
            }
        }

        static void DoOnAttachOptions(SerializedProperty optionsProp) {
            EditorGUILayout.LabelField("On Attach Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rigidbodyBehavior"));
            SerializedProperty prop = optionsProp.FindPropertyRelative("positionOptions.behavior");
            EditorGUILayout.PropertyField(prop, new GUIContent("Position behavior"));
            if (((PositionBehavior) prop.intValue) == PositionBehavior.Snap) {
                EditorGUI.indentLevel ++;
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("positionOptions.anchor1Param"));
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("positionOptions.anchor2Param"));
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("positionOptions.moveMethod"));
                prop = optionsProp.FindPropertyRelative("positionOptions.tweenOptions");
                DoTweenOptions(prop);
                EditorGUI.indentLevel --;
            }
            prop = optionsProp.FindPropertyRelative("rotationOptions.behavior");
            EditorGUILayout.PropertyField(prop, new GUIContent("Rotation behavior"));
            if (((RotationBehavior) prop.intValue) != RotationBehavior.DoNothing) {
                EditorGUI.indentLevel ++;
                if (((RotationBehavior) prop.intValue) == RotationBehavior.Snap) {
                    EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotationOptions.rotationParam"));
                } else if (((RotationBehavior) prop.intValue) == RotationBehavior.LookAt) {
                    EditorGUILayout.PropertyField(
                            optionsProp.FindPropertyRelative("rotationOptions.lookAtPositionParam"));
                    EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotationOptions.upParam"));
                } else if (((RotationBehavior) prop.intValue) == RotationBehavior.LookAt2D) {
                    EditorGUILayout.PropertyField(
                            optionsProp.FindPropertyRelative("rotationOptions.lookAtPositionParam"));
                } else if (((RotationBehavior) prop.intValue) == RotationBehavior.LookDirection) {
                    EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotationOptions.forwardParam"));
                    EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotationOptions.upParam"));
                }
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotationOptions.offsetParam"));
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotationOptions.rotateMethod"));
                prop = optionsProp.FindPropertyRelative("rotationOptions.tweenOptions");
                DoTweenOptions(prop);
                EditorGUI.indentLevel --;
            }
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("adoptSortingOrderOptions"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("adoptFlipXOptions"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("adoptFlipYOptions"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("adoptLayerOptions"));
        }

        static void DoOnDetachOptions(SerializedProperty optionsProp) {
            EditorGUILayout.LabelField("On Detach Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rigidbodyBehavior"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restorePosition"));
            if (optionsProp.FindPropertyRelative("restorePosition").boolValue) {
                EditorGUI.indentLevel ++;
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("moveMethod"));
                DoTweenOptions(optionsProp.FindPropertyRelative("tweenPositionOptions"));
                EditorGUI.indentLevel --;
            }
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restoreRotation"));
            if (optionsProp.FindPropertyRelative("restoreRotation").boolValue) {
                EditorGUI.indentLevel ++;
                EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("rotateMethod"));
                DoTweenOptions(optionsProp.FindPropertyRelative("tweenRotationOptions"));
                EditorGUI.indentLevel --;
            }
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restoreScale"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restoreLayer"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restoreSortingOrder"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restoreFlipX"));
            EditorGUILayout.PropertyField(optionsProp.FindPropertyRelative("restoreFlipY"));
        }

		new public static void DoInspectorGUI(SerializedObject obj, AttachStrategy attachStrategy) {
            DoOnAttachOptions(obj.FindProperty("onAttachOptions"));
            EditorGUILayout.Space();
            DoOnDetachOptions(obj.FindProperty("onDetachOptions"));
		}

	}

}