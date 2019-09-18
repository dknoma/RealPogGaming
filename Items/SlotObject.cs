using System;
using TMPro;
using UnityEngine;

namespace Items {
    public class SlotObject : MonoBehaviour {
        public int index;
        public ItemInstance itemInstance; // backend
        public GameObject prefabInstance; // frontend
        
        public void SetItem(ItemInstance itemInstance) {
            this.itemInstance = itemInstance;
            prefabInstance = Instantiate(itemInstance.item.itemObject, transform);
        }
        
        public void RemoveItem() {
            this.itemInstance = null;
//            this.prefabInstance.SetActive(false);
            Destroy(this.prefabInstance);
            this.prefabInstance = null;
        }
    }
}
