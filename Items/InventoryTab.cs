using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Items {
	[Serializable]
	public class InventoryTab {
		public string tabName;
		public InventorySubTab[] subTabs;
		public ItemInstance[] tabInventory;
		
		private Dictionary<string, int> itemIndexByName = new Dictionary<string, int>();
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

		// Get an item if it exists.
		public string GetItemInfo(int index) {
			// inventory[index] doesn't return null, so check item instead.
			string info = "N/A";
			if (!SlotEmpty(index)) {
				info = tabInventory[index].GetItemInfo();
			}
			return info;
		}
		
		public bool DecrementItemQuantity(int index) {
			if (SlotEmpty(index)) {
				// Nothing existed at the specified slot.
				return false;
			}
			ItemInstance item = tabInventory[index];
			int decrementedItemQuantity = tabInventory[index].quantity - 1;
			bool removed;
			int itemQuantity = tabInventory[index].quantity;
			if (decrementedItemQuantity > 0) {
				Debug.LogFormat("Decrementing {0}.", GetItemInfo(index));
//				tabInventory[index].quantity--;
				removed = true;
			} else if (decrementedItemQuantity == 0) {
				Debug.LogFormat("Decrementing {0}.", GetItemInfo(index));
//				item.quantity = 0;
//				tabInventory[index] = null;
				RemoveItem(index);
				removed = true;
			} else {
				decrementedItemQuantity = 0;
				removed = false;
			}
			item.quantity = decrementedItemQuantity;
			return removed;
		}
		
		public bool DecrementItemQuantity(int index, int decrementCount, out bool itemExists) {
			if (SlotEmpty(index)) {
				// Nothing existed at the specified slot.
				itemExists = false;
				return false;
			}
			Debug.LogFormat("Decrementing {0}.", GetItemInfo(index));
			ItemInstance item = tabInventory[index];
			int decrementedItemQuantity = tabInventory[index].quantity - decrementCount;
			bool decremented;
			if (decrementedItemQuantity > 0) {
//				tabInventory[index].quantity = decrementedItemQuantity;
				decremented = true;
				itemExists = true;
			} else if (decrementedItemQuantity == 0) {
				RemoveItem(index);
//				item.quantity = 0;
//				tabInventory[index] = null;
				decremented = true;
				itemExists = false;
			} else {
				decrementedItemQuantity = 0;
				decremented = false;
				itemExists = false;
			}
			item.quantity = decrementedItemQuantity;
			return decremented;
		}

		// Remove an item at an index if one exists at that index.
		public bool RemoveItem(int index) {
			if (SlotEmpty(index)) {
				// Nothing existed at the specified slot.
				return false;
			}
			Debug.LogFormat("Removing {0}.", GetItemInfo(index));
//			tabInventory[index].quantity = 0;
			ItemInstance item = tabInventory[index];
			item.quantity = 0;
			itemIndexByName.Remove(item.GetItemName());
			tabInventory[index] = null;
			return true;
		}

		// Insert an item, return the index where it was inserted.  -1 if error.
		public int InsertItem(ItemInstance item, int quantity, out bool itemExistsBeforeInsert) {
			int index = -1;
			string itemName = item.GetItemName();
			int stackLimit = item.item.stackLimit;
			itemExistsBeforeInsert = ItemExists(itemName);
			// If an item is in the inventory and can stack it, increase its quantity
			if (itemExistsBeforeInsert && CanStack(stackLimit)) {
				index = this.itemIndexByName[itemName];
				ItemInstance desiredItem = tabInventory[index];
				// If hasn't reached stack limit, increase stack quantity.
				if (!ReachedStackLimit(desiredItem.quantity, stackLimit)) {
					desiredItem.quantity += quantity; // update stack quantity
					Debug.LogFormat("Increased {0}'s quantity at index {1} in tab {2}.", item.GetItemName(), index,
					                this.tabName);
				} else {
					Debug.LogFormat("Reached stack limit of {0}.", stackLimit);
				}
			} else {
				// Else insert the item into the next available slot
				for (int i = 0; i < tabInventory.Length; i++) {
					if (!SlotEmpty(i)) continue;
					tabInventory[i] = item;
					index = i;
					itemIndexByName.Add(itemName, i);
					tabInventory[i].quantity += quantity;
					Debug.LogFormat("Not in inventory - Inserted {0} at index {1} in tab {2}.", item.GetItemName(), index,
					                this.tabName);
					break;
				}
			}

			// Couldn't find a free slot.
			return index;
		}

		public int GetItemIndexByName(string name) {
			return itemIndexByName.ContainsKey(name) ? itemIndexByName["name"] : -1;
		}

		/// <summary>
		/// Check if an item is already in the inventory
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private bool ItemExists(string name) {
			return itemIndexByName.ContainsKey(name);
		}

		/// <summary>
		/// Check if you can add
		/// </summary>
		/// <param name="itemQuantity"></param>
		/// <param name="stackLimit"></param>
		/// <returns></returns>
		private bool ReachedStackLimit(int itemQuantity, int stackLimit) {
			return itemQuantity == stackLimit;
		}
		
		
		/// <summary>
		/// Check if you can add
		/// </summary>
		/// <param name="stackLimit"></param>
		/// <returns></returns>
		private bool CanStack(int stackLimit) {
			return stackLimit > 1;
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
