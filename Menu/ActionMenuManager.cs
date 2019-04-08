using System;
using System.Collections.Generic;
using Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ActionMenuManager : MonoBehaviour {

    public static ActionMenuManager amm;
    
    private EventSystem eventSystem;
    
    private List<GameObject> targetButtons;
    
    [SerializeField]
    private GameObject actionMenuPrefab;
    private static GameObject _actionMenu;
    
    [SerializeField]
    private GameObject attackMenuPrefab;
    private static GameObject _attackMenu;
    
    [SerializeField]
    private GameObject targetButtonPrefab;
    private static GameObject _targetButton;
    
    private void Awake() {
        if (amm == null) {
            amm = this;
        } else if (amm != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        eventSystem = EventSystem.current;
    }

//    private void Update() {
//        if (Input.GetKeyDown("f")) {
//            TryActionsSpawn();
//        }
//    }

    public void TryActionsSpawn() {
        if (!BattleManager.bm.InBattle()) return;
        switch (BattleManager.bm.GetCurrentUnit().GetAffiliation()) {
            case Affiliation.Ally:
                Debug.Log("Current unit is an ally.");
                if (_actionMenu == null) {
                    _actionMenu = Instantiate(actionMenuPrefab, amm.transform, false);
                }
                if (eventSystem == null) {
                    eventSystem = EventSystem.current;
                }
                SetMenuActive(_actionMenu,true);
                SetButtonsActive(_actionMenu, true);
                eventSystem.SetSelectedGameObject(_actionMenu.transform.GetChild(0).gameObject);
                Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
                break;
            case Affiliation.Enemy:
                Debug.Log("Current unit is an enemy.");
                List<Player> allies = PlayerManager.pm.GetParty();
                int randomTarget = Random.Range(0, allies.Count);
                BattleManager.bm.SetCurrentTarget(allies[randomTarget].gameObject);
                BattleManager.bm.SetCurrentActionType(ActionType.Attack);
                BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChooseBasicAttack() {
//        actionMenu.SetActive(false);
//        Debug.LogFormat("Chose {0}", eventSystem.firstSelectedGameObject.name);
        Debug.Log("Chose basic attack: " + transform.name);
        SetButtonsActive(_actionMenu, false);
        if (_attackMenu == null) {
	        _attackMenu = Instantiate(attackMenuPrefab, amm.transform, false);
        }
        if (eventSystem == null) {
            eventSystem = EventSystem.current;
        }
        SetMenuActive(_attackMenu,true);
        SetButtonsActive(_attackMenu, true);
        eventSystem.SetSelectedGameObject(_attackMenu.transform.GetChild(0).gameObject);
        Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
    }

    public void AttackA() {
        SetButtonsActive(_attackMenu, false);
        Debug.Log("Doing Attack A");
        if (_targetButton == null) {
            _targetButton = targetButtonPrefab;
        }
        if (eventSystem == null) {
            eventSystem = EventSystem.current;
        }
        List<GameObject> enemies = BattleManager.bm.GetEnemies();
        targetButtons = new List<GameObject>();
        // TODO: Add new buttons at the enemies position
        int i = 0;
        foreach (var targetEnemy in enemies) {
            Debug.LogFormat("Found target {0}", targetEnemy.name);
            GameObject newButton =
                Instantiate(_targetButton, targetEnemy.transform.position, Quaternion.identity, amm.transform);
            Debug.LogFormat("newButton: {0}, {1}", newButton.name, newButton.transform.position);
            targetButtons.Add(newButton);
            targetButtons[i].GetComponent<TargetButton>().target = targetEnemy;
            i++;
        }
        Debug.LogFormat("_targetButton: {0}", targetButtons[0].name);
        eventSystem.SetSelectedGameObject(targetButtons[0]);
        Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
    }
    
    public void AttackB() {
        Debug.Log("Doing Attack B");
    }
    
    public void ChooseSupport() {
        SetButtonsActive(_actionMenu, false);
    }
    
    public void ChooseRun() {
        SetButtonsActive(_actionMenu, false);
        BattleManager.bm.EndBattle(WinStatus.Escape);
    }

    public void AttackTarget() {
        BattleManager.bm.SetCurrentTarget(eventSystem.currentSelectedGameObject.GetComponent<TargetButton>().target);
        BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
        SetMenusActive(false);
        ResetTargets();
    }

    public void SetTargetButtonsActive(bool isActive) {
        foreach (GameObject targetButton in targetButtons) {
            targetButton.SetActive(isActive);
        }
    }
    
    public void ResetTargets() {
        foreach (GameObject targetButton in targetButtons) {
            Destroy(targetButton);
        }
        targetButtons = new List<GameObject>();
    }
    
    public void SetMenusActive(bool isActive) {
        _actionMenu.SetActive(isActive);
        _attackMenu.SetActive(isActive);
    }
    
    public void SetMenuActive(GameObject menu, bool isActive) {
        menu.SetActive(isActive);
    }
    
    private static void SetButtonsActive(GameObject menu, bool isActive) {
//        eventSystem.currentSelectedGameObject.GetComponent<TmpButton>().interactable = false;
        foreach (Transform child in menu.transform) {
            if (child.GetComponent<Button>() == null) continue;
            child.GetComponent<Button>().interactable = isActive;
        }
    }
}
