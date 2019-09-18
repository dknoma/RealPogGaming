using System.Text;
using UnityEngine;

namespace Items {
	[System.Serializable]
	[CreateAssetMenu(menuName = "Items/Inventory", fileName = "Inventory.asset")]
	public class Inventory : ScriptableObject {
		private static Inventory _inventory;

		public static Inventory Instance {
			get {
				if(!_inventory) {
					Inventory[] tmp = Resources.FindObjectsOfTypeAll<Inventory>();
					if(tmp.Length > 0) {
						_inventory = tmp[0];
						Debug.Log("Found inventory as: " + _inventory);
					} else {
						Debug.Log("Did not find inventory, loading from file or template.");
						SaveManager.LoadOrInitializeInventory();
					}
				}

				return _inventory;
			}
		}
	
		/* Inventory Management */
		public static void InitializeFromDefault() {
			if(_inventory) {
				DestroyImmediate(_inventory);
			}
			_inventory = Instantiate((Inventory) Resources.Load("InventoryTemplate"));
			_inventory.hideFlags = HideFlags.HideAndDontSave;
		}

		public static void LoadFromJson(string path) {
			if(_inventory) {
				DestroyImmediate(_inventory);
			}
			_inventory = CreateInstance<Inventory>();
			JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), _inventory);
			_inventory.hideFlags = HideFlags.HideAndDontSave;
		}

		public void SaveToJson(string path) {
			Debug.LogFormat("Saving inventory to {0}", path);
			System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
		}

		/// <summary>
		/// Inventory Start
		/// </summary>
		public static int MAXIMUM_GOLD_CAPACITY = 999999;
		
		public InventoryTab[] inventoryTabs;
		public int goldCount;

		/// <summary>
		/// Get an item if it exists.
		/// </summary>
		/// <param name="tabIndex">Index of the specific inventory tab.</param>
		/// <param name="index">Desired index to get an ItemInstance.</param>
		/// <param name="item">The desired item.</param>
		/// <returns>Returns true if item was found and passes out the item that was found.</returns>
		public bool GetItem(int tabIndex, int index, out ItemInstance item) {
			bool gotItem;
			// inventory[index] doesn't return null, so check item instead.
			if (index > -1) {
				gotItem = inventoryTabs[tabIndex].GetItem(index, out item);
			} else {
				item = null;
				gotItem = false;
			}
			return gotItem;
		}
		
		/// <summary>
		/// Get an item's information if it exists.
		/// </summary>
		/// <param name="tabIndex">Index of the specific inventory tab.</param>
		/// <param name="index">Desired index to get an ItemInstance.</param>
		/// <returns>Returns the information of an item if it exists.</returns>
		public string GetItemInfo(int tabIndex, int index) {
			return inventoryTabs[tabIndex].GetItemInfo(index);
		}

		/// <summary>
		/// Remove an item at an index if one exists at that index. Checks if the indicated tab is a super tab category.
		/// </summary>
		/// <param name="tabIndex">Index of the specified inventory tab.</param>
		/// <param name="index">Desired index to remove an item from.</param>
		/// <returns>Returns true if an item was successfully removed.</returns>
		public bool RemoveItem(int tabIndex, int index) {
			return inventoryTabs[tabIndex].SubTabsEmpty() && inventoryTabs[tabIndex].RemoveItem(index);
		}

		/// <summary>
		/// Decrement an item's quantity at an index if one exists at that index. Checks if the indicated tab is a super tab category.
		/// </summary>
		/// <param name="tabIndex">Index of the specified inventory tab.</param>
		/// <param name="index">Desired index of the item to decrement.</param>
		/// <returns>Returns true if an item was successfully decremented.</returns>
		public bool DecrementItemQuantity(int tabIndex, int index) {
			return inventoryTabs[tabIndex].SubTabsEmpty() && inventoryTabs[tabIndex].DecrementItemQuantity(index);
		}

		public bool DecrementItemQuantity(int tabIndex, int index, int quantity, out bool itemExists) {
			bool tabHasNoSubtabs = inventoryTabs[tabIndex].SubTabsEmpty();
			bool decrementedItemQuantity;
			if (tabHasNoSubtabs) {
				decrementedItemQuantity =
					inventoryTabs[tabIndex].DecrementItemQuantity(index, quantity, out itemExists);
			} else {
				decrementedItemQuantity = false;
				itemExists = false;
			}
			return decrementedItemQuantity;
		}
		
		// Insert an item, return the index where it was inserted.  -1 if error.
		public int InsertItem(int tabIndex, ItemInstance item, int quantity, out bool itemExistsBeforeInsert) {
			bool itemIsNotNull = item != null;
			int index;
			if (itemIsNotNull) {
				index = inventoryTabs[tabIndex].InsertItem(item, quantity, out itemExistsBeforeInsert);
			} else {
				index = -1;
				itemExistsBeforeInsert = false;
			}
			return index;
		}

		public int GetItemIndexByName(int tabIndex, string name) {
			return this.inventoryTabs[tabIndex].GetItemIndexByName(name);
		}

		public int GetGoldCount() {
			return this.goldCount;
		}

		public bool TryAddGold(int goldCount) {
			bool canAddGold;
			if (this.goldCount == MAXIMUM_GOLD_CAPACITY) {
				canAddGold = false;
			} else {
				int total = this.goldCount + goldCount;
				this.goldCount = total > MAXIMUM_GOLD_CAPACITY || this.goldCount > MAXIMUM_GOLD_CAPACITY ? 
					MAXIMUM_GOLD_CAPACITY : total;
				canAddGold = true;
			}

			return canAddGold;
		}
		
		public bool TrySubtractGold(int goldCount) {
			bool canSubtractGold;
			if (this.goldCount == 0) {
				canSubtractGold = false;
			} else {
				int total = this.goldCount - goldCount;
				this.goldCount = total < 0 || this.goldCount < 0 ? 0 : total;
				canSubtractGold = true;
			}

			return canSubtractGold;
		}
		
		// Simply save.
		private void Save() {
			SaveManager.SaveInventory();
		}

		// FOR TESTING ONLY
		public void ClearInventory() {
			bool removed;
			for (int i = 0; i < inventoryTabs.Length; i++) {
				for (int j = 0; j < inventoryTabs[i].tabInventory.Length; j++) {
					removed = RemoveItem(i, j);
//					Debug.LogFormat("removed item at {0}: {1}", j, removed);
				}
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < inventoryTabs.Length; i++) {
				sb.AppendFormat("{{{0}}}, ", inventoryTabs[i]);
			}
			return string.Format("Inventory: [{0}]", sb);
		}
	}
}
