namespace SubC.Attachments.ClingyEditor {

    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    public class SpriteParamsWindow : EditorWindow {
        
        SpriteParams spriteParams;
        SerializedObject serializedObject;
        int frameToEdit = -1;
        ReorderableList paramsRL;
        Vector2 scrollPos, viewSize;

        [MenuItem ("Window/Clingy/Sprite Params Editor")]
        public static void Init() {
            SpriteParamsWindow window = (SpriteParamsWindow) 
                    EditorWindow.GetWindow(typeof(SpriteParamsWindow), false, "Clingy Sprites");
            window.Show();
            window.minSize = new Vector2(334, 149);
        }

        void OnEnable() {
            viewSize = new Vector2(position.width, position.height) * 1.5f;
            scrollPos = (viewSize - position.size) / 2;
            Undo.undoRedoPerformed += Repaint;
        }

        void OnDisable() {
            Undo.undoRedoPerformed -= Repaint;
        }

        void OnSelectionChange() {
            Repaint();
        }

        Vector2 GetSpriteCenter(Sprite sprite) {
            return viewSize / 2;
        }

        Vector2 GetSpritePos(Sprite sprite) {
            return viewSize / 2 - GetSpriteAspectSize(sprite) / 2;
        }

        float GetSpriteScale(Sprite sprite) {
            return GetSpriteAspectSize(sprite).x / sprite.rect.width;
        }

        Vector2 GetSpriteAspectSize(Sprite sprite) {
            return ClingyEditorUtils.GetSpriteAspectSize(sprite, new Vector2(position.width / 2, position.height / 2));
        }

        void SetPosition(ParamList paramList, Sprite sprite, Vector2 handlePos) {
            // int paramIndex = paramList.GetIndexOfParam(ParamType.Vector3);
            // if (paramIndex == -1)
            //     return;
            // Vector2 pivot = new Vector2(sprite.pivot.x, sprite.rect.height - sprite.pivot.y);
            // Vector2 position = (handlePos - GetSpritePos(sprite)) / GetSpriteScale(sprite) - pivot;
            // position.y = -position.y;
            // position /= sprite.pixelsPerUnit;
            // serializedObject.FindProperty("frames").GetArrayElementAtIndex(frameToEdit)
            //         .FindPropertyRelative("paramList._params").GetArrayElementAtIndex(paramIndex)
            //         .FindPropertyRelative("vector3Value").vector3Value = position;
        }

        void DoHandle(SpriteParams.SpriteFrame frame) {
            // ParamList paramList = frame.paramList;
            // if (!paramList.HasParam(ParamType.Vector3))
            //     return;

            // Sprite sprite = frame.sprite;
            // float scale = GetSpriteAspectSize(frame.sprite).x / frame.sprite.rect.width;
            // Vector2 position = SpriteParamsEditor.GetPositionParam(frame, scale, GetSpritePos(frame.sprite));
            // int controlId = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
            // GUIStyle handle = new GUIStyle("U2D.pivotDot");
            // Rect handleRect = new Rect(position.x - handle.fixedWidth / 2, position.y - handle.fixedHeight / 2, 
            //         handle.fixedWidth, handle.fixedHeight);
            
            // Event current = Event.current;
            // switch (Event.current.GetTypeForControl(controlId)) {
			// case EventType.MouseDown:
			// 	if (current.button == 0 && handleRect.Contains(Event.current.mousePosition) && !current.alt) {
			// 		GUIUtility.keyboardControl = controlId;
			// 		GUIUtility.hotControl = controlId;
			// 		current.Use();
			// 		EditorGUIUtility.SetWantsMouseJumping(1);
			// 	}
			// 	break;
			// case EventType.MouseUp:
			// 	if (GUIUtility.hotControl == controlId && (current.button == 0 || current.button == 2)) {
			// 		GUIUtility.hotControl = 0;
			// 		current.Use();
			// 		EditorGUIUtility.SetWantsMouseJumping(0);
			// 	}
			// 	break;
			// case EventType.MouseDrag:
			// 	if (GUIUtility.hotControl == controlId) {
            //         SetPosition(paramList, sprite, current.mousePosition);
			// 		current.Use();
			// 	}
			// 	break;
            // }

            // if (Event.current.type != EventType.Repaint)
            //     return;
            // handle.Draw(handleRect, GUIContent.none, controlId);
        }

        void DrawParamsWindow() {
            float w = 350;
            float h = 130 + Mathf.Max(serializedObject.FindProperty("frames").GetArrayElementAtIndex(frameToEdit)
                    .FindPropertyRelative("paramList._params").arraySize - 1, 0)
                        * ParamPropertyDrawer.GetPropertyHeight();
            float x = position.width - 24 - w;
            float y = position.height - 24 - h;
            bool wideMode = EditorGUIUtility.wideMode;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;
            EditorGUIUtility.wideMode = true;
            GUILayout.BeginArea(new Rect(x, y, w, h), "Clingy Sprite Params", new GUIStyle("window"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("previewSprite"));
            EditorGUILayout.Space();
            paramsRL.DoLayoutList();
            GUILayout.EndArea();
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.wideMode = wideMode;
        }

        void Reset() {
            spriteParams = null;
            serializedObject = null;
            frameToEdit = -1;
            paramsRL = null;
        }

        void OnGUI() {
            if (!Selection.activeObject || !(Selection.activeObject is SpriteParams)) {
                Reset();
                GUILayout.Label("No Clingy Sprite Params selected");
                return;
            }

            SpriteParams _spriteParams = (SpriteParams) Selection.activeObject;
            if (spriteParams != _spriteParams || serializedObject == null) {
                Reset();
                spriteParams = _spriteParams;
                serializedObject = new SerializedObject(spriteParams);
            }

            viewSize = new Vector2(position.width, position.height) * 1.5f;

            serializedObject.Update();

            SerializedProperty framesProp = serializedObject.FindProperty("frames");
            if (framesProp.arraySize == 0) {
                paramsRL = null;
                frameToEdit = -1;
                GUILayout.Label("Add some sprites in the inspector to get started!");
                return;
            }

            int _frameToEdit = serializedObject.FindProperty("frameToEdit").intValue;
            if (_frameToEdit >= framesProp.arraySize)
                _frameToEdit = 0;

            SerializedProperty frameProp = framesProp.GetArrayElementAtIndex(_frameToEdit);
            if (_frameToEdit != frameToEdit || paramsRL == null)
                paramsRL = ParamListEditor.CreateReorderableList(serializedObject, 
                        frameProp.FindPropertyRelative("paramList._params"), "Params");
            frameToEdit = _frameToEdit;
            SerializedProperty spriteProp = frameProp.FindPropertyRelative("sprite");
            Sprite sprite = (Sprite) spriteProp.objectReferenceValue;
            if (!sprite)
                return;

            GUI.Box(new Rect(0, 0, position.width - 14, position.height - 14), "", new GUIStyle("preBackground"));

            scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos,
                    new Rect(0, 0, viewSize.x, viewSize.y), true, true);
            SpriteParams.SpriteFrame frame = spriteParams.GetFrame(frameToEdit);
            SpriteParamsEditor.DrawSpriteAndPreview(frame, 
                    (Sprite) serializedObject.FindProperty("previewSprite").objectReferenceValue, 
                    GetSpriteAspectSize(frame.sprite), GetSpritePos(frame.sprite), clip: false);
            DoHandle(frame);
            GUI.EndScrollView();

            DrawParamsWindow();

            serializedObject.ApplyModifiedProperties();
        }
    }

}