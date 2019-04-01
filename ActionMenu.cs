using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuingOption {
	Action,
	Submenu
}

public class ActionMenu : MonoBehaviour {

	public enum ActionState {
		BasicAttack,
		Support,
		Special,
		Defend,
		Escape,
		Cancel
	}
	
	public GameObject cursor;
	
	public GameObject attackOptionPrefab;
	public GameObject runOptionPrefab;
	
	private Image currentActionImage;
	
	private readonly List<OptionContainer> actions = new List<OptionContainer>();
	private MenuGraph<OptionContainer> testMenu;
	private MenuGraph<GameObject> enemies;
	private MenuGraph<OptionContainer> currentMenu;
	
	// TODO: get current unit from battlemanager -> get actions from units battleaction list in Character -> 
	//		 instantiate actions into menu

	// TODO: action object -> action has action type; use state machine to determine how phases move
	private GameObject currentBattleOption;
	private BattleAction currentAction;		// This caches the current action so if action is canceled, goes back to main menu
	private Direction currentDirection;
	private bool axisDown;
	private bool inSubMenu;

	private GameObject attackoption;
	private GameObject runOption;

	private void OnEnable() {
		if (!BattleManager.bm.InBattle()) return;
		// Only instantiate objects when in battle
		if (attackoption == null) {
			Debug.Log("Instantiate attack option.");
			attackoption = Instantiate(attackOptionPrefab, transform, false);
		}
		if (runOption == null) {
			runOption = Instantiate(runOptionPrefab, transform, false);
		}
		// TODO: make options objects rather than images. This way they can render position and sort order correctly
		Character currentUnit = BattleManager.bm.GetCurrentUnit();
		actions.Add(new OptionContainer(attackoption, currentUnit.GetAction(MenuOption.Attack)));	// Add attack option render and action to the list
		actions.Add(new OptionContainer(runOption, currentUnit.GetAction(MenuOption.Escape))); 		// Add escape option render and action to the list
		int actionCount = actions.Count;
		Debug.Log("action menu size: " + actionCount);
		testMenu = new MenuGraph<OptionContainer>(actionCount, MenuType.Horizontal);
		testMenu.InitMenuItems(new OptionContainer[actionCount]);
		for(int i = 0; i < actionCount; i++) {
//			Image img = actions[i].GetComponent<Image>();
			testMenu.AddItem(actions[i], i);
			testMenu.GetItem(i).OptionRender().GetComponent<Image>().color = Color.grey;
		}
		currentMenu = testMenu;
		currentBattleOption = testMenu.GetCurrentItem().OptionRender();
		currentActionImage = currentBattleOption.GetComponent<Image>();
		currentActionImage.color = Color.white;

		currentAction = currentMenu.GetCurrentItem().Action();
		
		// Get current enemy list. Useful for targeting enemies
		int enemyCount = BattleManager.bm.GetEnemies().Count;
		enemies = new MenuGraph<GameObject>(enemyCount, MenuType.Both);
		enemies.InitMenuItems(new GameObject[enemyCount]);
		for(int i = 0; i < enemyCount; i++) {
			enemies.AddItem(BattleManager.bm.GetEnemies()[i], i);
		}
	}

	private void Update() {
		if (BattleManager.bm.InBattle() && BattleManager.bm.BattlePhase == BattlePhase.Action) {
			GetAxisDown();
			if (Input.GetButtonDown("Test")) {
				currentAction.DoAction(true); // TODO: actually navigate through menu, let it go to next submenu and still nagivate
			}
		}

//		if (Input.GetButtonDown("Test") && currentActionImage.CompareTag("AttackAction")) {
//			BattleManager.bm.BattlePhase = BattlePhase.Battle;
//		} else if (Input.GetButtonDown("Test") && currentActionImage.CompareTag("RunAction")) {
//			BattleManager.bm.EndBattle(WinStatus.Escape);
//		}
	}
	
	public void NavigateMenu(Direction direction) {
//		Debug.Log("direction: " + direction);
		currentMenu.TraverseOptions(direction);
		currentActionImage.color = Color.grey; // Change previous action to white
//		currentActionImage = testMenu.GetCurrentItem();
		Debug.Log("asdasda: " + currentMenu.GetCurrentItem().Action().name);
		currentAction = currentMenu.GetCurrentItem().Action();
		currentBattleOption = currentMenu.GetCurrentItem().OptionRender();
		currentActionImage = currentBattleOption.GetComponent<Image>();
		currentActionImage.color = Color.white; // Change current action to grey
	}

	private void GetAxisDown() {
		if (axisDown) {
			currentDirection = Direction.Null;
			if (!(Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) ||
			    !(Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon)) return;
//			Debug.Log("Reset axis down");
			axisDown = false;
			return;
		}
		if(Input.GetAxisRaw ("Horizontal") > 0) {
//			Debug.Log ("Right");
			axisDown = true;
			NavigateMenu(Direction.Right);
		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
//			Debug.Log ("Left");
			axisDown = true;
			NavigateMenu(Direction.Left);
		} else if(Input.GetAxisRaw ("Vertical") > 0) {
			axisDown = true;
			NavigateMenu(Direction.Up);
		} else if(Input.GetAxisRaw ("Vertical") < 0) {
			axisDown = true;
			NavigateMenu(Direction.Down);
		} else {
//			Debug.Log("Reset axis down");
			axisDown = false;
		}
	}

	private class OptionContainer {

		private readonly GameObject optionRender;
		private readonly BattleAction action;

		public OptionContainer(GameObject obj, BattleAction act) {
			optionRender = obj;
			action = act;
		}

		public GameObject OptionRender() {
			return optionRender;
		}

		public BattleAction Action() {
			return action;
		}
	}
}
