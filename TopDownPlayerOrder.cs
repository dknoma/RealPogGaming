using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(PlayerController))]
public class TopDownPlayerOrder : MonoBehaviour {

	private PlayerController character;
	private int platformSortOrder;

	private void OnEnable() {
		character = GetComponentInParent<PlayerController>();
	}

	void Update() {
		// Players sort oder cannot go any lower than the current platform's
		platformSortOrder = (int) (character.GetCurrentPlatform() != null ? character.GetCurrentPlatform().sortingOrder : -Mathf.Infinity);
		gameObject.GetComponent<Renderer>().sortingOrder = (int) Mathf.Clamp(transform.position.y * -10 + (character.currentHeight * 10), 
			platformSortOrder+1, Mathf.Infinity);
	}
}
