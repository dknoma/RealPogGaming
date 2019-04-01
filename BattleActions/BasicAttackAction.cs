using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum AttackType {
//	Basic,
//	Item
//}
[CreateAssetMenu(fileName = "BasicAttackAction", menuName = "ScriptableObjects/BasicAttackAction", order = 1)]
public class BasicAttackAction : BattleAction {
	public enum OptionState {
		OpenedOption,
		SelectAction,
		SelectTarget,			// If aoe, no target selection is needed
		DoAction,
		CancelAction
	}

	private OptionState currentOptionState;
	
//	public ActionType action;
	private Weapon weapon;
	private WeaponType weaponType;
	private Element element = Element.None;
	private int atk;

	[SerializeField] GameObject attackAImage;
	[SerializeField] GameObject attackBImage;

	private string attackAName = "";
	private string attackBName = "";

	private const string SWORD_A = "Power Strike";
	private const string SWORD_B = "Armor Piercer";
	private const string GREATSWORD_A = "Greater Cleave";
	private const string GREATSWORD_B = "Whirlwind";
	private const string DAGGER_A = "Rapid Slash";
	private const string DAGGER_B = "Rapid Pierce";
	private const string ROD_A = "Energy Blast";
	private const string ROD_B = "Energy Splash";
	private const string STAFF_A = "Energy Blast";
	private const string STAFF_B = "Heal";
	private const string BOW_A = "Strafe";
	private const string BOW_B = "Snipe";
	private const string SPEAR_A = "Heart Piercer";
	private const string SPEAR_B = "Armor Piercer";

	private MenuingOption menuingOption;
	
	private MenuGraph<GameObject> attackSelectMenu;

//	private AttackType currentAttack;
//	private PlayerController player;

	private void OnEnable() {
		menuOption = MenuOption.Attack;
		attackSelectMenu = new MenuGraph<GameObject>(2, MenuType.Vertical);
		attackSelectMenu.InitMenuItems(new GameObject[2]);	
//		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	public override void DoAction(bool selectingOption) {
		InitAttackOptions();
//		menuingOption = MenuingOption.Submenu;
		switch (currentOptionState) {
			case OptionState.OpenedOption:
				currentOptionState = OptionState.SelectAction;
				break;
			case OptionState.SelectAction:
				// TODO: navigate attack select menu
				//		 if select an attack -> to select target
				//		 else if cancel
				SelectAction(selectingOption);
				break;
			case OptionState.SelectTarget:
				// TODO: navigate target select menu
				currentOptionState = OptionState.DoAction;
				break;
			case OptionState.DoAction:
				DoAttack();
				break;
			case OptionState.CancelAction:
				// TODO: if cancel attack -> action menu backs up to optionnavigation state
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void InitAttackOptions() {
		switch (weaponType) {
			case WeaponType.GreatSword:
				attackAName = GREATSWORD_A;
				attackBName = GREATSWORD_B;
				break;
			case WeaponType.Sword:
				attackAName = SWORD_A;
				attackBName = SWORD_B;
				break;
			case WeaponType.Dagger:
				attackAName = DAGGER_A;
				attackBName = DAGGER_B;
				break;
			case WeaponType.Rod:
				attackAName = ROD_A;
				attackBName = ROD_B;
				break;
			case WeaponType.Staff:
				attackAName = STAFF_A;
				attackBName = STAFF_B;
				break;
			case WeaponType.Bow:
				attackAName = BOW_A;
				attackBName = BOW_B;
				
				break;
			case WeaponType.Spear:
				attackAName = SPEAR_A;
				attackBName = SPEAR_B;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void SelectAction(bool selectingOption) {
		if (selectingOption) {
			currentOptionState = OptionState.SelectTarget;
		} else {
			Debug.Log("Navigating sub menu.");
		}
	}

	private void DoAttack() {
		switch (weaponType) {
			case WeaponType.GreatSword:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Sword:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Dagger:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Rod:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Staff:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Bow:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Spear:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public override void SetWeapon(Weapon weapon) {
		this.weapon = weapon;
		weaponType = weapon.GetWeaponType();
		element = weapon.GetWeaponElement();
	}

//	public override void SetWeaponType(WeaponType type) {
//		weaponType = type;
//	}
//	
//	public override void SetElement(Element element) {
//		this.element = element;
//	}

	public override WeaponType GetWeaponType() {
		return weaponType;
	}
	
	public override Element GetElement() {
		return element;
	}
	public OptionState GetOptionState() {
		return currentOptionState;
	}
}
