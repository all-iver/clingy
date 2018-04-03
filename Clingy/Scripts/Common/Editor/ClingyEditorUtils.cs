namespace SubC.Attachments.ClingyEditor {

	using UnityEditor;
	using UnityEngine;
    using System.Collections.Generic;

	public static class ClingyEditorUtils {

		// static GUIStyle boldFoldoutStyle = new GUIStyle(EditorStyles.foldout);

		public static float GetPropertyHeight(SerializedProperty prop, string[] props) {
			float y = 0;
			foreach (string s in props) {
				// int i = s.IndexOf("=");
				// if (i != -1)
				// 	y += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative(s.Substring(0, i))) 
				// 			+ EditorGUIUtility.standardVerticalSpacing;
				// else
                SerializedProperty p = prop.FindPropertyRelative(s);
                if (p == null) {
                    Debug.LogWarning("Property not found: " + s);
                    continue;
                }
				y += EditorGUI.GetPropertyHeight(p) + EditorGUIUtility.standardVerticalSpacing;
			}
			return y;
		}

		public static void DrawProperties(SerializedProperty prop, string[] props, Rect rect) {
			rect.height = EditorGUIUtility.singleLineHeight;
			foreach (string s in props) {
				// int i = s.IndexOf("=");
				// if (i != -1)
				// 	rect.y += ClingyEditorUtils.PropertyField(rect, prop.FindPropertyRelative(s.Substring(0, i)), 
				// 			label: s.Substring(i + 1, s.Length - i - 1));
				// else
                SerializedProperty p = prop.FindPropertyRelative(s);
                if (p == null) {
                    Debug.LogWarning("Property not found: " + s);
                    continue;
                }
				EditorGUI.PropertyField(rect, p, true);
                rect.y += EditorGUI.GetPropertyHeight(p) + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		// public static void DrawHeader(string label, bool space = true, bool bold = true) {
		// 	if (space)
		// 		EditorGUILayout.Space();
		// 	EditorGUILayout.LabelField(label, bold ? EditorStyles.boldLabel : EditorStyles.label);
		// }

		// public static float PropertyField(Rect rect, SerializedProperty prop, string label = null, 
		// 		float labelPercentage = 0) {
		// 	// thanks to http://answers.unity3d.com/questions/606325/how-do-i-implement-draggable-properties-with-custo.html
		// 	// lets you set the label width for a property while keeping the label dragging behavior
		// 	// and bold changed-from-prefab behavior
		// 	float oldLabelWidth = EditorGUIUtility.labelWidth;
		// 	EditorGUIUtility.labelWidth = rect.width * labelPercentage;
 		// 	EditorGUI.PropertyField(rect, prop, new GUIContent(label == null ? prop.displayName : label), true);
		// 	EditorGUIUtility.labelWidth = oldLabelWidth;
		// 	return EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;
		// }

		// public static float PropertyPair(Rect rect, SerializedProperty prop1, SerializedProperty prop2, 
		// 		float margin = 4, float labelPercentage = 0.6f, string label1 = null, string label2 = null) {
		// 	Rect rect1 = new Rect(), rect2 = new Rect();
		// 	rect1.width = rect.width / 2 - margin;
		// 	rect1.x = rect.x;
		// 	rect1.y = rect.y;
		// 	rect1.height = rect.height;
		// 	rect2.width = rect1.width;
		// 	rect2.x = rect.x + rect.width / 2 + margin / 2;
		// 	rect2.y = rect.y;
		// 	rect2.height = rect.height;
		// 	float h1 = PropertyField(rect1, prop1, label: label1, labelPercentage: labelPercentage);
		// 	float h2 = PropertyField(rect2, prop2, label: label1, labelPercentage: labelPercentage);
		// 	return Mathf.Max(h1, h2);
		// }

		// public static bool Foldout(bool isExpanded, string label, bool bold = false) {
		// 	boldFoldoutStyle.font = EditorStyles.boldFont;
		// 	if (bold)
		// 		isExpanded = EditorGUILayout.Foldout(isExpanded, label, boldFoldoutStyle);
		// 	else
		// 		isExpanded = EditorGUILayout.Foldout(isExpanded, label);
		// 	return isExpanded;
		// }

		// public static bool DrawSerializedClass(SerializedProperty prop, 
		// 		string header = null, bool foldout = true, bool bold = true, bool space = true) {
		// 	int oldIndentLevel = EditorGUI.indentLevel;
		// 	int depth = prop.depth;
		// 	if (space)
		// 		EditorGUILayout.Space();
		// 	bool isExpanded = true;
		// 	if (header == null)
		// 		header = prop.displayName;
		// 	if (foldout) {
		// 		prop.isExpanded = Foldout(prop.isExpanded, header, bold: bold);
		// 		isExpanded = prop.isExpanded;
		// 	} else {
		// 		if (!header.Equals(""))
		// 			DrawHeader(header, space: false, bold: bold);
		// 		depth += 1;
		// 	}			
		// 	if (isExpanded) {
		// 		foreach (SerializedProperty p in prop) {
		// 			EditorGUI.indentLevel = oldIndentLevel + p.depth - depth;
		// 			if (p.hasVisibleChildren) {
		// 				EditorGUILayout.PropertyField(p, true);
		// 				// EditorGUILayout.LabelField(p.displayName);
		// 			} else {
		// 				EditorGUILayout.PropertyField(p);
		// 			}
		// 		}
		// 	}
		// 	EditorGUI.indentLevel = oldIndentLevel;
		// 	return isExpanded;
		// }

        public static Vector2 GetSpriteAspectSize(Sprite sprite, Vector2 size) {
            float aspectX = size.y * (sprite.rect.width / sprite.rect.height);
            float aspectY = size.x / (sprite.rect.width / sprite.rect.height);
            if (aspectX > aspectY)
                size.y = aspectY;
            else if (aspectY > aspectX)
                size.x = aspectX;
            return size;
        }

        // thanks to http://answers.unity3d.com/questions/751203/how-to-best-render-a-sprite-in-an-editorwindow-and.html
        public static void DrawTextureGUI(Vector2 position, Sprite sprite, Vector2 size, float rotation = 0, 
                Vector2 rotationPivot = default(Vector2), bool flipX = false, bool flipY = false) {
            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                    sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            size = GetSpriteAspectSize(sprite, size);
            Matrix4x4 matrix = GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(flipX ? -1 : 1, flipY ? -1 : 1), 
                    new Vector2(position.x, position.y) + rotationPivot);
            GUIUtility.RotateAroundPivot(-rotation, new Vector2(position.x, position.y) + rotationPivot);
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, size.x, size.y), 
                    sprite.texture, spriteRect);
            GUI.matrix = matrix;
        }

        // thanks to http://answers.unity3d.com/questions/983125/c-using-reflection-to-automate-finding-classes.html
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType) {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }
 
	}

}