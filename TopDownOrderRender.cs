using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TopDownOrderRender : MonoBehaviour {

	void Update() {
		gameObject.GetComponent<Renderer>().sortingOrder = (int)(transform.position.y * -10);
	}
}
