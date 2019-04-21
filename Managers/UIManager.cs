using System;
using System.Collections.Generic;
using BattleUI;
using Characters;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public enum ActionType {
	Attack,
	Heal,
	Revive,
	Buff,
	Defend,
	Escape
}
public class UIManager : MonoBehaviour {

	public static UIManager um;
	
	[SerializeField]
	private GameObject battleUI;
//    private static GameObject _battleUI;

	[SerializeField]
	private GameObject mainActionMenuPrefab;
	private static GameObject _mainActionMenu;
	
	[SerializeField]
	private GameObject attackMenuPrefab;
	private static GameObject _attackMenu;
	
	[SerializeField]
	private GameObject targetContainerPrefab;
	private static GameObject _targetContainer;
	
	[SerializeField]
	private GameObject targetButtonPrefab;
	
	private static EventSystem eventSystem = EventSystem.current;
	
	private readonly Stack<GameObject> previousOptions = new Stack<GameObject>();
	private readonly Dictionary<CharacterSlot, PlayerStatBar> playerStatBars = new Dictionary<CharacterSlot, PlayerStatBar>();


	// Player stats
	[SerializeField]
	private GameObject playerOneBarLocation;
	[SerializeField]
	private GameObject playerOneStatusBarsPrefab;
	private static GameObject _playerOneStatusBars;
	
	
	[SerializeField]
	private GameObject playerTwoBarLocation;
	[SerializeField]
	private GameObject playerTwoStatusBarsPrefab;
	private static GameObject _playerTwoStatusBars;
	
	
	[SerializeField]
	private GameObject playerThreeBarLocation;
	[SerializeField]
	private GameObject playerThreeStatusBarsPrefab;
	private static GameObject _playerThreeStatusBars;

	private Vector3 pOneLocation;
	private Vector3 pTwoLocation;
	private Vector3 pThreeLocation;
	
	
	private void Awake() {
		if (um == null) {
			um = this;
		} else if (um != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		eventSystem = EventSystem.current;
		pOneLocation = playerOneBarLocation.transform.position;
		pTwoLocation = playerTwoBarLocation.transform.position;
		pThreeLocation = playerThreeBarLocation.transform.position;
	}

	public void InitBattleUI() {
		if (!BattleManager.bm.InBattle()) return; // If not in battle, should not bring up battle UI
		Vector3 cameraPos = Camera.main.transform.position;
		battleUI.transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);
		battleUI.SetActive(true);
		int partyCount = PlayerManager.pm.GetPartyCount();
		Debug.LogFormat("Party count={0}", partyCount);
		if (_playerOneStatusBars == null) {
			_playerOneStatusBars = Instantiate(playerOneStatusBarsPrefab, battleUI.transform, false);
			_playerOneStatusBars.transform.localPosition = pOneLocation;
			PlayerStatBar statBar = _playerOneStatusBars.GetComponent<PlayerStatBar>();
			BattleEventManager.bem.ListenOnPlayerStats(statBar);	// Listen in on the player's stats whos slot matches the bars.
			playerStatBars.Add(CharacterSlot.One, statBar);
		}
		switch (partyCount) {
			case 1:
				break;
			case 2:
				if (_playerTwoStatusBars == null) {
					_playerTwoStatusBars = Instantiate(playerTwoStatusBarsPrefab, battleUI.transform, false);
					_playerTwoStatusBars.transform.localPosition = pTwoLocation;
					PlayerStatBar statBar = _playerTwoStatusBars.GetComponent<PlayerStatBar>();
					BattleEventManager.bem.ListenOnPlayerStats(statBar); // Listen in on the player's stats whos slot matches the bars.
					playerStatBars.Add(CharacterSlot.Two, statBar);
				}
				break;
			case 3:
				if (_playerTwoStatusBars == null) {
					_playerTwoStatusBars = Instantiate(playerTwoStatusBarsPrefab, battleUI.transform, false);
					_playerTwoStatusBars.transform.localPosition = pTwoLocation;
					PlayerStatBar statBar = _playerTwoStatusBars.GetComponent<PlayerStatBar>();
					BattleEventManager.bem.ListenOnPlayerStats(statBar); // Listen in on the player's stats whos slot matches the bars.
					playerStatBars.Add(CharacterSlot.Two, statBar);
				}
				if (_playerThreeStatusBars == null) {
					_playerThreeStatusBars = Instantiate(playerThreeStatusBarsPrefab, battleUI.transform, false);
					_playerThreeStatusBars.transform.localPosition = pThreeLocation;
					PlayerStatBar statBar = _playerThreeStatusBars.GetComponent<PlayerStatBar>();
					BattleEventManager.bem.ListenOnPlayerStats(statBar); // Listen in on the player's stats whos slot matches the bars.
					playerStatBars.Add(CharacterSlot.Three, statBar);
				}
				break;
			default:
				Debug.LogFormat("Invalid amount of party members, must be 1 <= count <= 3");
				break;
		}
		if (_mainActionMenu == null) {
			_mainActionMenu = Instantiate(mainActionMenuPrefab, BattleManager.battleCanvas.transform, false);
		} else {
			_mainActionMenu.SetActive(true);
		}
	}
	
	public void InitActions() {
		if (!BattleManager.bm.InBattle()) return;	// If not in battle, should not bring up battle UI
		if (eventSystem == null) {
			eventSystem = EventSystem.current;
		}
		SetMenuActive(_mainActionMenu,true);
		SetButtonsInteractable(_mainActionMenu, true);
		eventSystem.SetSelectedGameObject(GetDefaultMainAction());
//		if (!BattleManager.bm.InBattle()) return;
//		switch (BattleManager.bm.GetCurrentUnit().GetAffiliation()) {
//			case Affiliation.Ally:
//				Debug.Log("Current unit is an ally.");
//                if (_actionMenu == null) {
//                    _actionMenu = Instantiate(actionMenuPrefab, BattleManager.battleCanvas.transform, false);
//                }
//				Debug.Log("\t\tcurrentSelected " + eventSystem.currentSelectedGameObject.name);
//				break;
//			case Affiliation.Enemy:
////				Debug.Log("Current unit is an enemy.");
//				List<Player> allies = PlayerManager.pm.GetParty();
//				int randomTarget = Random.Range(0, allies.Count);
//				BattleManager.bm.SetCurrentTarget(allies[randomTarget].gameObject);
//				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
//				BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
//				break;
//			default:
//				throw new ArgumentOutOfRangeException();
//		}
	}

	public void DisableActions() {
		SetMenusActive(false);
	}
	
	/***************************
	 *
	 * 
	 * Main Action Menu Options
	 *
	 * 
	 ***************************/
	/// <summary>
	/// 
	/// </summary>
	public void ChooseBasicAttack() {
		if (_attackMenu == null) {
			_attackMenu = Instantiate(attackMenuPrefab, BattleManager.battleCanvas.transform, false);
		}
//		Debug.LogFormat("Chose basic attack");
		previousOptions.Push(eventSystem.currentSelectedGameObject);    // Add attack action to previous options
		SpriteButton button = eventSystem.currentSelectedGameObject.GetComponent<SpriteButton>();
//		Debug.LogFormat("enqueing {0}", previousOptions.Peek());
		SetButtonsInteractable(_mainActionMenu, false);
		SetMenuActive(_attackMenu,true);
		SetButtonsInteractable(_attackMenu, true);
		eventSystem.SetSelectedGameObject(_attackMenu.transform.GetChild(0).gameObject);
		button.SpriteRenderer.sprite = button.spriteState.highlightedSprite;	// Show the highlighted sprite of the button
		TmpButton tmpButton = eventSystem.currentSelectedGameObject.GetComponent<TmpButton>();
		tmpButton.SpriteRenderer.sprite = tmpButton.spriteState.highlightedSprite;
//		Debug.Log("current attack " + tmpButton.name);
	}

	public void ChooseBag() {
	}

	public void ChooseSupport() {
//        SetButtonsInteractable(_actionMenu, false);
//        previousOptions.Clear();
//        BattleManager.bm.EndBattle(WinStatus.Escape);
	}
	
	public void ChooseRun() {
		SpriteButton button = eventSystem.currentSelectedGameObject.GetComponent<SpriteButton>();
		SetButtonsInteractable(_mainActionMenu, false);
		button.SpriteRenderer.sprite = button.spriteState.pressedSprite;	// Show pressed sprite
		previousOptions.Clear();
		BattleManager.bm.EndBattle(WinStatus.Escape);
	}

	public void CancelMainAction() {
		eventSystem.SetSelectedGameObject(_mainActionMenu.transform.GetChild(3).gameObject);
	}
	
	/***************************
	 *
	 * 
	 * Basic Attack Menu Options
	 *
	 * 
	 ***************************/
	public void AttackA() {
		Debug.Log("Doing Attack A");
		if (_targetContainer == null) {
			_targetContainer =  Instantiate(targetContainerPrefab, BattleManager.battleCanvas.transform, false);
		}
		// Create buttons ONLY if container has no buttons
		if (_targetContainer.transform.childCount == 0) {
			List<GameObject> enemies = BattleManager.bm.GetEnemies();
			// TODO: Add new buttons at the enemies position
			foreach (var targetEnemy in enemies) {
//				Debug.LogFormat("Found target {0}", targetEnemy.name);
				GameObject newButton =
					Instantiate(targetButtonPrefab, targetEnemy.transform.position, Quaternion.identity,
								_targetContainer.transform);
				TargetButton targetButton = newButton.GetComponent<TargetButton>();
				targetButton.target = targetEnemy;
				Character currentUnit = BattleManager.bm.GetCurrentUnit();
				targetButton.AddChooseTargetEventListener(currentUnit.DoAttackA);	// Add target to current units attack
				currentUnit.AddActionListener(AttackTarget);
//				Debug.LogFormat("newButton: {0}, {1}", newButton.name, newButton.transform.position);
			}
		}
		previousOptions.Push(eventSystem.currentSelectedGameObject); // Add attack A option to previous options
		SetButtonsInteractable(_attackMenu, false);
		SetMenuActive(_attackMenu,false);
		SetMenuActive(_targetContainer,true);
		SetButtonsInteractable(_targetContainer, true);
		eventSystem.SetSelectedGameObject(_targetContainer.transform.GetChild(0).gameObject);
//		Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
	}
	
	public void AttackB() {
		Debug.Log("Doing Attack B");
		if (_targetContainer == null) {
			_targetContainer =  Instantiate(targetContainerPrefab, BattleManager.battleCanvas.transform, false);
		}
		// Create buttons ONLY if container has no buttons
		if (_targetContainer.transform.childCount == 0) {
			List<GameObject> enemies = BattleManager.bm.GetEnemies();
			// TODO: Add new buttons at the enemies position
			foreach (var targetEnemy in enemies) {
//				Debug.LogFormat("Found target {0}", targetEnemy.name);
				GameObject newButton =
					Instantiate(targetButtonPrefab, targetEnemy.transform.position, Quaternion.identity,
					            _targetContainer.transform);
				TargetButton targetButton = newButton.GetComponent<TargetButton>();
				targetButton.target = targetEnemy;
				Character currentUnit = BattleManager.bm.GetCurrentUnit();
				targetButton.AddChooseTargetEventListener(currentUnit.DoAttackB);
				currentUnit.AddActionListener(AttackTarget);
//				Debug.LogFormat("newButton: {0}, {1}", newButton.name, newButton.transform.position);
			}
		}
		previousOptions.Push(eventSystem.currentSelectedGameObject); // Add attack A option to previous options
		SetButtonsInteractable(_attackMenu, false);
		SetMenuActive(_attackMenu,false);
		SetMenuActive(_targetContainer,true);
		SetButtonsInteractable(_targetContainer, true);
		eventSystem.SetSelectedGameObject(_targetContainer.transform.GetChild(0).gameObject);
		
//		SetButtonsInteractable(_attackMenu, false);
//		SetMenuActive(_attackMenu,false);
//		BattleManager.bm.SetCurrentActionType(ActionType.Buff);
//		BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
//		SetButtonsInteractable(_mainActionMenu, false);
//		previousOptions.Clear();
	}

	public void AttackTarget() {
//		BattleManager.bm.SetCurrentActionType(ActionType.Attack);
//		BattleManager.bm.SetCurrentTarget(eventSystem.currentSelectedGameObject.GetComponent<TargetButton>().target);
//		BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
		SetButtonsInteractable(_mainActionMenu, false);
		ResetTargets();
		previousOptions.Clear();
	}

	public void CancelCurrentAction() {
		Debug.LogFormat("Canceling {0}", eventSystem.currentSelectedGameObject.name);
		GameObject previousOption = previousOptions.Pop();
		Debug.LogFormat("new curr: {0}", previousOption.name);
		SetMenuActive(eventSystem.currentSelectedGameObject.transform.parent.gameObject, false);
		// Reactivate parent and set its buttons to be interactable
		Transform parent = previousOption.transform.parent;
		SetMenuActive(parent.gameObject, transform);
		SetButtonsInteractable(parent.gameObject, true);
		eventSystem.SetSelectedGameObject(previousOption);
	}

	public void ResetTargets() {
		foreach (Transform targetButton in _targetContainer.transform) {
			Destroy(targetButton.gameObject);
		}
	}
	
	public void SetMenusActive(bool isActive) {
		_mainActionMenu.SetActive(isActive);
		_attackMenu.SetActive(isActive);
	}
	
	public void SetMenuActive(GameObject menu, bool isActive) {
		menu.SetActive(isActive);
	}
	
	private static void SetButtonsInteractable(GameObject menu, bool isActive) {
		foreach (Transform child in menu.transform) {
			if (child.GetComponent<MenuButton>() == null) continue;
//			Debug.LogFormat("disabling {0}", child.name);
			child.GetComponent<MenuButton>().interactable = isActive;
		}
	}
	
	public void DisableBattleUI() {
		battleUI.SetActive(false);
		_mainActionMenu.SetActive(false);
	}

	public GameObject GetBattleUI() {
		return battleUI;
	}

	public void EnableMainActionMenu() {
		_mainActionMenu.SetActive(true);
	}
	
	public GameObject GetMainActionMenu() {
		return _mainActionMenu;
	}

	public GameObject GetDefaultMainAction() {
		return _mainActionMenu.transform.GetChild(0).gameObject;
	}

	public PlayerStatBar GetPlayerStatBar(CharacterSlot slot) {
		return playerStatBars[slot];
	}
	
	public Dictionary<CharacterSlot, PlayerStatBar> GetPlayerStatBars() {
		return playerStatBars;
	}
}
