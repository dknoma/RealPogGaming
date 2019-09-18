using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Items {
	public class PlayerInventoryClient : MonoBehaviour {
		// list of tabs, list of subtabs and their slots, then list of slots of rest of tabs

		// TODO: make it so that we can display a basic inventory. SetActive()
		public List<InventoryTabObject> inventorySlots = new List<InventoryTabObject>();

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

		public void InsertItem(int tabIndex, ItemInstance item, int quantity) {
			int insertedIndex = this.inventory.InsertItem(tabIndex, item, quantity);
			if (insertedIndex > -1) {
				ItemInstance fetchedItem;
				this.inventory.GetItem(tabIndex, insertedIndex, out fetchedItem);
				Debug.LogFormat("fetched: {0}", fetchedItem);
				Slot desiredSlot = this.inventorySlots[tabIndex].tabInventorySlots[insertedIndex];
				int itemQuantity = desiredSlot.SetItemQuantity(fetchedItem);
			}
		}

		public void DecrementItemQuantity(int tabIndex, int index, int quantity) {
			if (this.inventory.DecrementItemQuantity(tabIndex, index, quantity)) {
				ItemInstance item;
				this.inventory.GetItem(tabIndex, index, out item);
				Slot desiredSlot = this.inventorySlots[tabIndex].tabInventorySlots[index];
				desiredSlot.SetItemQuantity(item);
			}
		}

		private void SetItemsInTabsSlots(int tabIndex, List<Slot> tabInventorySlots) {
			for(int i = 0; i < tabInventorySlots.Count; i++) {
				ItemInstance instance;
				if(this.inventory.GetItem(tabIndex, i, out instance)) {
					Debug.LogFormat("Instance: {0}", instance.GetItemName());
					tabInventorySlots[i].SetItem(instance);
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

			public string SetItemNameText(ItemInstance itemInstance) {
				string name;
				if (itemInstance != null) {
					name = itemInstance.GetItemName();
					this.itemNameText.text = name;
				} else {
					this.itemNameText.text = EMPTY_STRING;
					name = EMPTY_STRING;
				}
				return name;
			}

			public int SetItemQuantity(ItemInstance itemInstance) {
				int quantity;
				if (itemInstance != null) {
					quantity = itemInstance.quantity;
					this.itemQuantityText.text = quantity.ToString();
				} else {
					this.itemQuantityText.text = EMPTY_STRING;
					quantity = 0;
				}
				return quantity;
			}

			public void RemoveItem() {
				this.slot.RemoveItem();
			}
		}
	}
}
