using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandType { 
	TwoHanded, 
	OneHanded 
};

public enum WeaponType { 
	GreatSword, 
	Sword, 
	Dagger, 
	Staff, 
	Bow 
};

public enum WeaponSlot { 
	MainHand, 
	OffHand 
};

[ExecuteInEditMode]
public class Weapon : MonoBehaviour {


	// Can choose stats for the weapon in the inspector.
	public Element weaponElement = Element.None;
	public HandType handType;
	public WeaponType weaponType;
	public WeaponSlot weaponSlot;
	public int atk = 0;
}
