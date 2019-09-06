using Items;
using UnityEngine;

namespace Characters.Allies {
    public class InventoryTester : MonoBehaviour{
        
        [SerializeField]
        private Inventory inventory;

        private void Awake() {
            inventory = Inventory.Instance;
        }

        private void Update() {
            if (Input.GetKey(KeyCode.E)) {
                SaveManager.SaveInventory();
            }
        }
    }
}
