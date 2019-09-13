using UnityEngine;

namespace Items {
	[System.Serializable]
	public abstract class Item : ScriptableObject {
		public string itemName;
		public GameObject itemObject;
		public int stackLimit;
		[SerializeField] private string description;
		
		public virtual string GetItemInfo() {
			return GetDescription();
		}
		
		protected string GetDescription() {
			return this.description;
		}

	}
}
