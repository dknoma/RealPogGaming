using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : CharacterStats, IComparable {

	private Action currentAction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Action getCurrentAction() {
		return this.currentAction;
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

	/**
	 * This is the phase that occurs before the character is allowed to act. Checks any outstanding status
	 * afflictions or stat changes.
	 */ 
	public void statusPhase() {
		checkStatusAfflictions ();
	}
}
