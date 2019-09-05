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

		/* Inventory Start */
		public InventoryTab[] inventoryTabs;

		// Get an item if it exists.
		public bool GetItem(int tabIndex, int index, out ItemInstance item) {
			// inventory[index] doesn't return null, so check item instead.
			return inventoryTabs[tabIndex].GetItem(index, out item);
		}

		// Remove an item at an index if one exists at that index.
		public bool RemoveItem(int tabIndex, int index) {
			return inventoryTabs[tabIndex].RemoveItem(index);
		}

		// Insert an item, return the index where it was inserted.  -1 if error.
		public int InsertItem(int tabIndex, ItemInstance item) {
			return inventoryTabs[tabIndex].InsertItem(item);
		}

		// Simply save.
		private void Save() {
			SaveManager.SaveInventory();
		}
	}
}
