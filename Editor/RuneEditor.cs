using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rune))]
[CanEditMultipleObjects]
public class RuneEditor : Editor {

	public SerializedProperty
		runeType,
		statType,
		whichStat,
		statCount,
		statPercent;

	private bool showStats = false;

	void OnEnable() {
		runeType = serializedObject.FindProperty("runeType");
		statType = serializedObject.FindProperty("statType");
		whichStat = serializedObject.FindProperty("whichStat");
		statCount = serializedObject.FindProperty("statCount");
		statPercent = serializedObject.FindProperty("statPercent");
	}

	public override void OnInspectorGUI() {
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		Rune rune = target as Rune;
		// Get the rune type enum dropdown & update value and display value based on choice
		rune.runeType = (Rune.Type)EditorGUILayout.EnumPopup ("Rune Type", rune.runeType);

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

		// If rune is a stat rune, show all the stat options
		if(this.showStats) {
			rune.statType = (Rune.StatType)EditorGUILayout.EnumPopup (rune.statType);
			rune.whichStat = (Rune.Stat)EditorGUILayout.EnumPopup (rune.whichStat);
			// Check what kind of stat modifier is used: flat vs. percentage
			switch(rune.statType) {
			case Rune.StatType.Flat:
				rune.statCount = EditorGUILayout.IntField ("Stat Count", rune.statCount);
				break;
			case Rune.StatType.Percent:
				rune.statPercent = EditorGUILayout.IntField ("Stat Percentage", rune.statPercent);
				break;
			}
		}
	}
}
