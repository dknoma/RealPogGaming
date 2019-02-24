using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour {

	// Queue used for determining which unit goes when.
	private Queue actionQueue = new Queue();
	private Queue actionQueueUI = new Queue();
	// List of units to be part of the battle
	private List<Character> units = new List<Character>();
	private MaxHeap unitHeap;
	private Character currentUnit;
	private int numUnits;
	private int expToGive = 0;

	private bool battleOver = false;

	// Use this for initialization
	public void InitBattle () {
		addPartyToList();
		addEnemiesToList();
		this.numUnits = this.units.Capacity;
		this.unitHeap = new MaxHeap(this.units.ToArray(), 0, this.numUnits);
		startBattle ();
	}

	private void startBattle() {
		// Can add things here if need things to happen before a battle starts.
		// startEvent(); // Event class takes care of events that happen before a battle.
		// While battle isnt over, take turns
		while(!battleOver) {
			calculatePriority ();
			startTurns ();
		}
	}

	private void startTurns() {
		//
		while(this.actionQueue.Count > 0) {
			takeTurn ();
		}
	}

	public void takeTurn() {
		this.currentUnit = this.actionQueue.Dequeue() as Character;
		statusPhase ();
		actionPhase ();
		battlePhase ();
		resolveTurn ();
	}

	public void statusPhase() {
		// Check status of current unit before it acts
		this.currentUnit.checkStatusAfflictions();
	}

	public void actionPhase() {
		// Start of turn
		// Check if unit can act before acting
		if(this.currentUnit.canCharacterAct()) {
			// Check if party member or enemy
			// If party member, Choose action
				//	attack
				//	defend
				//	use item
				//	etc
				// Choose a target for the action if needed
			// Else, let enemy AI decide an attack
			// Action is determined by a characters Act() method
			//		this allows units to have special actions ex. two actions per turn, act every other turn, etc...
		} else {
			// character can't act. must end their turn
		}
	}

	public void battlePhase() {
		// When action is chosen, do stuff

	}

	public void resolveTurn() {
		
	}

	private void addPartyToList() {
		// Get party members from sibling component
		List<PartyMember> partyMembers = transform.parent.GetComponentInChildren<Party>().getPartyMembers();
		foreach(PartyMember member in partyMembers) {
			this.units.Add(member.getPartyMember().GetComponent<Character>());
		}
	}

	private void addEnemiesToList() {
		// TODO: create enemy list things
	}

	private void calculateDamage(Character source, Character target) {
		int damage = Mathf.RoundToInt(
			Mathf.Pow (source.currentAtk, 2) / target.getCurrentDef() 	// Standard atk-def calc
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)	// Level compensation
			* ElementalAffinity.calcElementalDamage(source.getCurrentBattleActions().getElement(), 
				target.getCurrentBattleActions().getElement()));  				// elemental multiplier
	}

	/**
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */ 
	private void calculatePriority() {
		for (int i = this.numUnits / 2; i >= 0; i--) {
			this.unitHeap.buildHeap(i);
		}
		this.unitHeap.sorting();
		foreach(Character character in this.unitHeap.getHeap()) {
			this.actionQueue.Enqueue(character);
			this.actionQueueUI.Enqueue(character);
		}
	}
}
