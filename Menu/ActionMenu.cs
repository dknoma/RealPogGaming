using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum MenuState {
	Main,			// Main menu: where player selects what option they wanna take
	AttackSubMenu,
	SubMenu,
	Target
}

public enum MenuOptionState {
	BasicAttack, 	// Player gets taken to a submenu where they choose which attack they want
	Support,     	// Player gets taken to a submenu where they choose another action
	Escape      	// Player tries to escape the battle
}

public enum SubMenuState {
	Nil,
	BasicAttack,	// Basic attack submenu: choose basic attack
	Support,		// Choose if defend or use items
	Items,			// Go through list of items and choose one
	Allies,			// Choose an ally to target
	Enemies,		// Choose an enemy to target
	All				// Choose any unit to target
}

public enum AttackOption {
	A,
	B
}

public enum ActionType {
	Attack,
	Heal,
	Revive,
	Buff,
	Defend,
	Escape
}

public class ActionMenu : MonoBehaviour {

	[SerializeField] private BasicAttackMenuObject basicAttackMenu;
	[SerializeField] private MainMenu main;
	[SerializeField] private EnemyTargetChooseMenu enemyTargets;
	
	public GameObject cursor;
	public GameObject attackOptionPrefab;
	public GameObject runOptionPrefab;

	private List<ActionCategoryContainer> actions;
	private Image currentActionImage;
	private GameObject currentBattleOptionRender;
	private GameObject attackOption;
	private GameObject runOption;
	private WeaponType currentUnitWeaponType;
	private MenuState menuState;
	private MenuOptionState menuOptionState;
	private SubMenuState subMenuState;
	private bool axisDown;
	private bool inSubMenu;
	private bool initializedMenu;

//	private string attackAName = "";
//	private string attackBName = "";
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

//	private void OnEnable() {
//		// Only instantiate objects when in battle
//		if (!BattleManager.bm.InBattle()) return;
//		switch (BattleManager.bm.GetCurrentUnit().GetAffiliation()) {
//			case Affiliation.Ally:
//				Debug.Log("Current unit is an ally.");
//				InitMenu();
//				break;
//			case Affiliation.EnemyPrefab:
//				Debug.Log("Current unit is an enemy.");
//				List<GameObject> allies = BattleManager.bm.GetAllies();
//				int randomTarget = Random.Range(0, allies.Count);
//				BattleManager.bm.SetCurrentTarget(allies[randomTarget]);
//				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
//				BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
//				break;
//			default:
//				throw new ArgumentOutOfRangeException();
//		}
//	}

	public void TryInitMenu() {
		// Only instantiate objects when in battle
		if (!BattleManager.bm.InBattle()) return;
		switch (BattleManager.bm.GetCurrentUnit().GetAffiliation()) {
			case Affiliation.Ally:
				Debug.Log("Current unit is an ally.");
				InitMenu();
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

	private void InitMenu() {
		Debug.Log("Init action menuObject.");
		if (attackOption == null) {
			Debug.Log("Instantiate attack option.");
			attackOption = Instantiate(attackOptionPrefab, transform, false);
		}
		if (runOption == null) {
			runOption = Instantiate(runOptionPrefab, transform, false);
		}
		// TODO: make options objects rather than images. This way they can render position and sort order correctly
//		Character currentUnit = BattleManager.bm.GetCurrentUnit();
		actions = new List<ActionCategoryContainer> {
					new ActionCategoryContainer(attackOption, MenuOptionState.BasicAttack), // Add attack option render and action to the list
					new ActionCategoryContainer(runOption, MenuOptionState.Escape) 			// Add escape option render and action to the list
				};

		int actionCount = actions.Count;
		Debug.Log("action menuObject size: " + actionCount);
		main.InitMainMenu(actionCount);
		for(int i = 0; i < actionCount; i++) {
			main.AddItem(actions[i], i);
			main.DeactivateButton(i);
		}
		main.ActivateCurrentButton();
		menuOptionState = main.GetCurrentItem().MenuOptionState();
		
		// Get current enemy list. Useful for targeting enemies
		int enemyCount = BattleManager.bm.GetEnemies().Count;
		enemyTargets.InitTargetMenu(enemyCount);
		for(int i = 0; i < enemyCount; i++) {
			enemyTargets.AddItem(BattleManager.bm.GetEnemies()[i], i);
		}
		Character currentChar = BattleManager.bm.GetCurrentUnit();
		currentUnitWeaponType = currentChar.GetAffiliation() == Affiliation.Ally 
			? currentChar.GetComponent<Player>().GetWeapon().GetWeaponType() 
			: currentChar.GetComponent<Enemy>().GetWeaponType();
		InitAttackOptions(currentUnitWeaponType);
		Debug.LogFormat("name: {0}, {1}", currentChar.GetComponent<Player>().name, currentChar.GetComponent<Player>().GetWeapon().name);
		Debug.Log("Unit's weapon ====== " + currentUnitWeaponType);
		if (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != 0 ||
		    Mathf.RoundToInt(Input.GetAxisRaw("Vertical")) != 0) {
			axisDown = true;
		}
		initializedMenu = true;
	}

	private void Update() {
		if (BattleManager.bm.InBattle() && BattleManager.bm.GetBattlePhase() == BattlePhase.Action && initializedMenu) {
			UseMenu();
		}

//		if (Input.GetButtonDown("Test") && currentActionImage.CompareTag("AttackAction")) {
//			BattleManager.bm.BattlePhase = BattlePhase.Battle;
//		} else if (Input.GetButtonDown("Test") && currentActionImage.CompareTag("RunAction")) {
//			BattleManager.bm.EndBattle(WinStatus.Escape);
//		}
	}
	
	private void UseMenu() {
		switch (menuState) {
			case MenuState.Main:
//				Debug.Log("Choosing menuObject category.");
				GetAxisDown(main);
				if (Input.GetButtonDown("Test")) {
					ChangeMenu();
				}
				break;
			case MenuState.AttackSubMenu:
				GetAxisDown(basicAttackMenu);
				if (Input.GetButtonDown("Test")) {
					DoMenuAction();
				}
				// TODO: If hit cancel, go back to main
				break;
			case MenuState.Target:
//				Debug.Log("Choosing target.");
				GetAxisDown(enemyTargets);
				if (Input.GetButtonDown("Test")) {
					ChooseTarget();
				}
				// TODO: If hit cancel, go back to main
				break;
			case MenuState.SubMenu:
////				Debug.Log("Choosing sub menuObject category.");
//				GetAxisDown(currentSubMenu);
//				if (Input.GetButtonDown("Test")) {
//					DoMenuAction();
//				}
				Debug.Log("DEPRACTED");
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ChangeMenu() {
		Debug.LogFormat("menuObject option state: {0}", menuOptionState);
		switch (menuOptionState) {
			case MenuOptionState.BasicAttack:
				basicAttackMenu.setParentTransform(transform);
				basicAttackMenu.ShowMenu();
				Debug.Log("Chose  basic attack.");
				Debug.LogFormat("Current attack: {0}", basicAttackMenu.GetCurrentItem().Text.text);
				menuState = MenuState.AttackSubMenu;
				break;
			case MenuOptionState.Support:
//				menuState = MenuState.SubMenu;
				break;
			case MenuOptionState.Escape:
				BattleManager.bm.EndBattle(WinStatus.Escape);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void DoMenuAction() {
		switch (menuOptionState) {
			case MenuOptionState.BasicAttack:
				basicAttackMenu.HideMenu();
				Debug.Log("Choosing target.");
//				currentTargets = enemies;
//				Debug.LogFormat("current target: {0}",currentTargets.GetCurrentItem().name);
				Debug.LogFormat("current target: {0}",enemyTargets.GetCurrentItem().name);
				menuState = MenuState.Target;
				subMenuState = SubMenuState.BasicAttack;
				// Current targets = enemies
				break;
			case MenuOptionState.Support:
//				menuState = MenuState.SubMenu;
				// Change menuObject to submenu of items and defense
				break;
			case MenuOptionState.Escape:
				BattleManager.bm.EndBattle(WinStatus.Escape);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ChooseTarget() {
//		GameObject target = currentTargets.GetCurrentItem();
//		GameObject target = enemyTargets.GetCurrentItem();
		Debug.Log("Chose target " + BattleManager.bm.GetCurrentTarget());
		subMenuState = SubMenuState.BasicAttack;
		ResolveActionSelection();
	}
	
	private void ResolveActionSelection() {
		switch (subMenuState) {
			case SubMenuState.Nil:
				break;
			case SubMenuState.BasicAttack:
//				Attack currentAttack = currentSubMenu.GetCurrentItem() as Attack;
				Debug.LogFormat("Using {0}, {1}", basicAttackMenu.GetCurrentItem().AttackOption, 
				                basicAttackMenu.AttackName());
//				BattleManager.bm.SetCurrentTarget(currentTargets.GetCurrentItem());
				BattleManager.bm.SetCurrentTarget(enemyTargets.GetCurrentItem());
				BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
				break;
			case SubMenuState.Support:
				break;
			case SubMenuState.Items:
				break;
			case SubMenuState.Allies:
				break;
			case SubMenuState.Enemies:
				break;
			case SubMenuState.All:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		// Reset menuObject state
		subMenuState = SubMenuState.Nil;
		menuOptionState = MenuOptionState.BasicAttack;
		menuState = MenuState.Main;
		initializedMenu = false;
	}

	private void NavigateMenu<T>(MenuObject<T> menuObject, Direction direction) {
		Type menuType = typeof(T);
		if (menuType == typeof(AttackButton)) {
			// Choosing an attack
			menuObject.NavigateMenu(direction);
//			Debug.LogFormat("Current attack: {0}", basicAttackMenu.AttackName());
		} else if(menuType == typeof(ActionCategoryContainer)) {
			menuObject.NavigateMenu(direction);
			ActionCategoryContainer currentItem = menuObject.GetCurrentItem() as ActionCategoryContainer;
			if (currentItem != null) {
//				currentActionImage.color = Color.grey; // Change previous action to white
				menuOptionState = currentItem.MenuOptionState();
//				currentActionImage = currentItem.OptionRender().GetComponent<Image>();
//				currentActionImage.color = Color.white; // Change current action to grey
				Debug.LogFormat("Current category = {0}", menuOptionState);
			} else {
				Debug.LogWarning("Current item was null.");
			}
		} else if (menuType == typeof(GameObject)) {
			// Choosing a target
			menuObject.NavigateMenu(direction);
			GameObject currentTarget = menuObject.GetCurrentItem() as GameObject;
			Debug.LogFormat("Current target: {0}", currentTarget.name);
		} else {
			Debug.Log("Unkown menu type.");
		}
	}
	
	private void GetAxisDown<T>(MenuObject<T> menu) {
		if (axisDown) {
//			currentDirection = Direction.Null;
			if (!(Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) ||
			    !(Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon)) return;
//			Debug.Log("Reset axis down");
			axisDown = false;
			return;
		}
		if(Input.GetAxisRaw ("Horizontal") > 0) {
//			Debug.Log ("Right");
			axisDown = true;
			const Direction dir = Direction.Right;
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Right);
		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
//			Debug.Log ("Left");
			axisDown = true;
			const Direction dir = Direction.Left;
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Left);
		} else if(Input.GetAxisRaw ("Vertical") > 0) {
//			Debug.Log ("Up");
			axisDown = true;
			const Direction dir = Direction.Up;
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Up);
		} else if(Input.GetAxisRaw ("Vertical") < 0) {
//			Debug.Log ("Down");
			axisDown = true;
			const Direction dir = Direction.Down;
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Down);
		} else {
//			Debug.Log("Reset axis down");
			axisDown = false;
		}
	}

//	private void DoAttack() {
//		switch (currentUnitWeaponType) {
//			case WeaponType.GreatSword:
//				break;
//			case WeaponType.Sword:
//				break;
//			case WeaponType.Dagger:
//				break;
//			case WeaponType.Rod:
//				break;
//			case WeaponType.Staff:
//				break;
//			case WeaponType.Bow:
//				break;
//			case WeaponType.Spear:
//				break;
//			default:
//				throw new ArgumentOutOfRangeException();
//		}
//	}
	
	private void InitAttackOptions(WeaponType type) {
		switch (type) {
			case WeaponType.GreatSword:
				basicAttackMenu.SetAttackAName(GREATSWORD_A);
				basicAttackMenu.SetAttackBName(GREATSWORD_B);
				break;
			case WeaponType.Sword:
				basicAttackMenu.SetAttackAName(SWORD_A);
				basicAttackMenu.SetAttackBName(SWORD_B);
				break;
			case WeaponType.Dagger:
				basicAttackMenu.SetAttackAName(DAGGER_A);
				basicAttackMenu.SetAttackBName(DAGGER_B);
				break;
			case WeaponType.Rod:
				basicAttackMenu.SetAttackAName(ROD_A);
				basicAttackMenu.SetAttackBName(ROD_B);
				break;
			case WeaponType.Staff:
				basicAttackMenu.SetAttackAName(STAFF_A);
				basicAttackMenu.SetAttackBName(STAFF_B);
				break;
			case WeaponType.Bow:
				basicAttackMenu.SetAttackAName(BOW_A);
				basicAttackMenu.SetAttackBName(BOW_B);
				break;
			case WeaponType.Spear:
//				attackAName = SPEAR_A;
//				attackBName = SPEAR_B;
				basicAttackMenu.SetAttackAName(SPEAR_A);
				basicAttackMenu.SetAttackBName(SPEAR_B);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private static void DoSoundEffect(MenuType menuType, Direction direction) {
		switch (menuType) {
			case MenuType.Horizontal:
				switch (direction) {
					case Direction.Right:
					case Direction.Left:
						// Make sound effect
						break;
				}
				break;
			case MenuType.Vertical:
				switch (direction) {
					case Direction.Up:
					case Direction.Down:
						// Make sound effect
						break;
				}
				break;
			case MenuType.Both:
				switch (direction) {
					case Direction.Up:
					case Direction.Down:
					case Direction.Right:
					case Direction.Left:
						// Make sound effect
						break;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("menuType", menuType, null);
		}
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
