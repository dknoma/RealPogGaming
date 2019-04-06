using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BasicAttackMenu", menuName = "Menu/BasicAttacks", order = 1)]
public class BasicAttackMenuObject : MenuObject<AttackButton> {

	public GameObject basicAttackMenuPrefab;
	public Sprite inactive;
	public Sprite active;

	private GameObject menuRender;
	private AttackButton buttonA;
	private AttackButton buttonB;
	private string attackAName;
	private string attackBName;
	
	public void ShowMenu() {
		if (menuRender == null) {
			// If menu doesnt exist in scene, initialize it
			menuRender = Instantiate(basicAttackMenuPrefab, parent.transform, false);
			GameObject childA = menuRender.transform.GetChild(0).gameObject;
			GameObject childB = menuRender.transform.GetChild(1).gameObject;
			buttonA = new AttackButton(childA, MenuOptionState.BasicAttack, AttackOption.A);
			buttonB = new AttackButton(childB, MenuOptionState.BasicAttack, AttackOption.B);
		} 
		menuRender.SetActive(true);
		buttonA.Text.text = attackAName;
		buttonB.Text.text = attackBName;
		menu.InitMenu(2, MenuType.Vertical);
		menu.InitMenuItems(new[]{buttonA, buttonB});
		Debug.LogFormat("A: {0}", buttonA.Text.text);
		Debug.LogFormat("B: {0}", buttonB.Text.text);
		menu.GetCurrentItem().OptionRender().GetComponent<SpriteRenderer>().sprite = active;
		menu.GetItem(0).Text.transform.position 
			= new Vector3(buttonA.OrigTextPos.x - 0.25f, buttonA.OrigTextPos.y + 0.25f, buttonA.OrigTextPos.z);
	}

	public void HideMenu() {
		if (menuRender == null) {
			menuRender = Instantiate(basicAttackMenuPrefab, parent.transform, false);
		} 
		DeactivateCurrentButton();
		menu.ResetCurrentIndex();
		menuRender.SetActive(false);
	}
	
	public override void NavigateMenu(Direction direction) {
		DeactivateCurrentButton();
		menu.TraverseOptions(direction);
		ActivateCurrentButton();
//		menu.GetCurrentItem().Renderer.sprite = inactive;
//		Vector3 textPos = menu.GetCurrentItem().Text.transform.position;
//		menu.GetCurrentItem().Text.transform.position = new Vector3(textPos.x + 0.25f, textPos.y - 0.25f, textPos.z);
//		
//		menu.TraverseOptions(direction);
//		
//		menu.GetCurrentItem().Renderer.sprite = active;
//		textPos = menu.GetCurrentItem().Text.transform.position;
//		menu.GetCurrentItem().Text.transform.position = new Vector3(textPos.x - 0.25f, textPos.y + 0.25f, textPos.z);
		Debug.LogFormat("Current attack: {0}", menu.GetCurrentItem().Text.text);
	}

	// TODO: Maybe actually make a button class?
	private void ActivateCurrentButton() {
		AttackButton opt = menu.GetCurrentItem();
		opt.Renderer.sprite = active;
		Vector3 texPost = opt.Text.transform.position;
		opt.Text.transform.position = new Vector3(texPost.x - 0.25f, texPost.y + 0.25f, texPost.z);
	}

	private void DeactivateCurrentButton() {
		AttackButton opt = menu.GetCurrentItem();
		opt.Renderer.sprite = inactive;
		opt.Text.transform.position = opt.OrigTextPos;
	}

	public string AttackName() {
		return menu.GetCurrentItem().Text.text;
	}

	public void SetAttackAName(string attackName) {
		attackAName = attackName;
	}
	
	public void SetAttackBName(string attackName) {
		attackBName = attackName;
	}
}

public class AttackButton : ActionCategoryContainer {
		
//	private GameObject button;
	private SpriteRenderer renderer;
	private TextMeshProUGUI text;
	private AttackOption attackOption;
	private Vector3 origTextPos;

	public AttackButton(GameObject obj, MenuOptionState option, AttackOption attack) {
		optionObject = obj;
		renderer = obj.GetComponent<SpriteRenderer>();
		text = obj.GetComponentInChildren<TextMeshProUGUI>();
		attackOption = attack;
		menuOptionState = option;
		origTextPos = text.transform.position;
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

	public Vector3 OrigTextPos {
		get { return origTextPos; }
		set { origTextPos = value; }
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