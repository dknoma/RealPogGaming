using Items;
using UnityEngine;

namespace Characters.Allies {
    public class InventoryTester : MonoBehaviour{
        
        [SerializeField]
        private Inventory inventory;

        public GameObject sword;
        private ItemInstance swordInstance;

        private void Awake() {
            inventory = Inventory.Instance;
            swordInstance = sword.GetComponent<ItemGameObject>().GetItemInstance();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                SaveManager.SaveInventory();
            } else if (Input.GetKeyDown(KeyCode.R)) {
                inventory.InsertItem(0, swordInstance, 1);
            }
        }
    }
}
