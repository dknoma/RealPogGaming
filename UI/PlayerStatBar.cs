using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour {

	public PlayerSlot slot;
	[SerializeField] private Transform hpBar;
	[SerializeField] private Transform mpBar;
	
	[SerializeField] private SpriteRenderer hpModStatus;
	[SerializeField] private SpriteRenderer atkModStatus;
	[SerializeField] private SpriteRenderer defModStatus;
	[SerializeField] private SpriteRenderer spdModStatus;

	[SerializeField] private Sprite neutralSprite;
	[SerializeField] private Sprite statUpSprite;
	[SerializeField] private Sprite statDownSprite;

	public void ListenOnPlayerStats(Player player) {
		player.AddHpValueChangeListener(UpdateHpBar);
	}
	
	/// <summary>
	/// Listen in on the corresponding Player who's slot matches this one's
	/// </summary>
	public void ListenOnPlayerStats() {
		Player player = PlayerManager.pm.GetPartyMemberLocations()[slot];
		player.AddHpValueChangeListener(UpdateHpBar);
		player.AddHpModListener(UpdateHpStatSprite);
		player.AddAtkModListener(UpdateAtkStatSprite);
		player.AddDefModListener(UpdateDefStatSprite);
		player.AddSpdModListener(UpdateSpdStatSprite);
	}

	public void UpdateHpBar(float percent) {
		if (percent <= 0) {
			hpBar.localScale = Vector3.zero;
		}
		Vector3 scale = hpBar.localScale;
		scale.x = percent;	// % ratio of currentHP/maxHP
		hpBar.localScale = scale;
	}
	
	public void UpdateMpBar(float percent) {
		if (percent <= 0) {
			hpBar.localScale = Vector3.zero;
		}
		Vector3 scale = mpBar.localScale;
		scale.x = percent;	// % ratio of currentMP/maxMP
		mpBar.localScale = scale;
	}

	public void UpdateHpStatSprite(bool statUp, bool statDown) {
		if(statUp && !statDown) {
			hpModStatus.sprite = statUpSprite;
		} else if(!statUp && statDown) {
			hpModStatus.sprite = statDownSprite;
		} else {
			hpModStatus.sprite = neutralSprite;
		}
	}
	
	public void UpdateAtkStatSprite(bool statUp, bool statDown) {
		Debug.LogFormat("Updating atk stat sprite.");
		if(statUp && !statDown) {
			atkModStatus.sprite = statUpSprite;
		} else if(!statUp && statDown) {
			atkModStatus.sprite = statDownSprite;
		} else {
			atkModStatus.sprite = neutralSprite;
		}
	}
	
	public void UpdateDefStatSprite(bool statUp, bool statDown) {
		if(statUp && !statDown) {
			defModStatus.sprite = statUpSprite;
		} else if(!statUp && statDown) {
			defModStatus.sprite = statDownSprite;
		} else {
			defModStatus.sprite = neutralSprite;
		}
	}
	
	public void UpdateSpdStatSprite(bool statUp, bool statDown) {
		if(statUp && !statDown) {
			spdModStatus.sprite = statUpSprite;
		} else if(!statUp && statDown) {
			spdModStatus.sprite = statDownSprite;
		} else {
			spdModStatus.sprite = neutralSprite;
		}
	}
}
