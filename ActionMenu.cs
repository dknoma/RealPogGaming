using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum 
public class ActionMenu : MonoBehaviour {

	private Image currentActionImage;
	
	private readonly List<GameObject> actions = new List<GameObject>();
	private MenuGraph<GameObject> testMenu;
	private MenuGraph<GameObject> enemies;
//	private MenuGraph<Image> testMenu;
	
	// TODO: get current unit from battlemanager -> get actions from units battleaction list in Character -> 
	//		 instantiate actions into menu

	// TODO: action object -> action has action type; use state machine to determine how phases move
	private GameObject currentBattleAction;
	private BattleAction currentAction;
	private Direction currentDirection;
	private bool axisDown;
	private bool inSubMenu;

	private void OnEnable() {
		foreach (Transform child in transform) {
			Debug.Log(child.name);
			actions.Add(child.gameObject);
		}
		int actionCount = actions.Count;
		Debug.Log("size: " + actionCount);
		testMenu = new MenuGraph<GameObject>(actionCount, MenuType.Horizontal);
		testMenu.InitMenuItems(new GameObject[actionCount]);		
//		testMenu = new MenuGraph<Image>(actions.Count, MenuType.Horizontal);
//		testMenu.InitMenuItems(new Image[actions.Count]);
		for(int i = 0; i < actionCount; i++) {
//			Image img = actions[i].GetComponent<Image>();
			testMenu.AddItem(actions[i], i);
			testMenu.GetItem(i).GetComponent<Image>().color = Color.white;
//			testMenu.AddItem(img, i);
//			testMenu.GetItem(i).color = Color.white;
		}
//		currentActionImage = testMenu.GetCurrentItem();
		currentBattleAction = testMenu.GetCurrentItem();
		currentActionImage = currentBattleAction.GetComponent<Image>();
		currentActionImage.color = Color.grey;
		
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
//			NavigateActionMenu();
		}
		if (Input.GetButtonDown("Test") && currentActionImage.CompareTag("AttackAction")) {
			BattleManager.bm.BattlePhase = BattlePhase.Battle;
		} else if (Input.GetButtonDown("Test") && currentActionImage.CompareTag("RunAction")) {
			BattleManager.bm.EndBattle(WinStatus.Escape);
		}
	}
	
	public void NavigateMenu(Direction direction) {
		Debug.Log("direction: " + direction);
		testMenu.TraverseOptions(direction);
		currentActionImage.color = Color.white; // Change previous action to white
//		currentActionImage = testMenu.GetCurrentItem();
		currentBattleAction = testMenu.GetCurrentItem();
		currentActionImage = currentBattleAction.GetComponent<Image>();
		currentActionImage.color = Color.grey; // Change current action to grey
	}
	
	
//	private void NavigateActionMenu() {
//		switch(currentDirection) {
//			case Direction.Up:
//				NavigateMenu(currentDirection);
//				break;
//			case Direction.Down:
//				NavigateMenu(currentDirection);
//				break;
//			case Direction.Left:
//				NavigateMenu(currentDirection);
//				break;
//			case Direction.Right:
//				NavigateMenu(currentDirection);
//				break;
//		}
//	}

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
//			currentDirection = Direction.Right;
			NavigateMenu(Direction.Right);
		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
//			Debug.Log ("Left");
			axisDown = true;
//			currentDirection = Direction.Left;
			NavigateMenu(Direction.Left);
		} else if(Input.GetAxisRaw ("Vertical") > 0) {
			axisDown = true;
//			currentDirection = Direction.Up;
			NavigateMenu(Direction.Up);
		} else if(Input.GetAxisRaw ("Vertical") < 0) {
			axisDown = true;
//			currentDirection = Direction.Down;
			NavigateMenu(Direction.Down);
		} else {
//			Debug.Log("Reset axis down");
			axisDown = false;
		}
	}
}
