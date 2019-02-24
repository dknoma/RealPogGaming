using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleActions))]
public class Character : CharacterStats, IComparable {

	private BattleActions currentBattleActions;

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

	/**
	 * Compare to method that helps sort units by speed.
	 */ 
	public int CompareTo(object obj) {
		if (obj == null) {
			return 1;
		} 
		Character otherCharacter = obj as Character;
		if(otherCharacter != null) {
			return this.currentSpd.CompareTo(otherCharacter.currentSpd);
		} else {
			throw new ArgumentException("Object is not a Character...");
		}
	}

	public bool canCharacterAct() {
		return this.canAct;
	}
}
