using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class GameMaster : MonoBehaviour {

	public static GameMaster gm;
		
	// Use a queue to insert character actions
	//		Resets every turn; turns determined when actionqueue is empty
//	private bool battleInProgress = false;

	private void Awake () {
		if (gm == null) {
			gm = this;
		} else if (gm != this) {
			//If instance already exists and it's not this: destroy this game object
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
//		battle = transform.GetComponentInChildren<BattleStates> ();
	}
	

//	void Update () {
//		if (Input.GetButtonDown("Fire1")) {
//			if (!battleInProgress) {
//				battleInProgress = true;
//				battle.InitBattle ();
////				Debug.Log ("Battle ended.");
	//		}
	//	}
	//	if(Input.GetButtonDown("Fire3")) {
	//		battle.EndBattle (BattleStates.WinStatus.Escape);
	//		battleInProgress = false;
	//		Debug.Log ("Ending the battle...");
	//	}
	//}
}
