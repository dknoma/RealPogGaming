using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { 
	NULL,
	Down, 
	DownRight, 
	Right, 
	UpRight, 
	Up, 
	UpLeft, 
	Left, 
	DownLeft 
};

// TODOLIST: Handle collision when unit wasn't able to make it over the top
//				if falling and is inside the collider, push them back in the other direction
//			 Fix jumping from the top while facing down
public class PlayerController : TopDownBehavior {

	[Header("Direction Variables")]
	public Direction facingDirection = Direction.Down;
	public bool isWalking;

	[Header("Physics Variables")]
	public float jumpSpeed = 30.0f;
	public float overworldSpeed = 0.35f;
	public int baseMask; // base filter
	public int platformMask;

	// Constants
	private const float boundCorrection = 2f;

	// Game Objects
	private Camera cam;
	private Animator animator;
	private BoxCollider2D myCollider;
	private Rigidbody2D rb2d;
	private ContactFilter2D baseContactFilter;
	private ContactFilter2D wallContactFilter;
	private ContactFilter2D platformContactFilter;
	private ContactFilter2D floorContactFilter;
	private PlayerShadow shadow;
	private GameObject jumpHeight;
	private PlatformInfo currentPlatformCheck;
	private PlatformInfo currentPlatform;
	private PlatformInfo nextPlatform;

	// Physics stuff
	private bool[] isDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
	private bool[] isPDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
	// Base physics
	private RaycastHit2D[] resultsUp;
	private RaycastHit2D[] resultsDown;
	private RaycastHit2D[] resultsRight;
	private RaycastHit2D[] resultsLeft;
	private RaycastHit2D[] resultsUpRight;
	private RaycastHit2D[] resultsUpLeft;
	private RaycastHit2D[] resultsDownRight;
	private RaycastHit2D[] resultsDownLeft;
	//private RaycastHit2D[] resultsLeft = new RaycastHit2D[3];
	// Platform physics
	private RaycastHit2D[] pResultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsLeft;
	private RaycastHit2D[] pResultsUpRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsUpLeft = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDownRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDownLeft = new RaycastHit2D[3];

	private RaycastHit2D[] floorHits = new RaycastHit2D[3];
	private RaycastHit2D[] wallHits = new RaycastHit2D[3];

	private RaycastHit2D currentLeftBase;
	private RaycastHit2D nextLeftBase;
	private float minLeftBaseDistance;
	private RaycastHit2D currentRightBase;
	private RaycastHit2D nextRightBase;
	private float minRightBaseDistance;
	private RaycastHit2D currentUpBase;
	private RaycastHit2D nextUpBase;
	private float minUpBaseDistance;
	private RaycastHit2D currentDownBase;
	private RaycastHit2D nextDownBase;
	private float minDownBaseDistance;

	private RaycastHit2D currentUpLeftBase;
	private RaycastHit2D nextUpLeftBase;
	private float minUpLeftBaseDistance;
	private RaycastHit2D currentUpRightBase;
	private RaycastHit2D nextUpRightBase;
	private float minUpRightBaseDistance;
	private RaycastHit2D currentDownLeftBase;
	private RaycastHit2D nextDownLeftBase;
	private float minDownLeftBaseDistance;
	private RaycastHit2D currentDownRightBase;
	private RaycastHit2D nextDownRightBase;
	private float minDownRightBaseDistance;

	private Collider2D[] wallChecks = new Collider2D[3];
	private Collider2D[] positionColliders = new Collider2D[2];
	private Direction fallingDirection;

	private float yOffset;
	private float yRadius;
	private float xRadius;
	private Vector3 groundPosition;
	private float groundHeight;
	private bool grounded = true;
	private bool rising;
	private bool jumping;
	private bool isFalling;
	private bool isOnPlatform;
	private bool landedOnCurrentPlatform;
	private bool leftCurrentPlatform;
	private float jumpingHeight;
	public float currentHeight;
	private float fallingHeight;
	private int totalHeight;
	private float diagonalMovementSpeed;
	private float diagonalBoundCorrection;

	// Coroutines
	private Coroutine jumpingRoutine;
	private Coroutine fallingRoutine;
	private Coroutine landingRoutine;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
		cam = Camera.main;
		myCollider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		//wallContactFilter = new ContactFilter2D();
		baseMask = LayerMask.GetMask("Base");
		platformMask = LayerMask.GetMask("Platform");
		baseContactFilter.SetLayerMask(baseMask);
		wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
		platformContactFilter.SetLayerMask(platformMask);
		floorContactFilter.SetLayerMask(platformMask);
		floorContactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
		//yOffset = Mathf.Abs(collider.offset.y);
		//yRadius = collider.size.y / 2;
		//xRadius = collider.size.x / 2;
		//Debug.Log(string.Format("coll offset: {0}",yOffset+yRadius));
		shadow = transform.parent.GetComponentInChildren<PlayerShadow>();
		jumpHeight = GameObject.FindGameObjectWithTag("JumpHeight");
		diagonalMovementSpeed = Mathf.Sqrt(overworldSpeed * overworldSpeed / 2);
		diagonalBoundCorrection = Mathf.Sqrt(2 * (boundCorrection*boundCorrection));
		Debug.Log("diag bound: " + diagonalBoundCorrection);
	}

	void Update() {
		//Debug.Log("curr h: " + currentHeight);
		//Debug.Log("fall direction: " + fallingDirection);
		//Debug.Log("shad pos: " + shadow.transform.position);

		if (!isFalling) {
			//if (!landedOnCurrentPlatform) {
			//	Debug.Log("not falling, not on plat");
			//	cam.transform.position = new Vector3((float)Math.Round(shadow.transform.position.x, 2)
			//	+ 0.005f, (float)Math.Round(shadow.transform.position.y, 2)
			//		+ 0.005f, cam.transform.position.z);
			//} else {
			//Debug.Log("not falling, on plat");
			cam.transform.position = Vector3.MoveTowards(cam.transform.position,
				new Vector3(shadow.transform.position.x, shadow.transform.position.y, cam.transform.position.z),
				jumpSpeed * Time.deltaTime);
			//}
			//cam.transform.position = Vector3.MoveTowards(cam.transform.position, shadow.transform.position, jumpSpeed * Time.deltaTime);
			//} else {
			//Debug.Log("Follow moving base.");
			//cam.transform.position = Vector3.MoveTowards(cam.transform.position, new Vector3(shadow.transform.position.x,
			//shadow.transform.transform.position.y + 2, cam.transform.position.z), jumpSpeed * Time.deltaTime);
			//}
		} else {
			//if (!jumping) {
			if (fallingHeight < 8) {
				//Debug.Log("falling, not jumping");
				//cam.transform.position = new Vector3((float)Math.Round(shadow.transform.position.x, 2)
				//+ 0.005f, (float)Math.Round(shadow.transform.position.y, 2)
				//+ 0.005f, cam.transform.position.z);
				cam.transform.position = Vector3.MoveTowards(cam.transform.position,
					new Vector3(shadow.transform.position.x, shadow.transform.position.y, cam.transform.position.z),
					jumpSpeed * 2 * Time.deltaTime);
			} else {
				//Debug.Log("falling, jumping");
				cam.transform.position = Vector3.MoveTowards(cam.transform.position,
					new Vector3(transform.position.x, transform.position.y, cam.transform.position.z),
					jumpSpeed * Time.deltaTime);
			}
		}

		Jump();
		MovePlayer();
		//groundPosition = shadow.transform.position + new Vector3(0, 2, 0);
		groundPosition = shadow.transform.position;
		//CheckPlatformCollision();
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

	private bool CheckAgainstPlatformHeight(PlatformInfo platform, bool higherThan) {
		//Debug.Log(string.Format("plat height: {0}", hit.transform.GetComponent<ObjectHeight>().height));
		if (higherThan) {
			//Debug.Log("current jh: " + jumpingHeight);
			return currentHeight >= platform.height;
		}
		return currentHeight <= platform.height;
	}

	private bool CheckIfInPlatform() {
		bool inPlat = currentPlatformCheck != null && shadow.transform.position.x >= currentPlatformCheck.leftBound && shadow.transform.position.x <= currentPlatformCheck.rightBound
			&& shadow.transform.position.y >= currentPlatformCheck.bottomBound && shadow.transform.position.y <= currentPlatformCheck.topBound;
		//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
		//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
		//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
		//inPlat));
		//Debug.Log("in plat: " + inPlat);
		return inPlat;
	}

	//private bool CheckIfInNewPlatform() {
	//	bool inPlat = currentPlatformCheck != null && shadow.transform.position.x >= currentPlatformCheck.leftBound && shadow.transform.position.x <= currentPlatformCheck.rightBound
	//		&& shadow.transform.position.y >= currentPlatformCheck.bottomBound && shadow.transform.position.y <= currentPlatformCheck.topBound;
	//	//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
	//	//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
	//	//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
	//	//inPlat));
	//	//Debug.Log("in plat: " + inPlat);
	//	return inPlat;
	//}

	private bool CheckIfInCollider(Vector3 myPosition, PlatformInfo platform) {
		bool inPlat = platform != null && myPosition.x >= platform.leftBound && myPosition.x <= platform.rightBound
			&& myPosition.y >= platform.bottomBound && myPosition.y <= platform.topBound;
		//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
		//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
		//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
		//inPlat));
		//Debug.Log("in plat: " + inPlat);
		return inPlat;
	}

	private void LowerHeight(float height) {
		//currentHeight -= height;
	}

	//private Vector3 RaiseObject(float x, float y, float z) {
	//	return new Vector3(x, y, z);
	//}

	IEnumerator RaisePlayerObjects() {
		//Debug.Log("shadow bb4: " + shadow.transform.position);
		if (isOnPlatform) {
			Debug.Log("Raising objects...");
			Debug.Log("grounded: " + grounded);
			switch (facingDirection) {
				case Direction.Up:
					//shadow.transform.position += new Vector3(0, currentPlatform.height + 4 + (boundCorrection * 2), 0);
					shadow.transform.position += new Vector3(0, currentPlatformCheck.height + (boundCorrection * 2), 0);
					groundPosition += new Vector3(0, currentPlatformCheck.height + (boundCorrection * 2), 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(0, currentPlatformCheck.height + (boundCorrection * 2), 0);
					break;
				case Direction.Down:
					PlatformInfo platformInfo = pResultsDown[0].transform.GetComponent<PlatformInfo>();
					Debug.Log("top bound: " + platformInfo.topBound);

					shadow.transform.position = new Vector3(shadow.transform.position.x,
						platformInfo.topBound - (boundCorrection * 2), shadow.transform.position.z);
					groundPosition = new Vector3(shadow.transform.position.x,
						platformInfo.topBound - (boundCorrection * 2), shadow.transform.position.z);
					yield return new WaitUntil(() => grounded);
					Debug.Log("Finished raising when facing down");
					jumpHeight.transform.position = new Vector3(jumpHeight.transform.position.x,
						platformInfo.topBound - (boundCorrection * 2), jumpHeight.transform.position.z);
					break;
				case Direction.Left:
					shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
					groundPosition += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
					break;
				case Direction.Right:
					shadow.transform.position += new Vector3(boundCorrection * 2, currentPlatformCheck.height, 0);
					groundPosition += new Vector3(boundCorrection * 2, currentPlatformCheck.height, 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(boundCorrection * 2, currentPlatformCheck.height, 0);
					break;
				case Direction.UpLeft:
					shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatformCheck.height + (boundCorrection * 2), 0);
					groundPosition += new Vector3(-(boundCorrection * 2), currentPlatformCheck.height + (boundCorrection * 2), 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatformCheck.height + (boundCorrection * 2), 0);
					break;
				case Direction.UpRight:
					shadow.transform.position += new Vector3(boundCorrection * 2, currentPlatformCheck.height + (boundCorrection * 2), 0);
					groundPosition += new Vector3(boundCorrection * 2, currentPlatformCheck.height + (boundCorrection * 2), 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(boundCorrection * 2, currentPlatformCheck.height + (boundCorrection * 2), 0);
					break;
				case Direction.DownLeft:
					shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatformCheck.height - (boundCorrection * 2), 0);
					groundPosition += new Vector3(-(boundCorrection * 2), currentPlatformCheck.height - (boundCorrection * 2), 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatformCheck.height - (boundCorrection * 2), 0);
					break;
				case Direction.DownRight:
					shadow.transform.position += new Vector3(boundCorrection * 2, currentPlatformCheck.height - (boundCorrection * 2), 0);
					groundPosition += new Vector3(boundCorrection * 2, currentPlatformCheck.height - (boundCorrection * 2), 0);
					yield return new WaitUntil(() => grounded);
					jumpHeight.transform.position += new Vector3(boundCorrection * 2, currentPlatformCheck.height - (boundCorrection * 2), 0);
					break;
			}
		}
		landedOnCurrentPlatform = false;
		//Debug.Log("shadow after: " + shadow.transform.position);
	}

	// Check the height of the floor that the player is trying to fall down to
	private void CheckForLowerPlatforms() {
		if (shadow.GetComponent<Collider2D>().Raycast(Vector2.down, floorContactFilter, floorHits, Mathf.Infinity) > 0) {
			fallingHeight = fallingHeight - floorHits[0].transform.GetComponent<ObjectHeight>().height;
		}
	}

	IEnumerator Fall() {
		//Debug.Log(string.Format("Falling {0}h", fallingHeight));
		currentPlatformCheck = null;
		CheckForLowerPlatforms();
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
				if (!jumping) {
					//Debug.Log("shadow bb4: " + shadow.transform.position);
					shadow.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
					//Debug.Log("shadow after: " + shadow.transform.position);
					jumpHeight.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
					groundPosition += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
				}
				//Debug.Log("Jumping:\tshadow pos: " + shadow.transform.position);
				//shadow.transform.position += new Vector3(0, currentHeight-fallingHeight - (boundCorrection * 2), 0);
				//Debug.Log("\tshadow after: " + shadow.transform.position);
				//groundPosition += new Vector3(0, currentHeight-fallingHeight - (boundCorrection * 2), 0);
				//yield return new WaitUntil(() => grounded);
				//jumpHeight.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
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
				shadow.transform.position += new Vector3(boundCorrection * 2 - fallingHeight - (boundCorrection * 2), 0);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
				break;
		}
		//Vector3 jumpHeightPos = jumpHeight.transform.position + new Vector3(0,-fallingHeight,0);
		//Vector3 newPlayerPos = new Vector3(shadow.transform.position.x, shadow.transform.position.y + 2,
		//shadow.transform.position.z);
		//Fall to the shadows position

		//Debug.Log("perparing for fall");
		float beforeFallHeight = currentHeight;
		while (true) {
			transform.position = Vector3.MoveTowards(transform.position, groundPosition, jumpSpeed * Time.deltaTime);
			//Mathf.Lerp(currentHeight, currentHeight - fallingHeight, jumpSpeed / 1.5f * Time.deltaTime);
			currentHeight = beforeFallHeight - (beforeFallHeight - (transform.position.y - groundPosition.y));
			//+ (currentPlatform != null ? currentPlatform.height : 0);
			if (transform.position == groundPosition) {
				currentHeight = beforeFallHeight - (beforeFallHeight - (transform.position.y - groundPosition.y));
				//Debug.Log("finished fall.");
				grounded = true;
				isFalling = false;
				if (fallingRoutine != null) {
					fallingDirection = new Direction();
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
				// Check if want to go ontop of a platform when facing up
				if (rb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					currentPlatformCheck = pResultsUp[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true) && !landedOnCurrentPlatform) {
						Debug.Log(string.Format("Inside {0}", currentPlatformCheck.name));
						currentPlatform = currentPlatformCheck;
						landedOnCurrentPlatform = true;
						//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
						//StopCoroutine(jumpingRoutine);
						//currentHeight = currentPlatform.height;
						//grounded = true;
						//isFalling = false;
						//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
						Debug.Log("c h: " + currentHeight);
						isOnPlatform = true;
						leftCurrentPlatform = false;
						landingRoutine = StartCoroutine(RaisePlayerObjects());
					}
				}
				// Check for falling when facing up
				if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
					//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					PlatformInfo leavingPlat = pResultsDown[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (!CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true)
					&& leavingPlat == currentPlatformCheck && landedOnCurrentPlatform) {
						Debug.Log(string.Format("Leaving {0} while facing {1}", currentPlatformCheck.name, facingDirection));
						//currentPlatform = null;
						isOnPlatform = false;
						fallingHeight = leavingPlat.height;
						if (!leftCurrentPlatform) {
							landedOnCurrentPlatform = false;
							leftCurrentPlatform = true;
							//Debug.Log("Leaving...");
							//LowerHeight(fallingHeight);
							fallingDirection = Direction.Up;
							fallingRoutine = StartCoroutine(Fall());
						}
					}
				}
				break;
			//TODO: Facing down
			case Direction.Down:
				// Check if want to go ontop of a platform when facing down
				if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
					//Debug.Log(string.Format("hit coll here: {0}", pResultsDown[0].point));
					// Block all upward directions to prevent sliding into walls
					currentPlatformCheck = pResultsDown[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (CheckIfInCollider(shadow.transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true) && !landedOnCurrentPlatform) {
						Debug.Log(string.Format("Down: Inside {0}", currentPlatformCheck.name));
						//currentHeight += currentPlatform.height;
						currentPlatform = currentPlatformCheck;
						landedOnCurrentPlatform = true;
						isOnPlatform = true;
						leftCurrentPlatform = false;
						landingRoutine = StartCoroutine(RaisePlayerObjects());
					}
				}
				// Check for falling when facing down
				if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0) {
					//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					PlatformInfo leavingPlat = pResultsUp[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (!CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true)
						&& leavingPlat == currentPlatformCheck && landedOnCurrentPlatform) {
						Debug.Log(string.Format("Leaving {0} while facing {1}", currentPlatformCheck.name, facingDirection));
						//currentPlatform = null;
						isOnPlatform = false;
						fallingHeight = leavingPlat.height;
						if (!leftCurrentPlatform) {
							isFalling = true;
							grounded = false;
							landedOnCurrentPlatform = false;
							leftCurrentPlatform = true;
							//Debug.Log("Leaving...");
							//LowerHeight(fallingHeight);
							fallingDirection = Direction.Down;
							fallingRoutine = StartCoroutine(Fall());
						}
					}
				}
				break;
			case Direction.Right:
				if (rb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					currentPlatformCheck = pResultsRight[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true) && !landedOnCurrentPlatform) {
						Debug.Log(string.Format("Facing right - Inside {0}", currentPlatformCheck.name));
						currentPlatform = currentPlatformCheck;
						landedOnCurrentPlatform = true;
						//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
						//StopCoroutine(jumpingRoutine);
						//currentHeight = currentPlatform.height;
						//grounded = true;
						//isFalling = false;
						//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
						Debug.Log("c h: " + currentHeight);
						isOnPlatform = true;
						leftCurrentPlatform = false;
						landingRoutine = StartCoroutine(RaisePlayerObjects());
					}
				}
				// Check for falling when facing up
				if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity) > 0) {
					//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					PlatformInfo leavingPlat = pResultsLeft[0].transform.GetComponent<PlatformInfo>();
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (!CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true)
					&& leavingPlat == currentPlatformCheck && landedOnCurrentPlatform) {
						Debug.Log(string.Format("Leaving {0} while facing {1}", currentPlatformCheck.name, facingDirection));
						//currentPlatform = null;
						isOnPlatform = false;
						fallingHeight = leavingPlat.height;
						if (!leftCurrentPlatform) {
							landedOnCurrentPlatform = false;
							leftCurrentPlatform = true;
							//Debug.Log("Leaving...");
							//LowerHeight(fallingHeight);
							fallingDirection = Direction.Right;
							fallingRoutine = StartCoroutine(Fall());
						}
					}
				}
				break;
			case Direction.Left:
				pResultsLeft = Physics2D.RaycastAll(rb2d.transform.position, Vector2.left, Mathf.Infinity, platformMask);
				if (pResultsLeft.Length > 0) {
					//if (rb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity) > 0) {
					// Block all upward directions to prevent sliding into walls
					//currentPlatformCheck = pResultsLeft[0].transform.GetComponent<PlatformInfo>();
					//nextPlatform = pResultsLeft.Length > 1 ? pResultsLeft[1].transform.GetComponent<PlatformInfo>() : null;
					for (int i = 0; i < pResultsLeft.Length; i++) {
						PlatformInfo checkPlat = pResultsLeft[i].transform.GetComponent<PlatformInfo>();
						//Debug.Log("check name: " + checkPlat.name);
						// If check is null
						//if(currentPlatform == null) {
						//	currentPlatform = checkPlat;
						//} else {
						//	nextPlatform = !checkPlat.name.Equals(currentPlatform.name) ? checkPlat : null;
						//}
						if (currentPlatform != null) {
							if (checkPlat != currentPlatform) {
								nextPlatform = checkPlat;
								break;
							}
							//nextPlatform = !checkPlat.name.Equals(currentPlatform.name) ? checkPlat : nextPlatform;
						} else {
							nextPlatform = checkPlat;
							break;
						}
						//nextPlatform = currentPlatform != null ?
						//(!checkPlat.name.Equals(currentPlatform.name) ? checkPlat : null) 
						//: checkPlat;
					}
					if (pResultsLeft.Length == 1) {
						nextPlatform = null;
					}
					//Debug.Log("[0]: " + pResultsLeft[0].distance + ", [1]: " + (pResultsLeft.Length > 1 ? pResultsLeft[1].distance : -1));
					//Debug.Log(string.Format("curr: {0}, next: {1}", currentPlatform != null ? currentPlatform.name : "No current plat", nextPlatform != null ? nextPlatform.name : "No next plat"));
					//if (CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true) 
					//&& !landedOnCurrentPlatform && !isOnPlatform) {
					//Debug.Log(string.Format("Facing left - Inside {0}", currentPlatformCheck.name));
					if (nextPlatform != null && CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true)
						&& jumping) {
						//&& !landedOnCurrentPlatform && !isOnPlatform) {
						Debug.Log(string.Format("Facing left - Inside {0}", nextPlatform.name));
						currentPlatform = nextPlatform;
						landedOnCurrentPlatform = true;
						//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
						//StopCoroutine(jumpingRoutine);
						//currentHeight = currentPlatform.height;
						//grounded = true;
						//isFalling = false;
						//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
						Debug.Log("c h[0]: " + currentHeight);
						isOnPlatform = true;
						leftCurrentPlatform = false;
						landingRoutine = StartCoroutine(RaisePlayerObjects());
					}
					//else if (CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(currentPlatformCheck, true)
					//	&& jumping && isOnPlatform) {
					//	currentPlatform = nextPlatform;
					//	Debug.Log(string.Format("Facing left - Inside next plat: {0}\n{1}", nextPlatform.name, nextPlatform.height));
					//	landedOnCurrentPlatform = true;
					//	//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
					//	//StopCoroutine(jumpingRoutine);
					//	//currentHeight = currentPlatform.height;
					//	//grounded = true;
					//	//isFalling = false;
					//	//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
					//	Debug.Log("c h[1]: " + currentHeight);
					//	isOnPlatform = true;
					//	leftCurrentPlatform = false;
					//	landingRoutine = StartCoroutine(RaisePlayerObjects());
					//}
				}
				// Check for falling when facing left
				//if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.left, platformContactFilter, pResultsRight, Mathf.Infinity) > 0) {
				//	//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
				//	// Block all upward directions to prevent sliding into walls
				//	PlatformInfo leavingPlat = pResultsRight[0].transform.GetComponent<PlatformInfo>();
				//	//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
				//	//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
				//	if (!CheckIfInCollider(transform.position, currentPlatformCheck) && isWalking && CheckAgainstPlatformHeight(pResultsRight[0], true)
				//	&& leavingPlat == currentPlatformCheck && landedOnCurrentPlatform) {
				//		Debug.Log(string.Format("Leaving {0} while facing {1}", currentPlatformCheck.name, facingDirection));
				//		//currentPlatform = null;
				//		isOnPlatform = false;
				//		fallingHeight = leavingPlat.height;
				//		if (!leftCurrentPlatform) {
				//			landedOnCurrentPlatform = false;
				//			leftCurrentPlatform = true;
				//			//Debug.Log("Leaving...");
				//			//LowerHeight(fallingHeight);
				//			fallingDirection = Direction.Left;
				//			fallingRoutine = StartCoroutine(Fall());
				//		}
				//	}
				//}
				break;
				//case Direction.UpRight:
				//	//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
				//	int upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
				//	int rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
				//	// ...
				//	if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)) {
				//		isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.Up] = true;
				//	} else {
				//		// Only unblock the diagonal if not blocked my multiple things
				//		if (!isDirectionBlocked[(int)Direction.Right]) {
				//			isDirectionBlocked[(int)facingDirection] = false;
				//		}
				//		isDirectionBlocked[(int)Direction.Up] = false;
				//	}
				//	if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
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
				//		&& currentHeight <= resultsUp[0].transform.GetComponent<ObjectHeight>().height
				//		&& currentHeight <= resultsRight[0].transform.GetComponent<ObjectHeight>().height) {
				//		Debug.Log("\t\tMoving down...");
				//		MoveInDirection(Direction.Down, 0.2f);
				//	}
				//	break;
				//case Direction.UpLeft:
				//	//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
				//	upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
				//	int leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				//	// ...
				//	if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)) {
				//		isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.Up] = true;
				//	} else {
				//		// Only unblock the diagonal if not blocked my multiple things
				//		if (!isDirectionBlocked[(int)Direction.Left]) {
				//			isDirectionBlocked[(int)facingDirection] = false;
				//		}
				//		isDirectionBlocked[(int)Direction.Up] = false;
				//	}
				//	if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
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
				//		&& CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)
				//		&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
				//		Debug.Log("\t\tMoving down...");
				//		MoveInDirection(Direction.Down, 0.2f);
				//	}
				//	break;
				//case Direction.DownRight:
				//	//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
				//	int downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				//	rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
				//	// ...
				//	if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)) {
				//		isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.Down] = true;
				//	} else {
				//		// Only unblock the diagonal if not blocked my multiple things
				//		if (!isDirectionBlocked[(int)Direction.Right]) {
				//			isDirectionBlocked[(int)facingDirection] = false;
				//		}
				//		isDirectionBlocked[(int)Direction.Down] = false;
				//	}
				//	if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
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
				//		&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)
				//		&& CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
				//		Debug.Log("\t\tMoving up...");
				//		MoveInDirection(Direction.Up, 0.2f);
				//	}
				//	break;
				//case Direction.DownLeft:
				////rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
				//downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				//leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				//// ...
				//if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.Down] = true;
				//} else {
				//	// Only unblock the diagonal if not blocked my multiple things
				//	if (!isDirectionBlocked[(int)Direction.Left]) {
				//		isDirectionBlocked[(int)facingDirection] = false;
				//	}
				//	isDirectionBlocked[(int)Direction.Down] = false;
				//}
				//if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
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
				//	&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)
				//	&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
				//	Debug.Log("\t\tMoving up...");
				//	MoveInDirection(Direction.Up, 0.2f);
				//}
				//break;
		}
	}



	private void ClearBlocks() {
		for (int i = 0; i < isDirectionBlocked.Length; i++) {
			isDirectionBlocked[i] = false;
		}
	}

	//private void UpdateShadowPosition() {
	//	if(currentHeight > shadow.transform.position.y) {
	//		shadow.transform.Translate(transform.position);
	//	}
	//}

	// Jumping routines
	private void Jump() {
		if (Input.GetButtonDown("Jump") && !isFalling) {
			//groundHeight += 4;
			if (grounded) {
				//groundPosition = transform.position;
				rising = true;
				jumping = true;
				jumpingRoutine = StartCoroutine(Jumping());
			}
			if (transform.position == groundPosition) {
				grounded = true;
			} else {
				grounded = false;
			}
		}
	}

	IEnumerator Jumping() {
		float previousHeight = currentHeight;
		while (rising) {
			if (transform.position.y >= jumpHeight.transform.position.y) {
				rising = false;
				isFalling = true;
			}
			if (rising) {
				//transform.Translate(Vector3.up * jumpSpeed * Time.smoothDeltaTime);
				//Vector3 maxHeight = new Vector3(groundPosition.x, groundPosition.y + maxJumpHeight, groundPosition.z);
				transform.position = Vector3.MoveTowards(transform.position, jumpHeight.transform.position,
						jumpSpeed * Time.deltaTime);
				// Calculate current height when jumping
				//jumpingHeight = transform.position.y - groundPosition.y;
				//Vector3.Distance(transform.position, groundPosition);
				currentHeight = previousHeight + transform.position.y - groundPosition.y;
			}
			yield return new WaitForEndOfFrame();
		}
		//yield return new WaitForSeconds(0.05f);
		float beforeFallHeight = currentHeight;
		//Debug.Log("Before height: " + beforeFallHeight);
		while (!rising) {
			//newCurrentHeight = currentHeight;
			//yield return new WaitForSeconds(0.01f);
			transform.position = Vector3.MoveTowards(transform.position, groundPosition,
				jumpSpeed * Time.deltaTime);
			//jumpingHeight = transform.position.y - groundPosition.y;
			Debug.Log(string.Format("\tcurr plat: {0}", currentPlatform != null ? currentPlatform.name : "no plat..."));
			currentHeight = beforeFallHeight - (beforeFallHeight - (transform.position.y - groundPosition.y))
			+ (currentPlatform != null ? currentPlatform.height : 0);
			//Debug.Log(string.Format("curr : {0}", currentHeight));
			//currentHeight = 0;
			//Debug.Log("Calc falling from jump...");
			//Debug.Log(string.Format("dist: {0}", (transform.position.y - groundPosition.y)));
			if (transform.position == groundPosition) {
				Debug.Log("Finished jumping!");
				grounded = true;
				isFalling = false;
				//StopCoroutine(jumpingRoutine);
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		jumping = false;
		//currentHeight -= newCurrentHeight;
	}

	//IEnumerator Falling() {
	//	// Start falling when finished jumping
	//	yield return new WaitUntil(() => isFalling);
	//	float beforeFallHeight = currentHeight;
	//	float newCurrentHeight = currentHeight;
	//	while (!jumping) {
	//		newCurrentHeight = currentHeight;
	//		//yield return new WaitForSeconds(0.01f);
	//		//transform.position = Vector3.MoveTowards(transform.position, groundPosition,
	//		//jumpSpeed * Time.deltaTime);
	//		//jumpingHeight = transform.position.y - groundPosition.y;
	//		transform.position = Vector3.MoveTowards(jumpHeight.transform.position, groundPosition,
	//			jumpSpeed * Time.deltaTime);
	//		currentHeight = beforeFallHeight - (beforeFallHeight - transform.position.y - groundPosition.y);
	//		//Debug.Log(string.Format("curr h: {0}", jumpingHeight));
	//		if (transform.position == groundPosition) {
	//			grounded = true;
	//			//StopCoroutine(jumpingRoutine);
	//			break;
	//		}
	//		yield return new WaitForEndOfFrame();
	//	}
	//	//currentHeight -= newCurrentHeight;
	//}

	// TODO: MovePlayer()
	private void MovePlayer() {
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing up
			StartDirection(Direction.Up);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
				Debug.Log("\trying to block with shadow...");
			}
			if (!isDirectionBlocked[(int)Direction.Up] 
				&& !isDirectionBlocked[(int)Direction.UpLeft] 
				&& !isDirectionBlocked[(int)Direction.UpRight]) {
				MoveInDirection(Direction.Up, overworldSpeed);		// Not blocked on any side, move normall
			} else if(!isDirectionBlocked[(int)Direction.Up] 
				&& isDirectionBlocked[(int)Direction.UpLeft]
				&& !isDirectionBlocked[(int)Direction.UpRight]) {
				MoveInDirection(Direction.UpRight, diagonalMovementSpeed);	// Blocked on top left, move diag right
			} else if(!isDirectionBlocked[(int)Direction.Up]
				&& !isDirectionBlocked[(int)Direction.UpLeft]
				&& isDirectionBlocked[(int)Direction.UpRight]) {
				MoveInDirection(Direction.UpLeft, diagonalMovementSpeed);  // Blocked on top right, move diag left
			}

		} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing down
			StartDirection(Direction.Down);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.Down]
				&& !isDirectionBlocked[(int)Direction.DownLeft]
				&& !isDirectionBlocked[(int)Direction.DownRight]) {
				Debug.Log("\t\tWALKING DOWN.");
				MoveInDirection(Direction.Down, overworldSpeed); 
			} else if (!isDirectionBlocked[(int)Direction.Down]
				&& isDirectionBlocked[(int)Direction.DownLeft]
				&& !isDirectionBlocked[(int)Direction.DownRight]) {
				MoveInDirection(Direction.DownRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]
				&& !isDirectionBlocked[(int)Direction.DownLeft]
				&& isDirectionBlocked[(int)Direction.DownRight]) {
				Debug.Log("\t\tWALKING DOWN LEFT.");
				MoveInDirection(Direction.DownLeft, diagonalMovementSpeed);
			}
		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing right
			StartDirection(Direction.Right);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.Right]
				&& !isDirectionBlocked[(int)Direction.UpRight]
				&& !isDirectionBlocked[(int)Direction.DownRight]) { 
				MoveInDirection(Direction.Right, overworldSpeed); 
			} else if (!isDirectionBlocked[(int)Direction.Right]
				&& isDirectionBlocked[(int)Direction.UpRight]
				&& !isDirectionBlocked[(int)Direction.DownRight]) {
				MoveInDirection(Direction.DownRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Right]
				&& !isDirectionBlocked[(int)Direction.UpRight]
				&& isDirectionBlocked[(int)Direction.DownRight]) {
				MoveInDirection(Direction.UpRight, diagonalMovementSpeed);
			}
		} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing left
			StartDirection(Direction.Left);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.Left]
				&& !isDirectionBlocked[(int)Direction.UpLeft]
				&& !isDirectionBlocked[(int)Direction.DownLeft]) {
				Debug.Log("\t\tWALKING LEFT.");
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left]
				&& isDirectionBlocked[(int)Direction.UpLeft]
				&& !isDirectionBlocked[(int)Direction.DownLeft]) {
				Debug.Log("\t\tWALKING DOWN LEFT.");
				MoveInDirection(Direction.DownLeft, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left]
				&& !isDirectionBlocked[(int)Direction.UpLeft]
			 	&& isDirectionBlocked[(int)Direction.DownLeft]) {
				MoveInDirection(Direction.UpLeft, diagonalMovementSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// TODO: Facing up-right
			StartDirection(Direction.UpRight);
			if (grounded && !isFalling) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.UpRight]) {
				Debug.Log("Moving up right...");
				MoveInDirection(Direction.UpRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Up, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// TODO: Facing up-left
			StartDirection(Direction.UpLeft);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.UpLeft]) {
				//transform.Translate(-0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpLeft, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Up, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing down-left
			StartDirection(Direction.DownLeft);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.DownLeft]) {
				//transform.Translate(-0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownLeft, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Down, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing down-right
			StartDirection(Direction.DownRight);
			if (grounded) {
				TryBlockDirections(rb2d);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			}
			if (!isDirectionBlocked[(int)Direction.DownRight]) {
				//transform.Translate(0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]) {
				MoveInDirection(Direction.Down, overworldSpeed);
			}
		} else {
			animator.SetBool("isWalking", false);
			isWalking = false;
		}
	}


	//TODO: handle case where more than half of collider is past the wall
	//Need to correct this and move player diagonally until they are no longer blocked
	private void TryBlockDirections(Rigidbody2D targetRb2d) {
		//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
		//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
		//resultsLeft[0].distance));
		//Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
		switch (facingDirection) {
			// TODO: Up()
			case Direction.Up:
				CastUp(targetRb2d,
					() => {
						currentUpBase = nextUpBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//isDirectionBlocked[(int)Direction.UpLeft] = true;
						//isDirectionBlocked[(int)Direction.UpRight] = true;
					},
					() => {
						currentDownBase = new RaycastHit2D();
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);
				// Perform left raycasts
				//CastLeft(targetRb2d,
				//	// If blocked on the right, block right direction
				//	() => {
				//		currentLeftBase = nextLeftBase;
				//		//isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.Left] = true;
				//	},
				//	() => {
				//		Debug.Log("LEFT IS NOT BEING BLOCKED");
				//		currentRightBase = new RaycastHit2D();
				//		minRightBaseDistance = Mathf.Infinity;   // Reset distance
				//	 // Only unblock the diagonal if not blocked my multiple things
				//	 //if (!isDirectionBlocked[(int)Direction.Down]) {
				//	 //	isDirectionBlocked[(int)facingDirection] = false;
				//	 //}
				//		isDirectionBlocked[(int)Direction.Left] = false;
				//		//isDirectionBlocked[(int)Direction.Left] = false;
				//	},
				//	// Clear all blocks
				//	() => {
				//		currentLeftBase = new RaycastHit2D();
				//		currentDownBase = new RaycastHit2D();
				//		minLeftBaseDistance = Mathf.Infinity;
				//		minDownBaseDistance = Mathf.Infinity;   // Reset distanc
				//	}
				//);
				CastUpLeft(targetRb2d,
					// try block up left
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block up from ul");
						currentUpLeftBase = nextUpLeftBase;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
					},
					// try clear sides
					() => {
						currentUpLeftBase = new RaycastHit2D();
						currentDownLeftBase = new RaycastHit2D();
						minUpLeftBaseDistance = Mathf.Infinity;
						minDownLeftBaseDistance = Mathf.Infinity;
					}
				);
				CastUpRight(targetRb2d,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block up from ur");
						currentUpRightBase = nextUpRightBase;
						isDirectionBlocked[(int)Direction.UpRight] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					},
					// try clear sides
					() => {
						currentUpRightBase = new RaycastHit2D();
						currentDownRightBase = new RaycastHit2D();
						minUpRightBaseDistance = Mathf.Infinity;
						minDownRightBaseDistance = Mathf.Infinity;
					}
				);
				break;
			case Direction.Down:
				CastDown(targetRb2d,
					() => {
						currentDownBase = nextDownBase;
						isDirectionBlocked[(int)facingDirection] = true;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;   // Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;   // Reset distance
						minDownBaseDistance = Mathf.Infinity;
						currentDownBase = new RaycastHit2D();
					}
				);
				//// Perform left raycasts
				//CastLeft(targetRb2d,
				//	// If blocked on the right, block right direction
				//	() => {
				//		currentLeftBase = nextLeftBase;
				//		//isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.Left] = true;
				//	},
				//	() => {
				//		Debug.Log("LEFT IS NOT BEING BLOCKED");
				//		currentRightBase = new RaycastHit2D();
				//		minRightBaseDistance = Mathf.Infinity;   // Reset distance
				//												 // Only unblock the diagonal if not blocked my multiple things
				//		//if (!isDirectionBlocked[(int)Direction.Down]) {
				//		//	isDirectionBlocked[(int)facingDirection] = false;
				//		//}
				//		isDirectionBlocked[(int)Direction.Left] = false;
				//		//isDirectionBlocked[(int)Direction.Left] = false;
				//	},
				//	// Clear all blocks
				//	() => {
				//		currentLeftBase = new RaycastHit2D();
				//		currentUpBase = new RaycastHit2D();
				//		minLeftBaseDistance = Mathf.Infinity;
				//		minRightBaseDistance = Mathf.Infinity;   // Reset distanc
				//	}
				//);
				CastDownLeft(targetRb2d,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block down from dl");
						currentDownLeftBase = nextDownLeftBase;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
					},
					// try unblock up right
					() => {
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					},
					// try clear sides
					() => {
						currentDownLeftBase = new RaycastHit2D();
						currentUpLeftBase = new RaycastHit2D();
						minDownLeftBaseDistance = Mathf.Infinity;
						minUpLeftBaseDistance = Mathf.Infinity;
					}
				);
				CastDownRight(targetRb2d,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block down from dr");
						currentDownRightBase = nextDownRightBase;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					},
					// try unblock up right
					() => {
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					// try clear sides
					() => {
						currentDownRightBase = new RaycastHit2D();
						currentUpRightBase = new RaycastHit2D();
						minDownRightBaseDistance = Mathf.Infinity;
						minUpRightBaseDistance = Mathf.Infinity;
					}
				);
				break;
			case Direction.Right:
				CastRight(targetRb2d,
					() => {
						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//isDirectionBlocked[(int)Direction.UpRight] = true;
						//isDirectionBlocked[(int)Direction.DownRight] = true;
					},
					() => {
						currentLeftBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					() => {
						currentLeftBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
						minRightBaseDistance = Mathf.Infinity;
						currentRightBase = new RaycastHit2D();
					}
				);
				CastUpRight(targetRb2d,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block up from ur");
						currentUpRightBase = nextUpRightBase;
						isDirectionBlocked[(int)Direction.UpRight] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					},
					// try clear sides
					() => {
						currentUpRightBase = new RaycastHit2D();
						currentDownRightBase = new RaycastHit2D();
						minUpRightBaseDistance = Mathf.Infinity;
						minDownRightBaseDistance = Mathf.Infinity;
					}
				);
				CastDownRight(targetRb2d,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block down from dr");
						currentDownRightBase = nextDownRightBase;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					},
					// try unblock up right
					() => {
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					// try clear sides
					() => {
						currentDownRightBase = new RaycastHit2D();
						currentUpRightBase = new RaycastHit2D();
						minDownRightBaseDistance = Mathf.Infinity;
						minUpRightBaseDistance = Mathf.Infinity;
					}
				);
				break;
			case Direction.Left:
				CastLeft(targetRb2d,
					() => {
						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//isDirectionBlocked[(int)Direction.UpLeft] = true;
						//isDirectionBlocked[(int)Direction.DownLeft] = true;
					},
					() => {
						Debug.Log("Unblockingu left");
						currentRightBase = new RaycastHit2D();
						minRightBaseDistance = Mathf.Infinity;  // Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					},
					() => {
						currentLeftBase = new RaycastHit2D();
						currentRightBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;
						minRightBaseDistance = Mathf.Infinity;  // Reset distances
					}
				);
				CastUpLeft(targetRb2d,
					// try block up left
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block up from ul");
						currentUpLeftBase = nextUpLeftBase;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
					},
					// try clear sides
					() => {
						currentUpLeftBase = new RaycastHit2D();
						currentDownRightBase = new RaycastHit2D();
						minUpLeftBaseDistance = Mathf.Infinity;
						minDownRightBaseDistance = Mathf.Infinity;
					}
				);
				CastDownLeft(targetRb2d,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						Debug.Log("block down from dl");
						currentDownLeftBase = nextDownLeftBase;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
					},
					// try unblock up right
					() => {
						//currentDownLeftBase = new RaycastHit2D();
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					},
					// try clear sides
					() => {
						currentDownLeftBase = new RaycastHit2D();
						currentUpRightBase = new RaycastHit2D();
						minDownLeftBaseDistance = Mathf.Infinity;
						minUpRightBaseDistance = Mathf.Infinity;
					}
				);
				//if (resultsLeft.Length > 0 && resultsUp.Length > 0 && resultsDown.Length > 0
				//	&& isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Down]
				//	//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
				//	&& currentHeight <= nextUpBase.transform.GetComponent<ObjectHeight>().height
				//	&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectHeight>().height) {
				//	//Debug.Log("\t\tMoving down...");
				//	//// If there is a diagonal block & is not a null block, free up the direction
				//	if (nextUpRightBase.collider != null && Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection
				//	&& nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) > boundCorrection) {
				//		//if (Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection&& Mathf.Abs(nextRightBase.distance) > boundCorrection) {
				//		Debug.Log("UR: Moving right...");
				//		isDirectionBlocked[(int)Direction.Right] = false;
				//	}
				//}
				break;
			// TODO: UpRight()
			case Direction.UpRight:
				//resultsUp = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.up, Mathf.Infinity, baseMask);
				//resultsRight = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.right, Mathf.Infinity, baseMask);
				CastUpRight(targetRb2d,
					// try block up right
					() => {
						Debug.Log("TRYING TO BBLOCK RIGHT UP");
						currentUpRightBase = nextUpRightBase;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						Debug.Log("unblocking up right...");
						isDirectionBlocked[(int)facingDirection] = false;
						if (nextUpBase.collider != null) {
							isDirectionBlocked[(int)Direction.Up] = false;
						}
						if (nextRightBase.collider != null) {
							isDirectionBlocked[(int)Direction.Right] = false;
						}
						//isDirectionBlocked[(int)Direction.Up] = false;
					},
					// try clear sides
					() => {
						currentUpRightBase = new RaycastHit2D();
						currentUpBase = new RaycastHit2D();
						minUpRightBaseDistance = Mathf.Infinity;
						minUpBaseDistance = Mathf.Infinity;
					}
				);
				CastUp(targetRb2d,
					() => {
						Debug.Log("UR: blocking up...");
						currentUpBase = nextUpBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Up] = true;
					},
					() => {
						currentDownBase = new RaycastHit2D();
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Right]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Up] = false;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);

				CastRight(targetRb2d,
					// If blocked on the right, block right direction
					() => {
						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Right] = true;
					},
					() => {
						Debug.Log("RIGHT IS NOT BEING BLOCKED");
						currentLeftBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Up]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Right] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
						currentLeftBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
						minRightBaseDistance = Mathf.Infinity;
						currentRightBase = new RaycastHit2D();
					}
				);

				//Debug.Log("u:" + nextUpBase.point + ", right:" + nextRightBase.point);
				//Debug.Log("u, r: " + nextUpBase.distance + ", " + nextRightBase.distance);
				if (resultsUp.Length > 0 && resultsRight.Length > 0
					&& isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Right]
					//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
					&& currentHeight <= nextUpBase.transform.GetComponent<ObjectHeight>().height
					&& currentHeight <= nextRightBase.transform.GetComponent<ObjectHeight>().height) {
					//Debug.Log("\t\tMoving down...");
					//// If there is a diagonal block & is not a null block, free up the direction
					if (nextUpRightBase.collider != null && Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection
					&& nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) > boundCorrection) {
					//if (Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection&& Mathf.Abs(nextRightBase.distance) > boundCorrection) {
						Debug.Log("UR: Moving right...");
						isDirectionBlocked[(int)Direction.Right] = false;
					}
				} 
				//else {
				//	if (nextUpRightBase.collider != null && Mathf.Abs(nextUpRightBase.distance) < Mathf.Epsilon
				//	&& nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) < Mathf.Epsilon
				//	&& nextUpBase.collider != null && Mathf.Abs(nextUpBase.distance) < Mathf.Epsilon) {
				//		Debug.Log("^>: Cornered");
				//		isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.Right] = true;
				//		isDirectionBlocked[(int)Direction.Up] = true;
				//	}
				//}
				break;
			// TODO: UpLeft()
			case Direction.UpLeft:
				CastUpLeft(targetRb2d,
					// try block up right
					() => {
						Debug.Log("TRYING TO BBLOCK LEFT UP");
						currentUpLeftBase = nextUpLeftBase;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						Debug.Log("unblocking up left...");
						isDirectionBlocked[(int)facingDirection] = false;
						if (nextUpBase.collider != null) {
							isDirectionBlocked[(int)Direction.Up] = false;
						}
						if (nextLeftBase.collider != null) {
							isDirectionBlocked[(int)Direction.Left] = false;
						}
						//isDirectionBlocked[(int)Direction.Up] = false;
					},
					// try clear sides
					() => {
						currentUpLeftBase = new RaycastHit2D();
						currentUpBase = new RaycastHit2D();
						minUpLeftBaseDistance = Mathf.Infinity;
						minUpBaseDistance = Mathf.Infinity;
					}
				);
				CastUp(targetRb2d,
					() => {
						currentUpBase = nextUpBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Up] = true;
					},
					() => {
						currentDownBase = new RaycastHit2D();
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Left]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Up] = false;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);

				CastLeft(targetRb2d,
					// If blocked on the right, block right direction
					() => {
						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Left] = true;
					},
					() => {
						Debug.Log("LEFT IS NOT BEING BLOCKED");
						currentRightBase = new RaycastHit2D();
						minRightBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Up]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Left] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
						Debug.Log("Clearing blcoks...");
						currentLeftBase = new RaycastHit2D();
						currentUpBase = new RaycastHit2D();
					 	minLeftBaseDistance = Mathf.Infinity;
						minRightBaseDistance = Mathf.Infinity;   // Reset distanc
					}
				);
				//Debug.Log("currentL: " + currentLeftBase.transform.name + ", nextL: " + nextLeftBase.transform.name);
				if (resultsUp.Length > 0 && resultsLeft.Length > 0
					&& isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Left]
					//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
					&& currentHeight <= nextUpBase.transform.GetComponent<ObjectHeight>().height
					&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectHeight>().height) {

					if (nextUpLeftBase.collider != null && Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection
					&& nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
					//if ( Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection
						//&& Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
						Debug.Log("\t\tMoving left...");
						isDirectionBlocked[(int)Direction.Left] = false;
					}
				} 
				break;
			case Direction.DownRight:
				CastDownRight(targetRb2d,
					// try block up right
					() => {
						Debug.Log("TRYING TO BBLOCK RIGHT DOWN");
						currentDownRightBase = nextDownRightBase;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						Debug.Log("unblocking down right...");
						isDirectionBlocked[(int)facingDirection] = false;
						if (nextDownRightBase.collider != null) {
							isDirectionBlocked[(int)Direction.Down] = false;
						}
						if (nextDownRightBase.collider != null) {
							isDirectionBlocked[(int)Direction.Right] = false;
						}
						//isDirectionBlocked[(int)Direction.Up] = false;
					},
					// try clear sides
					() => {
						currentDownRightBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minDownRightBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;
					}
				);
				CastDown(targetRb2d,
					() => {
						currentDownBase = nextDownBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Down] = true;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Right]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Down] = false;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);

				CastRight(targetRb2d,
					// If blocked on the right, block right direction
					() => {
						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Right] = true;
					},
					() => {
						Debug.Log("RIGHT IS NOT BEING BLOCKED");
						currentLeftBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Down]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Right] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
						currentRightBase = new RaycastHit2D();
						currentLeftBase = new RaycastHit2D();
						minRightBaseDistance = Mathf.Infinity;
						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);
				//Debug.Log("u:" + nextUpBase.point + ", right:" + nextRightBase.point);
				//Debug.Log("u, r: " + nextUpBase.distance + ", " + nextRightBase.distance);
				if (resultsDown.Length > 0 && resultsRight.Length > 0
					&& isDirectionBlocked[(int)Direction.Down] && isDirectionBlocked[(int)Direction.Right]
					//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
					&& currentHeight <= nextDownBase.transform.GetComponent<ObjectHeight>().height
					&& currentHeight <= nextRightBase.transform.GetComponent<ObjectHeight>().height) {

					if (nextDownRightBase.collider != null && Mathf.Abs(nextDownRightBase.distance) < diagonalBoundCorrection
						&& nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) > boundCorrection) {
						Debug.Log("\t\tMoving right...");
						isDirectionBlocked[(int)Direction.Right] = false;
					}
					//Debug.Log("\t\tMoving down...");
					//isDirectionBlocked[(int)Direction.Right] = false;
					//MoveInDirection(Direction.Down, 0.1f);
				}
				//if (resultsUp.Length > 0 && resultsLeft.Length > 0
				//	&& isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Left]
				//	//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
				//	&& currentHeight <= nextUpBase.transform.GetComponent<ObjectHeight>().height
				//	&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectHeight>().height) {

				//	if (nextUpLeftBase.collider != null && Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection
				//		&& nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
				//		Debug.Log("\t\tMoving left...");
				//		isDirectionBlocked[(int)Direction.Left] = false;
				//	}
				//}
				break;
			// TODO: DownLeft()
			case Direction.DownLeft:
				CastDownLeft(targetRb2d,
					// try block up right
					() => {
						Debug.Log("TRYING TO BBLOCK LEFT DOWN");
						currentDownLeftBase = nextDownLeftBase;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						Debug.Log("unblocking down left...");
						isDirectionBlocked[(int)facingDirection] = false;
						if (nextDownLeftBase.collider != null) {
							isDirectionBlocked[(int)Direction.Down] = false;
						}
						if (nextDownLeftBase.collider != null) {
							isDirectionBlocked[(int)Direction.Left] = false;
						}
						//isDirectionBlocked[(int)Direction.Up] = false;
					},
					// try clear sides
					() => {
						currentDownLeftBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minDownLeftBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;
					}
				);

				CastDown(targetRb2d,
					() => {
						currentDownBase = nextDownBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Down] = true;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;   // Reset distance
															  // Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Left]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Down] = false;
					},
					() => {
						currentUpBase = new RaycastHit2D();
						currentDownBase = new RaycastHit2D();
						minUpBaseDistance = Mathf.Infinity;
						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);
				// Perform left raycasts
				CastLeft(targetRb2d,
					// If blocked on the right, block right direction
					() => {
						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Left] = true;
					},
					() => {
						Debug.Log("LEFT IS NOT BEING BLOCKED");
						currentRightBase = new RaycastHit2D();
						minRightBaseDistance = Mathf.Infinity;   // Reset distance
																 // Only unblock the diagonal if not blocked my multiple things
						if (!isDirectionBlocked[(int)Direction.Down]) {
							isDirectionBlocked[(int)facingDirection] = false;
						}
						isDirectionBlocked[(int)Direction.Left] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
						currentLeftBase = new RaycastHit2D();
						currentUpBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;
						minRightBaseDistance = Mathf.Infinity;   // Reset distanc
					}
				);
				//Debug.Log("u:" + nextUpBase.point + ", right:" + nextRightBase.point);
				//Debug.Log("u, r: " + nextUpBase.distance + ", " + nextRightBase.distance);
				if (resultsDown.Length > 0 && resultsLeft.Length > 0
					&& isDirectionBlocked[(int)Direction.Down] && isDirectionBlocked[(int)Direction.Left]
					//&& nextUpBase.transform == nextRightBase.transform	
					&& currentHeight <= nextDownBase.transform.GetComponent<ObjectHeight>().height
					&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectHeight>().height) {
					if (nextDownLeftBase.collider != null && Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection
					&& nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
					//if (Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection
					//&& Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
						Debug.Log("\t\tMoving left...");
						isDirectionBlocked[(int)Direction.Left] = false;
					}
					//MoveInDirection(Direction.Down, 0.1f);
				}
				break;
		}
	}

	private bool CheckIfBlockPlayerByHeight(RaycastHit2D hit) {
		//hit.transform.GetComponent<CompositeCollider2D>().OverlapCollider(wallContactFilter, wallChecks);
		//Vector3 collPos = hit.transform.GetComponent<Collider2D>().GetComponent<ObjectPosition>().transform.position;
		//Physics2D.Raycast(collPos, Vector2.up, platformContactFilter, wallHits, Mathf.Infinity);
		//Debug.Log(string.Format("hits: {0}", hits));
		return currentHeight < hit.transform.GetComponent<ObjectHeight>().height;
		//+ wallHits[0].collider.GetComponent<ObjectHeight>().height;
		//+ wallChecks[0].GetComponent<ObjectHeight>().height && !isFalling;
	}

	// TODO: MoveInDirection()
	private void MoveInDirection(Direction diretction, float speed) {
		switch (diretction) {
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
				transform.Translate(speed, -speed, 0);
				shadow.transform.Translate(speed, -speed, 0);
				jumpHeight.transform.Translate(speed, -speed, 0);
				break;
			case Direction.DownLeft:
				transform.Translate(-speed, -speed, 0);
				shadow.transform.Translate(-speed, -speed, 0);
				jumpHeight.transform.Translate(-speed, -speed, 0);
				break;
			default:
				transform.Translate(0, 0, 0);
				shadow.transform.Translate(0, 0, 0);
				jumpHeight.transform.Translate(0, 0, 0);
				break;
		}
	}

	private void StartDirection(Direction direction) {
		isWalking = true;
		facingDirection = direction;
		animator.SetBool("isWalking", true);
		animator.SetInteger("direction", (int)facingDirection - 1);
		animator.SetTrigger("changeDirection");
	}

	private void ResetMins() {
		minLeftBaseDistance = Mathf.Infinity;
		minRightBaseDistance = Mathf.Infinity;
		minUpBaseDistance = Mathf.Infinity;
		minDownBaseDistance = Mathf.Infinity;
		minUpLeftBaseDistance = Mathf.Infinity;
		minUpRightBaseDistance = Mathf.Infinity;
		minDownLeftBaseDistance = Mathf.Infinity;
		minDownRightBaseDistance = Mathf.Infinity;
	}

	// TODO: Cast methods
	// TODO: Add side casts; for Up, need one ray at x=pos-boundCorrection & another at x=pos+boundCorrection
	//			if left.distance < right.distance || left.collider != null && right.collider == null
	//				move UpRight
	//			if left.distance > right.distance || left.collider == null && right.collider != null
	//				move UpLeft
	//			(...)
	private void CastUp(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSides) {
		resultsUp = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.up, Mathf.Infinity, baseMask);
		if (resultsUp.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsUp.Length; i++) {
				hit = resultsUp[i];
				//Debug.Log("checking ^ base...: " + hit.transform.name);
				if (currentUpBase.collider != null) {
					//Debug.Log("current up isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentUpBase.transform && hit.distance < minUpBaseDistance) {
						nextUpBase = hit;
						minUpBaseDistance = hit.distance;
						//break;
					}
				} else {
					//Debug.Log("current up is null. set next to be the check");
					// If current is null, nextbase is the closest one
					nextUpBase = hit;
					minUpBaseDistance = hit.distance;
					break;
				}
			}
			if ((currentUpBase.collider != null && !Contains(resultsUp, currentUpBase)) || resultsUp.Length == 1) {
				nextUpBase = hit;
				currentUpBase = new RaycastHit2D();
				minUpBaseDistance = hit.distance;
			}
			//Debug.Log("next up base: " + (nextUpBase.collider != null ? nextUpBase.transform.name : "no next up base"));
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, nextUpBase.distance, 0), Color.red);
			if (nextUpBase.collider != null && Mathf.Abs(nextUpBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextUpBase)
				&& fallingDirection != Direction.Up && !isOnPlatform) {
				Debug.Log("blocking up...");
				blockSide();
			} else {
				Debug.Log("unblocking up...");
				unblockSide();
			}
			//} else {
			//	Debug.Log("null hit");
			//}
		} else {
			Debug.Log("No base detected ^");
			clearSides();
			isDirectionBlocked[(int)Direction.Down] = false;
			isDirectionBlocked[(int)Direction.Up] = false;
			//ClearBlocks();
		}
	}

	private void CastDown(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsDown = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.down, Mathf.Infinity, baseMask);
		if (resultsDown.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsDown.Length; i++) {
				hit = resultsDown[i];
				//Debug.Log("checking v base...: " + hit.transform.name);
				if (currentDownBase.collider != null) {
					//Debug.Log("current down isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentDownBase.transform && hit.distance < minDownBaseDistance) {
						nextDownBase = hit;
						minDownBaseDistance = hit.distance;
						//break;
					}
				} else {
					//Debug.Log("current down is null. set next to be the check");
					// If current is null, nextbase is the closest one
					nextDownBase = hit;
					minDownBaseDistance = hit.distance;
					break;
				}
			}
			if (currentDownBase.collider != null && !Contains(resultsDown, currentDownBase) || resultsDown.Length == 1) {
				nextDownRightBase = hit;
				currentDownBase = new RaycastHit2D();
				minDownBaseDistance = hit.distance;
			}
			//Debug.Log("nextbase: " + (nextDownBase.collider != null ? nextDownBase.transform.name : "no next base"));
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, -nextDownRightBase.distance, 0), Color.red);
			if (nextDownBase.collider != null && Mathf.Abs(nextDownBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextDownBase)
				&& fallingDirection != Direction.Down && !isOnPlatform) {
				//Debug.Log("blocking down...");
				blockSide();
			} else {
				//Debug.Log("unblocking down...");
				unblockSide();
			}
			//} else {
			//	Debug.Log("null hit");
			//}
		} else {
			//Debug.Log("No base detected v");
			clearSide();
			isDirectionBlocked[(int)Direction.Down] = false;
			isDirectionBlocked[(int)Direction.Up] = false;
			//ClearBlocks();
		}
	}

	private void CastRight(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsRight = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.right, Mathf.Infinity, baseMask);
		if (resultsRight.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsRight.Length; i++) {
				hit = resultsRight[i];
				//Debug.Log("checking > base...: " + hit.transform.name);
				if (currentRightBase.collider != null) {
					//Debug.Log("current right isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentRightBase.transform && hit.distance < minRightBaseDistance) {
						nextRightBase = hit;
						minRightBaseDistance = hit.distance;
						//break;
					}
				} else {
					//Debug.Log("current right is null. set next to be the check");
					// If current is null, nextbase is the closest one
					nextRightBase = hit;
					minRightBaseDistance = hit.distance;
					break;
				}
			}
			if (currentRightBase.collider != null && !Contains(resultsRight, currentRightBase) || resultsRight.Length == 1) {
				Debug.Log("CURRENT ISNT IN LIST. REMOVE IT.");
				nextRightBase = hit;
				currentUpBase = new RaycastHit2D();
				minRightBaseDistance = hit.distance;
			}
			//Debug.Log("next right base: " + (nextRightBase.collider != null ? nextRightBase.transform.name : "no next right base"));
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(nextRightBase.distance, 0, 0), Color.red);
			if (nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextRightBase)
				&& fallingDirection != Direction.Right && !isOnPlatform) {
				Debug.Log("blocking right...");
				blockSide();
			} else if (nextRightBase.collider != null) {
				Debug.Log("unblocking right...");
				unblockSide();
			} else {
				Debug.Log("null hit");
			}
		} else {
			Debug.Log("No base detected >");
			clearSide();
			isDirectionBlocked[(int)Direction.Right] = false;
			isDirectionBlocked[(int)Direction.Left] = false;
			//isDirectionBlocked[(int)Direction.Down] = false;
			//isDirectionBlocked[(int)Direction.DownLeft] = false;
		}
	}

	private void CastLeft(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsLeft = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.left, Mathf.Infinity, baseMask);
		if (resultsLeft.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsLeft.Length; i++) {hit = resultsLeft[i];
				//Debug.Log("checking < base...: " + hit.transform.name);
				if (currentLeftBase.collider != null) {
					//Debug.Log("current left isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentLeftBase.transform && hit.distance < minLeftBaseDistance) {
						nextLeftBase = hit;
						minLeftBaseDistance = hit.distance;
						//break;
					} 
					//else if (hit.transform == currentLeftBase.transform && hit.distance >= minLeftBaseDistance) {
					//	// Base is the same base. update its distance
					//	nextLeftBase = hit;  // TODO: may not solve issue of when inside a bblock
					//	minLeftBaseDistance = hit.distance;
					//}
				} else {
					//Debug.Log("current left is null");
					// If current is null, nextbase is the closest one
					nextLeftBase = hit;
					minLeftBaseDistance = hit.distance;
					break;
				}
			}
			if(currentLeftBase.collider != null && !Contains(resultsLeft, currentLeftBase) || resultsLeft.Length == 1) {
				Debug.Log("CURRENT ISNT IN LIST. REMOVE IT.");
				nextLeftBase = hit;
				currentLeftBase = new RaycastHit2D();
				minLeftBaseDistance = hit.distance;
			}
			Debug.Log("next left base: " + (nextLeftBase.collider != null ? nextLeftBase.transform.name : "no next left base"));

			Debug.DrawRay(targetRb2d.transform.position, new Vector3(-nextLeftBase.distance, 0, 0), Color.red);
			if (nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextLeftBase)
				&& fallingDirection != Direction.Left && !isOnPlatform) {
				Debug.Log("blocking left...");
				blockSide();
			} else if (nextLeftBase.collider != null) {
				Debug.Log("unblocking left...");
				unblockSide();
			} else {
				Debug.Log("null hit");
			}
		} else {
			Debug.Log("No base detected <");
			clearSide();
			isDirectionBlocked[(int)Direction.Right] = false;
			isDirectionBlocked[(int)Direction.Left] = false;
			//ClearBlocks();
		}
	}

	private void CastUpRight(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsUpRight = Physics2D.RaycastAll(targetRb2d.transform.position, new Vector2(1,1), Mathf.Infinity, baseMask);
		if (resultsUpRight.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsUpRight.Length; i++) {
				hit = resultsUpRight[i];
				//Debug.Log("checking ^> base...: " + hit.transform.name);
				if (currentUpRightBase.collider != null) {
					//Debug.Log("current upright isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentUpRightBase.transform && hit.distance < minUpRightBaseDistance) {
						//Debug.Log("next and current are different");
						nextUpRightBase = hit;
						minUpRightBaseDistance = hit.distance;
					} else if (hit.transform == currentUpRightBase.transform && hit.distance >= minUpRightBaseDistance) {
						// Base is the same base. update its distance
						nextUpRightBase = hit;	// TODO: may not solve issue of when inside a bblock
						minUpRightBaseDistance = hit.distance;
					}
				} else {
					//Debug.Log("current upright is null");
					// If current is null, nextbase is the closest one
					nextUpRightBase = hit;
					minUpRightBaseDistance = hit.distance;
					break;
				}
			}
			if (currentUpRightBase.collider != null && !Contains(resultsUpRight, currentUpRightBase) || resultsUpRight.Length == 1) {
				nextUpRightBase = hit;
				currentUpRightBase = new RaycastHit2D();
				minUpRightBaseDistance = hit.distance;
			} 
			// TODO: next base isn't updating when next is the same transform
			Debug.Log("next ur base: " + (nextUpRightBase.collider != null ? nextUpRightBase.transform.name : "no next ur base") + ": " + nextUpRightBase.distance);
			Debug.DrawRay(targetRb2d.transform.position,
				Vector3.ClampMagnitude(new Vector3(nextUpRightBase.distance, nextUpRightBase.distance, 0), nextUpRightBase.distance)
				, Color.red);
			Debug.Log("is distance in bound: " + (Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection));
			if (nextUpRightBase.collider != null && Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextUpRightBase)
				&& fallingDirection != Direction.UpRight && !isOnPlatform) {
				//Debug.Log("blocking upright...");
				blockSide();
			} else {
				//Debug.Log("unblocking upright...");
				unblockSide();
			} 
			//else {
			//	Debug.Log("null hit");
			//}
		} else {
			//Debug.Log("No base detected ^>");
			clearSide();
			isDirectionBlocked[(int)Direction.UpRight] = false;
			isDirectionBlocked[(int)Direction.DownLeft] = false;
			//ClearBlocks();
		}
	}

	private void CastUpLeft(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsUpLeft = Physics2D.RaycastAll(targetRb2d.transform.position, new Vector2(-1, 1), Mathf.Infinity, baseMask);
		if (resultsUpLeft.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsUpLeft.Length; i++) {
				hit = resultsUpLeft[i];
				//Debug.Log("checking <^ base...: " + hit.transform.name);
				if (currentUpLeftBase.collider != null) {
					//Debug.Log("current upright isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentUpLeftBase.transform && hit.distance < minUpLeftBaseDistance) {
						nextUpLeftBase = hit;
						minUpLeftBaseDistance = hit.distance;
					} else if (hit.transform == currentUpLeftBase.transform && hit.distance >= minLeftBaseDistance) {
						// Base is the same base. update its distance
						nextUpLeftBase = hit;  // TODO: may not solve issue of when inside a bblock
						minUpLeftBaseDistance = hit.distance;
					}
				} else {
					//Debug.Log("current upright is null");
					// If current is null, nextbase is the closest one
					nextUpLeftBase = hit;
					minUpLeftBaseDistance = hit.distance;
					break;
				}
			}
			if (currentUpLeftBase.collider != null && !Contains(resultsUp, currentUpLeftBase) || resultsUpLeft.Length == 1) {
				nextUpLeftBase = hit;
				currentUpLeftBase = new RaycastHit2D();
				minUpLeftBaseDistance = hit.distance;
			}
			//Debug.Log("next ur base: " + (nextUpRightBase.collider != null ? nextUpRightBase.transform.name : "no next ur base"));
			Debug.DrawRay(targetRb2d.transform.position,
				Vector3.ClampMagnitude(new Vector3(-nextUpLeftBase.distance, nextUpLeftBase.distance, 0), nextUpLeftBase.distance)
				, Color.red);
			if (nextUpLeftBase.collider != null && Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextUpLeftBase)
				&& fallingDirection != Direction.UpLeft && !isOnPlatform) {
				//Debug.Log("blocking upright...");
				blockSide();
			} else if (nextUpLeftBase.collider != null) {
				//Debug.Log("unblocking upright...");
				unblockSide();
			} else {
				Debug.Log("null hit");
			}
		} else {
			//Debug.Log("No base detected ^>");
			clearSide();
			isDirectionBlocked[(int)Direction.UpLeft] = false;
			isDirectionBlocked[(int)Direction.DownRight] = false;
			//ClearBlocks();
		}
	}

	private void CastDownLeft(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsDownLeft = Physics2D.RaycastAll(targetRb2d.transform.position, new Vector2(-1, -1), Mathf.Infinity, baseMask);
		if (resultsDownLeft.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsDownLeft.Length; i++) {
				hit = resultsDownLeft[i];
				//Debug.Log("checking <v base...: " + hit.transform.name);
				if (currentDownLeftBase.collider != null) {
					//Debug.Log("current upright isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentDownLeftBase.transform && hit.distance < minDownLeftBaseDistance) {
						nextDownLeftBase = hit;
						minDownLeftBaseDistance = hit.distance;
					} else if (hit.transform == currentDownLeftBase.transform && hit.distance >= minDownLeftBaseDistance) {
						// Base is the same base. update its distance
						nextDownLeftBase = hit;  // TODO: may not solve issue of when inside a bblock
						minDownLeftBaseDistance = hit.distance;
					}
				} else {
					//Debug.Log("current upright is null");
					// If current is null, nextbase is the closest one
					nextDownLeftBase = hit;
					minDownLeftBaseDistance = hit.distance;
					break;
				}
			}
			// Update
			if (currentDownLeftBase.collider != null && !Contains(resultsDownLeft, currentDownLeftBase) || resultsDownLeft.Length == 1) {
				Debug.Log("Update down left....");
				nextDownLeftBase = hit;
				currentDownLeftBase = new RaycastHit2D();
				minDownLeftBaseDistance = hit.distance;
			}
			//if(resultsDownLeft.Length == 1) {
			//	nextDownLeftBase = hit;
			//	minDownLeftBaseDistance = hit.distance;
			//}
			Debug.Log("next dl base: " + (nextDownLeftBase.collider != null ? nextDownLeftBase.transform.name : "no next dl base"));
			Debug.DrawRay(targetRb2d.transform.position,
				Vector3.ClampMagnitude(new Vector3(-nextDownLeftBase.distance, -nextDownLeftBase.distance, 0), nextDownLeftBase.distance)
				, Color.red);
			// If hit isnt null and reached the bound, check if bblocked by height
			if (nextDownLeftBase.collider != null && Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextDownLeftBase)
				&& fallingDirection != Direction.DownLeft && !isOnPlatform) {
				Debug.Log("blocking down left...");
				blockSide();
				//} else if (nextDownLeftBase.collider != null) {
			} else {
				Debug.Log("unblocking down left...");
				unblockSide();
			} 
			//else {
			//	Debug.Log("null hit");
			//}
		} else {
			Debug.Log("No base detected <^");
			clearSide();
			isDirectionBlocked[(int)Direction.DownLeft] = false;
			isDirectionBlocked[(int)Direction.UpRight] = false;
			//ClearBlocks();
		}
	}

	private void CastDownRight(Rigidbody2D targetRb2d, Action blockSide, Action unblockSide, Action clearSide) {
		resultsDownRight = Physics2D.RaycastAll(targetRb2d.transform.position, new Vector2(1, -1), Mathf.Infinity, baseMask);
		if (resultsDownRight.Length > 0) {
			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
			// ...
			RaycastHit2D hit = new RaycastHit2D();
			for (int i = 0; i < resultsDownRight.Length; i++) {
				hit = resultsDownRight[i];
				Debug.Log("checking v> base...: " + hit.transform.name);
				if (currentDownRightBase.collider != null) {
					//Debug.Log("current upright isnt null");
					// If this hit isnt the same as the current one, and its distance is smaller,
					// 		Set the nextBase
					if (hit.transform != currentDownRightBase.transform && hit.distance < minDownRightBaseDistance) {
						nextDownRightBase = hit;
						minDownRightBaseDistance = hit.distance;
					} else if (hit.transform == currentDownRightBase.transform && hit.distance >= minDownRightBaseDistance) {
						// Base is the same base. update its distance
						nextDownRightBase = hit;  // TODO: may not solve issue of when inside a bblock
						minDownRightBaseDistance = hit.distance;
					}
				} else {
					//Debug.Log("current upright is null");
					// If current is null, nextbase is the closest one
					nextDownRightBase = hit;
					minDownRightBaseDistance = hit.distance;
					break;
				}
			}
			if (currentDownRightBase.collider != null && !Contains(resultsDownRight, currentDownRightBase) || resultsDownRight.Length == 1) {
				nextDownRightBase = hit;
				currentDownRightBase = new RaycastHit2D();
				minDownRightBaseDistance = hit.distance;
			}
			//Debug.Log("next ur base: " + (nextUpRightBase.collider != null ? nextUpRightBase.transform.name : "no next ur base"));
			Debug.DrawRay(targetRb2d.transform.position,
				Vector3.ClampMagnitude(new Vector3(nextDownRightBase.distance, -nextDownRightBase.distance, 0), nextDownRightBase.distance)
				, Color.red);
			if (nextDownRightBase.collider != null && Mathf.Abs(nextDownRightBase.distance) < diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextDownRightBase)
				&& fallingDirection != Direction.DownRight && !isOnPlatform) {
				//Debug.Log("blocking upright...");
				blockSide();
			} else if (nextDownRightBase.collider != null) {
				//Debug.Log("unblocking upright...");
				unblockSide();
			} else {
				Debug.Log("null hit");
			}
		} else {
			//Debug.Log("No base detected ^>");
			clearSide();
			isDirectionBlocked[(int)Direction.DownRight] = false;
			isDirectionBlocked[(int)Direction.UpLeft] = false;
			//ClearBlocks();
		}
	}

	private bool Contains(RaycastHit2D[] hits, RaycastHit2D current) {
		if(hits == null) {
			return false;
		}
		for (int i = 0; i < hits.Length; i++) {
			Debug.Log("current: " + current.collider.name + " checking: " + hits[i].collider.name);
			//if(Mathf.Abs(hits[i].distance - current.distance) < Mathf.Epsilon) {
			if (hits[i].transform.Equals(current.transform)) {
				return true;
			}
		}
		return false;
	}
}
