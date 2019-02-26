using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStates : MonoBehaviour {

	public enum WinStatus { Win, Lose, Escape };

	public enum Direction { Up, Down, Left, Right };
	public enum Button { Fire2, Submit, Cancel };

	public static int TURN_COUNT = 0;

	// Queue used for determining which unit goes when.
	private Queue turnQueue = new Queue();
	private Queue turnQueueUI = new Queue();
	private Party party;
	// List of units to be part of the battle
	private List<GameObject> units = new List<GameObject>();
	private List<GameObject> allies = new List<GameObject>();
	private List<GameObject> enemies = new List<GameObject>();
	private GameObject currentUnit;
	private int numUnits;
	private int expToGive = 0;
	private bool fastestFirst = true;

	public bool battleOver = false;
	private bool directionIsPressed = false;
	private bool directionCanBePressed = true;
	private bool buttonIsPressed = false;
	private bool buttonCanBePressed = true;
	private bool finishedActing = false;
	private bool finishedUnitAction = false;

	private bool finishedStatusPhase = false;
	private bool finishedActionPhase = false;
	private bool finishedBattlePhase = false;
	private bool finishedResolve = false;
	private bool finishedTurn = false;

	private Menu<int> testMenu;	// Get options from units Character component
	private int currentOption = 0;

	private Direction currentDirection;
	private Button currentButton;

	private Coroutine startBattleRoutine;
	private Coroutine startTurnRoutine;
	private Coroutine finishTurnRoutine;
	private Coroutine actionRoutine;
	private Coroutine battleRoutine;
	private Coroutine resolveRoutine;
	private Coroutine buttonInputRoutine;
	private Coroutine buttonResetRoutine;
	private Coroutine directionRoutine;
	private Coroutine directionResetRoutine;

	private void testMultiAxisMenu() {
		int width = 6;
		int height = 10;
		this.testMenu = new Menu<int> (width, height, Menu<int>.Type.Both);
		this.testMenu.initTestMenu (new int[width*height]);
//		this.testMenu.initMATestMenu (new int[width, height]);
	}

	private void testSingleAxisMenu() {
		int testSize = 5;
		this.testMenu = new Menu<int> (testSize, Menu<int>.Type.Horizontal);
		this.testMenu.initTestMenu (new int[testSize]);
//		this.testMenu.initSATestMenu (new int[testSize]);
	}

	void Start() {
		this.party = transform.parent.GetComponentInChildren<Party>();
		this.party.initPartyMembers ();

		testMultiAxisMenu ();
//		testSingleAxisMenu ();

//		this.startBattleRoutine = startBattle ();
//		this.startTurnRoutine = startTurns ();
//		this.finishTurnRoutine = finishTurn ();
//		this.actionRoutine = actionPhase ();
//		this.battleRoutine = battlePhase ();
//		this.resolveRoutine = resolveTurn ();
//		this.buttonInputRoutine = waitForButtonInput ();
//		this.buttonResetRoutine = waitForButtonReset ();
//		this.directionRoutine = waitForDirection ();
//		this.directionResetRoutine = waitForDirectionReset ();
	}

	// Use this for initialization
	public void InitBattle () {
		TURN_COUNT = 0;
		this.units = new List<GameObject> ();
		addPartyToList();
		addEnemiesToList();
		this.numUnits = this.units.Count;
		Debug.Log ("number of units: " + this.numUnits);
		//		StartCoroutine(this.startBattleRoutine);
		this.startBattleRoutine = StartCoroutine(startBattle());
	}

	private IEnumerator startBattle() {
		// Can add things here if need things to happen before a battle starts.
		// startEvent(); // Event class takes care of events that happen before a battle.
		// While battle isnt over, take turns
		this.battleOver = false;
		// While the battle isnt over, do another round of turns
		while(!battleOver) {
			Debug.Log ("Action.");
			calculatePriority (this.fastestFirst);
//			Debug.Log ("Queue size: " + this.turnQueue.Count);
//			StartCoroutine (this.startTurnRoutine);
			this.startTurnRoutine = StartCoroutine(startTurns());
			this.finishedTurn = false;
			this.finishTurnRoutine = StartCoroutine (finishTurn ());
//			StartCoroutine (this.finishTurnRoutine);
			yield return new WaitUntil(() => this.finishedTurn);
		}
		Debug.Log ("Battle over.\nTurn count: " + TURN_COUNT);
		this.battleOver = false;
	}

	private IEnumerator finishTurn() {
		yield return new WaitUntil(() => this.finishedStatusPhase && this.finishedActionPhase &&
			this.finishedBattlePhase && this.finishedResolve);
		this.finishedTurn = true;
	}

	IEnumerator startTurns() {
		// Keep this round going until all units finish their turns
		while(this.turnQueue.Count > 0) {
			TURN_COUNT++;
			Debug.Log ("Turn " + TURN_COUNT);
			takeTurn ();
			this.finishedTurn = false;
			this.finishTurnRoutine = StartCoroutine (finishTurn ());
//			StartCoroutine (this.finishTurnRoutine);
			yield return new WaitUntil(() => this.finishedTurn);
		}
		// TODO: Debug so unity doesnt crash
//		while(this.battleOver == false) {
//			yield return null;
//		}
//		yield return new WaitUntil(() => this.battleOver);
	}

	public void takeTurn() {
		Debug.Log ("Units in queue: " + this.turnQueue.Count);
		this.currentUnit = this.turnQueue.Dequeue() as GameObject;
		Debug.Log ("Starting " + this.currentUnit.name + "'s turn.");
		// Start acting
		statusPhase ();
		// Battle states
		this.actionRoutine = StartCoroutine(actionPhase ());
		this.battleRoutine = StartCoroutine(battlePhase ());
		this.resolveRoutine = StartCoroutine(resolveTurn ());
//		StartCoroutine(this.actionRoutine); // From here, have menu that can select options
//		StartCoroutine(this.battleRoutine);
//		StartCoroutine(this.resolveRoutine);
	}

	private void statusPhase() {
		// Check status of current unit before it acts
		this.finishedStatusPhase = false;
		this.currentUnit.GetComponent<Character>().checkStatusAfflictions();
		this.finishedStatusPhase = true;
	}

	IEnumerator actionPhase() {
		// Start of turn
		// Check if unit can act before acting
		if(this.currentUnit.GetComponent<Character>().canCharacterAct()) {
			yield return new WaitUntil(() => this.finishedStatusPhase);
			this.finishedActing = false;
			this.finishedActionPhase = false;
			while(this.currentUnit.GetComponent<Character>().canCharacterAct() && !this.finishedActing) {
				tryResetUnitInputs ();	// Try to reset previous inputs
				this.finishedUnitAction = false;
				this.buttonInputRoutine = StartCoroutine (waitForButtonInput ());
				this.directionRoutine = StartCoroutine (waitForDirection ());
//				StartCoroutine (this.buttonInputRoutine);
//				StartCoroutine (this.directionRoutine);
				yield return new WaitUntil (() => this.finishedUnitAction);
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
			}
			this.finishedActionPhase = true;
		} else {
			Debug.Log ("Skipping turn.");
		}
	}

	IEnumerator battlePhase() {
		// When action is chosen, do stuff
		this.finishedBattlePhase = false;
		yield return new WaitUntil (() => this.finishedActionPhase);
		// Do stuff once previous phase is finished
		this.finishedBattlePhase = true;
	}

	IEnumerator resolveTurn() {
		this.finishedResolve = false;
		yield return new WaitUntil (() => this.finishedBattlePhase);
		// Do stuff once previous phase is finished
		this.finishedResolve = true;
	}

	/*** Methods that determine poll rate of inputs ***/
	IEnumerator waitForButtonInput() {
		this.buttonIsPressed = false;
//		StartCoroutine(this.buttonResetRoutine);
		this.buttonResetRoutine = StartCoroutine (waitForButtonReset ());
		yield return new WaitUntil(() => (this.buttonIsPressed && this.buttonCanBePressed));
		switch(this.currentButton) {
		case Button.Fire2:
			Debug.Log ("Fire2");
			this.finishedUnitAction = true;
			break;
		case Button.Submit:
			Debug.Log ("Submit");
			this.finishedUnitAction = true;
			this.finishedActing = true;
			break;
		case Button.Cancel:
			Debug.Log ("Cancel");
			this.finishedUnitAction = true;
			break;
		}
	}

	IEnumerator waitForButtonReset() {
		if(this.buttonCanBePressed) {
			this.buttonCanBePressed = false;
			yield return new WaitForSecondsRealtime(0.3f);
			this.buttonIsPressed = false;
			this.buttonCanBePressed = true;
		}
		yield return new WaitUntil (() => Input.GetButtonDown ("Fire2") || Input.GetButtonDown ("Submit") || Input.GetButtonDown("Cancel"));
		if(Input.GetButtonDown ("Fire2")) {
			this.currentButton = Button.Fire2;
		} else if(Input.GetButtonDown ("Submit")) {
			this.currentButton = Button.Submit;
		} else if(Input.GetButtonDown("Cancel")) {
			this.currentButton = Button.Cancel;
		}
		this.buttonIsPressed = true;
	}

	IEnumerator waitForDirection() {
		this.directionIsPressed = false;
//		StartCoroutine(this.directionResetRoutine);
		this.directionResetRoutine = StartCoroutine (waitForDirectionReset ()); 
		yield return new WaitUntil(() => (this.directionIsPressed && this.directionCanBePressed));
		switch(this.currentDirection) {
		case Direction.Up:
			Debug.Log ("Up");
			this.testMenu.traverseTestMenu ((int) this.currentDirection);
			break;
		case Direction.Down:
			Debug.Log ("Down");
			this.testMenu.traverseTestMenu ((int) this.currentDirection);
			break;
		case Direction.Left:
			Debug.Log ("Left");
			this.testMenu.traverseTestMenu ((int) this.currentDirection);
			break;
		case Direction.Right:
			Debug.Log ("Right");
			this.testMenu.traverseTestMenu ((int) this.currentDirection);
			break;
		}
//		Debug.Log ("Current option: " + this.currentOption);
//		resetButtonInputs ();
		this.finishedUnitAction = true;
	}

	IEnumerator waitForDirectionReset() {
		if(this.directionCanBePressed) {
			this.directionCanBePressed = false;
			yield return new WaitForSecondsRealtime(0.3f);
			yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") == 0);
			this.directionIsPressed = false;
			this.directionCanBePressed = true;
		}
		yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") > 0 || Input.GetAxisRaw ("Horizontal") < 0
			|| Input.GetAxisRaw ("Vertical") > 0 || Input.GetAxisRaw ("Vertical") < 0);
//		yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") > 0 || Input.GetAxisRaw ("Horizontal") < 0);
		if(Input.GetAxisRaw ("Horizontal") > 0) {
			this.currentDirection = Direction.Right;
		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
			this.currentDirection = Direction.Left;
		} else if(Input.GetAxisRaw ("Vertical") > 0) {
			this.currentDirection = Direction.Up;
		} else if(Input.GetAxisRaw ("Vertical") < 0) {
			this.currentDirection = Direction.Down;
		}
		this.directionIsPressed = true;
	}

	/**************************************************
	 **************************************************
	 **************************************************/

//	private void traverseTestMenu(Direction direction) {
//		if(direction == Direction.Right) {
//			this.currentOption = (this.currentOption + 1) % this.testOptions.Count;
//		} else if (direction == Direction.Left) {
//			this.currentOption = (this.currentOption + this.testOptions.Count-1) % this.testOptions.Count;
//		}
//	}

	/**************************************************
	 **************************************************
	 **************************************************/
	public void endBattle(WinStatus winStatus) {
		switch(winStatus) {
		case WinStatus.Win:
			break;
		case WinStatus.Lose:
			break;
		case WinStatus.Escape:
			break;
		}
		stopAllCoroutines ();
		this.finishedTurn = true;
		this.turnQueue.Clear ();
		this.battleOver = true;
	}

	private void resetButtonInputs() {
		StopCoroutine(this.buttonInputRoutine);
		StopCoroutine(this.buttonResetRoutine);
	}

	private void resetDirectionInputs() {
		StopCoroutine(this.directionRoutine);
		StopCoroutine(this.directionResetRoutine);
	}

	private void tryResetUnitInputs() {
		if(this.buttonInputRoutine != null && this.buttonResetRoutine != null
			&& this.directionRoutine != null && this.directionResetRoutine != null) {
			resetUnitInputs ();
		} else {
			Debug.Log ("Most likely the battle just started. No routines to clean at this time.");
		}
	}

	private void resetUnitInputs() {
		StopCoroutine(this.buttonInputRoutine);
		StopCoroutine(this.buttonResetRoutine);
		StopCoroutine(this.directionRoutine);
		StopCoroutine(this.directionResetRoutine);
	}

	private void stopAllCoroutines() {
		StopCoroutine(this.actionRoutine);
		StopCoroutine(this.battleRoutine);
		StopCoroutine(this.resolveRoutine);
		StopCoroutine(this.buttonInputRoutine);
		StopCoroutine(this.buttonResetRoutine);
		StopCoroutine(this.directionRoutine);
		StopCoroutine(this.directionResetRoutine);
		StopCoroutine(this.startTurnRoutine);
	}

	/***************************
	 * 						   *
	 * Party related functions *
	 * 						   *
	 ***************************/ 
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
			this.enemies.Add (enemy);
		}
	}

	/***************************
	 * 						   *
	 *  Dmg related functions  *
	 * 						   *
	 ***************************/ 

	private int calculateDamage(Character source, Character target) {
		int damage = Mathf.RoundToInt(
			Mathf.Pow (source.getCurrentAtk(), 2) / target.getCurrentDef() 	// Standard atk-def calc
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)	// Level compensation
			* ElementalAffinity.calcElementalDamage(source.getCurrentBattleActions().getElement(), 
				target.getCurrentBattleActions().getElement()));  				// elemental multiplier
		return damage;
	}

	/**
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */ 
	private void calculatePriority(bool fastestFirst) {
		// Sor the units based on speed
		if (fastestFirst) {
			Sorting.descendingMergeSort (this.units, new CompareCharactersBySpeed ());
		} else {
			Sorting.mergeSort (this.units, new CompareCharactersBySpeed ());
		}
		// Add unit to the queue
		for(int i = 0; i < this.units.Count; i++) {
			this.turnQueue.Enqueue(this.units[i]);
			this.turnQueueUI.Enqueue(this.units[i]);
		}
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
