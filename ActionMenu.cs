using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum MenuState {
	Main,			// Main menu: where player selects what option they wanna take
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

	public GameObject cursor;
	
	public GameObject attackOptionPrefab;
	public GameObject runOptionPrefab;

	private Image currentActionImage;
	private WeaponType currentUnitWeaponType;

	private List<ActionCategoryContainer> actions;
	private MenuGraph<ActionCategoryContainer> mainMenu;
	private MenuGraph<GameObject> enemies;
	private MenuGraph<object> basicAttacks;
	private MenuGraph<object> currentSubMenu;
	private MenuGraph<GameObject> currentTargets;
	
	private MenuState menuState;
	private MenuOptionState menuOptionState;
	private SubMenuState subMenuState;
	private GameObject currentBattleOptionRender;
//	private Direction currentDirection;
	private bool axisDown;
	private bool inSubMenu;
	private bool initializedMenu;

	private GameObject attackOption;
	private GameObject runOption;
	
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

//	private void OnEnable() {
//		// Only instantiate objects when in battle
//		if (!BattleManager.bm.InBattle()) return;
//		switch (BattleManager.bm.GetCurrentUnit().GetAffiliation()) {
//			case Affiliation.Ally:
//				Debug.Log("Current unit is an ally.");
//				InitMenu();
//				break;
//			case Affiliation.Enemy:
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
				List<GameObject> allies = BattleManager.bm.GetAllies();
				int randomTarget = Random.Range(0, allies.Count);
				BattleManager.bm.SetCurrentTarget(allies[randomTarget]);
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.SetBattlePhase(BattlePhase.Battle);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void InitMenu() {
		Debug.Log("Init action menu.");
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
		Debug.Log("action menu size: " + actionCount);
		mainMenu = new MenuGraph<ActionCategoryContainer>(actionCount, MenuType.Horizontal);
		mainMenu.InitMenuItems(new ActionCategoryContainer[actionCount]);
		for(int i = 0; i < actionCount; i++) {
			mainMenu.AddItem(actions[i], i);
			mainMenu.GetItem(i).OptionRender().GetComponent<Image>().color = Color.grey;
		}
//		currentMenu = mainMenu;
//		currentBattleOptionRender = mainMenu.GetCurrentItem().OptionRender();
//		currentActionImage = currentBattleOptionRender.GetComponent<Image>();
		currentActionImage = mainMenu.GetCurrentItem().OptionRender().GetComponent<Image>();
		currentActionImage.color = Color.white;

		menuOptionState = mainMenu.GetCurrentItem().MenuOptionState();
		
		// Get current enemy list. Useful for targeting enemies
		int enemyCount = BattleManager.bm.GetEnemies().Count;
		enemies = new MenuGraph<GameObject>(enemyCount, MenuType.Both);
		enemies.InitMenuItems(new GameObject[enemyCount]);
		for(int i = 0; i < enemyCount; i++) {
			enemies.AddItem(BattleManager.bm.GetEnemies()[i], i);
		}
		currentUnitWeaponType = BattleManager.bm.GetCurrentUnit().GetWeapon().GetWeaponType();
		InitAttackOptions(currentUnitWeaponType);
		Debug.Log("Unit's weapon ====== " + currentUnitWeaponType);
		basicAttacks = new MenuGraph<object>(2, MenuType.Vertical);
		basicAttacks.InitMenuItems(new object[2]);
		basicAttacks.AddItem(new Attack(AttackOption.A, attackAName), 0);
		basicAttacks.AddItem(new Attack(AttackOption.B, attackBName), 1);
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
//				Debug.Log("Choosing menu category.");
				GetAxisDown(mainMenu);
				if (Input.GetButtonDown("Test")) {
					ChangeMenu();
				}
				break;
			case MenuState.SubMenu:
//				Debug.Log("Choosing sub menu category.");
				GetAxisDown(currentSubMenu);
				if (Input.GetButtonDown("Test")) {
					DoMenuAction();
				}
				// TODO: If hit cancel, go back to main
				break;
			case MenuState.Target:
//				Debug.Log("Choosing target.");
				GetAxisDown(currentTargets);
				if (Input.GetButtonDown("Test")) {
					ChooseTarget();
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ChangeMenu() {
		Debug.LogFormat("menu option state: {0}", menuOptionState);
		switch (menuOptionState) {
			case MenuOptionState.BasicAttack:
				Debug.Log("Chose  basic attack.");
				currentSubMenu = basicAttacks;
				Debug.LogFormat("Current attack: {0}", currentSubMenu.GetCurrentItem() as string);
				menuState = MenuState.SubMenu;
				break;
			case MenuOptionState.Support:
				menuState = MenuState.SubMenu;
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
				Debug.Log("Choosing target.");
				currentTargets = enemies;
				Debug.LogFormat("current taget: {0}",currentTargets.GetCurrentItem().name);
				menuState = MenuState.Target;
				subMenuState = SubMenuState.BasicAttack;
				// Current targets = enemies
				break;
			case MenuOptionState.Support:
//				menuState = MenuState.SubMenu;
				// Change menu to submenu of items and defense
				break;
			case MenuOptionState.Escape:
				BattleManager.bm.EndBattle(WinStatus.Escape);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ChooseTarget() {
		GameObject target = currentTargets.GetCurrentItem();
		Debug.Log("Chose target " + BattleManager.bm.GetCurrentTarget());
		subMenuState = SubMenuState.BasicAttack;
		ResolveActionSelection();
	}
	
	private void ResolveActionSelection() {
		switch (subMenuState) {
			case SubMenuState.Nil:
				break;
			case SubMenuState.BasicAttack:
				Attack currentAttack = currentSubMenu.GetCurrentItem() as Attack;
				Debug.LogFormat("Using {0}", currentAttack.Name());
				BattleManager.bm.SetCurrentTarget(currentTargets.GetCurrentItem());
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
		// Reset menu state
		subMenuState = SubMenuState.Nil;
		menuOptionState = MenuOptionState.BasicAttack;
		menuState = MenuState.Main;
		initializedMenu = false;
	}
	
	private void NavigateMenu<T>(MenuGraph<T> menu, Direction direction) {
		Type menuType = typeof(T);
		if(menuType == typeof(ActionCategoryContainer)) {
			menu.TraverseOptions(direction);
			ActionCategoryContainer currentItem = menu.GetCurrentItem() as ActionCategoryContainer;
			if (currentItem != null) {
				currentActionImage.color = Color.grey; // Change previous action to white
				menuOptionState = currentItem.MenuOptionState();
				currentActionImage = currentItem.OptionRender().GetComponent<Image>();
				currentActionImage.color = Color.white; // Change current action to grey
				Debug.LogFormat("Current category = {0}", currentItem.MenuOptionState());
			} else {
				Debug.LogWarning("Current item was null.");
			}
		} else if (menuType == typeof(GameObject)) {
			menu.TraverseOptions(direction);
			GameObject currentTarget = menu.GetCurrentItem() as GameObject;
			Debug.LogFormat("Current target: {0}", currentTarget.name);
		} else if (menuType == typeof(object)) {
			menu.TraverseOptions(direction);
			Attack currentAttack = menu.GetCurrentItem() as Attack;
			Debug.LogFormat("Current attack: {0}", currentAttack.Name());
		}
//		Debug.Log("direction: " + direction);
////		currentMenu.TraverseOptions(direction);
//		menu.TraverseOptions(direction);
//		currentActionImage.color = Color.grey; // Change previous action to white
////		currentActionImage = mainMenu.GetCurrentItem();
////		Debug.Log("asdasda: " + currentMenu.GetCurrentItem().name);
//		menuOptionState = menu.GetCurrentItem().MenuOptionState();
//		currentActionImage = menu.GetCurrentItem().OptionRender().GetComponent<Image>();
//		currentActionImage.color = Color.white; // Change current action to grey
//		currentBattleOptionRender = menu.GetCurrentItem().OptionRender();
//		currentActionImage = currentBattleOptionRender.GetComponent<Image>();
	}

	private void GetAxisDown<T>(MenuGraph<T> menu) {
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
//			DoSoundEffect(menu.GetMenuType(), dir);
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Right);
		} else if(Input.GetAxisRaw ("Horizontal") < 0) {
//			Debug.Log ("Left");
			axisDown = true;
			const Direction dir = Direction.Left;
//			DoSoundEffect(menu.GetMenuType(), dir);
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Left);
		} else if(Input.GetAxisRaw ("Vertical") > 0) {
//			Debug.Log ("Up");
			axisDown = true;
			const Direction dir = Direction.Up;
//			DoSoundEffect(menu.GetMenuType(), dir);
			NavigateMenu(menu, dir);
//			NavigateMenu(Direction.Up);
		} else if(Input.GetAxisRaw ("Vertical") < 0) {
//			Debug.Log ("Down");
			axisDown = true;
			const Direction dir = Direction.Down;
//			DoSoundEffect(menu.GetMenuType(), dir);
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

	private class ActionCategoryContainer {

		private readonly GameObject optionRender;
		private readonly MenuOptionState menuOptionState;

		public ActionCategoryContainer(GameObject obj, MenuOptionState option) {
			optionRender = obj;
			menuOptionState = option;
		}

		public GameObject OptionRender() {
			return optionRender;
		}

		public MenuOptionState MenuOptionState() {
			return menuOptionState;
		}
	}

	private class Attack {

		private readonly AttackOption attackOption;
		private readonly string name;

		public Attack(AttackOption atk, string name) {
			attackOption = atk;
			this.name = name;
		}

		public AttackOption Option() {
			return attackOption;
		}

		public string Name() {
			return name;
		}
	}
}
