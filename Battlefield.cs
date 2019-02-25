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
//		private List<CharacterToSort> units = new List<CharacterToSort>();
	//	private List<Character> units = new List<Character>();
	private List<GameObject> units = new List<GameObject>();
//	private MaxHeap unitHeap;
	private GameObject currentUnit;
	private int numUnits;
	private int expToGive = 0;

	private bool battleOver = false;

	void Start() {
		this.party = transform.parent.GetComponentInChildren<Party>();
		this.party.initPartyMembers ();
	}

	// Use this for initialization
	public void InitBattle () {
//		this.party = transform.parent.GetComponentInChildren<Party>();
//		this.units = new List<CharacterToSort> ();
		//		this.units = new List<Character> ();
		this.units = new List<GameObject> ();
		addPartyToList();
		addEnemiesToList();
		this.numUnits = this.units.Count;
		Debug.Log ("number of units: " + this.numUnits);
		//		this.unitHeap = new MaxHeap(this.units.ToArray(), 0, this.numUnits-1);

		string[] strings = { "a", "c", "b"};
		Sorting.mergeSort (strings);
		debugList (strings);
		debugList ();
		Sorting.descendingMergeSort (this.units, new CompareCharacters());
		debugList ();
//		CharacterToSort[] characters = Sorting.mergeSort(this.units.ToArray()) as CharacterToSort[];
		//		Character[] characters = Sorting.mergeSort(this.units.ToArray()) as Character[];
//		Character[] characters = Sorting.descendingMergeSort(this.units.ToArray()) as Character[];
//		debugList (characters);
//		startBattle ();
	}


	public void debugList(object[] array) {
		for(int i = 0; i < array.Length; i++) {
			Debug.Log ("thing: " + array[i]);
		}
	}

	public void debugList(Character[] array) {
		for(int i = 0; i < array.Length; i++) {
			Character character = array[i];
			Debug.Log ("Char: " + character.name);
		}
	}

	public void debugList() {
		for(int i = 0; i < this.units.Count; i++) {
			GameObject character = this.units[i];
			Debug.Log ("Char: " + character.name);
		}
	}
//	public void debugList() {
//		for(int i = 0; i < this.units.Count; i++) {
//			CharacterToSort character = this.units[i] as CharacterToSort;
//			Debug.Log ("Char: " + character);
//		}
//	}
//	public void debugList() {
//		for(int i = 0; i < this.units.Count; i++) {
//			Character character = this.units[i] as Character;
//			Debug.Log ("Char: " + character.name);
//		}
//	}

	private void startBattle() {
		// Can add things here if need things to happen before a battle starts.
		// startEvent(); // Event class takes care of events that happen before a battle.
		// While battle isnt over, take turns
		while(!battleOver) {
			Debug.Log ("Action.");
//			calculatePriority ();
			startTurns ();
		}
		Debug.Log ("Battle over.");
		this.battleOver = false;
	}

	private void startTurns() {
		//
		while(this.actionQueue.Count > 0) {
			takeTurn ();
		}
		// TODO: Debug so unity doesnt crash
		this.battleOver = true;
	}

	public void takeTurn() {
		this.currentUnit = this.actionQueue.Dequeue() as GameObject;
		statusPhase ();
		actionPhase ();
		battlePhase ();
		resolveTurn ();
	}

	public void statusPhase() {
		// Check status of current unit before it acts
		this.currentUnit.GetComponent<Character>().checkStatusAfflictions();
	}

	public void actionPhase() {
		// Start of turn
		// Check if unit can act before acting
		if(this.currentUnit.GetComponent<Character>().canCharacterAct()) {
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
//			this.units.Add (member.GetComponent<Character> ());
//			this.units.Add(new CharacterToSort(member.name, member.GetComponent<Character>().getCurrentSpd()));
		}
	}

	private void addEnemiesToList() {
		GameObject enemyPool = GameObject.FindGameObjectWithTag("EnemyPool");
		int enemyCount = enemyPool.transform.childCount;
		for(int i = 0; i < enemyCount; i++) {
			GameObject enemy = enemyPool.transform.GetChild (i).gameObject;
			this.units.Add (enemy);
//			this.units.Add (enemy.GetComponent<Character> ());
//			this.units.Add(new CharacterToSort(enemy.name, enemy.GetComponent<Character>().getCurrentSpd()));
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
//		Sorting.mergeSort(this.units, new CompareCharacters());
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

//	private class CharacterToSort : IComparable {
//
//		private string characterName;
//		private int currentSpeed;
//
//		public CharacterToSort(string name, int currentSpd) {
//			this.characterName = name;
//			this.currentSpeed = currentSpd;
//		}
//
//		/**
//		 * Compare to method that helps sort units by speed.
//		 */ 
//		int IComparable.CompareTo(object obj) {
//			if (obj == null) {
//				return 1;
//			} 
//			CharacterToSort otherCharacter = obj as CharacterToSort;
//			if(otherCharacter != null) {
//				return this.currentSpeed - otherCharacter.currentSpeed;
//			} else {
//				throw new ArgumentException("Object is not a Character...");
//			}
//		}
//
//		public override string ToString() {
//			return "Name: " + this.characterName + ", current speed: " + this.currentSpeed;
//		}
//	}

	// Custom comparer for character game objects
	private class CompareCharacters : IComparer {
		int IComparer.Compare(object x, object y) {
			GameObject src = x as GameObject;
			GameObject target = y as GameObject;
//			Debug.Log ("src spd: " + src.GetComponent<Character>().getCurrentSpd() + 
//				", target spd: " + target.GetComponent<Character>().getCurrentSpd());
			return src.GetComponent<Character>().getCurrentSpd() - target.GetComponent<Character>().getCurrentSpd();
		}
	}
}
