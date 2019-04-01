using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public static PlayerManager pm;
    
    private List<Character> characters = new List<Character>();
    private Dictionary<string, CharacterEquipement> characterEquipements = new Dictionary<string, CharacterEquipement>();
    
    private void OnEnable() {
        if (pm == null) {
            pm = this;
        } else if (pm != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Character mc = GameObject.Find("MC").GetComponent<Character>();
        AddPlayableCharacter(mc);
        CreateAllCharacterEquipmentObject();
    }

    public void AddPlayableCharacter(Character character) {
        characters.Add(character);
    }
    
    public List<Character> GetCharacters() {
        Debug.Log("GETTING LIST "+characters[0]);
        return characters;
    }
    
    public void CreateAllCharacterEquipmentObject() {
        List<Character> chars = GetCharacters();
        foreach (Character character in chars) {
            characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
        }
    }

    public void AddCharacterEquipmentObject(Character character) {
        characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
    }

}
