using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { 
	Down, 
	DownRight, 
	Right, 
	UpRight, 
	Up, 
	UpLeft, 
	Left, 
	DownLeft 
};

public class PlayerController : TopDownBehavior {

	[Header("Direction Variables")]
	public Direction facingDirection = Direction.Down;
	public bool isWalking;

	[Header("Physics Variables")]
	public float jumpSpeed = 30.0f;
	public float overworldSpeed = 0.35f;

	// Constants
	private const float boundCorrection = 0.5f;

	// Game Objects
	private Camera cam;
	private Animator animator;
	private BoxCollider2D collider;
	private Rigidbody2D rb2d;
	private ContactFilter2D baseContactFilter;
	private ContactFilter2D wallContactFilter;
	private ContactFilter2D platformContactFilter;
	private PlayerShadow shadow;
	private GameObject jumpHeight;
	private PlatformInfo currentPlatform;

	// Physics stuff
	private bool[] isDirectionBlocked = new bool[(int)Direction.DownLeft+1];
	private bool[] isPDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
	// Base physics
	private RaycastHit2D[] resultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] resultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] resultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] resultsLeft = new RaycastHit2D[3];
	// Platform physics
	private RaycastHit2D[] pResultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsLeft = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsUpRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsUpLeft = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDownRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDownLeft = new RaycastHit2D[3];

	private Collider2D[] wallChecks = new Collider2D[3];
	private Collider2D[] positionColliders = new Collider2D[2];
	private Direction fallingDirection;

	private float yOffset;
	private float yRadius;
	private float xRadius;
	private Vector3 groundPosition;
	private float groundHeight;
	private bool grounded = true;
	private bool jumping;
	private bool isFalling;
	private bool isOnPlatform;
	private bool landedOnCurrentPlatform;
	private bool leftCurrentPlatform;
	private float jumpingHeight;
	public float currentHeight;
	private float fallingHeight;
	private int totalHeight;

	// Coroutines
	private Coroutine jumpingRoutine;
	private Coroutine fallingRoutine;
	private Coroutine landingRoutine;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
		cam = Camera.main;
		collider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		//wallContactFilter = new ContactFilter2D();
		baseContactFilter.SetLayerMask(LayerMask.GetMask("Base"));
		wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
		platformContactFilter.SetLayerMask(LayerMask.GetMask("Platform"));
		//yOffset = Mathf.Abs(collider.offset.y);
		//yRadius = collider.size.y / 2;
		//xRadius = collider.size.x / 2;
		//Debug.Log(string.Format("coll offset: {0}",yOffset+yRadius));
		shadow = transform.parent.GetComponentInChildren<PlayerShadow>();
		jumpHeight = GameObject.FindGameObjectWithTag("JumpHeight");
	}

	void Update() {
		//Debug.Log("curr h: " + currentHeight);
		//Debug.Log("falling: " + isFalling);
		Debug.Log("shad pos: " + shadow.transform.position);

		// Let camera focus on player
		//Vector3 newCamPos;
		//if (grounded || (grounded && isWalking && facingDirection != Direction.Left 
		//&& facingDirection != Direction.Right)) {
		//if (grounded) {
		//	cam.transform.position = new Vector3((float)Math.Round(transform.position.x, 2)
		//	+ 0.005f, (float)Math.Round(transform.transform.position.y, 2)
		//		+ 0.005f, cam.transform.position.z);
		//} else if (!grounded && isWalking && facingDirection != Direction.Left && facingDirection != Direction.Right) {
		//	cam.transform.position = Vector3.MoveTowards(cam.transform.position,
		//		new Vector3((float)Math.Round(transform.position.x, 2)
		//		+ 0.005f, (float)Math.Round(transform.transform.position.y, 2), cam.transform.position.z),
		//		jumpSpeed * 2 * Time.deltaTime);
		//} else if (!grounded && !isWalking) {

		//} else {
		//	cam.transform.position = new Vector3((float)Math.Round(transform.position.x, 2) 
		//	+ 0.005f, cam.transform.position.y, cam.transform.position.z);
		//}
		if (!isFalling) {
			//if (!jumping) {
			//	cam.transform.position = new Vector3((float)Math.Round(shadow.transform.position.x, 2)
			//	+ 0.005f, (float)Math.Round(shadow.transform.transform.position.y + 2, 2)
			//		+ 0.005f, cam.transform.position.z);
			//} else {
				Debug.Log("Follow moving base.");
				cam.transform.position = Vector3.MoveTowards(cam.transform.position, new Vector3(shadow.transform.position.x,
				shadow.transform.transform.position.y + 2, cam.transform.position.z), jumpSpeed * Time.deltaTime);
			//}
		} else {
			cam.transform.position = Vector3.MoveTowards(cam.transform.position,
				new Vector3(transform.position.x, transform.transform.position.y, cam.transform.position.z),
				jumpSpeed * Time.deltaTime);
		}

		MovePlayer();
		groundPosition = shadow.transform.position + new Vector3(0, 2, 0);
		CheckPlatformCollision();
		//groundHeight = shadow.totalHeight;
		//Debug.Log("player pos: " + transform.position);
	}

	public float GetJumpingHeight() {
		return jumpingHeight;
	}

	public float GetCurrentHeight() {
		return currentHeight;
	}

	public float GetTotalHeight() {
		return totalHeight;
	}

	private bool CheckAgainstPlatformHeight(RaycastHit2D hit, bool higherThan) {
		Debug.Log(string.Format("plat height: {0}", hit.transform.GetComponent<ObjectHeight>().height));
		if (higherThan) {
			//Debug.Log("current jh: " + jumpingHeight);
			return currentHeight + jumpingHeight >= hit.transform.GetComponent<ObjectHeight>().height;
		}
		return currentHeight + jumpingHeight <= hit.transform.GetComponent<ObjectHeight>().height;
	}

	private bool CheckIfInPlatform() {
		bool inPlat = currentPlatform != null && shadow.transform.position.x >= currentPlatform.leftBound && shadow.transform.position.x <= currentPlatform.rightBound
			&& shadow.transform.position.y >= currentPlatform.bottomBound && shadow.transform.position.y <= currentPlatform.topBound;
		//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
		//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
		//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
		//inPlat));
		Debug.Log("in plat: " + inPlat);
		return inPlat;
	}

	private bool CheckIfInCollider(Vector3 targetPosition) {
		bool inPlat = currentPlatform != null && targetPosition.x >= currentPlatform.leftBound && targetPosition.x <= currentPlatform.rightBound
			&& targetPosition.y >= currentPlatform.bottomBound && targetPosition.y <= currentPlatform.topBound;
		//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
		//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
		//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
		//inPlat));
		Debug.Log("in plat: " + inPlat);
		return inPlat;
	}

	private void LowerHeight(float height) {
		currentHeight -= height;
	}

	IEnumerator RaisePlayerObjects() {
		Debug.Log("shadow bb4: " + shadow.transform.position);
		switch (fallingDirection) {
			case Direction.Up:
				shadow.transform.position += new Vector3(0, currentPlatform.height + (boundCorrection * 2), 0);
				groundPosition += new Vector3(0, currentPlatform.height + (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(0, currentPlatform.height + (boundCorrection * 2), 0);
				break;
			case Direction.Down:
				shadow.transform.position += new Vector3(0, currentPlatform.height - (boundCorrection * 2), 0);
				groundPosition += new Vector3(0, currentPlatform.height - (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(0, currentPlatform.height - (boundCorrection * 2), 0);
				break;
			case Direction.Left:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
				groundPosition += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
				break;
			case Direction.Right:
				shadow.transform.position += new Vector3(boundCorrection * 2, currentPlatform.height, 0);
				groundPosition += new Vector3(boundCorrection * 2, currentPlatform.height, 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, currentPlatform.height, 0);
				break;
			case Direction.UpLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height + (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), currentPlatform.height + (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height + (boundCorrection * 2), 0);
				break;
			case Direction.UpRight:
				shadow.transform.position += new Vector3(boundCorrection * 2, currentPlatform.height + (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, currentPlatform.height + (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, currentPlatform.height + (boundCorrection * 2), 0);
				break;
			case Direction.DownLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height - (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), currentPlatform.height - (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height - (boundCorrection * 2), 0);
				break;
			case Direction.DownRight:
				shadow.transform.position += new Vector3(boundCorrection * 2, currentPlatform.height - (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, currentPlatform.height - (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, currentPlatform.height - (boundCorrection * 2), 0);
				break;
		}
		Debug.Log("shadow after: " + shadow.transform.position);
	}

	IEnumerator Fall() {
		Debug.Log(string.Format("Falling {0}", fallingHeight));
		isFalling = true;
		//shadow.transform.position = new Vector3(shadow.transform.position.x, shadow.transform.position.y - fallingHeight,
		//shadow.transform.position.z);
		// Check which way to pad landing to prevent getting stuck
		switch (fallingDirection) {
			case Direction.Up:
				shadow.transform.position += new Vector3(0, -fallingHeight + (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(0, -fallingHeight + (boundCorrection * 2), 0);
				groundPosition += new Vector3(0, -fallingHeight + (boundCorrection * 2), 0);
				break;
			case Direction.Down:
				shadow.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
				groundPosition += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
				break;
			case Direction.Left:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
				break;
			case Direction.Right:
				shadow.transform.position += new Vector3(boundCorrection * 2, -fallingHeight, 0);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, -fallingHeight, 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight, 0);
				break;
			case Direction.UpLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
				break;
			case Direction.UpRight:
				shadow.transform.position += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
				break;
			case Direction.DownLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
				break;
			case Direction.DownRight:
				shadow.transform.position += new Vector3(boundCorrection * 2 -fallingHeight - (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
				break;

		}
		//Vector3 jumpHeightPos = jumpHeight.transform.position + new Vector3(0,-fallingHeight,0);
		//Vector3 newPlayerPos = new Vector3(shadow.transform.position.x, shadow.transform.position.y + 2,
			//shadow.transform.position.z);
		// Fall to the shadows position
		while (true) {
			transform.position = Vector3.MoveTowards(transform.position, groundPosition, jumpSpeed/1.5f * Time.deltaTime);
			if (transform.position == groundPosition) {
				Debug.Log("finished fall.");
				grounded = true;
				isFalling = false;
				if(fallingRoutine != null) {
					StopCoroutine(fallingRoutine);
				} else {
					break;
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
	}

	private void CheckPlatformCollision() {
		//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
		//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
		//resultsLeft[0].distance));
		switch (facingDirection) {
			case Direction.Up:
				if (rb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					currentPlatform = pResultsUp[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (CheckIfInCollider(transform.position) && isWalking && CheckAgainstPlatformHeight(pResultsUp[0], true) && !landedOnCurrentPlatform) {
						Debug.Log(string.Format("Inside {0}", currentPlatform.name));
						currentHeight = currentPlatform.height;
						isOnPlatform = true;
						leftCurrentPlatform = false;
						landingRoutine = StartCoroutine(RaisePlayerObjects());
						landedOnCurrentPlatform = true;
					}
				}
				if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
				//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					PlatformInfo leavingPlat = pResultsDown[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (!CheckIfInCollider(transform.position) && isWalking && CheckAgainstPlatformHeight(pResultsDown[0], true)
					&& leavingPlat == currentPlatform && landedOnCurrentPlatform) {
						Debug.Log(string.Format("Leaving {0}", currentPlatform.name));
						currentPlatform = null;
						isOnPlatform = false;
						fallingHeight = leavingPlat.height;
						if (!leftCurrentPlatform) {
							//Debug.Log("Leaving...");
							//isFalling = false;
							LowerHeight(fallingHeight);
							fallingDirection = Direction.Up;
							fallingRoutine = StartCoroutine(Fall());
							landedOnCurrentPlatform = false;
							leftCurrentPlatform = true;
						}
					}
				}
				break;
			//case Direction.Down:
			//	if (targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity) > 0) {
			//		// Block all downward ...
			//		if (Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
			//			isDirectionBlocked[(int)facingDirection] = true;
			//			isDirectionBlocked[(int)Direction.DownLeft] = true;
			//			isDirectionBlocked[(int)Direction.DownRight] = true;
			//		} else {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//	}
			//	break;
			//case Direction.Right:
			//	if (targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity) > 0) {
			//		// ...
			//		if (Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
			//			isDirectionBlocked[(int)facingDirection] = true;
			//			isDirectionBlocked[(int)Direction.UpRight] = true;
			//			isDirectionBlocked[(int)Direction.DownRight] = true;
			//		} else {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//	}
			//	break;
			//case Direction.Left:
			//	if (targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity) > 0) {
			//		// ...
			//		if (Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
			//			isDirectionBlocked[(int)facingDirection] = true;
			//			isDirectionBlocked[(int)Direction.UpLeft] = true;
			//			isDirectionBlocked[(int)Direction.DownLeft] = true;
			//		} else {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//	}
			//	break;
			//case Direction.UpRight:
			//	//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
			//	int upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
			//	int rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
			//	// ...
			//	if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
			//		isDirectionBlocked[(int)facingDirection] = true;
			//		isDirectionBlocked[(int)Direction.Up] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isDirectionBlocked[(int)Direction.Right]) {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isDirectionBlocked[(int)Direction.Up] = false;
			//	}
			//	if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
			//		isDirectionBlocked[(int)facingDirection] = true;
			//		isDirectionBlocked[(int)Direction.Right] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isDirectionBlocked[(int)Direction.Up]) {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isDirectionBlocked[(int)Direction.Right] = false;
			//	}
			//	// Correct position if hit a corner
			//	if (upHit > 00 && rightHit > 00 && Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon
			//	&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon
			//		&& currentHeight + jumpingHeight <= resultsUp[0].transform.GetComponent<ObjectHeight>().height
			//		&& currentHeight + jumpingHeight <= resultsRight[0].transform.GetComponent<ObjectHeight>().height) {
			//		Debug.Log("\t\tMoving down...");
			//		MoveInDirection(Direction.Down, 0.2f);
			//	}
			//	break;
			//case Direction.UpLeft:
			//	//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
			//	upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
			//	int leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
			//	// ...
			//	if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
			//		isDirectionBlocked[(int)facingDirection] = true;
			//		isDirectionBlocked[(int)Direction.Up] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isDirectionBlocked[(int)Direction.Left]) {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isDirectionBlocked[(int)Direction.Up] = false;
			//	}
			//	if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
			//		isDirectionBlocked[(int)facingDirection] = true;
			//		isDirectionBlocked[(int)Direction.Left] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isDirectionBlocked[(int)Direction.Up]) {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isDirectionBlocked[(int)Direction.Left] = false;
			//	}
			//	// Correct position if hit a corner
			//	if (upHit > 0 && leftHit > 0 && Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon
			//	&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon
			//		&& CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)
			//		&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
			//		Debug.Log("\t\tMoving down...");
			//		MoveInDirection(Direction.Down, 0.2f);
			//	}
			//	break;
			//case Direction.DownRight:
			//	//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
			//	int downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
			//	rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
			//	// ...
			//	if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
			//		isDirectionBlocked[(int)facingDirection] = true;
			//		isDirectionBlocked[(int)Direction.Down] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isDirectionBlocked[(int)Direction.Right]) {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isDirectionBlocked[(int)Direction.Down] = false;
			//	}
			//	if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
			//		isDirectionBlocked[(int)facingDirection] = true;
			//		isDirectionBlocked[(int)Direction.Right] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isDirectionBlocked[(int)Direction.Down]) {
			//			isDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isDirectionBlocked[(int)Direction.Right] = false;
			//	}
			//	// Correct position if hit a corner
			//	if (downHit > 0 && rightHit > 0 && Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
			//	&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon
			//		&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)
			//		&& CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
			//		Debug.Log("\t\tMoving up...");
			//		MoveInDirection(Direction.Up, 0.2f);
			//	}
			//	break;
			//case Direction.DownLeft:
				////rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
				//downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				//leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				//// ...
				//if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.Down] = true;
				//} else {
				//	// Only unblock the diagonal if not blocked my multiple things
				//	if (!isDirectionBlocked[(int)Direction.Left]) {
				//		isDirectionBlocked[(int)facingDirection] = false;
				//	}
				//	isDirectionBlocked[(int)Direction.Down] = false;
				//}
				//if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.Left] = true;
				//} else {
				//	// Only unblock the diagonal if not blocked my multiple things
				//	if (!isDirectionBlocked[(int)Direction.Down]) {
				//		isDirectionBlocked[(int)facingDirection] = false;
				//	}
				//	isDirectionBlocked[(int)Direction.Left] = false;
				//}
				//// Correct position if hit a corner
				//if (downHit > 0 && leftHit > 0 && Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
				//&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon
				//	&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)
				//	&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
				//	Debug.Log("\t\tMoving up...");
				//	MoveInDirection(Direction.Up, 0.2f);
				//}
				//break;
		}
	}



	private void ClearBlocks() {
		for(int i = 0; i < isDirectionBlocked.Length; i++) {
			isDirectionBlocked[i] = false;
		}
	}

	private void UpdateShadowPosition() {
		if(currentHeight > shadow.transform.position.y) {
			shadow.transform.Translate(transform.position);
		}
	}

	// Jumping routines
	private void InputJump() {
		if(Input.GetButtonDown("Jump") && !isFalling) {
			//groundHeight += 4;
			if(grounded) {
				//groundPosition = transform.position;
				jumping = true;
				jumpingRoutine = StartCoroutine(Jump());
			}
			if(transform.position == groundPosition) {
				grounded = true;
			} else {
				grounded = false;
			}
		}
	}

	IEnumerator Jump() {
		//while (true) {
		//	if (transform.position.y >= jumpHeight.transform.position.y) {
		//		jumping = false;
		//	}
		//	if (jumping) {
		//		//transform.Translate(Vector3.up * jumpSpeed * Time.smoothDeltaTime);
		//		//Vector3 maxHeight = new Vector3(groundPosition.x, groundPosition.y + maxJumpHeight, groundPosition.z);
		//		transform.position = Vector3.MoveTowards(transform.position, jumpHeight.transform.position,
		//				jumpSpeed * Time.deltaTime);
		//		// Calculate current height when jumping
		//		jumpingHeight = transform.position.y - groundPosition.y;
		//		//Debug.Log(string.Format("curr h: {0}", jumpingHeight));
		//	} else {
		//		//yield return new WaitForSeconds(0.01f);
		//		transform.position = Vector3.MoveTowards(transform.position, groundPosition,
		//			jumpSpeed * Time.deltaTime);
		//		jumpingHeight = transform.position.y - groundPosition.y;
		//		//Debug.Log(string.Format("curr h: {0}", jumpingHeight));
		//		if (transform.position == groundPosition) {
		//			grounded = true;
		//			StopCoroutine(jumpingRoutine);
		//		}
		//	}
		//	yield return new WaitForEndOfFrame();
		//}

		while (jumping) {
			if (transform.position.y >= jumpHeight.transform.position.y) {
				jumping = false;
			}
			if (jumping) {
				//transform.Translate(Vector3.up * jumpSpeed * Time.smoothDeltaTime);
				//Vector3 maxHeight = new Vector3(groundPosition.x, groundPosition.y + maxJumpHeight, groundPosition.z);
				transform.position = Vector3.MoveTowards(transform.position, jumpHeight.transform.position,
						jumpSpeed * Time.deltaTime);
				// Calculate current height when jumping
				jumpingHeight = transform.position.y - groundPosition.y;
				//Debug.Log(string.Format("curr h: {0}", jumpingHeight));
			}
			yield return new WaitForEndOfFrame();
		}
		//yield return new WaitForSeconds(0.05f);
		while (!jumping) {
			//yield return new WaitForSeconds(0.01f);
			transform.position = Vector3.MoveTowards(transform.position, groundPosition,
				jumpSpeed * Time.deltaTime);
			jumpingHeight = transform.position.y - groundPosition.y;
			//Debug.Log(string.Format("curr h: {0}", jumpingHeight));
			if (transform.position == groundPosition) {
				grounded = true;
				StopCoroutine(jumpingRoutine);
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private void MovePlayer() {
		InputJump();
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing up
			StartDirection(Direction.Up);
			if (!isDirectionBlocked[(int)Direction.Up]) { MoveInDirection(Direction.Up, overworldSpeed); }
			if (grounded) { 
				TryBlockDirections(rb2d, Vector2.up); 
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.up);
			}

		} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing down
			StartDirection(Direction.Down);
			if (!isDirectionBlocked[(int)Direction.Down]) { MoveInDirection(Direction.Down, overworldSpeed); }
			if (grounded) {
				TryBlockDirections(rb2d, Vector2.down);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.down);
			}
		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing right
			StartDirection(Direction.Right);
			if (!isDirectionBlocked[(int)Direction.Right]) { MoveInDirection(Direction.Right, overworldSpeed); }
			if (grounded) {
				TryBlockDirections(rb2d, Vector2.right);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.right);
			}
		} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing left
			// TODO:FIX ISSUE WHERE WAITING ON A WALL CAN CAUSE A DIRECTION TO BE BLOCKED
			StartDirection(Direction.Left);

			if (!isDirectionBlocked[(int)Direction.Left]) { MoveInDirection(Direction.Left, overworldSpeed); }

			if (grounded) {
				TryBlockDirections(rb2d, Vector2.left);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.left);
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing up-right
			StartDirection(Direction.UpRight);
			// TODO:
			if (!isDirectionBlocked[(int)Direction.UpRight]) {
				//transform.Translate(0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpRight, overworldSpeed*0.85f);
			} else if(!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Up, overworldSpeed);
			}

			if (grounded) {
				TryBlockDirections(rb2d, new Vector2(1, 1));
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(1,1));
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing up-left
			StartDirection(Direction.UpLeft);
			if (!isDirectionBlocked[(int)Direction.UpLeft]) {
				//transform.Translate(-0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpLeft, overworldSpeed*0.85f);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Up, overworldSpeed);
			}
			if (grounded) {
				TryBlockDirections(rb2d, new Vector2(-1, 1));
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(-1,1));
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing down-left
			StartDirection(Direction.DownLeft);
			if (!isDirectionBlocked[(int)Direction.DownLeft]) {
				//transform.Translate(-0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownLeft, overworldSpeed - 0.1f);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Down, overworldSpeed);
			}
			if (grounded) {
				TryBlockDirections(rb2d, new Vector2(-1, -1));
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(-1, -1));
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing down-right
			StartDirection(Direction.DownRight);
			if (!isDirectionBlocked[(int)Direction.DownRight]) { 
				//transform.Translate(0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownRight, overworldSpeed - 0.1f);
			} else if (!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Down, overworldSpeed);
			}
			if (grounded) {
				TryBlockDirections(rb2d, new Vector2(1, -1));
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(1,-1));
			}
		} else {
			animator.SetBool("isWalking", false);
			isWalking = false;
		}
	}

	 //TODO: handle case where more than half of collider is past the wall
	 		 //Need to correct this and move player diagonally until they are no longer blocked
	private void TryBlockDirections(Rigidbody2D targetRb2d, Vector2 direction) {
		//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
		//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
			//resultsLeft[0].distance));
		//Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
		switch (facingDirection) {
			case Direction.Up:
				if (targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					if (Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
						isDirectionBlocked[(int)Direction.UpRight] = true;
					} else {
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					}
				}
				break;
			// TODO: handle case when going down (behind a platform)
			case Direction.Down:
				if(targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity) > 0) {
					// Block all downward ...
					if (Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					} else {
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;
					}
				}
				break;
			case Direction.Right:
				if(targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity) > 0) {
					// ...
					if (Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.UpRight] = true;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					} else {
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;
					}
				}
				break;
			case Direction.Left:
				if (targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity) > 0) {
					// ...
					if (Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
					} else {
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					}
				}
				break;
			case Direction.UpRight:
				//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
				int upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
				int rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
				// ...
				if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Right]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Up] = false;
				}
				if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Right] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Up]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Right] = false;
					isDirectionBlocked[(int)Direction.Left] = false;
				}
				// Correct position if hit a corner
				if(upHit > 00 && rightHit > 00 && Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon 
				&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon 
					&& currentHeight + jumpingHeight <= resultsUp[0].transform.GetComponent<ObjectHeight>().height
					&& currentHeight + jumpingHeight <= resultsRight[0].transform.GetComponent<ObjectHeight>().height) {
					Debug.Log("\t\tMoving down...");
					MoveInDirection(Direction.Down, 0.2f);
				}
				break;
			case Direction.UpLeft:
				//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
				upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
				int leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				// ...
				if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Left]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Up] = false;
				}
				if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Left] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Up]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Right] = false;
					isDirectionBlocked[(int)Direction.Left] = false;
				}
				// Correct position if hit a corner
				if (upHit > 0 && leftHit > 0 && Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon
				&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon
					&& CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)
					&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
					Debug.Log("\t\tMoving down...");
					MoveInDirection(Direction.Down, 0.2f);
				}
				break;
			case Direction.DownRight:
				//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
				int downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
				// ...
				if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Right]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Down] = false;
				}
				if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Right] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Down]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Right] = false;
					isDirectionBlocked[(int)Direction.Left] = false;
				}
				// Correct position if hit a corner
				if (downHit > 0 && rightHit > 0 && Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
				&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon
					&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)
					&& CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
					Debug.Log("\t\tMoving up...");
					MoveInDirection(Direction.Up, 0.2f);
				}
				break;
			case Direction.DownLeft:
				//rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
				downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				// ...
				if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Left]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Down] = false;
				}
				if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Left] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Down]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Right] = false;
					isDirectionBlocked[(int)Direction.Left] = false;
				}
				// Correct position if hit a corner
				if (downHit > 0 && leftHit > 0 && Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
				&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon
					&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)
					&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
					Debug.Log("\t\tMoving up...");
					MoveInDirection(Direction.Up, 0.2f);
				}
				break;
		}
	}

	//private void TryBlockDirections(Rigidbody2D targetRb2d, Vector2 direction) {
	//	//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
	//	//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
	//	//resultsLeft[0].distance));
	//	//Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
	//	targetRb2d.GetAttachedColliders(positionColliders);
	//	switch (facingDirection) {
	//		case Direction.Up:
	//			if (targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity) > 0) {
	//			//positionColliders[0].offset
	//			//if (positionColliders[0].Raycast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity) > 0) {
	//			//if (Physics2D.Raycast(targetRb2d.transform.position, Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity) > 0) {
	//				// Block all upward directions to prevent sliding into walls
	//				if (Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
	//					isDirectionBlocked[(int)facingDirection] = true;
	//					isDirectionBlocked[(int)Direction.UpLeft] = true;
	//					isDirectionBlocked[(int)Direction.UpRight] = true;
	//				} else {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//					isDirectionBlocked[(int)Direction.UpLeft] = false;
	//					isDirectionBlocked[(int)Direction.UpRight] = false;
	//				}
	//			}
	//			break;
	//		// TODO: handle case when going down (behind a platform)
	//		case Direction.Down:
	//			if (targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity) > 0) {
	//				// Block all downward ...
	//				if (Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
	//					isDirectionBlocked[(int)facingDirection] = true;
	//					isDirectionBlocked[(int)Direction.DownLeft] = true;
	//					isDirectionBlocked[(int)Direction.DownRight] = true;
	//				} else {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//					isDirectionBlocked[(int)Direction.DownLeft] = false;
	//					isDirectionBlocked[(int)Direction.DownRight] = false;
	//				}
	//			}
	//			break;
	//		case Direction.Right:
	//			if (targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity) > 0) {
	//				// ...
	//				if (Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
	//					isDirectionBlocked[(int)facingDirection] = true;
	//					isDirectionBlocked[(int)Direction.UpRight] = true;
	//					isDirectionBlocked[(int)Direction.DownRight] = true;
	//				} else {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//					isDirectionBlocked[(int)Direction.UpRight] = false;
	//					isDirectionBlocked[(int)Direction.DownRight] = false;
	//				}
	//			}
	//			break;
	//		case Direction.Left:
	//			if (targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity) > 0) {
	//				// ...
	//				if (Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
	//					isDirectionBlocked[(int)facingDirection] = true;
	//					isDirectionBlocked[(int)Direction.UpLeft] = true;
	//					isDirectionBlocked[(int)Direction.DownLeft] = true;
	//				} else {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//					isDirectionBlocked[(int)Direction.UpLeft] = false;
	//					isDirectionBlocked[(int)Direction.DownLeft] = false;
	//				}
	//			}
	//			break;
	//		case Direction.UpRight:
	//			//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
	//			int upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
	//			int rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
	//			// ...
	//			if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Up] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Right]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Up] = false;
	//			}
	//			if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Right] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Up]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Right] = false;
	//				isDirectionBlocked[(int)Direction.Left] = false;
	//			}
	//			// Correct position if hit a corner
	//			if (upHit > 00 && rightHit > 00 && Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon
	//			&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon
	//				&& currentHeight + jumpingHeight <= resultsUp[0].transform.GetComponent<ObjectHeight>().height
	//				&& currentHeight + jumpingHeight <= resultsRight[0].transform.GetComponent<ObjectHeight>().height) {
	//				Debug.Log("\t\tMoving down...");
	//				MoveInDirection(Direction.Down, 0.2f);
	//			}
	//			break;
	//		case Direction.UpLeft:
	//			//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
	//			upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
	//			int leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
	//			// ...
	//			if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Up] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Left]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Up] = false;
	//			}
	//			if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Left] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Up]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Right] = false;
	//				isDirectionBlocked[(int)Direction.Left] = false;
	//			}
	//			// Correct position if hit a corner
	//			if (upHit > 0 && leftHit > 0 && Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon
	//			&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon
	//				&& CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight + jumpingHeight)
	//				&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
	//				Debug.Log("\t\tMoving down...");
	//				MoveInDirection(Direction.Down, 0.2f);
	//			}
	//			break;
	//		case Direction.DownRight:
	//			//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
	//			int downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
	//			rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
	//			// ...
	//			if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Down] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Right]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Down] = false;
	//			}
	//			if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Right] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Down]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Right] = false;
	//				isDirectionBlocked[(int)Direction.Left] = false;
	//			}
	//			// Correct position if hit a corner
	//			if (downHit > 0 && rightHit > 0 && Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
	//			&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon
	//				&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)
	//				&& CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight + jumpingHeight)) {
	//				Debug.Log("\t\tMoving up...");
	//				MoveInDirection(Direction.Up, 0.2f);
	//			}
	//			break;
	//		case Direction.DownLeft:
	//			//rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
	//			downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
	//			leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
	//			// ...
	//			if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Down] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Left]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Down] = false;
	//			}
	//			if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Left] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Down]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Right] = false;
	//				isDirectionBlocked[(int)Direction.Left] = false;
	//			}
	//			// Correct position if hit a corner
	//			if (downHit > 0 && leftHit > 0 && Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
	//			&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon
	//				&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight + jumpingHeight)
	//				&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight + jumpingHeight)) {
	//				Debug.Log("\t\tMoving up...");
	//				MoveInDirection(Direction.Up, 0.2f);
	//			}
	//			break;

	//	}
	//}

	private bool CheckIfBlockPlayerByHeight(RaycastHit2D hit, float height) {
		hit.transform.GetComponent<CompositeCollider2D>().OverlapCollider(wallContactFilter, wallChecks);
		Debug.Log(string.Format("coll: {0}", wallChecks[0].name));
		return height < hit.transform.GetComponent<ObjectHeight>().height
		+ wallChecks[0].GetComponent<ObjectHeight>().height && !isFalling;
	}

	private void MoveInDirection(Direction diretction, float speed) {
		switch(diretction) {
			case Direction.Up:
				transform.Translate(0, speed, 0);
				shadow.transform.Translate(0, speed, 0);
				jumpHeight.transform.Translate(0, speed, 0);
				break;
			case Direction.Down:
				transform.Translate(0, -speed, 0);
				shadow.transform.Translate(0, -speed, 0);
				jumpHeight.transform.Translate(0, -speed, 0);
				break;
			case Direction.Left:
				transform.Translate(-speed, 0, 0);
				shadow.transform.Translate(-speed, 0, 0);
				jumpHeight.transform.Translate(-speed, 0, 0);
				break;
			case Direction.Right:
				transform.Translate(speed, 0, 0);
				shadow.transform.Translate(speed, 0, 0);
				jumpHeight.transform.Translate(speed, 0, 0);
				break;
			case Direction.UpRight:
				transform.Translate(speed, speed, 0);
				shadow.transform.Translate(speed, speed, 0);
				jumpHeight.transform.Translate(speed, speed, 0);
				break;
			case Direction.UpLeft:
				transform.Translate(-speed, speed, 0);
				shadow.transform.Translate(-speed, speed, 0);
				jumpHeight.transform.Translate(-speed, speed, 0);
				break;
			case Direction.DownRight:
				transform.Translate(speed,-speed, 0);
				shadow.transform.Translate(speed, -speed, 0);
				jumpHeight.transform.Translate(speed, -speed, 0);
				break;
			case Direction.DownLeft:
				transform.Translate(-speed,-speed, 0);
				shadow.transform.Translate(-speed, -speed, 0);
				jumpHeight.transform.Translate(-speed, -speed, 0);
				break;
		}
	}

	private void StartDirection(Direction direction) {
		isWalking = true;
		facingDirection = direction;
		animator.SetBool("isWalking", true);
		animator.SetInteger("direction", (int)facingDirection);
		animator.SetTrigger("changeDirection");
	}
}

//private void MovePlayer() {
//	InputJump();
//	//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
//	if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
//		// Facing up
//		StartDirection(Direction.Up);
//		if (!isDirectionBlocked[(int)Direction.Up] && !isPDirectionBlocked[(int)Direction.Up]) { MoveInDirection(Direction.Up, overworldSpeed); }
//		if (grounded) {
//			TryBlockDirections(rb2d, Vector2.up);
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.up);
//		}

//	} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
//		// Facing down
//		StartDirection(Direction.Down);
//		if (!isDirectionBlocked[(int)Direction.Down] && !isPDirectionBlocked[(int)Direction.Down]) { MoveInDirection(Direction.Down, overworldSpeed); }
//		if (grounded) {
//			TryBlockDirections(rb2d, Vector2.down);
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.down);
//		}
//	} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
//		// Facing right
//		StartDirection(Direction.Right);

//		//if (!isDirectionBlocked[(int)Direction.Right]) {
//		//	if (!isPDirectionBlocked[(int)Direction.Right]) {
//		//		MoveInDirection(Direction.Right, overworldSpeed);
//		//	}
//		//}
//		//if (!isPDirectionBlocked[(int)Direction.Right]) {
//		//	if (!isDirectionBlocked[(int)Direction.Right]) {
//		//		MoveInDirection(Direction.Right, overworldSpeed);
//		//	}
//		//}
//		if (!isDirectionBlocked[(int)Direction.Right] && !isPDirectionBlocked[(int)Direction.Right]) { MoveInDirection(Direction.Right, overworldSpeed); }
//		if (grounded) {
//			TryBlockDirections(rb2d, Vector2.right);
//			TryBlockPlatformDirections(rb2d, Vector2.right);
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.right);
//		}
//	} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
//		// Facing left
//		// TODO:
//		StartDirection(Direction.Left);

//		if (!isDirectionBlocked[(int)Direction.Left] && !isPDirectionBlocked[(int)Direction.Left]) { MoveInDirection(Direction.Left, overworldSpeed); }
//		//if (!isDirectionBlocked[(int)Direction.Left]) {
//		//	if (!isPDirectionBlocked[(int)Direction.Left]) {
//		//		MoveInDirection(Direction.Left, overworldSpeed);
//		//	}
//		//}
//		//if (!isPDirectionBlocked[(int)Direction.Left]) {
//		//	if (!isDirectionBlocked[(int)Direction.Left]) {
//		//		MoveInDirection(Direction.Left, overworldSpeed);
//		//	}
//		//}

//		if (grounded) {
//			TryBlockDirections(rb2d, Vector2.left);
//			TryBlockPlatformDirections(rb2d, Vector2.left);
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.left);
//		}
//	} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
//		// Facing up-right
//		StartDirection(Direction.UpRight);
//		// TODO:
//		if (!isDirectionBlocked[(int)Direction.UpRight] && !isPDirectionBlocked[(int)Direction.UpRight]) {
//			//transform.Translate(0.35f, 0.35f, 0);
//			MoveInDirection(Direction.UpRight, overworldSpeed * 0.85f);
//		} else if ((!isDirectionBlocked[(int)Direction.Right] && !isPDirectionBlocked[(int)Direction.Right])
//		&& (isDirectionBlocked[(int)Direction.Up] && isPDirectionBlocked[(int)Direction.Up])) {
//			MoveInDirection(Direction.Right, overworldSpeed);
//		} else if (!isDirectionBlocked[(int)Direction.Up] && !isPDirectionBlocked[(int)Direction.Up]) {
//			MoveInDirection(Direction.Up, overworldSpeed);
//		}

//		if (grounded) {
//			TryBlockDirections(rb2d, new Vector2(1, 1));
//			TryBlockPlatformDirections(rb2d, new Vector2(1, 1));
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(1, 1));
//		}
//	} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
//		// Facing up-left
//		StartDirection(Direction.UpLeft);
//		if (!isDirectionBlocked[(int)Direction.UpLeft]) {
//			//transform.Translate(-0.35f, 0.35f, 0);
//			MoveInDirection(Direction.UpLeft, overworldSpeed * 0.85f);
//		} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up]) {
//			MoveInDirection(Direction.Left, overworldSpeed);
//		} else if (!isDirectionBlocked[(int)Direction.Up]) {
//			MoveInDirection(Direction.Up, overworldSpeed);
//		}
//		if (grounded) {
//			TryBlockDirections(rb2d, new Vector2(-1, 1));
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(-1, 1));
//		}
//	} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") < 0) {
//		// Facing down-left
//		StartDirection(Direction.DownLeft);
//		if (!isDirectionBlocked[(int)Direction.DownLeft]) {
//			//transform.Translate(-0.35f, -0.35f, 0);
//			MoveInDirection(Direction.DownLeft, overworldSpeed - 0.1f);
//		} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Down]) {
//			MoveInDirection(Direction.Left, overworldSpeed);
//		} else if (!isDirectionBlocked[(int)Direction.Down]) {
//			MoveInDirection(Direction.Down, overworldSpeed);
//		}
//		if (grounded) {
//			TryBlockDirections(rb2d, new Vector2(-1, -1));
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(-1, -1));
//		}
//	} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
//		// Facing down-right
//		StartDirection(Direction.DownRight);
//		if (!isDirectionBlocked[(int)Direction.DownRight]) {
//			//transform.Translate(0.35f, -0.35f, 0);
//			MoveInDirection(Direction.DownRight, overworldSpeed - 0.1f);
//		} else if (!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Down]) {
//			MoveInDirection(Direction.Right, overworldSpeed);
//		} else if (!isDirectionBlocked[(int)Direction.Down]) {
//			MoveInDirection(Direction.Down, overworldSpeed);
//		}
//		if (grounded) {
//			TryBlockDirections(rb2d, new Vector2(1, -1));
//		} else {
//			TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), new Vector2(1, -1));
//		}
//	} else {
//		animator.SetBool("isWalking", false);
//		isWalking = false;
//	}
//}

//private void CheckIfNextToPlatform() {
//	switch (facingDirection) {
//		case Direction.Up:
//			if (rb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0) {
//				// Block all upward directions to prevent sliding into walls
//				//Debug.Log(string.Format("thing: {0}", pResultsUp[0].distance));
//				if (Mathf.Abs(pResultsUp[0].distance) < 4.5f && !isOnPlatform && jumping) {
//					int platHeight = (int)pResultsUp[0].collider.gameObject.GetComponent<ObjectHeight>().height;
//					if (jumpingHeight + totalHeight >= platHeight) {
//						Debug.Log("Trying to jump on platform");
//						baseContactFilter.ClearLayerMask();
//						baseContactFilter.NoFilter();
//						ClearBlocks();
//						//totalHeight += platHeight;
//						//isOnPlatform = true;
//					}
//				} else if (Mathf.Abs(pResultsUp[0].distance) >= 4.5f && !isOnPlatform) {
//					Debug.Log("\t\t\tFalling...");
//					//wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
//				}
//			}
//			break;
//		// TODO: handle case when going down (behind a platform)
//		case Direction.Down:
//			rb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
//			break;
//		case Direction.Right:
//			rb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
//			break;
//		case Direction.Left:
//			rb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
//			break;
//		case Direction.UpRight:
//			//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
//			rb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
//			rb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
//			break;
//		case Direction.UpLeft:
//			//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
//			rb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
//			rb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
//			break;
//		case Direction.DownRight:
//			//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
//			rb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
//			rb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
//			break;
//		case Direction.DownLeft:
//			//rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
//			rb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
//			rb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
//			break;

//	}
//}

//private void OnCollisionStay2D(Collision2D coll) {
//	if (coll.gameObject.CompareTag("Platform")) {
//		currentPlatform = coll.gameObject.GetComponent<PlatformInfo>();
//		Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", coll.gameObject.name,
//		currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));

//		int platHeight = (int) coll.gameObject.GetComponent<ObjectHeight>().height;
//		if (!isOnPlatform && jumpingHeight + totalHeight >= platHeight) {
//			Debug.Log(string.Format("Jump onto {0}, h: {1}", coll.gameObject.name, platHeight));
//			//baseContactFilter.ClearLayerMask();
//			//baseContactFilter.NoFilter();
//			isOnPlatform = true;
//			totalHeight += platHeight;
//			//ClearBlocks();
//		}
//	}
//}

//private void OnCollisionExit2D(Collision2D coll) {
//	if (coll.gameObject.CompareTag("Platform")) {
//		Debug.Log(string.Format("Exiting platform {0}", coll.gameObject.name));
//		if (isOnPlatform) {
//			Debug.Log("\t\t\tExiting: Falling...");
//			//if (currentHeight + totalHeight > coll.gameObject.GetComponent<ObjectHeight>().height) {
//			//wallContactFilter.ClearLayerMask();
//			isFalling = true;
//			//totalHeight -= (int)coll.gameObject.GetComponent<ObjectHeight>().height;
//			//wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
//			//isOnPlatform = false;
//			//ClearBlocks();
//			//}
//		}
//	}
//}

//private void TryBlockPlatformDirections(Rigidbody2D targetRb2d, Vector2 direction) {
//	//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
//	//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
//	//resultsLeft[0].distance));
//	switch (facingDirection) {
//		case Direction.Up:
//			rb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity);
//			// Block all upward directions to prevent sliding into walls
//			//if (Mathf.Abs(resultsUp[0].distance) < boundCorrection) {
//			//	isDirectionBlocked[(int)facingDirection] = true;
//			//	isDirectionBlocked[(int)Direction.UpLeft] = true;
//			//	isDirectionBlocked[(int)Direction.UpRight] = true;
//			//} else {
//			//	isDirectionBlocked[(int)facingDirection] = false;
//			//}
//			break;
//		case Direction.Down:
//			rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
//			// Block all downward ...
//			//if (Mathf.Abs(resultsDown[0].distance) < boundCorrection) {
//			//	isDirectionBlocked[(int)facingDirection] = true;
//			//	isDirectionBlocked[(int)Direction.DownLeft] = true;
//			//	isDirectionBlocked[(int)Direction.DownRight] = true;
//			//} else {
//			//	isDirectionBlocked[(int)facingDirection] = false;
//			//}
//			break;
//		case Direction.Right:
//			targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity);
//			// ...
//			if (Mathf.Abs(pResultsRight[0].distance) < boundCorrection) {
//				isPDirectionBlocked[(int)facingDirection] = true;
//				isPDirectionBlocked[(int)Direction.UpRight] = true;
//				isPDirectionBlocked[(int)Direction.DownRight] = true;
//			} else {
//				isPDirectionBlocked[(int)facingDirection] = false;
//				//isPDirectionBlocked[(int)Direction.UpRight] = false;
//				//isPDirectionBlocked[(int)Direction.DownRight] = false;
//			}
//			//bool isBlocked = Mathf.Abs(pResultsRight[0].distance) < boundCorrection;
//			//isPDirectionBlocked[(int)facingDirection] = isBlocked;
//			//isPDirectionBlocked[(int)Direction.UpRight] = isBlocked;
//			//isPDirectionBlocked[(int)Direction.DownRight] = isBlocked;
//			break;
//		case Direction.Left:
//			//targetRb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity);
//			//// ...
//			//if (Mathf.Abs(pResultsLeft[0].distance) < boundCorrection) {
//			//	isPDirectionBlocked[(int)facingDirection] = true;
//			//	isPDirectionBlocked[(int)Direction.UpLeft] = true;
//			//	isPDirectionBlocked[(int)Direction.DownLeft] = true;
//			//} else {
//			//	isPDirectionBlocked[(int)facingDirection] = false;
//			//}
//			break;
//		case Direction.UpRight:
//			//int upRightHit = targetRb2d.Cast(new Vector2(1, 1), platformContactFilter, pr, Mathf.Infinity);
//			int upHit = targetRb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity);
//			int rightHit = targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity);
//			// ...
//			//if (targetRb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0 &&
//			//targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity) > 0) {
//			if (upHit > 0 && rightHit > 0) {
//				if (Mathf.Abs(pResultsUp[0].distance) < boundCorrection) {
//					isPDirectionBlocked[(int)facingDirection] = true;
//					isPDirectionBlocked[(int)Direction.Up] = true;
//				} else {
//					// Only unblock the diagonal if not blocked my multiple things
//					if (!isPDirectionBlocked[(int)Direction.Right]) {
//						isPDirectionBlocked[(int)facingDirection] = false;
//					}
//					isPDirectionBlocked[(int)Direction.Up] = false;
//				}
//				if (Mathf.Abs(pResultsRight[0].distance) < boundCorrection) {
//					isPDirectionBlocked[(int)facingDirection] = true;
//					isPDirectionBlocked[(int)Direction.Right] = true;
//				} else {
//					// Only unblock the diagonal if not blocked my multiple things
//					if (!isPDirectionBlocked[(int)Direction.Up]) {
//						isPDirectionBlocked[(int)facingDirection] = false;
//					}
//					isPDirectionBlocked[(int)Direction.Right] = false;
//				}
//				// Correct position if hit a corner
//				if (Mathf.Abs(pResultsUp[0].distance) < Mathf.Epsilon
//				&& Mathf.Abs(pResultsRight[0].distance) < Mathf.Epsilon) {
//					Debug.Log("\t\tMoving down...");
//					MoveInDirection(Direction.Down, 0.2f);
//				}
//			}
//			break;
//		//case Direction.UpLeft:
//		//	//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
//		//	targetRb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity);
//		//	targetRb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity);
//		//	// ...
//		//	if (Mathf.Abs(pResultsUp[0].distance) < boundCorrection) {
//		//		isPDirectionBlocked[(int)facingDirection] = true;
//		//		isPDirectionBlocked[(int)Direction.Up] = true;
//		//	} else {
//		//		// Only unblock the diagonal if not blocked my multiple things
//		//		if (!isPDirectionBlocked[(int)Direction.Left]) {
//		//			isPDirectionBlocked[(int)facingDirection] = false;
//		//		}
//		//		isPDirectionBlocked[(int)Direction.Up] = false;
//		//	}
//		//	if (Mathf.Abs(resultsLeft[0].distance) < boundCorrection) {
//		//		isPDirectionBlocked[(int)facingDirection] = true;
//		//		isPDirectionBlocked[(int)Direction.Left] = true;
//		//	} else {
//		//		// Only unblock the diagonal if not blocked my multiple things
//		//		if (!isPDirectionBlocked[(int)Direction.Up]) {
//		//			isPDirectionBlocked[(int)facingDirection] = false;
//		//		}
//		//		isPDirectionBlocked[(int)Direction.Left] = false;
//		//	}
//		//	// Correct position if hit a corner
//		//	if (Mathf.Abs(pResultsUp[0].distance) < Mathf.Epsilon
//		//	&& Mathf.Abs(pResultsLeft[0].distance) < Mathf.Epsilon) {
//		//		Debug.Log("\t\tMoving down...");
//		//		MoveInDirection(Direction.Down, 0.2f);
//		//	}
//		//	break;
//		//case Direction.DownRight:
//		//	//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
//		//	targetRb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
//		//	targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity);
//		//	// ...
//		//	if (Mathf.Abs(pResultsDown[0].distance) < boundCorrection) {
//		//		isPDirectionBlocked[(int)facingDirection] = true;
//		//		isPDirectionBlocked[(int)Direction.Down] = true;
//		//	} else {
//		//		// Only unblock the diagonal if not blocked my multiple things
//		//		if (!isPDirectionBlocked[(int)Direction.Right]) {
//		//			isPDirectionBlocked[(int)facingDirection] = false;
//		//		}
//		//		isPDirectionBlocked[(int)Direction.Down] = false;
//		//	}
//		//	if (Mathf.Abs(pResultsRight[0].distance) < boundCorrection) {
//		//		isPDirectionBlocked[(int)facingDirection] = true;
//		//		isPDirectionBlocked[(int)Direction.Right] = true;
//		//	} else {
//		//		// Only unblock the diagonal if not blocked my multiple things
//		//		if (!isPDirectionBlocked[(int)Direction.Down]) {
//		//			isPDirectionBlocked[(int)facingDirection] = false;
//		//		}
//		//		isPDirectionBlocked[(int)Direction.Right] = false;
//		//	}
//		//	// Correct position if hit a corner
//		//	if (Mathf.Abs(pResultsDown[0].distance) < Mathf.Epsilon
//		//	&& Mathf.Abs(pResultsRight[0].distance) < Mathf.Epsilon) {
//		//		Debug.Log("\t\tMoving up...");
//		//		MoveInDirection(Direction.Up, 0.2f);
//		//	}
//		//	break;
//		//case Direction.DownLeft:
//			////rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
//			//targetRb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
//			//targetRb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity);
//			//// ...
//			//if (Mathf.Abs(pResultsDown[0].distance) < boundCorrection) {
//			//	isPDirectionBlocked[(int)facingDirection] = true;
//			//	isPDirectionBlocked[(int)Direction.Down] = true;
//			//} else {
//			//	// Only unblock the diagonal if not blocked my multiple things
//			//	if (!isPDirectionBlocked[(int)Direction.Left]) {
//			//		isPDirectionBlocked[(int)facingDirection] = false;
//			//	}
//			//	isPDirectionBlocked[(int)Direction.Down] = false;
//			//}
//			//if (Mathf.Abs(pResultsLeft[0].distance) < boundCorrection) {
//			//	isPDirectionBlocked[(int)facingDirection] = true;
//			//	isPDirectionBlocked[(int)Direction.Left] = true;
//			//} else {
//			//	// Only unblock the diagonal if not blocked my multiple things
//			//	if (!isPDirectionBlocked[(int)Direction.Down]) {
//			//		isPDirectionBlocked[(int)facingDirection] = false;
//			//	}
//			//	isPDirectionBlocked[(int)Direction.Left] = false;
//			//}
//			//// Correct position if hit a corner
//			//if (Mathf.Abs(pResultsDown[0].distance) < Mathf.Epsilon
//			//&& Mathf.Abs(pResultsLeft[0].distance) < Mathf.Epsilon) {
//			//	Debug.Log("\t\tMoving up...");
//			//	MoveInDirection(Direction.Up, 0.2f);
//			//}
//			//break;

//	}
//}
