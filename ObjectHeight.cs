using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectHeight : MonoBehaviour {

	public float height;
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
	private Vector3 bottomeOfObject;

	void OnEnable() {
		// If platform doesn't have a fixed height, calculate the height from the bottom of the platform
		if (!fixedHeight) {
			objectContactFilter.SetLayerMask(objectMaskToCheck);
			//platformContactFilter.SetLayerMask(LayerMask.GetMask("Platform"));
			coll = GetComponent<CompositeCollider2D>();
			GameObject dummyObj = GetComponentInChildren<ObjectPosition>().gameObject;
			objectPosition = dummyObj.transform.position;
			Debug.Log(string.Format("{0} pos: {1}", name, objectPosition));

			// Send raycast down to check the height of this object
			if (!checkUpCollider) {
				if (!startRayAboveEdge) {
					bottomeOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y - 0.1f,
						objectPosition.z);
				} else {
					bottomeOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y + 0.1f,
						objectPosition.z);
				}
			} else {
				if (startRayAboveEdge) {
					bottomeOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y + 0.1f,
						objectPosition.z);
				} else {
					bottomeOfObject = new Vector3(objectPosition.x, objectPosition.y - coll.bounds.extents.y - 0.1f,
						objectPosition.z);
				}
			}
			Physics2D.Raycast(bottomeOfObject, Vector2.up, objectContactFilter, groundHits, Mathf.Infinity);
			Debug.Log(string.Format("{0} from center: {1}, name: {2}", name,
				Mathf.Round(groundHits[0].distance), groundHits[0].collider.name));
			height = Mathf.Round(groundHits[0].distance);
			// Set sort order to platform positions sorting order
			if (!fixedSorting && dummyObj.GetComponent<Renderer>() != null) {
				GetComponent<Renderer>().sortingOrder = dummyObj.GetComponent<Renderer>().sortingOrder;
			}
		} else {
			Debug.Log(string.Format("Using a fixed height: {0}", height));
		}
	}

	// Draw debug ray to show where the raycast is originating from
	// Red line means the cast is down; yellow is up
	void Update() {
		if (debugLines) {
			Vector3 lineOrigin = new Vector3(bottomeOfObject.x + debugRayPos, bottomeOfObject.y, bottomeOfObject.y);
			if (!checkUpCollider) {
				Debug.DrawRay(lineOrigin, Vector3.down * height, downColor);
			} else {
				Debug.DrawRay(lineOrigin, Vector3.up * height, upColor);
			}
		}
	}
}
