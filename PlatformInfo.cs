using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformInfo : MonoBehaviour {

	public Vector3 position;
	public float height;
	public float width;
	public float topBound;
	public float bottomBound;
	public float rightBound;
	public float leftBound;
	public int sortingOrder;

	private void OnEnable() {
		position = GetComponentInChildren<ObjectPosition>().transform.position;
		height = GetComponent<CompositeCollider2D>().bounds.extents.y * 2;
		width = GetComponent<CompositeCollider2D>().bounds.extents.x * 2;
		topBound = position.y + (height / 2);
		bottomBound = position.y - (height / 2);
		rightBound = position.x + (width / 2);
		leftBound = position.x - (width / 2);
		sortingOrder = GetComponentInChildren<Renderer>().sortingOrder;
	}
}
