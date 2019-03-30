using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : StatusEffects {

	public enum Affiliation { Ally, Enemy };

	/* Public/inspector elements */
	public Affiliation affiliation;
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
	protected int maxHP = 20;
	protected int currentHP;
	protected int totalRuneHP;
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
	protected const int BOG = (int) Status.Bog;
	protected const int BURN = (int) Status.Burn;
	protected const int POISON = (int) Status.Poison;
	protected const int RUNE_LOCK = (int) Status.RuneLock;
	protected const int STUN = (int) Status.Stun;
	protected const int SILENCE = (int) Status.Silence;

	protected const int ATKUP = (int) StatChange.ATKUp;
	protected const int DEFUP = (int) StatChange.DEFUp;
	protected const int SPDUP = (int) StatChange.SpeedUp;
	protected const int HPUP = (int) StatChange.HPUp;
	protected const int ATKDOWN = (int) StatChange.ATKDown;
	protected const int DEFDOWN = (int) StatChange.DEFDown;
	protected const int SPDDOWN = (int) StatChange.SpeedDown;
	protected const int HPDESTRUCTION = (int) StatChange.HPDestruct;

	private void Awake() {
		expUntilLevelUp = CalcNextLevel();
		currentHP = maxHP;
		currentAtk = baseAtk;
		currentDef = baseDef;
		currentSpd = baseSpd;
		statusTurnCounter = new int[getStatusAfflictions().Length];
		statChangeTurnCounter = new int[getStatChangeAfflictions().Length];
		canAct = true;
		canCastSpells = true;
		if (bogResist) { addStatusResist (BOG); } 
		if (burnResist) { addStatusResist (BURN); } 
		if (poisonResist) { addStatusResist (POISON); } 
		if (runeLockResist) { addStatusResist (RUNE_LOCK); } 
		if (stunResist) { addStatusResist (STUN); } 
		if (silenceResist) { addStatusResist (SILENCE); } 
		if(transform.GetComponentInChildren<RuneSlots>() != null) {
			transform.GetComponentInChildren<RuneSlots>().GetStatCount();
		}
	}

	/******
	 * HP *
	 ******/
	// Return a copy of this characters max hp
	public int GetMaxHP() { return 0 + maxHP; }

	// Return a copy of this characters current attack
	public int GetCurrentHP() { return 0 + currentHP; }

	// Modify current HP w/ new value
	public void ModifyHP(int hp) { currentHP += hp; }

	// A % modifier for HP
	public void ModifyHP(float hpPercentage) { currentHP += (int) Mathf.Round(maxHP * hpPercentage); }

	public int GetRuneHP() { return 0 + totalRuneHP; }

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
	public bool tryStatusAffliction(int status, int turnsToAfflict) {
		if(!doesResistStatus(status) && !hasStatusAffliction && !afflictedByStatus(status)) {
			// TODO: afflict the state
			afflictStatus(status);
			statusTurnCounter[status] = turnsToAfflict;
			hasStatusAffliction = true;
			return true;
		}
		return false;
	}

	public bool tryStatChange(int statChange, int turnsToAfflict) {
		if(!doesResistStatChange(statChange) && !afflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			afflictStatChange(statChange);
			statChangeTurnCounter [statChange] = turnsToAfflict;
			return true;
		}
		return false;
	}

	public bool tryRemoveStatus(int status) {
		if(!doesResistStatusRemoval(status) && !afflictedByStatus(status)) {
			// TODO: afflict the stat change
			removeStatus(status);
			statusTurnCounter [status] = 0;
			return true;
		}
		return false;
	}

	public bool tryRemoveStatChange(int statChange) {
		if(!doesResistStatChangeRemoval(statChange) && !afflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			removeStatChange(statChange);
			statChangeTurnCounter [statChange] = 0;
			return true;
		}
		return false;
	}

	public bool doesResistStatus(int status) {
		return resistsStatusEffect(status);
	}

	public bool doesResistStatChange(int statChange) {
		return resistsStatChange(statChange);
	}

	public bool doesResistStatusRemoval(int status) {
		return resistsStatusEffectRemoval(status);
	}

	public bool doesResistStatChangeRemoval(int statChange) {
		return resistsStatChangeRemoval(statChange);
	}

	/**
	 * Initial state of the affliction. This method is used to initialize effects that only happen once
	 * to the character. checkStatusAfflictions() is used for effects that happen each turn.
	 */ 
	public void initAffliction(int status, int turns) {
		// Check status afflictions and do action depending on the affliction
		if (tryStatusAffliction (status, turns)) {
			switch (status) {
			case BOG:
				if(tryStatChange(SPDDOWN, turns)) {
					ModifySpd (false);
				} else {
					Debug.Log("Already afflicted by speed down...");
				}
				break;
			case BURN:
				if(tryStatChange(ATKDOWN, turns)) {
					ModifyAtk (false);
				} else {
					Debug.Log("Already afflicted by physical attack down...");
				}
				break;
			case POISON:
				if(tryStatChange(ATKDOWN, turns)) {
					ModifyAtk (false);
				} else {
					Debug.Log("Already afflicted by magical attack down...");
				}
				break;
			case RUNE_LOCK:
				disableRunes = true;
				break;
			case STUN:
				canAct = false;
				break;
			case SILENCE:
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
	public void removeAffliction(int status) {
		switch (status) {
		case BOG:
			if(tryRemoveStatus(BOG)) {
				hasStatusAffliction = false;
			}
			if (tryRemoveStatChange (SPDDOWN)) {
				ModifySpd (true);// Only need to happen once, not every turn
			}
			break;
		case BURN:
			ModifyAtk (true); // Only need to happen once, not every turn
			if(tryRemoveStatus(BURN)){
				hasStatusAffliction = false;
			}
			// Tries to remove atk down if not afflicted with a stat change removal resist
			if (tryRemoveStatChange (ATKDOWN)) {
				ModifyAtk (true);// Only need to happen once, not every turn
			}
			break;
		case POISON:
			if(tryRemoveStatus (POISON)){
				hasStatusAffliction = false;
			}
			if (tryRemoveStatChange (ATKDOWN)) {
				ModifyAtk (true);
			}
			break;
		case RUNE_LOCK:
			if(tryRemoveStatus(STUN)) {
				hasStatusAffliction = false;
				disableRunes = false;
			}
			break;
		case STUN:
			if(tryRemoveStatus(STUN)) {
				hasStatusAffliction = false;
				canAct = true;
			}
			break;
		case SILENCE:
			if(tryRemoveStatus(SILENCE)) {
				hasStatusAffliction = false;
				canCastSpells = true;
			}
			break;
		}
	}

	public void removeAllAfflictions() {
		for (int status = 0; status < afflictedStatuses.Length; status++) {
			switch (status) {
			case BOG:
				if (tryRemoveStatChange (SPDDOWN)) {
					ModifySpd (true);// Only need to happen once, not every turn
				}
				if(tryRemoveStatus(BOG)) {
					hasStatusAffliction = false;
				}
				break;
			case BURN:
				ModifyAtk (true); // Only need to happen once, not every turn
				if(tryRemoveStatus(BURN)) {
					hasStatusAffliction = false;
				}
			// Tries to remove atk down if not afflicted with a stat change removal resist
				if (tryRemoveStatChange (ATKDOWN)) {
					ModifyAtk (true);// Only need to happen once, not every turn
				}
				break;
			case POISON:
				if(tryRemoveStatus(POISON)) {
					hasStatusAffliction = false;
				}
				if (tryRemoveStatChange (ATKDOWN)) {
					ModifyAtk (true);
				}
				break;
			case RUNE_LOCK:
				if(tryRemoveStatus(STUN)) {
					hasStatusAffliction = false;
					disableRunes = false;
				}
				break;
			case STUN:
				if(tryRemoveStatus(STUN)) {
					hasStatusAffliction = false;
				}
				canAct = true;
				break;
			case SILENCE:
				if(tryRemoveStatus(SILENCE)) {
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
	public void checkStatusAfflictions() {
		for(int i = 0; i < afflictedStatuses.Length; i++) {
			// Check status afflictions and do action depending on the affliction
			switch (i) {
			case BOG:
				//Nothing planned atm for this status effect. Only SPDDown atm.
				break;
			case BURN:
				// 8% DoT & p atk down
//				if (this.statusTurnCounter [BURN] > 0) {
//					modifyHP (-0.08f);
//				}
				break;
			case POISON:
				// 10% DoT & m atk down
//				if (this.statusTurnCounter [BURN] > 0) {
//					modifyHP (-0.1f);
//				}
				break;
			case RUNE_LOCK:
				// Can't use runes for x turns
				if(afflictedStatuses[RUNE_LOCK]) {
					disableRunes = true;
				}
				break;
			case STUN:
				// Can't act for x turns
				if(afflictedStatuses[STUN]) {
					canAct = false;
				}
				break;
			case SILENCE:
				// Can't cast spells for x turns. Can still use skills
				if(afflictedStatuses[SILENCE]) {
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
				case BOG:
					//Nothing planned atm for this status effect. Only SPDDown atm.
					break;
				case BURN:
					// 8% DoT & p atk down
					if (statusTurnCounter [BURN] > 0) {
						ModifyHP (-0.08f);
						statusTurnCounter[BURN] -= 1;
						if(statusTurnCounter[BURN] == 0) {
							tryRemoveStatus (BURN);
						}
					}
					break;
				case POISON:
					// 10% DoT & m atk down
					if (statusTurnCounter [POISON] > 0) {
						ModifyHP (-0.1f);
						statusTurnCounter[POISON] -= 1;
						if(statusTurnCounter[POISON] == 0) {
							tryRemoveStatus (POISON);
						}
					}
					break;
				case RUNE_LOCK:
					statusTurnCounter[RUNE_LOCK] -= 1;
					if (statusTurnCounter[RUNE_LOCK] == 0) {
						tryRemoveStatus(RUNE_LOCK);
					}
					break;
				case STUN:
					// Can't act for x turns
					if(statusTurnCounter[STUN] > 0) {
						statusTurnCounter[STUN] -= 1;
						if(statusTurnCounter[STUN] == 0) {
							tryRemoveStatus (STUN);
						}
					}
					break;
				case SILENCE:
					// Can't cast spells for x turns. Can still use skills
					if(statusTurnCounter[SILENCE] > 0) {
						statusTurnCounter[SILENCE] -= 1;
						if(statusTurnCounter[SILENCE] == 0) {
							tryRemoveStatus (SILENCE);
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
