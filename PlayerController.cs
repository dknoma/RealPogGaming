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
	private const float boundCorrection = 0.5f;

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
	private bool[] isDirectionBlocked = new bool[(int)Direction.DownLeft+1];
	private bool[] isPDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
	// Base physics
	private RaycastHit2D[] resultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] resultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] resultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] resultsLeft;
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
			if(fallingHeight < 8) {
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
		groundPosition = shadow.transform.position ;
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
		if(shadow.GetComponent<Collider2D>().Raycast(Vector2.down, floorContactFilter, floorHits, Mathf.Infinity) > 0) {
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
				shadow.transform.position += new Vector3(boundCorrection * 2 -fallingHeight - (boundCorrection * 2), 0);
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
				if(fallingRoutine != null) {
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
				if(pResultsLeft.Length > 0) {
				//if (rb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity) > 0) {
				// Block all upward directions to prevent sliding into walls
					//currentPlatformCheck = pResultsLeft[0].transform.GetComponent<PlatformInfo>();
					//nextPlatform = pResultsLeft.Length > 1 ? pResultsLeft[1].transform.GetComponent<PlatformInfo>() : null;
					for(int i = 0; i < pResultsLeft.Length; i++) {
						PlatformInfo checkPlat = pResultsLeft[i].transform.GetComponent<PlatformInfo>();
						//Debug.Log("check name: " + checkPlat.name);
						// If check is null
						//if(currentPlatform == null) {
						//	currentPlatform = checkPlat;
						//} else {
						//	nextPlatform = !checkPlat.name.Equals(currentPlatform.name) ? checkPlat : null;
						//}
						if (currentPlatform != null) {
							if(checkPlat != currentPlatform) {
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
					if(pResultsLeft.Length == 1) {
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
		for(int i = 0; i < isDirectionBlocked.Length; i++) {
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
		if(Input.GetButtonDown("Jump") && !isFalling) {
			//groundHeight += 4;
			if(grounded) {
				//groundPosition = transform.position;
				rising = true;
				jumping = true;
				jumpingRoutine = StartCoroutine(Jumping());
			}
			if(transform.position == groundPosition) {
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

	private void MovePlayer() {
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing up
			StartDirection(Direction.Up);
			if (grounded) {
				TryBlockDirections(rb2d, Vector2.up);
			} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>(), Vector2.up);
				Debug.Log("\trying to block with shadow...");
			}
			if (!isDirectionBlocked[(int)Direction.Up]) {
				//Debug.Log("\tInput up");
				MoveInDirection(Direction.Up, overworldSpeed); 
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
					if (Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0])
						&& fallingDirection != Direction.Up) {
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
						isDirectionBlocked[(int)Direction.UpRight] = true;
					} else {
						Debug.Log("No block up");
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
					if (Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0])
						&& fallingDirection != Direction.Down) {
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
				resultsRight = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.right, Mathf.Infinity, baseMask);
				if (resultsRight.Length > 0) {
					//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
					// ...
					for (int i = 0; i < resultsRight.Length; i++) {
						RaycastHit2D hit = resultsRight[i];
						Debug.Log("checking > base...: " + hit.transform.name);
						if (currentRightBase.collider != null) {
							Debug.Log("current right isnt null");
							// If this hit isnt the same as the current one, and its distance is smaller,
							// 		Set the nextBase
							if (hit.transform != currentRightBase.transform && hit.distance < minRightBaseDistance) {
								nextRightBase = hit;
								minRightBaseDistance = hit.distance;
								//break;
							}
						} else {
							Debug.Log("current right is null. set next to be the check");
							// If current is null, nextbase is the closest one
							nextRightBase = hit;
							minRightBaseDistance = hit.distance;
							break;
						}
					}
					Debug.Log("nextbase: " + (nextRightBase.collider != null ? nextRightBase.transform.name : "no next base"));

					if (nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextRightBase)
						&& fallingDirection != Direction.Right && !isOnPlatform) {
						Debug.Log("blocking left...");
						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.UpRight] = true;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					} else if (nextRightBase.collider != null) {
						Debug.Log("unblocking left...");
						currentLeftBase = new RaycastHit2D();
						minLeftBaseDistance = Mathf.Infinity;	// Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;	// TODO: get right checks to work
					} else {
						Debug.Log("null hit");
					}
				} else {
					Debug.Log("No base detected >");
					currentLeftBase = new RaycastHit2D();
					minLeftBaseDistance = Mathf.Infinity;   // Reset distance
					minRightBaseDistance = Mathf.Infinity;
					currentRightBase = new RaycastHit2D();
					ClearBlocks();
				}
				//if(targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity) > 0) {
				//	// ...
				//	if (Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0]) &&
				//	fallingDirection != Direction.Right) {
				//		isDirectionBlocked[(int)facingDirection] = true;
				//		isDirectionBlocked[(int)Direction.UpRight] = true;
				//		isDirectionBlocked[(int)Direction.DownRight] = true;
				//	} else {
				//		isDirectionBlocked[(int)facingDirection] = false;
				//		isDirectionBlocked[(int)Direction.UpRight] = false;
				//		isDirectionBlocked[(int)Direction.DownRight] = false;
				//	}
				//}
				break;
			case Direction.Left:
				//if (targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity) > 0) {
				//RaycastHit2D[] hits = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.left, Mathf.Infinity, layerMask);
				resultsLeft = Physics2D.RaycastAll(targetRb2d.transform.position, Vector2.left, Mathf.Infinity, baseMask);
				if (resultsLeft.Length > 0) {
					//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
					// ...
					for (int i = 0; i < resultsLeft.Length; i++) {
						RaycastHit2D hit = resultsLeft[i];
						Debug.Log("checking < base...: " + hit.transform.name);
						if (currentLeftBase.collider != null) {
							Debug.Log("current left isnt null");
							// If this hit isnt the same as the current one, and its distance is smaller,
							// 		Set the nextBase
							if (hit.transform != currentLeftBase.transform && hit.distance < minLeftBaseDistance) {
								nextLeftBase = hit;
								minLeftBaseDistance = hit.distance;
								//break;
							}
						} else {
							Debug.Log("current left is null");
							// If current is null, nextbase is the closest one
							nextLeftBase = hit;
							minLeftBaseDistance = hit.distance;
							break;
						}
					}
					//if (resultsLeft.Length == 1) {
					//	nextLeftBase = new RaycastHit2D();
					//}
					Debug.Log("nextbase: " + (nextLeftBase.collider != null ? nextLeftBase.transform.name : "no next base"));

					if (nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextLeftBase)
						&& fallingDirection != Direction.Left && !isOnPlatform) {
						Debug.Log("blocking left...");
						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
					} else if (nextLeftBase.collider != null) {
						Debug.Log("unblocking left...");
						currentRightBase = new RaycastHit2D();
						minRightBaseDistance = Mathf.Infinity;	// Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					} else {
						Debug.Log("null hit");
					}
				} else {
					Debug.Log("No base detected <");
					currentRightBase = new RaycastHit2D();
					minRightBaseDistance = Mathf.Infinity;  // Reset distance
					minRightBaseDistance = Mathf.Infinity;
					currentLeftBase = new RaycastHit2D();
					ClearBlocks();
				}

				//if (Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0])
				//	&& fallingDirection != Direction.Left && !isOnPlatform) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.UpLeft] = true;
				//	isDirectionBlocked[(int)Direction.DownLeft] = true;
				//} else if (resultsLeft.Length > 1 && Mathf.Abs(resultsLeft[1].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[1])
				//	&& fallingDirection != Direction.Left && isOnPlatform) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.UpLeft] = true;
				//	isDirectionBlocked[(int)Direction.DownLeft] = true;
				//} else {
				//	isDirectionBlocked[(int)facingDirection] = false;
				//	isDirectionBlocked[(int)Direction.UpLeft] = false;
				//	isDirectionBlocked[(int)Direction.DownLeft] = false;
				//}
				break;
			case Direction.UpRight:
				//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
				int upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
				int rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
				// ...
				if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0])
					&& fallingDirection != Direction.Up) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Right]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Up] = false;
				}
				if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0])
					&& fallingDirection != Direction.Right) {
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
					&& currentHeight <= resultsUp[0].transform.GetComponent<ObjectHeight>().height
					&& currentHeight <= resultsRight[0].transform.GetComponent<ObjectHeight>().height) {
					Debug.Log("\t\tMoving down...");
					MoveInDirection(Direction.Down, 0.2f);
				}
				break;
			case Direction.UpLeft:
				//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
				upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
				int leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				// ...
				if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0])) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Left]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Up] = false;
				}
				if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0])) {
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
					&& CheckIfBlockPlayerByHeight(resultsUp[0])
					&& CheckIfBlockPlayerByHeight(resultsLeft[0])) {
					Debug.Log("\t\tMoving down...");
					MoveInDirection(Direction.Down, 0.2f);
				}
				break;
			case Direction.DownRight:
				//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
				int downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
				// ...
				if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0])) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Right]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Down] = false;
				}
				if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0])) {
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
					&& CheckIfBlockPlayerByHeight(resultsDown[0])
					&& CheckIfBlockPlayerByHeight(resultsRight[0])) {
					Debug.Log("\t\tMoving up...");
					MoveInDirection(Direction.Up, 0.2f);
				}
				break;
			case Direction.DownLeft:
				//rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
				downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
				leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
				// ...
				if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0])) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Left]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Down] = false;
				}
				if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0])) {
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
					&& CheckIfBlockPlayerByHeight(resultsDown[0])
					&& CheckIfBlockPlayerByHeight(resultsLeft[0])) {
					Debug.Log("\t\tMoving up...");
					MoveInDirection(Direction.Up, 0.2f);
				}
				break;
		}
	}

	// TODO: Check may not be going through... idk what the !isFalling was for
	private bool CheckIfBlockPlayerByHeight(RaycastHit2D hit) {
		//hit.transform.GetComponent<CompositeCollider2D>().OverlapCollider(wallContactFilter, wallChecks);
		//Vector3 collPos = hit.transform.GetComponent<Collider2D>().GetComponent<ObjectPosition>().transform.position;
		//Physics2D.Raycast(collPos, Vector2.up, platformContactFilter, wallHits, Mathf.Infinity);
		//Debug.Log(string.Format("hits: {0}", hits));
		return currentHeight < hit.transform.GetComponent<ObjectHeight>().height;
		//+ wallHits[0].collider.GetComponent<ObjectHeight>().height;
		//+ wallChecks[0].GetComponent<ObjectHeight>().height && !isFalling;
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
		animator.SetInteger("direction", (int)facingDirection-1);
		animator.SetTrigger("changeDirection");
	}
}