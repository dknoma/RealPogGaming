using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machines:
/// BattleState - what goes on during initialization, ongoing interactions, and ending the state.
/// TurneState  - what goes on during initialization, ongoing interactions, and ending the state.
/// BattlePhase - beginning of turn status, player actions, battle calculations, turn resolution.
/// WinStatus   - How the battle ended. Determines if player gets exp/loot, goes back to last save point, or escaped.
/// </summary>
public enum BattleState {
	Nil,
	Init,
	Ongoing,
	End
}

public enum TurnState {
	Nil,
	Init,
	Ongoing,
	End
}

public enum BattlePhase {
	Nil,
	Status,
	Action,
	Battle,
	Resolution
}

public enum WinStatus {
	Nil,
	Win, 
	Lose, 
	Escape
}

/// <inheritdoc />
/// <summary>
/// TODO: Maybe can convert states to enums instead of coroutines; do fixed update w/ state machine instead
/// </summary>
[DisallowMultipleComponent]
public class BattleManager : MonoBehaviour {
	private enum Direction { Up, Down, Left, Right }
	private enum Button { Fire2, Submit, Jump, Cancel }

	private static BattleManager bm;
	private static int TURN_COUNT;
	
	public BattleState BattleState { get; set; }
	public TurnState TurnState { get; set; }
	public BattlePhase BattlePhase { get; set; }
	public WinStatus WinStatus { get; set; }
	
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

	private bool takingTurn;
	private bool calculatingBattle;
	private bool resolvingTurn;

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
	
//	private BattleState battleState;
//	private TurnState turnState;
//	private BattlePhase battlePhase;

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

	private void Awake() {
		if (bm == null) {
			bm = this;
		} else if (bm != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		
		party = GameObject.FindGameObjectWithTag("Party").GetComponent<Party>();
		party.initPartyMembers ();
	}


	private void Update() {
		if (Input.GetButtonDown("Fire1") && BattleState == BattleState.Nil) {
			BattleState = BattleState.Init;
		}
		CheckBattleState();
	}

	private void CheckBattleState() {
		switch (BattleState) {
			case BattleState.Nil:
				// Not in battle.
				break;
			case BattleState.Init:
				InitBattle();
				break;
			case BattleState.Ongoing:
				CheckTurnState();
				break;
			case BattleState.End:
				TurnState = TurnState.Nil;
				BattlePhase = BattlePhase.Nil;
				BattleState = BattleState.Nil;
				takingTurn = false;
				calculatingBattle = false;
				resolvingTurn = false;
				switch(WinStatus) {
					case WinStatus.Nil:
						// Default state.
						break;
					case WinStatus.Win:
						GrantExpToParty();
						break;
					case WinStatus.Lose:
						break;
					case WinStatus.Escape:
						break;
					default:
						throw new ArgumentOutOfRangeException("WinStatus", WinStatus, null);
				}
				Debug.Log("Ending battle.");
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	private void CheckTurnState() {
		switch (TurnState) {
			case TurnState.Nil:
				// Not in battle.
				break;
			case TurnState.Init:
				if (turnQueue.Count == 0) {
					CalculatePriority (fastestFirst);
				}
				if (!takingTurn) {
					StartTurn();
				}
				break;
			case TurnState.Ongoing:
				CheckBattlePhase();
				break;
			case TurnState.End:
				// Finished turn: let next unit take its turn.
				takingTurn = false;
				calculatingBattle = false;
				resolvingTurn = false;
				TurnState = TurnState.Init;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	private void CheckBattlePhase() {
		switch (BattlePhase) {
			case BattlePhase.Nil:
				break;
			case BattlePhase.Status:
				// TODO: check what actions can take if any
				Debug.Log("Checking" + currentUnit.name + "'s status.");
				BattlePhase = BattlePhase.Action;
				break;
			case BattlePhase.Action:
				// TODO: let unit do their actions: attack, use items, run away
				if (Input.GetButtonDown("Test")) {
					Debug.Log("Ending action phase...");
					BattlePhase = BattlePhase.Battle;
				}
				break;
			case BattlePhase.Battle:
				// TODO: resolve the unit's action: attacking(action commands for bonus dmg), healing(possible action
				//		 commands here as well.), item useage, Calculate damage/heals, running away.
				if (!calculatingBattle) {
					calculatingBattle = true;
					GameObject target = currentUnit.CompareTag("Enemy") ? party.frontUnit : enemies[0];
					int dmg = CalculateDamage(currentUnit, target);
					TryDamage(dmg, target);
				}
				if (Input.GetButtonDown("Test")) {
					Debug.Log("Ending battle phase...");
					BattlePhase = BattlePhase.Resolution;
				}
				break;
			case BattlePhase.Resolution:
				// TODO: resolve end of turn effects: do DoT, decrement status counters
				if (!resolvingTurn) {
					resolvingTurn = true;
					currentUnit.GetComponent<Character>().resolveStatusAfflictions();
				}
				if (Input.GetButtonDown("Test")) {
					Debug.Log("Ending resolution phase...");
					BattlePhase = BattlePhase.Nil;
					TurnState = TurnState.End;	// End turn
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	// Use this for initialization
	private void InitBattle () {
		// Initialize battle settings
		TURN_COUNT = 0;
		units = new List<GameObject> ();
		turnQueue = new Queue ();
		// Add all units involved in the battle to the list
		AddPartyToList();
		AddEnemiesToList();
		// Init each characters rune bonuses
		foreach(GameObject unit in units) {
			CalculateRuneStats(unit);
		}
		numUnits = units.Count;
		TurnState = TurnState.Init; 		// Turn initialization: calculate turn order, etc.
		Debug.Log ("number of units: " + numUnits);
		BattleState = BattleState.Ongoing;	// Go ahead and start the battle
	}
	
	private void StartTurn() {
		// Keep this round going until all units finish their turns
		takingTurn = true;
		TURN_COUNT++;
		Debug.Log ("Turn " + TURN_COUNT);
		currentUnit = turnQueue.Dequeue() as GameObject;
		// TODO: can insert check here for possible cutscenes during a battle.
		Debug.Log("============= Begin Turn =============");
		Debug.Log(string.Format("Name: {0}, HP: {1}, exp until level up: {2}",
		                        currentUnit.name,
		                        currentUnit.GetComponent<Character>().GetCurrentHP(),
		                        currentUnit.GetComponent<Character>().expUntilLevelUp));
		BattlePhase = BattlePhase.Status;	// The start of the units turn
		TurnState = TurnState.Ongoing;		// Go ahead and start the turns
	}
	
	private bool TryDamage(int dmg, GameObject target) {
		// TODO: Check if target can be damaged.
		Debug.Log(string.Format("\t{0} dealt {1} damage to {2}", currentUnit.name, dmg, target.name));
		target.GetComponent<Character>().ModifyHP(-dmg);
		Debug.Log(string.Format("current HP: {0}", target.GetComponent<Character>().GetCurrentHP()));
		if(target.GetComponent<Character>().GetCurrentHP() <= 0) {
			// Remove unit from the list if no more HP
			units.Remove(target);
			// TODO: if party member, make incapacitated: can be revived
			if(target.CompareTag("Enemy")) {
				enemies.Remove(target);
				expToGive += target.GetComponent<Character>().expToGrant;
				if(enemies.Count == 0) {
					EndBattle(WinStatus.Win);
				}
			}
		}
		return true;
	}
	
	/**************************************************
	 **************************************************
	 **************************************************/
	// ReSharper disable once MemberCanBePrivate.Global
	public void EndBattle(WinStatus winStatus) {
		turnQueue.Clear ();
		WinStatus = winStatus;
		BattleState = BattleState.End;
//		finishedTurn = true;
//		battleOver = true;
	}
	/***************************
	 * 						   *
	 * Party related functions *
	 * 						   *
	 ***************************/ 
	private void AddPartyToList() {
		// Get party members from sibling component
		List<GameObject> partyMembers = party.getPartyMembers();
		foreach(GameObject member in partyMembers) {
			units.Add(member);
            allies.Add(member);
		}
	}

	private void AddEnemiesToList() {
		// TODO: create enemy manager; takes care of instantiating enemies in a battle, etc
		GameObject enemyPool = GameObject.FindGameObjectWithTag("EnemyPool");
		int enemyCount = enemyPool.transform.childCount;
		for(int i = 0; i < enemyCount; i++) {
			GameObject enemy = enemyPool.transform.GetChild (i).gameObject;
			units.Add (enemy);
			enemies.Add (enemy);
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
		CalculateRuneStats (currentUnit);
		int damage = Mathf.RoundToInt(
			(Mathf.Pow(source.GetCurrentAtk(), 2) + source.GetRuneAtk())/ (target.GetCurrentDef() + target.GetRuneDef()) 	// Standard atk-def calc
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)	// Level compensation
			* ElementalAffinity.CalcElementalDamage(source.getCurrentBattleActions().getElement(), 
				target.element));  				// elemental multiplier
		return damage;
	}

	private void GrantExpToParty() {
		foreach(GameObject ally in allies) {
			ally.GetComponent<Character>().GrantExp(expToGive);
		}
	}

	/*
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */
	private void CalculatePriority(bool fastestFirst) {
		// Sor the units based on speed
		if (fastestFirst) {
			Sorting.descendingMergeSort (units, new CompareCharactersBySpeed ());
		} else {
			Sorting.mergeSort (units, new CompareCharactersBySpeed ());
		}
		// Add unit to the queue
		for(int i = 0; i < units.Count; i++) {
			turnQueue.Enqueue(units[i]);
			turnQueueUI.Enqueue(units[i]);
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
	
	
//	private void StatusPhase() {
//		// Check status of current unit before it acts
//		finishedStatusPhase = false;
//		currentUnit.GetComponent<Character>().checkStatusAfflictions();
//		finishedStatusPhase = true;
//	}
//	private void StartBattle() {
//		// Can add things here if need things to happen before a battle starts.
//		// startEvent(); // Event class takes care of events that happen before a battle.
//		// While battle isnt over, take turns
////		battleOver = false;
//		// While the battle isnt over, do another round of turns
//		Debug.Log ("Action.");
//		CalculatePriority (fastestFirst);
//		TurnState = TurnState.Init;
////		startTurnRoutine = StartCoroutine(StartTurns());
////		finishedTurn = false;
////		finishTurnRoutine = StartCoroutine (FinishTurn ());
////		Debug.Log ("Battle over.\nTurn count: " + TURN_COUNT);
////		battleOver = false;
//	}

//	private IEnumerator StartBattle() {
//		// Can add things here if need things to happen before a battle starts.
//		// startEvent(); // Event class takes care of events that happen before a battle.
//		// While battle isnt over, take turns
//		battleOver = false;
//		// While the battle isnt over, do another round of turns
//		while(!battleOver) {
//			Debug.Log ("Action.");
//			CalculatePriority (fastestFirst);
////			Debug.Log ("Queue size: " + this.turnQueue.Count);
//			startTurnRoutine = StartCoroutine(StartTurns());
//			finishedTurn = false;
//			finishTurnRoutine = StartCoroutine (FinishTurn ());
//			yield return new WaitUntil(() => finishedTurn);
//		}
//		Debug.Log ("Battle over.\nTurn count: " + TURN_COUNT);
//		battleOver = false;
//	}

//	private IEnumerator FinishTurn() {
//		yield return new WaitUntil(() => finishedStatusPhase && finishedActionPhase &&
//			finishedBattlePhase && finishedResolve);
//		finishedTurn = true;
//	}

//	private IEnumerator StartTurns() {
//		// Keep this round going until all units finish their turns
//		while(turnQueue.Count > 0) {
//			TURN_COUNT++;
//			Debug.Log ("Turn " + TURN_COUNT);
//			TakeTurn ();
//			finishedTurn = false;
//			finishTurnRoutine = StartCoroutine (FinishTurn ());
//			yield return new WaitUntil(() => finishedTurn);
//		}
//	}

//	private void TakeTurn() {
////		Debug.Log ("Units in queue: " + this.turnQueue.Count);
//		currentUnit = turnQueue.Dequeue() as GameObject;
//		// TODO: can insert check here for possible cutscenes during a battle.
//		Debug.Log("=== Begin Turn ===");
//		Debug.Log(string.Format("Name: {0}, HP: {1}, exp until level up: {2}",
//		currentUnit.name,
//		currentUnit.GetComponent<Character>().GetCurrentHP(),
//		currentUnit.GetComponent<Character>().expUntilLevelUp));
//		// Start acting
//		StatusPhase ();
//		// Battle states
//		actionRoutine = StartCoroutine(ActionPhase ());
////		battleRoutine = StartCoroutine(BattlePhase ());
//		resolveRoutine = StartCoroutine(ResolveTurn ());
////		StartCoroutine(this.actionRoutine); // From here, have menu that can select options
////		StartCoroutine(this.battleRoutine);
////		StartCoroutine(this.resolveRoutine);
//	}

//	private IEnumerator ActionPhase() {
//		// Start of turn
//		// Check if unit can act before acting
//		finishedActing = false;
//		finishedActionPhase = false;
//		if (currentUnit.GetComponent<Character>().canCharacterAct()) {
//			yield return new WaitUntil(() => finishedStatusPhase);
//            Debug.Log("=== Entering Action Phase ===");
//			while(currentUnit.GetComponent<Character>().canCharacterAct() && !finishedActing) {
//				TryResetUnitInputs ();	// Try to reset previous inputs
//				finishedUnitAction = false;
//				buttonInputRoutine = StartCoroutine (WaitForButtonInput ());
//				directionRoutine = StartCoroutine (WaitForDirection ());
////				StartCoroutine (this.buttonInputRoutine);
////				StartCoroutine (this.directionRoutine);
//				yield return new WaitUntil (() => finishedUnitAction);
//				// Check if party member or enemy
//				// If party member, Choose action
//				//	attack
//				//	defend
//				//	use item
//				//	etc
//				// Choose a target for the action if needed
//				// Else, let enemy AI decide an attack
//				// Action is determined by a characters Act() method
//				//		this allows units to have special actions ex. two actions per turn, act every other turn, etc...
//			}
//			finishedActionPhase = true;
//		} else {
//			Debug.Log ("Skipping turn.");
//			finishedActing = true;
//			finishedActionPhase = true;
//		}
//	}

//	private IEnumerator BattlePhase() {
//		// When action is chosen, do stuff
//		finishedBattlePhase = false;
//		yield return new WaitUntil (() => finishedActionPhase);
//        Debug.Log("=== Entering Battle Phase ===");
//        // Do stuff once previous phase is finished
//        GameObject target = currentUnit.CompareTag("Enemy") ? party.frontUnit : enemies[0];
//		int dmg = CalculateDamage(currentUnit, target);
//		yield return new WaitUntil(() => TryDamage(dmg, target));
//		finishedBattlePhase = true;
//	}

//	private IEnumerator ResolveTurn() {
//		finishedResolve = false;
//		yield return new WaitUntil (() => finishedBattlePhase);
//        // Do stuff once previous phase is finished
//        Debug.Log("=== Resolving Turn ===");
//        currentUnit.GetComponent<Character>().resolveStatusAfflictions();
//        finishedResolve = true;
//	}

    

    /*** 
	 * Methods that determine poll rate of inputs 
     ***/
//    private IEnumerator WaitForButtonInput() {
//		buttonIsPressed = false;
////		StartCoroutine(this.buttonResetRoutine);
//		buttonResetRoutine = StartCoroutine (WaitForButtonReset ());
//		yield return new WaitUntil(() => (buttonIsPressed && buttonCanBePressed));
//		switch(currentButton) {
//			case Button.Fire2:
//				Debug.Log ("Fire2");
//				finishedUnitAction = true;
//				break;
//			case Button.Submit:
//				Debug.Log ("Submit");
//				finishedUnitAction = true;
//				//this.finishedActing = true;
//				break;
//			case Button.Jump:
//				Debug.Log ("Jump");
//	//			this.finishedUnitAction = true;
//				finishedUnitAction = true;
//				finishedActing = true;
//				break;
//			case Button.Cancel:
//				Debug.Log ("Cancel");
//				finishedUnitAction = true;
//				break;
//			default:
//				Debug.Log("uh");
//				break;
//		}
//	}
//
//    private IEnumerator WaitForButtonReset() {
//		if(buttonCanBePressed) {
//			buttonCanBePressed = false;
//			yield return new WaitForSecondsRealtime(0.3f);
//			buttonIsPressed = false;
//			buttonCanBePressed = true;
//		}
//		yield return new WaitUntil (() => Input.GetButtonDown ("Fire2") || Input.GetButtonDown ("Jump") || Input.GetButtonDown ("Submit") || Input.GetButtonDown("Cancel"));
//		if(Input.GetButtonDown ("Fire2")) {
//			currentButton = Button.Fire2;
//		} else if(Input.GetButtonDown ("Submit")) {
//			currentButton = Button.Submit;
//		} else if(Input.GetButtonDown("Jump")) {
//			currentButton = Button.Jump;
//		} else if(Input.GetButtonDown("Cancel")) {
//			currentButton = Button.Cancel;
//		}
//		buttonIsPressed = true;
//	}
//
//	protected IEnumerator WaitForDirection() {
//		directionIsPressed = false;
////		StartCoroutine(this.directionResetRoutine);
//		directionResetRoutine = StartCoroutine (WaitForDirectionReset ()); 
//		yield return new WaitUntil(() => (directionIsPressed && directionCanBePressed));
//		switch(currentDirection) {
//			case Direction.Up:
//				Debug.Log ("Up");
//				testMenu.TraverseOptions ((int) currentDirection);
//				break;
//			case Direction.Down:
//				Debug.Log ("Down");
//				testMenu.TraverseOptions ((int) currentDirection);
//				break;
//			case Direction.Left:
//				Debug.Log ("Left");
//				testMenu.TraverseOptions ((int) currentDirection);
//				break;
//			case Direction.Right:
//				Debug.Log ("Right");
//				testMenu.TraverseOptions ((int) currentDirection);
//				break;
//			default:
//				throw new ArgumentOutOfRangeException();
//		}
////		Debug.Log ("Current option: " + this.currentOption);
////		resetButtonInputs ();
//		finishedUnitAction = true;
//	}

//	private IEnumerator WaitForDirectionReset() {
//		if(directionCanBePressed) {
//			directionCanBePressed = false;
//			yield return new WaitForSecondsRealtime(0.3f);
//			yield return new WaitUntil (() => (int)Input.GetAxisRaw ("Horizontal") == 0);
//			directionIsPressed = false;
//			directionCanBePressed = true;
//		}
//		yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") > 0 || Input.GetAxisRaw ("Horizontal") < 0
//			|| Input.GetAxisRaw ("Vertical") > 0 || Input.GetAxisRaw ("Vertical") < 0);
////		yield return new WaitUntil (() => Input.GetAxisRaw ("Horizontal") > 0 || Input.GetAxisRaw ("Horizontal") < 0);
//		if(Input.GetAxisRaw ("Horizontal") > 0) {
//			currentDirection = Direction.Right;
//		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
//			currentDirection = Direction.Left;
//		} else if(Input.GetAxisRaw ("Vertical") > 0) {
//			currentDirection = Direction.Up;
//		} else if(Input.GetAxisRaw ("Vertical") < 0) {
//			currentDirection = Direction.Down;
//		}
//		directionIsPressed = true;
//	}

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

	private void ResetButtonInputs() {
		StopCoroutine(buttonInputRoutine);
		StopCoroutine(buttonResetRoutine);
	}

	private void ResetDirectionInputs() {
		StopCoroutine(directionRoutine);
		StopCoroutine(directionResetRoutine);
	}

	private void TryResetUnitInputs() {
		if(buttonInputRoutine != null && buttonResetRoutine != null
			&& directionRoutine != null && directionResetRoutine != null) {
			ResetUnitInputs ();
		} else {
			Debug.Log ("Most likely the battle just started. No routines to clean at this time.");
		}
	}

	private void ResetUnitInputs() {
		StopCoroutine(buttonInputRoutine);
		StopCoroutine(buttonResetRoutine);
		StopCoroutine(directionRoutine);
		StopCoroutine(directionResetRoutine);
	}

	private void StopAllCoroutines() {
		StopCoroutine(actionRoutine);
		StopCoroutine(battleRoutine);
		StopCoroutine(resolveRoutine);
		StopCoroutine(buttonInputRoutine);
		StopCoroutine(buttonResetRoutine);
		StopCoroutine(directionRoutine);
		StopCoroutine(directionResetRoutine);
		StopCoroutine(startTurnRoutine);
	}
	
	
	private void TestMultiAxisMenu() {
		const int width = 6;
		const int height = 10;
		testMenu = new MenuGraph<int> (width, height, MenuGraph<int>.Type.Both);
		testMenu.initTestMenu (new int[width*height]);
//		this.testMenu.initMATestMenu (new int[width, height]);
	}

	private void TestSingleAxisMenu() {
		const int testSize = 5;
		testMenu = new MenuGraph<int> (testSize, MenuGraph<int>.Type.Horizontal);
		testMenu.initTestMenu (new int[testSize]);
//		this.testMenu.initSATestMenu (new int[testSize]);
	}
}
