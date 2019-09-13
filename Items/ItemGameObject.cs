using UnityEngine;

namespace Items {
	public class ItemGameObject : MonoBehaviour {
		
		[SerializeField] private ItemInstance itemInstance;	// Holds a reference to an instance of an item

		private void Awake() {
			Instantiate(itemInstance.item.itemObject, this.transform);
		}

		public ItemInstance GetItemInstance() {
			return this.itemInstance;
		}
	}
}
