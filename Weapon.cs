using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Weapon : MonoBehaviour {

	public enum HandType { TwoHanded, OneHanded };
	public enum WeaponType { GreatSword, Sword, Dagger, Staff, Bow };
	public enum WeaponSlot { MainHand, OffHand };

	// Can choose stats for the weapon in the inspector.
	public ElementalAffinity.Element weaponElement = ElementalAffinity.Element.None;
	public HandType handType;
	public WeaponType weaponType;
	public WeaponSlot weaponSlot;
	public int atk = 0;
}
