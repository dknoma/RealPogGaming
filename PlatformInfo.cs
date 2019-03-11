using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformInfo : MonoBehaviour {

	[Header("Object Info and Settings")]
	public float height;
	public bool fixedHeight;

	private void OnEnable() {
		// If platform doesn't have a fixed height, calculate the height from the bottom of the platform
		if (!fixedHeight) {
			var platObj = gameObject.FindComponentInChildWithTag<Transform>("Platform");
			var baseObj = gameObject.FindComponentInChildWithTag<Transform>("Base");
			
			height = platObj.GetChild(0).position.y - baseObj.GetChild(0).position.y;
		} else {
			Debug.Log(string.Format("Using a fixed height: {0}", height));
		}
	}
}
