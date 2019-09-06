using System.Collections.Generic;
using UnityEngine;

namespace Items {
	public class PlayerInventoryClient : MonoBehaviour {
		// list of tabs, list of subtabs and their slots, then list of slots of rest of tabs
		// 0
		//     0    
		//        100
		//     1
		//        100
		//     2
		//        100
		// 1
		//    100
		// 2
		//    100
		// 3
		//    100
		// 4
		//    100

		public List<InventoryTabObject> inventorySlots;

		private void Start() {
			inventorySlots = new List<InventoryTabObject>();
			
//            inventorySlots.AddRange(FindObjectsOfType<Slot>());
//            inventorySlots.Sort((a,b) => a.index - b.index);
			
			PopulateInitial();
		}

		public void PopulateInitial() {
			// iterate through all tabs
			for(int i = 0; i < inventorySlots.Count; i++) {
				InventoryTabObject tabObject = inventorySlots[i];
				// if has subtab, iterate through all the subtabs and all their slots
				if(tabObject.subTabs.Count > 0) {
					for(int j = 0; j < tabObject.subTabs.Count; j++) {
						SetItemsInTabsSlots(tabObject.subTabs[j].tabInventorySlots);
					} 
				} else {
					// else iterate over all slots in a tab
					SetItemsInTabsSlots(tabObject.tabInventorySlots);
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

		private void SetItemsInTabsSlots(List<Slot> tabInventorySlots) {
			for(int i = 0; i < tabInventorySlots.Count; i++) {
				ItemInstance instance;
				if(Inventory.Instance.GetItem(i, i, out instance)) {
					tabInventorySlots[i].SetItem(instance);
				}
			}
		}
		
		private void RemoveItemsFromTabsSlots(List<Slot> tabInventorySlots) {
			for(int j = 0; j < tabInventorySlots.Count; j++) {
				tabInventorySlots[j].RemoveItem();
			}
		}
	}
}
