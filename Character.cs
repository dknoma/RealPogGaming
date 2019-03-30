using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleActions))]
public class Character : CharacterStats, IComparable {

//	public List<ScriptableObject> battleActionsList = new List<ScriptableObject>();
	public BattleAction basicAttackAction;
	public BattleAction supportAction;
//	public BattleAction specialAction;
	public BattleAction defendAction;
	public BattleAction escapeAction;
	
	private Dictionary<ActionType, ScriptableObject> battleActions = new Dictionary<ActionType, ScriptableObject>();
	
	private BattleActions currentBattleActions;
	private int partySlot;

	// Use this for initialization
	private void Start () {
		currentBattleActions = GetComponent<BattleActions>();
		Weapon weapon = GetComponentInChildren<Weapon> ();
		if(weapon != null) {
			currentBattleActions.setElement (weapon.weaponElement);
		}
//		battleActions[ActionType.Attack]
//		if (battleActionsList.Count > 0) {
//				
//		}
	}

	public BattleActions GetCurrentBattleActions() {
		return currentBattleActions;
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
