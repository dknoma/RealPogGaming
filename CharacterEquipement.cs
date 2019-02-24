using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipement : MonoBehaviour {


	/** Advanced Equipment ****************************************************************/
	// If wearing overall, then no chest or bottom; If Chest and/or bottom, then no overall
//	enum AdvArmorSlots { Head, Overall, Chest, Bottom, Shoes, Gloves };
//	enum AdvWeaponSlots { MainHand, OffHand };
//	enum AdvAccessorySlots { Pendant, Ring1, Ring2, Earring };
	/**************************************************************************************/

	/** Simple Equipment ******************************************************************/
	// Armor encompasses chest, bottom, shoes
	public enum ArmorSlots { Head, Armor, Gloves };
	public enum WeaponSlots { MainHand, OffHand };
	public enum AccessorySlots { Pendant, Ring, Earring };
	/**************************************************************************************/

	public GameObject headgear;
	public GameObject armor;
	public GameObject gloves;
	public GameObject weapon;
	public GameObject pendant;
	public GameObject ring;
	public GameObject earring;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
