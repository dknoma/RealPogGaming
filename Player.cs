using UnityEngine;

public class Player : Character {

    private int partySlot;
    private CharacterEquipement equipement;
    private Weapon weapon;
    
    private void OnEnable () {
        equipement = gameObject.GetComponent<CharacterEquipement>();
        Debug.LogFormat("{0} weapon {1}", name, equipement.GetWeapon());
        SetWeapon(equipement.GetWeapon());
    }
    
    public Weapon GetWeapon() {
        return weapon;
    }

    public void SetWeapon(Weapon newWeapon) {
        weapon = newWeapon;
        SetAttackElement(weapon.GetWeaponElement());
    }
    public int GetPartySlot() {
        return partySlot;
    }

    public void SetPartySlot(int slot) {
        partySlot = slot;
    }
}
