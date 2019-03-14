using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

public enum Direction { 
	Null,
	Down, 
	DownRight, 
	Right, 
	UpRight, 
	Up, 
	UpLeft, 
	Left, 
	DownLeft 
}

/*
 * TODO: Handle collision when unit wasn't able to make it over the top
 *		- if falling and is inside the collider, push them back in the other direction
 * 		- Fix jumping from the top while facing down
 * 		- If slow, make Dictionaries that hold components (platform info, etc) and get from ids
*/			 
public class PlayerController : TopDownBehavior {

	[Header("Direction Variables")]
	public Direction facingDirection = Direction.Down;
	public bool isWalking;

	[Header("Physics Variables")]
	public float jumpSpeed = 5.0f;
	public float overworldSpeed = 0.35f;
	public float gravityModifier = 5;

	private int platformMask;
	private int baseMask; // base filter

	// Constants
	private const float boundCorrection = 1.625f;

	// Game Objects
	private Animator animator;
	private BoxCollider2D myCollider;
	private Rigidbody2D rb2d;
	private ContactFilter2D baseContactFilter;
	private ContactFilter2D wallContactFilter;
	private ContactFilter2D platformContactFilter;
	private ContactFilter2D floorContactFilter;
	private ContactFilter2D boundsContactFilter;
	private PlayerShadow shadow;
	private Transform jumpHeight;
//	private GameObject jumpHeight;
	private ObjectInfo nextPlatform;
	private ObjectInfo currentPlatform;
	private ObjectInfo previousPlatform;

	// Physics stuff
	private bool[] isDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
//	private bool[] isPDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
	// Base physics
	private RaycastHit2D[] resultsUp = new RaycastHit2D[2];
	private RaycastHit2D[] resultsDown = new RaycastHit2D[2];
	private RaycastHit2D[] resultsRight = new RaycastHit2D[2];
	private RaycastHit2D[] resultsLeft = new RaycastHit2D[2];
	private RaycastHit2D[] resultsUpRight = new RaycastHit2D[2];
	private RaycastHit2D[] resultsUpLeft = new RaycastHit2D[2];
	private RaycastHit2D[] resultsDownRight = new RaycastHit2D[2];
	private RaycastHit2D[] resultsDownLeft = new RaycastHit2D[2];
	
	// Platform physics
	private RaycastHit2D[] pResultsUp = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsDown = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsRight = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsLeft;
	private RaycastHit2D[] pResultsUpRight = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsUpLeft = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsDownRight = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsDownLeft = new RaycastHit2D[2];

	private RaycastHit2D[] floorHits = new RaycastHit2D[2];
	private RaycastHit2D[] wallHits = new RaycastHit2D[2];

	// Raycast stuff
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

	private RaycastHit2D[] currentPlatformHits;
	private RaycastHit2D currentPlatformHit;
	private RaycastHit2D nextPlatformHit;
	private float minPlatformDistance;

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
	
	private Vector2 velocity;
	private Vector2 targetVelocity;
	private Vector2 groundNormal;
	private Vector2 move;
	
	[HideInInspector]
	public bool jumping;
	public float currentHeight;
	
	[HideInInspector]
	public bool isFalling;
	
	private bool fallingOffPlatform;
	private bool isOnPlatform;
	private bool transitionToCurrentPlatform;
	private bool leftCurrentPlatform;
	private float jumpingHeight;
	private float fallingHeight;
	private int totalHeight;
	private float blockingCorrectionSpeed;
	private float diagonalMovementSpeed;
	private float diagonalBoundCorrection;

	// Coroutines
	private Coroutine jumpingRoutine;
	private Coroutine fallingRoutine;
	private Coroutine platformRoutine;
	private Coroutine landingRoutine;
	private static readonly int IsWalkingAni = Animator.StringToHash("isWalking");
	private static readonly int DirectionAni = Animator.StringToHash("direction");
	private static readonly int ChangeDirectionAni = Animator.StringToHash("changeDirection");

	private void Start() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
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
		boundsContactFilter.SetLayerMask(LayerMask.GetMask("Bounds"));
		//yOffset = Mathf.Abs(collider.offset.y);
		//yRadius = collider.size.y / 2;
		//xRadius = collider.size.x / 2;
		//Debug.Log(string.Format("coll offset: {0}",yOffset+yRadius));
		shadow = transform.parent.GetComponentInChildren<PlayerShadow>();
		jumpHeight = gameObject.FindComponentInSiblingsWithTag<Transform>("JumpHeight");
		diagonalMovementSpeed = Mathf.Sqrt(overworldSpeed * overworldSpeed / 2);
		diagonalBoundCorrection = Mathf.Sqrt(2 * (boundCorrection*boundCorrection));
		blockingCorrectionSpeed = overworldSpeed;
		previousPlatform = null;
		//Debug.Log("diag bound: " + diagonalBoundCorrection);
	}

	// Reset animation settings when main player is changed
	public void ChangeCharacter() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
	}

	private void Update() {
		//Debug.Log("curr h: " + currentHeight);
		//Debug.Log("fall direction: " + fallingDirection);
		//Debug.Log("shad pos: " + shadow.transform.position);

		Jump();
		MovePlayer();
		CheckPlatformCollision();
//		ComputeVelocity();
		//groundHeight = shadow.totalHeight;
		//Debug.Log("player pos: " + transform.position);
	}

	private void FixedUpdate() {
		groundPosition = shadow.transform.position;
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
		
		
//		Vector2 deltaPosition = velocity * Time.deltaTime;
//		Vector2 moveAlongGround = new Vector2 (groundNormal.y, -groundNormal.x);
//
//		Vector2 moving = moveAlongGround * deltaPosition.x;
//		
//		Debug.Log("moveAlongGround " + moveAlongGround);
//
//		float distance = moving.magnitude;
//
//		if (distance > 0) {
//			Vector2 currentNormal = transform.position.normalized;

//		Debug.Log("height " + currentHeight);
//		Debug.Log("velocity " + velocity);
		if (!grounded) {
			transform.position += (Vector3)velocity;
			currentHeight = transform.position.y - groundPosition.y
			                + (currentPlatform != null ? currentPlatform.height : 0);
			if (!(transform.position.y - shadow.transform.position.y < 0.0001)) return;
			Debug.Log("is grounded.");
			grounded = true;
			isFalling = false;
			transform.position = shadow.transform.position;
			currentHeight = transform.position.y - groundPosition.y
			                + (currentPlatform != null ? currentPlatform.height : 0);
//				currentHeight = (float)Math.Round(currentHeight);
//				groundNormal = currentNormal;
//				currentNormal.x = 0;
		} else {
			transform.position = shadow.transform.position;
			velocity = Vector2.zero;
		}
//		if (Mathf.Abs(transform.position.y - shadow.transform.position.y) < 0.0001) {
//			Debug.Log("is grounded.");
//			grounded = true;
////			transform.position = shadow.transform.position;
////				groundNormal = currentNormal;
////				currentNormal.x = 0;
//		} else {
////				rb2d.position += velocity;
//			Debug.Log("is NOT grounded.");
//			grounded = false;
//		}
//		rb2d.position += velocity;

//		if (velocity.y > 0) {
//			rb2d.position += velocity;
//		}

//
//			float projection = Vector2.Dot(velocity, currentNormal);
//			if (projection < 0) {
//				velocity = velocity - projection * currentNormal;
//			}
//
//			distance = transform.position.y - shadow.transform.position.y;
//		}
//
//		if (velocity.y < 0 && !grounded) { //If have y movement and not grounded, character is falling
//			isFalling = true;
//		} else{
//			isFalling = false;
//		}
//		rb2d.position = rb2d.position + moving.normalized * distance;
	}

	private void Jump() {
		if (Input.GetButtonDown("Jump") && grounded) { //if jump button is pressed and is grounded
			Debug.Log("jumping");
			grounded = false;
			jumping = true;
			velocity.y = jumpSpeed;
		}
	}

//	private void Jump() {
////		Debug.Log("falling? " + isFalling);
////		if (Input.GetButtonDown("Jump") && !isFalling) {
////			//groundHeight += 4;
////			if (grounded) {
////				//groundPosition = transform.position;
////				rising = true;
////				jumping = true;
////				jumpingRoutine = StartCoroutine(Jumping());
////			}
////			grounded = transform.position == groundPosition;
////		}
//		if (Input.GetButtonDown("Jump") && grounded) { //if jump button is pressed and is grounded
//			Debug.Log("jumping");
//			grounded = false;
//			jumping = true;
//			velocity.y = jumpSpeed;
////			rb2d.position += velocity;
//		}
//	}
//	private IEnumerator Jumping() {
//		float previousHeight = currentHeight;
//		while (rising) {
//			if (transform.position.y >= jumpHeight.position.y) {
//				rising = false;
////				isFalling = true;
//			}
//			if (rising) {
//				//transform.Translate(Vector3.up * jumpSpeed * Time.smoothDeltaTime);
//				//Vector3 maxHeight = new Vector3(groundPosition.x, groundPosition.y + maxJumpHeight, groundPosition.z);
//				transform.position = Vector3.MoveTowards(transform.position, jumpHeight.position,
//						jumpSpeed * Time.deltaTime);
//				// Calculate current height when jumping
//				//jumpingHeight = transform.position.y - groundPosition.y;
//				//Vector3.Distance(transform.position, groundPosition);
//				currentHeight = previousHeight + transform.position.y - groundPosition.y;
//			}
//			yield return new WaitForEndOfFrame();
//		}
//		//yield return new WaitForSeconds(0.05f);
//		float beforeFallHeight = currentHeight;
//		//Debug.Log("Before height: " + beforeFallHeight);
//		while (!rising) {
//			//newCurrentHeight = currentHeight;
//			//yield return new WaitForSeconds(0.01f);
//			transform.position = Vector3.MoveTowards(transform.position, groundPosition,
//				jumpSpeed * Time.deltaTime);
//			//jumpingHeight = transform.position.y - groundPosition.y;
//			//Debug.Log(string.Format("\tcurr plat: {0}", currentPlatform != null ? currentPlatform.name : "no plat..."));
//			currentHeight = beforeFallHeight - (beforeFallHeight - (transform.position.y - groundPosition.y))
//			+ (currentPlatform != null ? currentPlatform.height : 0);
//			//Debug.Log(string.Format("curr : {0}", currentHeight));
//			//currentHeight = 0;
//			//Debug.Log("Calc falling from jump...");
//			//Debug.Log(string.Format("dist: {0}", (transform.position.y - groundPosition.y)));
//			if (transform.position == groundPosition) {
//				//Debug.Log("Finished jumping!");
//				grounded = true;
//				isFalling = false;
//				//StopCoroutine(jumpingRoutine);
//				break;
//			}
//			yield return new WaitForEndOfFrame();
//		}
//		jumping = false;
//		//currentHeight -= newCurrentHeight;
//	}
	
	public float GetJumpingHeight() {
		return jumpingHeight;
	}

	public float GetCurrentHeight() {
		return currentHeight;
	}

	public float GetTotalHeight() {
		return totalHeight;
	}

	private bool HigherThanPlatform(ObjectInfo platform) {
		//Debug.Log(string.Format("plat height: {0}", hit.transform.GetComponent<ObjectInfo>().height));
		//Debug.Log("current jh: " + jumpingHeight);
		return currentHeight >= platform.height;
	}

//	private bool CheckIfInPlatform() {
//		var shadowPos = shadow.transform.position;
//		var inPlat = nextPlatform != null && shadowPos.x >= nextPlatform.leftBound && shadowPos.x <= nextPlatform.rightBound
//			&& shadowPos.y >= nextPlatform.bottomBound && shadowPos.y <= nextPlatform.topBound;
//		//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
//		//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
//		//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
//		//inPlat));
//		//Debug.Log("in plat: " + inPlat);
//		return inPlat;
//	}

	//private bool CheckIfInNewPlatform() {
	//	bool inPlat = nextPlatform != null && shadow.transform.position.x >= nextPlatform.leftBound && shadow.transform.position.x <= nextPlatform.rightBound
	//		&& shadow.transform.position.y >= nextPlatform.bottomBound && shadow.transform.position.y <= nextPlatform.topBound;
	//	//Debug.Log(string.Format("sl:{0} cl:{1}\nsr:{2} cr:{3}\nsb:{4} cb:{5}\nst:{6} ct:{7}\nin: {8}",
	//	//shadow.transform.position.x, currentPlatform.leftBound, shadow.transform.position.x, currentPlatform.rightBound,
	//	//shadow.transform.position.y, currentPlatform.bottomBound, shadow.transform.position.y, currentPlatform.topBound,
	//	//inPlat));
	//	//Debug.Log("in plat: " + inPlat);
	//	return inPlat;
	//}

	private static bool CheckIfInCollider(Vector3 myPosition, ObjectInfo platform) {
		return myPosition.x >= platform.leftBound && myPosition.x <= platform.rightBound 
		       && myPosition.y >= platform.bottomBound && myPosition.y <= platform.topBound;
	}

	public ObjectInfo GetCurrentPlatform() {
		return currentPlatform;
	}

	private IEnumerator RaisePlayerObjects() {
		//Debug.Log("shadow bb4: " + shadow.transform.position);
		transitionToCurrentPlatform = true;
		Debug.Log("Raising objects...");
		Debug.Log("grounded: " + grounded);
		var shadPos = shadow.transform.position;
//		var newJumpPos = jumpHeight.position + new Vector3(0, Mathf.Round(currentPlatform.height*2), 0);
		switch (facingDirection) {
			case Direction.Up:
				//shadow.transform.position += new Vector3(0, currentPlatform.height + 4 + (boundCorrection * 2), 0);
				shadow.transform.position += new Vector3(0, currentPlatform.height, 0);
				groundPosition += new Vector3(0, currentPlatform.height, 0);
//				Debug.Log("jh b4: " + jumpHeight.position);
				yield return new WaitUntil(() => !rising);
//				Debug.Log("jh: " + jumpHeight.position);
				jumpHeight.position += new Vector3(0, currentPlatform.height, 0);
				break;
			case Direction.Down:
				var platformInfo = pResultsDown[0].transform.GetComponent<ObjectInfo>();
				Debug.Log("top bound: " + platformInfo.topBound);

				shadow.transform.position = new Vector3(shadPos.x,
				                                        platformInfo.topBound - (boundCorrection * 2), shadPos.z);
				groundPosition = new Vector3(shadPos.x,
				                             platformInfo.topBound - (boundCorrection * 2), shadPos.z);
				yield return new WaitUntil(() => grounded);
				Debug.Log("Finished raising when facing down");
				jumpHeight.position = new Vector3(jumpHeight.position.x,
				                                            platformInfo.topBound - (boundCorrection * 2), jumpHeight.position.z);
				break;
			case Direction.Left:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
				groundPosition += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), currentPlatform.height, 0);
				break;
			case Direction.Right:
				shadow.transform.position += new Vector3(boundCorrection * 2, nextPlatform.height, 0);
				groundPosition += new Vector3(boundCorrection * 2, nextPlatform.height, 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, nextPlatform.height, 0);
				break;
			case Direction.UpLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), nextPlatform.height + (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), nextPlatform.height + (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), nextPlatform.height + (boundCorrection * 2), 0);
				break;
			case Direction.UpRight:
				shadow.transform.position += new Vector3(boundCorrection * 2, nextPlatform.height + (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, nextPlatform.height + (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, nextPlatform.height + (boundCorrection * 2), 0);
				break;
			case Direction.DownLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), nextPlatform.height - (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), nextPlatform.height - (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(-(boundCorrection * 2), nextPlatform.height - (boundCorrection * 2), 0);
				break;
			case Direction.DownRight:
				shadow.transform.position += new Vector3(boundCorrection * 2, nextPlatform.height - (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, nextPlatform.height - (boundCorrection * 2), 0);
				yield return new WaitUntil(() => grounded);
				jumpHeight.transform.position += new Vector3(boundCorrection * 2, nextPlatform.height - (boundCorrection * 2), 0);
				break;
			case Direction.Null:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		transitionToCurrentPlatform = false;
		//Debug.Log("shadow after: " + shadow.transform.position);
	}



//	IEnumerator Fall() {
//		Debug.Log(string.Format("Falling {0}h", fallingHeight));
//		isFalling = true;
//		nextPlatform = null;
//		CheckForLowerPlatforms();
//		fallingOffPlatform = true;
//		//shadow.transform.position = new Vector3(shadow.transform.position.x, shadow.transform.position.y - fallingHeight,
//		//shadow.transform.position.z);
//		// Check which way to pad landing to prevent getting stuck
//		switch (fallingDirection) {
//			case Direction.Up:
//				shadow.transform.position += new Vector3(0, -fallingHeight + 0.5f, 0);
//				groundPosition += new Vector3(0, -fallingHeight + 0.5f, 0);
//				yield return new WaitUntil(() => grounded);
//				jumpHeight.position += new Vector3(0, -fallingHeight + 0.5f, 0);
//				//shadow.transform.position += new Vector3(0, -fallingHeight + (boundCorrection), 0);
//				//jumpHeight.transform.position += new Vector3(0, -fallingHeight + (boundCorrection), 0);
//				//groundPosition += new Vector3(0, -fallingHeight + (boundCorrection), 0);
//				break;
//			case Direction.Down:
//				if (!jumping) {
//					//Debug.Log("shadow bb4: " + shadow.transform.position);
//					shadow.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
//					//Debug.Log("shadow after: " + shadow.transform.position);
//					jumpHeight.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
//					groundPosition += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
//				}
//				//Debug.Log("Jumping:\tshadow pos: " + shadow.transform.position);
//				//shadow.transform.position += new Vector3(0, currentHeight-fallingHeight - (boundCorrection * 2), 0);
//				//Debug.Log("\tshadow after: " + shadow.transform.position);
//				//groundPosition += new Vector3(0, currentHeight-fallingHeight - (boundCorrection * 2), 0);
//				//yield return new WaitUntil(() => grounded);
//				//jumpHeight.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
//				break;
//			case Direction.Left:
//				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
//				jumpHeight.position += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
//				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
//				break;
//			case Direction.Right:
//				shadow.transform.position += new Vector3(boundCorrection * 2, -fallingHeight, 0);
//				jumpHeight.position += new Vector3(boundCorrection * 2, -fallingHeight, 0);
//				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight, 0);
//				break;
//			case Direction.UpLeft:
//				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
//				jumpHeight.position += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
//				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
//				break;
//			case Direction.UpRight:
//				shadow.transform.position += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
//				jumpHeight.position += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
//				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
//				break;
//			case Direction.DownLeft:
//				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
//				jumpHeight.position += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
//				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
//				break;
//			case Direction.DownRight:
//				shadow.transform.position += new Vector3(boundCorrection * 2 - fallingHeight - (boundCorrection * 2), 0);
//				jumpHeight.position += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
//				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
//				break;
//		}
//		//Vector3 jumpHeightPos = jumpHeight.transform.position + new Vector3(0,-fallingHeight,0);
//		//Vector3 newPlayerPos = new Vector3(shadow.transform.position.x, shadow.transform.position.y + 2,
//		//shadow.transform.position.z);
//		//Fall to the shadows position
//
//		//Debug.Log("perparing for fall");
//		float beforeFallHeight = currentHeight;
//		while (true) {
//			transform.position = Vector3.MoveTowards(transform.position, groundPosition, jumpSpeed * Time.deltaTime);
//			//Mathf.Lerp(currentHeight, currentHeight - fallingHeight, jumpSpeed / 1.5f * Time.deltaTime);
//			currentHeight = beforeFallHeight - (beforeFallHeight - (transform.position.y - groundPosition.y));
//			//+ (currentPlatform != null ? currentPlatform.height : 0);
//			if (transform.position == groundPosition) {
//				currentHeight = beforeFallHeight - (beforeFallHeight - (transform.position.y - groundPosition.y));
//				//Debug.Log("finished fall.");
//				grounded = true;
//				isFalling = false;
//				if (fallingRoutine != null) {
//					fallingDirection = new Direction();
//					fallingOffPlatform = false;
//					StopCoroutine(fallingRoutine);
//				} else {
//					break;
//				}
//			}
//			yield return new WaitForEndOfFrame();
//		}
//		yield return new WaitForEndOfFrame();
//	}

	private void Fall() {
		Debug.Log(string.Format("Falling {0}h", fallingHeight));
		grounded = false;
		isFalling = true;
		nextPlatform = null;
		fallingOffPlatform = true;
		//shadow.transform.position = new Vector3(shadow.transform.position.x, shadow.transform.position.y - fallingHeight,
		//shadow.transform.position.z);
		// Check which way to pad landing to prevent getting stuck
		switch (fallingDirection) {
			case Direction.Up:
				shadow.transform.position += new Vector3(0, -fallingHeight + 0.5f, 0);
				groundPosition += new Vector3(0, -fallingHeight + 0.5f, 0);
				//shadow.transform.position += new Vector3(0, -fallingHeight + (boundCorrection), 0);
				//jumpHeight.transform.position += new Vector3(0, -fallingHeight + (boundCorrection), 0);
				//groundPosition += new Vector3(0, -fallingHeight + (boundCorrection), 0);
				break;
			case Direction.Down:
				if (!jumping) {
					//Debug.Log("shadow bb4: " + shadow.transform.position);
					shadow.transform.position += new Vector3(0, -fallingHeight - (boundCorrection * 2), 0);
					//Debug.Log("shadow after: " + shadow.transform.position);
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
				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight, 0);
				break;
			case Direction.Right:
				shadow.transform.position += new Vector3(boundCorrection * 2, -fallingHeight, 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight, 0);
				break;
			case Direction.UpLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight + (boundCorrection * 2), 0);
				break;
			case Direction.UpRight:
				shadow.transform.position += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight + (boundCorrection * 2), 0);
				break;
			case Direction.DownLeft:
				shadow.transform.position += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
				groundPosition += new Vector3(-(boundCorrection * 2), -fallingHeight - (boundCorrection * 2), 0);
				break;
			case Direction.DownRight:
				shadow.transform.position += new Vector3(boundCorrection * 2 - fallingHeight - (boundCorrection * 2), 0);
				groundPosition += new Vector3(boundCorrection * 2, -fallingHeight - (boundCorrection * 2), 0);
				break;
			case Direction.Null:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}


//	private void OnCollisionEnter2D(Collision2D coll) {
//		if (coll.gameObject.CompareTag("Platform")) {
//			ObjectInfo plat = coll.gameObject.GetComponent<ObjectInfo>();
//			
//		}
//	}

	/// <summary>
	///	Check the height of the floor that the player is trying to fall down to
	/// </summary>
	/// <returns>Height to displace current height by</returns>
	private float CheckForLowerPlatforms() {
		if (shadow.GetComponent<Collider2D>().Raycast(Vector2.down, floorContactFilter, floorHits, Mathf.Infinity) > 0) {
			return previousPlatform.height - floorHits[0].transform.GetComponent<ObjectInfo>().height;
		}
		return previousPlatform.height;
	}

//	if(nextPlatform != null && CheckIfInCollider(transform.position, nextPlatform) && isWalking
//	&& HigherThanPlatform(nextPlatform) && currentPlatform != nextPlatform) {
	private void CheckPlatformCollision() {
		//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
		//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
		//resultsLeft[0].distance));
		switch (facingDirection) {
			case Direction.Up:
				if (!transitionToCurrentPlatform) {
					var upHits = Physics2D.RaycastNonAlloc(rb2d.transform.position, Vector2.up, pResultsUp,
					                                       Mathf.Infinity, platformMask);
					currentPlatformHit =
						Physics2D.Raycast(rb2d.transform.position, Vector2.up, Mathf.Infinity, platformMask);
					if (upHits > 0) {
						if (upHits > 1) {
							if (currentPlatformHit.collider != null) {
//								Debug.Log("up isnt null");								
								if (pResultsUp[1].transform != currentPlatformHit.transform &&
								    pResultsUp[1].distance <= minPlatformDistance) {
//									Debug.Log("1 normal: " + resultsUp[1].point);
									nextPlatformHit = pResultsUp[1];
									minPlatformDistance = nextPlatformHit.distance;
								} else {
//									Debug.Log("0 normal: " + resultsUp[0].point);
									nextPlatformHit = pResultsUp[0];
									minPlatformDistance = nextPlatformHit.distance;
								}

								previousPlatform = currentPlatform;
							} else {
								// If current is null, nextbase is the closest one
//								Debug.Log("null normal: " + resultsUp[0].point);
//					currentPlatformHit = pResultsUp[0];
								nextPlatformHit = currentPlatformHit;
								minPlatformDistance = nextPlatformHit.distance;
							}
						} else {
//							Debug.Log("1== normal: " + resultsUp[0].point);
//				currentPlatformHit = pResultsUp[0];
							nextPlatformHit = currentPlatformHit;
							minPlatformDistance = nextPlatformHit.distance;
						}

						// Checks the values from the next platform
						//Debug.Log("bnext " + nextPlatformHit.transform.name);
						nextPlatform = nextPlatformHit.transform.GetComponent<ObjectInfo>();
						// If player is inside the platform, is moving into it, is higher than the platform, and the platform isnt what we're on
						// Transition to the new platform
						// Return if something is null
//						if (nextPlatform == null || !CheckIfInCollider(transform.position, nextPlatform) ||
//						    !isWalking || !HigherThanPlatform(nextPlatform) || currentPlatform == nextPlatform) return;
//						previousPlatform = nextPlatform;
						if (nextPlatform != null && CheckIfInCollider(transform.position, nextPlatform) && isWalking
						    && HigherThanPlatform(nextPlatform) && currentPlatform != nextPlatform) {
							Debug.Log(string.Format("Inside {0}", nextPlatform.name));
							//currentPlatformHit = nextPlatformHit;
							currentPlatform = currentPlatformHit.transform.GetComponent<ObjectInfo>();
							previousPlatform = currentPlatform;
							//StopCoroutine(jumpingRoutine);
							//grounded = true;
							//isFalling = false;
							//Debug.Log("c h: " + currentHeight);
							isOnPlatform = true;
							leftCurrentPlatform = false;
							landingRoutine = StartCoroutine(RaisePlayerObjects()); // happens multiple times?
						}

//						Debug.Log(string.Format("prev {0}", previousPlatform.name));
					} else {
						currentPlatformHit = new RaycastHit2D();
						currentPlatform = null;
//						previousPlatform = null;
						minPlatformDistance = Mathf.Infinity;
						Debug.Log("No platforms. Reset.");
					}
				}
//				Debug.Log("c h: " + currentHeight);
				int downHits = Physics2D.RaycastNonAlloc(shadow.GetComponent<Rigidbody2D>().transform.position,
				                                         Vector2.down, pResultsDown, Mathf.Infinity, platformMask);
				// Check for falling when facing up
				//if(shadow.GetComponent<Rigidbody2D>().Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
				if (downHits > 0) {
//					Debug.Log(string.Format("next, prev: {0}, {1}", nextPlatform.bottomBound, previousPlatform.topBound));
					
					//Debug.Log("p res down from shadow");
					//Debug.Log("prev " + previousPlatform.transform.name);
					// Block all upward directions to prevent sliding into walls
					//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
					//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
					if (previousPlatform != null
					    && !CheckIfInCollider(shadow.GetComponent<Rigidbody2D>().transform.position,  previousPlatform) 
					    && isWalking && HigherThanPlatform(previousPlatform)
					    && Mathf.RoundToInt(nextPlatform.bottomBound) != Mathf.RoundToInt(previousPlatform.topBound) ) {
						Debug.Log(string.Format("next, prev: {0}, {1}", nextPlatform.bottomBound, previousPlatform.topBound));
						//if(previousPlatform != null && !CheckIfInCollider(transform.position, previousPlatform) && isWalking && HigherThanPlatform(previousPlatform)) {
						//&& (currentPlatform == nextPlatform || currentPlatform == null)) {
						//Debug.Log(string.Format("Leaving {0} while facing {1}", currentPlatform.name, facingDirection));
						//currentPlatform = null;
						isOnPlatform = false;
						if (!leftCurrentPlatform) {
							fallingHeight = CheckForLowerPlatforms();
							transitionToCurrentPlatform = false;
							leftCurrentPlatform = true;
//							currentPlatform = null;
							//Debug.Log("Leaving...");
							fallingDirection = Direction.Up;
							Fall();
						}
					}
				}

				break;
			
			//TODO: Facing down
			//case Direction.Down:
			//	// Check if want to go ontop of a platform when facing down
			//	if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
			//		//Debug.Log(string.Format("hit coll here: {0}", pResultsDown[0].point));
			//		// Block all upward directions to prevent sliding into walls
			//		nextPlatform = pResultsDown[0].transform.GetComponent<PlatformInfo>();
			//		//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
			//		//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
			//		if (CheckIfInCollider(shadow.transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true) && !transitionToCurrentPlatform) {
			//			Debug.Log(string.Format("Down: Inside {0}", nextPlatform.name));
			//			//currentHeight += currentPlatform.height;
			//			currentPlatform = nextPlatform;
			//			transitionToCurrentPlatform = true;
			//			isOnPlatform = true;
			//			leftCurrentPlatform = false;
			//			landingRoutine = StartCoroutine(RaisePlayerObjects());
			//		}
			//	}
			//	// Check for falling when facing down
			//	if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0) {
			//		//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
			//		// Block all upward directions to prevent sliding into walls
			//		PlatformInfo leavingPlat = pResultsUp[0].transform.GetComponent<PlatformInfo>();
			//		//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
			//		//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
			//		if (!CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true)
			//			&& leavingPlat == nextPlatform && transitionToCurrentPlatform) {
			//			Debug.Log(string.Format("Leaving {0} while facing {1}", nextPlatform.name, facingDirection));
			//			//currentPlatform = null;
			//			isOnPlatform = false;
			//			fallingHeight = leavingPlat.height;
			//			if (!leftCurrentPlatform) {
			//				isFalling = true;
			//				grounded = false;
			//				transitionToCurrentPlatform = false;
			//				leftCurrentPlatform = true;
			//				//Debug.Log("Leaving...");
			//				//LowerHeight(fallingHeight);
			//				fallingDirection = Direction.Down;
			//				fallingRoutine = StartCoroutine(Fall());
			//			}
			//		}
			//	}
			//	break;
			//case Direction.Right:
			//	if (rb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity) > 0) {
			//		// Block all upward directions to prevent sliding into walls
			//		nextPlatform = pResultsRight[0].transform.GetComponent<PlatformInfo>();
			//		//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
			//		//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
			//		if (CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true) && !transitionToCurrentPlatform) {
			//			Debug.Log(string.Format("Facing right - Inside {0}", nextPlatform.name));
			//			currentPlatform = nextPlatform;
			//			transitionToCurrentPlatform = true;
			//			//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
			//			//StopCoroutine(jumpingRoutine);
			//			//currentHeight = currentPlatform.height;
			//			//grounded = true;
			//			//isFalling = false;
			//			//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
			//			Debug.Log("c h: " + currentHeight);
			//			isOnPlatform = true;
			//			leftCurrentPlatform = false;
			//			landingRoutine = StartCoroutine(RaisePlayerObjects());
			//		}
			//	}
			//	// Check for falling when facing up
			//	if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity) > 0) {
			//		//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
			//		// Block all upward directions to prevent sliding into walls
			//		PlatformInfo leavingPlat = pResultsLeft[0].transform.GetComponent<PlatformInfo>();
			//		//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
			//		//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
			//		if (!CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true)
			//		&& leavingPlat == nextPlatform && transitionToCurrentPlatform) {
			//			Debug.Log(string.Format("Leaving {0} while facing {1}", nextPlatform.name, facingDirection));
			//			//currentPlatform = null;
			//			isOnPlatform = false;
			//			fallingHeight = leavingPlat.height;
			//			if (!leftCurrentPlatform) {
			//				transitionToCurrentPlatform = false;
			//				leftCurrentPlatform = true;
			//				//Debug.Log("Leaving...");
			//				//LowerHeight(fallingHeight);
			//				fallingDirection = Direction.Right;
			//				fallingRoutine = StartCoroutine(Fall());
			//			}
			//		}
			//	}
			//	break;
			//case Direction.Left:
			//	pResultsLeft = Physics2D.RaycastAll(rb2d.transform.position, Vector2.left, Mathf.Infinity, platformMask);
			//	if (pResultsLeft.Length > 0) {
			//		//if (rb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity) > 0) {
			//		// Block all upward directions to prevent sliding into walls
			//		//nextPlatform = pResultsLeft[0].transform.GetComponent<PlatformInfo>();
			//		//nextPlatform = pResultsLeft.Length > 1 ? pResultsLeft[1].transform.GetComponent<PlatformInfo>() : null;
			//		for (int i = 0; i < pResultsLeft.Length; i++) {
			//			PlatformInfo checkPlat = pResultsLeft[i].transform.GetComponent<PlatformInfo>();
			//			//Debug.Log("check name: " + checkPlat.name);
			//			// If check is null
			//			//if(currentPlatform == null) {
			//			//	currentPlatform = checkPlat;
			//			//} else {
			//			//	nextPlatform = !checkPlat.name.Equals(currentPlatform.name) ? checkPlat : null;
			//			//}
			//			if (currentPlatform != null) {
			//				if (checkPlat != currentPlatform) {
			//					nextPlatform = checkPlat;
			//					break;
			//				}
			//				//nextPlatform = !checkPlat.name.Equals(currentPlatform.name) ? checkPlat : nextPlatform;
			//			} else {
			//				nextPlatform = checkPlat;
			//				break;
			//			}
			//			//nextPlatform = currentPlatform != null ?
			//			//(!checkPlat.name.Equals(currentPlatform.name) ? checkPlat : null) 
			//			//: checkPlat;
			//		}
			//		if (pResultsLeft.Length == 1) {
			//			nextPlatform = null;
			//		}
			//		//Debug.Log("[0]: " + pResultsLeft[0].distance + ", [1]: " + (pResultsLeft.Length > 1 ? pResultsLeft[1].distance : -1));
			//		//Debug.Log(string.Format("curr: {0}, next: {1}", currentPlatform != null ? currentPlatform.name : "No current plat", nextPlatform != null ? nextPlatform.name : "No next plat"));
			//		//if (CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true) 
			//		//&& !transitionToCurrentPlatform && !isOnPlatform) {
			//		//Debug.Log(string.Format("Facing left - Inside {0}", nextPlatform.name));
			//		if (nextPlatform != null && CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true)
			//			&& jumping) {
			//			//&& !transitionToCurrentPlatform && !isOnPlatform) {
			//			Debug.Log(string.Format("Facing left - Inside {0}", nextPlatform.name));
			//			currentPlatform = nextPlatform;
			//			transitionToCurrentPlatform = true;
			//			//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
			//			//StopCoroutine(jumpingRoutine);
			//			//currentHeight = currentPlatform.height;
			//			//grounded = true;
			//			//isFalling = false;
			//			//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
			//			Debug.Log("c h[0]: " + currentHeight);
			//			isOnPlatform = true;
			//			leftCurrentPlatform = false;
			//			landingRoutine = StartCoroutine(RaisePlayerObjects());
			//		}
			//		//else if (CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(nextPlatform, true)
			//		//	&& jumping && isOnPlatform) {
			//		//	currentPlatform = nextPlatform;
			//		//	Debug.Log(string.Format("Facing left - Inside next plat: {0}\n{1}", nextPlatform.name, nextPlatform.height));
			//		//	transitionToCurrentPlatform = true;
			//		//	//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
			//		//	//StopCoroutine(jumpingRoutine);
			//		//	//currentHeight = currentPlatform.height;
			//		//	//grounded = true;
			//		//	//isFalling = false;
			//		//	//currentHeight = Mathf.Round(currentHeight + currentPlatform.height);
			//		//	Debug.Log("c h[1]: " + currentHeight);
			//		//	isOnPlatform = true;
			//		//	leftCurrentPlatform = false;
			//		//	landingRoutine = StartCoroutine(RaisePlayerObjects());
			//		//}
			//	}
			//	// Check for falling when facing left
			//	//if (shadow.GetComponent<Rigidbody2D>().Cast(Vector2.left, platformContactFilter, pResultsRight, Mathf.Infinity) > 0) {
			//	//	//if (rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity) > 0) {
			//	//	// Block all upward directions to prevent sliding into walls
			//	//	PlatformInfo leavingPlat = pResultsRight[0].transform.GetComponent<PlatformInfo>();
			//	//	//Debug.Log(string.Format("Touching platform {0}, bounds(<,^,v,>): ({1},{2},{3},{4})", pResultsUp[0].transform.name,
			//	//	//currentPlatform.leftBound, currentPlatform.topBound, currentPlatform.bottomBound, currentPlatform.rightBound));
			//	//	if (!CheckIfInCollider(transform.position, nextPlatform) && isWalking && CheckAgainstPlatformHeight(pResultsRight[0], true)
			//	//	&& leavingPlat == nextPlatform && transitionToCurrentPlatform) {
			//	//		Debug.Log(string.Format("Leaving {0} while facing {1}", nextPlatform.name, facingDirection));
			//	//		//currentPlatform = null;
			//	//		isOnPlatform = false;
			//	//		fallingHeight = leavingPlat.height;
			//	//		if (!leftCurrentPlatform) {
			//	//			transitionToCurrentPlatform = false;
			//	//			leftCurrentPlatform = true;
			//	//			//Debug.Log("Leaving...");
			//	//			//LowerHeight(fallingHeight);
			//	//			fallingDirection = Direction.Left;
			//	//			fallingRoutine = StartCoroutine(Fall());
			//	//		}
			//	//	}
			//	//}
			//	break;

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
				//		&& currentHeight <= resultsUp[0].transform.GetComponent<ObjectInfo>().height
				//		&& currentHeight <= resultsRight[0].transform.GetComponent<ObjectInfo>().height) {
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

	private void CorrectMovement(RaycastHit2D hit, Direction direction) {
		if (hit.distance < diagonalBoundCorrection - 0.25f) {
			MoveInDirection(direction, blockingCorrectionSpeed);
		}
	}

	// TODO: MovePlayer()
	private void MovePlayer() {
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
//			CheckPlatformCollision();
			// Facing up
			StartDirection(Direction.Up);
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
				//Debug.Log("\trying to block with shadow...");
			//}
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
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
			if (!isDirectionBlocked[(int)Direction.Down]
				&& !isDirectionBlocked[(int)Direction.DownLeft]
				&& !isDirectionBlocked[(int)Direction.DownRight]) {
				//Debug.Log("\t\tWALKING DOWN.");
				MoveInDirection(Direction.Down, overworldSpeed); 
			} else if (!isDirectionBlocked[(int)Direction.Down]
				&& isDirectionBlocked[(int)Direction.DownLeft]
				&& !isDirectionBlocked[(int)Direction.DownRight]) {
				MoveInDirection(Direction.DownRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]
				&& !isDirectionBlocked[(int)Direction.DownLeft]
				&& isDirectionBlocked[(int)Direction.DownRight]) {
				//Debug.Log("\t\tWALKING DOWN LEFT.");
				MoveInDirection(Direction.DownLeft, diagonalMovementSpeed);
			}
		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing right
			StartDirection(Direction.Right);
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
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
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
			if (!isDirectionBlocked[(int)Direction.Left]
				&& !isDirectionBlocked[(int)Direction.UpLeft]
				&& !isDirectionBlocked[(int)Direction.DownLeft]) {
				//Debug.Log("\t\tWALKING LEFT.");
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left]
				&& isDirectionBlocked[(int)Direction.UpLeft]
				&& !isDirectionBlocked[(int)Direction.DownLeft]) {
				//Debug.Log("\t\tWALKING DOWN LEFT.");
				MoveInDirection(Direction.DownLeft, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left]
				&& !isDirectionBlocked[(int)Direction.UpLeft]
				&& isDirectionBlocked[(int)Direction.DownLeft]) {
				MoveInDirection(Direction.UpLeft, diagonalMovementSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// TODO: Facing up-right
			StartDirection(Direction.UpRight);
			//if (grounded && !isFalling) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
			if (!isDirectionBlocked[(int)Direction.UpRight]) {
				//Debug.Log("Moving up right...");
				MoveInDirection(Direction.UpRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Up]) {
				CorrectMovement(nextUpRightBase, Direction.DownRight);
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				CorrectMovement(nextUpRightBase, Direction.UpLeft);
				MoveInDirection(Direction.Up, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// TODO: Facing up-left
			StartDirection(Direction.UpLeft);
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
			if (!isDirectionBlocked[(int)Direction.UpLeft]) {
				//transform.Translate(-0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpLeft, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up]) {
				CorrectMovement(nextUpLeftBase, Direction.DownLeft);
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				CorrectMovement(nextUpLeftBase, Direction.UpRight);
				MoveInDirection(Direction.Up, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing down-left
			StartDirection(Direction.DownLeft);
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
			if (!isDirectionBlocked[(int)Direction.DownLeft]) {
				//transform.Translate(-0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownLeft, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Down]) {
				CorrectMovement(nextDownLeftBase, Direction.UpLeft);
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]) {
				CorrectMovement(nextDownLeftBase, Direction.DownRight);
				MoveInDirection(Direction.Down, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing down-right
			StartDirection(Direction.DownRight);
			//if (grounded) {
			//	TryBlockDirections(rb2d);
			//} else {
				TryBlockDirections(shadow.GetComponent<Rigidbody2D>());
			//}
			if (!isDirectionBlocked[(int)Direction.DownRight]) {
				//transform.Translate(0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownRight, diagonalMovementSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Down]) {
				CorrectMovement(nextDownRightBase, Direction.UpLeft);
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Down]) {
				CorrectMovement(nextDownRightBase, Direction.DownLeft);
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
				CastUp(targetRb2d, baseMask,
					() => {
//						currentUpBase = nextUpBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//isDirectionBlocked[(int)Direction.UpLeft] = true;
						//isDirectionBlocked[(int)Direction.UpRight] = true;
					},
					() => {
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
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
				CastUpLeft(targetRb2d, baseMask,
					// try block up left
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block up from ul");
//						currentUpLeftBase = nextUpLeftBase;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
					},
					// try clear sides
					() => {
//						currentUpLeftBase = new RaycastHit2D();
//						currentDownLeftBase = new RaycastHit2D();
//						minUpLeftBaseDistance = Mathf.Infinity;
//						minDownLeftBaseDistance = Mathf.Infinity;
					}
				);
				CastUpRight(targetRb2d, baseMask,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block up from ur");
//						currentUpRightBase = nextUpRightBase;
						isDirectionBlocked[(int)Direction.UpRight] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					},
					// try clear sides
					() => {
//						currentUpRightBase = new RaycastHit2D();
//						currentDownRightBase = new RaycastHit2D();
//						minUpRightBaseDistance = Mathf.Infinity;
//						minDownRightBaseDistance = Mathf.Infinity;
					}
				);
				break;
			case Direction.Down:
				CastDown(targetRb2d, baseMask,
					() => {
//						currentDownBase = nextDownBase;
						isDirectionBlocked[(int)facingDirection] = true;
					},
					() => {
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					() => {
//						currentDownBase = new RaycastHit2D();
//						currentUpBase = new RaycastHit2D();
//						minDownBaseDistance = Mathf.Infinity;
//						minUpBaseDistance = Mathf.Infinity;   // Reset distance
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
				CastDownLeft(targetRb2d, baseMask,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block down from dl");
//						currentDownLeftBase = nextDownLeftBase;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
					},
					// try unblock up right
					() => {
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					},
					// try clear sides
					() => {
//						currentDownLeftBase = new RaycastHit2D();
//						currentUpLeftBase = new RaycastHit2D();
//						minDownLeftBaseDistance = Mathf.Infinity;
//						minUpLeftBaseDistance = Mathf.Infinity;
					}
				);
				CastDownRight(targetRb2d, baseMask,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block down from dr");
//						currentDownRightBase = nextDownRightBase;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					},
					// try unblock up right
					() => {
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					// try clear sides
					() => {
//						currentDownRightBase = new RaycastHit2D();
//						currentUpRightBase = new RaycastHit2D();
//						minDownRightBaseDistance = Mathf.Infinity;
//						minUpRightBaseDistance = Mathf.Infinity;
					}
				);
				break;
			case Direction.Right:
				CastRight(targetRb2d, baseMask,
					() => {
//						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//isDirectionBlocked[(int)Direction.UpRight] = true;
						//isDirectionBlocked[(int)Direction.DownRight] = true;
					},
					() => {
//						currentLeftBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					() => {
//						currentLeftBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
//						minRightBaseDistance = Mathf.Infinity;
//						currentRightBase = new RaycastHit2D();
					}
				);
				CastUpRight(targetRb2d, baseMask,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block up from ur");
//						currentUpRightBase = nextUpRightBase;
						isDirectionBlocked[(int)Direction.UpRight] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpRight] = false;
					},
					// try clear sides
					() => {
//						currentUpRightBase = new RaycastHit2D();
//						currentDownRightBase = new RaycastHit2D();
//						minUpRightBaseDistance = Mathf.Infinity;
//						minDownRightBaseDistance = Mathf.Infinity;
					}
				);
				CastDownRight(targetRb2d, baseMask,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block down from dr");
//						currentDownRightBase = nextDownRightBase;
						isDirectionBlocked[(int)Direction.DownRight] = true;
					},
					// try unblock up right
					() => {
						isDirectionBlocked[(int)Direction.DownRight] = false;
					},
					// try clear sides
					() => {
//						currentDownRightBase = new RaycastHit2D();
//						currentUpRightBase = new RaycastHit2D();
//						minDownRightBaseDistance = Mathf.Infinity;
//						minUpRightBaseDistance = Mathf.Infinity;
					}
				);
				break;
			case Direction.Left:
				CastLeft(targetRb2d, baseMask,
					() => {
//						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//isDirectionBlocked[(int)Direction.UpLeft] = true;
						//isDirectionBlocked[(int)Direction.DownLeft] = true;
					},
					() => {
						//Debug.Log("Unblockingu left");
//						currentRightBase = new RaycastHit2D();
//						minRightBaseDistance = Mathf.Infinity;  // Reset distance
						isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					},
					() => {
//						currentLeftBase = new RaycastHit2D();
//						currentRightBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;
//						minRightBaseDistance = Mathf.Infinity;  // Reset distances
					}
				);
				CastUpLeft(targetRb2d, baseMask,
					// try block up left
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block up from ul");
//						currentUpLeftBase = nextUpLeftBase;
						isDirectionBlocked[(int)Direction.UpLeft] = true;
					},
					// try unblock up right
					() => {
						//isDirectionBlocked[(int)facingDirection] = false;
						isDirectionBlocked[(int)Direction.UpLeft] = false;
					},
					// try clear sides
					() => {
//						currentUpLeftBase = new RaycastHit2D();
//						currentDownRightBase = new RaycastHit2D();
//						minUpLeftBaseDistance = Mathf.Infinity;
//						minDownRightBaseDistance = Mathf.Infinity;
					}
				);
				CastDownLeft(targetRb2d, baseMask,
					// try block up right
					() => {
						// If base is too close on left side, block up left, meaning up will movediag instead
						//Debug.Log("block down from dl");
//						currentDownLeftBase = nextDownLeftBase;
						isDirectionBlocked[(int)Direction.DownLeft] = true;
					},
					// try unblock up right
					() => {
						//currentDownLeftBase = new RaycastHit2D();
						isDirectionBlocked[(int)Direction.DownLeft] = false;
					},
					// try clear sides
					() => {
//						currentDownLeftBase = new RaycastHit2D();
//						currentUpRightBase = new RaycastHit2D();
//						minDownLeftBaseDistance = Mathf.Infinity;
//						minUpRightBaseDistance = Mathf.Infinity;
					}
				);
				//if (resultsLeft.Length > 0 && resultsUp.Length > 0 && resultsDown.Length > 0
				//	&& isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Down]
				//	//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
				//	&& currentHeight <= nextUpBase.transform.GetComponent<ObjectInfo>().height
				//	&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectInfo>().height) {
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
				CastUpRight(targetRb2d, baseMask,
					// try block up right
					() => {
						//Debug.Log("TRYING TO BBLOCK RIGHT UP");
//						currentUpRightBase = nextUpRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						//Debug.Log("unblocking up right...");
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
//						currentUpRightBase = new RaycastHit2D();
//						currentUpBase = new RaycastHit2D();
//						minUpRightBaseDistance = Mathf.Infinity;
//						minUpBaseDistance = Mathf.Infinity;
					}
				);
				CastUp(targetRb2d, baseMask,
					() => {
						//Debug.Log("UR: blocking up...");
//						currentUpBase = nextUpBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Up] = true;
					},
					() => {
//						currentDownBase = new RaycastHit2D();
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Right]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Up] = false;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);

				CastRight(targetRb2d, baseMask,
					// If blocked on the right, block right direction
					() => {
//						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Right] = true;
					},
					() => {
						//Debug.Log("RIGHT IS NOT BEING BLOCKED");
//						currentLeftBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Up]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Right] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
//						currentLeftBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
//						minRightBaseDistance = Mathf.Infinity;
//						currentRightBase = new RaycastHit2D();
					}
				);

				//Debug.Log("u:" + nextUpBase.point + ", right:" + nextRightBase.point);
				//Debug.Log("u, r: " + nextUpBase.distance + ", " + nextRightBase.distance);
				if (resultsUp.Length > 0 && resultsRight.Length > 0
					&& isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Right]
					//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
					&& currentHeight <= nextUpBase.transform.GetComponent<ObjectInfo>().height
					&& currentHeight <= nextRightBase.transform.GetComponent<ObjectInfo>().height) {
					//Debug.Log("\t\tMoving down...");
					//// If there is a diagonal block & is not a null block, free up the direction
					if (nextUpRightBase.collider != null && Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection
					&& nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) > boundCorrection) {
					//if (Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection&& Mathf.Abs(nextRightBase.distance) > boundCorrection) {
						//Debug.Log("UR: Moving right...");
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
				CastUpLeft(targetRb2d, baseMask,
					// try block up right
					() => {
						//Debug.Log("TRYING TO BBLOCK LEFT UP");
//						currentUpLeftBase = nextUpLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						//Debug.Log("unblocking up left...");
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
//						currentUpLeftBase = new RaycastHit2D();
//						currentUpBase = new RaycastHit2D();
//						minUpLeftBaseDistance = Mathf.Infinity;
//						minUpBaseDistance = Mathf.Infinity;
					}
				);
				CastUp(targetRb2d, baseMask,
					() => {
//						currentUpBase = nextUpBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Up] = true;
					},
					() => {
//						currentDownBase = new RaycastHit2D();
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Left]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Up] = false;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);

				CastLeft(targetRb2d, baseMask,
					// If blocked on the right, block right direction
					() => {
//						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Left] = true;
					},
					() => {
						//Debug.Log("LEFT IS NOT BEING BLOCKED");
//						currentRightBase = new RaycastHit2D();
//						minRightBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Up]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Left] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
						//Debug.Log("Clearing blcoks...");
//						currentLeftBase = new RaycastHit2D();
//						currentUpBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;
//						minRightBaseDistance = Mathf.Infinity;   // Reset distanc
					}
				);
				//Debug.Log("currentL: " + currentLeftBase.transform.name + ", nextL: " + nextLeftBase.transform.name);
				if (resultsUp.Length > 0 && resultsLeft.Length > 0
					&& isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Left]
					//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
					&& currentHeight <= nextUpBase.transform.GetComponent<ObjectInfo>().height
					&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectInfo>().height) {

					if (nextUpLeftBase.collider != null && Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection
					&& nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
					//if ( Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection
						//&& Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
						//Debug.Log("\t\tMoving left...");
						isDirectionBlocked[(int)Direction.Left] = false;
					}
				} 
				break;
			case Direction.DownRight:
				CastDownRight(targetRb2d, baseMask,
					// try block up right
					() => {
						//Debug.Log("TRYING TO BBLOCK RIGHT DOWN");
//						currentDownRightBase = nextDownRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						//Debug.Log("unblocking down right...");
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
//						currentDownRightBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minDownRightBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;
					}
				);
				CastDown(targetRb2d, baseMask,
					() => {
//						currentDownBase = nextDownBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Down] = true;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Right]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Down] = false;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);

				CastRight(targetRb2d, baseMask,
					// If blocked on the right, block right direction
					() => {
//						currentRightBase = nextRightBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Right] = true;
					},
					() => {
						//Debug.Log("RIGHT IS NOT BEING BLOCKED");
//						currentLeftBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
																// Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Down]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Right] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
//						currentRightBase = new RaycastHit2D();
//						currentLeftBase = new RaycastHit2D();
//						minRightBaseDistance = Mathf.Infinity;
//						minLeftBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);
				//Debug.Log("u:" + nextUpBase.point + ", right:" + nextRightBase.point);
				//Debug.Log("u, r: " + nextUpBase.distance + ", " + nextRightBase.distance);
				if (resultsDown.Length > 0 && resultsRight.Length > 0
					&& isDirectionBlocked[(int)Direction.Down] && isDirectionBlocked[(int)Direction.Right]
					//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
					&& currentHeight <= nextDownBase.transform.GetComponent<ObjectInfo>().height
					&& currentHeight <= nextRightBase.transform.GetComponent<ObjectInfo>().height) {

					if (nextDownRightBase.collider != null && Mathf.Abs(nextDownRightBase.distance) < diagonalBoundCorrection
						&& nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) > boundCorrection) {
						//Debug.Log("\t\tMoving right...");
						isDirectionBlocked[(int)Direction.Right] = false;
					}
					//Debug.Log("\t\tMoving down...");
					//isDirectionBlocked[(int)Direction.Right] = false;
					//MoveInDirection(Direction.Down, 0.1f);
				}
				//if (resultsUp.Length > 0 && resultsLeft.Length > 0
				//	&& isDirectionBlocked[(int)Direction.Up] && isDirectionBlocked[(int)Direction.Left]
				//	//&& nextUpBase.transform == nextRightBase.transform	//TODO: Handle what happens when touch corner
				//	&& currentHeight <= nextUpBase.transform.GetComponent<ObjectInfo>().height
				//	&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectInfo>().height) {

				//	if (nextUpLeftBase.collider != null && Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection
				//		&& nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
				//		Debug.Log("\t\tMoving left...");
				//		isDirectionBlocked[(int)Direction.Left] = false;
				//	}
				//}
				break;
			// TODO: DownLeft()
			case Direction.DownLeft:
				CastDownLeft(targetRb2d, baseMask,
					// try block up right
					() => {
						//Debug.Log("TRYING TO BBLOCK LEFT DOWN");
//						currentDownLeftBase = nextDownLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						//if (nextUpBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Up] = true;
						//}
						//if (nextRightBase.collider != null) {
						//	isDirectionBlocked[(int)Direction.Right] = true;
						//}
					},
					// try unblock up right
					() => {
						//Debug.Log("unblocking down left...");
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
						//Debug.Log("clearing dl and down");
//						currentDownLeftBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minDownLeftBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;
					}
				);

				CastDown(targetRb2d, baseMask,
					() => {
//						currentDownBase = nextDownBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Down] = true;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;   // Reset distance
															  // Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Left]) {
						//	Debug.Log("LEFT IS NOT BEING BLOCKED");
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Down] = false;
					},
					() => {
//						currentUpBase = new RaycastHit2D();
//						currentDownBase = new RaycastHit2D();
//						minUpBaseDistance = Mathf.Infinity;
//						minDownBaseDistance = Mathf.Infinity;   // Reset distance
					}
				);
				// Perform left raycasts
				CastLeft(targetRb2d, baseMask,
					// If blocked on the right, block right direction
					() => {
//						currentLeftBase = nextLeftBase;
						isDirectionBlocked[(int)facingDirection] = true;
						isDirectionBlocked[(int)Direction.Left] = true;
					},
					() => {
//						currentRightBase = new RaycastHit2D();
//						minRightBaseDistance = Mathf.Infinity;   // Reset distance
																 // Only unblock the diagonal if not blocked my multiple things
						//if (!isDirectionBlocked[(int)Direction.Down]) {
						//	isDirectionBlocked[(int)facingDirection] = false;
						//}
						isDirectionBlocked[(int)Direction.Left] = false;
						//isDirectionBlocked[(int)Direction.Left] = false;
					},
					// Clear all blocks
					() => {
//						currentLeftBase = new RaycastHit2D();
//						currentUpBase = new RaycastHit2D();
//						minLeftBaseDistance = Mathf.Infinity;
//						minRightBaseDistance = Mathf.Infinity;   // Reset distanc
					}
				);
				//Debug.Log("u:" + nextUpBase.point + ", right:" + nextRightBase.point);
				//Debug.Log("u, r: " + nextUpBase.distance + ", " + nextRightBase.distance);
				if (resultsDown.Length > 0 && resultsLeft.Length > 0
					&& isDirectionBlocked[(int)Direction.Down] && isDirectionBlocked[(int)Direction.Left]
					//&& nextUpBase.transform == nextRightBase.transform	
					&& currentHeight <= nextDownBase.transform.GetComponent<ObjectInfo>().height
					&& currentHeight <= nextLeftBase.transform.GetComponent<ObjectInfo>().height) {
					if (nextDownLeftBase.collider != null && Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection
					&& nextLeftBase.collider != null && Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
					//if (Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection
					//&& Mathf.Abs(nextLeftBase.distance) > boundCorrection) {
						//Debug.Log("\t\tMoving left...");
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
		//Debug.Log("height: " + hit.transform.GetComponent<ObjectInfo>().height);
		return currentHeight < hit.transform.GetComponent<ObjectInfo>().height;
		//+ wallHits[0].collider.GetComponent<ObjectInfo>().height;
		//+ wallChecks[0].GetComponent<ObjectInfo>().height && !isFalling;
	}

	// TODO: MoveInDirection()
	private void MoveInDirection(Direction direction, float speed) {
		switch (direction) {
			case Direction.Up:
				transform.Translate(0, speed, 0);
				shadow.transform.Translate(0, speed, 0);
				jumpHeight.Translate(0, speed, 0);
				move.x = 0;
				move.y = speed;
				break;
			case Direction.Down:
				transform.Translate(0, -speed, 0);
				shadow.transform.Translate(0, -speed, 0);
				jumpHeight.Translate(0, -speed, 0);
				move.x = 0;
				move.y = -speed;
				break;
			case Direction.Left:
				transform.Translate(-speed, 0, 0);
				shadow.transform.Translate(-speed, 0, 0);
				jumpHeight.Translate(-speed, 0, 0);
				move.x = -speed;
				move.y = 0;
				break;
			case Direction.Right:
				transform.Translate(speed, 0, 0);
				shadow.transform.Translate(speed, 0, 0);
				jumpHeight.Translate(speed, 0, 0);
				move.x = speed;
				move.y = 0;
				break;
			case Direction.UpRight:
				transform.Translate(speed, speed, 0);
				shadow.transform.Translate(speed, speed, 0);
				jumpHeight.Translate(speed, speed, 0);
				move.x = speed;
				move.y = speed;
				break;
			case Direction.UpLeft:
				transform.Translate(-speed, speed, 0);
				shadow.transform.Translate(-speed, speed, 0);
				jumpHeight.Translate(-speed, speed, 0);
				move.x = -speed;
				move.y = speed;
				break;
			case Direction.DownRight:
				transform.Translate(speed, -speed, 0);
				shadow.transform.Translate(speed, -speed, 0);
				jumpHeight.Translate(speed, -speed, 0);
				move.x = speed;
				move.y = -speed;
				break;
			case Direction.DownLeft:
				transform.Translate(-speed, -speed, 0);
				shadow.transform.Translate(-speed, -speed, 0);
				jumpHeight.Translate(-speed, -speed, 0);
				move.x = -speed;
				move.y = -speed;
				break;
			default:
				transform.Translate(0, 0, 0);
				shadow.transform.Translate(0, 0, 0);
				jumpHeight.Translate(0, 0, 0);
				move = Vector2.zero;
				break;
		}
	}

	private void StartDirection(Direction direction) {
		isWalking = true;
		facingDirection = direction;
		animator.SetBool(IsWalkingAni, true);
		animator.SetInteger(DirectionAni, (int)facingDirection - 1);
		animator.SetTrigger(ChangeDirectionAni);
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

	// private void RayCastCheck(Rigidbody2D targetRb2d, int layermask, RaycastHit2D[] results, Vector2 direction,
	//     RaycastHit2D currentHit, RaycastHit2D nextHit, float minDistance,
	//     Action hits, Action noHits) {
	//     results = Physics2D.RaycastAll(targetRb2d.transform.position, direction, Mathf.Infinity, layermask);
	//     if (results.Length > 0) {
	//Debug.Log(string.Format("[0]: {0}, [1]:{1}", results[0].distance, results[0].transform.name));
	//// ...
	//if (results.Length == 1 || (currentHit.collider != null && !Contains(results, currentHit))) {
	//             nextHit = results[0];
	//             currentHit = new RaycastHit2D();
	//             minDistance = nextHit.distance;
	//         }  else  {
	//             for (int i = 0; i < results.Length; i++)  {
	//                 RaycastHit2D hit = results[i];
	//                 //Debug.Log("checking ^ base...: " + hit.transform.name);
	//                 if (currentHit.collider != null)  {
	//                     //Debug.Log("current up isnt null");
	//                     // If this hit isnt the same as the current one, and its distance is smaller,
	//                     // 		Set the nextBase
	//                     if (hit.transform != currentHit.transform && hit.distance < minDistance)  {
	//                         nextHit = hit;
	//                         minDistance = hit.distance;
	//                         //break;
	//                     }
	//                 }  else  {
	//                     //Debug.Log("current up is null. set next to be the check");
	//                     // If current is null, nextbase is the closest one
	//                     nextHit = hit;
	//                     minDistance = hit.distance;
	//                     break;
	//                 }
	//             }
	//}
	//Debug.Log(string.Format("next: {0}, {1}", nextHit.distance, nextHit.transform.name));
	//hits();
	//     }  else {
	//         noHits();
	//     }
	// }
	// TODO: Cast methods
	// TODO: Add side casts; for Up, need one ray at x=pos-boundCorrection & another at x=pos+boundCorrection
	//			if left.distance < right.distance || left.collider != null && right.collider == null
	//				move UpRight
	//			if left.distance > right.distance || left.collider == null && right.collider != null
	//				move UpLeft
	//			(...)
	// ReSharper disable once InconsistentNaming
	private void CastUp(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSides) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, Vector2.up, resultsUp,Mathf.Infinity, layermask);
		
		if (hits > 0) {
			if (hits > 1) {
				if (currentUpBase.collider != null) {
//					Debug.Log("up isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsUp[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsUp[1].transform.GetComponent<ObjectInfo>()));
					if (resultsUp[1].transform != currentUpBase.transform && resultsUp[1].distance <= minUpBaseDistance) {
//						Debug.Log("1 normal: " + resultsUp[1].point);
						nextUpBase = resultsUp[1];
						minUpBaseDistance = nextUpBase.distance;
					} else {
//						Debug.Log("0 normal: " + resultsUp[0].point);
						nextUpBase = resultsUp[0];
						minUpBaseDistance = nextUpBase.distance;
					}
				} else {
					// If current is null, nextbase is the closest one
//					Debug.Log("null normal: " + resultsUp[0].point);
					currentUpBase = resultsUp[0];
					nextUpBase = currentUpBase;
					minUpBaseDistance = nextUpBase.distance;
				}
			} else {
//				Debug.Log("1== normal: " + resultsUp[0].point);
				currentUpBase = resultsUp[0];
				nextUpBase = currentUpBase;
				minUpBaseDistance = nextUpBase.distance;
			}
//			Debug.Log("next up base: " + (nextUpBase.collider != null ? nextUpBase.transform.name : "no next up base"));
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, nextUpBase.distance + currentHeight, 0), Color.red);
//			if (nextUpBase.collider != null && Mathf.Abs(nextUpBase.distance + currentHeight) < boundCorrection && CheckIfBlockPlayerByHeight(nextUpBase)
//			    && fallingDirection != Direction.Up) {
			if (nextUpBase.collider != null) {
				var obj = nextUpBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if (Mathf.Abs(targetRb2d.position.y - obj.bottomBound - currentHeight) < boundCorrection && CheckIfBlockPlayerByHeight(nextUpBase) &&
				    fallingDirection != Direction.Up) {
//					Debug.Log("normal blocking up...");
					blockSide();
				} else {
					currentDownBase = new RaycastHit2D();
					minDownBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
//				Debug.Log("unblocking up...");
				unblockSide();
			}
		} else 	{
			//Debug.Log("No base detected ^");
			clearSides();
			currentUpBase = new RaycastHit2D();
			nextUpBase = new RaycastHit2D();
			minUpBaseDistance = Mathf.Infinity;
			currentDownBase = new RaycastHit2D();
			nextDownBase = new RaycastHit2D();
			minDownBaseDistance = Mathf.Infinity;
			isDirectionBlocked[(int) Direction.Down] = false;
			isDirectionBlocked[(int) Direction.Up] = false;
			//ClearBlocks();
		}
//		if(resultsUp.Length > 0) {
//			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
//			// ...
//			if(resultsUp.Length == 1 || (currentUpBase.collider != null && !Contains(resultsUp, currentUpBase))) {
//				nextUpBase = resultsUp[0];
//				currentUpBase = new RaycastHit2D();
//				minUpBaseDistance = nextUpBase.distance;
//			} else {
//				//for(int i = 0; i < resultsUp.Length; i++) {
//				//	RaycastHit2D hit = resultsUp[i];
//					//Debug.Log("checking ^ base...: " + hit.transform.name);
//				if(currentUpBase.collider != null) {
//					//Debug.Log("current up isnt null");
//					// If this hit isnt the same as the current one, and its distance is smaller,
//					// 		Set the nextBase
//					if(resultsUp[1].transform != currentUpBase.transform && resultsUp[1].distance < minUpBaseDistance) {
//						nextUpBase = resultsUp[1];
//						minUpBaseDistance = resultsUp[1].distance;
//						//break;
//					} else {
//						nextUpBase = resultsUp[0];
//						minUpBaseDistance = resultsUp[0].distance;
//					}
//				} else {
//					//Debug.Log("current up is null. set next to be the check");
//					// If current is null, nextbase is the closest one
//					nextUpBase = resultsUp[0];
//					minUpBaseDistance = resultsUp[0].distance;
//					//break;
//				}
//				//}
//			}
//			//Debug.Log("next up base: " + (nextUpBase.collider != null ? nextUpBase.transform.name : "no next up base"));
//			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, nextUpBase.distance, 0), Color.red);
//			if(nextUpBase.collider != null && Mathf.Abs(nextUpBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextUpBase)
//				&& fallingDirection != Direction.Up && !isOnPlatform) {
//				//Debug.Log("blocking up...");
//				blockSide();
//			} else {
//				//Debug.Log("unblocking up...");
//				unblockSide();
//			}
		//} else {
		//	Debug.Log("null hit");
		//}
	}

	// ReSharper disable once InconsistentNaming
	private void CastDown(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSides) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, Vector2.down, resultsDown,Mathf.Infinity, layermask);
				
		if (hits > 0) {
			if (hits > 1) {
				if (currentDownBase.collider != null) {
//					Debug.Log("up isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsDown[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsDown[1].transform.GetComponent<ObjectInfo>()));
					if (resultsDown[1].transform != currentDownBase.transform && resultsDown[1].distance <= minDownBaseDistance) {
//						Debug.Log("1 normal: " + resultsUp[1].point);
						nextDownBase = resultsDown[1];
						minDownBaseDistance = nextDownBase.distance;
					} else {
//						Debug.Log("0 normal: " + resultsUp[0].point);
						nextDownBase = resultsDown[0];
						minDownBaseDistance = nextDownBase.distance;
					}
				} else {
					// If current is null, nextbase is the closest one
//					Debug.Log("null normal: " + resultsUp[0].point);
					currentDownBase = resultsDown[0];
					nextDownBase = currentDownBase;
					minDownBaseDistance = nextDownBase.distance;
				}
			} else {
//				Debug.Log("1== normal: " + resultsUp[0].point);
				currentDownBase = resultsDown[0];
				nextDownBase = currentDownBase;
				minDownBaseDistance = nextDownBase.distance;
			}
			Debug.Log("next down base: " + nextDownBase.transform.name + ". curr p height=" + currentHeight);
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, -nextDownBase.distance, 0), Color.red);
//			if (nextUpBase.collider != null && Mathf.Abs(nextUpBase.distance + currentHeight) < boundCorrection && CheckIfBlockPlayerByHeight(nextUpBase)
//			    && fallingDirection != Direction.Up) {
			if (nextDownBase.collider != null) {
				var obj = nextDownBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if (Mathf.Abs(targetRb2d.position.y - obj.topBound) < boundCorrection && CheckIfBlockPlayerByHeight(nextDownBase) &&
				    fallingDirection != Direction.Down) {
//					Debug.Log("normal blocking down...");
					blockSide();
				} else {
//					Debug.Log("normal unblocking down...");
					currentUpBase = new RaycastHit2D();
					minUpBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
//				Debug.Log("unblocking down...");
				unblockSide();
			}
		} else 	{
			//Debug.Log("No base detected ^");
			clearSides();
			currentDownBase = new RaycastHit2D();
			nextDownBase = new RaycastHit2D();
			minDownBaseDistance = Mathf.Infinity;
			currentUpBase = new RaycastHit2D();
			nextUpBase = new RaycastHit2D();
			minUpBaseDistance = Mathf.Infinity;
			isDirectionBlocked[(int) Direction.Down] = false;
			isDirectionBlocked[(int) Direction.Up] = false;
			//ClearBlocks();
		}
		
		
//		if (hits > 0) {
//			if (hits > 1) {
//				if (currentDownBase.collider != null) {
//					//Debug.Log("current left isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsDown[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsDown[1].transform.GetComponent<ObjectInfo>()));
//					if (resultsDown[1].transform != currentDownBase.transform) {
//						nextDownBase = resultsDown[1];
//						minDownBaseDistance = resultsDown[1].distance;
//					} else {
//						nextDownBase = resultsDown[0];
//						minDownBaseDistance = resultsDown[0].distance;
//					}
//				} else {
//					//Debug.Log("current left is null");
//					// If current is null, nextbase is the closest one
//					currentDownBase = resultsDown[0];
//					minDownBaseDistance = resultsDown[0].distance;
//				}
//			} else {
//				currentDownBase = resultsDown[0];
//				minDownBaseDistance = resultsDown[0].distance;
//			}
//			//			Debug.Log("next left base: " + (nextLeftBase.collider != null ? nextLeftBase.transform.name : "no next left base"));
//			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, -nextDownBase.distance, 0), Color.red);
//			if (nextDownBase.collider != null && Mathf.Abs(nextDownBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextDownBase)
//			    && fallingDirection != Direction.Down) {
//				Debug.Log("blocking down...");
//				blockSide();
//			} else if (nextDownBase.collider != null) {
//				Debug.Log("unblocking down...");
//				unblockSide();
//			} else {
//				Debug.Log("null hit");
//			}
//
//		
//		//		if (resultsDown.Length > 0) {
////			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
////			// ...
////			if (resultsDown.Length == 1 || (currentDownBase.collider != null && !Contains(resultsDown, currentDownBase))) {
////				nextDownBase = resultsDown[0];
////				currentDownBase = new RaycastHit2D();
////				minDownBaseDistance = nextDownBase.distance;
////			} else {
////				//for (int i = 0; i < resultsDown.Length; i++) {
////				//	RaycastHit2D hit = resultsDown[i];
////					//Debug.Log("checking v base...: " + hit.transform.name);
////				if (currentDownBase.collider != null) {
////					//Debug.Log("current down isnt null");
////					// If this hit isnt the same as the current one, and its distance is smaller,
////					// 		Set the nextBase
////					if (resultsDown[1].transform != currentDownBase.transform && resultsDown[1].distance < minDownBaseDistance) {
////					nextDownBase = resultsDown[1];
////					minDownBaseDistance = resultsDown[1].distance;
////					//break;
////					} else {
////						nextDownBase = resultsDown[0];
////						minDownBaseDistance = resultsDown[0].distance;
////					}
////				} else {
////					//Debug.Log("current down is null. set next to be the check");
////					// If current is null, nextbase is the closest one
////					nextDownBase = resultsDown[0];
////					minDownBaseDistance = resultsDown[0].distance;
////					//break;
////				}
////				//}
////			}
////			//Debug.Log("nextbase: " + (nextDownBase.collider != null ? nextDownBase.transform.name : "no next base"));
////			Debug.DrawRay(targetRb2d.transform.position, new Vector3(0, -nextDownRightBase.distance, 0), Color.red);
////			if (nextDownBase.collider != null && Mathf.Abs(nextDownBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextDownBase)
////				&& fallingDirection != Direction.Down && !isOnPlatform) {
////				//Debug.Log("blocking down...");
////				blockSide();
////			} else {
////				//Debug.Log("unblocking down...");
////				unblockSide();
////			}
//			//} else {
//			//	Debug.Log("null hit");
//			//}
//		} else {
//			//Debug.Log("No base detected v");
//			clearSides();
//			currentDownBase = new RaycastHit2D();
//			nextDownBase = new RaycastHit2D();
//			currentUpBase = new RaycastHit2D();
//			nextUpBase = new RaycastHit2D();
//			isDirectionBlocked[(int)Direction.Down] = false;
//			isDirectionBlocked[(int)Direction.Up] = false;
//			//ClearBlocks();
//		}
	}

	// ReSharper disable once InconsistentNaming
	private void CastRight(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSide) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, Vector2.right, resultsRight,Mathf.Infinity, layermask);
		if (hits > 0) {
			if (hits > 1) {
				if (currentRightBase.collider != null) {
					//Debug.Log("current left isnt null");
					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsRight[0].transform.GetComponent<ObjectInfo>()));
					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsRight[1].transform.GetComponent<ObjectInfo>()));
					if (resultsRight[1].transform != currentRightBase.transform && resultsRight[1].distance <= minRightBaseDistance) {
						nextRightBase = resultsRight[1];
						minRightBaseDistance = nextRightBase.distance;
					} else {
						nextRightBase = resultsRight[0];
						minRightBaseDistance = nextRightBase.distance;
					}
				} else {
					//Debug.Log("current left is null");
					// If current is null, nextbase is the closest one
					currentRightBase = resultsRight[0];
					nextRightBase = currentRightBase;
					minRightBaseDistance = resultsRight[0].distance;
				}
			} else {
				currentRightBase = resultsRight[0];
				nextRightBase = currentRightBase;
				minRightBaseDistance = resultsRight[0].distance;
			}
	//			Debug.Log("next left base: " + (nextLeftBase.collider != null ? nextLeftBase.transform.name : "no next left base"));
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(nextRightBase.distance, 0, 0), Color.red);
			if (nextRightBase.collider != null) {
				var obj = nextRightBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if (Mathf.Abs(targetRb2d.position.x - obj.leftBound) < boundCorrection && CheckIfBlockPlayerByHeight(nextRightBase) &&
				    fallingDirection != Direction.Right) {
					Debug.Log("normal blocking right...");
					blockSide();
				} else {
					Debug.Log("normal unblocking right...");
					currentRightBase = new RaycastHit2D();
					minRightBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
				Debug.Log("unblocking right...");
				unblockSide();
			}
		} else {
			//Debug.Log("No base detected >");
			clearSide();
			currentRightBase = new RaycastHit2D();
			nextRightBase = new RaycastHit2D();
			minRightBaseDistance = Mathf.Infinity; // Reset distance
			currentLeftBase = new RaycastHit2D();
			nextLeftBase = new RaycastHit2D();
			minLeftBaseDistance = Mathf.Infinity; // Reset distance
			isDirectionBlocked[(int)Direction.Right] = false;
			isDirectionBlocked[(int)Direction.Left] = false;
			//isDirectionBlocked[(int)Direction.Down] = false;
			//isDirectionBlocked[(int)Direction.DownLeft] = false;
		}
//			if (nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextRightBase)
//				&& fallingDirection != Direction.Right) {
//				Debug.Log("blocking right...");
//				blockSide();
//			} else if (nextRightBase.collider != null) {
//				Debug.Log("unblocking right...");
//				unblockSide();
//			} else {
//				Debug.Log("null hit");
//			}
////			if (hits > 0) {
/// 
//				if (hits > 1) {
////					Debug.Log(string.Format("[0]: {0}, {1}", resultsRight[0].collider != null
////						                        ? resultsRight[0].transform.name : "NONE",resultsRight[0].fraction));
////					Debug.Log(string.Format("[1]: {0}, {1}", resultsRight[1].collider != null
////						                        ? resultsRight[1].transform.name : "NONE", resultsRight[1].fraction));
//				
//					if (currentRightBase.collider != null) {
//						if (resultsRight[1].transform != currentRightBase.transform) {
//							nextRightBase = resultsRight[1];
//							minRightBaseDistance = resultsRight[1].distance;
//						} else {
//							nextRightBase = resultsRight[0];
//							minRightBaseDistance = resultsRight[0].distance;
//						}
//					} else {
//						// If current is null, nextbase is the closest one
//						currentRightBase = resultsRight[0];
//						minRightBaseDistance = resultsRight[0].distance;
//					}
//				} else {
////					Debug.Log(string.Format("1 Hit - [0]: {0}, {1}", resultsRight[0].collider != null
////						                        ? resultsRight[0].transform.name : "NONE",resultsRight[0].fraction));
//					currentRightBase = resultsRight[0];
//					minRightBaseDistance = resultsRight[0].distance;
//				}
//			//Debug.Log("next right base: " + (nextRightBase.collider != null ? nextRightBase.transform.name : "no next right base"));
//			Debug.DrawRay(targetRb2d.transform.position, new Vector3(nextRightBase.distance, 0, 0), Color.red);
//			if (nextRightBase.collider != null && Mathf.Abs(nextRightBase.distance) < boundCorrection && CheckIfBlockPlayerByHeight(nextRightBase)
//				&& fallingDirection != Direction.Right) {
//				//Debug.Log("blocking right...");
//				blockSide();
//			} else if (nextRightBase.collider != null) {
//				//Debug.Log("unblocking right...");
//				unblockSide();
//			} else {
//				//Debug.Log("null hit");
//			}
	}

	// ReSharper disable once InconsistentNaming
	private void CastLeft(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSide) {
		var hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, Vector2.left,  resultsLeft,
		                                     Mathf.Infinity, layermask);
		if (hits > 0) {
			if (hits > 1) {
				if (currentLeftBase.collider != null) {
					//Debug.Log("current left isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsLeft[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsLeft[1].transform.GetComponent<ObjectInfo>()));
					if (resultsLeft[1].transform != currentLeftBase.transform && resultsLeft[1].distance <= minLeftBaseDistance) {
						nextLeftBase = resultsLeft[1];
						minLeftBaseDistance = nextLeftBase.distance;
					} else {
						nextLeftBase = resultsLeft[0];
						minLeftBaseDistance = nextLeftBase.distance;
					}
				} else {
					//Debug.Log("current left is null");
					// If current is null, nextbase is the closest one
					currentLeftBase = resultsLeft[0];
					nextLeftBase = currentLeftBase;
					minLeftBaseDistance = nextLeftBase.distance;
				}
			} else {
				currentLeftBase = resultsLeft[0];
				nextLeftBase = currentLeftBase;
				minLeftBaseDistance = currentLeftBase.distance;
			}
			Debug.Log("next left base: " + (nextLeftBase.collider != null ? nextLeftBase.transform.name : "no next left base"));
			Debug.DrawRay(targetRb2d.transform.position, new Vector3(-nextLeftBase.distance, 0, 0), Color.red);
			if (nextLeftBase.collider != null) {
				var obj = nextLeftBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if (Mathf.Abs(targetRb2d.position.x - obj.rightBound) < boundCorrection && CheckIfBlockPlayerByHeight(nextLeftBase) &&
				    fallingDirection != Direction.UpLeft) {
					Debug.Log("normal blocking left...");
					blockSide();
				} else {
					Debug.Log("normal unblocking left...");
					currentLeftBase = new RaycastHit2D();
					minLeftBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
				Debug.Log("unblocking left...");
				unblockSide();
			}
		} else {
			Debug.Log("No base detected <");
			clearSide();
			currentLeftBase = new RaycastHit2D();
			nextLeftBase = new RaycastHit2D();
			minLeftBaseDistance = Mathf.Infinity; // Reset distance
			currentRightBase = new RaycastHit2D();
			nextRightBase = new RaycastHit2D();
			minRightBaseDistance = Mathf.Infinity; // Reset distance
			isDirectionBlocked[(int)Direction.Right] = false;
			isDirectionBlocked[(int)Direction.Left] = false;
			//ClearBlocks();
		}
	}

	// ReSharper disable once InconsistentNaming
	private void CastUpRight(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSides) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, new Vector2(1,1), resultsUpRight, Mathf.Infinity, layermask);
		
		if (hits > 0) {
			if (hits > 1) {
				if (currentUpRightBase.collider != null) {
//					Debug.Log("up isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsDownLeft[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsDownLeft[1].transform.GetComponent<ObjectInfo>()));
					if (resultsUpRight[1].transform != currentUpRightBase.transform && resultsUpRight[1].distance <= minUpRightBaseDistance) {
//						Debug.Log("1 normal: " + resultsUp[1].point);
						nextUpRightBase = resultsUpRight[1];
						minUpRightBaseDistance = nextUpRightBase.distance;
					} else {
//						Debug.Log("0 normal: " + resultsUp[0].point);
						nextUpRightBase = resultsUpRight[0];
						minUpRightBaseDistance = nextUpRightBase.distance;
					}
				} else {
					// If current is null, nextbase is the closest one
//					Debug.Log("null normal: " + resultsUp[0].point);
					currentUpRightBase = resultsUpRight[0];
					nextUpRightBase = currentUpRightBase;
					minUpRightBaseDistance = nextUpRightBase.distance;
				}
			} else {
//				Debug.Log("1== normal: " + resultsUp[0].point);
				currentUpRightBase = resultsUpRight[0];
				nextUpRightBase = currentUpRightBase;
				minUpRightBaseDistance = nextUpRightBase.distance;
			}
//			Debug.Log("next up right base: " + nextUpRightBase.transform.name + ". curr p height=" + currentHeight);
			Debug.DrawRay(targetRb2d.transform.position,
			              Vector3.ClampMagnitude(new Vector3(nextUpRightBase.distance, nextUpRightBase.distance, 0), nextUpRightBase.distance)
			              , Color.red);
			if (nextUpRightBase.collider != null) {
				var obj = nextUpRightBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if ((
				    targetRb2d.position.x <= obj.rightBound && targetRb2d.position.x >= obj.leftBound 
				    && Mathf.Abs(targetRb2d.position.y - obj.bottomBound - currentHeight) < boundCorrection
				    || 
				    targetRb2d.position.y <= obj.topBound && targetRb2d.position.y >= obj.bottomBound + currentHeight
				    && Mathf.Abs(targetRb2d.position.x - obj.leftBound) < boundCorrection
				    )
				    && CheckIfBlockPlayerByHeight(nextUpRightBase) &&
				    fallingDirection != Direction.UpRight) {
//					Debug.Log("normal blocking up right...");
					blockSide();
				} else {
//					Debug.Log("normal unblocking up right...");
					currentDownLeftBase = new RaycastHit2D();
					minDownLeftBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
//				Debug.Log("unblocking up right...");
				unblockSide();
			}
		} else 	{
			//Debug.Log("No base detected ^");
			clearSides();
			currentUpRightBase = new RaycastHit2D();
			nextUpRightBase = new RaycastHit2D();
			currentDownLeftBase = new RaycastHit2D();
			nextDownLeftBase = new RaycastHit2D();
			minDownLeftBaseDistance = Mathf.Infinity;
			minUpRightBaseDistance = Mathf.Infinity;
			isDirectionBlocked[(int)Direction.DownLeft] = false;
			isDirectionBlocked[(int)Direction.UpRight] = false;
		}
		
//		if (resultsUpRight.Length > 0) {
//			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
//			// If only hitting a single object, this object is our next/current base; or update the current objects length
//			if (resultsUpRight.Length == 1 || (currentUpRightBase.collider != null && !Contains(resultsUpRight, currentUpRightBase))) {
//				nextUpRightBase = resultsUpRight[0];
//				currentUpRightBase = new RaycastHit2D();
//				minUpRightBaseDistance = nextUpRightBase.distance;
//			} else {
//				//for (int i = 0; i < resultsUpRight.Length; i++) {
//				//	RaycastHit2D hit = resultsUpRight[i];
//					//Debug.Log("checking ^> base...: " + hit.transform.name);
//				if (currentUpRightBase.collider != null) {
//					//Debug.Log("current upright isnt null");
//					// If this hit isnt the same as the current one, and its distance is smaller,
//					// 		Set the nextBase
//					if (resultsUpRight[1].transform != currentUpRightBase.transform && resultsUpRight[1].distance < minUpRightBaseDistance) {
//						//Debug.Log("next and current are different");
//						nextUpRightBase = resultsUpRight[1];
//						minUpRightBaseDistance = resultsUpRight[1].distance;
//					} else {
//						// Base is the same base. update its distance
//						nextUpRightBase = resultsUpRight[0];
//						minUpRightBaseDistance = resultsUpRight[0].distance;
//					}
//				} else {
//					//Debug.Log("current upright is null");
//					// If current is null, nextbase is the closest one
//					nextUpRightBase = resultsUpRight[0];
//					minUpRightBaseDistance = resultsUpRight[0].distance;
//					//break;
//				}
//				//}
//			}
//			//Debug.Log("next ur base: " + (nextUpRightBase.collider != null ? nextUpRightBase.transform.name : "no next ur base") + ": " + nextUpRightBase.distance);
//			Debug.DrawRay(targetRb2d.transform.position,
//				Vector3.ClampMagnitude(new Vector3(nextUpRightBase.distance, nextUpRightBase.distance, 0), nextUpRightBase.distance)
//				, Color.red);
//			//Debug.Log("is distance in bound: " + (Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection));
//			if (nextUpRightBase.collider != null && Mathf.Abs(nextUpRightBase.distance) <= diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextUpRightBase)
//				&& fallingDirection != Direction.UpRight) {
//				//Debug.Log("blocking upright...");
//				blockSide();
//			} else if (nextUpRightBase.collider != null) {
//			//&& ((nextUpBase.collider == null && nextRightBase.collider == null && Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection)
//			//|| (nextUpRightBase.distance < nextUpBase.distance
//			//&& nextUpRightBase.distance < nextRightBase.distance && Mathf.Abs(nextUpRightBase.distance) < diagonalBoundCorrection))) {
//			//Debug.Log("unblocking upright...");
//			unblockSide();
//			} else {
//				//Debug.Log("null up right hit");
//			}
//		} else {
//			//Debug.Log("No base detected ^>");
//			clearSide();
//			isDirectionBlocked[(int)Direction.UpRight] = false;
//			isDirectionBlocked[(int)Direction.DownLeft] = false;
//			//ClearBlocks();
//		}
	}

	// ReSharper disable once InconsistentNaming
	private void CastUpLeft(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSides) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, new Vector2(-1, 1), resultsUpLeft, Mathf.Infinity, layermask);
		
		if (hits > 0) {
			if (hits > 1) {
				if (currentUpLeftBase.collider != null) {
//					Debug.Log("up isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsDownLeft[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsDownLeft[1].transform.GetComponent<ObjectInfo>()));
					if (resultsUpLeft[1].transform != currentUpLeftBase.transform && resultsUpLeft[1].distance <= minUpLeftBaseDistance) {
//						Debug.Log("1 normal: " + resultsUp[1].point);
						nextUpLeftBase = resultsUpLeft[1];
						minUpLeftBaseDistance = nextUpLeftBase.distance;
					} else {
//						Debug.Log("0 normal: " + resultsUp[0].point);
						nextUpLeftBase = resultsUpLeft[0];
						minUpLeftBaseDistance = nextUpLeftBase.distance;
					}
				} else {
					// If current is null, nextbase is the closest one
//					Debug.Log("null normal: " + resultsUp[0].point);
					currentUpLeftBase = resultsUpLeft[0];
					nextUpLeftBase = currentUpLeftBase;
					minUpLeftBaseDistance = nextUpLeftBase.distance;
				}
			} else {
//				Debug.Log("1== normal: " + resultsUp[0].point);
				currentUpLeftBase = resultsUpLeft[0];
				nextUpLeftBase = currentUpLeftBase;
				minUpLeftBaseDistance = nextUpLeftBase.distance;
			}
//			Debug.Log("next down right base: " + nextUpLeftBase.transform.name + ". curr p height=" + currentHeight);
			Debug.DrawRay(targetRb2d.transform.position,
			Vector3.ClampMagnitude(new Vector3(-nextUpLeftBase.distance, nextUpLeftBase.distance, 0), nextUpLeftBase.distance)
				, Color.red);
			if (nextUpLeftBase.collider != null) {
				var obj = nextUpLeftBase.transform.GetComponent<ObjectInfo>(); 
				// Check if current pos is next to the next wall, taking height into account.
				if ((
					targetRb2d.position.x <= obj.rightBound && targetRb2d.position.x >= obj.leftBound 
					&& Mathf.Abs(targetRb2d.position.y - obj.bottomBound - currentHeight) < boundCorrection
					|| 
					targetRb2d.position.y <= obj.topBound && targetRb2d.position.y >= obj.bottomBound + currentHeight
					&& Mathf.Abs(targetRb2d.position.x - obj.rightBound) < boundCorrection
				    )
				    && CheckIfBlockPlayerByHeight(nextUpLeftBase) &&
				    fallingDirection != Direction.UpLeft) {
//					Debug.Log("normal blocking up left...");
					blockSide();
				} else {
//					Debug.Log("normal unblocking up left...");
					currentDownRightBase = new RaycastHit2D();
					minDownRightBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
//				Debug.Log("unblocking up left...");
				unblockSide();
			}
		} else 	{
			//Debug.Log("No base detected ^");
			clearSides();
			currentUpLeftBase = new RaycastHit2D();
			nextUpLeftBase = new RaycastHit2D();
			currentDownRightBase = new RaycastHit2D();
			nextDownRightBase = new RaycastHit2D();
			minDownRightBaseDistance = Mathf.Infinity;
			minUpLeftBaseDistance = Mathf.Infinity;
			isDirectionBlocked[(int)Direction.DownRight] = false;
			isDirectionBlocked[(int)Direction.UpLeft] = false;
		}
		
//		if (resultsUpLeft.Length > 0) {
//			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
//			// If only hitting a single object, this object is our next/current base; or update the current objects length
//			if (resultsUpLeft.Length == 1 || (currentUpLeftBase.collider != null && !Contains(resultsUpLeft, currentUpLeftBase))) {
//				nextUpLeftBase = resultsUpLeft[0];
//				currentUpLeftBase = new RaycastHit2D();
//				minUpLeftBaseDistance = nextUpLeftBase.distance;
//			} else {
//				//RaycastHit2D hit = new RaycastHit2D();
//				//for (int i = 0; i < resultsUpLeft.Length; i++) {
//				//	RaycastHit2D hit = resultsUpLeft[i];
//					//Debug.Log("checking <^ base...: " + hit.transform.name);
//				if (currentUpLeftBase.collider != null) {
//					//Debug.Log("current upright isnt null");
//					// If this hit isnt the same as the current one, and its distance is smaller,
//					// 		Set the nextBase
//					if (resultsUpLeft[1].transform != currentUpLeftBase.transform && resultsUpLeft[1].distance < minUpLeftBaseDistance) {
//						nextUpLeftBase = resultsUpLeft[1];
//						minUpLeftBaseDistance = resultsUpLeft[1].distance;
//					} else {
//						// Base is the same base. update its distance
//						nextUpLeftBase = resultsUpLeft[0];
//						minUpLeftBaseDistance = resultsUpLeft[0].distance;
//					}
//				} else {
//					//Debug.Log("current upright is null");
//					// If current is null, nextbase is the closest one
//					nextUpLeftBase = resultsUpLeft[0];
//					minUpLeftBaseDistance = resultsUpLeft[0].distance;
//				}
//				//}
//			}
//			//Debug.Log("next ul base: " + (nextUpLeftBase.collider != null ? nextUpLeftBase.transform.name : "no next ul base"));
//			//Debug.Log("nextupleft: " + nextUpLeftBase.distance + "\ndiag bound: " + diagonalBoundCorrection);
//			Debug.DrawRay(targetRb2d.transform.position,
//			Vector3.ClampMagnitude(new Vector3(-nextUpLeftBase.distance, nextUpLeftBase.distance, 0), nextUpLeftBase.distance)
//				, Color.red);
//			if (nextUpLeftBase.collider != null && Mathf.Abs(nextUpLeftBase.distance) < diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextUpLeftBase)
//				&& fallingDirection != Direction.UpLeft) {
//				//Debug.Log("blocking upright...");
//				blockSide();
//			} else if (nextUpLeftBase.collider != null) {
//				//} else if (nextUpLeftBase.collider != null
//				//&& ((nextUpBase.collider == null && nextLeftBase.collider == null && Mathf.Abs(nextUpLeftBase.distance) > diagonalBoundCorrection)
//				//|| (nextUpLeftBase.distance > nextUpBase.distance
//				//&& nextUpLeftBase.distance > nextLeftBase.distance && Mathf.Abs(nextUpLeftBase.distance) > diagonalBoundCorrection))) {
//				//Debug.Log("unblocking upleft...");
//				unblockSide();
//			} else {
//		//} else if ((nextUpBase.collider == null && nextLeftBase.collider == null && Mathf.Abs(nextUpLeftBase.distance) > diagonalBoundCorrection)
//			//|| (nextUpLeftBase.distance > nextUpBase.distance
//			//&& nextUpLeftBase.distance > nextLeftBase.distance && Mathf.Abs(nextUpLeftBase.distance) > diagonalBoundCorrection)) {
//				//Debug.Log("null up left hit");
//				isDirectionBlocked[(int)Direction.UpLeft] = true;
//			}
//		} else {
//			//Debug.Log("No base detected ^>");
//			clearSides();
//			isDirectionBlocked[(int)Direction.UpLeft] = false;
//			isDirectionBlocked[(int)Direction.DownRight] = false;
//			//ClearBlocks();
//		}
	}

	// ReSharper disable once InconsistentNaming
	private void CastDownLeft(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSides) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, new Vector2(-1, -1), resultsDownLeft, Mathf.Infinity, layermask);
		
		if (hits > 0) {
			if (hits > 1) {
				if (currentDownLeftBase.collider != null) {
//					Debug.Log("up isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsDownLeft[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsDownLeft[1].transform.GetComponent<ObjectInfo>()));
					if (resultsDownLeft[1].transform != currentDownLeftBase.transform && resultsDownLeft[1].distance <= minDownLeftBaseDistance) {
//						Debug.Log("1 normal: " + resultsUp[1].point);
						nextDownLeftBase = resultsDownLeft[1];
						minDownLeftBaseDistance = nextDownLeftBase.distance;
					} else {
//						Debug.Log("0 normal: " + resultsUp[0].point);
						nextDownLeftBase = resultsDownLeft[0];
						minDownLeftBaseDistance = nextDownLeftBase.distance;
					}
				} else {
					// If current is null, nextbase is the closest one
//					Debug.Log("null normal: " + resultsUp[0].point);
					currentDownLeftBase = resultsDownLeft[0];
					nextDownLeftBase = currentDownLeftBase;
					minDownLeftBaseDistance = nextDownLeftBase.distance;
				}
			} else {
//				Debug.Log("1== normal: " + resultsUp[0].point);
				currentDownLeftBase = resultsDownLeft[0];
				nextDownLeftBase = currentDownLeftBase;
				minDownLeftBaseDistance = nextDownLeftBase.distance;
			}
//			Debug.Log("next down left base: " + nextDownLeftBase.transform.name + ". curr p height=" + currentHeight);
			Debug.DrawRay(targetRb2d.transform.position,
			              Vector3.ClampMagnitude(new Vector3(-nextDownLeftBase.distance, -nextDownLeftBase.distance, 0), nextDownLeftBase.distance)
			              , Color.red);
			if (nextDownLeftBase.collider != null) {
				var obj = nextDownLeftBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if ((
				    targetRb2d.position.x <= obj.rightBound && targetRb2d.position.x >= obj.leftBound 
				    && Mathf.Abs(targetRb2d.position.y - obj.topBound) < boundCorrection
				    || 
				    targetRb2d.position.y <= obj.topBound && targetRb2d.position.y >= obj.bottomBound 
					 && Mathf.Abs(targetRb2d.position.x - obj.rightBound) < boundCorrection
				    )
				    && CheckIfBlockPlayerByHeight(nextDownLeftBase) &&
				    fallingDirection != Direction.DownLeft) {
					Debug.Log("normal blocking down left...");
					blockSide();
				} else {
					Debug.Log("normal unblocking down left...");
					currentUpRightBase = new RaycastHit2D();
					minUpRightBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
				Debug.Log("unblocking down...");
				unblockSide();
			}
		} else 	{
			//Debug.Log("No base detected ^");
			clearSides();
			currentDownLeftBase = new RaycastHit2D();
			nextDownLeftBase = new RaycastHit2D();
			minDownLeftBaseDistance = Mathf.Infinity;
			currentUpRightBase = new RaycastHit2D();
			nextUpRightBase = new RaycastHit2D();
			minUpRightBaseDistance = Mathf.Infinity;
			isDirectionBlocked[(int)Direction.DownLeft] = false;
			isDirectionBlocked[(int)Direction.UpRight] = false;
		}
		
		
//		if (resultsDownLeft.Length > 0) {
//			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
//			// If only hitting a single object, this object is our next/current base; or update the current objects length
//			if (resultsDownLeft.Length == 1 || (currentDownLeftBase.collider != null && !Contains(resultsDownLeft, currentDownLeftBase))) {
//				//Debug.Log("Update down left....");
//				nextDownLeftBase = resultsDownLeft[0];
//				currentDownLeftBase = new RaycastHit2D();
//				minDownLeftBaseDistance = nextDownLeftBase.distance;
//			} else {
//				//for (int i = 0; i < resultsDownLeft.Length; i++) {
//				//	RaycastHit2D hit = resultsDownLeft[i];
//					//Debug.Log("checking <v base...: " + hit.transform.name);
//				if (currentDownLeftBase.collider != null) {
//					//Debug.Log("current upright isnt null");
//					// If this hit isnt the same as the current one, and its distance is smaller,
//					// 		Set the nextBase
//					if (resultsDownLeft[1].transform != currentDownLeftBase.transform && resultsDownLeft[1].distance < minDownLeftBaseDistance) {
//						nextDownLeftBase = resultsDownLeft[1];
//						minDownLeftBaseDistance = resultsDownLeft[1].distance;
//					} else {
//						// Base is the same base. update its distance
//						nextDownLeftBase = resultsDownLeft[0];
//						minDownLeftBaseDistance = resultsDownLeft[0].distance;
//					}
//				} else {
//					//Debug.Log("current upright is null");
//					// If current is null, nextbase is the closest one
//					nextDownLeftBase = resultsDownLeft[0];
//					minDownLeftBaseDistance = resultsDownLeft[0].distance;
//				}
//				//}
//			}
//			//Debug.Log("next dl base: " + (nextDownLeftBase.collider != null ? nextDownLeftBase.transform.name : "no next dl base"));
//			Debug.DrawRay(targetRb2d.transform.position,
//				Vector3.ClampMagnitude(new Vector3(-nextDownLeftBase.distance, -nextDownLeftBase.distance, 0), nextDownLeftBase.distance)
//				, Color.red);
//			// If hit isnt null and reached the bound, check if bblocked by height
//			if (nextDownLeftBase.collider != null && Mathf.Abs(nextDownLeftBase.distance) <= diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextDownLeftBase)
//				&& fallingDirection != Direction.DownLeft) {
//				//Debug.Log("blocking down left...");
//				blockSide();
//				//} else if (nextDownLeftBase.collider != null) {
//			} else if (nextDownLeftBase.collider != null) {
//			//&& ((nextDownBase.collider == null && nextLeftBase.collider == null && Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection)
//			//|| (nextDownLeftBase.distance < nextDownBase.distance
//			//&& nextDownLeftBase.distance < nextLeftBase.distance && Mathf.Abs(nextDownLeftBase.distance) < diagonalBoundCorrection))) {
//				//Debug.Log("unblocking down left...");
//				unblockSide();
//			} else {
//				//Debug.Log("null down left hit");
//			}
//		} else {
//			//Debug.Log("No base detected <^");
//			clearSide();
//			isDirectionBlocked[(int)Direction.DownLeft] = false;
//			isDirectionBlocked[(int)Direction.UpRight] = false;
//			//ClearBlocks();
//		}
	}

	// ReSharper disable once InconsistentNaming
	private void CastDownRight(Rigidbody2D targetRb2d, int layermask, Action blockSide, Action unblockSide, Action clearSides) {
		int hits = Physics2D.RaycastNonAlloc(targetRb2d.transform.position, new Vector2(1, -1), resultsDownRight, Mathf.Infinity, layermask);
		
		if (hits > 0) {
			if (hits > 1) {
				if (currentDownRightBase.collider != null) {
//					Debug.Log("up isnt null");
//					Debug.Log("Inside 0: " + CheckIfInCollider(transform.position, resultsDownLeft[0].transform.GetComponent<ObjectInfo>()));
//					Debug.Log("Inside 1: " + CheckIfInCollider(transform.position, resultsDownLeft[1].transform.GetComponent<ObjectInfo>()));
					if (resultsDownRight[1].transform != currentDownRightBase.transform && resultsDownRight[1].distance <= minDownRightBaseDistance) {
//						Debug.Log("1 normal: " + resultsUp[1].point);
						nextDownRightBase = resultsDownRight[1];
						minDownRightBaseDistance = nextDownRightBase.distance;
					} else {
//						Debug.Log("0 normal: " + resultsUp[0].point);
						nextDownRightBase = resultsDownRight[0];
						minDownRightBaseDistance = nextDownRightBase.distance;
					}
				} else {
					// If current is null, nextbase is the closest one
//					Debug.Log("null normal: " + resultsUp[0].point);
					currentDownRightBase = resultsDownRight[0];
					nextDownRightBase = currentDownRightBase;
					minDownRightBaseDistance = nextDownRightBase.distance;
				}
			} else {
//				Debug.Log("1== normal: " + resultsUp[0].point);
				currentDownRightBase = resultsDownRight[0];
				nextDownRightBase = currentDownRightBase;
				minDownRightBaseDistance = nextDownRightBase.distance;
			}
//			Debug.Log("next down right base: " + nextDownRightBase.transform.name + ". curr p height=" + currentHeight);
			Debug.DrawRay(targetRb2d.transform.position,
			              Vector3.ClampMagnitude(new Vector3(nextDownRightBase.distance, -nextDownRightBase.distance, 0), nextDownRightBase.distance)
			              , Color.red);
			if (nextDownRightBase.collider != null) {
				var obj = nextDownRightBase.transform.GetComponent<ObjectInfo>();
				// Check if current pos is next to the next wall, taking height into account.
				if ((
				    targetRb2d.position.x <= obj.rightBound && targetRb2d.position.x >= obj.leftBound 
				    && Mathf.Abs(targetRb2d.position.y - obj.topBound) < boundCorrection
				    || 
				    targetRb2d.position.y <= obj.topBound && targetRb2d.position.y >= obj.bottomBound 
					&& Mathf.Abs(targetRb2d.position.x - obj.leftBound) < boundCorrection
				    )
				    && CheckIfBlockPlayerByHeight(nextDownRightBase) &&
				    fallingDirection != Direction.DownRight) {
					Debug.Log("normal blocking down right...");
					blockSide();
				} else {
					Debug.Log("normal unblocking down right...");
					currentUpLeftBase = new RaycastHit2D();
					minUpLeftBaseDistance = Mathf.Infinity; // Reset distance
					unblockSide();
				}
			} else /*if (nextUpBase.collider != null)*/ {
				Debug.Log("unblocking down...");
				unblockSide();
			}
		} else 	{
			//Debug.Log("No base detected ^");
			clearSides();
			currentDownRightBase = new RaycastHit2D();
			nextDownRightBase = new RaycastHit2D();
			minDownRightBaseDistance = Mathf.Infinity;
			currentUpLeftBase = new RaycastHit2D();
			nextUpLeftBase = new RaycastHit2D();
			minUpLeftBaseDistance = Mathf.Infinity;
			isDirectionBlocked[(int)Direction.DownRight] = false;
			isDirectionBlocked[(int)Direction.UpLeft] = false;
		}
		
//		if (resultsDownRight.Length > 0) {
//			//Debug.Log(string.Format("[0]: {0}, [1]:{1}", resultsLeft[0].distance, resultsLeft.Length > 1 ? resultsLeft[1].distance : 0));
//			// If only hitting a single object, this object is our next/current base; or update the current objects length
//			if (resultsDownRight.Length == 1 || (currentDownRightBase.collider != null && !Contains(resultsDownRight, currentDownRightBase))) {
//				//Debug.Log("One down right base.");
//				nextDownRightBase = resultsDownRight[0];
//				currentDownRightBase = new RaycastHit2D();
//				minDownRightBaseDistance = nextDownRightBase.distance;
//			} else {
//				//for (int i = 0; i < resultsDownRight.Length; i++) {
//				//	RaycastHit2D hit = resultsDownRight[i];
//					//Debug.Log("checking v> base...: " + hit.transform.name);
//				if (currentDownRightBase.collider != null) {
//					//Debug.Log("current upright isnt null");
//					// If this hit isnt the same as the current one, and its distance is smaller,
//					// 		Set the nextBase
//					if (resultsDownRight[1].transform != currentDownRightBase.transform && resultsDownRight[1].distance < minDownRightBaseDistance) {
//						nextDownRightBase = resultsDownRight[1];
//						minDownRightBaseDistance = resultsDownRight[1].distance;
//					} else {
//						// Base is the same base. update its distance
//						nextDownRightBase = resultsDownRight[0];
//						minDownRightBaseDistance = resultsDownRight[0].distance;
//					}
//				} else {
//					//Debug.Log("current upright is null");
//					// If current is null, nextbase is the closest one
//					nextDownRightBase = resultsDownRight[0];
//					minDownRightBaseDistance = resultsDownRight[0].distance;
//				}
//				//}
//			}
//			//Debug.Log("next ur base: " + (nextUpRightBase.collider != null ? nextUpRightBase.transform.name : "no next ur base"));
//			Debug.DrawRay(targetRb2d.transform.position,
//				Vector3.ClampMagnitude(new Vector3(nextDownRightBase.distance, -nextDownRightBase.distance, 0), nextDownRightBase.distance)
//				, Color.red);
//			if (nextDownRightBase.collider != null && Mathf.Abs(nextDownRightBase.distance) <= diagonalBoundCorrection && CheckIfBlockPlayerByHeight(nextDownRightBase)
//				&& fallingDirection != Direction.DownRight) {
//				//Debug.Log("blocking upright...");
//				blockSide();
//			} else if (nextDownRightBase.collider != null) {
//			//&& ((nextDownBase.collider == null && nextRightBase.collider == null && Mathf.Abs(nextDownRightBase.distance) < diagonalBoundCorrection)
//			//|| (nextDownRightBase.distance < nextDownBase.distance
//			//&& nextDownRightBase.distance < nextRightBase.distance && Mathf.Abs(nextDownRightBase.distance) < diagonalBoundCorrection))) {
//			//Debug.Log("unblocking upright...");
//				unblockSide();
//			} else {
//				//Debug.Log("null down right hit");
//			}
//		} else {
//			//Debug.Log("No base detected ^>");
//			clearSide();
//			isDirectionBlocked[(int)Direction.DownRight] = false;
//			isDirectionBlocked[(int)Direction.UpLeft] = false;
//			//ClearBlocks();
//		}
	}

	/// <summary>
	/// Checks if the RaycasteHit2D array contains the current hit.
	/// </summary>
	/// <param name="hits"></param>
	/// <param name="current"></param>
	/// <returns>bool</returns>
	private static bool Contains(RaycastHit2D[] hits, RaycastHit2D current) {
		return hits != null && hits.Any(t => t.transform.Equals(current.transform));
	}
	/*
	 * 
		if(hits == null) {
			return false;
		}
		for (int i = 0; i < hits.Length; i++) {
			//Debug.Log("current: " + current.collider.name + " checking: " + hits[i].collider.name);
			//if(Mathf.Abs(hits[i].distance - current.distance) < Mathf.Epsilon) {
			if (hits[i].transform.Equals(current.transform)) {
				return true;
			}
		}
		return false;
	 */
}
