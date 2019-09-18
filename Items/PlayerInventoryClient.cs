using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Items {
	public class PlayerInventoryClient : MonoBehaviour {
		// list of tabs, list of subtabs and their slots, then list of slots of rest of tabs

		// TODO: make it so that we can display a basic inventory. SetActive()
		public List<InventoryTabObject> inventorySlots = new List<InventoryTabObject>();

		private static readonly string EMPTY_STRING = "";
		
		private Inventory inventory;

		private void Start() {
//            inventorySlots.AddRange(FindObjectsOfType<Slot>());
//            inventorySlots.Sort((a,b) => a.index - b.index);
			this.inventory = Inventory.Instance;

			
			PopulateInitial();
		}

		public void PopulateInitial() {
			// iterate through all tabs
			for(int i = 0; i < inventorySlots.Count; i++) {
				InventoryTabObject tabObject = inventorySlots[i];
				// if has subtab, iterate through all the subtabs and all their slots
				if(tabObject.subTabs.Count > 0) {
					for(int j = 0; j < tabObject.subTabs.Count; j++) {
						SetItemsInTabsSlots(i, tabObject.subTabs[j].tabInventorySlots);
					} 
				} else {
					// else iterate over all slots in a tab
					SetItemsInTabsSlots(i, tabObject.tabInventorySlots);
				}
			}
		}

		public void Clear() {
			for(int i = 0; i < inventorySlots.Count; i++) {
				InventoryTabObject tabObject = inventorySlots[i];
				// if has subtab, iterate through all the subtabs and all their slots
				if(tabObject.subTabs.Count > 0) {
					for(int j = 0; j < tabObject.subTabs.Count; j++) {
						RemoveItemsFromTabsSlots(tabObject.subTabs[j].tabInventorySlots);
					} 
				} else {
					// else iterate over all slots in a tab
					RemoveItemsFromTabsSlots(tabObject.tabInventorySlots);
				}
			}
		}

//		private bool GetItemIndexByName(int tabIndex, string name) {
//			int index = this.inventory.GetItemIndexByName(tabIndex, name);
//			ItemInstance fetchedItem;
//			return this.inventory.GetItem(tabIndex, index, out fetchedItem);
//		}

		public void InsertItem(int tabIndex, ItemInstance item, int quantity) {
			bool itemExistsBeforeInsert;
			int insertedIndex = this.inventory.InsertItem(tabIndex, item, quantity, out itemExistsBeforeInsert);

			ItemInstance fetchedItem;
			if (insertedIndex > -1) {
				Slot desiredSlot = this.inventorySlots[tabIndex].tabInventorySlots[insertedIndex];
				if (!itemExistsBeforeInsert) {
					desiredSlot.SetItemNameText(item.GetItemName());
				}
				
				bool gotItem = this.inventory.GetItem(tabIndex, insertedIndex, out fetchedItem);
				if (!gotItem) {
					return;	// if item doesn't exist, then something must be wrong...
				}
				
				int itemQuantity = desiredSlot.SetItemQuantity(fetchedItem.quantity);
				Debug.LogFormat("fetched: {0}", fetchedItem);
			}
		}

		public void DecrementItemQuantity(int tabIndex, int index, int quantity) {
			bool itemExistsAfterDecrement;
			if (this.inventory.DecrementItemQuantity(tabIndex, index, quantity, out itemExistsAfterDecrement)) {
				Slot desiredSlot = this.inventorySlots[tabIndex].tabInventorySlots[index];
				int decrementedQuantity;
				if (itemExistsAfterDecrement) {
					ItemInstance item;
					this.inventory.GetItem(tabIndex, index, out item);
					decrementedQuantity = item.quantity;
				} else {
					desiredSlot.SetItemNameText(EMPTY_STRING);
					decrementedQuantity = 0;
				}
				desiredSlot.SetItemQuantity(decrementedQuantity);
			}
		}

		private void SetItemsInTabsSlots(int tabIndex, List<Slot> tabInventorySlots) {
			for(int i = 0; i < tabInventorySlots.Count; i++) {
				ItemInstance itemInstance;
				if(this.inventory.GetItem(tabIndex, i, out itemInstance)) {
					Debug.LogFormat("Instance: {0}", itemInstance.GetItemName());
					Slot currentSlot = tabInventorySlots[i];
					currentSlot.SetItem(itemInstance);
					currentSlot.SetItemNameText(itemInstance.GetItemName());
					currentSlot.SetItemQuantity(itemInstance.quantity);
				}
			}
		}
		
		private void RemoveItemsFromTabsSlots(List<Slot> tabInventorySlots) {
			for(int j = 0; j < tabInventorySlots.Count; j++) {
				tabInventorySlots[j].RemoveItem();
			}
		}
		
		[Serializable]
		public class InventoryTabObject {
			public string name;
			public GameObject frontendInstance; // frontend
			public List<InventorySubTabObject> subTabs = new List<InventorySubTabObject>();
			public List<Slot> tabInventorySlots = new List<Slot>();
		}
		
		[Serializable]
		public class InventorySubTabObject {
			public string name;
			public GameObject frontendInstance; // frontend
			public List<Slot> tabInventorySlots = new List<Slot>();
		}
		
		[Serializable]
		public class Slot {
			public SlotObject slot;
			public TextMeshProUGUI itemNameText;
			public TextMeshProUGUI itemQuantityText;
			
			private static readonly string EMPTY_STRING = "";

			public void SetItem(ItemInstance itemInstance) {
				this.slot.SetItem(itemInstance);
			}

			public string SetItemNameText(string name) {
				this.itemNameText.text = name;
				return name;
			}

			public int SetItemQuantity(int quantity) {
				this.itemQuantityText.text = quantity > 0 ? quantity.ToString() : EMPTY_STRING;
				return quantity;
			}

			public void RemoveItem() {
				this.slot.RemoveItem();
			}
		}
	}
}
