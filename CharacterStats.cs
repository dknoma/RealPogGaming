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

	void Awake() {
		this.expUntilLevelUp = calcNextLevel();
		this.currentHP = this.maxHP;
		this.currentAtk = this.baseAtk;
		this.currentDef = this.baseDef;
		this.currentSpd = this.baseSpd;
		this.statusTurnCounter = new int[getStatusAfflictions().Length];
		this.statChangeTurnCounter = new int[getStatChangeAfflictions().Length];
		this.canAct = true;
		this.canCastSpells = true;
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
	public int GetMaxHP() { return 0 + this.maxHP; }

	// Return a copy of this characters current attack
	public int GetCurrentHP() { return 0 + this.currentHP; }

	// Modify current HP w/ new value
	public void ModifyHP(int hp) { this.currentHP += hp; }

	// A % modifier for HP
	public void ModifyHP(float hpPercentage) { this.currentHP += (int) Mathf.Round(this.maxHP * hpPercentage); }

	public int GetRuneHP() { return 0 + this.totalRuneHP; }

	/*******
	 * ATK *
	 *******/
	// Return a copy of this characters base attack
	public int GetBaseAtk() { return 0 + this.baseAtk; }

	// Return a copy of this characters current attack
	public int GetCurrentAtk() { return 0 + this.currentAtk; }

	// Modify current attack w/ new value
	public void ModifyAtk(bool up) { this.currentAtk += atkMod(this.baseAtk, up); }

	public int GetRuneAtk() { return 0 + this.totalRuneAtk; }

	/*******
	 * DEF *
	 *******/
	// Return a copy of this characters base defense
	public int GetBaseDef() { return 0 + this.baseDef; }

	// Return a copy of this characters current defense
	public int GetCurrentDef() { return 0 + this.currentDef; }

	// Modify current defense w/ new value
	public void ModifyDef(bool up) { this.currentDef += defMod(this.baseDef, up); }

	public int GetRuneDef() { return 0 + this.totalRuneDef; }

	/*******
	 * SPD *
	 *******/
	// Return a copy of this characters base defense
	public int GetBaseSpd() { return 0 + this.baseSpd; }

	// Return a copy of this characters current defense
	public int GetCurrentSpd() { return 0 + this.currentSpd; }

	// Modify current defense w/ new value
	public void ModifySpd(bool up) { this.currentSpd += defMod(this.baseSpd, up); }

	public int GetRuneSpd() { return 0 + this.totalRuneSpd; }

	/* 
	 * Status effects
	 */
	public bool tryStatusAffliction(int status, int turnsToAfflict) {
		if(!doesResistStatus(status) && !hasStatusAffliction && !afflictedByStatus(status)) {
			// TODO: afflict the state
			afflictStatus(status);
			this.statusTurnCounter[status] = turnsToAfflict;
			this.hasStatusAffliction = true;
			return true;
		}
		return false;
	}

	public bool tryStatChange(int statChange, int turnsToAfflict) {
		if(!doesResistStatChange(statChange) && !afflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			afflictStatChange(statChange);
			this.statChangeTurnCounter [statChange] = turnsToAfflict;
			return true;
		}
		return false;
	}

	public bool tryRemoveStatus(int status) {
		if(!doesResistStatusRemoval(status) && !afflictedByStatus(status)) {
			// TODO: afflict the stat change
			removeStatus(status);
			this.statusTurnCounter [status] = 0;
			return true;
		}
		return false;
	}

	public bool tryRemoveStatChange(int statChange) {
		if(!doesResistStatChangeRemoval(statChange) && !afflictedByStatChange(statChange)) {
			// TODO: afflict the stat change
			removeStatChange(statChange);
			this.statChangeTurnCounter [statChange] = 0;
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
				this.disableRunes = true;
				break;
			case STUN:
				this.canAct = false;
				break;
			case SILENCE:
				this.canCastSpells = false;
				break;
			}
			this.hasStatusAffliction = true;
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
				this.hasStatusAffliction = false;
			}
			if (tryRemoveStatChange (SPDDOWN)) {
				ModifySpd (true);// Only need to happen once, not every turn
			}
			break;
		case BURN:
			ModifyAtk (true); // Only need to happen once, not every turn
			if(tryRemoveStatus(BURN)){
				this.hasStatusAffliction = false;
			}
			// Tries to remove atk down if not afflicted with a stat change removal resist
			if (tryRemoveStatChange (ATKDOWN)) {
				ModifyAtk (true);// Only need to happen once, not every turn
			}
			break;
		case POISON:
			if(tryRemoveStatus (POISON)){
				this.hasStatusAffliction = false;
			}
			if (tryRemoveStatChange (ATKDOWN)) {
				ModifyAtk (true);
			}
			break;
		case RUNE_LOCK:
			if(tryRemoveStatus(STUN)) {
				this.hasStatusAffliction = false;
				this.disableRunes = false;
			}
			break;
		case STUN:
			if(tryRemoveStatus(STUN)) {
				this.hasStatusAffliction = false;
				this.canAct = true;
			}
			break;
		case SILENCE:
			if(tryRemoveStatus(SILENCE)) {
				this.hasStatusAffliction = false;
				this.canCastSpells = true;
			}
			break;
		}
	}

	public void removeAllAfflictions() {
		for (int status = 0; status < this.afflictedStatuses.Length; status++) {
			switch (status) {
			case BOG:
				if (tryRemoveStatChange (SPDDOWN)) {
					ModifySpd (true);// Only need to happen once, not every turn
				}
				if(tryRemoveStatus(BOG)) {
					this.hasStatusAffliction = false;
				}
				break;
			case BURN:
				ModifyAtk (true); // Only need to happen once, not every turn
				if(tryRemoveStatus(BURN)) {
					this.hasStatusAffliction = false;
				}
			// Tries to remove atk down if not afflicted with a stat change removal resist
				if (tryRemoveStatChange (ATKDOWN)) {
					ModifyAtk (true);// Only need to happen once, not every turn
				}
				break;
			case POISON:
				if(tryRemoveStatus(POISON)) {
					this.hasStatusAffliction = false;
				}
				if (tryRemoveStatChange (ATKDOWN)) {
					ModifyAtk (true);
				}
				break;
			case RUNE_LOCK:
				if(tryRemoveStatus(STUN)) {
					this.hasStatusAffliction = false;
					this.disableRunes = false;
				}
				break;
			case STUN:
				if(tryRemoveStatus(STUN)) {
					this.hasStatusAffliction = false;
				}
				this.canAct = true;
				break;
			case SILENCE:
				if(tryRemoveStatus(SILENCE)) {
					this.hasStatusAffliction = false;
				}
				this.canCastSpells = true;
				break;
			}
		}
	}

	/**
	 * Check this characters status afflictions. This method is meant to be executed at the beginning of each of
	 * this character's turns.
	 */ 
	public void checkStatusAfflictions() {
		for(int i = 0; i < this.afflictedStatuses.Length; i++) {
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
				if(this.afflictedStatuses[RUNE_LOCK]) {
					this.disableRunes = true;
				}
				break;
			case STUN:
				// Can't act for x turns
				if(this.afflictedStatuses[STUN]) {
					this.canAct = false;
				}
				break;
			case SILENCE:
				// Can't cast spells for x turns. Can still use skills
				if(this.afflictedStatuses[SILENCE]) {
					this.canCastSpells = false;
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
	public void resolveStatusAfflictions() {
		for(int i = 0; i < this.afflictedStatuses.Length; i++) {
			// Check status afflictions and do action depending on the affliction
			switch (i) {
				case BOG:
					//Nothing planned atm for this status effect. Only SPDDown atm.
					break;
				case BURN:
					// 8% DoT & p atk down
					if (this.statusTurnCounter [BURN] > 0) {
						ModifyHP (-0.08f);
						this.statusTurnCounter[BURN] -= 1;
						if(this.statusTurnCounter[BURN] == 0) {
							tryRemoveStatus (BURN);
						}
					}
					break;
				case POISON:
					// 10% DoT & m atk down
					if (this.statusTurnCounter [POISON] > 0) {
						ModifyHP (-0.1f);
						this.statusTurnCounter[POISON] -= 1;
						if(this.statusTurnCounter[POISON] == 0) {
							tryRemoveStatus (POISON);
						}
					}
					break;
				case RUNE_LOCK:
					this.statusTurnCounter[RUNE_LOCK] -= 1;
					if (this.statusTurnCounter[RUNE_LOCK] == 0) {
						tryRemoveStatus(RUNE_LOCK);
					}
					break;
				case STUN:
					// Can't act for x turns
					if(this.statusTurnCounter[STUN] > 0) {
						this.statusTurnCounter[STUN] -= 1;
						if(this.statusTurnCounter[STUN] == 0) {
							tryRemoveStatus (STUN);
						}
					}
					break;
				case SILENCE:
					// Can't cast spells for x turns. Can still use skills
					if(this.statusTurnCounter[SILENCE] > 0) {
						this.statusTurnCounter[SILENCE] -= 1;
						if(this.statusTurnCounter[SILENCE] == 0) {
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
		//int remainingExp = exp - this.expUntilLevelUp;
		int totalExp = this.currentExp + exp;
		int remainingExp = this.expUntilLevelUp - totalExp;
		if(remainingExp > 0) {
			this.currentExp = totalExp;
			this.expUntilLevelUp = remainingExp;
			Debug.Log(string.Format("{0} has gained {1} exp! exp until next level: {2}",
				this.name, exp, this.expUntilLevelUp));
		} else {
			this.currentLevel += 1;
			this.currentExp = Mathf.Abs(remainingExp);
			this.expUntilLevelUp = calcNextLevel() - this.currentExp;
			Debug.Log(string.Format("{0} has leveled up: {1}, exp until next level: {2}", 
				this.name, this.currentLevel, this.expUntilLevelUp));
		}
	}

	// Increase/decrease atk by 25% of the base value
	private int atkMod(int baseAtk, bool up) {
		return (int) (up ? baseAtk * 0.25f : baseAtk * (-0.25f));
	}

	// Increase/decrease defense by 25% of the base value
	private int defMod(int baseDef, bool up) {
		return (int) (up ? baseDef * 0.25f : baseDef * (-0.25f));
	}

	// Increase/decrease speed by 20% of the base value
	private int spdMod(int baseSpd, bool up) {
		return (int) (up ? baseSpd * 0.2f : baseSpd * (-0.2f));
	}

	// Calculate the amount of exp required to get to the next level
	// 1->2: 1, 2->3: 5, 3->4: 33, 4->5: 73, ...
	private int calcNextLevel() {
		return (int) Mathf.Round(4+(15*(Mathf.Pow(this.currentLevel, 3)))/14);
	}
}
