using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectHeight : MonoBehaviour {

	public float height;
	public bool determinePriortyWithHeight;
	public bool fixedHeight;
	public bool fixedSorting;
	public bool checkUpCollider;
	public bool startRayAboveEdge;
	public bool debugLines = true;
	public LayerMask objectMaskToCheck;
	public float debugRayPos;
	public Color upColor;
	public Color downColor;

	private CompositeCollider2D coll;
	private ContactFilter2D objectContactFilter;
	private ContactFilter2D  platformContactFilter;
	private ContactFilter2D voidContactFilter;
	private RaycastHit2D[] groundHits = new RaycastHit2D[3];
	private RaycastHit2D[] voidHits = new RaycastHit2D[3];
	private Vector3 objectPosition;
	private Vector3 boundOfObject;

	void OnEnable() {
		// If platform doesn't have a fixed height, calculate the height from the bottom of the platform
		if (!fixedHeight) {
			objectContactFilter.SetLayerMask(objectMaskToCheck);
			//platformContactFilter.SetLayerMask(LayerMask.GetMask("Platform"));
			coll = GetComponent<CompositeCollider2D>();
			GameObject dummyObj = GetComponentInChildren<ObjectPosition>().gameObject;
			objectPosition = dummyObj.transform.position;
			//Debug.Log(string.Format("{0} pos: {1}", name, objectPosition));

			// Send raycast down to check the height of this object
			if (!checkUpCollider) {
				if (!startRayAboveEdge) {
					boundOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y - 0.1f,
						objectPosition.z);
				} else {
					boundOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y + 0.1f,
						objectPosition.z);
				}
				Physics2D.Raycast(boundOfObject, Vector2.down, objectContactFilter, groundHits, Mathf.Infinity);
			} else {
				if (startRayAboveEdge) {
					boundOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y + 0.1f,
						objectPosition.z);
				} else {
					boundOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y - 0.1f,
						objectPosition.z);
				}
				Physics2D.Raycast(boundOfObject, Vector2.up, objectContactFilter, groundHits, Mathf.Infinity);
			}
			//Debug.Log(string.Format("{0} from center: {1}, name: {2}", name,
				//Mathf.Round(groundHits[0].distance), groundHits[0].collider.name));
			height = Mathf.Round(groundHits[0].distance);
		} else {
			Debug.Log(string.Format("Using a fixed height: {0}", height));
		}
	}

	//private void OnValidate() {
	//	ObjectPosition dummyObj = GetComponentInChildren<ObjectPosition>();
	//	if(dummyObj == null) {
	//		Debug.unityLogger.LogError("ObjectHeight","ObjectPosition component could not be located in the children. Please make sure it's attached.", dummyObj);
	//	}
	//}

	// Draw debug ray to show where the raycast is originating from
	// Red line means the cast is down; yellow is up
	void Update() {
		if (debugLines) {
			Vector3 lineOrigin = new Vector3(boundOfObject.x + debugRayPos, boundOfObject.y, boundOfObject.y);
			if (!checkUpCollider) {
				Debug.DrawRay(lineOrigin, Vector3.down * height, downColor);
			} else {
				Debug.DrawRay(lineOrigin, Vector3.up * height, upColor);
			}
		}
		GameObject dummyObj = GetComponentInChildren<ObjectPosition>().gameObject;
		// Set sort order to platform positions sorting order
		if (!fixedSorting && dummyObj.GetComponent<Renderer>() != null) {
			GetComponent<Renderer>().sortingOrder = dummyObj.GetComponent<Renderer>().sortingOrder;
		}
	}
}
