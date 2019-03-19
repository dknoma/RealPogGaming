using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class ObjectInfo : MonoBehaviour {

	[Header("Object Info and Settings")]
	public float height;
	public bool determinePriortyWithHeight;
	public bool fixedHeight;
	public bool fixedSorting;
	public Vector3 position;
	[DisableInspectorEdit] public float h;
	[DisableInspectorEdit] public float w;
	[DisableInspectorEdit] public float topBound;
	[DisableInspectorEdit] public float bottomBound;
	[DisableInspectorEdit] public float rightBound;
	[DisableInspectorEdit] public float leftBound;
	[DisableInspectorEdit] public float baseTopBound;
	[DisableInspectorEdit] public float baseBottomBound;
	[DisableInspectorEdit] public float baseRightBound;
	[DisableInspectorEdit] public float baseLeftBound;
	[DisableInspectorEdit] public int sortingOrder;
	[DisableInspectorEdit] public ObjectValues values;
	

	private CompositeCollider2D coll;
	private ContactFilter2D objectContactFilter;
	private ContactFilter2D  platformContactFilter;
	private ContactFilter2D voidContactFilter;
	private Vector3 objectPosition;
	private Vector3 boundOfObject;

	private GameObject child;
	private Renderer myRenderer;
	private Renderer childRenderer;

	private void OnEnable() {
		myRenderer = GetComponent<Renderer>();
		if (transform.childCount > 0) {
			child = transform.GetChild(0).gameObject;
			Debug.Log("child " + child.name);
			childRenderer = child.GetComponent<Renderer>();
		}
		// If platform doesn't have a fixed height, calculate the height from the bottom of the platform
		if (!fixedHeight) {
			height = GetComponentInParent<PlatformInfo>() != null ? GetComponentInParent<PlatformInfo>().height : 0;
		} else {
			Debug.Log(string.Format("Using a fixed height: {0}", height));
		}
		
//		position = child.transform.TransformPoint(child.transform.position);
		position = child.transform.position;
		h = GetComponent<CompositeCollider2D>().bounds.extents.y * 2;
		w = GetComponent<CompositeCollider2D>().bounds.extents.x * 2;
		topBound = position.y + h / 2;
		bottomBound = position.y - h / 2;
		rightBound = position.x + w / 2;
		leftBound = position.x - w / 2;
		baseTopBound = topBound - height;
		baseBottomBound = bottomBound - height;
		baseRightBound = rightBound;
		baseLeftBound = leftBound;
		sortingOrder = childRenderer.sortingOrder;
		values = new ObjectValues(position, h, w, height, sortingOrder, name);
	}

	// Draw debug ray to show where the raycast is originating from
	// Red line means the cast is down; yellow is up
	private void Update() {
		// Set sort order to platform positions sorting order
		if (fixedSorting || childRenderer == null) return;
		myRenderer.sortingOrder = childRenderer.sortingOrder;
		values.sortingOrder = myRenderer.sortingOrder;
	}
}
