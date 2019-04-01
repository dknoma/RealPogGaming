using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "WeaponValues/ValuesSkeleton", order = 1)]
public class WeaponValues : ScriptableObject {


	// Can choose stats for the weapon in the inspector.
	[SerializeField] private Element weaponElement = Element.None;
	private HandType handType;
	[SerializeField] private WeaponType weaponType;
	private WeaponSlot weaponSlot;
	[SerializeField] private int hp;
	[SerializeField] private int atk;
	[SerializeField] private int def;
	[SerializeField] private int spd;
	
	public void SetWeaponType(WeaponType type) {
		weaponType = type; 
	}
	public void SetWeaponElement(Element element) {
		weaponElement = element;
	}
	public WeaponType GetWeaponType() {
		return weaponType; 
	}
	public Element GetWeaponElement() {
		return weaponElement;
	}
}
