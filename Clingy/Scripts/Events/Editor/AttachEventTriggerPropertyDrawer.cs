namespace SubC.Attachments.ClingyEditor {

    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(AttachEventTrigger), true)]
    public class AttachEventTriggerPropertyDrawer : PropertyDrawer {

        protected GUIContent deleteButton = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
        protected float buttonWidth = 200;
        protected float buttonHeight = 20;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
            AttachEventTrigger eventTrigger = (AttachEventTrigger) 
                    fieldInfo.GetValue(prop.serializedObject.targetObject);
            SerializedProperty entriesProp = prop.FindPropertyRelative("entries");
            float y = position.y + EditorGUIUtility.standardVerticalSpacing;
            int indexToDelete = -1;
            for (int i = 0; i < entriesProp.arraySize; i++) {
                if (entriesProp.GetArrayElementAtIndex(i).FindPropertyRelative("hideInInspector").boolValue)
                    continue;
                SerializedProperty callback = entriesProp.GetArrayElementAtIndex(i).FindPropertyRelative("callback");
                SerializedProperty eventType = entriesProp.GetArrayElementAtIndex(i).FindPropertyRelative("eventType");
                float h = EditorGUI.GetPropertyHeight(callback);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), callback, 
                        new GUIContent(((AttachEventType) eventType.intValue).ToString()));
                Vector2 vector = GUIStyle.none.CalcSize(this.deleteButton);
                Rect delete = new Rect(position.xMax - vector.x - 8f, y + 1f, vector.x, vector.y);
				if (GUI.Button(delete, this.deleteButton, GUIStyle.none))
					indexToDelete = i;
                y += EditorGUI.GetPropertyHeight(callback) + EditorGUIUtility.standardVerticalSpacing;
            }
            if (indexToDelete != -1)
                entriesProp.DeleteArrayElementAtIndex(indexToDelete);
            if (GUI.Button(new Rect(position.center.x - buttonWidth / 2, y, buttonWidth, buttonHeight), 
                    "Add Event Listener"))
                ShowAddTriggerMenu(eventTrigger);
    	}

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            SerializedProperty entriesProp = prop.FindPropertyRelative("entries");
            float h = EditorGUIUtility.standardVerticalSpacing;
            for (int i = 0; i < entriesProp.arraySize; i++) {
                if (entriesProp.GetArrayElementAtIndex(i).FindPropertyRelative("hideInInspector").boolValue)
                    continue;
                SerializedProperty callback = entriesProp.GetArrayElementAtIndex(i).FindPropertyRelative("callback");
                h += EditorGUI.GetPropertyHeight(callback) + EditorGUIUtility.standardVerticalSpacing;
            }
            h += buttonHeight + EditorGUIUtility.standardVerticalSpacing;
            return h;
        }

		private void ShowAddTriggerMenu(AttachEventTrigger eventTrigger) {
			GenericMenu genericMenu = new GenericMenu();
			for (int i = 0; i < eventTrigger.supportedEventTypes.Length; i++) {
                AttachEventType eventType = eventTrigger.supportedEventTypes[i];
				if (!eventTrigger.HasVisibleEntryForEventType(eventTrigger.supportedEventTypes[i])) {
				    genericMenu.AddItem(new GUIContent(eventType.ToString()), false, 
                            new GenericMenu.MenuFunction2((object index) => { 
                                eventTrigger.GetOrCreateEvent(eventType, hideInInspector: false);
                            }), i);
				}
				else {
				    genericMenu.AddDisabledItem(new GUIContent(eventType.ToString()));
				}
			}
			genericMenu.ShowAsContext();
			Event.current.Use();
		}
    
    }

}