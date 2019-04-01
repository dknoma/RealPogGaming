using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BattleActions))]
public class Character : CharacterStats, IComparable {

//	public List<ScriptableObject> battleActionsList = new List<ScriptableObject>();
	public BattleAction basicAttackAction;
	public BattleAction supportAction;
//	public BattleAction specialAction;
	public BattleAction defendAction;
	public BattleAction escapeAction;
	
	private Dictionary<ActionType, ScriptableObject> battleActions = new Dictionary<ActionType, ScriptableObject>();
	
	private BattleAction currentBattleAction;
	private int partySlot;

	private CharacterEquipement equipement;
	private Weapon weapon;
	private Element attackElement;

	// Use this for initialization
	private void OnEnable () {
		currentBattleAction = basicAttackAction;
		equipement = gameObject.GetComponent<CharacterEquipement>();
		Debug.LogFormat("{0} weapon {1}", name, equipement.GetWeapon());
		SetWeapon(equipement.GetWeapon());
		basicAttackAction.SetWeapon(weapon);
//		WeaponValues weaponValues = GetComponentInChildren<WeaponValues> ();
//		if(weaponValues != null) {
//			currentBattleAction.SetWeapon(weaponValues);
////			currentBattleAction.SetWeaponType(weaponValues.weaponType);
////			currentBattleAction.SetElement (weaponValues.weaponElement);
//		}
//		battleActions[ActionType.Attack]
//		if (battleActionsList.Count > 0) {
//				
//		}
	}

	public void SetWeapon(Weapon newWeapon) {
		weapon = newWeapon;
		attackElement = weapon.GetWeaponElement();
		basicAttackAction.SetWeapon(weapon);
	}

	public Element GetAttackElement() {
		return attackElement;
	}

	public void SetPartySlot(int slot) {
		partySlot = slot;
	}

	/**
	 * Compare to method that helps sort units by speed.
	 */ 
	int IComparable.CompareTo(object obj) {
		if (obj == null) {
			return 1;
		} 
		Character otherCharacter = obj as Character;
		if(otherCharacter != null) {
			return currentSpd - otherCharacter.currentSpd;
		}
		throw new ArgumentException("Object is not a Character...");
	}

	public bool CanCharacterAct() {
		return canAct;
	}
		
	public int GetPartySlot() {
		return partySlot;
	}
}
