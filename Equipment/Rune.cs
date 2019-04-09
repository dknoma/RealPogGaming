using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rune : MonoBehaviour {

	public enum Type { Skill, Stat };
	public enum StatType { Flat, Percent };
	public enum Stat { Hp, Atk, Def, Spd };

	public Type runeType;
	public StatType statType;
	public Stat whichStat;
	public int statCount;
	public int statPercent;
}
