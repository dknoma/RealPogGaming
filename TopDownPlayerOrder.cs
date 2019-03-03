using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerOrder : MonoBehaviour {
// Update is called once per frame
	void Update(){
		// Get object height object from parent
		// Update the objects sorting order depending on if want to determine
		// order w/ height
		PlayerController character = GetComponentInParent<PlayerController>();
		gameObject.GetComponent<Renderer>().sortingOrder = (int)(transform.position.y * -10
		+ ((character != null  ? character.GetJumpingHeight() + (character.GetTotalHeight()*10) : 0) * 10));
	}
}
