using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuOption {
	Attack,
	Support,
	Special,
	Defend,
	Escape
}

public enum ActionType {
	Attack,
	Heal,
	Revive,
	Buff,
	Defend,
	Escape
}

/// <inheritdoc />
/// <summary>
/// Action object. Used to make action buttons for battle action menus
/// </summary>
public class BattleAction : ScriptableObject {

	protected MenuOption menuOption;
	public virtual void DoAction() {}
	public virtual void SetWeapon(Weapon weapon) {}
	public MenuOption GetMenuOptionType() {
		return menuOption;
	}
	public virtual WeaponType GetWeaponType() {
		return WeaponType.Sword;
	}
	
	public virtual Element GetElement() {
		return Element.None;
	}
}
