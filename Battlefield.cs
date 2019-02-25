using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour {

	public enum WinStatus { Win, Lose, Escape };

	// Queue used for determining which unit goes when.
	private Queue actionQueue = new Queue();
	private Queue actionQueueUI = new Queue();
	private Party party;
	// List of units to be part of the battle
	private List<GameObject> units = new List<GameObject>();
	private GameObject currentUnit;
	private int numUnits;
	private int expToGive = 0;

	public bool battleOver = false;
	private bool isPressed = false;
	private bool canBePressed = true;
	private bool finishedStatusPhase = false;
	private bool finishedActionPhase = false;
	private bool finishedBattlePhase = false;
	private bool finishedResolve = false;
	private bool finishedTurn = false;

	void Start() {
		this.party = transform.parent.GetComponentInChildren<Party>();
		this.party.initPartyMembers ();
	}

	public bool battleFinished() {
		return this.battleOver;
	}

	// Use this for initialization
	public void InitBattle () {
		this.units = new List<GameObject> ();
		addPartyToList();
		addEnemiesToList();
		this.numUnits = this.units.Count;
		Debug.Log ("number of units: " + this.numUnits);
		StartCoroutine(startBattle ());
	}

	private IEnumerator startBattle() {
		// Can add things here if need things to happen before a battle starts.
		// startEvent(); // Event class takes care of events that happen before a battle.
		// While battle isnt over, take turns
		while(!battleOver) {
			Debug.Log ("Action.");
			calculatePriority ();
			Debug.Log ("Queue size: " + this.actionQueue.Count);
	//			startTurns ();
			StartCoroutine (startTurns ());
			this.finishedTurn = false;
			StartCoroutine (finishTurn ());
			yield return new WaitUntil(() => this.finishedTurn);
		}
		Debug.Log ("Battle over.");
		this.battleOver = false;
	}

	private IEnumerator finishTurn() {
		yield return new WaitUntil(() => this.finishedStatusPhase && this.finishedActionPhase &&
			this.finishedBattlePhase && this.finishedResolve);
		this.finishedTurn = true;
	}

	private IEnumerator startTurns() {
//	private void startTurns() {
//		while(this.actionQueue.Count > 0 && this.battleOver == false) {
//			takeTurn ();
//			yield return null;
//		}
		while(this.actionQueue.Count > 0) {
			takeTurn ();
			this.finishedTurn = false;
			StartCoroutine (finishTurn ());
			yield return new WaitUntil(() => this.finishedTurn);
		}
		// TODO: Debug so unity doesnt crash
//		while(this.battleOver == false) {
//			yield return null;
//		}
//		yield return new WaitUntil(() => this.battleOver);
	}

	public void takeTurn() {
		Debug.Log ("Units in queue: " + this.actionQueue.Count);
		this.currentUnit = this.actionQueue.Dequeue() as GameObject;
		Debug.Log ("Starting " + this.currentUnit.name + "'s turn.");
		//		battlePhase ();
		//		resolveTurn ();
		statusPhase ();
		StartCoroutine(actionPhase ());
		StartCoroutine(battlePhase ());
		StartCoroutine(resolveTurn ());
	}

	public void statusPhase() {
		// Check status of current unit before it acts
		this.finishedStatusPhase = false;
		this.currentUnit.GetComponent<Character>().checkStatusAfflictions();
		this.finishedStatusPhase = true;
	}

	public IEnumerator actionPhase() {
		// Start of turn
		// Check if unit can act before acting
		yield return new WaitUntil(() => this.finishedStatusPhase);
		this.finishedActionPhase = false;
		if(this.currentUnit.GetComponent<Character>().canCharacterAct()) {
//			Debug.Log ("Units left in queue: " + this.actionQueue.Count);
			StartCoroutine (waitForInput ());
			yield return new WaitUntil (() => this.finishedActionPhase);
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

	public IEnumerator battlePhase() {
		// When action is chosen, do stuff
		this.finishedBattlePhase = false;
		yield return new WaitUntil (() => this.finishedActionPhase);
		// Do stuff once previous phase is finished
		this.finishedBattlePhase = true;
	}

	public IEnumerator resolveTurn() {
		this.finishedResolve = false;
		yield return new WaitUntil (() => this.finishedBattlePhase);
		// Do stuff once previous phase is finished
		this.finishedResolve = true;
//		this.battleOver = true;
	}

	/*** Methods that determine poll rate of inputs ***/
	IEnumerator waitForInput() {
		this.isPressed = false;
		StartCoroutine(waitForButtonReset());
		yield return new WaitUntil(() => this.isPressed && this.canBePressed);
		this.finishedActionPhase = true;
	}

	IEnumerator waitForButtonReset() {
		if(this.canBePressed) {
			this.canBePressed = false;
			yield return new WaitForSecondsRealtime(0.4f);
			yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") == 0);
			this.isPressed = false;
			this.canBePressed = true;

		}
		yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") != 0);
		this.isPressed = true;
	}
	/**************************************************/

	// TODO: Debug to end battle phase
	public void endBattle(WinStatus winStatus) {
		switch(winStatus) {
		case WinStatus.Win:
			break;
		case WinStatus.Lose:
			break;
		case WinStatus.Escape:
			break;
		}
		this.battleOver = true;
	}

	private void addPartyToList() {
		// Get party members from sibling component
		List<GameObject> partyMembers = this.party.getPartyMembers();
		foreach(GameObject member in partyMembers) {
			this.units.Add (member);
		}
	}

	private void addEnemiesToList() {
		GameObject enemyPool = GameObject.FindGameObjectWithTag("EnemyPool");
		int enemyCount = enemyPool.transform.childCount;
		for(int i = 0; i < enemyCount; i++) {
			GameObject enemy = enemyPool.transform.GetChild (i).gameObject;
			this.units.Add (enemy);
		}
	}

	private void calculateDamage(Character source, Character target) {
		int damage = Mathf.RoundToInt(
			Mathf.Pow (source.getCurrentAtk(), 2) / target.getCurrentDef() 	// Standard atk-def calc
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)	// Level compensation
			* ElementalAffinity.calcElementalDamage(source.getCurrentBattleActions().getElement(), 
				target.getCurrentBattleActions().getElement()));  				// elemental multiplier
	}

	/**
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */ 
	private void calculatePriority() {
		// Sor the units based on speed
		Sorting.descendingMergeSort (this.units, new CompareCharactersBySpeed());
		// Add unit to the queue
		for(int i = 0; i < this.units.Count; i++) {
			this.actionQueue.Enqueue(this.units[i]);
			this.actionQueueUI.Enqueue(this.units[i]);
		}
//		for (int i = this.numUnits / 2; i >= 0; i--) {
//			this.unitHeap.buildHeap(i);
//		}
//		this.unitHeap.sorting();
//		for(int i = 0; i < this.unitHeap.size(); i++) {
//			this.actionQueue.Enqueue(this.unitHeap.get(i));
//			this.actionQueueUI.Enqueue(this.unitHeap.get(i));
//		}

//		foreach(Character character in this.unitHeap.getHeap()) {
//			this.actionQueue.Enqueue(character);
//			this.actionQueueUI.Enqueue(character);
//		}
	}

	// Custom comparer for character game objects
	private class CompareCharactersBySpeed : IComparer {
		int IComparer.Compare(object x, object y) {
			GameObject src = x as GameObject;
			GameObject target = y as GameObject;
			return src.GetComponent<Character>().getCurrentSpd() - target.GetComponent<Character>().getCurrentSpd();
		}
	}
}
