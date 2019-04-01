using UnityEngine;

public enum HandType { 
	TwoHanded, 
	OneHanded 
}

public enum WeaponType { 
	GreatSword, 
	Sword, 
	Dagger, 
	Rod,
	Staff, 
	Bow,
	Spear
}

public enum WeaponSlot { 
	MainHand, 
	OffHand 
}
public class Weapon : MonoBehaviour {

//	[SerializeField] protected WeaponValues values;	// could use weaponvalues to store updated values and json
	[SerializeField] protected Element weaponElement = Element.None;
	private HandType handType;
	[SerializeField] protected WeaponType weaponType;
	private WeaponSlot weaponSlot;
	[SerializeField] protected int hp;
	[SerializeField] protected int atk;
	[SerializeField] protected int def;
	[SerializeField] protected int spd;
	[SerializeField] protected bool basicAttack;
	[SerializeField] protected bool altAttack;
	[SerializeField] protected bool specialAttack;
	
	private void OnEnable() {
		Debug.LogFormat("{0}", name);
	}
	
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
