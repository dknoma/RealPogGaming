using System;using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.Events;

public enum Affiliation {
	Ally,
	Enemy
}
//[RequireComponent(typeof(BattleActions))]
public class Character : CharacterStats, IComparable {

	public GameObject icon;	// Icon to display inside of the turn queue
	
	[SerializeField] private Affiliation affiliation;
	private Element attackElement;

	protected readonly UnityEvent actionEvent = new UnityEvent();
	protected readonly UnityEvent attackAEvent = new UnityEvent();
	protected readonly UnityEvent attackBEvent = new UnityEvent();
	protected readonly UnityEvent supportEvent = new UnityEvent();
//	private bool incapacitated;
    
//	public bool Incapacitated {
//		get { return incapacitated; }
//		set { incapacitated = value; }
//	}
	
	// Use this for initialization
//	private void OnEnable () {
//		equipement = gameObject.GetComponent<CharacterEquipement>();
//		Debug.LogFormat("{0} weapon {1}", name, equipement.GetWeapon());
//		SetWeapon(equipement.GetWeapon());
//	}

	/**
	 * Compare to method that helps sort units by speed.
	 */ 
	int IComparable.CompareTo(object obj) {
		if (obj == null) {
			return 1;
		} 
		Character otherCharacter = obj as Character;
		if(otherCharacter != null) {
			return currentSpd - otherCharacter.currentSpd;
		}
		throw new ArgumentException("Object is not a Character...");
	}

	public Affiliation GetAffiliation() {
		return affiliation;
	}
	
	public bool CanCharacterAct() {
		return canAct;
	}

	public Element GetAttackElement() {
		return attackElement;
	}
	
	public void SetAttackElement(Element ele) {
		attackElement = ele;
	}

	public void BuffAtk() {
		Debug.LogFormat("Modding attack");
		TryStatChange(StatChange.AtkUp, 3);
	}
	
	protected bool TryDamage(int dmg, GameObject target) {
		// TODO: Check if target can be damaged.
		Debug.Log(string.Format("\t{0} dealt {1} damage to {2}", name, dmg, target.name));
		Character unit = target.GetComponent<Character>();
		unit.ModifyCurrentHp(-dmg);
		Debug.Log(string.Format("{0}'s HP: {1}", target.name, unit.GetCurrentHp()));
		// If HP is still positive, return
//		if (unit.GetCurrentHp() > 0) return true;
////		unit.TryIncapacitate();	// Incapacitate the unit. Cannot act until revived
////		units.Remove(target); // Remove unit from the list if no more HP
////		RemoveTargetFromQueue(unit);
//		Debug.LogFormat("contains {0}: {1}", target, units.Contains(target));
//		// TODO: if party member, make incapacitated: can be revived
//		if(target.CompareTag("Enemy")) {
//			RemoveTargetFromQueue(unit); // Remove enemy from queue, does not perform any more actions
//			enemies.Remove(target);
//			target.SetActive(false);
//			expToGive += unit.expToGrant;
//			if(enemies.Count == 0) {
//				EndBattle(WinStatus.Win);
//			}
//		}
//		if(target.CompareTag("Ally")) {
////			Player player = target.GetComponent<Player>();
////			PlayerStatBar statBar = UIManager.um.GetPlayerStatBar(player.slot);
////			statBar.UpdateHpBar(0);
////			PlayerManager.pm.IncapacitateUnit(unit.GetComponent<Player>());
//
//			// Allies do not get removed from queue in case their turn comes around when revived
//			if(PlayerManager.pm.AllAlliesIncapacitated()) {
//				EndBattle(WinStatus.Lose);
//			}
//		}
		return true;
	}
	
	protected int CalculateDamage(GameObject unit) {
		Character target = unit.GetComponent<Character>();
		// Calculate the current units rune bonuses
		CalculateRuneStats();
		int damage = Mathf.RoundToInt(
			// Standard atk-def calc
			(Mathf.Pow(currentAtk, 2) + totalRuneAtk) /(target.GetCurrentDef() + target.GetRuneDef()) 								
			// Level compensation
			*(1 +(currentLevel*2 - target.currentLevel) / 50)			
			// Elemental multiplier
			* ElementalAffinity.CalcElementalDamage(attackElement, target.element));  														
		return damage;
	}
	
	protected int CalculateSingleHitDamage(GameObject unit) {
		Character target = unit.GetComponent<Character>();
//		Debug.Log(string.Format("{0}'s atk: {1}, {2}'s def: {3}", 
//			source.name, source.GetCurrentAtk(), target.name, target.GetCurrentDef()));
//		Debug.Log(string.Format("{0}'s attack element: {1}, {2} element: {3}", 
//			source.name, source.GetAttackElement(), target.name, target.element));
		// Calculate the current units rune bonuses
		CalculateRuneStats();
		int damage = Mathf.RoundToInt(
			// Standard atk-def calc
			(Mathf.Pow(currentAtk, 2) + totalRuneAtk) /(target.GetCurrentDef() + target.GetRuneDef()) 								
			// Level compensation
			*(1 +(currentLevel*2 - target.currentLevel) / 50)			
			// Elemental multiplier
			* ElementalAffinity.CalcElementalDamage(attackElement, target.element));  														
		return damage;
	}		
	
	protected int CalculateMultiHitDamage(int attackValue, GameObject unit) {
		Character target = unit.GetComponent<Character>();
//		Debug.Log(string.Format("{0}'s atk: {1}, {2}'s def: {3}", 
//			source.name, source.GetCurrentAtk(), target.name, target.GetCurrentDef()));
//		Debug.Log(string.Format("{0}'s attack element: {1}, {2} element: {3}", 
//			source.name, source.GetAttackElement(), target.name, target.element));
		// Calculate the current units rune bonuses
		CalculateRuneStats();
		int damage = Mathf.RoundToInt(
			// Standard atk-def calc
			Mathf.Pow(attackValue, 2) / (target.GetCurrentDef() + target.GetRuneDef())					
			// Level compensation
			*(1 +(currentLevel*2 - target.currentLevel) / 50)			
			// Elemental multiplier
			* ElementalAffinity.CalcElementalDamage(attackElement, target.element));  														
		return damage;
	}		
	
	protected void CalculateRuneStats() {
//		RuneSlots slots = currentUnit.GetComponent<RuneSlots>();
	}

	public void AddActionListener(UnityAction call) {
		actionEvent.AddListener(call);
	}
	
	public void RemoveActionListener(UnityAction call) {
		actionEvent.RemoveListener(call);
	}
	
	/// <summary>
	/// Attack A event
	/// </summary>
	public virtual void DoAttackA(GameObject target) {
		// TODO: Create sub classes for each unit. maybe even make one for enemies (obviously would do attacks automatically)
		//		 	Maybe move attack names and stuff to this class as well.
		//			Attacks need target(s)
		//			Support skills need target(s) as well
		//				targets probably chosen from Buttons during battle
		//				Perform action on target, invoke action event to listeners (battle manager in this case)
		attackAEvent.Invoke();
	}
	
	public virtual void DoAttackB(GameObject target) {
		attackBEvent.Invoke();
	}

	public virtual void DoSupport(GameObject target) {
		supportEvent.Invoke();
	}

	protected virtual void SwordA() {
	}
	
	protected virtual void SwordB() {
	}

	protected virtual void Support() {
	}
}
