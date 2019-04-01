using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public static PlayerManager pm;
    
    private List<Character> characters = new List<Character>();
    
    private void OnEnable() {
        if (pm == null) {
            pm = this;
        } else if (pm != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Character mc = GameObject.Find("MC").GetComponent<Character>();
        AddPlayableCharacter(mc);
    }

    public void AddPlayableCharacter(Character character) {
        characters.Add(character);
    }

    public List<Character> GetCharacters() {
        return characters;
    }
}
