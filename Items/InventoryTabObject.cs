using System.Collections.Generic;

namespace Items {
	[System.Serializable]
	public class InventoryTabObject {
		public string name;
		public List<InventoryTabObject> subTabs = new List<InventoryTabObject>();
		public List<Slot> tabInventorySlots = new List<Slot>();
		
	}
}
