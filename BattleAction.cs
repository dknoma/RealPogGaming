using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuingOption {
	Action,
	Submenu
}

public enum ActionType {
	Attack,
	Support,
//	Special,
	Defend,
	Escape
}

/// <inheritdoc />
/// <summary>
/// Action object. Used to make action buttons for battle action menus
/// </summary>
public class BattleAction : ScriptableObject {

	protected ActionType actionType;
	protected virtual void DoAction() {}

	public ActionType GetActionType() {
		return actionType;
	}
}
