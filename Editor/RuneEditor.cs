using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rune))]
[CanEditMultipleObjects]
public class RuneEditor : Editor {

//	public SerializedProperty runeType;
	public SerializedProperty
		runeType,
		statType,
		whichStat;

	private bool showStats = false;

	void OnEnable() {
		runeType = serializedObject.FindProperty("runeType");
		statType = serializedObject.FindProperty("statType");
		whichStat = serializedObject.FindProperty("whichStat");
	}

	public override void OnInspectorGUI() {
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();
		// Get the rune type enum dropdown
//		EditorGUILayout.PropertyField (runeType);
		Rune rune = target as Rune;
		rune.runeType = (Rune.Type)EditorGUILayout.EnumPopup ("Rune Type", rune.runeType);	// Update the value of the dropdown based on enum index
//		rune.statType = (Rune.StatType)statType.enumValueIndex;
//		rune.whichStat = (Rune.Stat)whichStat.enumValueIndex;
//		rune.runeType = (Rune.Type)runeType.enumValueIndex;	// Update the value of the dropdown based on enum index
//		rune.statType = (Rune.StatType)statType.enumValueIndex;
//		rune.whichStat = (Rune.Stat)whichStat.enumValueIndex;

		// Check the rune type. Show/hide options depending on the chosen enum
		switch(rune.runeType) {
		case Rune.Type.Skill:
			// TODO: make a skill popup that refers to skill scripts?
			this.showStats = false;
			break;
		case Rune.Type.Stat:
			this.showStats = true;
			break;
		}

		if(this.showStats) {
			rune.statType = (Rune.StatType)EditorGUILayout.EnumPopup (rune.statType);
			rune.whichStat = (Rune.Stat)EditorGUILayout.EnumPopup (rune.whichStat);
		}
	}
}
