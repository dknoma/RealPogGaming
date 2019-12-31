// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using Characters.Allies;
// using Managers;
// using Unity.Collections;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using Object = System.Object;
// // ReSharper disable CommentTypo
// // ReSharper disable IdentifierTypo
// #pragma warning disable 649

// /*
//  * TODO: Handle collision when unit wasn't able to make it over the top
//  *		- if falling and is inside the collider, push them back in the other direction
//  * 		- Fix jumping from the top while facing down
//  * 		- If slow, make Dictionaries that hold components (platform info, etc) and get from ids
// */			 
// [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
// public class TestPhysicsController : MonoBehaviour {

// 	[Header("Direction Variables")]
// 	public OverworldMovementDirection facingDirection = OverworldMovementDirection.Down;
// 	public bool isWalking;

// 	[Header("Physics Variables")]
// 	public float jumpSpeed = 5.0f;
// 	public float overworldSpeed = 0.35f;
// 	public float gravityModifier = 5;

// 	private int platformMask;
// 	private int baseMask;
// 	private int boundMask;

// 	// Constants
// 	private const float BOUND_CORRECTION = 0.8125f;
// //	1.625f;
// //	private const float RAY_LIMIT = 24f;

// 	// Game Objects
// 	private Animator animator;
// //	private BoxCollider2D myCollider;
// 	// ReSharper disable once InconsistentNaming
// 	private Rigidbody2D rb2d;
	
// 	// private ContactFilter2D baseContactFilter;
// 	// private ContactFilter2D wallContactFilter;
// 	// private ContactFilter2D platformContactFilter;
// 	// private ContactFilter2D floorContactFilter;
// 	// private ContactFilter2D boundsContactFilter;
	
// 	private PlayerShadowCopy shadow;
	
// 	[HideInInspector]
// 	public ObjectInfo nextPlatform;
// 	[HideInInspector]
// 	public ObjectInfo currentPlatform;
// 	private ObjectInfo previousPlatform;
// 	private ObjectInfo ground;

// 	// Physics stuff
// 	// public bool[] isDirectionBlocked = new bool[(int)OverworldMovementDirection.DownLeft + 1];
// 	// private bool[] isPDirectionBlocked = new bool[(int)OverworldMovementDirection.DownLeft + 1];
// 	// Base physics
// 	// private RaycastHit2D[] resultsUp = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsDown = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsRight = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsLeft = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsUpRight = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsUpLeft = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsDownRight = new RaycastHit2D[2];
// 	// private RaycastHit2D[] resultsDownLeft = new RaycastHit2D[2];
	
// 	// Platform physics
// //	private RaycastHit2D[] pResultsUp = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsDown = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsRight = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsLeft  = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsUpRight = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsUpLeft = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsDownRight = new RaycastHit2D[2];
// //	private RaycastHit2D[] pResultsDownLeft = new RaycastHit2D[2];
// //
// //	private RaycastHit2D[] floorHits = new RaycastHit2D[2];
// //	private RaycastHit2D[] wallHits = new RaycastHit2D[2];
// //
// //	private RaycastHit2D upHit;
// //	private RaycastHit2D downHit;
// //	private RaycastHit2D leftHit;
// //	private RaycastHit2D rightHit;

// 	// Raycast stuff
// 	// private RaycastHit2D currentLeftBase;
// 	// private RaycastHit2D nextLeftBase;
// 	// private float minLeftBaseDistance;
// 	// private RaycastHit2D currentRightBase;
// 	// private RaycastHit2D nextRightBase;
// 	// private float minRightBaseDistance;
// 	// private RaycastHit2D currentUpBase;
// 	// private RaycastHit2D nextUpBase;
// 	// private float minUpBaseDistance;
// 	// private RaycastHit2D currentDownBase;
// 	// private RaycastHit2D nextDownBase;
// 	// private float minDownBaseDistance;
// 	// private RaycastHit2D currentUpLeftBase;
// 	// private RaycastHit2D nextUpLeftBase;
// 	// private float minUpLeftBaseDistance;
// 	// private RaycastHit2D currentUpRightBase;
// 	// private RaycastHit2D nextUpRightBase;
// 	// private float minUpRightBaseDistance;
// 	// private RaycastHit2D currentDownLeftBase;
// 	// private RaycastHit2D nextDownLeftBase;
// 	// private float minDownLeftBaseDistance;
// 	// private RaycastHit2D currentDownRightBase;
// 	// private RaycastHit2D nextDownRightBase;
// 	// private float minDownRightBaseDistance;
// 	//
// 	// private RaycastHit2D[] currentPlatformHits;
// 	// private RaycastHit2D currentPlatformHit;
// 	// private RaycastHit2D nextPlatformHit;
// 	// private float minPlatformDistance;

// //	private Collider2D[] wallChecks = new Collider2D[3];
// //	private Collider2D[] positionColliders = new Collider2D[2];

// 	private OverworldMovementDirection fallingDirection;

// // 	private float yOffset;
// // 	private float yRadius;
// // 	private float xRadius;
// // //	private Vector3 groundPosition;
// // 	private float groundHeight;
// 	public bool grounded = true;
// 	private bool rising;
// // 	private bool lowering;
	
// 	private Vector2 velocity;
// 	private Vector2 targetVelocity;
// 	private Vector2 groundNormal;
// 	private Vector2 move;
	
// 	[HideInInspector]
// 	public bool jumping;
// 	public float currentHeight;
	
// 	[HideInInspector]
// 	public bool isFalling;
	
// 	// private bool fallingOffPlatform;
// 	// private bool isOnPlatform;
// 	// private bool transitionToCurrentPlatform;
// 	// private bool leftCurrentPlatform;
// 	//
// 	// private float raisingHeight;
// 	// private float fallingHeight;
	
// 	private float blockingCorrectionSpeed;
// 	private float diagonalMovementSpeed;
// 	private float diagonalBoundCorrection;

// 	// Coroutines
// 	// private Coroutine jumpingRoutine;
// 	// private Coroutine fallingRoutine;
// 	// private Coroutine platformRoutine;
// 	// private Coroutine landingRoutine;
	
// 	private static readonly int IsWalkingAni = Animator.StringToHash("isWalking");
// 	private static readonly int DirectionAni = Animator.StringToHash("direction");
// 	private static readonly int ChangeDirectionAni = Animator.StringToHash("changeDirection");

// 	private void Start() {
// 		animator = GetComponentInChildren<Animator>();
// 		animator.ResetTrigger("changeDirection");
		
// 		rb2d = GetComponent<Rigidbody2D>();
		
// 		baseMask = LayerMask.GetMask("Base");
// 		platformMask = LayerMask.GetMask("Platform");
// 		boundMask = LayerMask.GetMask("Bounds");
		
// 		// baseContactFilter.SetLayerMask(baseMask);
// 		// wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
// 		// platformContactFilter.SetLayerMask(platformMask);
// 		// floorContactFilter.SetLayerMask(platformMask);
// 		// floorContactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
// 		// boundsContactFilter.SetLayerMask(boundMask);
// 		//Debug.Log(string.Format("coll offset: {0}",yOffset+yRadius));
		
// 		shadow = transform.parent.GetComponentInChildren<PlayerShadowCopy>();
// //		jumpHeight = gameObject.FindComponentInSiblingsWithTag<Transform>("JumpHeight");
// 		diagonalMovementSpeed = Mathf.Sqrt(overworldSpeed * overworldSpeed / 2);
// 		// diagonalBoundCorrection = Mathf.Sqrt(2 * (BOUND_CORRECTION*BOUND_CORRECTION));
// 		// blockingCorrectionSpeed = overworldSpeed;
// 	}
	
// 	private Vector3 pointTo3(Vector2 point, int height) {
// 	    return new Vector3(point.x, height, point.y - height);
// 	}
	
// 	// TODO: collider has multiple contact points, how to get contact point we want?
// 	private void OnCollisionStay2D(Collision2D coll) {
//         if(coll.gameObject.CompareTag("Platform")) {
//             Platform plat = coll.gameObject.GetComponent<Platform>();
//             ContactPoint2D[] contacts = coll.contacts;
//             ContactPoint2D first = contacts[0];
            
//             foreach(ContactPoint2D point in coll.contacts) {
//                 Vector3 point3 = pointTo3(point.point, plat.Height);
                
//                 Debug.Log(string.Format("\t\ttouching {0}, {1}, {2}", plat.name, plat.Height, point3.ToString("F4")));
//             }
            
//             // Vector3 point = pointTo3(first.point, plat.Height);
            
//             // Debug.Log(string.Format("\t\ttouching {0}, {1}, {2}", plat.name, plat.Height, point.ToString("F4")));
//         }
//     }

// 	// private void OnCollisionEnter2D(Collision2D coll) {
// 	// 	if(coll.gameObject.CompareTag("Platform")) {
// 	// 		Debug.Log(string.Format("\t\ttouched next plat {0}", coll.gameObject.name));
// 	// 		ObjectInfo platValues = coll.gameObject.GetComponent<ObjectInfo>();
// 	// 		nextPlatform = platValues;
// 	// 	}
// 	// 	
// 	// 	// Start battle if run into an enemy
// 	// 	if(coll.gameObject.CompareTag("Enemy")) {
// 	// 		if (Vector2.Distance(coll.GetContact(0).point, shadow.transform.position) <= shadow.GetComponent<Collider2D>().bounds.size.x) {
// 	// 			Debug.Log(string.Format("\t\tcurrent enemy {0}, {1}", coll.gameObject.name, Vector2.Distance(coll.GetContact(0).point, shadow.transform.position)));
// 	// 			AreaEnemyManager.aem.EnemyToBattle(coll.gameObject);
// 	// 			Enemy enemy = coll.gameObject.GetComponent<Enemy>();
// 	// 			// For the enemy overworld sprite, add them to listen in on defeated events to do their animation
// 	// 			BattleEventManager.bem.AddDefeatEnemyEvent(enemy.slot, enemy.DoDefeatEnemyAnimation);
// 	// 			BattleManager.bm.SetBattleState(BattleState.Init);
// 	// 		}
// 	// 	}
// 	// }

// 	/// <summary>
// 	/// Reset animation settings when main player is changed
// 	/// </summary>
// 	public void ChangeCharacter() {
// 		animator = GetComponentInChildren<Animator>();
// 		animator.ResetTrigger("changeDirection");
// 	}

// 	private void Update() {
// 		//Debug.Log("curr h: " + currentHeight);
// 		//Debug.Log("fall direction: " + fallingDirection);
// //		Debug.Log("shad pos: " + shadow.transform.position);

// 		// Jump();
// 		if (BattleManager.bm.InBattle()) {
// 			StopDirection(facingDirection);
// 		}
// 		// CheckPlatformCollision();
// 		// ComputeVelocity();
// 		// groundHeight = shadow.totalHeight;
// 		// Debug.Log("player pos: " + transform.position);
// 	}

// 	private void FixedUpdate() {
// 		MovePlayer();
// //		upHit = RaycastInDirection(OverworldMovementDirection.Up);
// //		downHit = RaycastInDirection(OverworldMovementDirection.Down);
// //		leftHit = RaycastInDirection(OverworldMovementDirection.Left);
// //		rightHit = RaycastInDirection(OverworldMovementDirection.Right);
// //		groundPosition = shadow.transform.position;
// 		velocity += Physics2D.gravity * (gravityModifier * Time.deltaTime);
// //		Debug.Log("height " + currentHeight);
// //		Debug.Log("velocity " + velocity);
// 		if (!grounded) {
// 			transform.position += (Vector3) velocity;
// 			//currentHeight = transform.position.y - shadow.transform.position.y + currentPlatform.height;
// 			currentHeight = transform.position.y - shadow.transform.position.y + 0;
// //			                + (currentPlatform != null
// //				            ? currentPlatform.height : 0);
// //			Debug.Log("p height " + currentPlatform.height);
// 			if (transform.position.y - shadow.transform.position.y >= 0.0001) return;
// 			Debug.Log("is grounded.");
// 			fallingDirection = OverworldMovementDirection.Null;					// Reset falling direction
// 			grounded = true;									// Player is grounded
// 			isFalling = false;									// Player is not falling
// 			transform.position = shadow.transform.position;		// Make sure player doesnt go lower than allowed
// 			rising = false;
// 			// currentHeight = transform.position.y - shadow.transform.position.y + currentPlatform.height;
// 			currentHeight = transform.position.y - shadow.transform.position.y + 0;
// 		} else {
// 			jumping = false;
// 			transform.position = shadow.transform.position;
// //			currentHeight = currentPlatform != null ? currentPlatform.height : 0;
// //			Debug.LogFormat("currr: {0}", currentPlatform.name);
// 			// currentHeight = currentPlatform.height;
// 			currentHeight = 0;
// //			currentPlatform = Math.Abs(currentHeight) < Mathf.Epsilon ? ground : currentPlatform;
// 			velocity = Vector2.zero;
// 			// transitionToCurrentPlatform = false;
// 		}
// 	}

// 	private void Jump() {
// 		if (!Input.GetButtonDown("Jump") || !grounded) return; //if jump button is pressed and is grounded
// 		Debug.Log("jumping");
// 		grounded = false;
// 		jumping = true;
// 		velocity.y = jumpSpeed;
// 		// leftCurrentPlatform = false;
// 	}
	
// 	private void MovePlayer() {
// 		if (BattleManager.bm.InBattle()) return;
// 		//Debug.Log(string.Format("v: {0}, h {1}", I+nput.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
// 		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
// 			// Facing up
// 			StartDirection(OverworldMovementDirection.Up);
// 			MoveInDirection(OverworldMovementDirection.Up, overworldSpeed);
// 		} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
// 			// Facing down
// 			StartDirection(OverworldMovementDirection.Down);
// 			MoveInDirection(OverworldMovementDirection.Down, overworldSpeed);
// 		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
// 			// Facing right
// 			StartDirection(OverworldMovementDirection.Right);
// 			MoveInDirection(OverworldMovementDirection.Right, overworldSpeed); 
// 		} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
// 			// Facing left
// 			StartDirection(OverworldMovementDirection.Left);
// 			MoveInDirection(OverworldMovementDirection.Left, overworldSpeed);
// 		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
// 			StartDirection(OverworldMovementDirection.UpRight);
// 			MoveInDirection(OverworldMovementDirection.UpRight, diagonalMovementSpeed);
// 		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
// 			StartDirection(OverworldMovementDirection.UpLeft);
// 			MoveInDirection(OverworldMovementDirection.UpLeft, diagonalMovementSpeed);
// 		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") < 0) {
// 			// Facing down-left
// 			StartDirection(OverworldMovementDirection.DownLeft);
// 			MoveInDirection(OverworldMovementDirection.DownLeft, diagonalMovementSpeed);
// 		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
// 			// Facing down-right
// 			StartDirection(OverworldMovementDirection.DownRight);
// 			MoveInDirection(OverworldMovementDirection.DownRight, diagonalMovementSpeed);
// 		} else {
// 			animator.SetBool(IsWalkingAni, false);
// 			isWalking = false;
// 		}
// 	}

// 	[ReadOnly]
// 	public Vector3 posWrapper;
		
// 	private void movePos(float x, float y, float z) {
// 		posWrapper += new Vector3(x, y, z) * Time.fixedDeltaTime;
// 		transform.position += new Vector3(x, z, 0) * Time.fixedDeltaTime;
// 		shadow.transform.position += new Vector3(x, z, 0) * Time.fixedDeltaTime;
// 	}
 
// 	private void MoveInDirection(OverworldMovementDirection direction, float speed) {
// 		switch (direction) {
// 			case OverworldMovementDirection.Up:
// 				movePos(0, 0, speed);
// 				break;
// 			case OverworldMovementDirection.Down:
// 				movePos(0, 0, -speed);
// 				break;
// 			case OverworldMovementDirection.Left:
// 				movePos(-speed,0, 0);
// 				break;
// 			case OverworldMovementDirection.Right:
// 				movePos(speed,0, 0);
// 				break;
// 			case OverworldMovementDirection.UpRight:
// 				movePos(speed,0, speed);
// 				break;
// 			case OverworldMovementDirection.UpLeft:
// 				movePos(-speed,0, speed);
// 				break;
// 			case OverworldMovementDirection.DownRight:
// 				movePos(speed,0, -speed);
// 				break;
// 			case OverworldMovementDirection.DownLeft:
// 				movePos(-speed,0, -speed);
// 				break;
// 			default:
// 				throw new ArgumentOutOfRangeException("direction", direction, null);
// 		}
// 	}

// 	private void StartDirection(OverworldMovementDirection direction) {
// 		isWalking = true;
// 		facingDirection = direction;
// 		animator.SetBool(IsWalkingAni, true);
// 		animator.SetInteger(DirectionAni, (int)facingDirection - 1);
// 		animator.SetTrigger(ChangeDirectionAni);
// 	}
	
// 	private void StopDirection(OverworldMovementDirection direction) {
// 		isWalking = false;
// 		facingDirection = direction;
// 		animator.SetBool(IsWalkingAni, false);
// 		animator.SetInteger(DirectionAni, (int)facingDirection - 1);
// 		animator.SetTrigger(ChangeDirectionAni);
// 	}
// }
