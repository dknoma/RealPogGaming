using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour {

	public PlayerSlot slot;
	[SerializeField] private Transform hpBar;
	[SerializeField] private Transform mpBar;

	public void UpdateHpBar(float percent) {
		if (percent <= 0) {
			hpBar.localScale = Vector3.zero;
		}
		Vector3 scale = hpBar.localScale;
		scale.x = hpBar.localScale.x * percent;
		hpBar.localScale = scale;
	}
	
	public void UpdateMpBar(float percent) {
		if (percent <= 0) {
			hpBar.localScale = Vector3.zero;
		}
		Vector3 scale = mpBar.localScale;
		scale.x = mpBar.localScale.x * percent;
		mpBar.localScale = scale;
	}
}
