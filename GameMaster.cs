using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	// Use a queue to insert character actions
	//		Resets every turn; turns determined when actionqueue is empty
	private BattleStates battle;
	private bool battleInProgress = false;
	private Camera cam;

	void Start () {
		battle = transform.GetComponentInChildren<BattleStates> ();
		cam = Camera.main;
	}

	void Update () {
		// Let camera focus on player
		Vector3 newCamPos = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);

		if (Input.GetButtonDown("Fire1")) {
			if (!battleInProgress) {
				battleInProgress = true;
				battle.InitBattle ();
//				Debug.Log ("Battle ended.");
			}
		}
		if(Input.GetButtonDown("Fire3")) {
			battle.EndBattle (BattleStates.WinStatus.Escape);
			battleInProgress = false;
			Debug.Log ("Ending the battle...");
		}
	}
}
