using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : StatusEffects {

	/* Public/inspector elements */
	public int expToGrant = 10;
	public bool bogResist;
	public bool burnResist;
	public bool poisonResist;
	public bool runeLockResist;
	public bool stunResist;
	public bool silenceResist;

	protected int maxLevel = 100;
	public int currentLevel = 1;

	public int expUntilLevelUp;
	public int currentExp;

	[SerializeField]
	protected int maxHp = 20;
	[SerializeField]
	protected int currentHp;
	protected int totalRuneHp;
	[SerializeField]
	protected int baseAtk = 5;
	protected int currentAtk;
	protected int totalRuneAtk;
	[SerializeField]
	protected int baseDef = 5;
	protected int currentDef;
	protected int totalRuneDef;
	[SerializeField]
	protected int baseSpd = 5;
	protected int currentSpd;
	protected int totalRuneSpd;
	public Element element;

	//[SerializeField]
	//	protected int basePAtk = 5;
	//	protected int currentPAtk = basePAtk;
	//[SerializeField]
	//	protected int baseMAtk = 5;
	//	protected int currentMAtk = baseMAtk;
	//[SerializeField]
	//	protected int basePDef = 5;
	//	protected int currentPDef = basePDef;
	//[SerializeField]
	//	protected int baseMDef = 5;
	//	protected int currentMDef = baseMDef;


	//	private StatusEffects characterStatus = new StatusEffects();
	protected Dictionary<Status, int> statusTurnCounter = new Dictionary<Status, int>();
	protected Dictionary<StatChange, int> statChangeTurnCounter = new Dictionary<StatChange, int>();
	protected bool disableRunes;
	protected bool canAct;
	protected bool canCastSpells;
	protected bool hasStatusAffliction;


	// Status effect constants
//	protected const int Bog = (int) Status.Bog;
//	protected const int Burn = (int) Status.Burn;
//	protected const int Poison = (int) Status.Poison;
//	protected const int RuneLock = (int) Status.RuneLock;
//	protected const int Stun = (int) Status.Stun;
//	protected const int Silence = (int) Status.Silence;
//
//	protected const int Atkup = (int) StatChange.AtkUp;
//	protected const int Defup = (int) StatChange.DefUp;
//	protected const int Spdup = (int) StatChange.SpeedUp;
//	protected const int Hpup = (int) StatChange.HpUp;
//	protected const int Atkdown = (int) StatChange.AtkDown;
//	protected const int Defdown = (int) StatChange.DefDown;
//	protected const int Spddown = (int) StatChange.SpeedDown;
//	protected const int Hpdestruction = (int) StatChange.HpDestruct;

	private void Awake() {
		expUntilLevelUp = CalcNextLevel();
		currentHp = maxHp;
		currentAtk = baseAtk;
		currentDef = baseDef;
		currentSpd = baseSpd;
//		statusTurnCounter = new int[GetStatusAfflictions().Count];
//		statChangeTurnCounter = new int[GetStatChangeAfflictions().Count];
		foreach (Status status in Enum.GetValues(typeof(Status))) {
			statusTurnCounter.Add(status, 0);
		}
		foreach (StatChange statChange in Enum.GetValues(typeof(StatChange))) {
			statChangeTurnCounter.Add(statChange, 0);
		}
		canAct = true;
		canCastSpells = true;
		if(bogResist) { AddStatusResist (Status.Bog); } 
		if(burnResist) { AddStatusResist (Status.Bog); } 
		if(poisonResist) { AddStatusResist (Status.Poison); } 
		if(runeLockResist) { AddStatusResist (Status.RuneLock); } 
		if(stunResist) { AddStatusResist (Status.Stun); } 
		if(silenceResist) { AddStatusResist (Status.Silence); } 
		if(transform.GetComponentInChildren<RuneSlots>() != null) {
			transform.GetComponentInChildren<RuneSlots>().GetStatCount();
		}
	}

	/******
	 * HP *
	 ******/
	// Return a copy of this characters max hp
	public int GetMaxHp() { return 0 + maxHp; }

	// Return a copy of this characters current attack
	public int GetCurrentHp() { return 0 + currentHp; }

	// Modify current HP w/ new value
	public void ModifyHp(int hp) { currentHp += hp; }

	// A % modifier for HP
	public void ModifyHp(float hpPercentage) { currentHp += (int) Mathf.Round(maxHp * hpPercentage); }

	public int GetRuneHp() { return 0 + totalRuneHp; }

	/*******
	 * ATK *
	 *******/
	// Return a copy of this characters base attack
	public int GetBaseAtk() { return 0 + baseAtk; }

	// Return a copy of this characters current attack
	public int GetCurrentAtk() { return 0 + currentAtk; }

	// Modify current attack w/ new value
	public void ModifyAtk(bool up) { currentAtk += AtkMod(baseAtk, up); }

	public int GetRuneAtk() { return 0 + totalRuneAtk; }

	/*******
	 * DEF *
	 *******/
	// Return a copy of this characters base defense
	public int GetBaseDef() { return 0 + baseDef; }

	// Return a copy of this characters current defense
	public int GetCurrentDef() { return 0 + currentDef; }

	// Modify current defense w/ new value
	public void ModifyDef(bool up) { currentDef += DefMod(baseDef, up); }

	public int GetRuneDef() { return 0 + totalRuneDef; }

	/*******
	 * SPD *
	 *******/
	// Return a copy of this characters base defense
	public int GetBaseSpd() { return 0 + baseSpd; }

	// Return a copy of this characters current defense
	public int GetCurrentSpd() { return 0 + currentSpd; }

	// Modify current defense w/ new value
	public void ModifySpd(bool up) { currentSpd += DefMod(baseSpd, up); }

	public int GetRuneSpd() { return 0 + totalRuneSpd; }

	/* 
	 * Status effects
	 */
	public bool Incapacitate() {
		if(!AfflictedByStatus(Status.Incapacitated)) {
			// TODO: afflict the state
			AfflictStatus(Status.Incapacitated);
			hasStatusAffliction = true;
			return true;
		}
		return false;
	}

	public bool IsAfflictedBy(Status status) {
		return afflictedStatuses[status];
	}
	
	public bool TryStatusAffliction(Status status, int turnsToAfflict) {
		if(!DoesResistStatus(status) && !hasStatusAffliction && !AfflictedByStatus(status)) {
			// TODO: afflict the state
			AfflictStatus(status);
			statusTurnCounter[status] = turnsToAfflict;
			hasStatusAffliction = true;
			return true;
		}
		return false;
	}

	public bool TryStatChange(StatChange statChange, int turnsToAfflict) {
		if(!DoesResistStatChange(statChange) && !AfflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			AfflictStatChange(statChange);
			statChangeTurnCounter [statChange] = turnsToAfflict;
			return true;
		}
		return false;
	}

	public bool TryRemoveStatus(Status status) {
		if(!DoesResistStatusRemoval(status) && !AfflictedByStatus(status)) {
			// TODO: afflict the stat change
			RemoveStatus(status);
			statusTurnCounter [status] = 0;
			return true;
		}
		return false;
	}

	public bool TryRemoveStatChange(StatChange statChange) {
		if(!DoesResistStatChangeRemoval(statChange) && !AfflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			RemoveStatChange(statChange);
			statChangeTurnCounter [statChange] = 0;
			return true;
		}
		return false;
	}

	public bool DoesResistStatus(Status status) {
		return ResistsStatusEffect(status);
	}

	public bool DoesResistStatChange(StatChange statChange) {
		return ResistsStatChange(statChange);
	}

	public bool DoesResistStatusRemoval(Status status) {
		return ResistsStatusEffectRemoval(status);
	}

	public bool DoesResistStatChangeRemoval(StatChange statChange) {
		return ResistsStatChangeRemoval(statChange);
	}

	/**
	 * Initial state of the affliction. This method is used to initialize effects that only happen once
	 * to the character. checkStatusAfflictions() is used for effects that happen each turn.
	 */ 
	public void InitAffliction(Status status, int turns) {
		// Check status afflictions and do action depending on the affliction
		if(TryStatusAffliction (status, turns)) {
			switch (status) {
				case Status.Bog:
					if(TryStatChange(StatChange.SpeedDown, turns)) {
						ModifySpd (false);
					} else {
						Debug.Log("Already afflicted by speed down...");
					}
					break;
				case Status.Burn:
					if(TryStatChange(StatChange.AtkDown, turns)) {
						ModifyAtk (false);
					} else {
						Debug.Log("Already afflicted by physical attack down...");
					}
					break;
				case Status.Poison:
					if(TryStatChange(StatChange.AtkDown, turns)) {
						ModifyAtk (false);
					} else {
						Debug.Log("Already afflicted by magical attack down...");
					}
					break;
				case Status.RuneLock:
					disableRunes = true;
					break;
				case Status.Stun:
					canAct = false;
					break;
				case Status.Silence:
					canCastSpells = false;
					break;
				case Status.Incapacitated:
					canAct = false;
					break;
				default:
					throw new ArgumentOutOfRangeException("status", status, null);
			}
			hasStatusAffliction = true;
		} else {
			Debug.Log("Already afflicted with status " + status);
		}
	}

	/**
	 * Removes the specified affliction. This method reverses any 
	 */ 
	public void RemoveAffliction(Status status) {
		switch (status) {
			case Status.Bog:
				if(TryRemoveStatus(Status.Bog)) {
//					hasStatusAffliction = false;
				}
				if(TryRemoveStatChange (StatChange.SpeedDown)) {
					ModifySpd (true);// Only need to happen once, not every turn
				}
				break;
			case Status.Burn:
				ModifyAtk (true); // Only need to happen once, not every turn
				if(TryRemoveStatus(Status.Burn)){
//					hasStatusAffliction = false;
				}
				// Tries to remove atk down ifnot afflicted with a stat change removal resist
				if(TryRemoveStatChange (StatChange.AtkDown)) {
					ModifyAtk (true);// Only need to happen once, not every turn
				}
				break;
			case Status.Poison:
				if(TryRemoveStatus (Status.Poison)){
//					hasStatusAffliction = false;
				}
				if(TryRemoveStatChange (StatChange.AtkDown)) {
					ModifyAtk (true);
				}
				break;
			case Status.RuneLock:
				if(TryRemoveStatus(Status.RuneLock)) {
//					hasStatusAffliction = false;
					disableRunes = false;
				}
				break;
			case Status.Stun:
				if(TryRemoveStatus(Status.Stun)) {
//					hasStatusAffliction = false;
					canAct = true;
				}
				break;
			case Status.Silence:
				if(TryRemoveStatus(Status.Silence)) {
//					hasStatusAffliction = false;
					canCastSpells = true;
				}
				break;
			case Status.Incapacitated:
				if(TryRemoveStatus(Status.Incapacitated)) {
//					hasStatusAffliction = false;
					canAct = true;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
		}
	}

	public void RemoveAllAfflictions() {
		foreach(Status status in Enum.GetValues(typeof(Status))) {
			switch (status) {
				case Status.Bog:
					if(TryRemoveStatChange (StatChange.SpeedDown)) {
						ModifySpd (true);// Only need to happen once, not every turn
					}
					if(TryRemoveStatus(Status.Bog)) {
						hasStatusAffliction = false;
					}
					break;
				case Status.Burn:
					ModifyAtk (true); // Only need to happen once, not every turn
					if(TryRemoveStatus(Status.Burn)) {
						hasStatusAffliction = false;
					}
				// Tries to remove atk down ifnot afflicted with a stat change removal resist
					if(TryRemoveStatChange (StatChange.AtkDown)) {
						ModifyAtk (true);// Only need to happen once, not every turn
					}
					break;
				case Status.Poison:
					if(TryRemoveStatus(Status.Poison)) {
						hasStatusAffliction = false;
					}
					if(TryRemoveStatChange (StatChange.AtkDown)) {
						ModifyAtk (true);
					}
					break;
				case Status.RuneLock:
					if(TryRemoveStatus(Status.RuneLock)) {
						hasStatusAffliction = false;
						disableRunes = false;
					}
					break;
				case Status.Stun:
					if(TryRemoveStatus(Status.Stun)) {
						hasStatusAffliction = false;
					}
					canAct = true;
					break;
				case Status.Silence:
					if(TryRemoveStatus(Status.Silence)) {
						hasStatusAffliction = false;
					}
					canCastSpells = true;
					break;
				}
		}
	}

	/**
	 * Check this characters status afflictions. This method is meant to be executed at the beginning of each of
	 * this character's turns.
	 */ 
	public void CheckStatusAfflictions() {
//		for(int i = 0; i < afflictedStatuses.Count; i++) {
		foreach(KeyValuePair<Status, bool> status in afflictedStatuses) {
			// Check status afflictions and do action depending on the affliction
			switch (status.Key) {
				case Status.Bog:
					//Nothing planned atm for this status effect. Only SPDDown atm.
					break;
				case Status.Burn:
					// 8% DoT & p atk down
	//				if(this.statusTurnCounter [BURN] > 0) {
	//					modifyHP (-0.08f);
	//				}
					break;
				case Status.Poison:
					// 10% DoT & m atk down
	//				if(this.statusTurnCounter [BURN] > 0) {
	//					modifyHP (-0.1f);
	//				}
					break;
				case Status.RuneLock:
					// Can't use runes for x turns
					if(afflictedStatuses[Status.RuneLock]) {
						disableRunes = true;
					}
					break;
				case Status.Stun:
					// Can't act for x turns
					if(afflictedStatuses[Status.Stun]) {
						canAct = false;
					}
					break;
				case Status.Silence:
					// Can't cast spells for x turns. Can still use skills
					if(afflictedStatuses[Status.Silence]) {
						canCastSpells = false;
					}
					break;
				case Status.Incapacitated:
					if(afflictedStatuses[Status.Incapacitated]) {
						canAct = false;
					}
					break;
				default:
					Debug.Log ("Affliction not valid");
					break;
			}
		}
	}

	/**
	 * Check this characters status afflictions. This method is meant to be executed at the end of each of
	 * this character's turns.
	 */ 
	public void ResolveStatusAfflictions() {
//		for(int i = 0; i < afflictedStatuses.Length; i++) {
		foreach(KeyValuePair<Status, bool> status in afflictedStatuses) {
			// Check status afflictions and do action depending on the affliction
			switch (status.Key) {
				case Status.Bog:
					//Nothing planned atm for this status effect. Only SPDDown atm.
					break;
				case Status.Burn:
					// 8% DoT & p atk down
					Status s = Status.Burn;
					if(statusTurnCounter [s] > 0) {
						ModifyHp (-0.08f);
						statusTurnCounter[s] -= 1;
						if(statusTurnCounter[s] == 0) {
							TryRemoveStatus (s);
						}
					}
					break;
				case Status.Poison:
					// 10% DoT & m atk 
					s = Status.Poison;
					if(statusTurnCounter [s] > 0) {
						ModifyHp (-0.1f);
						statusTurnCounter[s] -= 1;
						if(statusTurnCounter[s] == 0) {
							TryRemoveStatus (s);
						}
					}
					break;
				case Status.RuneLock:
					s = Status.RuneLock;
					statusTurnCounter[s] -= 1;
					if(statusTurnCounter[s] == 0) {
						TryRemoveStatus(s);
					}
					break;
				case Status.Stun:
					// Can't act for x turns
					s = Status.Stun;
					if(statusTurnCounter[s] > 0) {
						statusTurnCounter[s] -= 1;
						if(statusTurnCounter[s] == 0) {
							TryRemoveStatus (s);
						}
					}
					break;
				case Status.Silence:
					s = Status.Silence;
					// Can't cast spells for x turns. Can still use skills
					if(statusTurnCounter[s] > 0) {
						statusTurnCounter[s] -= 1;
						if(statusTurnCounter[s] == 0) {
							TryRemoveStatus (s);
						}
					}
					break;
				default:
					Debug.Log ("Affliction not valid");
					break;
			}
		}
	}

//	public void tryStatUp(StatUps statUp) {
//		if(!status.alreadyAfflictedStatUp[statUp]) {
//			// TODO: afflict the stat up
//		}
//	}
//
//	public void tryStatDown(StatDowns statDown) {
//		if(!status.alreadyAfflictedStatDown[statDown]) {
//			// TODO: afflict the stat down
//		}
//	}

	public void GrantExp(int exp) {
		int totalExp = currentExp + exp;
		int expToNextLevel = CalcNextLevel();
		Debug.LogFormat("\tGained {0} exp, my total exp: {1}, remaining exp: {2}, total needed to go to next {3}",
		                exp, totalExp, expToNextLevel - totalExp, expToNextLevel);
		if(totalExp < expToNextLevel) {
			currentExp = totalExp;
			expUntilLevelUp = expToNextLevel - currentExp;
			Debug.Log(string.Format("\t{0} has gained {1} exp! exp until next level: {2}",
			                        name, exp, expUntilLevelUp));
		} else {
			currentLevel += 1;
			currentExp = totalExp - expToNextLevel;
			expUntilLevelUp = CalcNextLevel() - currentExp;	// Recalculate for next level
			Debug.Log(string.Format("\t{0} has leveled up: {1}, exp until next level: {2}", 
			                        name, currentLevel, expUntilLevelUp));
		}
		//int remainingExp = exp - this.expUntilLevelUp;
//		Debug.Log(string.Format("\tcurrent {0}, exp until next level: {1}", currentExp, expUntilLevelUp));
//		int remainingExp = expUntilLevelUp - totalExp;
//		Debug.LogFormat("\tGained {0} exp, my total exp: {1}, remaining exp: {2}, total needed to go to next {3}",
//		                exp, totalExp, remainingExp, CalcNextLevel());
//		if(remainingExp > 0) {
//			currentExp = totalExp;
//			expUntilLevelUp = remainingExp;
//			Debug.Log(string.Format("\t{0} has gained {1} exp! exp until next level: {2}",
//				name, exp, expUntilLevelUp));
//		} else {
//			currentLevel += 1;
//			currentExp = Mathf.Abs(remainingExp);
//			expUntilLevelUp = CalcNextLevel() - currentExp;
//			Debug.Log(string.Format("\t{0} has leveled up: {1}, exp until next level: {2}", 
//				name, currentLevel, expUntilLevelUp));
//		}
	}

	// Increase/decrease atk by 25% of the base value
	private int AtkMod(int baseAtk, bool up) {
		return (int) (up ? baseAtk * 0.25f : baseAtk * (-0.25f));
	}

	// Increase/decrease defense by 25% of the base value
	private int DefMod(int baseDef, bool up) {
		return (int) (up ? baseDef * 0.25f : baseDef * (-0.25f));
	}

	// Increase/decrease speed by 20% of the base value
	private int SpdMod(int baseSpd, bool up) {
		return (int) (up ? baseSpd * 0.2f : baseSpd * (-0.2f));
	}

	// Calculate the amount of exp required to get to the next level
	// 1->2: 1, 2->3: 5, 3->4: 33, 4->5: 73, ...
	private int CalcNextLevel() {
		return (int) Mathf.Round(4+15*Mathf.Pow(currentLevel, 3)/14);
	}
}
