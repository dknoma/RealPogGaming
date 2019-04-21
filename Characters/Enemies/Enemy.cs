using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class Enemy : Character {

	public WeaponType weaponType;

	private Vector3 cursorSpot;
	private int enemySlot;

//	private void Awake() {
//		
//	}

	// Enemies don't necessarily have weapons, but having a weapon type may affect damage output in battle
	// Maybe we want this maybe we dont.
	public WeaponType GetWeaponType() {
		return weaponType;
	}
	
	public void DoDefeatEnemyAnimation() {
		StartCoroutine(DefeatEnemyAnimation());
	}
    
	private IEnumerator DefeatEnemyAnimation() {
		// Disable colliders and movement
		Debug.LogFormat("Defeated {0}", name);
		GetComponent<Collider2D>().enabled = false;
		SpriteRenderer enemyRenderer = GetComponent<SpriteRenderer>();
		int i = 1;
		while(i <= 20) {
//            enemyToBattle.SetActive(false);
			enemyRenderer.enabled = false;
			float secondsToWait = 0.7f / i;
			yield return new WaitForSeconds(secondsToWait);
			enemyRenderer.enabled = true;
			yield return new WaitForSeconds(secondsToWait);
			i += 2;
		}
		Debug.LogFormat("done");
		gameObject.SetActive(false);
	}
}
