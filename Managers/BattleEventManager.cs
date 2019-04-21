using System;
using System.Collections.Generic;
using BattleUI;
using Characters;
using Characters.Allies;
using UnityEngine;
using UnityEngine.Events;

namespace Managers {
	public class BattleEventManager : MonoBehaviour {

		public static BattleEventManager bem;
	
		// Action events
		public UnityEvent ActionEvent { get; private set; }
		
		public readonly Dictionary<CharacterSlot, UnityEvent> allyIncapacitatedEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyIncapacitatedEvents = new Dictionary<CharacterSlot, UnityEvent>();

		public readonly Dictionary<CharacterSlot, UnityEvent> hpValueChangeEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> mpValueChangeEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> hpModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> mpModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> atkModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> defModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> spdModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyHpValueChangeEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyMpValueChangeEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyHpModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyMpModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyAtkModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemyDefModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		public readonly Dictionary<CharacterSlot, UnityEvent> enemySpdModEvents = new Dictionary<CharacterSlot, UnityEvent>();
		
//		public UnityEvent HpValueChangeEvent { get; private set; }
//
//		public UnityEvent HpModEvent { get; private set; }
//
//		public UnityEvent MpValueChangeEvent { get; private set; }
//
//		public UnityEvent MpModEvent { get; private set; }
//
//		public UnityEvent AtkModEvent { get; private set; }
//
//		public UnityEvent DefModEvent { get; private set; }
//
//		public UnityEvent SpdModEvent { get; private set; }

//		public UnityEvent IncapacitatedEvent {
//			get { return incapacitatedEvent; }
//		}

		private void Awake() {
			if (bem == null) {
				bem = this;
			} else if (bem != this) {
				Destroy(gameObject);
			}
			DontDestroyOnLoad(gameObject);
			ActionEvent = new UnityEvent();
//			HpValueChangeEvent = new UnityEvent();
//			HpModEvent = new UnityEvent();                                 // Event when character max HP changes
//			MpValueChangeEvent = new UnityEvent(); // Event when character gains/loses current HP
//			MpModEvent = new UnityEvent();         // Event when character max HP changes
//			AtkModEvent = new UnityEvent();        // Event when character base atk changes
//			DefModEvent = new UnityEvent();        // Event when character base def changes
//			SpdModEvent = new UnityEvent();        // Event when character base spd changes
			InitAllPlayerEvents();
			InitAllEnemyEvents();
		}

		public void InitAllPlayerEvents() {
			InitPlayerEvents(allyIncapacitatedEvents);
			InitPlayerEvents(hpValueChangeEvents);
			InitPlayerEvents(mpValueChangeEvents);
			InitPlayerEvents(hpModEvents);
			InitPlayerEvents(mpModEvents);
			InitPlayerEvents(atkModEvents);
			InitPlayerEvents(defModEvents);
			InitPlayerEvents(spdModEvents);
		}
		
		public void InitAllEnemyEvents() {
			InitEnemyEvents(enemyHpValueChangeEvents);
			InitEnemyEvents(enemyMpValueChangeEvents);
			InitEnemyEvents(enemyHpModEvents);
			InitEnemyEvents(enemyMpModEvents);
			InitEnemyEvents(enemyAtkModEvents);
			InitEnemyEvents(enemyDefModEvents);
			InitEnemyEvents(enemySpdModEvents);
		}
		
		private void InitPlayerEvents(Dictionary<CharacterSlot, UnityEvent> events) {
			if (events.Count == 0) {
				int i = 0;
				foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot))) {
					if (i >= PlayerManager.MAX_PARTY_MEMBERS) {
						break;
					}

					UnityEvent statModEvent = new UnityEvent();
					events.Add(slot, statModEvent);
					i++;
				}
			}
		}

		private void InitEnemyEvents(Dictionary<CharacterSlot, UnityEvent> events) {
			if (events.Count == 0) {
				foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot))) {
					UnityEvent statModEvent = new UnityEvent();
					events.Add(slot, statModEvent);
				}
			}
		}
		
		public void ListenOnPlayerStats(PlayerStatBar statBar) {
			statBar.MyPlayer = PlayerManager.pm.GetPartyMemberLocations()[statBar.slot];
			AddHpValueChangeListener(statBar.MyPlayer, statBar.UpdateHpBar);
			AddMpValueChangeListener(statBar.MyPlayer, statBar.UpdateMpBar);
			AddHpModListener(statBar.MyPlayer, statBar.UpdateHpStatSprite);
			AddAtkModListener(statBar.MyPlayer, statBar.UpdateAtkStatSprite);
			AddDefModListener(statBar.MyPlayer, statBar.UpdateDefStatSprite);
			AddSpdModListener(statBar.MyPlayer, statBar.UpdateSpdStatSprite);
//			statBar.MyPlayer.AddHpValueChangeListener(statBar.UpdateHpBar);
//			statBar.MyPlayer.AddMpValueChangeListener(statBar.UpdateMpBar);
//			statBar.MyPlayer.AddHpModListener(statBar.UpdateHpStatSprite);
//			statBar.MyPlayer.AddAtkModListener(statBar.UpdateAtkStatSprite);
//			statBar.MyPlayer.AddDefModListener(statBar.UpdateDefStatSprite);
//			statBar.MyPlayer.AddSpdModListener(statBar.UpdateSpdStatSprite);
		}
		
		public void RemoveListenOnPlayerStats(PlayerStatBar statBar) {
			statBar.MyPlayer = PlayerManager.pm.GetPartyMemberLocations()[statBar.slot];
			RemoveHpValueChangeListener(statBar.MyPlayer, statBar.UpdateHpBar);
			RemoveMpValueChangeListener(statBar.MyPlayer, statBar.UpdateMpBar);
			RemoveHpModListener(statBar.MyPlayer, statBar.UpdateHpStatSprite);
			RemoveAtkModListener(statBar.MyPlayer, statBar.UpdateAtkStatSprite);
			RemoveDefModListener(statBar.MyPlayer, statBar.UpdateDefStatSprite);
			RemoveSpdModListener(statBar.MyPlayer, statBar.UpdateSpdStatSprite);
		}


//		/// <summary>
//		/// Add listeners on when this character's health is changed
//		/// </summary>
//		/// <param name="character"></param>
//		/// <param name="call">Takes a float as a parameter for the listener.</param>
//		public void AddHpValueChangeListener(Player character, UnityAction<Character, float> call) {
//			hpValueChangeEvents[character.slot].AddListener(() => call(character, character.GetCurrentHp() / (float) character.GetMaxHp()));
//		}
//	
//		public void AddMpValueChangeListener(Player character, UnityAction<Character, float> call) {
//			mpValueChangeEvent.AddListener(() => call(this, currentMp / (float) maxMp));
//		}
//	
//		/// <summary>
//		/// Add listeners on when this character's base attack is changed
//		/// </summary>
//		/// <param name="call"></param>
//		public void AddHpModListener(Player character, UnityAction<bool, bool> call) {
//			hpModEvent.AddListener(() => call(AfflictedByStatChange(StatChange.HpUp), AfflictedByStatChange(StatChange.HpDestruct)));
//		}
//	
//		public void AddMpModListener(Player character, UnityAction<bool, bool> call) {
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

//		public void InitAllyIncapacitatedEvents() {
//			if (allyIncapacitatedEvents.Count == 0) {
//				foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot))) {
//					UnityEvent incapacitatedEvent = new UnityEvent();
//					allyIncapacitatedEvents.Add(slot, incapacitatedEvent);
//				}
//			}
//		}
		
		public void InitEnemyIncapacitatedEvents(int count) {
			for (int i = 0; i < count; i++) {
				UnityEvent incapacitatedEvent = new UnityEvent();
				enemyIncapacitatedEvents.Add((CharacterSlot)i, incapacitatedEvent);
			}
		}

		public void AddAllyIncapacitatedListener(CharacterSlot slot, GameObject ally, UnityAction<GameObject> call) {
			if (allyIncapacitatedEvents.Count == 0) {
				InitPlayerEvents(allyIncapacitatedEvents);
			}
			allyIncapacitatedEvents[slot].AddListener(() => call(ally));
		}

		/// <summary>
		/// Add listeners on when this character is incapacitated
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="enemy"></param>
		/// <param name="call"></param>
		public void AddEnemyIncapacitatedListener(CharacterSlot slot, GameObject enemy, UnityAction<GameObject> call) {
			if (enemyIncapacitatedEvents.Count == 0) {
				InitPlayerEvents(enemyIncapacitatedEvents);
			}
			Debug.LogFormat("Adding incapacitated listener.");
			enemyIncapacitatedEvents[slot].AddListener(() => call(enemy));
		}

		public void InvokeIncapacitatedEvent(Character character) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					allyIncapacitatedEvents[character.slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyIncapacitatedEvents[character.slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Remove listeners on when this character is incapacitated
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="enemy"></param>
		/// <param name="call"></param>
		public void RemoveEnemyIncapacitatedListeners(CharacterSlot slot, GameObject enemy, UnityAction<GameObject> call) {
			enemyIncapacitatedEvents[slot].RemoveListener(() => call(enemy));
			enemyIncapacitatedEvents.Remove(slot);
		}

		/// <summary>
		/// Clear enemy incapacitated events
		/// </summary>
		public void ClearIncapacitatedEvents() {
			enemyIncapacitatedEvents.Clear();
		}
		
		public void ClearEnemyStatEvents() {
			enemyHpValueChangeEvents.Clear();
			enemyMpValueChangeEvents.Clear();
			enemyHpModEvents.Clear();
			enemyMpModEvents.Clear();
			enemyAtkModEvents.Clear();
			enemyDefModEvents.Clear();
			enemySpdModEvents.Clear();
		}
		
		public void InvokeHpValueChangeEvent(Affiliation affiliation, CharacterSlot slot) {
			Debug.LogFormat("slot {0}", slot);
			switch (affiliation) {
				case Affiliation.Ally:
					hpValueChangeEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyHpValueChangeEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void InvokeMpValueChangeEvent(Affiliation affiliation, CharacterSlot slot) {
			switch (affiliation) {
				case Affiliation.Ally:
					mpValueChangeEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyMpValueChangeEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void InvokeHpModEvent(Affiliation affiliation, CharacterSlot slot) {
			switch (affiliation) {
				case Affiliation.Ally:
					hpModEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyHpModEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void InvokeMpModEvent(Affiliation affiliation, CharacterSlot slot) {
			switch (affiliation) {
				case Affiliation.Ally:
					mpModEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyMpModEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void InvokeAtkModEvent(Affiliation affiliation, CharacterSlot slot) {
			switch (affiliation) {
				case Affiliation.Ally:
					atkModEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyAtkModEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void InvokeDefModEvent(Affiliation affiliation, CharacterSlot slot) {
			switch (affiliation) {
				case Affiliation.Ally:
					defModEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemyDefModEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void InvokeSpdModEvent(Affiliation affiliation, CharacterSlot slot) {
			switch (affiliation) {
				case Affiliation.Ally:
					spdModEvents[slot].Invoke();
					break;
				case Affiliation.Enemy:
					enemySpdModEvents[slot].Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public void AddHpValueChangeListener(Character character, UnityAction<Character, float> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					hpValueChangeEvents[character.slot].AddListener(() => call(character, character.GetCurrentHp() / (float) character.GetMaxHp()));
					break;
				case Affiliation.Enemy:
					enemyHpValueChangeEvents[character.slot].AddListener(() => call(character, character.GetCurrentHp() / (float) character.GetMaxHp()));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public void AddMpValueChangeListener(Character character, UnityAction<Character, float> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					mpValueChangeEvents[character.slot].AddListener(() => call(character, character.GetCurrentMp() / (float) character.GetMaxMp()));
					break;
				case Affiliation.Enemy:
					enemyMpValueChangeEvents[character.slot].AddListener(() => call(character, character.GetCurrentMp() / (float) character.GetMaxMp()));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Add listeners on when this character's base attack is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void AddHpModListener(Character character, UnityAction<bool, bool> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					hpModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.HpUp), character.AfflictedByStatChange(StatChange.HpDestruct)));
					break;
				case Affiliation.Enemy:
					enemyHpModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.HpUp), character.AfflictedByStatChange(StatChange.HpDestruct)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	
		public void AddMpModListener(Character character, UnityAction<bool, bool> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					mpModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.HpUp), character.AfflictedByStatChange(StatChange.HpDestruct)));
					break;
				case Affiliation.Enemy:
					enemyMpModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.HpUp), character.AfflictedByStatChange(StatChange.HpDestruct)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Add listeners on when this character's base attack is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void AddAtkModListener(Character character, UnityAction<bool, bool> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					atkModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.AtkUp), character.AfflictedByStatChange(StatChange.AtkDown)));
					break;
				case Affiliation.Enemy:
					enemyAtkModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.AtkUp), character.AfflictedByStatChange(StatChange.AtkDown)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Add listeners on when this character's base defense is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void AddDefModListener(Character character, UnityAction<bool, bool> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					defModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.DefUp), character.AfflictedByStatChange(StatChange.DefDown)));
					break;
				case Affiliation.Enemy:
					enemyDefModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.DefUp), character.AfflictedByStatChange(StatChange.DefDown)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Add listeners on when this character's base speed is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void AddSpdModListener(Character character, UnityAction<bool, bool> call) {
			switch (character.GetAffiliation()) {
				case Affiliation.Ally:
					spdModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.SpdUp), character.AfflictedByStatChange(StatChange.SpdDown)));
					break;
				case Affiliation.Enemy:
					enemySpdModEvents[character.slot].AddListener(() => call(character.AfflictedByStatChange(StatChange.SpdUp), character.AfflictedByStatChange(StatChange.SpdDown)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		
		// REMOVE EVENTS
		
		
		public void RemoveHpValueChangeListener(Character character, UnityAction<Character, float> call) {
			hpValueChangeEvents[character.slot].RemoveListener(() => call(character, character.GetCurrentHp() / (float) character.GetMaxHp()));
		}
		
		public void RemoveMpValueChangeListener(Character character, UnityAction<Character, float> call) {
			mpValueChangeEvents[character.slot].RemoveListener(() => call(character, character.GetCurrentMp() / (float) character.GetMaxMp()));
		}

		/// <summary>
		/// Add listeners on when this character's base attack is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void RemoveHpModListener(Character character, UnityAction<bool, bool> call) {
			hpModEvents[character.slot].RemoveListener(() => call(character.AfflictedByStatChange(StatChange.HpUp), character.AfflictedByStatChange(StatChange.HpDestruct)));
		}
	
		public void RemoveMpModListener(Character character, UnityAction<bool, bool> call) {
			mpModEvents[character.slot].RemoveListener(() => call(character.AfflictedByStatChange(StatChange.HpUp), character.AfflictedByStatChange(StatChange.HpDestruct)));
		}

		/// <summary>
		/// Add listeners on when this character's base attack is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void RemoveAtkModListener(Character character, UnityAction<bool, bool> call) {
			atkModEvents[character.slot].RemoveListener(() => call(character.AfflictedByStatChange(StatChange.AtkUp), character.AfflictedByStatChange(StatChange.AtkDown)));
		}

		/// <summary>
		/// Add listeners on when this character's base defense is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void RemoveDefModListener(Character character, UnityAction<bool, bool> call) {
			defModEvents[character.slot].RemoveListener(() => call(character.AfflictedByStatChange(StatChange.DefUp), character.AfflictedByStatChange(StatChange.DefDown)));
		}

		/// <summary>
		/// Add listeners on when this character's base speed is changed
		/// </summary>
		/// <param name="character"></param>
		/// <param name="call"></param>
		public void RemoveSpdModListener(Character character, UnityAction<bool, bool> call) {
			spdModEvents[character.slot].RemoveListener(() => call(character.AfflictedByStatChange(StatChange.SpdUp), character.AfflictedByStatChange(StatChange.SpdDown)));
		}
	}
}
