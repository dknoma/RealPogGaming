using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformHeight : MonoBehaviour {

	public float height;
	public bool fixedHeight;
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
			GameObject platform = GetComponentInChildren<PlatformPosition>().gameObject;
			platformPosition = platform.transform.position;
			Debug.Log(string.Format("{0} pos: {1}", name, platformPosition));
			bottomOfPlatform = new Vector3(platformPosition.x, platformPosition.y - coll.bounds.extents.y - .1f,
				platformPosition.z);
			int hits = Physics2D.Raycast(bottomOfPlatform, Vector2.down, groundContactFilter, groundHits, Mathf.Infinity);
			Debug.Log(string.Format("{0} from center: {1}, name: {2}", name,
				Mathf.Round(groundHits[0].distance), groundHits[0].collider.name));
			height = Mathf.Round(groundHits[0].distance);
			GetComponent<Renderer>().sortingOrder = platform.GetComponent<Renderer>().sortingOrder;
		} else {
			Debug.Log(string.Format("Using a fixed height: {0}", height));
		}
	}

	// Draw debug ray to show where the raycast is originating from
    void Update() {
		Debug.DrawRay(bottomOfPlatform, Vector3.down*10, Color.red);
	}
}
