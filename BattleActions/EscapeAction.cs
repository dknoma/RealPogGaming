using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum AttackType {
//	Basic,
//	Item
//}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EscapeAction", order = 1)]
public class EscapeAction : BattleAction {

//	private AttackType currentAttack;
//	private PlayerController player;

	private void OnEnable() {
		menuOption = MenuOption.Escape;
	}

	public override void DoAction(bool selectingOption) {
		BattleManager.bm.EndBattle(WinStatus.Escape);
	}
}
