namespace Items {
	[System.Serializable]
	public class InventoryTab {
		public string tabName;
		public InventoryTab[] subTabs;
		public ItemInstance[] tabInventory;
		
		public bool SlotEmpty(int index) {
			return tabInventory[index] == null || tabInventory[index].item == null;
		}

		// Get an item if it exists.
		public bool GetItem(int index, out ItemInstance item) {
			// inventory[index] doesn't return null, so check item instead.
			if (SlotEmpty(index)) {
				item = null;
				return false;
			}

			item = tabInventory[index];
			return true;
		}

		// Remove an item at an index if one exists at that index.
		public bool RemoveItem(int index) {
			if (SlotEmpty(index)) {
				// Nothing existed at the specified slot.
				return false;
			}

			tabInventory[index] = null;
			return true;
		}

		// Insert an item, return the index where it was inserted.  -1 if error.
		public int InsertItem(ItemInstance item) {
			int index = -1;
			for (int i = 0; i < tabInventory.Length; i++) {
				if(!SlotEmpty(i)) continue;
				tabInventory[i] = item;
				index = i;
				break;
			}

			// Couldn't find a free slot.
			return index;
		}
	}
}
