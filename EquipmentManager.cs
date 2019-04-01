using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

	public static EquipmentManager em;
	
	private Dictionary<string, CharacterEquipement> characterEquipements = new Dictionary<string, CharacterEquipement>();
	// TODO: changing characters equipment

	private void OnEnable() {
		if (em == null) {
			em = this;
		} else if (em != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		// TODO: use json to store data in a file
		CreateAllCharacterEquipmentObject();
	}

	public void CreateAllCharacterEquipmentObject() {
		foreach (Character character in PlayerManager.pm.GetCharacters()) {
			characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
		}
	}

	public void AddCharacterEquipmentObject(Character character) {
		characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
	}
}
