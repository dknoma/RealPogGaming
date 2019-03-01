using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStates : MonoBehaviour {

	public enum WinStatus { Win, Lose, Escape };

	public enum Direction { Up, Down, Left, Right };
	public enum Button { Fire2, Submit, Jump, Cancel };

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
	private GameObject currentTarget;
	private int numUnits;
	private int expToGive = 0;
	private bool fastestFirst = true;
	private bool currentIsAlly = true;

	public bool battleOver;
	private bool directionIsPressed;
	private bool directionCanBePressed = true;
	private bool buttonIsPressed;
	private bool buttonCanBePressed = true;
	private bool finishedActing;
	private bool finishedUnitAction;

	private bool finishedStatusPhase;
	private bool finishedActionPhase;
	private bool finishedBattlePhase;
	private bool finishedResolve;
	private bool finishedTurn;

	private MenuGraph<int> testMenu;	// Get options from units Character component
	//private int currentOption = 0;

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

	private void TestMultiAxisMenu() {
		int width = 6;
		int height = 10;
		this.testMenu = new MenuGraph<int> (width, height, MenuGraph<int>.Type.Both);
		this.testMenu.initTestMenu (new int[width*height]);
//		this.testMenu.initMATestMenu (new int[width, height]);
	}

	private void TestSingleAxisMenu() {
		int testSize = 5;
		this.testMenu = new MenuGraph<int> (testSize, MenuGraph<int>.Type.Horizontal);
		this.testMenu.initTestMenu (new int[testSize]);
//		this.testMenu.initSATestMenu (new int[testSize]);
	}

	void Start() {
		this.party = GameObject.FindGameObjectWithTag("Party").GetComponent<Party>();
		this.party.initPartyMembers ();
	}

	// Use this for initialization
	public void InitBattle () {
		TestMultiAxisMenu ();
		//		TestSingleAxisMenu ();
		// Initialize battle settings
		TURN_COUNT = 0;
		this.units = new List<GameObject> ();
		this.turnQueue = new Queue ();
		// Add all units involved in the battle to the list
		AddPartyToList();
		AddEnemiesToList();
		// Init each characters rune bonuses
		foreach(GameObject unit in units) {
			CalculateRuneStats(unit);
		}
		this.numUnits = this.units.Count;
		Debug.Log ("number of units: " + this.numUnits);
		this.startBattleRoutine = StartCoroutine(StartBattle());
	}

	private IEnumerator StartBattle() {
		// Can add things here if need things to happen before a battle starts.
		// startEvent(); // Event class takes care of events that happen before a battle.
		// While battle isnt over, take turns
		this.battleOver = false;
		// While the battle isnt over, do another round of turns
		while(!battleOver) {
			Debug.Log ("Action.");
			CalculatePriority (this.fastestFirst);
//			Debug.Log ("Queue size: " + this.turnQueue.Count);
			this.startTurnRoutine = StartCoroutine(StartTurns());
			this.finishedTurn = false;
			this.finishTurnRoutine = StartCoroutine (FinishTurn ());
			yield return new WaitUntil(() => this.finishedTurn);
		}
		Debug.Log ("Battle over.\nTurn count: " + TURN_COUNT);
		this.battleOver = false;
	}

	private IEnumerator FinishTurn() {
		yield return new WaitUntil(() => this.finishedStatusPhase && this.finishedActionPhase &&
			this.finishedBattlePhase && this.finishedResolve);
		this.finishedTurn = true;
	}

	IEnumerator StartTurns() {
		// Keep this round going until all units finish their turns
		while(this.turnQueue.Count > 0) {
			TURN_COUNT++;
			Debug.Log ("Turn " + TURN_COUNT);
			TakeTurn ();
			this.finishedTurn = false;
			this.finishTurnRoutine = StartCoroutine (FinishTurn ());
			yield return new WaitUntil(() => this.finishedTurn);
		}
	}

	public void TakeTurn() {
//		Debug.Log ("Units in queue: " + this.turnQueue.Count);
		this.currentUnit = this.turnQueue.Dequeue() as GameObject;
		Debug.Log("=== Begin Turn ===");
		Debug.Log(string.Format("Name: {0}, HP: {1}, exp until level up: {2}",
			this.currentUnit.name,
			this.currentUnit.GetComponent<Character>().GetCurrentHP(),
			this.currentUnit.GetComponent<Character>().expUntilLevelUp));
		// Start acting
		StatusPhase ();
		// Battle states
		this.actionRoutine = StartCoroutine(ActionPhase ());
		this.battleRoutine = StartCoroutine(BattlePhase ());
		this.resolveRoutine = StartCoroutine(ResolveTurn ());
//		StartCoroutine(this.actionRoutine); // From here, have menu that can select options
//		StartCoroutine(this.battleRoutine);
//		StartCoroutine(this.resolveRoutine);
	}

	private void StatusPhase() {
		// Check status of current unit before it acts
		this.finishedStatusPhase = false;
		this.currentUnit.GetComponent<Character>().checkStatusAfflictions();
		this.finishedStatusPhase = true;
	}

	IEnumerator ActionPhase() {
		// Start of turn
		// Check if unit can act before acting
		this.finishedActing = false;
		this.finishedActionPhase = false;
		if (this.currentUnit.GetComponent<Character>().canCharacterAct()) {
			yield return new WaitUntil(() => this.finishedStatusPhase);
            Debug.Log("=== Entering Action Phase ===");
			while(this.currentUnit.GetComponent<Character>().canCharacterAct() && !this.finishedActing) {
				TryResetUnitInputs ();	// Try to reset previous inputs
				this.finishedUnitAction = false;
				this.buttonInputRoutine = StartCoroutine (WaitForButtonInput ());
				this.directionRoutine = StartCoroutine (WaitForDirection ());
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
			this.finishedActing = true;
			this.finishedActionPhase = true;
		}
	}

	IEnumerator BattlePhase() {
		// When action is chosen, do stuff
		this.finishedBattlePhase = false;
		yield return new WaitUntil (() => this.finishedActionPhase);
        Debug.Log("=== Entering Battle Phase ===");
        // Do stuff once previous phase is finished
        GameObject target = this.currentUnit.CompareTag("Enemy") ? this.party.frontUnit : this.enemies[0];
		int dmg = CalculateDamage(this.currentUnit, target);
		yield return new WaitUntil(() => TryDamage(dmg, target));
		this.finishedBattlePhase = true;
	}

	IEnumerator ResolveTurn() {
		this.finishedResolve = false;
		yield return new WaitUntil (() => this.finishedBattlePhase);
        // Do stuff once previous phase is finished
        Debug.Log("=== Resolving Turn ===");
        this.currentUnit.GetComponent<Character>().resolveStatusAfflictions();
        this.finishedResolve = true;
	}

    private bool TryDamage(int dmg, GameObject target) {
		// TODO: Check if target can be damaged.
		Debug.Log(string.Format("\t{0} dealt {1} damage to {2}", this.currentUnit.name, dmg, target.name));
        target.GetComponent<Character>().ModifyHP(-dmg);
		Debug.Log(string.Format("current HP: {0}", target.GetComponent<Character>().GetCurrentHP()));
        if(target.GetComponent<Character>().GetCurrentHP() <= 0) {
			// Remove unit from the list if no more HP
			this.units.Remove(target);
			if(target.CompareTag("Enemy")) {
				this.enemies.Remove(target);
				this.expToGive += target.GetComponent<Character>().expToGrant;
				if(this.enemies.Count == 0) {
					EndBattle(WinStatus.Win);
				}
			}
		}
		return true;
	}

    /*** 
	 * Methods that determine poll rate of inputs 
     ***/
    IEnumerator WaitForButtonInput() {
		this.buttonIsPressed = false;
//		StartCoroutine(this.buttonResetRoutine);
		this.buttonResetRoutine = StartCoroutine (WaitForButtonReset ());
		yield return new WaitUntil(() => (this.buttonIsPressed && this.buttonCanBePressed));
		switch(this.currentButton) {
			case Button.Fire2:
				Debug.Log ("Fire2");
				this.finishedUnitAction = true;
				break;
			case Button.Submit:
				Debug.Log ("Submit");
				this.finishedUnitAction = true;
				//this.finishedActing = true;
				break;
			case Button.Jump:
				Debug.Log ("Jump");
	//			this.finishedUnitAction = true;
				this.finishedUnitAction = true;
				this.finishedActing = true;
				break;
			case Button.Cancel:
				Debug.Log ("Cancel");
				this.finishedUnitAction = true;
				break;
		}
	}

	IEnumerator WaitForButtonReset() {
		if(this.buttonCanBePressed) {
			this.buttonCanBePressed = false;
			yield return new WaitForSecondsRealtime(0.3f);
			this.buttonIsPressed = false;
			this.buttonCanBePressed = true;
		}
		yield return new WaitUntil (() => Input.GetButtonDown ("Fire2") || Input.GetButtonDown ("Jump") || Input.GetButtonDown ("Submit") || Input.GetButtonDown("Cancel"));
		if(Input.GetButtonDown ("Fire2")) {
			this.currentButton = Button.Fire2;
		} else if(Input.GetButtonDown ("Submit")) {
			this.currentButton = Button.Submit;
		} else if(Input.GetButtonDown("Jump")) {
			this.currentButton = Button.Jump;
		} else if(Input.GetButtonDown("Cancel")) {
			this.currentButton = Button.Cancel;
		}
		this.buttonIsPressed = true;
	}

	IEnumerator WaitForDirection() {
		this.directionIsPressed = false;
//		StartCoroutine(this.directionResetRoutine);
		this.directionResetRoutine = StartCoroutine (WaitForDirectionReset ()); 
		yield return new WaitUntil(() => (this.directionIsPressed && this.directionCanBePressed));
		switch(this.currentDirection) {
			case Direction.Up:
				Debug.Log ("Up");
				this.testMenu.TraverseOptions ((int) this.currentDirection);
				break;
			case Direction.Down:
				Debug.Log ("Down");
				this.testMenu.TraverseOptions ((int) this.currentDirection);
				break;
			case Direction.Left:
				Debug.Log ("Left");
				this.testMenu.TraverseOptions ((int) this.currentDirection);
				break;
			case Direction.Right:
				Debug.Log ("Right");
				this.testMenu.TraverseOptions ((int) this.currentDirection);
				break;
		}
//		Debug.Log ("Current option: " + this.currentOption);
//		resetButtonInputs ();
		this.finishedUnitAction = true;
	}

	IEnumerator WaitForDirectionReset() {
		if(this.directionCanBePressed) {
			this.directionCanBePressed = false;
			yield return new WaitForSecondsRealtime(0.3f);
			yield return new WaitUntil (() => (int)Input.GetAxisRaw ("Horizontal") == 0);
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
	public void EndBattle(WinStatus winStatus) {
		switch(winStatus) {
			case WinStatus.Win:
				GrantExpToParty();
				break;
			case WinStatus.Lose:
				break;
			case WinStatus.Escape:
				break;
		}
		StopAllCoroutines ();
		this.finishedTurn = true;
		this.turnQueue.Clear ();
		this.battleOver = true;
	}

	private void ResetButtonInputs() {
		StopCoroutine(this.buttonInputRoutine);
		StopCoroutine(this.buttonResetRoutine);
	}

	private void ResetDirectionInputs() {
		StopCoroutine(this.directionRoutine);
		StopCoroutine(this.directionResetRoutine);
	}

	private void TryResetUnitInputs() {
		if(this.buttonInputRoutine != null && this.buttonResetRoutine != null
			&& this.directionRoutine != null && this.directionResetRoutine != null) {
			ResetUnitInputs ();
		} else {
			Debug.Log ("Most likely the battle just started. No routines to clean at this time.");
		}
	}

	private void ResetUnitInputs() {
		StopCoroutine(this.buttonInputRoutine);
		StopCoroutine(this.buttonResetRoutine);
		StopCoroutine(this.directionRoutine);
		StopCoroutine(this.directionResetRoutine);
	}

	private void StopAllCoroutines() {
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
	private void AddPartyToList() {
		// Get party members from sibling component
		List<GameObject> partyMembers = this.party.getPartyMembers();
		foreach(GameObject member in partyMembers) {
			this.units.Add(member);
            this.allies.Add(member);
		}
	}

	private void AddEnemiesToList() {
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

	private int CalculateDamage(GameObject src, GameObject dest) {
		Character 
		source = src.GetComponent<Character> (),
		target = dest.GetComponent<Character> ();
		Debug.Log (string.Format("{0}'s atk: {1}, {2}'s def: {3}", 
			source.name, source.GetCurrentAtk(), target.name, target.GetCurrentDef()));
		Debug.Log (string.Format("{0}'s attack element: {1}, {2} element: {3}", 
			source.name, source.getCurrentBattleActions().getElement(), target.name, target.element));
		// Calculate the current units rune bonuses
		CalculateRuneStats (this.currentUnit);
		int damage = Mathf.RoundToInt(
			(Mathf.Pow(source.GetCurrentAtk(), 2) + source.GetRuneAtk())/ (target.GetCurrentDef() + target.GetRuneDef()) 	// Standard atk-def calc
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)	// Level compensation
			* ElementalAffinity.CalcElementalDamage(source.getCurrentBattleActions().getElement(), 
				target.element));  				// elemental multiplier
		return damage;
	}

	private void GrantExpToParty() {
		foreach(GameObject ally in this.allies) {
			ally.GetComponent<Character>().GrantExp(this.expToGive);
		}
	}

	/*
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */
	private void CalculatePriority(bool fastestFirst) {
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

	private void CalculateRuneStats(GameObject currentUnit) {
		RuneSlots slots = currentUnit.GetComponent<RuneSlots>();
	}

	private void CheckRuneStatus() {
		// TODO: Check if unit can use rune effects
		// Runes don't add to current stat, rather show to the side the bonuses they give
		// Allows us to disable those bonuses, add a rune calculation to the damage formula
	}

	// Custom comparer for character game objects
	private class CompareCharactersBySpeed : IComparer {
		int IComparer.Compare(object x, object y) {
			GameObject src = x as GameObject;
			GameObject target = y as GameObject;
			return src.GetComponent<Character>().GetCurrentSpd() - target.GetComponent<Character>().GetCurrentSpd();
		}
	}
}
