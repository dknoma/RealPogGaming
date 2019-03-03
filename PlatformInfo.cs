using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHeight))]
[ExecuteInEditMode]
public class PlatformInfo : MonoBehaviour {

	public Vector3 position;
	public float height;
	public float h;
	public float w;
	public float topBound;
	public float bottomBound;
	public float rightBound;
	public float leftBound;
	public int sortingOrder;

	private void OnEnable() {
		position = GetComponentInChildren<ObjectPosition>().transform.position;
		h = GetComponent<CompositeCollider2D>().bounds.extents.y * 2;
		w = GetComponent<CompositeCollider2D>().bounds.extents.x * 2;
		height = GetComponent<ObjectHeight>().height;
		topBound = position.y + (h / 2);
		bottomBound = position.y - (h / 2);
		rightBound = position.x + (w / 2);
		leftBound = position.x - (w / 2);
		sortingOrder = GetComponentInChildren<Renderer>().sortingOrder;
	}

	private void OnValidate() {
		ObjectPosition objectPosition = GetComponentInChildren<ObjectPosition>();
		if (objectPosition == null) {
			Debug.LogError("Component ObjectPosition could not be located. Please make sure to include the child object to make this script run correctly.", objectPosition);
		}
	}
}
