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

public class PlayerController : MonoBehaviour {


	[Header("Direction Variables")]
	public Direction facingDirection = Direction.Down;
	public bool isWalking;

	[Header("Physics Variables")]
	public float jumpSpeed = 30.0f;
	public float overworldSpeed = 0.35f;

	// Game Objects
	private Camera cam;
	private Animator animator;
	private BoxCollider2D collider;
	private Rigidbody2D rb2d;
	private ContactFilter2D wallContactFilter;
	private ContactFilter2D platformContactFilter;
	private PlayerShadow shadow;
	private GameObject jumpHeight;

	// Physics stuff
	private bool[] isDirectionBlocked = new bool[(int)Direction.DownLeft+1];
	private bool[] isPDirectionBlocked = new bool[(int)Direction.DownLeft + 1];
	private RaycastHit2D[] resultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] resultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] resultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] resultsLeft = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsUp = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsDown = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsRight = new RaycastHit2D[3];
	private RaycastHit2D[] pResultsLeft = new RaycastHit2D[3];
	//private RaycastHit2D[] resultsUpRight = new RaycastHit2D[3];
	//private RaycastHit2D[] resultsUpLeft = new RaycastHit2D[3];
	//private RaycastHit2D[] resultsDownRight = new RaycastHit2D[3];
	//private RaycastHit2D[] resultsDownLeft = new RaycastHit2D[3];
	private float yOffset;
	private float yRadius;
	private float xRadius;
	private Vector3 groundPosition;
	private float groundHeight;
	private bool grounded = true;
	private bool jumping;
	private bool isFalling;
	private float currentHeight;

	// Coroutines
	private Coroutine jumper;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
		cam = Camera.main;
		collider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		//wallContactFilter = new ContactFilter2D();
		wallContactFilter.SetLayerMask(LayerMask.GetMask("Wall"));
		platformContactFilter.SetLayerMask(LayerMask.GetMask("Platform"));
		yOffset = Mathf.Abs(collider.offset.y);
		yRadius = collider.size.y / 2;
		xRadius = collider.size.x / 2;
		Debug.Log(string.Format("coll offset: {0}",yOffset+yRadius));
		shadow = transform.parent.GetComponentInChildren<PlayerShadow>();
		jumpHeight = GameObject.FindGameObjectWithTag("JumpHeight");
	}

	void Update() {
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
		cam.transform.position = new Vector3((float)Math.Round(shadow.transform.position.x, 2)
		+ 0.005f, (float)Math.Round(shadow.transform.transform.position.y+2, 2)
			+ 0.005f, cam.transform.position.z);

		//cam.transform.position = newCamPos;
		MovePlayer();
		groundPosition = shadow.transform.position + new Vector3(0, 2, 0);
		//groundHeight = shadow.totalHeight;
		//Debug.Log("original pos: " + groundHeight);
		//Debug.Log("player pos: " + transform.position);
	}

	// Jumping routines
	private void InputJump() {
		if(Input.GetButtonDown("Jump")) {
			//groundHeight += 4;
			if(grounded) {
				//groundPosition = transform.position;
				jumping = true;
				jumper = StartCoroutine(Jump());
			}
			if(transform.position == groundPosition) {
				grounded = true;
			} else {
				grounded = false;
			}
		}
	}

	IEnumerator Jump() {
		while (true) {
			if (transform.position.y >= jumpHeight.transform.position.y) {
				jumping = false;
			}
			if (jumping) {
				//transform.Translate(Vector3.up * jumpSpeed * Time.smoothDeltaTime);
				//Vector3 maxHeight = new Vector3(groundPosition.x, groundPosition.y + maxJumpHeight, groundPosition.z);
				transform.position = Vector3.MoveTowards(transform.position, jumpHeight.transform.position,
						jumpSpeed * Time.deltaTime);
				// Calculate current height when jumping
				currentHeight = transform.position.y - groundPosition.y;
				Debug.Log(string.Format("curr h: {0}", currentHeight));
			} else {
				transform.position = Vector3.MoveTowards(transform.position, groundPosition,
					jumpSpeed * Time.deltaTime);
				currentHeight = transform.position.y - groundPosition.y;
				Debug.Log(string.Format("curr h: {0}", currentHeight));
				if (transform.position == groundPosition) {
					grounded = true;
					StopCoroutine(jumper);
				}
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
			// TODO:
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

	private void TryBlockPlatformDirections(Rigidbody2D targetRb2d, Vector2 direction) {
		//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
		//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
		//resultsLeft[0].distance));
		switch (facingDirection) {
			case Direction.Up:
				rb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity);
				// Block all upward directions to prevent sliding into walls
				//if (Mathf.Abs(resultsUp[0].distance) < 0.5f) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.UpLeft] = true;
				//	isDirectionBlocked[(int)Direction.UpRight] = true;
				//} else {
				//	isDirectionBlocked[(int)facingDirection] = false;
				//}
				break;
			case Direction.Down:
				rb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
				// Block all downward ...
				//if (Mathf.Abs(resultsDown[0].distance) < 0.5f) {
				//	isDirectionBlocked[(int)facingDirection] = true;
				//	isDirectionBlocked[(int)Direction.DownLeft] = true;
				//	isDirectionBlocked[(int)Direction.DownRight] = true;
				//} else {
				//	isDirectionBlocked[(int)facingDirection] = false;
				//}
				break;
			case Direction.Right:
				targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity);
				// ...
				if (Mathf.Abs(pResultsRight[0].distance) < 0.5f) {
					isPDirectionBlocked[(int)facingDirection] = true;
					isPDirectionBlocked[(int)Direction.UpRight] = true;
					isPDirectionBlocked[(int)Direction.DownRight] = true;
				} else {
					isPDirectionBlocked[(int)facingDirection] = false;
					//isPDirectionBlocked[(int)Direction.UpRight] = false;
					//isPDirectionBlocked[(int)Direction.DownRight] = false;
				}
				//bool isBlocked = Mathf.Abs(pResultsRight[0].distance) < 0.5f;
				//isPDirectionBlocked[(int)facingDirection] = isBlocked;
				//isPDirectionBlocked[(int)Direction.UpRight] = isBlocked;
				//isPDirectionBlocked[(int)Direction.DownRight] = isBlocked;
				break;
			case Direction.Left:
				//targetRb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity);
				//// ...
				//if (Mathf.Abs(pResultsLeft[0].distance) < 0.5f) {
				//	isPDirectionBlocked[(int)facingDirection] = true;
				//	isPDirectionBlocked[(int)Direction.UpLeft] = true;
				//	isPDirectionBlocked[(int)Direction.DownLeft] = true;
				//} else {
				//	isPDirectionBlocked[(int)facingDirection] = false;
				//}
				break;
			case Direction.UpRight:
				//int upRightHit = targetRb2d.Cast(new Vector2(1, 1), platformContactFilter, pr, Mathf.Infinity);
				int upHit = targetRb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity);
				int rightHit = targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity);
				// ...
				//if (targetRb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity) > 0 &&
				//targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity) > 0) {
				if (upHit > 0 && rightHit > 0) {
					if (Mathf.Abs(pResultsUp[0].distance) < 0.5f) {
						isPDirectionBlocked[(int)facingDirection] = true;
						isPDirectionBlocked[(int)Direction.Up] = true;
					} else {
						// Only unblock the diagonal if not blocked my multiple things
						if (!isPDirectionBlocked[(int)Direction.Right]) {
							isPDirectionBlocked[(int)facingDirection] = false;
						}
						isPDirectionBlocked[(int)Direction.Up] = false;
					}
					if (Mathf.Abs(pResultsRight[0].distance) < 0.5f) {
						isPDirectionBlocked[(int)facingDirection] = true;
						isPDirectionBlocked[(int)Direction.Right] = true;
					} else {
						// Only unblock the diagonal if not blocked my multiple things
						if (!isPDirectionBlocked[(int)Direction.Up]) {
							isPDirectionBlocked[(int)facingDirection] = false;
						}
						isPDirectionBlocked[(int)Direction.Right] = false;
					}
					// Correct position if hit a corner
					if (Mathf.Abs(pResultsUp[0].distance) < Mathf.Epsilon
					&& Mathf.Abs(pResultsRight[0].distance) < Mathf.Epsilon) {
						Debug.Log("\t\tMoving down...");
						MoveInDirection(Direction.Down, 0.2f);
					}
				}
				break;
			//case Direction.UpLeft:
			//	//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
			//	targetRb2d.Cast(Vector2.up, platformContactFilter, pResultsUp, Mathf.Infinity);
			//	targetRb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity);
			//	// ...
			//	if (Mathf.Abs(pResultsUp[0].distance) < 0.5f) {
			//		isPDirectionBlocked[(int)facingDirection] = true;
			//		isPDirectionBlocked[(int)Direction.Up] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isPDirectionBlocked[(int)Direction.Left]) {
			//			isPDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isPDirectionBlocked[(int)Direction.Up] = false;
			//	}
			//	if (Mathf.Abs(resultsLeft[0].distance) < 0.5f) {
			//		isPDirectionBlocked[(int)facingDirection] = true;
			//		isPDirectionBlocked[(int)Direction.Left] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isPDirectionBlocked[(int)Direction.Up]) {
			//			isPDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isPDirectionBlocked[(int)Direction.Left] = false;
			//	}
			//	// Correct position if hit a corner
			//	if (Mathf.Abs(pResultsUp[0].distance) < Mathf.Epsilon
			//	&& Mathf.Abs(pResultsLeft[0].distance) < Mathf.Epsilon) {
			//		Debug.Log("\t\tMoving down...");
			//		MoveInDirection(Direction.Down, 0.2f);
			//	}
			//	break;
			//case Direction.DownRight:
			//	//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
			//	targetRb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
			//	targetRb2d.Cast(Vector2.right, platformContactFilter, pResultsRight, Mathf.Infinity);
			//	// ...
			//	if (Mathf.Abs(pResultsDown[0].distance) < 0.5f) {
			//		isPDirectionBlocked[(int)facingDirection] = true;
			//		isPDirectionBlocked[(int)Direction.Down] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isPDirectionBlocked[(int)Direction.Right]) {
			//			isPDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isPDirectionBlocked[(int)Direction.Down] = false;
			//	}
			//	if (Mathf.Abs(pResultsRight[0].distance) < 0.5f) {
			//		isPDirectionBlocked[(int)facingDirection] = true;
			//		isPDirectionBlocked[(int)Direction.Right] = true;
			//	} else {
			//		// Only unblock the diagonal if not blocked my multiple things
			//		if (!isPDirectionBlocked[(int)Direction.Down]) {
			//			isPDirectionBlocked[(int)facingDirection] = false;
			//		}
			//		isPDirectionBlocked[(int)Direction.Right] = false;
			//	}
			//	// Correct position if hit a corner
			//	if (Mathf.Abs(pResultsDown[0].distance) < Mathf.Epsilon
			//	&& Mathf.Abs(pResultsRight[0].distance) < Mathf.Epsilon) {
			//		Debug.Log("\t\tMoving up...");
			//		MoveInDirection(Direction.Up, 0.2f);
			//	}
			//	break;
			//case Direction.DownLeft:
				////rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
				//targetRb2d.Cast(Vector2.down, platformContactFilter, pResultsDown, Mathf.Infinity);
				//targetRb2d.Cast(Vector2.left, platformContactFilter, pResultsLeft, Mathf.Infinity);
				//// ...
				//if (Mathf.Abs(pResultsDown[0].distance) < 0.5f) {
				//	isPDirectionBlocked[(int)facingDirection] = true;
				//	isPDirectionBlocked[(int)Direction.Down] = true;
				//} else {
				//	// Only unblock the diagonal if not blocked my multiple things
				//	if (!isPDirectionBlocked[(int)Direction.Left]) {
				//		isPDirectionBlocked[(int)facingDirection] = false;
				//	}
				//	isPDirectionBlocked[(int)Direction.Down] = false;
				//}
				//if (Mathf.Abs(pResultsLeft[0].distance) < 0.5f) {
				//	isPDirectionBlocked[(int)facingDirection] = true;
				//	isPDirectionBlocked[(int)Direction.Left] = true;
				//} else {
				//	// Only unblock the diagonal if not blocked my multiple things
				//	if (!isPDirectionBlocked[(int)Direction.Down]) {
				//		isPDirectionBlocked[(int)facingDirection] = false;
				//	}
				//	isPDirectionBlocked[(int)Direction.Left] = false;
				//}
				//// Correct position if hit a corner
				//if (Mathf.Abs(pResultsDown[0].distance) < Mathf.Epsilon
				//&& Mathf.Abs(pResultsLeft[0].distance) < Mathf.Epsilon) {
				//	Debug.Log("\t\tMoving up...");
				//	MoveInDirection(Direction.Up, 0.2f);
				//}
				//break;

		}
	}

	// TODO: handle case where more than half of collider is past the wall
	// 		 Need to correct this and move player diagonally until they are no longer blocked
	private void TryBlockDirections(Rigidbody2D rb2d, Vector2 direction) {
		//Debug.Log(string.Format("up:{0}, down:{1}, right:{2}, left:{3}",
		//resultsUp[0].distance, resultsDown[0].distance, resultsRight[0].distance,
			//resultsLeft[0].distance));
		//Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
		switch (facingDirection) {
			case Direction.Up:
				rb2d.Cast(Vector2.up, wallContactFilter, resultsUp, Mathf.Infinity);
				// Block all upward directions to prevent sliding into walls
				if (Mathf.Abs(resultsUp[0].distance) < 0.5f) {
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpLeft] = true;
					isDirectionBlocked[(int)Direction.UpRight] = true;
				} else {
					isDirectionBlocked[(int)facingDirection] = false;
				}
				break;
			// TODO: handle case when going down (behind a platform)
			case Direction.Down:
				rb2d.Cast(Vector2.down, wallContactFilter, resultsDown, Mathf.Infinity);
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
				rb2d.Cast(Vector2.right, wallContactFilter, resultsRight, Mathf.Infinity);
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
				rb2d.Cast(Vector2.left, wallContactFilter, resultsLeft, Mathf.Infinity);
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
				//rb2d.Cast(new Vector2(1, 1), contactFilter, resultsUpRight, Mathf.Infinity);
				rb2d.Cast(Vector2.up, wallContactFilter, resultsUp, Mathf.Infinity);
				rb2d.Cast(Vector2.right, wallContactFilter, resultsRight, Mathf.Infinity);
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
				// Correct position if hit a corner
				if(Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon 
				&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon) {
					Debug.Log("\t\tMoving down...");
					MoveInDirection(Direction.Down, 0.2f);
				}
				break;
			case Direction.UpLeft:
				//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
				rb2d.Cast(Vector2.up, wallContactFilter, resultsUp, Mathf.Infinity);
				rb2d.Cast(Vector2.left, wallContactFilter, resultsLeft, Mathf.Infinity);
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
				// Correct position if hit a corner
				if (Mathf.Abs(resultsUp[0].distance) < Mathf.Epsilon
				&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon) {
					Debug.Log("\t\tMoving down...");
					MoveInDirection(Direction.Down, 0.2f);
				}
				break;
			case Direction.DownRight:
				//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
				rb2d.Cast(Vector2.down, wallContactFilter, resultsDown, Mathf.Infinity);
				rb2d.Cast(Vector2.right, wallContactFilter, resultsRight, Mathf.Infinity);
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
				// Correct position if hit a corner
				if (Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
				&& Mathf.Abs(resultsRight[0].distance) < Mathf.Epsilon) {
					Debug.Log("\t\tMoving up...");
					MoveInDirection(Direction.Up, 0.2f);
				}
				break;
			case Direction.DownLeft:
				//rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
				rb2d.Cast(Vector2.down, wallContactFilter, resultsDown, Mathf.Infinity);
				rb2d.Cast(Vector2.left, wallContactFilter, resultsLeft, Mathf.Infinity);
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
				// Correct position if hit a corner
				if (Mathf.Abs(resultsDown[0].distance) < Mathf.Epsilon
				&& Mathf.Abs(resultsLeft[0].distance) < Mathf.Epsilon) {
					Debug.Log("\t\tMoving up...");
					MoveInDirection(Direction.Up, 0.2f);
				}
				break;

		}
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