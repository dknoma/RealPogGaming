using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Affiliation {
	Ally,
	Enemy
}
//[RequireComponent(typeof(BattleActions))]
public class Character : CharacterStats, IComparable {

	public Sprite icon;	// Icon to display inside of the turn queue
	
	[SerializeField] private Affiliation affiliation;
	private Element attackElement;
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
}
