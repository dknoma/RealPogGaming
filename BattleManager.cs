using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	
	public static BattleManager bm;
	
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
	private bool inCutscene;

	private bool inBattle;
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

	private bool axisDown;

	private ActionMenu testMenu;	// Get options from units Character component
	//private int currentOption = 0;

	private Direction currentDirection;
	private Button currentButton;

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
		party.InitPartyMembers ();
	}

	private void Start() {
		testMenu = GameObject.FindGameObjectWithTag("TestMenu").GetComponent<ActionMenu>();
	}


	private void Update() {
		if (Input.GetButtonDown("Fire1") && BattleState == BattleState.Nil) {
			BattleState = BattleState.Init;
		} else if (Input.GetButtonDown("Test2")) {
			
		}
		CheckBattleState();
	}

	private void CheckBattleState() {
		switch (BattleState) {
			case BattleState.Nil:
				// Not in battle.
				break;
			case BattleState.Init:
				inBattle = true;
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
						Debug.Log("Escaped battle successfully.");
						break;
					default:
						throw new ArgumentOutOfRangeException("WinStatus", WinStatus, null);
				}
				inBattle = false;
				expToGive = 0;
				units = new List<GameObject>();
				allies = new List<GameObject>();
				enemies = new List<GameObject>();
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
		if (!inCutscene) {
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
					
//					directionRoutine = StartCoroutine (WaitForDirection ());
					GetAxisDown();
					NavigateActionMennu();
					
					if (Input.GetButtonDown("Test") && testMenu.CurrentAction.CompareTag("AttackAction")) {
						BattlePhase = BattlePhase.Battle;
					} else if (Input.GetButtonDown("Test") && testMenu.CurrentAction.CompareTag("RunAction")) {
						EndBattle(WinStatus.Escape);
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
					BattlePhase = BattlePhase.Resolution;
//					if (Input.GetButtonDown("Test")) {
//						Debug.Log("Ending battle phase...");
//					}
					break;
				case BattlePhase.Resolution:
					// TODO: resolve end of turn effects: do DoT, decrement status counters
					if (!resolvingTurn) {
						resolvingTurn = true;
						currentUnit.GetComponent<Character>().ResolveStatusAfflictions();
					}
					BattlePhase = BattlePhase.Nil;
					TurnState = TurnState.End; // End turn
//					if (Input.GetButtonDown("Test")) {
//						Debug.Log("Ending resolution phase...");
//					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		} else {
			Debug.Log("In cutscene.");
		}
	}

	private void NavigateActionMennu() {
		switch(currentDirection) {
			case Direction.Up:
//				Debug.Log ("Up");
				testMenu.NavigateMenu(currentDirection);
				break;
			case Direction.Down:
//				Debug.Log ("Down");
				testMenu.NavigateMenu(currentDirection);
				break;
			case Direction.Left:
//				Debug.Log ("Left");
				testMenu.NavigateMenu(currentDirection);
				break;
			case Direction.Right:
//				Debug.Log ("Right");
				testMenu.NavigateMenu(currentDirection);
				break;
			default:
//				Debug.Log ("no move");
				break;
		}
	}

	private void GetAxisDown() {
		if (axisDown) {
			currentDirection = Direction.Null;
			if (!(Math.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) ||
			    !(Math.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon)) return;
//			Debug.Log("Reset axis down");
			axisDown = false;

			return;
		}
		if(Input.GetAxisRaw ("Horizontal") > 0) {
//			Debug.Log ("Right");
			axisDown = true;
			currentDirection = Direction.Right;
		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
//			Debug.Log ("Left");
			axisDown = true;
			currentDirection = Direction.Left;
		} else if(Input.GetAxisRaw ("Vertical") > 0) {
			axisDown = true;
			currentDirection = Direction.Up;
		} else if(Input.GetAxisRaw ("Vertical") < 0) {
			axisDown = true;
			currentDirection = Direction.Down;
		} else {
//			Debug.Log("Reset axis down");
			axisDown = false;
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
		Character unit = currentUnit.GetComponent<Character>();
		Debug.Log("============= Begin Turn =============");
		Debug.Log(string.Format("Name: {0}, HP: {1}, exp until level up: {2}",
		                        currentUnit.name, unit.GetCurrentHP(), unit.expUntilLevelUp));
		BattlePhase = BattlePhase.Status;	// The start of the units turn
		TurnState = TurnState.Ongoing;		// Go ahead and start the turns
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

	public bool InBattle() {
		return inBattle;
	}
	
	/***************************
	 * 						   *
	 * Party related functions *
	 * 						   *
	 ***************************/ 
	private void AddPartyToList() {
		// Get party members from sibling component
		List<GameObject> partyMembers = party.GetPartyMembers();
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
				Debug.Log(string.Format("ending the battleeeeeeeeeeeee"));
				enemies.Remove(target);
				expToGive += target.GetComponent<Character>().expToGrant;
				if(enemies.Count == 0) {
					EndBattle(WinStatus.Win);
				}
			}
		}
		return true;
	}
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
			// Standard atk-def calc
			(Mathf.Pow(source.GetCurrentAtk(), 2) + source.GetRuneAtk()) / (target.GetCurrentDef() + target.GetRuneDef()) 								
			// Level compensation
			* (1 + (source.currentLevel*2 - target.currentLevel) / 50)			
			// Elemental multiplier
			* ElementalAffinity.CalcElementalDamage(source.getCurrentBattleActions().getElement(),	
				target.element));  														
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
}
