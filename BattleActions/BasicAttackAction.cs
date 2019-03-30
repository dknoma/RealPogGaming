using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum AttackType {
//	Basic,
//	Item
//}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BasicAttackAction", order = 1)]
public class BasicAttackAction : BattleAction {

//	public ActionType action;
	[SerializeField] private WeaponType weaponType;

//	private AttackType currentAttack;
//	private PlayerController player;

	private void OnEnable() {
		actionType = ActionType.Attack;
//		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	protected override void DoAction() {
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
			case WeaponType.Staff:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			case WeaponType.Bow:
				BattleManager.bm.SetCurrentActionType(ActionType.Attack);
				BattleManager.bm.BattlePhase = BattlePhase.Battle;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void SetWeaponType(WeaponType type) {
		weaponType = type;
	}
}
