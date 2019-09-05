using TopDown;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopDownBlock))]
public class TopDownBlockEditor : Editor {
	
	public SerializedProperty
		topRender,
		frontRender,
//		bottomRender,
//		backRender,
		height;

	private const KeyCode UP = KeyCode.LeftBracket;
	private const KeyCode DOWN = KeyCode.RightBracket;
	
	private void OnEnable() {
		topRender = serializedObject.FindProperty("topRender");
		frontRender = serializedObject.FindProperty("frontRender");
//		bottomRender = serializedObject.FindProperty("bottomRender");
//		backRender = serializedObject.FindProperty("backRender");
		height = serializedObject.FindProperty("height");
	}
	
	public override void OnInspectorGUI() {
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();
		TopDownBlock blockScript = (TopDownBlock) target;
		
		blockScript.topRender = EditorGUILayout.ObjectField(blockScript.topRender, typeof(Sprite), false) as Sprite;
		blockScript.frontRender = EditorGUILayout.ObjectField(blockScript.frontRender, typeof(Sprite), false) as Sprite;
		blockScript.Height = EditorGUILayout.IntField ("Block height", blockScript.Height);
//		blockScript.bottomRender = EditorGUILayout.ObjectField(blockScript.bottomRender, typeof(Sprite), false) as Sprite;
//		blockScript.backRender = EditorGUILayout.ObjectField(blockScript.backRender, typeof(Sprite), false) as Sprite;
	}

	private void OnSceneGUI() {
		TopDownBlock blockScript = (TopDownBlock) target;
		Event e = Event.current;
		switch (e.type) {
			case EventType.KeyDown:
				// Change the blocks height based off of keyboard input (UP = '[' and DOWN = ']')
				switch (e.keyCode) {
					case UP:
						blockScript.Height++;
						break;
					case DOWN:
						blockScript.Height--;
						break;
				}
				break;
		}
	}
}
