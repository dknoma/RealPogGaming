using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Player status effect class. 
 */
public class StatusEffects : MonoBehaviour {

	public enum Status { /*Bog,*/ Burn, Poison, Stun, Silence };
	public enum StatChange { ATKUp, /*SpeedUp,*/ DEFUp, HPUp, ATKDown, /*SpeedDown,*/ DEFDown, HPDestruct}
	//	public enum StatUps { PATKUp, MATKUp, /*SpeedUp,*/ PDEFUp, MDEFUp, HPUp }; 
	//	public enum StatDowns { PATKDown, MATKDown, /*SpeedDown,*/ PDEFDown, MDEFDown, HPDestruct };
//	public enum StatChange { PATKUp, MATKUp, /*SpeedUp,*/ PDEFUp, MDEFUp, HPUp, 
//		PATKDown, MATKDown, /*SpeedDown,*/ PDEFDown, MDEFDown, HPDestruct}

	// Decides whether a character has a status affliction or not
	private bool[] afflictedStatuses = new bool[(int) Status.Silence+1];
	//	private bool[] afflictedStatUpStatus = new bool[StatUps.HPUp+1];
	//	private bool[] afflictedStatDownStatus = new bool[StatDowns.HPDestruct+1];
	private bool[] afflictedStatChange = new bool[(int) StatChange.HPDestruct+1];

	// Decides whether a character is immune to a state or not
	private bool[] statusResists = new bool[(int) Status.Silence+1];
	private bool[] statChangeResists = new bool[(int) StatChange.HPDestruct+1];
	private bool[] statChangeRemovalResists = new bool[(int) StatChange.HPDestruct+1];	

	/* 
	 * Afflictions 
	 */
	public bool[] getStatusAfflictions() {
		return this.afflictedStatuses;
	}

	public bool[] getStatChangeAfflictions() {
		return this.afflictedStatChange;
	}

	public bool afflictedByStatus(Status status) {
		return this.afflictedStatuses[(int) status];
	}

	public bool afflictedByStatChange(StatChange statChange) {
		return this.afflictedStatChange[(int) statChange];
	}

	public void afflictStatus(Status status) {
		this.afflictedStatuses[(int) status] = true;
	}

	public void removeStatus(int status) {
		this.afflictedStatuses[status] = false;
	}

	public void afflictStatChange(StatChange statChange) {
		this.afflictedStatChange[(int) statChange] = true;
	}

	public void removeStatChange(int statChange) {
		this.afflictedStatChange[statChange] = false;
	}

	//	public bool alreadyAfflictedStatUp(StatUps statUp) {
	//		return this.afflictedStatUpStatus[statUp];
	//	}
	//
	//	public bool alreadyAfflictedStatDown(StatDowns statDown) {
	//		return this.afflictedStatDownStatus[statDown];
	//	}

	/* 
	 * Resists 
	 */
	public bool resistsStatusEffect(Status status) {
		return this.statusResists[(int) status];
	}

	public bool resistsStatChange(StatChange statChange) {
		return this.statChangeResists[(int) statChange];
	}

	public bool resistsStatChangeRemoval(StatChange statChange) {
		return this.statChangeRemovalResists[(int) statChange];
	}
}
