﻿using System.Text;

namespace Items {
	[System.Serializable]
	public class InventoryTab {
		public string tabName;
		public InventorySubTab[] subTabs;
		public ItemInstance[] tabInventory;
		// TODO - add functionality for stacking items
		// 		- prevent stacking of certain kinds of items too
		
		public bool SlotEmpty(int index) {
			return tabInventory[index] == null || tabInventory[index].item == null;
		}

		public bool SubTabsEmpty() {
			return subTabs == null || subTabs.Length == 0;
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

		public int SwapItems(int secondIndex, int firstIndex, ItemInstance item) {
			ItemInstance otherItem;
			GetItem(secondIndex, out otherItem);
			tabInventory[firstIndex] = otherItem;
			tabInventory[secondIndex] = item;
			return secondIndex;
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Tab: \"{0}\", items: [", tabName);
			if(subTabs.Length > 0) {
				for(int i = 0; i < subTabs.Length; i++) {
					sb.AppendFormat("{{{0}}}, ",subTabs[i]);
				}
				sb.Remove(sb.Length-1, 1);
			} else {
				for(int i = 0; i < tabInventory.Length; i++) {
					sb.AppendFormat("{0}, ",tabInventory[i]);
				}
				sb.Remove(sb.Length-1, 1);
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
}
