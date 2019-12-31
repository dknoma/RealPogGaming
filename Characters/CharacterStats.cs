// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Managers;
// using UnityEngine;
// using UnityEngine.Events;

// public enum CharacterSlot {
// 	One,
// 	Two,
// 	Three,
// 	Four,
// 	Five,
// }

// public enum Affiliation {
// 	Ally,
// 	Enemy
// }

// namespace Characters {
// 	public class CharacterStats : StatusEffects {
		
// 		[SerializeField] protected Affiliation affiliation;
		
// 		/* Public/inspector elements */
// 		public int expToGrant = 10;
// 		public bool bogResist;
// 		public bool burnResist;
// 		public bool poisonResist;
// 		public bool runeLockResist;
// 		public bool stunResist;
// 		public bool silenceResist;
// 		public Element element;

// 		protected int maxLevel = 100;
// 		public int currentLevel = 1;

// 		public int expUntilLevelUp;
// 		public int currentExp;
// 		protected int potentialExp;

// 		[SerializeField]
// 		protected int baseHp = 20;
// 		protected int maxHp = 20;
// 		[SerializeField]
// 		protected int currentHp;
// 		protected int totalRuneHp;
// 		[SerializeField]
// 		protected int baseMp = 15;
// 		protected int maxMp = 15;
// 		[SerializeField]
// 		protected int currentMp;
// 		protected int totalRuneMp;
// 		[SerializeField]
// 		protected int baseAtk = 5;
// 		protected int currentAtk;
// 		protected int totalRuneAtk;
// 		[SerializeField]
// 		protected int baseDef = 5;
// 		protected int currentDef;
// 		protected int totalRuneDef;
// 		[SerializeField]
// 		protected int baseSpd = 5;
// 		protected int currentSpd;
// 		protected int totalRuneSpd;
// 		protected int readiness;
// 		protected bool ready;
// 		protected const int READINESS_THRESHOLD = 100;

// 		public int Readiness {
// 			get { return readiness; }
// 			set { readiness = value; }
// 		}
	
// 		public bool Ready {
// 			get { return ready; }
// 			set { ready = value; }
// 		}

// 		//	private StatusEffects characterStatus = new StatusEffects();
// 		protected Dictionary<Status, int> statusTurnCounter = new Dictionary<Status, int>();
// 		protected Dictionary<StatChange, int> statChangeTurnCounter = new Dictionary<StatChange, int>();
// 		protected bool disableRunes;
// 		protected bool canAct;
// 		protected bool canCastSpells;
// 		protected bool hasStatusAffliction;
// 	}
// }
