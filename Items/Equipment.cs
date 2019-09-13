using System;
using UnityEngine;

namespace Items {
	public class Equipment : Item {
		[SerializeField] private EquipmentStats stats;

		public override string GetItemInfo() {
			return string.Format(" | Hp: {0} Atk: {1} Def: {2} Spd: {3}\n{4}",
	                            this.stats.getHp(), this.stats.getAtk(), this.stats.getDef(), this.stats.getSpd(),
								GetDescription());
		}
	}

	[Serializable]
	public class EquipmentStats {
		[SerializeField] private int hp;
		[SerializeField] private int atk;
		[SerializeField] private int def;
		[SerializeField] private int spd;

		public int getHp() {
			return this.hp;
		}
		
		public int getAtk() {
			return this.atk;
		}
		
		public int getDef() {
			return this.def;
		}
		
		public int getSpd() {
			return this.spd;
		}
	}
}
