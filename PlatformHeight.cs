using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformHeight : MonoBehaviour {

	public float height;
	public bool fixedHeight;
	public bool fixedSorting;
	public bool checkUpCollider;
	public bool debugLines = true;
	public bool startRayAboveEdge;
	public float debugRayPos;
	public Color upColor;
	public Color downColor;
	//public LayerMask groundMask = LayerMask.GetMask("Ground");
	//public LayerMask voidMask = LayerMask.GetMask("Void");

	//private Rigidbody2D rb2d;
	private CompositeCollider2D coll;
	private ContactFilter2D groundContactFilter;
	private ContactFilter2D voidContactFilter;
	private RaycastHit2D[] groundHits = new RaycastHit2D[3];
	private RaycastHit2D[] voidHits = new RaycastHit2D[3];
	private Vector3 platformPosition;
	private Vector3 bottomOfPlatform;

	void OnEnable()  {
		// If platform doesn't have a fixed height, calculate the height from the bottom of the platform
		if (!fixedHeight) {
			groundContactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
			coll = GetComponent<CompositeCollider2D>();
			GameObject dummyObj = GetComponentInChildren<Objecto>().gameObject;
			platformPosition = dummyObj.transform.position;
			Debug.Log(string.Format("{0} pos: {1}", name, platformPosition));
			if(!checkUpCollider) {
				if (!startRayAboveEdge) {
					bottomOfPlatform = new Vector3(platformPosition.x, platformPosition.y - coll.bounds.extents.y - .1f,
					platformPosition.z);
				} else {
					bottomOfPlatform = new Vector3(platformPosition.x, platformPosition.y - coll.bounds.extents.y + .1f,
					platformPosition.z);
				}
				Physics2D.Raycast(bottomOfPlatform, Vector2.down, groundContactFilter, groundHits, Mathf.Infinity);
			} else {
				if (startRayAboveEdge) {
					bottomOfPlatform = new Vector3(platformPosition.x, platformPosition.y - coll.bounds.extents.y + .1f,
					platformPosition.z);
				} else {
					bottomOfPlatform = new Vector3(platformPosition.x, platformPosition.y - coll.bounds.extents.y - .1f,
					platformPosition.z);
				}
				Physics2D.Raycast(bottomOfPlatform, Vector2.up, groundContactFilter, groundHits, Mathf.Infinity);
			}
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
	void Update() {
		if (debugLines) {
			Vector3 lineOrigin = new Vector3(bottomOfPlatform.x + 0.5f, bottomOfPlatform.y, bottomOfPlatform.y);
			if (!checkUpCollider) {
				Debug.DrawRay(lineOrigin, Vector3.down * height, downColor);
			} else {
				Debug.DrawRay(lineOrigin, Vector3.up * height, upColor);
			}
		}
	}
}
