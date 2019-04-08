using UnityEngine;
using UnityEngine.UI;

public class TargetButton : MenuButton {

	private GameObject targetUnit;
	
	public GameObject target {
		get {
			return targetUnit;
		}
		set {
			targetUnit = value;
		}
	}
}
