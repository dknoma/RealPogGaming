using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : StatusEffects {

	/* Public/inspector elements */
	public int expToGrant = 10;
	public bool bogResist = false;
	public bool burnResist = false;
	public bool poisonResist = false;
	public bool runeLockResist = false;
	public bool stunResist = false;
	public bool silenceResist = false;

	protected int maxLevel = 100;
	public int currentLevel = 1;

	public int expUntilLevelUp;
	public int currentExp = 0;

	[SerializeField]
	protected int maxHp = 20;
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
	protected int[] statusTurnCounter;
	protected int[] statChangeTurnCounter;
	protected bool disableRunes = false;
	protected bool canAct;
	protected bool canCastSpells;
	protected bool hasStatusAffliction = false;


	// Status effect constants
	protected const int Bog = (int) Status.Bog;
	protected const int Burn = (int) Status.Burn;
	protected const int Poison = (int) Status.Poison;
	protected const int RuneLock = (int) Status.RuneLock;
	protected const int Stun = (int) Status.Stun;
	protected const int Silence = (int) Status.Silence;

	protected const int Atkup = (int) StatChange.AtkUp;
	protected const int Defup = (int) StatChange.DefUp;
	protected const int Spdup = (int) StatChange.SpeedUp;
	protected const int Hpup = (int) StatChange.HpUp;
	protected const int Atkdown = (int) StatChange.AtkDown;
	protected const int Defdown = (int) StatChange.DefDown;
	protected const int Spddown = (int) StatChange.SpeedDown;
	protected const int Hpdestruction = (int) StatChange.HpDestruct;

	private void Awake() {
		expUntilLevelUp = CalcNextLevel();
		currentHp = maxHp;
		currentAtk = baseAtk;
		currentDef = baseDef;
		currentSpd = baseSpd;
		statusTurnCounter = new int[GetStatusAfflictions().Length];
		statChangeTurnCounter = new int[GetStatChangeAfflictions().Length];
		canAct = true;
		canCastSpells = true;
		if (bogResist) { AddStatusResist (Bog); } 
		if (burnResist) { AddStatusResist (Burn); } 
		if (poisonResist) { AddStatusResist (Poison); } 
		if (runeLockResist) { AddStatusResist (RuneLock); } 
		if (stunResist) { AddStatusResist (Stun); } 
		if (silenceResist) { AddStatusResist (Silence); } 
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
	public bool TryStatusAffliction(int status, int turnsToAfflict) {
		if(!DoesResistStatus(status) && !hasStatusAffliction && !AfflictedByStatus(status)) {
			// TODO: afflict the state
			AfflictStatus(status);
			statusTurnCounter[status] = turnsToAfflict;
			hasStatusAffliction = true;
			return true;
		}
		return false;
	}

	public bool TryStatChange(int statChange, int turnsToAfflict) {
		if(!DoesResistStatChange(statChange) && !AfflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			AfflictStatChange(statChange);
			statChangeTurnCounter [statChange] = turnsToAfflict;
			return true;
		}
		return false;
	}

	public bool TryRemoveStatus(int status) {
		if(!DoesResistStatusRemoval(status) && !AfflictedByStatus(status)) {
			// TODO: afflict the stat change
			RemoveStatus(status);
			statusTurnCounter [status] = 0;
			return true;
		}
		return false;
	}

	public bool TryRemoveStatChange(int statChange) {
		if(!DoesResistStatChangeRemoval(statChange) && !AfflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			RemoveStatChange(statChange);
			statChangeTurnCounter [statChange] = 0;
			return true;
		}
		return false;
	}

	public bool DoesResistStatus(int status) {
		return ResistsStatusEffect(status);
	}

	public bool DoesResistStatChange(int statChange) {
		return ResistsStatChange(statChange);
	}

	public bool DoesResistStatusRemoval(int status) {
		return ResistsStatusEffectRemoval(status);
	}

	public bool DoesResistStatChangeRemoval(int statChange) {
		return ResistsStatChangeRemoval(statChange);
	}

	/**
	 * Initial state of the affliction. This method is used to initialize effects that only happen once
	 * to the character. checkStatusAfflictions() is used for effects that happen each turn.
	 */ 
	public void InitAffliction(int status, int turns) {
		// Check status afflictions and do action depending on the affliction
		if (TryStatusAffliction (status, turns)) {
			switch (status) {
			case Bog:
				if(TryStatChange(Spddown, turns)) {
					ModifySpd (false);
				} else {
					Debug.Log("Already afflicted by speed down...");
				}
				break;
			case Burn:
				if(TryStatChange(Atkdown, turns)) {
					ModifyAtk (false);
				} else {
					Debug.Log("Already afflicted by physical attack down...");
				}
				break;
			case Poison:
				if(TryStatChange(Atkdown, turns)) {
					ModifyAtk (false);
				} else {
					Debug.Log("Already afflicted by magical attack down...");
				}
				break;
			case RuneLock:
				disableRunes = true;
				break;
			case Stun:
				canAct = false;
				break;
			case Silence:
				canCastSpells = false;
				break;
			}
			hasStatusAffliction = true;
		} else {
			Debug.Log("Already afflicted with status " + status);
		}
	}

	/**
	 * Removes the specified affliction. This method reverses any 
	 */ 
	public void RemoveAffliction(int status) {
		switch (status) {
		case Bog:
			if(TryRemoveStatus(Bog)) {
				hasStatusAffliction = false;
			}
			if (TryRemoveStatChange (Spddown)) {
				ModifySpd (true);// Only need to happen once, not every turn
			}
			break;
		case Burn:
			ModifyAtk (true); // Only need to happen once, not every turn
			if(TryRemoveStatus(Burn)){
				hasStatusAffliction = false;
			}
			// Tries to remove atk down if not afflicted with a stat change removal resist
			if (TryRemoveStatChange (Atkdown)) {
				ModifyAtk (true);// Only need to happen once, not every turn
			}
			break;
		case Poison:
			if(TryRemoveStatus (Poison)){
				hasStatusAffliction = false;
			}
			if (TryRemoveStatChange (Atkdown)) {
				ModifyAtk (true);
			}
			break;
		case RuneLock:
			if(TryRemoveStatus(Stun)) {
				hasStatusAffliction = false;
				disableRunes = false;
			}
			break;
		case Stun:
			if(TryRemoveStatus(Stun)) {
				hasStatusAffliction = false;
				canAct = true;
			}
			break;
		case Silence:
			if(TryRemoveStatus(Silence)) {
				hasStatusAffliction = false;
				canCastSpells = true;
			}
			break;
		}
	}

	public void RemoveAllAfflictions() {
		for (int status = 0; status < afflictedStatuses.Length; status++) {
			switch (status) {
			case Bog:
				if (TryRemoveStatChange (Spddown)) {
					ModifySpd (true);// Only need to happen once, not every turn
				}
				if(TryRemoveStatus(Bog)) {
					hasStatusAffliction = false;
				}
				break;
			case Burn:
				ModifyAtk (true); // Only need to happen once, not every turn
				if(TryRemoveStatus(Burn)) {
					hasStatusAffliction = false;
				}
			// Tries to remove atk down if not afflicted with a stat change removal resist
				if (TryRemoveStatChange (Atkdown)) {
					ModifyAtk (true);// Only need to happen once, not every turn
				}
				break;
			case Poison:
				if(TryRemoveStatus(Poison)) {
					hasStatusAffliction = false;
				}
				if (TryRemoveStatChange (Atkdown)) {
					ModifyAtk (true);
				}
				break;
			case RuneLock:
				if(TryRemoveStatus(Stun)) {
					hasStatusAffliction = false;
					disableRunes = false;
				}
				break;
			case Stun:
				if(TryRemoveStatus(Stun)) {
					hasStatusAffliction = false;
				}
				canAct = true;
				break;
			case Silence:
				if(TryRemoveStatus(Silence)) {
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
		for(int i = 0; i < afflictedStatuses.Length; i++) {
			// Check status afflictions and do action depending on the affliction
			switch (i) {
			case Bog:
				//Nothing planned atm for this status effect. Only SPDDown atm.
				break;
			case Burn:
				// 8% DoT & p atk down
//				if (this.statusTurnCounter [BURN] > 0) {
//					modifyHP (-0.08f);
//				}
				break;
			case Poison:
				// 10% DoT & m atk down
//				if (this.statusTurnCounter [BURN] > 0) {
//					modifyHP (-0.1f);
//				}
				break;
			case RuneLock:
				// Can't use runes for x turns
				if(afflictedStatuses[RuneLock]) {
					disableRunes = true;
				}
				break;
			case Stun:
				// Can't act for x turns
				if(afflictedStatuses[Stun]) {
					canAct = false;
				}
				break;
			case Silence:
				// Can't cast spells for x turns. Can still use skills
				if(afflictedStatuses[Silence]) {
					canCastSpells = false;
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
		for(int i = 0; i < afflictedStatuses.Length; i++) {
			// Check status afflictions and do action depending on the affliction
			switch (i) {
				case Bog:
					//Nothing planned atm for this status effect. Only SPDDown atm.
					break;
				case Burn:
					// 8% DoT & p atk down
					if (statusTurnCounter [Burn] > 0) {
						ModifyHp (-0.08f);
						statusTurnCounter[Burn] -= 1;
						if(statusTurnCounter[Burn] == 0) {
							TryRemoveStatus (Burn);
						}
					}
					break;
				case Poison:
					// 10% DoT & m atk down
					if (statusTurnCounter [Poison] > 0) {
						ModifyHp (-0.1f);
						statusTurnCounter[Poison] -= 1;
						if(statusTurnCounter[Poison] == 0) {
							TryRemoveStatus (Poison);
						}
					}
					break;
				case RuneLock:
					statusTurnCounter[RuneLock] -= 1;
					if (statusTurnCounter[RuneLock] == 0) {
						TryRemoveStatus(RuneLock);
					}
					break;
				case Stun:
					// Can't act for x turns
					if(statusTurnCounter[Stun] > 0) {
						statusTurnCounter[Stun] -= 1;
						if(statusTurnCounter[Stun] == 0) {
							TryRemoveStatus (Stun);
						}
					}
					break;
				case Silence:
					// Can't cast spells for x turns. Can still use skills
					if(statusTurnCounter[Silence] > 0) {
						statusTurnCounter[Silence] -= 1;
						if(statusTurnCounter[Silence] == 0) {
							TryRemoveStatus (Silence);
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
