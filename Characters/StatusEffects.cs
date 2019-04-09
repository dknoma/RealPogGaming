using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Status {
	Bog, 
	Burn, 
	Poison, 
	RuneLock, 
	Stun, 
	Silence, 
	Incapacitated
}
public enum StatChange { 
	AtkUp, 
	DefUp, 
	SpeedUp, 
	HpUp, 
	AtkDown, 
	DefDown, 
	SpeedDown, 
	HpDestruct}
/*
 * Player status effect class. 
 */
public class StatusEffects : MonoBehaviour {

	//	public enum StatUps { PATKUp, MATKUp, /*SpeedUp,*/ PDEFUp, MDEFUp, HPUp }; 
	//	public enum StatDowns { PATKDown, MATKDown, /*SpeedDown,*/ PDEFDown, MDEFDown, HPDestruct };
//	public enum StatChange { PATKUp, MATKUp, /*SpeedUp,*/ PDEFUp, MDEFUp, HPUp, 
//		PATKDown, MATKDown, /*SpeedDown,*/ PDEFDown, MDEFDown, HPDestruct}

	protected Dictionary<Status, bool> afflictedStatuses = new Dictionary<Status, bool>();
	protected Dictionary<StatChange, bool> afflictedStatChange = new Dictionary<StatChange, bool>();
	protected Dictionary<Status, bool> statusResists = new Dictionary<Status, bool>();
	protected Dictionary<StatChange, bool> statChangeResists = new Dictionary<StatChange, bool>();
	protected Dictionary<Status, bool> statusRemovalResists = new Dictionary<Status, bool>();
	protected Dictionary<StatChange, bool> statChangeRemovalResists = new Dictionary<StatChange, bool>();

	// Decides whether a character has a status affliction or not
//	protected bool[] afflictedStatuses = new bool[(int) Status.Silence+1];
	//	private bool[] afflictedStatUpStatus = new bool[StatUps.HPUp+1];
	//	private bool[] afflictedStatDownStatus = new bool[StatDowns.HPDestruct+1];
//	protected bool[] afflictedStatChange = new bool[(int) StatChange.HpDestruct+1];
	// Decides whether a character is immune to a state or not
//	protected bool[] statusResists = new bool[(int) Status.Silence+1];
//	protected bool[] statChangeResists = new bool[(int) StatChange.HpDestruct+1];
//	protected bool[] statusRemovalResists = new bool[(int) Status.Silence+1];	
//	protected bool[] statChangeRemovalResists = new bool[(int) StatChange.HpDestruct+1];

	private void Awake() {
		// TODO: check persistant data for statuses, else init all to false
		foreach (Status status in Enum.GetValues(typeof(Status))) {
			afflictedStatuses.Add(status, false);
			statusResists.Add(status, false);
			statusRemovalResists.Add(status, false);
		}
		foreach (StatChange statChange in Enum.GetValues(typeof(StatChange))) {
			afflictedStatChange.Add(statChange, false);
			statChangeResists.Add(statChange, false);
			statChangeRemovalResists.Add(statChange, false);
		}
	}

	/* 
	 * Afflictions 
	 */
	protected Dictionary<Status, bool> GetStatusAfflictions() {
		return afflictedStatuses;
	}

	protected Dictionary<StatChange, bool> GetStatChangeAfflictions() {
		return afflictedStatChange;
	}

	protected bool AfflictedByStatus(Status status) {
		Debug.LogFormat("Checking if afflicted by {0}", status);
		return afflictedStatuses[status];
	}

	protected bool AfflictedByStatChange(StatChange statChange) {
		return afflictedStatChange[statChange];
	}

	protected void AfflictStatus(Status status) {
		afflictedStatuses[status] = true;
	}

	protected void RemoveStatus(Status status) {
		afflictedStatuses[(Status) status] = false;
	}

	protected void AfflictStatChange(StatChange statChange) {
		afflictedStatChange[statChange] = true;
	}

	protected void RemoveStatChange(StatChange statChange) {
		afflictedStatChange[statChange] = false;
	}

	/* 
	 * Resists 
	 */
	public void AddStatusResist(Status status) {
		statusResists[status] = true;
	}

	public void RemoveStatusResist(Status status) {
		statusResists[status] = false;
	}

	protected bool ResistsStatusEffect(Status status) {
		return statusResists[status];
	}

	protected bool ResistsStatChange(StatChange statChange) {
		return statChangeResists[statChange];
	}

	protected bool ResistsStatusEffectRemoval(Status status) {
		return statusRemovalResists[status];
	}

	protected bool ResistsStatChangeRemoval(StatChange statChange) {
		return statChangeRemovalResists[statChange];
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
