using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneSlots : MonoBehaviour {

	public enum Slots { Skill, Stat1, Stat2, Stat3, Stat4 };

	// key: slot (int) Slots.Slot, value: rune game object
	//private Dictionary<Slots, GameObject> runes = new Dictionary<Slots, GameObject>();
	//public Dictionary<int, GameObject> runes = new Dictionary<int, GameObject>();
	[SerializeField]
	private GameObject[] runes = new GameObject[(int)Slots.Stat4 + 1];

	public int totalRuneHp;
	public int totalRuneAtk;
	public int totalRuneDef;
	public int totalRuneSpd;

	public void GetStatCount() {
		for(int i = 1; i < runes.Length; i++) {
			if (runes[i] != null) {
				Rune rune = runes[i].GetComponent<Rune>();
				Rune.Stat stat = rune.whichStat;
				switch (stat) {
					case Rune.Stat.Hp:
						totalRuneHp += rune.statCount;
						break;
					case Rune.Stat.Atk:
						totalRuneAtk += rune.statCount;
						break;
					case Rune.Stat.Def:
						totalRuneDef += rune.statCount;
						break;
					case Rune.Stat.Spd:
						totalRuneSpd += rune.statCount;
						break;
				}
			}
		}
		//foreach(KeyValuePair<Slots, GameObject> rune in runes) {
		//	if ((int)rune.Key > 0) {
		//		Rune.Stat stat = rune.Value.GetComponent<Rune>().whichStat;
		//		switch (stat) {
		//			case Rune.Stat.HP:
		//				totalRuneHP += rune.Value.GetComponent<Rune>().statCount;
		//				break;
		//			case Rune.Stat.Atk:
		//				totalRuneAtk += rune.Value.GetComponent<Rune>().statCount;
		//				break;
		//			case Rune.Stat.Def:
		//				totalRuneDef += rune.Value.GetComponent<Rune>().statCount;
		//				break;
		//			case Rune.Stat.Spd:
		//				totalRuneSpd += rune.Value.GetComponent<Rune>().statCount;
		//				break;
		//		}
		//	}
		//}
	}

	public bool TryAddRune(GameObject rune, Slots slot) {
		if(runes[(int)slot] != null) {
			return false;	// TODO: ask if want to remove current rune and replace with new one
		}
		runes[(int)slot] = rune;
		return true;
	}
}
