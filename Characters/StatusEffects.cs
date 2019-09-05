using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters {
	public enum Status {
		Bog, 
		Burn, 
		Poison, 
		Stun, 
		HeartBind,
		SoulBind,
		RuneBind, 
		Incapacitated
	}
	public enum StatChange { 
		AtkUp, 
		DefUp, 
		SpdUp, 
		HpUp, 
		MpUp,
		AtkDown, 
		DefDown, 
		SpdDown, 
		HpDestruct,
		MpDown
	}
/*
 * Player status effect class. 
 */
	public class StatusEffects : MonoBehaviour {

		//	public enum StatUps { PATKUp, MATKUp, /*SpdUp,*/ PDEFUp, MDEFUp, HPUp }; 
		//	public enum StatDowns { PATKDown, MATKDown, /*SpdDown,*/ PDEFDown, MDEFDown, HPDestruct };
//	public enum StatChange { PATKUp, MATKUp, /*SpdUp,*/ PDEFUp, MDEFUp, HPUp, 
//		PATKDown, MATKDown, /*SpdDown,*/ PDEFDown, MDEFDown, HPDestruct}

		protected readonly Dictionary<Status, bool> afflictedStatuses = new Dictionary<Status, bool>();
		protected readonly Dictionary<StatChange, bool> afflictedStatChanges = new Dictionary<StatChange, bool>();
		protected readonly Dictionary<Status, bool> statusResists = new Dictionary<Status, bool>();
		protected readonly Dictionary<StatChange, bool> statChangeResists = new Dictionary<StatChange, bool>();
		protected readonly Dictionary<Status, bool> statusRemovalResists = new Dictionary<Status, bool>();
		protected readonly Dictionary<StatChange, bool> statChangeRemovalResists = new Dictionary<StatChange, bool>();

		// Decides whether a character has a status affliction or not
//	protected bool[] afflictedStatuses = new bool[(int) Status.SoulBind+1];
		//	private bool[] afflictedStatUpStatus = new bool[StatUps.HPUp+1];
		//	private bool[] afflictedStatDownStatus = new bool[StatDowns.HPDestruct+1];
//	protected bool[] afflictedStatChanges = new bool[(int) StatChange.HpDestruct+1];
		// Decides whether a character is immune to a state or not
//	protected bool[] statusResists = new bool[(int) Status.SoulBind+1];
//	protected bool[] statChangeResists = new bool[(int) StatChange.HpDestruct+1];
//	protected bool[] statusRemovalResists = new bool[(int) Status.SoulBind+1];	
//	protected bool[] statChangeRemovalResists = new bool[(int) StatChange.HpDestruct+1];

		private void Awake() {
			// TODO: check persistant data for statuses, else init all to false
			foreach (Status status in Enum.GetValues(typeof(Status))) {
				afflictedStatuses.Add(status, false);
				statusResists.Add(status, false);
				statusRemovalResists.Add(status, false);
			}
			foreach (StatChange statChange in Enum.GetValues(typeof(StatChange))) {
				afflictedStatChanges.Add(statChange, false);
				statChangeResists.Add(statChange, false);
				statChangeRemovalResists.Add(statChange, false);
			}
		}

		/* 
	 * Afflictions 
	 */
		protected Dictionary<Status, bool> GetStatusAfflictions() {
			return afflictedStatuses;
		}

		protected Dictionary<StatChange, bool> GetStatChangeAfflictions() {
			return afflictedStatChanges;
		}

		public bool AfflictedByStatus(Status status) {
			return afflictedStatuses.ContainsKey(status) && afflictedStatuses[status];
		}

		public bool AfflictedByStatChange(StatChange statChange) {
			return afflictedStatChanges.ContainsKey(statChange) && afflictedStatChanges[statChange];
		}

		protected void AfflictStatus(Status status) {
			if (!afflictedStatuses.ContainsKey(status)) {
				afflictedStatuses.Add(status, true);
				return;
			}
			afflictedStatuses[status] = true;
		}

		protected void RemoveStatus(Status status) {
			afflictedStatuses[status] = false;
		}

		protected void AfflictStatChange(StatChange statChange) {
			if (!afflictedStatChanges.ContainsKey(statChange)) {
				afflictedStatChanges.Add(statChange, true);
				return;
			}
			afflictedStatChanges[statChange] = true;
		}

		protected void RemoveStatChange(StatChange statChange) {
			afflictedStatChanges[statChange] = false;
		}

		/* 
	 * Resists 
	 */
		public void AddStatusResist(Status status) {
			statusResists[status] = true;
		}

		public void RemoveStatusResist(Status status) {
			statusResists[status] = false;
		}

		protected bool ResistsStatusEffect(Status status) {
			return statusResists.ContainsKey(status) && statusResists[status];
		}

		protected bool ResistsStatChange(StatChange statChange) {
			return statChangeResists.ContainsKey(statChange) && statChangeResists[statChange];
		}

		protected bool ResistsStatusEffectRemoval(Status status) {
			return statusRemovalResists.ContainsKey(status) && statusRemovalResists[status];
		}

		protected bool ResistsStatChangeRemoval(StatChange statChange) {
			return statChangeRemovalResists.ContainsKey(statChange) && statChangeRemovalResists[statChange];
		}
//	protected bool afflictedByStatus(Status status) {
//		return this.afflictedStatuses[(int) status];
//	}
//
//	protected bool afflictedByStatChange(StatChange statChange) {
//		return this.afflictedStatChanges[(int) statChange];
//	}
//
//	protected void afflictStatus(Status status) {
//		this.afflictedStatuses[(int) status] = true;
//	}
//
//	protected void removeStatus(int status) {
//		this.afflictedStatuses[status] = false;
//	}
//
//	protected void afflictStatChange(StatChange statChange) {
//		this.afflictedStatChanges[(int) statChange] = true;
//	}
//
//	protected void removeStatChange(int statChange) {
//		this.afflictedStatChanges[statChange] = false;
//	}
//
//	/* 
//	 * Resists 
//	 */
//	protected bool resistsStatusEffect(Status status) {
//		return this.statusResists[(int) status];
//	}
//
//	protected bool resistsStatChange(StatChange statChange) {
//		return this.statChangeResists[(int) statChange];
//	}
//
//	protected bool resistsStatusEffectRemoval(Status status) {
//		return this.statusRemovalResists[(int) status];
//	}
//
//	protected bool resistsStatChangeRemoval(StatChange statChange) {
//		return this.statChangeRemovalResists[(int) statChange];
//	}
	}
}