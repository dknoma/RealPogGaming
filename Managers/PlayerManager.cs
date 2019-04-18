using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public static PlayerManager pm;
    
    [SerializeField] private GameObject mainCharacter;
    [SerializeField] private GameObject otherCharacter;
    
    private readonly List<Player> characters = new List<Player>();
    private readonly List<Player> partyMembers = new List<Player>();
    
    private Dictionary<string, CharacterEquipement> characterEquipements = new Dictionary<string, CharacterEquipement>();
//    private List<PartyMember> allyReserves = new List<PartyMember>();
    private const int MAX_PARTY_MEMBERS = 3;
    private int incapacitatedCount;
    public int IncapacitatedCount {
        get { return incapacitatedCount; }
    }

    private void OnEnable() {
        if (pm == null) {
            pm = this;
        } else if (pm != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
//        AddPlayableCharacter(mainCharacter.GetComponent<Player>());
        AddAllyToParty(mainCharacter.GetComponent<Player>());
        AddAllyToParty(otherCharacter.GetComponent<Player>());
//        CreateAllCharacterEquipmentObject();
    }

    public void AddPlayableCharacter(Player player) {
        characters.Add(player);
    }
    
    public void AddAllyToParty(Player newAlly) {
        if(partyMembers.Count > MAX_PARTY_MEMBERS) {
            // If party member count > MAX_PARTY_MEMBERS, move those members to the reserves
            for (int i = partyMembers.Count - 1; i >= MAX_PARTY_MEMBERS; i--) {
                partyMembers.RemoveAt(i);
            }
            characters.Add(newAlly);
            return;
        }
        if (partyMembers.Count == MAX_PARTY_MEMBERS) {
            // If party count == MAX_PARTY_MEMBERS, add new ally to reserves
            characters.Add(newAlly);
            return;
        }
        newAlly.SetInParty(true);
        partyMembers.Add(newAlly);
        newAlly.SetPartySlot(partyMembers.Count-1);
        characters.Add(newAlly);
    }
    
    public List<Player> GetCharacters() {
        Debug.Log("GETTING LIST "+characters[0]);
        return characters;
    }
    
    public List<Player> GetParty() {
        return partyMembers;
    }

    public int GetPartyCount() {
        return partyMembers.Count;
    }
    
    public int AllyCount() {
        return partyMembers.Count;
    }

    public bool AllAlliesIncapacitated() {
        return incapacitatedCount == MAX_PARTY_MEMBERS;
    }

    public void IncapacitateUnit(Character unit) {
        unit.Incapacitate();
        incapacitatedCount = incapacitatedCount+1 <= 3 ? incapacitatedCount+1 : MAX_PARTY_MEMBERS;
    }
    
    public void ReviveAllyFlatHp(Character unit, int hp) {
        unit.TryRemoveStatus(Status.Incapacitated);
        unit.ModifyHp(hp);
        incapacitatedCount = incapacitatedCount - 1 >= 0 ? incapacitatedCount - 1 : 0;
    }
    
    public void ReviveAllyPercentHp(Character unit, float hpPercent) {
        unit.TryRemoveStatus(Status.Incapacitated);
        unit.ModifyHp(hpPercent);
        incapacitatedCount = incapacitatedCount - 1 >= 0 ? incapacitatedCount - 1 : 0;
    }
//    public void CreateAllCharacterEquipmentObject() {
//        List<Player> chars = GetCharacters();
//        foreach (Player character in chars) {
//            characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
//        }
//    }
//
//    public void AddCharacterEquipmentObject(Player character) {
//        characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
//    }
}