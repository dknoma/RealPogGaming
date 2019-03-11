using UnityEngine;

[ExecuteInEditMode]
public class TopDownOrderRender : MonoBehaviour {

	private Renderer myRenderer;
	private ObjectInfo objInfo;
	
	private void OnValidate() {
		objInfo = GetComponentInParent<ObjectInfo>();
		myRenderer = GetComponent<Renderer>();
		if (myRenderer == null) {
			Debug.LogError("Parent did not have a Renderer attached to it. Cannot change sorting order.", this);
		}
	}

	private void Update() {
		// Get object height object from parent
		// Update the objects sorting order depending on if want to determine
		// order w/ height
		myRenderer.sortingOrder = (int)(transform.position.y * -10 + (objInfo != null && objInfo.determinePriortyWithHeight ? objInfo.height : 0) * 10);
	}
}
