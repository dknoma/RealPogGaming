using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

	public WeaponType weaponType;

	private Vector3 cursorSpot;

//	private void Awake() {
//		
//	}

	// Enemies don't necessarily have weapons, but having a weapon type may affect damage output in battle
	// Maybe we want this maybe we dont.
	public WeaponType GetWeaponType() {
		return weaponType;
	}
}
