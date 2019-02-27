using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon))]
[CanEditMultipleObjects]
public class WeaponEditor : Editor {

	public SerializedProperty
	weaponElement,
	handType,
	weaponType,
	weaponSlot,
	atk;

//	private bool canChooseSlotType = true;

	void OnEnable() {
		weaponElement = serializedObject.FindProperty("weaponElement");
		handType = serializedObject.FindProperty("handType");
		weaponType = serializedObject.FindProperty("weaponType");
		weaponSlot = serializedObject.FindProperty("weaponSlot");
		atk = serializedObject.FindProperty ("atk");
	}

	public override void OnInspectorGUI() {
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		Weapon weapon = target as Weapon;
		// Get the rune type enum dropdown & update value and display value based on choice
		weapon.weaponElement = (ElementalAffinity.Element)EditorGUILayout.EnumPopup ("Weapon Element", weapon.weaponElement);
		weapon.weaponType = (Weapon.WeaponType)EditorGUILayout.EnumPopup ("Weapon Type", weapon.weaponType);
		weapon.handType = (Weapon.HandType)EditorGUILayout.EnumPopup ("Hand Type", weapon.handType);

		switch(weapon.handType) {
//		case Weapon.HandType.OneHanded:
//			this.canChooseSlotType = true;
//			break;
		case Weapon.HandType.TwoHanded:
			// If two-handed, can only use the main hand
			weapon.weaponSlot = Weapon.WeaponSlot.MainHand;
			break;
		}
		// Disale the selection if condition has been met, else able to edit selection
		using(new EditorGUI.DisabledScope(weapon.handType == Weapon.HandType.TwoHanded)) {
			weapon.weaponSlot = (Weapon.WeaponSlot)EditorGUILayout.EnumPopup ("Weapon Slot", weapon.weaponSlot);
		}
		weapon.atk = (int)EditorGUILayout.IntField ("Attack", weapon.atk);
	}
}
