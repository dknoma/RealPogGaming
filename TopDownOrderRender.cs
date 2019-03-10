using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TopDownOrderRender : MonoBehaviour {

	//private void OnValidate() {
	//	Renderer myRenderer = GetComponentInParent<Renderer>();
	//	if (myRenderer == null) {
	//		Debug.LogError("Parent did not have a Renderer attached to it. Cannot change sorting order.", this);
	//	}
	//}

	void Update() {
		// Get object height object from parent
		// Update the objects sorting order depending on if want to determine
		// order w/ height
		ObjectInfo objInfo = GetComponentInParent<ObjectInfo>();
		gameObject.GetComponent<Renderer>().sortingOrder = (int)(transform.position.y * -10 
		+ ((objInfo != null && objInfo.determinePriortyWithHeight ? objInfo.height : 0) * 10));
	}
}
