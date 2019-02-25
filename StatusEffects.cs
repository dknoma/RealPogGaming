using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Player status effect class. 
 */
public class StatusEffects : MonoBehaviour {

	public enum Status { Bog, Burn, Poison, Stun, Silence };
	public enum StatChange { ATKUp, DEFUp, SpeedUp, HPUp, ATKDown, DEFDown, SpeedDown, HPDestruct}
	//	public enum StatUps { PATKUp, MATKUp, /*SpeedUp,*/ PDEFUp, MDEFUp, HPUp }; 
	//	public enum StatDowns { PATKDown, MATKDown, /*SpeedDown,*/ PDEFDown, MDEFDown, HPDestruct };
//	public enum StatChange { PATKUp, MATKUp, /*SpeedUp,*/ PDEFUp, MDEFUp, HPUp, 
//		PATKDown, MATKDown, /*SpeedDown,*/ PDEFDown, MDEFDown, HPDestruct}

	// Decides whether a character has a status affliction or not
	protected bool[] afflictedStatuses = new bool[(int) Status.Silence+1];
	//	private bool[] afflictedStatUpStatus = new bool[StatUps.HPUp+1];
	//	private bool[] afflictedStatDownStatus = new bool[StatDowns.HPDestruct+1];
	protected bool[] afflictedStatChange = new bool[(int) StatChange.HPDestruct+1];

	// Decides whether a character is immune to a state or not
	protected bool[] statusResists = new bool[(int) Status.Silence+1];
	protected bool[] statChangeResists = new bool[(int) StatChange.HPDestruct+1];
	protected bool[] statusRemovalResists = new bool[(int) Status.Silence+1];	
	protected bool[] statChangeRemovalResists = new bool[(int) StatChange.HPDestruct+1];	

	/* 
	 * Afflictions 
	 */
	protected bool[] getStatusAfflictions() {
		return this.afflictedStatuses;
	}

	protected bool[] getStatChangeAfflictions() {
		return this.afflictedStatChange;
	}

	protected bool afflictedByStatus(int status) {
		return this.afflictedStatuses[status];
	}

	protected bool afflictedByStatChange(int statChange) {
		return this.afflictedStatChange[statChange];
	}

	protected void afflictStatus(int status) {
		this.afflictedStatuses[status] = true;
	}

	protected void removeStatus(int status) {
		this.afflictedStatuses[status] = false;
	}

	protected void afflictStatChange(int statChange) {
		this.afflictedStatChange[statChange] = true;
	}

	protected void removeStatChange(int statChange) {
		this.afflictedStatChange[statChange] = false;
	}

	/* 
	 * Resists 
	 */
	protected bool resistsStatusEffect(int status) {
		return this.statusResists[status];
	}

	protected bool resistsStatChange(int statChange) {
		return this.statChangeResists[statChange];
	}

	protected bool resistsStatusEffectRemoval(int status) {
		return this.statusRemovalResists[status];
	}

	protected bool resistsStatChangeRemoval(int statChange) {
		return this.statChangeRemovalResists[statChange];
	}
//	protected bool afflictedByStatus(Status status) {
//		return this.afflictedStatuses[(int) status];
//	}
//
//	protected bool afflictedByStatChange(StatChange statChange) {
//		return this.afflictedStatChange[(int) statChange];
//	}
//
//	protected void afflictStatus(Status status) {
//		this.afflictedStatuses[(int) status] = true;
//	}
//
//	protected void removeStatus(int status) {
//		this.afflictedStatuses[status] = false;
//	}
//
//	protected void afflictStatChange(StatChange statChange) {
//		this.afflictedStatChange[(int) statChange] = true;
//	}
//
//	protected void removeStatChange(int statChange) {
//		this.afflictedStatChange[statChange] = false;
//	}
//
//	/* 
//	 * Resists 
//	 */
//	protected bool resistsStatusEffect(Status status) {
//		return this.statusResists[(int) status];
//	}
//
//	protected bool resistsStatChange(StatChange statChange) {
//		return this.statChangeResists[(int) statChange];
//	}
//
//	protected bool resistsStatusEffectRemoval(Status status) {
//		return this.statusRemovalResists[(int) status];
//	}
//
//	protected bool resistsStatChangeRemoval(StatChange statChange) {
//		return this.statChangeRemovalResists[(int) statChange];
//	}
}
