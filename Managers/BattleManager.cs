using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
	Loading,
	Ongoing,
	End,
	Unloading,
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
	public static GameObject battleCanvas;
	
	private static int _turnCount;

	private BattleState BattleState { get; set; }
	private TurnState TurnState { get; set; }
	private BattlePhase BattlePhase { get; set; }
	private WinStatus WinStatus { get; set; }

	private ActionType currentActionType;

//	[SerializeField] private GameObject actionMenu;
//	private GameObject _actionMenu;
//	[SerializeField] private GameObject battleCanvas;
	
	// Queue used for determining which unit goes when.
	private Queue<Character> turnQueue = new Queue<Character>();
	private Queue<Character> turnQueueUi = new Queue<Character>();
	private Party party;
	// List of units to be part of the battle
	private List<GameObject> units = new List<GameObject>();
	private List<GameObject> enemies = new List<GameObject>();
	private Character currentUnit;
	private GameObject currentTarget;
	private GameObject currentUnitIcon;
	
	private int allyCount;
	private int enemyCount;
	private int numUnits;
	private int expToGive;
	private bool fastestFirst = true;
//	private bool currentIsAlly = true;

	private bool isLoadingBattle;
	private bool takingTurn;
	private bool calculatingBattle;
	private bool resolvingTurn;
	private bool inCutscene;
	private bool menuActivated;
	private bool isLoading;
	private bool unloading;
	private bool removingBattleObjects;

	private bool inBattle;
	private bool directionIsPressed;
//	private bool directionCanBePressed = true;
	private bool buttonIsPressed;
//	private bool buttonCanBePressed = true;
//	private bool finishedActing;
	private bool finishedUnitAction;

	private bool finishedStatusPhase;
	private bool finishedActionPhase;
	private bool finishedBattlePhase;
	private bool finishedResolve;
	private bool finishedTurn;

	private const int MAX_QUEUE_SIZE = 8;
	private const int MAX_QUEUE_ICONS = MAX_QUEUE_SIZE / 2;
	
//	private Dictionary <string, UnityEvent> eventDictionary;
	private readonly UnityEvent endBattle = new UnityEvent();
	private readonly UnityEvent defeatedEnemyEvent = new UnityEvent();
	
//	private readonly UnityEvent buffEvent = new UnityEvent();
//	private bool axisDown;
//	private Direction currentDirection;
//	private Button currentButton;
//	private GameObject testMenu;
//	private ActionMenu actionMenu;	// Get options from units Character component
	//private int currentOption = 0;

	private void OnEnable() {
		if(bm == null) {
			bm = this;
		} else if(bm != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		BattleState = BattleState.Nil;
		TurnState = TurnState.Nil;
		BattlePhase = BattlePhase.Nil;
		WinStatus = WinStatus.Nil;
		battleCanvas = GameObject.FindGameObjectWithTag("BattleCanvas");
	}

	private void Update() {
		CheckBattleState();
	}

	public void AddEndBattleListener(UnityAction call) {
		endBattle.AddListener(call);
	}

	public void AddDefeatEnemyEvent(UnityAction call) {
		defeatedEnemyEvent.AddListener(call);
	}

	private void CheckBattleState() {
		switch(BattleState) {
			case BattleState.Nil:
				// Not in battle.
//				inBattle = false;
//				unloading = false;
				break;
			case BattleState.Init:
				inBattle = true;
				unloading = false;
				removingBattleObjects = false;
				if(!isLoadingBattle) {
					isLoadingBattle = true;
					InitBattle();
				}
				break;
			case BattleState.Loading:
				// Load stuff when transitioning
				LoadBattleData();
				break;
			case BattleState.Ongoing:
				CheckTurnState();
				break;
			case BattleState.End:
				endBattle.Invoke();
				isLoading = false;
				isLoadingBattle = false;
				TurnState = TurnState.Nil;
				BattlePhase = BattlePhase.Nil;
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
				expToGive = 0;
				Debug.Log("Ending battle.");
				BattleState = BattleState.Unloading;
				break;
			case BattleState.Unloading:
				if(!unloading) {
					unloading = true;
					ScreenTransitionManager.screenTransitionManager.DoScreenTransition(Transition.Fade, 0.5f);
				}
				if(unloading) {
					if(!removingBattleObjects &&
					    !ScreenTransitionManager.screenTransitionManager.IsTransitioningToBlack()) {
						removingBattleObjects = true;
						RemoveEnemiesFromBattle();
						BattleBackgroundManager.bbm.ShowBackground(false);
						BattleScene.battleScene.ResetEnemyPositions();
//						Destroy(_actionMenu);	// Disable ui w/ UIManager
						UIManager.um.DisableBattleUI();
						if(WinStatus == WinStatus.Win) {
							AreaEnemyManager.aem.DoDefeatEnemyAnimation();
//							defeatedEnemyEvent.Invoke();
						}
					}
					if(!ScreenTransitionManager.screenTransitionManager.IsTransitioning()) {
						units = new List<GameObject>();
						enemies = new List<GameObject>();
						turnQueue.Clear();
						BattleState = BattleState.Nil;
						inBattle = false;
					}
				}

				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	private void CheckTurnState() {
		switch(TurnState) {
			case TurnState.Nil:
				// Not in battle.
				break;
			case TurnState.Init:
				if(turnQueue.Count <= MAX_QUEUE_ICONS) {
					CalculatePriority(fastestFirst);
				}
				if(!takingTurn) {
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
		if(!inCutscene) {
			switch(BattlePhase) {
				case BattlePhase.Nil:
//					finishedActing = false;
					break;
				case BattlePhase.Status:
//					finishedActing = false;
					menuActivated = false;
					// check what actions can take if any
//					Debug.Log("Checking" + currentUnit.name + "'s status.");
					currentUnit.CheckStatusAfflictions();
					if(!currentUnit.CanCharacterAct()) {
						// If incapacitated, skip turn
						Debug.Log(string.Format("============= Begin Turn =============\nSkipping {0}'s turn.",
						                        currentUnit.name));
						SetBattlePhase(BattlePhase.Resolution);
					} else {
						Debug.Log(string.Format("============= Begin Turn =============\nName: {0}, HP: {1}, exp until level up: {2}",
						                        currentUnit.name, currentUnit.GetCurrentHp(), currentUnit.expUntilLevelUp));
						BattlePhase = BattlePhase.Action;
					}
					break;
				case BattlePhase.Action:
					// TODO []: let unit do their actions: attack, use items, run away
					// TODO [x]: have a bool that will change from the action menu depending on the action
					if(!menuActivated) {
						menuActivated = true;
						currentUnit.AddActionListener(MoveToBattlePhase);
						switch (GetCurrentUnit().GetAffiliation()) {
							case Affiliation.Ally:
								UIManager.um.InitActions(); // Spawn buttons after done transitioning
								break;
							case Affiliation.Enemy:
								Debug.Log("Current unit is an enemy.");
								List<Player> allies = PlayerManager.pm.GetParty();
								int randomTarget = Random.Range(0, allies.Count);
								Player target = allies[randomTarget];
								while (target.IsIncapacitated()) {
									randomTarget = Random.Range(0, allies.Count);
									target = allies[randomTarget];
								}
								Debug.LogFormat("Chose ally to attack.");
								currentUnit.DoAttackA(target.gameObject);
//								SetCurrentTarget(target.gameObject);
//								SetCurrentActionType(ActionType.Attack);
//								BattlePhase = BattlePhase.Battle;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
//						UIManager.um.InitActions();	// Spawn buttons after done transitioning
//						actionMenu.gameObject.SetActive(true);
//						actionMenu.TryInitMenu();
					}

//					if(finishedActing) {
//						BattlePhase = BattlePhase.Battle;
//					}
					break;
				case BattlePhase.Battle:
					// TODO: resolve the unit's action: attacking(action commands for bonus dmg), healing(possible action
					//		 commands here as well.), item useage, Calculate damage/heals, running away.
					if(!calculatingBattle) {
						calculatingBattle = true;
						currentUnit.RemoveActionListener(MoveToBattlePhase);
						BattlePhase = BattlePhase.Resolution;
//						switch(currentActionType) {
//							case ActionType.Attack:
////								GameObject target = currentUnit.CompareTag("EnemyPrefab") ? party.frontUnit : enemies[0];
////								int dmg = CalculateDamage(currentUnit, target);
////								TryDamage(dmg, target);
//								int dmg = CalculateDamage(currentUnit, currentTarget);
//								TryDamage(dmg, currentTarget);
//								break;
//							case ActionType.Heal:
//								break;
//							case ActionType.Defend:
//								break;
//							case ActionType.Escape:
//								EndBattle(WinStatus.Escape);
//								break;
//							case ActionType.Revive:
//								break;
//							case ActionType.Buff:
//								buffEvent.AddListener(currentUnit.BuffAtk);
//								buffEvent.Invoke();
//								buffEvent.RemoveListener(currentUnit.BuffAtk);
//								break;
//							default:
//								throw new ArgumentOutOfRangeException();
//						}
					}
					break;
				case BattlePhase.Resolution:
					// TODO: resolve end of turn effects: do DoT, decrement status counters
					if(!resolvingTurn) {
						resolvingTurn = true;
						currentUnit.GetComponent<Character>().ResolveStatusAfflictions();
						currentUnit.GetComponent<Character>().ResolveStatChanges();
					}
//					actionMenu.gameObject.SetActive(false);
					BattlePhase = BattlePhase.Nil;
					TurnState = TurnState.End; // End turn
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		} else {
			Debug.Log("In cutscene.");
		}
	}

	private void MoveToBattlePhase() {
		BattlePhase = BattlePhase.Battle;
	}
	
//	private static IEnumerator TransitionScreen() {
//		ScreenTransitionManager.screenTransitionManager.TransitionToBlack(Transition.Sawtooth);
//		yield return new WaitUntil(() => !ScreenTransitionManager.screenTransitionManager.IsTransitioning());
//		yield return new WaitForSeconds(1);
//		ScreenTransitionManager.screenTransitionManager.TransitionFromBlack(Transition.Sawtooth);
//	}

	// Use this for initialization
	private void InitBattle() {
		ScreenTransitionManager.screenTransitionManager.DoScreenTransition(Transition.Triangle);
		// Initialize battle settings
		_turnCount = 0;
		units = new List<GameObject>();
		enemies = new List<GameObject>();
		turnQueue = new Queue<Character>();
		// Init each characters rune bonuses
		foreach(GameObject unit in units) {
			CalculateRuneStats(unit.GetComponent<Character>());
		}
		numUnits = units.Count;
		Debug.Log("number of units: " + numUnits);
//		if(_actionMenu == null) {
//			_actionMenu = Instantiate(actionMenu, battleCanvas.transform, false);
////			battleCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
////			battleCanvas.worldCamera = Camera.main;
////			actionMenu = battleCanvas.GetComponentInChildren<ActionMenu>();
//		}
//		actionMenu.gameObject.SetActive(false);
		BattleState = BattleState.Loading;	// Go ahead and start the battle
//		testMenu.SetActive(true);
	}

	private void LoadBattleData() {
		if(!isLoading && !ScreenTransitionManager.screenTransitionManager.IsTransitioningToBlack()) {
			isLoading = true;
			// Add all units involved in the battle to the list
			AddPartyToList();
			AddEnemiesToList();
			BattleBackgroundManager.bbm.SetArea(WorldArea.PaltryPlains); // testing
			BattleBackgroundManager.bbm.LoadBackground();
			UIManager.um.InitBattleUI();
		}
		if(isLoading && !ScreenTransitionManager.screenTransitionManager.IsTransitioning()) {
			TurnState = TurnState.Init; // Turn initialization: calculate turn order, etc.
			BattleState = BattleState.Ongoing;
		}
	}
	 
	private void StartTurn() {
		// Keep this round going until all units finish their turns
		takingTurn = true;
		_turnCount++;
//		Debug.Log("Queue size " + turnQueue.Count);
//		Debug.Log("Turn " + _turnCount);
		currentUnit = turnQueue.Dequeue();
		// TODO: can insert check here for possible cutscenes during a battle.
//		Character unit = currentUnit;
//		actionMenu.gameObject.SetActive(true);
//		actionMenu.TryInitMenu();
		BattlePhase = BattlePhase.Status;	// The start of the units turn
		TurnState = TurnState.Ongoing;		// Go ahead and start the turns
	}
	
	/**************************************************
	 **************************************************
	 **************************************************/
	public void SetCurrentActionType(ActionType actionType) {
		currentActionType = actionType;
	}
	
//	public void FinishAction() {
//		finishedActing = true;
//	}
	
	public void EndBattle(WinStatus winStatus) {
		turnQueue.Clear();
		WinStatus = winStatus;
		BattleState = BattleState.End;
		BattlePhase = BattlePhase.Nil;
		TurnState = TurnState.Nil;
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
		List<Player> partyMembers = PlayerManager.pm.GetParty();
		foreach(Player member in partyMembers) {
			Debug.LogFormat("member readiness: {0}".Remove(member.Readiness));
			member.AddIncapacitatedListener(TargetDefeated);
			units.Add(member.gameObject);
		}
	}

	private int GenerateRandomEnemyCount() {
		return Random.Range(1, 5);	// random number from 1-4
	}

	private void AddEnemiesToList() {
		// TODO: create enemy manager; takes care of instantiating enemies in a battle, etc
//		GameObject enemyPool = GameObject.FindGameObjectWithTag("EnemyPool");
//		int enemyCount = enemyPool.transform.childCount;
		BattleScene.battleScene.InitBattleScene();
		int generatedEnemyCount = GenerateRandomEnemyCount();
		BattleScene.battleScene.UpdateEnemyPositions(generatedEnemyCount);
		for(int i = 0; i < generatedEnemyCount; i++) {
			GameObject enemyToInstantiate = BattleSceneDataManager.bsdm.GetEnemyPrefab("Enemy1 - Battle");
			Vector3 pos = BattleScene.battleScene.GetEnemyPosition(i);
//			Debug.LogFormat("Pos for enemy {0}", pos);
			GameObject newEnemy = Instantiate(enemyToInstantiate, BattleScene.battleScene.gameObject.transform);
			newEnemy.transform.position = pos;
			units.Add(newEnemy);
			enemies.Add(newEnemy);
			Enemy enemy = newEnemy.GetComponent<Enemy>();
			enemy.AddIncapacitatedListener(TargetDefeated);
		}
//		for(int i = 0; i < enemyCount; i++) {
//			GameObject enemy = enemyPool.transform.GetChild(i).gameObject;
//			units.Add(enemy);
//			enemies.Add(enemy);
//		}
	}

	private void RemoveEnemiesFromBattle() {
		if(enemies.Count == 0 || enemies == null) return;
		foreach(GameObject enemy in enemies) {
			Destroy(enemy);
		}
	}

	/***************************
	 * 						   *
	 *  Dmg related functions  *
	 * 						   *
	 ***************************/ 
//	private bool TryDamage(int dmg, GameObject target) {
//		// TODO: Check if target can be damaged.
//		Debug.Log(string.Format("\t{0} dealt {1} damage to {2}", currentUnit.name, dmg, target.name));
//		Character unit = target.GetComponent<Character>();
//		unit.ModifyCurrentHp(-dmg);
//		
//		// If target is an ally, update their hp bars/numbers
////		if (target.GetComponent<Player>() != null) {
////			Player player = target.GetComponent<Player>();
////			PlayerStatBar statBar = UIManager.um.GetPlayerStatBar(player.slot);
////			float hpPercent = (0.000000f+player.GetCurrentHp()) / (0.000000f + player.GetMaxHp());
////			Debug.LogFormat("percent: {0}", hpPercent);
////			statBar.UpdateHpBar(hpPercent);
////		}
//		
//		Debug.Log(string.Format("{0}'s HP: {1}", target.name, unit.GetCurrentHp()));
//		// If HP is still positive, return
//		if (unit.GetCurrentHp() > 0) return true;
////		unit.TryIncapacitate();	// Incapacitate the unit. Cannot act until revived
////		units.Remove(target); // Remove unit from the list if no more HP
////		RemoveTargetFromQueue(unit);
//		Debug.LogFormat("contains {0}: {1}", target, units.Contains(target));
//		// TODO: if party member, make incapacitated: can be revived
//		if(target.CompareTag("Enemy")) {
//			RemoveTargetFromQueue(unit);	// Remove enemy from queue, does not perform any more actions
//			enemies.Remove(target);
//			target.SetActive(false);
//			expToGive += unit.expToGrant;
//			if(enemies.Count == 0) {
//				EndBattle(WinStatus.Win);
//			}
//		}
//		if(target.CompareTag("Ally")) {
////			Player player = target.GetComponent<Player>();
////			PlayerStatBar statBar = UIManager.um.GetPlayerStatBar(player.slot);
////			statBar.UpdateHpBar(0);
////			PlayerManager.pm.IncapacitateUnit(unit.GetComponent<Player>());
//
//			// Allies do not get removed from queue in case their turn comes around when revived
//			if(PlayerManager.pm.AllAlliesIncapacitated()) {
//				EndBattle(WinStatus.Lose);
//			}
//		}
//		return true;
//	}

	/// <summary>
	/// Response method to the current target's incapacitated event.
	/// </summary>
	public void TargetDefeated(GameObject defeatedUnit) {	
		Debug.Log(string.Format("\t{0} has been defeated.", defeatedUnit.name));
		Character unit = defeatedUnit.GetComponent<Character>();
		Debug.LogFormat("Units contains {0}: {1}", defeatedUnit, units.Contains(defeatedUnit));
		// TODO: if party member, make incapacitated: can be revived
		Affiliation targetAffiliation = unit.GetAffiliation();
		switch (targetAffiliation) {
			case Affiliation.Ally:
				// Allies do not get removed from queue in case their turn comes around when revived
				if(PlayerManager.pm.AllAlliesIncapacitated()) {
					EndBattle(WinStatus.Lose);
				}
				break;
			case Affiliation.Enemy:
				RemoveTargetFromQueue(unit); // Remove enemy from queue, does not perform any more actions
				enemies.Remove(defeatedUnit);
				defeatedUnit.SetActive(false);
				expToGive += unit.expToGrant;
				if(enemies.Count == 0) {
					EndBattle(WinStatus.Win);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	private void RemoveTargetFromQueue(Character target) {
		List<Character> unitListFromQueue = turnQueue.ToList();
		unitListFromQueue.RemoveAll(unit => unit == target);
		turnQueue = new Queue<Character>(unitListFromQueue);
//		foreach (Character unit in unitListFromQueue) {
//		}
	}
	
//	private int CalculateDamage(Character src, GameObject dest) {
//		Character 
//		source = src,
//		target = dest.GetComponent<Character>();
////		Debug.Log(string.Format("{0}'s atk: {1}, {2}'s def: {3}", 
////			source.name, source.GetCurrentAtk(), target.name, target.GetCurrentDef()));
////		Debug.Log(string.Format("{0}'s attack element: {1}, {2} element: {3}", 
////			source.name, source.GetAttackElement(), target.name, target.element));
//		// Calculate the current units rune bonuses
//		CalculateRuneStats(src);
//		int damage = Mathf.RoundToInt(
//			// Standard atk-def calc
//			(Mathf.Pow(source.GetCurrentAtk(), 2) + source.GetRuneAtk()) /(target.GetCurrentDef() + target.GetRuneDef()) 								
//			// Level compensation
//			*(1 +(source.currentLevel*2 - target.currentLevel) / 50)			
//			// Elemental multiplier
//			* ElementalAffinity.CalcElementalDamage(source.GetAttackElement(),	
//				target.element));  														
//		return damage;
//	}
	
	private void GrantExpToParty() {
		foreach(Player ally in PlayerManager.pm.GetParty()) {
			ally.GrantExp(expToGive);
		}
	}

	/*
	 * Sort the characters by their speeds, then enqueue them into the action queue.
	 */
//	private void CalculatePriority(bool byFastestFirst) {
//		// Sor the units based on speed
//		if(byFastestFirst) {
//			Sorting.DescendingMergeSort(units, new CompareCharactersBySpeed());
//		} else {
//			Sorting.MergeSort(units, new CompareCharactersBySpeed());
//		}
//		// Add unit to the queue
//		for (int i = 0; i < units.Count; i++) {
//			Character character = units[i].GetComponent<Character>();
//			turnQueue.Enqueue(character);
//			turnQueueUi.Enqueue(character);
//		}
//	}

	private void CalculatePriority(bool byFastestFirst) {
		Sorting.DescendingMergeSort(units, new CompareCharactersBySpeed());
//		Queue<Character> temp = new Queue<Character>();
		while (turnQueue.Count < MAX_QUEUE_SIZE) {			// Queue up to 100 actions
			foreach (GameObject unit in units) {
				Character character = unit.GetComponent<Character>();
				// TODO: This can allow players to move more than twice a turn, 20x the spd means can move 20 times
				// 			Tho, at this point, the player would be able to kill enemies in one hit.
				character.IncrementReadiness(character.GetCurrentSpd(), byFastestFirst);
				if (!character.Ready) continue; // If character is ready, add to queue and reset readiness
				turnQueue.Enqueue(character);
				character.ResetReadiness();
				if (turnQueue.Count >= MAX_QUEUE_SIZE) break; // Break out if reach threshold
			}
		}
		Debug.LogFormat("Queued up units: {0}",turnQueue.Count);
	}

//	private void InitQueueIcons() {
//		int i = 0;
//		foreach (GameObject unit in units) {
//			Character character = unit.GetComponent<Character>();
//			Debug.LogFormat("|---> {0}", character.name);
//			turnQueueUi.Enqueue(character);
//			if (i == MAX_QUEUE_ICONS) {
//				
//			}
//			i++;
//		}
//	}

	private void CalculateRuneStats(Character currentUnit) {
//		RuneSlots slots = currentUnit.GetComponent<RuneSlots>();
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
	/**
	 * GETSET
	 */
	
	public int EnemyCount() {
		return enemyCount;
	}
	
	public Character GetCurrentUnit() {
		return currentUnit;
	}
	
	public void SetCurrentTarget(GameObject target) {
		currentTarget = target;
	}
	
	public GameObject GetCurrentTarget() {
		return currentTarget;
	}

	
	public List<GameObject> GetEnemies() {
		return enemies;
	}
	
	public BattleState GetBattleState() {
		return BattleState;
	}
	
	public void SetBattleState(BattleState state) {
		BattleState = state;
	}
	
	public BattlePhase GetBattlePhase() {
		return BattlePhase;
	}
	
	public void SetBattlePhase(BattlePhase phase) {
		BattlePhase = phase;
	}
}
