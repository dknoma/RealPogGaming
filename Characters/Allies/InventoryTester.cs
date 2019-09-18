using Items;
using UnityEngine;

namespace Characters.Allies {
    public class InventoryTester : MonoBehaviour{
        
        private Inventory inventory;
        [SerializeField] private PlayerInventoryClient inventoryClient;


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
                inventoryClient.InsertItem(0, swordInstance, 1);
            } else if (Input.GetKeyDown(KeyCode.F)) {
                inventoryClient.DecrementItemQuantity(0, 0, 1);
            } else if (Input.GetKeyDown(KeyCode.T)) {
                string info = inventory.GetItemInfo(0,0);
                Debug.Log(info);
            } else if (Input.GetKeyDown(KeyCode.P)) {
                inventory.ClearInventory();
            } else if (Input.GetKeyDown(KeyCode.G)) {
                bool added = inventory.TryAddGold(10000);
                Debug.LogFormat("Added gold: {0}", added);
            } else if (Input.GetKeyDown(KeyCode.H)) {
                bool subtracted = inventory.TrySubtractGold(10000);
                Debug.LogFormat("Subtracted gold: {0}", subtracted);
            }
        }
    }
}
