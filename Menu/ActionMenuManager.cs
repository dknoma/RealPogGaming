using System.Collections.Generic;
using Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenuManager : MonoBehaviour {

    public static ActionMenuManager amm;
    
    private EventSystem eventSystem;
    
    private List<Button> targets = new List<Button>();
    
    [SerializeField]
    private GameObject actionMenuPrefab;
    private static GameObject _actionMenu;
    
    [SerializeField]
    private GameObject attackMenuPrefab;
    private static GameObject _attackMenu;

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

    private void Update() {
        if (Input.GetKeyDown("f")) {
            SpawnActions();
        }
    }

    public void SpawnActions() {
        if (_actionMenu == null) {
            _actionMenu = Instantiate(actionMenuPrefab, amm.transform, false);
        }
        eventSystem.SetSelectedGameObject(_actionMenu.transform.GetChild(0).gameObject);
//        eventSystem.firstSelectedGameObject = actionMenu.transform.GetChild(0).gameObject;
        Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
    }

    private static void DisableButtons() {
//        eventSystem.currentSelectedGameObject.GetComponent<TmpButton>().interactable = false;
        foreach (Transform child in _actionMenu.transform) {
            child.GetComponent<Button>().interactable = false;
        }
    }

    public void ChooseBasicAttack() {
//        actionMenu.SetActive(false);
//        Debug.LogFormat("Chose {0}", eventSystem.firstSelectedGameObject.name);
        Debug.Log("Chose basic attack: " + transform.name);
        DisableButtons();
        if (_attackMenu == null) {
	        _attackMenu = Instantiate(attackMenuPrefab, amm.transform, false);
        }
        if (eventSystem == null) {
            eventSystem = EventSystem.current;
        }
        eventSystem.SetSelectedGameObject(_attackMenu.transform.GetChild(0).gameObject);
        Debug.Log("currentSelected " + eventSystem.currentSelectedGameObject.name);
    }

    public void AttackA() {
        Debug.Log("Attack A");
        List<GameObject> enemies = BattleManager.bm.GetEnemies();
        foreach (var target in enemies) {
//            targets.Add();
        }
    }
    
    public void AttackB() {
        Debug.Log("Attack B");
    }
    
    public void ChooseSupport() {
        DisableButtons();
    }
    
    public void ChooseRun() {
        DisableButtons();
        BattleManager.bm.EndBattle(WinStatus.Escape);
    }
}
