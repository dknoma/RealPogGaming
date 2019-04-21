using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Characters {
//[RequireComponent(typeof(BattleActions))]
	public class Character : CharacterStats, IComparable {

		public CharacterSlot slot;
		public GameObject icon; // Icon to display inside of the turn queue
		
		private Element attackElement;

//		private readonly UnityEvent hpValueChangeEvent = new UnityEvent(); // Event when player gains/loses current HP
//		private readonly UnityEvent hpModEvent = new UnityEvent();         // Event when player max HP changes
//		private readonly UnityEvent mpValueChangeEvent = new UnityEvent(); // Event when player gains/loses current HP
//		private readonly UnityEvent mpModEvent = new UnityEvent();         // Event when player max HP changes
//		private readonly UnityEvent atkModEvent = new UnityEvent();        // Event when player base atk changes
//		private readonly UnityEvent defModEvent = new UnityEvent();        // Event when player base def changes
//		private readonly UnityEvent spdModEvent = new UnityEvent();        // Event when player base spd changes

		private void Awake() {
			expUntilLevelUp = CalcNextLevel();
			maxHp = baseHp;
			maxMp = baseMp;
			currentHp = maxHp;
			currentMp = maxMp;
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

		private void Start () {
		BattleManager.bm.AddEndBattleListener(ClearReadiness);   // Listen in on Battle Manager end battle event and clear readiness
		BattleManager.bm.AddEndBattleListener(ClearStatChanges); // Listen in on Battle Manager end battle event and clear stat changes
	}

		/**
	 * Compare to method that helps sort units by speed.
	 */ 
		int IComparable.CompareTo(object obj) {
			if (obj == null) {
				return 1;
			} 
			Character otherCharacter = obj as Character;
			if(otherCharacter != null) {
				return currentSpd - otherCharacter.currentSpd;
			}
			throw new ArgumentException("Object is not a Character...");
		}
	
		public bool CanCharacterAct() {
			return canAct;
		}

		public Element GetAttackElement() {
			return attackElement;
		}
	
		public void SetAttackElement(Element ele) {
			attackElement = ele;
		}

		public bool TryIncapacitate() {
			if(!AfflictedByStatus(Status.Incapacitated)) {
				// TODO: afflict the state
				AfflictStatus(Status.Incapacitated);
				hasStatusAffliction = true;
				BattleEventManager.bem.InvokeIncapacitatedEvent(GetComponent<Character>());
				return true;
			}
			return false;
		}
		
		///
		/// Stat Methods
		///
	
		///
		/// HP methods
		///
		/// Return a copy of this characters max hp
		/// 
		public int GetMaxHp() { return 0 + maxHp; }

		public void ModifyHp(bool up) {
			maxHp += HpMod(baseHp, up);
			BattleEventManager.bem.InvokeHpModEvent(affiliation, slot);
			//hpModEvent.Invoke();
		}
	
		///
		/// Return a copy of this characters current attack
		/// 
		public int GetCurrentHp() { return 0 + currentHp; }

		///
		/// Modify current HP w/ new value
		/// 
		public void ModifyCurrentHp(int hp) {
			currentHp = Mathf.Clamp(currentHp + hp, 0,maxHp);
			if (currentHp <= 0) {
				TryIncapacitate();
			}
			Debug.LogFormat("{0} hp %: {1}", name, currentHp / (float) maxHp);
			BattleEventManager.bem.InvokeHpValueChangeEvent(affiliation, slot);
//			hpValueChangeEvent.Invoke();
		}

		// A % modifier for HP
		public void ModifyCurrentHpByPercentageOfMax(float hpPercentage) { 
			currentHp = (int) Mathf.Clamp(currentHp + Mathf.Round(maxHp * hpPercentage), 0, maxHp); 
			if (currentHp <= 0) {
				TryIncapacitate();
			}
			BattleEventManager.bem.InvokeHpValueChangeEvent(affiliation, slot);
//			hpValueChangeEvent.Invoke();
		}
		
		public void ModifyCurrentHpByPercentageOfCurrent(float hpPercentage) { 
			currentHp = (int) Mathf.Clamp(currentHp + Mathf.Round(currentHp * hpPercentage), 1, maxHp); 
			if (currentHp <= 0) {
				TryIncapacitate();
			}
			BattleEventManager.bem.InvokeHpValueChangeEvent(affiliation, slot);
//			hpValueChangeEvent.Invoke();
		}
	
		public int GetRuneHp() { return 0 + totalRuneHp; }
	
		/// MP Methods
	
		/// <summary>
		/// Get current MP
		/// </summary>
		/// <returns>Character's max MP.</returns>
		public int GetMaxMp() { return 0 + maxMp; }

		public void ModifyMp(bool up) {
			maxMp += HpMod(baseMp, up);
			BattleEventManager.bem.InvokeMpModEvent(affiliation, slot);
//			mpModEvent.Invoke();
		}
	
		///
		/// Return a copy of this characters current mp
		/// 
		public int GetCurrentMp() { return 0 + currentMp; }

		///
		/// Modify current MP w/ new value
		/// 
		public void ModifyCurrentMp(int mp) {
			currentMp += mp;
			if (currentMp <= 0) {
				TryIncapacitate();
			} else if (currentMp > maxMp) {
				currentMp = maxMp;
			}
			Debug.LogFormat("{0} hp %: {1}", name, currentMp / (float) maxMp);
			BattleEventManager.bem.InvokeMpValueChangeEvent(affiliation, slot);
//			mpValueChangeEvent.Invoke();
		}
	 
		/// <summary>
		/// A % modifier for MP
		/// </summary>
		/// <param name="mpPercentage"></param>
		public void ModifyCurrentMp(float mpPercentage) { 
			currentMp += (int) Mathf.Round(maxMp * mpPercentage); 
			if (currentMp <= 0) {
				TryIncapacitate();
			} else if (currentMp > maxMp) {
				currentMp = maxMp;
			}
			BattleEventManager.bem.InvokeMpValueChangeEvent(affiliation, slot);
//			mpValueChangeEvent.Invoke();
		}

		///
		/// Atk
		///
		/// Return a copy of this characters base attack
		/// 
		public int GetBaseAtk() { return 0 + baseAtk; }

		// Return a copy of this characters current attack
		public int GetCurrentAtk() { return 0 + currentAtk; }

		// Modify current attack w/ new value
		public void ModifyAtk(bool up) {
			currentAtk += AtkMod(baseAtk, up);
			BattleEventManager.bem.InvokeAtkModEvent(affiliation, slot);
//			atkModEvent.Invoke();
		}

		public int GetRuneAtk() { return 0 + totalRuneAtk; }

		/*******
	 * DEF *
	 *******/
		// Return a copy of this characters base defense
		public int GetBaseDef() { return 0 + baseDef; }

		// Return a copy of this characters current defense
		public int GetCurrentDef() { return 0 + currentDef; }

		// Modify current defense w/ new value
		public void ModifyDef(bool up) {
			currentDef += DefMod(baseDef, up);
			BattleEventManager.bem.InvokeDefModEvent(affiliation, slot);
//			defModEvent.Invoke();
		}

		public int GetRuneDef() { return 0 + totalRuneDef; }

		/*******
	 * SPD *
	 *******/
		// Return a copy of this characters base defense
		public int GetBaseSpd() { return 0 + baseSpd; }

		// Return a copy of this characters current defense
		public int GetCurrentSpd() { return 0 + currentSpd; }

		// Modify current defense w/ new value
		public void ModifySpd(bool up) {
			currentSpd += DefMod(baseSpd, up);
			BattleEventManager.bem.InvokeSpdModEvent(affiliation, slot);
//			spdModEvent.Invoke();
		}

		public int GetRuneSpd() { return 0 + totalRuneSpd; }

		/* 
	 * Status effects
	 */

		public bool IsAfflictedBy(Status status) {
			return afflictedStatuses[status];
		}
	
		public bool TryStatusAffliction(Status status, int turnsToAfflict) {
			if(!DoesResistStatus(status) && !hasStatusAffliction && !AfflictedByStatus(status)) {
				// afflict the state
				AfflictStatus(status);
				statusTurnCounter[status] = turnsToAfflict;
				hasStatusAffliction = true;
				return true;
			}
			return false;
		}

		public bool TryStatChange(StatChange statChange, int turnsToAfflict) {
			if (DoesResistStatChange(statChange) || AfflictedByStatChange(statChange)) return false;
			// afflict the stat change
			AfflictStatChange(statChange);
			statChangeTurnCounter [statChange] = turnsToAfflict;
			switch (statChange) {
				case StatChange.HpUp:
					ModifyHp(true);
					break;
				case StatChange.MpUp:
					ModifyMp(true);
					break;
				case StatChange.AtkUp:
					ModifyAtk(true);
					break;
				case StatChange.DefUp:
					ModifyDef(true);
					break;
				case StatChange.SpdUp:
					ModifySpd(true);
					break;
				case StatChange.HpDestruct:
					ModifyHp(false);
					break;
				case StatChange.MpDown:
					ModifyMp(false);
					break;
				case StatChange.AtkDown:
					ModifyAtk(false);
					break;
				case StatChange.DefDown:
					ModifyDef(false);
					break;
				case StatChange.SpdDown:
					ModifySpd(false);
					break;
				default:
					throw new ArgumentOutOfRangeException("statChange", statChange, null);
			}
			return true;
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
	
		/// <summary>
		/// Reset stat changes. Invokes stat mod events to notify listeners of stat changes.
		/// </summary>
		/// <param name="statChange">Desired stat change</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void ResetStatChange(StatChange statChange) {
			if (afflictedStatChanges.ContainsKey(statChange)) {
				afflictedStatChanges[statChange] = false;
			} else {
				afflictedStatChanges.Add(statChange, false);
			}
			switch (statChange) {
				case StatChange.AtkUp:
				case StatChange.AtkDown:
					currentAtk = baseAtk;
					BattleEventManager.bem.InvokeAtkModEvent(affiliation, slot);
//					atkModEvent.Invoke();
					break;
				case StatChange.DefUp:
				case StatChange.DefDown:
					currentDef = baseDef;
					BattleEventManager.bem.InvokeDefModEvent(affiliation, slot);
//					defModEvent.Invoke();
					break;
				case StatChange.SpdUp:
				case StatChange.SpdDown:
					currentSpd = baseSpd;
					BattleEventManager.bem.InvokeSpdModEvent(affiliation, slot);
//					spdModEvent.Invoke();
					break;
				case StatChange.HpUp:
				case StatChange.HpDestruct:
					maxHp = baseHp;
					BattleEventManager.bem.InvokeHpModEvent(affiliation, slot);
//					hpModEvent.Invoke();
					break;
				case StatChange.MpUp:
				case StatChange.MpDown:
					maxMp = baseMp;
					BattleEventManager.bem.InvokeMpModEvent(affiliation, slot);
//					mpModEvent.Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException("statChange", statChange, null);
			}
		}
	
		/// <summary>
		/// Remove all stat changes. 
		/// </summary>
		public void ClearStatChanges() {
			var statChanges = afflictedStatChanges.Keys.ToList(); // Gets the list of keys in order to reset each stat change
			foreach (StatChange statChange in statChanges) {
				ResetStatChange(statChange);
			}
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
						if(TryStatChange(StatChange.SpdDown, turns)) {
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
					if(TryRemoveStatChange (StatChange.SpdDown)) {
						ModifySpd (true); // Only need to happen once, not every turn
					}
					break;
				case Status.Burn:
					ModifyAtk (true); // Only need to happen once, not every turn
					if(TryRemoveStatus(Status.Burn)){
//					hasStatusAffliction = false;
					}
					// Tries to remove atk down ifnot afflicted with a stat change removal resist
					if(TryRemoveStatChange (StatChange.AtkDown)) {
						ModifyAtk (true); // Only need to happen once, not every turn
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
						if(TryRemoveStatChange (StatChange.SpdDown)) {
							ModifySpd (true); // Only need to happen once, not every turn
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
							ModifyAtk (true); // Only need to happen once, not every turn
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
							ModifyCurrentHpByPercentageOfMax (-0.08f);
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
							ModifyCurrentHpByPercentageOfMax (-0.1f);
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
					case Status.Incapacitated:
						break;
					default:
						Debug.Log ("Affliction not valid");
						break;
				}
			}
		}

		public void ResolveStatChanges() {
			foreach(KeyValuePair<StatChange, bool> statChanges in afflictedStatChanges) {
				// Check status afflictions and do action depending on the affliction
				StatChange sc = statChanges.Key;
				switch (sc) {
					case StatChange.HpUp:
					case StatChange.MpUp:
					case StatChange.AtkUp:
					case StatChange.DefUp:
					case StatChange.SpdUp:
					case StatChange.HpDestruct:
					case StatChange.MpDown:
					case StatChange.AtkDown:
					case StatChange.DefDown:
					case StatChange.SpdDown:
						if (statChangeTurnCounter[sc] > 0) {
							statChangeTurnCounter[sc] -= 1;
							if (statChangeTurnCounter[sc] == 0) {
								TryRemoveStatChange(sc);
							}
						}
						break;
					default:
						Debug.Log ("Affliction not valid");
						break;
				}
			}
		}

		public bool IsIncapacitated() {
			return afflictedStatuses.ContainsKey(Status.Incapacitated) && afflictedStatuses[Status.Incapacitated];
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
		public Affiliation GetAffiliation() {
			return affiliation;
		}
		
		// Calculate the amount of exp required to get to the next level
		// 1->2: 1, 2->3: 5, 3->4: 33, 4->5: 73, ...
		protected int CalcNextLevel() {
			return (int) Mathf.Round(4+15*Mathf.Pow(currentLevel, 3)/14);
		}
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
				expUntilLevelUp = CalcNextLevel() - currentExp; // Recalculate for next level
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

		private int ModStat(StatChange statChange) {
			switch (statChange) {
				case StatChange.AtkUp:
					return Mathf.RoundToInt(baseAtk * 0.25f);
				case StatChange.DefUp:
					return Mathf.RoundToInt(baseDef * 0.25f);
				case StatChange.SpdUp:
					return Mathf.RoundToInt(baseSpd * 0.2f);
				case StatChange.HpUp:
					return Mathf.RoundToInt(baseHp * 0.15f);
				case StatChange.AtkDown:
					return Mathf.RoundToInt(baseAtk * -0.25f);
				case StatChange.DefDown:
					return Mathf.RoundToInt(baseDef * -0.25f);
				case StatChange.SpdDown:
					return Mathf.RoundToInt(baseSpd * -0.2f);
				case StatChange.HpDestruct:
					return Mathf.RoundToInt(baseHp * -0.15f);
				default:
					throw new ArgumentOutOfRangeException("statChange", statChange, null);
			}
		}

		private int HpMod(int baseHp, bool up) {
			return (int) (up ? baseHp * 0.15f : baseHp * -0.15f);
		}
	
		private int MpMod(int basemp, bool up) {
			return (int) (up ? baseMp * 0.2f : baseMp * -0.2f);
		}

		// Increase/decrease atk by 25% of the base value
		private int AtkMod(int baseAtk, bool up) {
			return (int) (up ? baseAtk * 0.25f : baseAtk * -0.25f);
		}

		// Increase/decrease defense by 25% of the base value
		private int DefMod(int baseDef, bool up) {
			return (int) (up ? baseDef * 0.25f : baseDef * -0.25f);
		}

		// Increase/decrease speed by 20% of the base value
		private int SpdMod(int baseSpd, bool up) {
			return (int) (up ? baseSpd * 0.2f : baseSpd * -0.2f);
		}

		public void IncrementReadiness(int speed, bool fastestFirst) {
			readiness = fastestFirst ? readiness + speed : readiness + (READINESS_THRESHOLD - speed);
			if (readiness >= READINESS_THRESHOLD) {
				ready = true;
			}
		}
	
		public void ResetReadiness() {
			readiness = readiness <= READINESS_THRESHOLD ? 0 : readiness % READINESS_THRESHOLD;
			ready = false;
		}

		public void ClearReadiness() {
			readiness = 0;
			ready = false;
		}

	
		protected bool TryDamage(int dmg, GameObject target) {
			// TODO: Check if target can be damaged.
			Debug.Log(string.Format("\t{0} dealt {1} damage to {2}", name, dmg, target.name));
			Character unit = target.GetComponent<Character>();
			unit.ModifyCurrentHp(-dmg);
			Debug.Log(string.Format("{0}'s HP: {1}", target.name, unit.GetCurrentHp()));
			// If HP is still positive, return
//		if (unit.GetCurrentHp() > 0) return true;
////		unit.TryIncapacitate();	// Incapacitate the unit. Cannot act until revived
////		units.Remove(target); // Remove unit from the list if no more HP
////		RemoveTargetFromQueue(unit);
//		Debug.LogFormat("contains {0}: {1}", target, units.Contains(target));
//		// TODO: if party member, make incapacitated: can be revived
//		if(target.CompareTag("Enemy")) {
//			RemoveTargetFromQueue(unit); // Remove enemy from queue, does not perform any more actions
//			enemies.Remove(target);
//			target.SetActive(false);
//			expToGive += unit.expToGrant;
//			if(enemies.Count == 0) {
//				EndBattle(WinStatus.Win);
//			}
//		}
//		if(target.CompareTag("Ally")) {
////			Player player = target.GetComponent<Player>();
////			PlayerStatBar statBar = UIManager.um.GetPlayerStatBar(player.slot);
////			statBar.UpdateHpBar(0);
////			PlayerManager.pm.IncapacitateUnit(unit.GetComponent<Player>());
//
//			// Allies do not get removed from queue in case their turn comes around when revived
//			if(PlayerManager.pm.AllAlliesIncapacitated()) {
//				EndBattle(WinStatus.Lose);
//			}
//		}
			return true;
		}
	
		protected int CalculateDamage(GameObject unit) {
			Character target = unit.GetComponent<Character>();
			// Calculate the current units rune bonuses
			CalculateRuneStats();
			int damage = Mathf.RoundToInt(
				// Standard atk-def calc
				(Mathf.Pow(currentAtk, 2) + totalRuneAtk) /(target.GetCurrentDef() + target.GetRuneDef()) 								
				// Level compensation
				*(1 +(currentLevel*2 - target.currentLevel) / 50)			
				// Elemental multiplier
				* ElementalAffinity.CalcElementalDamage(attackElement, target.element));  														
			return damage;
		}
	
		protected int CalculateSingleHitDamage(GameObject unit) {
			Character target = unit.GetComponent<Character>();
//		Debug.Log(string.Format("{0}'s atk: {1}, {2}'s def: {3}", 
//			source.name, source.GetCurrentAtk(), target.name, target.GetCurrentDef()));
//		Debug.Log(string.Format("{0}'s attack element: {1}, {2} element: {3}", 
//			source.name, source.GetAttackElement(), target.name, target.element));
			// Calculate the current units rune bonuses
			CalculateRuneStats();
			int damage = Mathf.RoundToInt(
				// Standard atk-def calc
				(Mathf.Pow(currentAtk, 2) + totalRuneAtk) /(target.GetCurrentDef() + target.GetRuneDef()) 								
				// Level compensation
				*(1 +(currentLevel*2 - target.currentLevel) / 50)			
				// Elemental multiplier
				* ElementalAffinity.CalcElementalDamage(attackElement, target.element));  														
			return damage;
		}		
	
		protected int CalculateMultiHitDamage(int attackValue, GameObject unit) {
			Character target = unit.GetComponent<Character>();
//		Debug.Log(string.Format("{0}'s atk: {1}, {2}'s def: {3}", 
//			source.name, source.GetCurrentAtk(), target.name, target.GetCurrentDef()));
//		Debug.Log(string.Format("{0}'s attack element: {1}, {2} element: {3}", 
//			source.name, source.GetAttackElement(), target.name, target.element));
			// Calculate the current units rune bonuses
			CalculateRuneStats();
			int damage = Mathf.RoundToInt(
				// Standard atk-def calc
				Mathf.Pow(attackValue, 2) / (target.GetCurrentDef() + target.GetRuneDef())					
				// Level compensation
				*(1 +(currentLevel*2 - target.currentLevel) / 50)			
				// Elemental multiplier
				* ElementalAffinity.CalcElementalDamage(attackElement, target.element));  														
			return damage;
		}		
//	
//		/// <summary>
//		/// Add listeners on when this character's health is changed
//		/// </summary>
//		/// <param name="call">Takes a float as a parameter for the listener.</param>
//		public void AddHpValueChangeListener(UnityAction<Character, float> call) {
//			hpValueChangeEvent.AddListener(() => call(this, currentHp / (float) maxHp));
//		}
//	
//		public void AddMpValueChangeListener(UnityAction<Character, float> call) {
//			mpValueChangeEvent.AddListener(() => call(this, currentMp / (float) maxMp));
//		}
//	
//		/// <summary>
//		/// Add listeners on when this character's base attack is changed
//		/// </summary>
//		/// <param name="call"></param>
//		public void AddHpModListener(UnityAction<bool, bool> call) {
//			hpModEvent.AddListener(() => call(AfflictedByStatChange(StatChange.HpUp), AfflictedByStatChange(StatChange.HpDestruct)));
//		}
//	
//		public void AddMpModListener(UnityAction<bool, bool> call) {
//			mpModEvent.AddListener(() => call(AfflictedByStatChange(StatChange.HpUp), AfflictedByStatChange(StatChange.HpDestruct)));
//		}
//	
//		/// <summary>
//		/// Add listeners on when this character's base attack is changed
//		/// </summary>
//		/// <param name="call"></param>
//		public void AddAtkModListener(UnityAction<bool, bool> call) {
//			atkModEvent.AddListener(() => call(AfflictedByStatChange(StatChange.AtkUp), AfflictedByStatChange(StatChange.AtkDown)));
//		}
//	
//		/// <summary>
//		/// Add listeners on when this character's base defense is changed
//		/// </summary>
//		/// <param name="call"></param>
//		public void AddDefModListener(UnityAction<bool, bool> call) {
//			defModEvent.AddListener(() => call(AfflictedByStatChange(StatChange.DefUp), AfflictedByStatChange(StatChange.DefDown)));
//		}
//	
//		/// <summary>
//		/// Add listeners on when this character's base speed is changed
//		/// </summary>
//		/// <param name="call"></param>
//		public void AddSpdModListener(UnityAction<bool, bool> call) {
//			spdModEvent.AddListener(() => call(AfflictedByStatChange(StatChange.SpdUp), AfflictedByStatChange(StatChange.SpdDown)));
//		}
		
		protected void CalculateRuneStats() {
//		RuneSlots slots = currentUnit.GetComponent<RuneSlots>();
		}

		public void AddActionListener(UnityAction call) {
			BattleEventManager.bem.ActionEvent.AddListener(call);
		}
	
		public void RemoveActionListener(UnityAction call) {
			BattleEventManager.bem.ActionEvent.RemoveListener(call);
		}
	
		// These events are called either directly by a manager or by OnClick button events (UIManager buttons)
	
		/// <summary>
		/// Attack A event
		/// </summary>
		public virtual void DoAttackA(GameObject target) {
			// TODO: Create sub classes for each unit. maybe even make one for enemies (obviously would do attacks automatically)
			//		 	Maybe move attack names and stuff to this class as well.
			//			Attacks need target(s)
			//			Support skills need target(s) as well
			//				targets probably chosen from Buttons during battle
			//				Perform action on target, invoke action event to listeners (battle manager in this case)
//		attackAEvent.Invoke();
			BattleEventManager.bem.ActionEvent.Invoke();
		}
	
		public virtual void DoAttackB(GameObject target) {
//		attackBEvent.Invoke();
			BattleEventManager.bem.ActionEvent.Invoke();
		}

		public virtual void DoSupport(GameObject target) {
//		supportEvent.Invoke();
			BattleEventManager.bem.ActionEvent.Invoke();
		}

		protected virtual void SwordA() {
		}
	
		protected virtual void SwordB() {
		}

		protected virtual void Support() {
		}
	}
	
	
}