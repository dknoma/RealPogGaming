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
	private int numUnits;

	private float mitigationRate = 0.85f;
	private int maxDef = 100;
	private int maxAtk = 100;

	// Use this for initialization
	void Start () {
		this.numUnits = this.units.Capacity;
		this.unitHeap = new MaxHeap(this.units.ToArray(), 0, this.numUnits);
	}


	public void calculateDamage(Character source, Character target) {
		int damage = Mathf.RoundToInt(
			Mathf.Pow (source.currentAtk, 2) / target.getCurrentDef() 	// Standard atk-def calc
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)	// Level compensation
			* ElementalAffinity.calcElementalDamage(source.getCurrentAction().getElement(), 
				target.getCurrentAction().getElement()));  				// elemental multiplier
	}

	/**
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */ 
	public void calculatePriority() {
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
