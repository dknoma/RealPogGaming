using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour {
	
	public ObjectInfo currentPlatform;
	private ObjectInfo previousPlatform;
//	private ObjectInfo ground;
//	private ObjectValues nextPlatform;
	
	private const float BOUND_CORRECTION = 1.625f;

	private bool rising;
	private bool lowering;
	private bool leftCurrentPlatform;
	private bool isOnPlatform;
	private bool overAnotherPlat;
	private float fallingHeight;
	private float groundedHeight;
	
	private PlayerController player;
	private Direction fallingDirection;
	private Collider2D myColl;
	
	private ContactFilter2D baseContactFilter;
	private ContactFilter2D wallContactFilter;
	private ContactFilter2D platformContactFilter;
	private ContactFilter2D floorContactFilter;
	private ContactFilter2D boundsContactFilter;
	
	// Platform physics
	private RaycastHit2D[] pResultsUp = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsDown = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsRight = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsLeft  = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsUpRight = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsUpLeft = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsDownRight = new RaycastHit2D[2];
	private RaycastHit2D[] pResultsDownLeft = new RaycastHit2D[2];
	
	private void Awake() {
		int baseMask = LayerMask.GetMask("Base");
		int platformMask = LayerMask.GetMask("Platform");
		baseContactFilter.SetLayerMask(baseMask);
		platformContactFilter.SetLayerMask(platformMask);
		floorContactFilter.SetLayerMask(platformMask);
		floorContactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
		wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
		boundsContactFilter.SetLayerMask(LayerMask.GetMask("Bounds"));
		player = gameObject.FindComponentInSiblingsWithTag<PlayerController>("Player");
		myColl = gameObject.GetComponent<Collider2D>();
		
		Debug.LogFormat("player {0}", player.transform.name);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, platformMask);
		Debug.LogFormat("hit {0}", hit.transform.name);
		Debug.LogFormat("aslkfhasflkjasfl;kj: {0}", hit.transform.GetComponent<ObjectInfo>());
		player.currentPlatform = hit.transform.GetComponent<ObjectInfo>();
		Debug.LogFormat("Initial platform: {0}", player.currentPlatform.name);
		previousPlatform = player.currentPlatform;
//		ground = new ObjectValues("ground", 0, Vector3.zero, -9999999);
//		previousPlatform = ground;
	}

	private void Update() {
		fallingDirection = player.facingDirection;
		groundedHeight = player.currentPlatform.height;
	}

	private void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.CompareTag("Platform")) {
			Debug.Log(string.Format("\t\ttouched next plat {0}", coll.gameObject.name));
			ObjectInfo platValues = coll.gameObject.GetComponent<ObjectInfo>();
//			player.nextPlatform = platValues;
			if (player.nextPlatform != player.currentPlatform) {
				overAnotherPlat = true;
			}
		}
	}
	
	private void OnCollisionStay2D(Collision2D coll) {
		if(coll.gameObject.CompareTag("Platform")) {
			ObjectInfo platValues = coll.gameObject.GetComponent<ObjectInfo>();
			if (player.currentHeight < platValues.height && !player.grounded) {
				Debug.Log("returning...");
				return;
			}
			if (!overAnotherPlat) {
				player.currentPlatform = platValues;
			}
//			Debug.Log(string.Format("\t\tinside plat {0}", player.currentPlatform.name));
			if(player.isWalking && player.nextPlatform != currentPlatform 
			                    && Mathf.RoundToInt(player.nextPlatform.height) - Mathf.RoundToInt(groundedHeight) != 0
			                    && HigherThanPlatform(player.nextPlatform) 
			                    && !rising) {
				Debug.Log("UP: trying to jump onto " + player.nextPlatform.name);
				Debug.Log(string.Format("Inside {0}", player.nextPlatform.name));
				overAnotherPlat = false;
				currentPlatform = player.nextPlatform;
				isOnPlatform = true;
				leftCurrentPlatform = false;
				rising = true;
				lowering = false;
//				nextPlatform = ground;
				RaisePlayerObjects(); // happens multiple
			}
		}
	}
	
	private void OnCollisionExit2D(Collision2D coll) {
		if(coll.gameObject.CompareTag("Platform")) {
			Debug.Log(string.Format("\t\texiting plat {0}", coll.gameObject.name));
			overAnotherPlat = false;
			ObjectInfo platValues = coll.gameObject.GetComponent<ObjectInfo>();
			previousPlatform = platValues;
			if(fallingDirection == Direction.Down) {
				int hits = myColl.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
				if(hits > 0) {
					Debug.Log("DOWN GROUND");
					ObjectInfo plat = pResultsDown[0].transform.GetComponent<ObjectInfo>();
					player.nextPlatform = plat;
				} 
//				else {
//					shadow.gameObject.GetComponent<Collider2D>().Cast(Vector2.down, floorContactFilter, pResultsDown);
//					ObjectValues plat = pResultsDown[0].transform.GetComponent<ObjectInfo>().values;
//					nextPlatform = plat;
//				}
			}
			Debug.Log("\tnextPlatform: " + player.nextPlatform.name + ", prev: " + previousPlatform.name);
			if (Mathf.Round(player.nextPlatform.height) < Mathf.Round(previousPlatform.height)
			    && player.isWalking && !lowering) {
				Debug.Log("normal falling - " + previousPlatform.name + " to " + player.nextPlatform.name);
				fallingHeight = CalculateFallingHeight(fallingDirection);
				lowering = true;
				rising = false;
//				isOnPlatform = false;
//				transitionToCurrentPlatform = false;
				leftCurrentPlatform = true;
				Fall();
//				currentPlatform = nextPlatform;
			}
//			currentPlatform = ground;
//			player.nextPlatform = ground;
		}
	}
	
	private bool HigherThanPlatform(ObjectInfo platform) {
		return player.currentHeight > platform.height;
	}

	private void RaisePlayerObjects() {
//		Vector3 shadPos = shadow.transform.position;
		float raiseHeight = previousPlatform != null && previousPlatform != player.nextPlatform
			                    ? player.nextPlatform.height - previousPlatform.height
			                    : player.nextPlatform.height;
//		Debug.Log("raising=" + raiseHeight);
//		Debug.Log("nextPlatform: " + nextPlatform.name + ", current: " + previousPlatform.name +  ", " + raiseHeight);
//		if (!jumping) return;
		switch(fallingDirection) {
			case Direction.Up:
				transform.position += new Vector3(0, raiseHeight, 0);
				break;
			case Direction.Down:
				Debug.Log("Raising facing Down");
				transform.position += new Vector3(0, raiseHeight - BOUND_CORRECTION, 0);
				break;
			case Direction.Left:
				transform.position += new Vector3(0, raiseHeight, 0);
				break;
			case Direction.Right:
				transform.position += new Vector3(0, raiseHeight, 0);
				break;
			case Direction.UpLeft:
				transform.position += new Vector3(-BOUND_CORRECTION, raiseHeight + BOUND_CORRECTION, 0);
				break;
			case Direction.UpRight:
				transform.position += new Vector3(BOUND_CORRECTION, raiseHeight + BOUND_CORRECTION, 0);
				break;
			case Direction.DownLeft:
				transform.position += new Vector3(-BOUND_CORRECTION, raiseHeight - BOUND_CORRECTION, 0);
				break;
			case Direction.DownRight:
				transform.position += new Vector3(BOUND_CORRECTION, raiseHeight - BOUND_CORRECTION, 0);
				break;
			case Direction.Null:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Fall() {
		// Check which way to pad landing to prevent getting stuck
		switch (fallingDirection) {
			case Direction.Up:
				transform.position += new Vector3(0, -fallingHeight + BOUND_CORRECTION, 0);
				break;
			case Direction.Down:
				transform.position += new Vector3(0, -fallingHeight - BOUND_CORRECTION, 0);
				break;
			case Direction.Left:
				transform.position += new Vector3(0, -fallingHeight, 0);
				break;
			case Direction.Right:
				transform.position += new Vector3(0, -fallingHeight, 0);
				break;
			case Direction.UpLeft:
				transform.position += new Vector3(-BOUND_CORRECTION, -fallingHeight, 0);
				break;
			case Direction.UpRight:
				transform.position += new Vector3(BOUND_CORRECTION, -fallingHeight, 0);
				break;
			case Direction.DownLeft:
				transform.position += new Vector3(-BOUND_CORRECTION, -fallingHeight, 0);
				break;
			case Direction.DownRight:
				transform.position += new Vector3(BOUND_CORRECTION - fallingHeight, 0);
				break;
			case Direction.Null:
				break;
			default: 
				throw new ArgumentOutOfRangeException();
		}
	}

	private void InitPlatform() {
		myColl.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
		player.currentPlatform = pResultsDown[0].collider.GetComponent<ObjectInfo>();
		Debug.LogFormat("Initial platform: {0}", player.currentPlatform.name);
	}
	
	/// <summary>
	///	Check the height of the floor that the player is trying to fall down to
	/// </summary>
	/// <returns>Height to displace current height by</returns>
	private float CalculateFallingHeight(Direction direction) {
//		if (previousPlatform != null 
//		    && shadow.GetComponent<Collider2D>().Raycast(Vector2.down, floorContactFilter, floorHits, Mathf.Infinity) > 0) {
//			return previousPlatform.height - floorHits[0].transform.GetComponent<ObjectInfo>().height;
//		}
		return previousPlatform.height - player.nextPlatform.height;
//		switch (direction) {
//			case Direction.Null:
//				break;
//			case Direction.Up:
////				if(previousPlatform != null && nextPlatform != null 
////				                            && Mathf.Abs(previousPlatform.baseTopBound - nextPlatform.baseBottomBound) < Mathf.Epsilon) {
////					return previousPlatform.height - nextPlatform.height;
////				}
//				return previousPlatform.height - nextPlatform.height;
//				break;
//			case Direction.Down:
//				return previousPlatform.height - nextPlatform.height;
////				if(previousPlatform != null && nextPlatform != null 
////				    && Mathf.Abs(previousPlatform.baseBottomBound - nextPlatform.baseTopBound) < Mathf.Epsilon) {
////					return previousPlatform.height - nextPlatform.height;
////				}
//				break;
//			case Direction.Left:
//				break;
//			case Direction.Right:
//				break;
//			case Direction.UpRight:
//				break;
//			case Direction.UpLeft:
//				break;
//			case Direction.DownRight:
//				break;
//			case Direction.DownLeft:
//				break;
//			default:
//				throw new ArgumentOutOfRangeException("direction", direction, null);
//		}
//		// else return default heights
//		Debug.Log("\t||||| Falling default height |||||");
//		return previousPlatform != null ? previousPlatform.height : player.currentHeight;
	}
}
