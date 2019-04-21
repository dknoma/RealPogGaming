using System;
using System.Collections.Generic;
using BattleUI;
using Characters;
using UnityEngine;
using UnityEngine.Events;

namespace Managers {
	public class BattleEventManager : MonoBehaviour {

		public static BattleEventManager bem;
	
		// Action events
		public UnityEvent ActionEvent { get; private set; }
		
		public Dictionary<CharacterSlot, UnityEvent> allyIncapacitatedEvents= new Dictionary<CharacterSlot, UnityEvent>();
		public Dictionary<CharacterSlot, UnityEvent> enemyIncapacitatedEvents = new Dictionary<CharacterSlot, UnityEvent>();

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
//			HpModEvent = new UnityEvent();                                 // Event when player max HP changes
//			MpValueChangeEvent = new UnityEvent(); // Event when player gains/loses current HP
//			MpModEvent = new UnityEvent();         // Event when player max HP changes
//			AtkModEvent = new UnityEvent();        // Event when player base atk changes
//			DefModEvent = new UnityEvent();        // Event when player base def changes
//			SpdModEvent = new UnityEvent();        // Event when player base spd changes
			if (allyIncapacitatedEvents.Count == 0) {
				foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot))) {
					UnityEvent incapacitatedEvent = new UnityEvent();
					allyIncapacitatedEvents.Add(slot, incapacitatedEvent);
				}
			}
		}

		public void ListenOnPlayerStats(PlayerStatBar statBar) {
			statBar.MyPlayer = PlayerManager.pm.GetPartyMemberLocations()[statBar.slot];
//			AddHpValueChangeListener(statBar.MyPlayer, statBar.UpdateHpBar);
//			AddMpValueChangeListener(statBar.MyPlayer, statBar.UpdateMpBar);
//			AddHpModListener(statBar.MyPlayer, statBar.UpdateHpStatSprite);
//			AddAtkModListener(statBar.MyPlayer, statBar.UpdateAtkStatSprite);
//			AddDefModListener(statBar.MyPlayer, statBar.UpdateDefStatSprite);
//			AddSpdModListener(statBar.MyPlayer, statBar.UpdateSpdStatSprite);
			statBar.MyPlayer.AddHpValueChangeListener(statBar.UpdateHpBar);
			statBar.MyPlayer.AddMpValueChangeListener(statBar.UpdateMpBar);
			statBar.MyPlayer.AddHpModListener(statBar.UpdateHpStatSprite);
			statBar.MyPlayer.AddAtkModListener(statBar.UpdateAtkStatSprite);
			statBar.MyPlayer.AddDefModListener(statBar.UpdateDefStatSprite);
			statBar.MyPlayer.AddSpdModListener(statBar.UpdateSpdStatSprite);
		} 

		public void InitAllyIncapacitatedEvents() {
			if (allyIncapacitatedEvents.Count == 0) {
				foreach (CharacterSlot slot in Enum.GetValues(typeof(CharacterSlot))) {
					UnityEvent incapacitatedEvent = new UnityEvent();
					allyIncapacitatedEvents.Add(slot, incapacitatedEvent);
				}
			}
		}
		
		public void InitEnemyIncapacitatedEvents(int count) {
			for (int i = 0; i < count; i++) {
				UnityEvent incapacitatedEvent = new UnityEvent();
				enemyIncapacitatedEvents.Add((CharacterSlot)i, incapacitatedEvent);
			}
		}

		public void AddAllyIncapacitatedListener(CharacterSlot slot, GameObject ally, UnityAction<GameObject> call) {
			allyIncapacitatedEvents[slot].AddListener(() => call(ally));
		}

		/// <summary>
		/// Add listeners on when this character is incapacitated
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="enemy"></param>
		/// <param name="call"></param>
		public void AddEnemyIncapacitatedListener(CharacterSlot slot, GameObject enemy, UnityAction<GameObject> call) {
			Debug.LogFormat("Adding incapacitated listener.");
//			UnityEvent incapacitatedEvent = new UnityEvent();
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
			allyIncapacitatedEvents.Clear();
			enemyIncapacitatedEvents.Clear();
		}
		
//		public void AddHpValueChangeListener(Player player, UnityAction<CharacterStats, float> call) {
//			HpValueChangeEvent.AddListener(() => call(player, player.GetCurrentHp() / (float) player.GetMaxHp()));
//		}
//		
//		public void AddMpValueChangeListener(Player player, UnityAction<CharacterStats, float> call) {
//			MpValueChangeEvent.AddListener(() => call(player, player.GetCurrentMp() / (float) player.GetMaxMp()));
//		}
//
//		/// <summary>
//		/// Add listeners on when this character's base attack is changed
//		/// </summary>
//		/// <param name="player"></param>
//		/// <param name="call"></param>
//		public void AddHpModListener(Player player, UnityAction<bool, bool> call) {
//			HpModEvent.AddListener(() => call(player.AfflictedByStatChange(StatChange.HpUp), player.AfflictedByStatChange(StatChange.HpDestruct)));
//		}
//	
//		public void AddMpModListener(Player player, UnityAction<bool, bool> call) {
//			MpModEvent.AddListener(() => call(player.AfflictedByStatChange(StatChange.HpUp), player.AfflictedByStatChange(StatChange.HpDestruct)));
//		}
//
//		/// <summary>
//		/// Add listeners on when this character's base attack is changed
//		/// </summary>
//		/// <param name="player"></param>
//		/// <param name="call"></param>
//		public void AddAtkModListener(Player player, UnityAction<bool, bool> call) {
//			AtkModEvent.AddListener(() => call(player.AfflictedByStatChange(StatChange.AtkUp), player.AfflictedByStatChange(StatChange.AtkDown)));
//		}
//
//		/// <summary>
//		/// Add listeners on when this character's base defense is changed
//		/// </summary>
//		/// <param name="player"></param>
//		/// <param name="call"></param>
//		public void AddDefModListener(Player player, UnityAction<bool, bool> call) {
//			DefModEvent.AddListener(() => call(player.AfflictedByStatChange(StatChange.DefUp), player.AfflictedByStatChange(StatChange.DefDown)));
//		}
//
//		/// <summary>
//		/// Add listeners on when this character's base speed is changed
//		/// </summary>
//		/// <param name="player"></param>
//		/// <param name="call"></param>
//		public void AddSpdModListener(Player player, UnityAction<bool, bool> call) {
//			SpdModEvent.AddListener(() => call(player.AfflictedByStatChange(StatChange.SpdUp), player.AfflictedByStatChange(StatChange.SpdDown)));
//		}
	}
}
