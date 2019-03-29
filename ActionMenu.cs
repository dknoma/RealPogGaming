using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour {

	public Image CurrentAction { get; private set; }
	
	private readonly List<GameObject> actions = new List<GameObject>();
	private MenuGraph<Image> testMenu;
	
	private Direction currentDirection;
	private bool axisDown;

	private void OnEnable() {
		foreach (Transform child in transform) {
			Debug.Log(child.name);
			actions.Add(child.gameObject);
		}
		Debug.Log("size: " + actions.Count);
		testMenu = new MenuGraph<Image>(actions.Count, MenuType.Horizontal);
		testMenu.InitMenuItems(new Image[actions.Count]);
		for(int i = 0; i < actions.Count; i++) {
			Image img = actions[i].GetComponent<Image>();
			testMenu.AddItem(img, i);
			testMenu.GetItem(i).color = Color.white;
		}
		CurrentAction = testMenu.GetCurrentItem();
		CurrentAction.color = Color.grey;
	}

	private void Update() {
		if (BattleManager.bm.InBattle() && BattleManager.bm.BattlePhase == BattlePhase.Action) {
			GetAxisDown();
//			NavigateActionMenu();
		}
		if (Input.GetButtonDown("Test") && CurrentAction.CompareTag("AttackAction")) {
			BattleManager.bm.BattlePhase = BattlePhase.Battle;
		} else if (Input.GetButtonDown("Test") && CurrentAction.CompareTag("RunAction")) {
			BattleManager.bm.EndBattle(WinStatus.Escape);
		}
	}
	
	public void NavigateMenu(Direction direction) {
		Debug.Log("direction: " + direction);
		testMenu.TraverseOptions((int) direction);
		CurrentAction.color = Color.white; // Change previous action to white
		CurrentAction = testMenu.GetCurrentItem();
		CurrentAction.color = Color.grey; // Change current action to grey
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
