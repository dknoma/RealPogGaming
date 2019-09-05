using UnityEngine;

namespace Items {

//	enum ItemType {
//		Weapon,
//		Armor,
//		Accessory,
//		Consumable,
//		Quest
//	}
	[System.Serializable]
	public abstract class Item : ScriptableObject {
		public string itemName;
		public GameObject itemObject;
	}
}
