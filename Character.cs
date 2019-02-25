using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleActions))]
public class Character : CharacterStats, IComparable {

	private BattleActions currentBattleActions;
	private int partySlot = 0;

	// Use this for initialization
	void Start () {
		this.currentBattleActions = GetComponent<BattleActions>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public BattleActions getCurrentBattleActions() {
		return this.currentBattleActions;
	}

	public void setPartySlot(int slot) {
		this.partySlot = slot;
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
			return this.currentSpd - otherCharacter.currentSpd;
		} else {
			throw new ArgumentException("Object is not a Character...");
		}
	}

	public bool canCharacterAct() {
		return this.canAct;
	}
		
	public int getPartySlot() {
		return this.partySlot;
	}
}
