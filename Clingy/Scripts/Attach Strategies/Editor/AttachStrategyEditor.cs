namespace SubC.Attachments.ClingyEditor {

    using UnityEngine;
	using UnityEditor;
    using UnityEditorInternal;
    using System.Reflection;

	[CustomEditor(typeof(AttachStrategy), true)]
	public class AttachStrategyEditor : Editor {

        ReorderableList paramsRL;
        string[] categoryNames;
        Texture2D headerTexture;
        GUIStyle headerStyle;
        SerializedProperty selectedCategoryProp;

        protected virtual void DoInspectorGUI() {
            bool foundFirstProp = false;
            bool foundProps = false;
            SerializedProperty p = serializedObject.GetIterator();
            if (p.NextVisible(true)) {
                if (!p.NextVisible(true)) // skip the "Script" property
                    return;
                EditorGUILayout.Space();
                do {
                    if (p.name.Equals("transitioners")) {
                        foundFirstProp = true;
                    } else if (foundFirstProp) {
                        EditorGUILayout.PropertyField(p, true);
                        foundProps = true;
                    }
                } while(p.NextVisible(false));
            }
            if (foundProps)
                EditorGUILayout.Space();
        }

        bool DoBaseInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            DoInspectorGUI();
            bool changed = EditorGUI.EndChangeCheck() && EditorApplication.isPlaying;
            
            // SerializedProperty prop = serializedObject.FindProperty("reattachWhenParamsUpdated");
            // EditorGUILayout.PropertyField(prop);
            // EditorGUILayout.Space();

            GUILayout.BeginVertical(GUI.skin.box);
            int oldSelected = selectedCategoryProp.intValue;
            GUILayout.BeginVertical(headerStyle);
            selectedCategoryProp.intValue = EditorGUILayout.Popup("Transitioner for:", selectedCategoryProp.intValue, 
                    categoryNames);
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            DoTransitioner();
            GUILayout.EndVertical();

            return changed;
        }

        void DoTransitioner() {
            SerializedProperty prop = serializedObject.FindProperty("transitioners");
            if (prop.arraySize > selectedCategoryProp.intValue)
                prop = prop.GetArrayElementAtIndex(selectedCategoryProp.intValue);
            else
                prop = null;
            if (prop != null && prop.objectReferenceValue) {
                SerializedObject obj = new SerializedObject(prop.objectReferenceValue);
                obj.Update();
                string displayName = "Unknown Transitioner";
                if (obj.targetObject.GetType().GetField("displayName") != null)
                    displayName = (string) obj.targetObject.GetType().GetField("displayName").GetValue(null);
                EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel);
                DoChangeTransitionerButton();
                // EditorGUI.indentLevel ++;
                string editorClass = "SubC.Attachments.ClingyEditor.TransitionerEditor";
                MethodInfo methodInfo = obj.targetObject.GetType().GetMethod("GetEditorClass",
                        BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                if (methodInfo != null)
                    editorClass = (string) methodInfo.Invoke(null, null);
                System.Type t = System.Type.GetType(editorClass);
                methodInfo = t.GetMethod("DoInspectorGUI");
                if (methodInfo != null) {
                    EditorGUILayout.Space();
                    methodInfo.Invoke(null, new object[] { obj, (AttachStrategy) target });
                }
                // EditorGUI.indentLevel --;
                obj.ApplyModifiedProperties();
            } else {
                EditorGUILayout.LabelField("No Transitioner", EditorStyles.boldLabel);
                DoChangeTransitionerButton();
            }
        }

        protected virtual void OnEnable() {
            selectedCategoryProp = serializedObject.FindProperty("_selectedCategory");

            headerTexture = new Texture2D(1, 1);
            headerTexture.SetPixel(0, 0, new Color(0.6f, 0.6f, 0.6f));
            headerTexture.Apply();
            headerStyle = new GUIStyle();
            headerStyle.normal.background = headerTexture;

            AttachStrategy strategy = (AttachStrategy) target;
            categoryNames = new string[strategy.GetCategories().Length];
            for (int i = 0; i < strategy.GetCategories().Length; i++)
                categoryNames[i] = strategy.GetCategories()[i].label;
        }

        void DoChangeTransitionerButton() {
            if (GUILayout.Button("Change Transitioner")) {
                System.Type[] transitionerTypes = System.AppDomain.CurrentDomain.GetAllDerivedTypes(
                        typeof(Transitioner));
    			GenericMenu genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("None"), false, new GenericMenu.MenuFunction(() => {
                    SetTransitioner(selectedCategoryProp.intValue, null);
                }));
                foreach (System.Type t in transitionerTypes) {
                    MethodInfo methodInfo = t.GetMethod("SupportsAttachStrategyType",
                            BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo != null)
                        if (!(bool) methodInfo.Invoke(null, new object[] { serializedObject.targetObject.GetType() }))
                            continue;

                    string displayName = t.FullName;
                    if (t.GetField("displayName") != null)
                        displayName = (string) t.GetField("displayName").GetValue(null);
                    genericMenu.AddItem(new GUIContent(displayName), false, new GenericMenu.MenuFunction(() => {
                        SetTransitioner(selectedCategoryProp.intValue, t);
                    }));
                }
                genericMenu.ShowAsContext();
            }
        }

		public override void OnInspectorGUI() {
			serializedObject.Update();
            bool changed = DoBaseInspectorGUI();
			serializedObject.ApplyModifiedProperties();
            if (changed) {
                foreach (Attachment a in ClingyComponent.instance.attachments.Values) {
                    if (a.strategy == target)
                        ((AttachStrategy) target).UpdateForEditorChanges(a);
                }
            }
		}

        void DeleteChildAssetsWithName(string name) {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(
                    AssetDatabase.GetAssetPath(serializedObject.targetObject));
            foreach (Object a in assets) {
                if (a != null && a.name.Equals(name))
                    Object.DestroyImmediate(a, true);
            }
            AssetDatabase.SaveAssets();
        }

        Transitioner SetTransitioner(int index, System.Type transitionerType, string assetName, 
                string propName) {
            DeleteChildAssetsWithName(assetName);
            if (transitionerType == null)
                return null;
            Transitioner newChild = (Transitioner) CreateInstance(transitionerType.ToString());
            newChild.Reset((AttachStrategy) target);
            newChild.hideFlags = HideFlags.HideInHierarchy;
            newChild.name = assetName;
            AssetDatabase.AddObjectToAsset(newChild, serializedObject.targetObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SerializedProperty prop = serializedObject.FindProperty(propName);
            if (prop.arraySize < index + 1)
                prop.arraySize = index + 1;
            prop.GetArrayElementAtIndex(index).objectReferenceValue = newChild;
			serializedObject.ApplyModifiedProperties();
            return newChild;
        }

        Transitioner SetTransitioner(int index, System.Type transitionerType) {
            return SetTransitioner(index, transitionerType, "Transitioner" + index, "transitioners");
        }

	}

}