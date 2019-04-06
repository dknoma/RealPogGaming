using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BasicAttackMenu", menuName = "Menu/BasicAttacks", order = 1)]
public class BasicAttackMenuObject : MenuObject<BasicAttackOption> {

	public GameObject basicAttackMenuPrefab;
	public Sprite inactive;
	public Sprite active;

	private GameObject menuRender;
	private BasicAttackOption optionA;
	private BasicAttackOption optionB;

	private void OnEnable() {
//		GameObject childA = menuRender.transform.GetChild(0).gameObject;
//		GameObject childB = menuRender.transform.GetChild(1).gameObject;
//		optionA = new BasicAttackOption(childA, MenuOptionState.BasicAttack, AttackOption.A);
//		optionB = new BasicAttackOption(childB, MenuOptionState.BasicAttack, AttackOption.B);
//		menu.InitMenu(2, MenuType.Vertical);
//		menu.InitMenuItems(new[]{optionA, optionB});
//		Debug.LogFormat("A: {0}", optionA.Text.text);
//		Debug.LogFormat("B: {0}", optionB.Text.text);
	}
	
//	public void InitAttackMenu() {
//		menu.InitMenu(2, menuType);
//		menu.InitMenuItems(new []{optionA, optionB});
//	}
	
	public void ShowMenu() {
		if (menuRender == null) {
			menuRender = Instantiate(basicAttackMenuPrefab, parent.transform, false);
			GameObject childA = menuRender.transform.GetChild(0).gameObject;
			GameObject childB = menuRender.transform.GetChild(1).gameObject;
			optionA = new BasicAttackOption(childA, MenuOptionState.BasicAttack, AttackOption.A);
			optionB = new BasicAttackOption(childB, MenuOptionState.BasicAttack, AttackOption.B);
			menu.InitMenu(2, MenuType.Vertical);
			menu.InitMenuItems(new[]{optionA, optionB});
			Debug.LogFormat("A: {0}", optionA.Text.text);
			Debug.LogFormat("B: {0}", optionB.Text.text);
		} 
		menuRender.SetActive(true);
		menu.GetCurrentItem().OptionRender().GetComponent<SpriteRenderer>().sprite = active;
		Vector3 textPos = menu.GetItem(0).Text.transform.position;
		menu.GetItem(0).Text.transform.position = new Vector3(textPos.x - 0.25f, textPos.y + 0.25f, textPos.z);
	}

	public void HideMenu() {
		if (menuRender == null) {
			menuRender = Instantiate(basicAttackMenuPrefab, parent.transform, false);
		} 
		menu.ResetCurrentIndex();
		menuRender.SetActive(false);
	}
	
	public override void NavigateMenu(Direction direction) {
		menu.GetCurrentItem().Renderer.sprite = inactive;
		Debug.LogFormat("previous render: {0}",
		                menu.GetCurrentItem().Renderer.sprite.name);
		
		Vector3 textPos = menu.GetCurrentItem().Text.transform.position;
		menu.GetCurrentItem().Text.transform.position = new Vector3(textPos.x + 0.25f, textPos.y - 0.25f, textPos.z);
		
		menu.TraverseOptions(direction);
		
		menu.GetCurrentItem().Renderer.sprite = active;
		textPos = menu.GetCurrentItem().Text.transform.position;
		menu.GetCurrentItem().Text.transform.position = new Vector3(textPos.x - 0.25f, textPos.y + 0.25f, textPos.z);
		
		Debug.LogFormat("current render: {0}",
		                menu.GetCurrentItem().Renderer.sprite.name);
		Debug.LogFormat("Current attack: {0}", menu.GetCurrentItem().Text.text);
	}

	public string AttackName() {
		return menu.GetCurrentItem().Text.text;
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