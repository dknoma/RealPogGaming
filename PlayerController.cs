using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public enum Direction { Down, DownRight, Right, UpRight, Up, UpLeft, Left, DownLeft };

	public Direction facingDirection = Direction.Down;
	public Direction blockedDirection;
	public bool isWalking;

	private Camera cam;
	private Animator animator;
	private bool isFalling;
	//private bool isBlocked;
	private BoxCollider2D collider;
	private Rigidbody2D rb2d;
	private ContactFilter2D contactFilter;
	private bool[] isDirectionBlocked = new bool[(int)Direction.DownLeft+1];
	private RaycastHit2D[] resultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] resultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] resultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] resultsLeft = new RaycastHit2D[3];
	private float yOffset;
	private float yRadius;
	private float xRadius;


	private readonly float overworldSpeed = 0.35f;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
		cam = Camera.main;
		collider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		contactFilter = new ContactFilter2D();
		LayerMask mask = LayerMask.GetMask("Wall");
		contactFilter.SetLayerMask(mask);
		yOffset = Mathf.Abs(collider.offset.y);
		yRadius = collider.size.y / 2;
		xRadius = collider.size.x / 2;
		Debug.Log(string.Format("coll offset: {0}",yOffset+yRadius));
	}

	void Update() {
		// Let camera focus on player
		Vector3 newCamPos = new Vector3((float)Math.Round(transform.position.x, 2)+0.005f, (float)Math.Round(transform.transform.position.y, 2)+0.005f, cam.transform.position.z);
		cam.transform.position = newCamPos;
		MovePlayer();
		//TryBlockDirections();
	}

	//private float TryBlockDirections(Vector2 direction) {
	//rb2d.Cast(direction, results, Mathf.Infinity);
	//collider.Cast(direction, results, Mathf.Infinity, true);

	//collider.Cast(Vector2.down, results, Mathf.Infinity, true);
	//collider.Cast(Vector2.right, results, Mathf.Infinity, true);
	//collider.Cast(Vector2.up, results1, Mathf.Infinity, true);
	//collider.Cast(Vector2.left, results2, Mathf.Infinity, true);
	//Debug.Log(string.Format("1:{0}, 2:{1}", results1[0].distance, results2[0].distance));
	//Debug.Log(string.Format("0:{0}, 1:{1}, 2:{2}, 3:{3}, 4:{4}", results[0].distance, 
	//results[1].distance, results[2].distance, results[3].distance,
	//results[4].distance));

	//float distance = results1[0].distance;
	//if(Mathf.Abs(distance) < 0.5f) {
	//private void CastRays(Direction direction) {
	//	switch (direction) {
	//		case Direction.Up:
	//			Vector2 currPos = new Vector2(transform.position.x, transform.position.y - yOffset - yRadius);
	//			Physics2D.Raycast(currPos, Vector2.up, contactFilter, resultsUp, Mathf.Infinity);
	//			Debug.Log(string.Format("hit: {0}, is touching collider?: {1}, size: {2}", resultsUp[0].distance,
	//				resultsUp[0].distance <= collider.size.y, collider.size.y));
	//			break;
	//		case Direction.Down:
	//			currPos = new Vector2(transform.position.x, transform.position.y);
	//			Physics2D.Raycast(currPos, Vector2.down, contactFilter, resultsDown, Mathf.Infinity);
	//			Debug.Log(string.Format("hit: {0}, is touching collider?: {1}, size: {2}", resultsUp[0].distance,
	//			resultsUp[0].distance <= collider.size.y, collider.size.y));
	//			break;
	//		case Direction.Left:
	//			break;
	//		case Direction.Right:
	//			break;
	//	}
	//}

	private void TryBlockDirections(Vector2 direction) {
		Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}", 
		resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
			resultsLeft[0].distance));
		//Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
		switch (facingDirection) {
			case Direction.Up:
				rb2d.Cast(Vector2.up, resultsUp, Mathf.Infinity);
				// Block all upward directions to prevent sliding into walls
				if (Mathf.Abs(resultsUp[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpLeft] = true;
					isDirectionBlocked[(int)Direction.UpRight] = true;
				} else {
					isDirectionBlocked[(int)facingDirection] = false;
				}
				break;
			case Direction.Down:
				rb2d.Cast(Vector2.down, resultsDown, Mathf.Infinity);
				// Block all downward ...
				if (Mathf.Abs(resultsDown[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.DownLeft] = true;
					isDirectionBlocked[(int)Direction.DownRight] = true;
				} else {
					isDirectionBlocked[(int)facingDirection] = false;
				}
					break;
			case Direction.Right:
				rb2d.Cast(Vector2.right, resultsRight, Mathf.Infinity);
				// ...
				if (Mathf.Abs(resultsRight[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpRight] = true;
					isDirectionBlocked[(int)Direction.DownRight] = true;
				} else {
					isDirectionBlocked[(int)facingDirection] = false;
				}
				break;
			case Direction.Left:
				rb2d.Cast(Vector2.left, resultsLeft, Mathf.Infinity);
				// ...
				if (Mathf.Abs(resultsLeft[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpLeft] = true;
					isDirectionBlocked[(int)Direction.DownLeft] = true;
				} else {
					isDirectionBlocked[(int)facingDirection] = false;
				}
				break;
			case Direction.UpRight:
				rb2d.Cast(Vector2.up, resultsUp, Mathf.Infinity);
				rb2d.Cast(Vector2.right, resultsRight, Mathf.Infinity);
				// ...
				if (Mathf.Abs(resultsUp[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Right]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Up] = false;
				}
				if (Mathf.Abs(resultsRight[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Right] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Up]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Right] = false;
				}
				break;
			case Direction.UpLeft:
				rb2d.Cast(Vector2.up, resultsUp, Mathf.Infinity);
				rb2d.Cast(Vector2.left, resultsLeft, Mathf.Infinity);
				// ...
				if (Mathf.Abs(resultsUp[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Left]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Up] = false;
				}
				if (Mathf.Abs(resultsLeft[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Left] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Up]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Left] = false;
				}
				break;
			case Direction.DownRight:
				rb2d.Cast(Vector2.down, resultsDown, Mathf.Infinity);
				rb2d.Cast(Vector2.right, resultsRight, Mathf.Infinity);
				// ...
				if (Mathf.Abs(resultsDown[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Right]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Down] = false;
				}
				if (Mathf.Abs(resultsRight[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Right] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Down]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Right] = false;
				}
				break;
			case Direction.DownLeft:
				rb2d.Cast(Vector2.down, resultsDown, Mathf.Infinity);
				rb2d.Cast(Vector2.left, resultsLeft, Mathf.Infinity);
				// ...
				if (Mathf.Abs(resultsDown[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Left]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Down] = false;
				}
				if (Mathf.Abs(resultsLeft[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Left] = true;
				} else {
					// Only unblock the diagonal if not blocked my multiple things
					if (!isDirectionBlocked[(int)Direction.Down]) {
						isDirectionBlocked[(int)facingDirection] = false;
					}
					isDirectionBlocked[(int)Direction.Left] = false;
				}
				break;

		}
	}


private void MovePlayer() {
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.5f) {
			// Facing up
			StartDirection(Direction.Up);
			if (!isDirectionBlocked[(int)Direction.Up]) { MoveInDirection(Direction.Up, overworldSpeed); }
			TryBlockDirections(Vector2.up);
		} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.5f) {
			// Facing down
			StartDirection(Direction.Down);
			if (!isDirectionBlocked[(int)Direction.Down]) { MoveInDirection(Direction.Down, overworldSpeed); }
			TryBlockDirections(Vector2.down);
		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < 0.5f) {
			// Facing right
			StartDirection(Direction.Right);
			if (!isDirectionBlocked[(int)Direction.Right]) { MoveInDirection(Direction.Right, overworldSpeed); }
			TryBlockDirections(Vector2.right);
		} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < 0.5f) {
			// Facing left
			StartDirection(Direction.Left);
			if (!isDirectionBlocked[(int)Direction.Left]) { MoveInDirection(Direction.Left, overworldSpeed); }
			TryBlockDirections(Vector2.left);
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing up-right
			StartDirection(Direction.UpRight);
			if (!isDirectionBlocked[(int)Direction.UpRight]) {
				//transform.Translate(0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpRight, overworldSpeed-0.1f);
			} else if(!isDirectionBlocked[(int)Direction.Right] && isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Up, overworldSpeed);
			}
			//TryBlockDirections(Vector2.right);
			TryBlockDirections(new Vector2(1,1));
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing up-left
			StartDirection(Direction.UpLeft);
			if (!isDirectionBlocked[(int)Direction.UpLeft]) {
				//transform.Translate(-0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpLeft, overworldSpeed - 0.1f);
			} else if (!isDirectionBlocked[(int)Direction.Left] && isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Left, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Up]) {
				MoveInDirection(Direction.Up, overworldSpeed);
			}
			//TryBlockDirections(Vector2.left);
			TryBlockDirections(new Vector2(-1, 1));
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
			//TryBlockDirections(Vector2.left);
			TryBlockDirections(new Vector2(-1, -1));
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
			//TryBlockDirections(Vector2.right);
			TryBlockDirections(new Vector2(1, -1));
		} else {
			animator.SetBool("isWalking", false);
			isWalking = false;
		}
	}

	private void MoveInDirection(Direction diretction, float speed) {
		switch(diretction) {
			case Direction.Up:
				transform.Translate(0, speed, 0);
				break;
			case Direction.Down:
				transform.Translate(0, -speed, 0);
				break;
			case Direction.Left:
				transform.Translate(-speed, 0, 0);
				break;
			case Direction.Right:
				transform.Translate(speed, 0, 0);
				break;
			case Direction.UpRight:
				transform.Translate(speed, speed, 0);
				break;
			case Direction.UpLeft:
				transform.Translate(-speed, speed, 0);
				break;
			case Direction.DownRight:
				transform.Translate(speed,-speed, 0);
				break;
			case Direction.DownLeft:
				transform.Translate(-speed,-speed, 0);
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
