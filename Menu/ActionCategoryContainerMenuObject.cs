using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BasicAttackMenu", menuName = "Menu/BasicAttacks", order = 1)]
public class ActionCategoryContainerMenuObject : MenuObject<BasicAttackOption> {

	public GameObject basicAttackMenu;
	public Sprite inactive;
	public Sprite active;

	private BasicAttackOption optionA;
	private BasicAttackOption optionB;

	private void OnEnable() {
		GameObject childA = basicAttackMenu.transform.GetChild(0).gameObject;
		GameObject childB = basicAttackMenu.transform.GetChild(1).gameObject;
		optionA = new BasicAttackOption(childA, MenuOptionState.BasicAttack, AttackOption.A);
		optionB = new BasicAttackOption(childB, MenuOptionState.BasicAttack, AttackOption.B);
		menu.InitMenuItems(new[]{optionA,optionB});
		Debug.LogFormat("A: {0}", optionA.Text.text);
		Debug.LogFormat("B: {0}", optionB.Text.text);
	}

	public override void NavigateMenu(Direction direction) {
		menu.GetCurrentItem().OptionRender().GetComponent<SpriteRenderer>().sprite = inactive;
		menu.TraverseOptions(direction);
		menu.GetCurrentItem().OptionRender().GetComponent<SpriteRenderer>().sprite = active;
	}

	public string AttackName() {
		return menu.GetCurrentItem().Text.text;
	}
}

public class ActionCategoryContainer {

	protected GameObject optionObject;
	protected MenuOptionState menuOptionState;

	public ActionCategoryContainer() {
	}
	
	public ActionCategoryContainer(GameObject obj, MenuOptionState option) {
		optionObject = obj;
		menuOptionState = option;
	}

	public GameObject OptionRender() {
		return optionObject;
	}

	public MenuOptionState MenuOptionState() {
		return menuOptionState;
	}
}

public class BasicAttackOption : ActionCategoryContainer {
		
//	private GameObject button;
	private SpriteRenderer renderer;
	private TextMeshProUGUI text;
	private AttackOption attackOption;

	public BasicAttackOption(GameObject obj, MenuOptionState option, AttackOption attack) {
		optionObject = obj;
		renderer = obj.GetComponent<SpriteRenderer>();
		text = obj.GetComponentInChildren<TextMeshProUGUI>();
		attackOption = attack;
		menuOptionState = option;
	}

	public GameObject Button {
		get { return optionObject; }
		set { optionObject = value; }
	}
		
	public SpriteRenderer Renderer {
		get { return renderer; }
		set { renderer = value; }
	}
		
	public TextMeshProUGUI Text {
		get { return text; }
//		set { text = value; }
	}
	
	public AttackOption AttackOption {
		get { return attackOption; }
		set { attackOption = value; }
	}
}

//public class Attack {
//
//	private readonly AttackOption attackOption;
//	private readonly string name;
//
//	public Attack(AttackOption atk, string name) {
//		attackOption = atk;
//		this.name = name;
//	}
//
//	public AttackOption Option() {
//		return attackOption;
//	}
//
//	public string Name() {
//		return name;
//	}
//}