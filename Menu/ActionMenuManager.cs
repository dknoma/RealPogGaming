using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ActionMenuManager : MonoBehaviour {

    public static ActionMenuManager amm;
    
    private static EventSystem eventSystem = EventSystem.current;
    
    private Stack<GameObject> previousOptions = new Stack<GameObject>();

    [SerializeField]
    private GameObject actionMenuPrefab;
    private static GameObject _actionMenu;
    
    [SerializeField]
    private GameObject attackMenuPrefab;
    private static GameObject _attackMenu;
    
    [SerializeField]
    private GameObject targetContainerPrefab;
    private static GameObject _targetContainer;
    
    [SerializeField]
    private GameObject targetButtonPrefab;
    
    private void OnEnable() {
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
                SetButtonsInteractable(_actionMenu, true);
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
    
    public void ChooseBasicAttack() {
        if (_attackMenu == null) {
            _attackMenu = Instantiate(attackMenuPrefab, amm.transform, false);
        }
        Debug.LogFormat("Chose basic attack");
        previousOptions.Push(eventSystem.currentSelectedGameObject);    // Add attack action to previous options
        Debug.LogFormat("enqueing {0}", previousOptions.Peek());
        SetButtonsInteractable(_actionMenu, false);
        SetMenuActive(_attackMenu,true);
        SetButtonsInteractable(_attackMenu, true);
        eventSystem.SetSelectedGameObject(_attackMenu.transform.GetChild(0).gameObject);
        Debug.Log("current attack " + eventSystem.currentSelectedGameObject.name);
    }

    public void AttackA() {
        Debug.Log("Doing Attack A");
        if (_targetContainer == null) {
            _targetContainer =  Instantiate(targetContainerPrefab, amm.transform, false);
            List<GameObject> enemies = BattleManager.bm.GetEnemies();
            // TODO: Add new buttons at the enemies position
            foreach (var targetEnemy in enemies) {
                Debug.LogFormat("Found target {0}", targetEnemy.name);
                GameObject newButton =
                    Instantiate(targetButtonPrefab, targetEnemy.transform.position, Quaternion.identity, _targetContainer.transform);
                newButton.GetComponent<TargetButton>().target = targetEnemy;
                Debug.LogFormat("newButton: {0}, {1}", newButton.name, newButton.transform.position);
            }
        }
        previousOptions.Push(eventSystem.currentSelectedGameObject); // Add attack A option to previous options
        SetButtonsInteractable(_attackMenu, false);
        SetMenuActive(_attackMenu,false);
        SetMenuActive(_targetContainer,true);
        SetButtonsInteractable(_targetContainer, true);
        eventSystem.SetSelectedGameObject(_targetContainer.transform.GetChild(0).gameObject);
        Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
    }
    
    public void AttackB() {
        Debug.Log("Doing Attack B");
    }
    
    public void ChooseSupport() {
        SetButtonsInteractable(_actionMenu, false);
    }
    
    public void ChooseRun() {
        SetButtonsInteractable(_actionMenu, false);
        previousOptions.Clear();
        BattleManager.bm.EndBattle(WinStatus.Escape);
    }

    public void AttackTarget() {
        BattleManager.bm.SetCurrentTarget(eventSystem.currentSelectedGameObject.GetComponent<TargetButton>().target);
        BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
        SetMenusActive(false);
        ResetTargets();
        previousOptions.Clear();
    }

    public void ResetTargets() {
        foreach (GameObject targetButton in _targetContainer.transform) {
            Destroy(targetButton);
        }
    }
    
    public void SetMenusActive(bool isActive) {
        _actionMenu.SetActive(isActive);
        _attackMenu.SetActive(isActive);
    }
    
    public void SetMenuActive(GameObject menu, bool isActive) {
        menu.SetActive(isActive);
    }
    
    private static void SetButtonsInteractable(GameObject menu, bool isActive) {
        foreach (Transform child in menu.transform) {
            if (child.GetComponent<MenuButton>() == null) continue;
            Debug.LogFormat("disabling {0}", child.name);
            child.GetComponent<MenuButton>().interactable = isActive;
        }
    }
}
