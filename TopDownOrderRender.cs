using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TopDownOrderRender : MonoBehaviour {

	void Update() {
		// Get object height object from parent
		// Update the objects sorting order depending on if want to determine
		// order w/ height
		ObjectHeight objHeight = GetComponentInParent<ObjectHeight>();
		gameObject.GetComponent<Renderer>().sortingOrder = (int)(transform.position.y * -10 
		+ ((objHeight != null && objHeight.determinePriortyWithHeight ? objHeight.height : 0) * 10));
	}
}
