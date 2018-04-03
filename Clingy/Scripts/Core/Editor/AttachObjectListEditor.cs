namespace SubC.Attachments.ClingyEditor {

	using UnityEngine;
	using UnityEditor;
    using UnityEditorInternal;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;

	public class AttachObjectListEditor {

        SerializedObject serializedObject;
        ReorderableList objectsRL, paramsRL;
        int selectedObject = -1;
        string objectListPath, strategyPath;

        public AttachObjectListEditor(SerializedObject serializedObject, string objectListPath, string strategyPath) {
            this.serializedObject = serializedObject;
            this.objectListPath = objectListPath;
            this.strategyPath = strategyPath;
        }

        AttachObjectList objectList { 
            get {
                FieldInfo fieldInfo = serializedObject.targetObject.GetType().GetField(objectListPath, 
                        BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                    return null;
                return (AttachObjectList) fieldInfo.GetValue(serializedObject.targetObject);
            } 
        }

        AttachStrategy strategy { 
            get {
                if (string.IsNullOrEmpty(strategyPath))
                    return null;
                FieldInfo fieldInfo = serializedObject.targetObject.GetType().GetField(strategyPath, 
                        BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                    return null;
                return (AttachStrategy) fieldInfo.GetValue(serializedObject.targetObject);
            } 
        }

        List<AttachObject> objects { 
            get {
                FieldInfo fieldInfo = objectList.GetType().GetField("objects", 
                        BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                    return null;
                return (List<AttachObject>) fieldInfo.GetValue(objectList);
            } 
        }

		// public void OnSceneGUI(AttachRequest request, Object undoObject) {
		// 	// if (request.attach)
		// 	// 	ClingyAnchorListEditor.DoAnchorHandles(request.attachAnchors, undoObject, request.attach.transform);
		// 	// if (request.attachTo)
		// 	// 	ClingyAnchorListEditor.DoAnchorHandles(request.attachToAnchors, undoObject, request.attachTo.transform, 
        //     //             isReceiver: true);
		// }

        SerializedProperty objectListProp { get { return serializedObject.FindProperty(objectListPath); } }

        SerializedProperty objectsProp { get { return objectListProp.FindPropertyRelative("objects"); } } 

        AttachObject GetObjectByFlattenedIndex(int flattenedIndex) {
            int i = 0;
            foreach (AttachObject obj in objectList) {
                if (i == flattenedIndex)
                    return obj;
                i ++;
            }
            throw new System.IndexOutOfRangeException();
        }

        int GetCountForCategory(int category) {
            return objectList.Count(category);
            // int count = 0;
            // for (int i = 0; i < objectsProp.arraySize; i++)
            //     if (objectsProp.GetArrayElementAtIndex(i).FindPropertyRelative("_category").intValue == category)
            //         count ++;
            // return count;
        }

        bool CanAddForCategory(int category) {
            if (!strategy || category >= strategy.GetCategories().Length)
                return false;
            return strategy.GetCategories()[category].max < 0
                    || GetCountForCategory(category) < strategy.GetCategories()[category].max;
        }

        bool CanRemoveForCategory(int category) {
            if (!strategy || category >= strategy.GetCategories().Length)
                return true;
            return GetCountForCategory(category) > strategy.GetCategories()[category].min;
        }

        bool CanAddAttachObjects() {
            if (!strategy)
                return false;
            for (int c = 0; c < strategy.GetCategories().Length; c++) {
                if (CanAddForCategory(c))
                    return true;
            }
            return false;
        }

        bool CanRemoveSelectedAttachObject() {
            if (objectsRL.index < 0)
                return false;
            return true;
            // if (!attachStrategy)
            //     return true;
            // return CanRemoveForCategory(
            //         objectsProp.GetArrayElementAtIndex(objectsRL.index).FindPropertyRelative("category").intValue);
        }

        int GetCategory(int flattenedIndex) {
            return GetObjectByFlattenedIndex(flattenedIndex).category;
            // return objectsRL.serializedProperty.GetArrayElementAtIndex(flattenedIndex)
            //         .FindPropertyRelative("_category").intValue;
        }

        int GetIndexInCategory(int flattenedIndex, int category) {
            return objectList.GetIndexInCategory(objects[flattenedIndex]);
            // int count = 0;
            // for (int i = 0; i < objectsProp.arraySize; i++) {
            //     if (i == flattenedIndex)
            //         return count;
            //     if (GetCategory(i) == category)
            //         count ++;
            // }
            // throw new System.Exception("Flattened index is out of bounds.");
        }

        string GetLabelForAttachObject(int flattenedIndex) {
            int category = GetCategory(flattenedIndex);
            if (strategy) {
                if (category >= strategy.GetCategories().Length)
                    return "???";
                return strategy.GetCategories()[category].GetLabelForObject(
                        GetIndexInCategory(flattenedIndex, category));
            }
            return "#" + category + "." + GetIndexInCategory(flattenedIndex, category);
        }

        void EnsureCategoryMinimums() {
            if (!strategy)
                return;
            for (int c = 0; c < strategy.GetCategories().Length; c++) {
                int min = strategy.GetCategories()[c].min;
                int count = GetCountForCategory(c);
                while (count < min) {
                    objectList.Add(null, c);
                    count ++;
                }
            }
        }

		void ShowAddObjectMenu() {
			GenericMenu genericMenu = new GenericMenu();
            for (int c = 0; c < strategy.GetCategories().Length; c++) {
                AttachCategoryDefinition category = strategy.GetCategories()[c];
                // if (category.min == category.max)
                //     continue;
                if (CanAddForCategory(c)) {
                    genericMenu.AddItem(new GUIContent(category.objectLabel), false, 
                            new GenericMenu.MenuFunction2((object cat) => {
                                objectList.Add(null, (int) cat);
                                objectsRL.index = objectList.Count() - 1;
                            }), c);
                } else {
                    genericMenu.AddDisabledItem(new GUIContent(category.label));
                }
            }
			genericMenu.ShowAsContext();
			Event.current.Use();
		}

		public void OnEnable() {
			objectsRL = new ReorderableList(objects, typeof(AttachObject));
            // objectsRL = new ReorderableList(attachRequest.combinedObjects, 
            //         typeof(AttachRequestObject), false, true, true, true);
			objectsRL.drawHeaderCallback = (Rect rect) => {
    			EditorGUI.LabelField(rect, "Attach Objects");
			};
			objectsRL.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(rect, GetLabelForAttachObject(index), EditorStyles.boldLabel);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				// SerializedProperty objProp = objectsRL.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.BeginChangeCheck();
                AttachObject obj = GetObjectByFlattenedIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                GameObject seedObject = (GameObject) EditorGUI.ObjectField(rect, "Game Object", obj.seedObject, 
                        typeof(GameObject), true);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                string keyPattern = EditorGUI.DelayedTextField(rect, "Key Pattern", obj.keyPattern);
                if (EditorGUI.EndChangeCheck()) {
                    objectList.Replace(obj, 
                            new AttachObject(seedObject, obj.category, keyPattern, obj.defaultParams.Clone()));
                }
            };
			objectsRL.onAddCallback = (ReorderableList rl) => {
                ShowAddObjectMenu();
			};
            objectsRL.onRemoveCallback = (ReorderableList rl) => {
                objectList.Remove(GetCategory(rl.index), GetIndexInCategory(rl.index, GetCategory(rl.index)));
                if (rl.index == objectList.Count())
                    rl.index --;
                // paramsRL = null;
                // selectedObject = -1;
                // rl.index = -1;
            };
            objectsRL.onReorderCallback = (ReorderableList rl) => {
                objectList.HandleFlattenedReorder(selectedObject, rl.index);
            };
			objectsRL.elementHeightCallback = (int index) => {
                return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 4;
				// SerializedProperty objProp = objectsRL.serializedProperty.GetArrayElementAtIndex(index);
				// return ClingyEditorUtils.GetPropHeight(objProp, drawProps) + EditorGUIUtility.singleLineHeight
                //         + EditorGUIUtility.standardVerticalSpacing * 3;
			};
		}

        void DoAttachDropArea(int category, string categoryLabel) {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            var style = new GUIStyle("box");
            if (EditorGUIUtility.isProSkin)
                style.normal.textColor = Color.white;
            // thanks to ProCamera2D for the idea - Unity does not provide any simple way to select multiple assets 
            // and add them to a reorderable list, but drag/drop works well
            GUI.Box(drop_area, "\nDrag " + categoryLabel + " here", style);
            if ((evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated) 
                    && drop_area.Contains(evt.mousePosition)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    foreach (Object obj in DragAndDrop.objectReferences) {
                        if (!(obj is GameObject))
                            continue;
                        objectList.Add((GameObject) obj, category);
                    }
                    
                }
            }
            EditorGUILayout.Space();
        }

        void ResetParamsRL() {
            paramsRL = new ReorderableList(new string[] {}, typeof(string), false, true, false, false);
            paramsRL.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, "Default Params (select an object above)");
            };
        }

		public void OnInspectorGUI(SerializedProperty requestProp, bool disableAddButton) {
			// if (strategy == null)
			// 	EditorGUILayout.HelpBox("An Attach Strategy describes how objects will attach. You can create one "
            //             + "from the menu in Assets->Create->Clingy.", MessageType.Warning);
            // EditorGUI.BeginChangeCheck();
			// AttachStrategy newStrategy = (AttachStrategy) EditorGUILayout.ObjectField("Attach Strategy", 
            //         strategy, typeof(AttachStrategy), false);
            // if (EditorGUI.EndChangeCheck()) {
            //     strategy = newStrategy;
            //     if (showAttachObjects)
            //         EnsureCategoryMinimums();
            // }

			// SerializedProperty prop = objectsProp;
            objectsRL.displayAdd = !disableAddButton && CanAddAttachObjects();
            objectsRL.displayRemove = CanRemoveSelectedAttachObject();
            EditorGUILayout.Space();
            if (strategy) {
                for (int c = 0; c < strategy.GetCategories().Length; c++) {
                    AttachCategoryDefinition category = strategy.GetCategories()[c];
                    if (category.max < 0 || GetCountForCategory(c) < category.max)
                        DoAttachDropArea(c, category.label);
                }
            }
            objectsRL.DoLayoutList();

            if (selectedObject != objectsRL.index) {
                selectedObject = objectsRL.index;
                if (selectedObject != -1)
                    paramsRL = ParamListEditor.CreateReorderableList(serializedObject, 
                            objectsProp.GetArrayElementAtIndex(selectedObject)
                            .FindPropertyRelative("_defaultParams._params"), 
                            "Default Params for " + GetLabelForAttachObject(selectedObject));
                else
                    paramsRL = null;
            }

            if (paramsRL == null)
                ResetParamsRL();

            if (CanAddAttachObjects() || CanRemoveSelectedAttachObject())
                EditorGUILayout.Space();
            paramsRL.DoLayoutList();
		}

	}

}