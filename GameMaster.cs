using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	// Use a queue to insert character actions
	//		Resets every turn; turns determined when actionqueue is empty
	private Battlefield battle;

	void Start () {
		this.battle = transform.GetComponentInChildren<Battlefield> ();
	}

	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			this.battle.InitBattle ();
			Debug.Log ("End");
		}
		if(Input.GetButtonDown("Fire2")) {
//			this.battle.endBattle (Battlefield.WinStatus.Escape);
			this.battle.debugList();
			Debug.Log ("End");
		}
	}
}
