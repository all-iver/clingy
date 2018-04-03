namespace SubC.Attachments.ClingyEditor {

    using UnityEditor;
    using UnityEngine;
    using System.Linq;
    using UnityEditorInternal;

    [CustomEditor(typeof(SpriteParams))]
    public class SpriteParamsEditor : Editor {

        ReorderableList framesRL;

        public static Vector2 GetPositionParam(SpriteParams.SpriteFrame frame, float scale, 
                Vector2 offset) {
            Vector2 position = Vector2.zero;
            if (frame.paramList.HasParam(ParamType.Vector3, "position"))
                position = frame.paramList.GetParam(ParamType.Vector3, "position").vector3Value 
                        * frame.sprite.pixelsPerUnit;
            position.y = -position.y;
            Vector2 pivot = new Vector2(frame.sprite.pivot.x, frame.sprite.rect.height - frame.sprite.pivot.y);
            Vector2 handlePos = offset + (pivot + position) * scale;
            return handlePos;
        }

        static void DrawPreviewSprite(SpriteParams.SpriteFrame frame, Sprite previewSprite, Vector2 size,
                Vector2 offset, bool clip = true) {
            float scale = size.x / frame.sprite.rect.width;
            Vector2 position = GetPositionParam(frame, scale, offset);
            // very simple clipping - would be pretty hard to do it right :/
            if (clip && ((position - offset).x > size.x || (position - offset).x < 0
                    || (position - offset).y > size.y || (position - offset).y < 0))
                return;
            Vector2 previewSize = ClingyEditorUtils.GetSpriteAspectSize(previewSprite, previewSprite.rect.size * scale) 
                    * (frame.sprite.pixelsPerUnit / previewSprite.pixelsPerUnit);
            Vector2 previewPivot = new Vector2(previewSprite.pivot.x, previewSprite.rect.height - previewSprite.pivot.y) 
                    * (frame.sprite.pixelsPerUnit / previewSprite.pixelsPerUnit);
            previewPivot *= scale;
            bool flipX = false;
            // if (frame.paramList.HasParam(ParamType.Bool))
            //     flipX = frame.paramList.GetParam(ParamType.Bool).boolValue;
            bool flipY = false;
            // if (frame.paramList.HasParam(ParamType.FlipY))
            //     flipY = frame.paramList.GetParam(ParamType.FlipY).boolValue;
            float angle = 0;
            // if (frame.paramList.HasParam(ParamType.Rotation))
            //     angle = frame.paramList.GetParam(ParamType.Rotation).quaternionValue.eulerAngles.z;
            ClingyEditorUtils.DrawTextureGUI(position - previewPivot, previewSprite, previewSize, angle, previewPivot, 
                    flipX, flipY);
        }

        public static void DrawSpriteAndPreview(SpriteParams.SpriteFrame frame, Sprite previewSprite, 
                Vector2 size, Vector2 offset, bool clip = true) {
            // assuming all params are local
            // todo - show a warning that results may vary from the displayed preview?
            int sortingOffset = 0;
            // if (frame.paramList.HasParam(ParamType.Integer))
            //     sortingOffset = frame.paramList.GetParam(ParamType.Integer).intValue;
            if (sortingOffset >= 0)
                ClingyEditorUtils.DrawTextureGUI(offset, frame.sprite, size);
            if (previewSprite != null)
                DrawPreviewSprite(frame, previewSprite, size, offset, clip: clip);
            if (sortingOffset < 0)
                ClingyEditorUtils.DrawTextureGUI(offset, frame.sprite, size);
        }

        // Vector2 GetAnchorPos(SerializedProperty spriteAnchorProp, Sprite sprite) {
        //     Vector2 position = spriteAnchorProp.FindPropertyRelative("anchor.position").vector3Value 
        //             * sprite.pixelsPerUnit;
        //     position.y = -position.y;
        //     Vector2 pivot = new Vector2(sprite.pivot.x, sprite.rect.height - sprite.pivot.y);
        //     Vector2 handlePos = GetSpritePos(sprite) + (pivot + position) * GetSpriteScale(sprite);
        //     return handlePos;
        // }

        // void DrawAnchor(SerializedProperty spriteAnchorProp, Sprite sprite, Vector2 pos) {
        //     Vector2 handlePos = GetAnchorPos(spriteAnchorProp, sprite);
        //     int controlId = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
        //     GUIStyle handle = new GUIStyle("U2D.pivotDot");
        //     if (handlePos.x < handle.fixedWidth / 2 || handlePos.x > spriteThumbnailSize.x - handle.fixedWidth / 2 
        //             || handlePos.y < handle.fixedHeight / 2 || handlePos.y > spriteThumbnailSize.x - handle.fixedHeight / 2)
        //         return;
        //     handlePos += pos;
        //     Rect handleRect = new Rect(handlePos.x - handle.fixedWidth / 2, handlePos.y - handle.fixedHeight / 2, 
        //             handle.fixedWidth, handle.fixedHeight);
        //     if (Event.current.type == EventType.Repaint)
        //         handle.Draw(handleRect, GUIContent.none, controlId);
        // }

        // void CopyAnchor(SerializedProperty spriteAnchorProp) {
        //     string s = string.Format("__ClingyAnchor|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", 
        //             spriteAnchorProp.FindPropertyRelative("anchor.position").vector3Value.x, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.position").vector3Value.y, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.position").vector3Value.z, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.rotation").quaternionValue.x, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.rotation").quaternionValue.y, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.rotation").quaternionValue.z, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.rotation").quaternionValue.w, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.sortingOffset").intValue, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.flipX").boolValue, 
        //             spriteAnchorProp.FindPropertyRelative("anchor.flipY").boolValue);
        //     EditorGUIUtility.systemCopyBuffer = s;
        // }

        // void PasteAnchor(SerializedProperty spriteAnchorProp) {
        //     if (!EditorGUIUtility.systemCopyBuffer.StartsWith("__ClingyAnchor"))
        //         return;
        //     string[] parts = EditorGUIUtility.systemCopyBuffer.Split('|');
        //     if (parts.Length != 11)
        //         return;
        //     Vector3 v = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
        //     spriteAnchorProp.FindPropertyRelative("anchor.position").vector3Value = v; 
        //     Quaternion q = new Quaternion(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]),
        //             float.Parse(parts[7]));
        //     spriteAnchorProp.FindPropertyRelative("anchor.rotation").quaternionValue = q; 
        //     spriteAnchorProp.FindPropertyRelative("anchor.sortingOffset").intValue = int.Parse(parts[8]); 
        //     spriteAnchorProp.FindPropertyRelative("anchor.flipX").boolValue = bool.Parse(parts[9]); 
        //     spriteAnchorProp.FindPropertyRelative("anchor.flipY").boolValue = bool.Parse(parts[10]);
        // }

        void OnEnable() {
            framesRL = new ReorderableList(serializedObject, serializedObject.FindProperty("frames"), true, 
                    true, false, true);
			framesRL.drawHeaderCallback = (Rect rect) => {  
    			EditorGUI.LabelField(rect, "Sprites");
			};
			framesRL.elementHeightCallback = (int index) => {
				if (serializedObject.FindProperty("frames").arraySize == 0)
					return 0;
                return 60;
            };
			framesRL.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                Vector2 thumbSize = new Vector2(rect.height - 15, rect.height - 15);
                SpriteParams spriteParams = (SpriteParams) target;
                SpriteParams.SpriteFrame frame = spriteParams.GetFrame(index);
                DrawSpriteAndPreview(frame, 
                        (Sprite) serializedObject.FindProperty("previewSprite").objectReferenceValue, 
                        ClingyEditorUtils.GetSpriteAspectSize(frame.sprite, thumbSize), rect.position);
                // DrawAnchor(framesProp, sprite, rect.position);                
                float btnWidth = (rect.width) / 3f - thumbSize.x + 20;
                if (GUI.Button(new Rect(rect.x + 60, rect.y + rect.height / 2 - 11, btnWidth, 20), "Edit")) {
                    serializedObject.FindProperty("frameToEdit").intValue = index;
                    ShowEditor();
                }
                // if (GUI.Button(new Rect(rect.x + 60 + btnWidth + 5, rect.y + rect.height / 2 - 11, btnWidth, 20), 
                //         "Copy")) {
                //     // CopyAnchor(framesProp);
                // }
                // if (GUI.Button(new Rect(rect.x + 60 + btnWidth * 2 + 10, rect.y + rect.height / 2 - 11, btnWidth, 20), 
                //         "Paste")) {
                //     // PasteAnchor(framesProp);
                // }
            };
        }

        void ShowEditor() {
            SpriteParamsWindow.Init();
            Selection.activeObject = target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SpriteParams spriteParams = (SpriteParams) target;
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            var style = new GUIStyle("box");
            if (EditorGUIUtility.isProSkin)
                style.normal.textColor = Color.white;
            // thanks to ProCamera2D for the idea - Unity does not provide any simple way to select multiple assets 
            // and add them to a reorderable list, but drag/drop works well
            GUI.Box(drop_area, "\nAdd sprites by dragging them here!", style);
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        foreach (Object obj in DragAndDrop.objectReferences) {
                            Sprite[] sprites = new Sprite[0];
                            if (obj is Texture2D) {
                                Texture2D tex = (Texture2D) obj;
                                string spriteSheet = AssetDatabase.GetAssetPath(tex);
                                sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
                            } else if (obj is Sprite) {
                                sprites = new Sprite[] { obj as Sprite };
                            } else if (obj is GameObject) {
                                GameObject go = (GameObject) obj;
                                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                                if (sr && sr.sprite)
                                    sprites = new Sprite[] { sr.sprite };
                            }
                            foreach (Sprite s in sprites)
                                spriteParams.AddFrame(s);
                        }
                        
                    }
                    break;
            }
            EditorGUILayout.Space();

            framesRL.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

    }

}