using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	// Use a queue to insert character actions
	//		Resets every turn; turns determined when actionqueue is empty
	private BattleStates battle;
	private bool battleInProgress = false;

	void Start () {
		this.battle = transform.GetComponentInChildren<BattleStates> ();
	}

	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			if (!this.battleInProgress) {
				this.battleInProgress = true;
				this.battle.InitBattle ();
//				Debug.Log ("Battle ended.");
			}
		}
		if(Input.GetButtonDown("Fire3")) {
			this.battle.EndBattle (BattleStates.WinStatus.Escape);
			this.battleInProgress = false;
			Debug.Log ("Ending the battle...");
		}
	}
}
