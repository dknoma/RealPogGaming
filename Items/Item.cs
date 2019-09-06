using UnityEngine;

namespace Items {
	[System.Serializable]
	public abstract class Item : ScriptableObject {
		public string itemName;
		public GameObject itemObject;
	}
}
