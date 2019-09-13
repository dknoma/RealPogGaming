using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
	[System.Serializable]
	[CreateAssetMenu(menuName = "Items/Weapon", fileName = "WeaponName.asset")]
	public class Weapon : Equipment {
		public enum WeaponType {
			Longsword,
			Greatsword,
			Lance,
			Dagger,
			Bow,
			Rod,
			Staff
		}

		public WeaponType weaponType;
	}

}
