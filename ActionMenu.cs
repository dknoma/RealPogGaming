using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour {

	private readonly List<GameObject> actions = new List<GameObject>();
	private MenuGraph<Image> testMenu;
	public Image CurrentAction { get; private set; }

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
		}
		CurrentAction = testMenu.GetCurrentItem();
		CurrentAction.color = Color.grey;
	}

	public void NavigateMenu(Direction direction) {
		Debug.Log("direction: " + direction);
		testMenu.TraverseOptions((int) direction);
		CurrentAction.color = Color.white; // Change previous action to white
		CurrentAction = testMenu.GetCurrentItem();
		CurrentAction.color = Color.grey; // Change current action to grey
	}

}
