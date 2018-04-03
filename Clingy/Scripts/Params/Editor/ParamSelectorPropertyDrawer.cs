namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ParamSelectorAttribute))]
    public class ParamSelectorPropertyDrawer : PropertyDrawer {

        AttachStrategy GetStrategy(SerializedProperty property) {
            // find the attach strategy via the asset database.  this gets the strategy even if the target object 
            // is a transitioner (because transitioners are sub-assets of strategies).  if the asset has just been 
            // created and the user has not finished editing the name, the strategy will be null.
            return AssetDatabase.LoadAssetAtPath<AttachStrategy>
                    (AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            ParamSelectorAttribute attr = (ParamSelectorAttribute) attribute;
            Rect initialRect = position;

            if (attr.providers == null || attr.providers.Length == 0)
                attr.GetProvidersFromStrategy(GetStrategy(property));
            else if (!attr.hasLabels)
                attr.BuildLabels(GetStrategy(property));
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginDisabledGroup(attr.providers.Length == 0);
            
            position = EditorGUI.PrefixLabel(position, label);
            float width = position.width / 2 - 2;
            Rect rect = new Rect(position.x, position.y, width, EditorGUIUtility.singleLineHeight);

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            ParamRelativityType relativityType 
                    = (ParamRelativityType) property.FindPropertyRelative("relativityType").enumValueIndex;
            ParamNormalRelativity relativeTo 
                    = (ParamNormalRelativity) property.FindPropertyRelative("relativeTo").enumValueIndex;

            // draw the provider picker
            SerializedProperty prop = property.FindPropertyRelative("provider");
            DrawProviderPicker(rect, prop, attr);

            // draw the name prop
            rect.x += width + 4;
            prop = property.FindPropertyRelative("defaultParam.name");
            EditorGUI.BeginChangeCheck();
            string newName = EditorGUI.DelayedTextField(rect, prop.stringValue);
            if (EditorGUI.EndChangeCheck())
                prop.stringValue = newName;

            EditorGUI.indentLevel = indentLevel + 1;
            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float yOff = lineHeight;

            // relativity
            if (relativityType != ParamRelativityType.None) {
                rect = initialRect;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += yOff;
                DrawRelativityProps(rect, property, attr);
                yOff += lineHeight;
            }

            // position options
            bool showPositionOptions = property.FindPropertyRelative("showPositionOptions").boolValue;
            if (showPositionOptions && relativityType == ParamRelativityType.Normal 
                    && relativeTo != ParamNormalRelativity.World) {
                rect = initialRect;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += yOff;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("useTransform"));            
                rect.y += lineHeight;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("useSpriteFlip"));
                yOff += lineHeight * 2;
            }

            // default value
            rect = initialRect;
            rect.y += yOff;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect = EditorGUI.PrefixLabel(rect, new GUIContent("Default"));
            ParamPropertyDrawer.DrawValueProp(rect, property.FindPropertyRelative("defaultParam"), "");

            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
    	}

        void DrawProviderPicker(Rect rect, SerializedProperty providerProp, ParamSelectorAttribute attr) {
            SerializedProperty prop = providerProp;
            try {
                attr.GetIndexOfProvider(prop.intValue);
            } catch {
                if (attr.providers != null)
                    prop.intValue = attr.providers[0];
            }
            if (attr.providers != null && attr.hasLabels) {
                EditorGUI.BeginChangeCheck();
                int newSelectedIndex = EditorGUI.Popup(rect, "", attr.GetIndexOfProvider(prop.intValue), attr.labels);
                if (EditorGUI.EndChangeCheck())
                    prop.intValue = attr.providers[newSelectedIndex];
            } else {
                EditorGUI.Popup(rect, "", 0, attr.labels);
            }
        }

        void DrawRelativityProps(Rect rect, SerializedProperty property, ParamSelectorAttribute attr) {
            ParamRelativityType relativityType 
                    = (ParamRelativityType) property.FindPropertyRelative("relativityType").enumValueIndex;
            if (relativityType == ParamRelativityType.Normal) {
                SerializedProperty prop = property.FindPropertyRelative("relativeTo");
                ParamNormalRelativity relativeTo = (ParamNormalRelativity) prop.enumValueIndex;
                if (relativeTo == ParamNormalRelativity.Object) {
                    // draw the label
                    rect = EditorGUI.PrefixLabel(rect, new GUIContent("Relative To"));
                    float width = rect.width / 2 - 2;

                    // draw the relative-to prop
                    rect = new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight);
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    EditorGUI.BeginChangeCheck();
                    ParamNormalRelativity newRelativeTo = (ParamNormalRelativity) EditorGUI.EnumPopup(rect, 
                            relativeTo);
                    if (EditorGUI.EndChangeCheck())
                        prop.enumValueIndex = (int) newRelativeTo;

                    // draw a provider picker
                    rect.x += width + 4;
                    prop = property.FindPropertyRelative("relativeToProvider");
                    DrawProviderPicker(rect, prop, attr);

                    EditorGUI.indentLevel = indentLevel;
                } else {
                    EditorGUI.PropertyField(rect, prop);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            int lines = 2;
            ParamRelativityType relativityType 
                    = (ParamRelativityType) property.FindPropertyRelative("relativityType").enumValueIndex;
            ParamNormalRelativity relativeTo 
                    = (ParamNormalRelativity) property.FindPropertyRelative("relativeTo").enumValueIndex;
            if (relativityType != ParamRelativityType.None)
                lines ++;
            bool showPositionOptions = property.FindPropertyRelative("showPositionOptions").boolValue;
            if (showPositionOptions && relativityType == ParamRelativityType.Normal 
                    && relativeTo != ParamNormalRelativity.World)
                lines += 2;
            return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        }

    }

}