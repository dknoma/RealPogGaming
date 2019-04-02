using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(fileName = "Equipment", menuName = "CharacterEquipement/Equipment", order = 1)]
public class CharacterEquipement : MonoBehaviour {
	/** Advanced Equipment ****************************************************************/
	// If wearing overall, then no chest or bottom; If Chest and/or bottom, then no overall
//	enum AdvArmorSlots { Head, Overall, Chest, Bottom, Shoes, Gloves };
//	enum AdvWeaponSlots { MainHand, OffHand };
//	enum AdvAccessorySlots { Pendant, Ring1, Ring2, Earring };
	/**************************************************************************************/

	/** Simple Equipment ******************************************************************/
	// Armor encompasses chest, bottom, shoes
	public enum ArmorSlots { Head, Armor, Gloves }
	public enum WeaponSlots { MainHand, OffHand }
	public enum AccessorySlots { Slot1, Slot2 }
	/**************************************************************************************/

	[SerializeField] private Armor headgear;
	[SerializeField] private Armor armor;
	[SerializeField] private Armor gloves;
	[SerializeField] private Weapon weapon;
	[SerializeField] private Accessory pendant;
	[SerializeField] private Accessory ring;
	[SerializeField] private Accessory earring;

	private int totalHp;
	private int totalAtk;
	private int totalDef;
	private int totalSpd;
	private int runeHp;
	private int runeAtk;
	private int runeDef;
	private int runeSpd;

	public void SetWeapon(Weapon newWeapon) {
		weapon = newWeapon;
	}
	
	public Weapon GetWeapon() {
		return weapon;
	}
}
