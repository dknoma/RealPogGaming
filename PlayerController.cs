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
	}

	void Update() {
		// Let camera focus on player
		Vector3 newCamPos = new Vector3((float)Math.Round(transform.position.x, 2)+0.005f, (float)Math.Round(transform.transform.position.y, 2)+0.005f, cam.transform.position.z);
		cam.transform.position = newCamPos;
		MovePlayer();
		//TryBlockDirections();
	}

	private void TryBlockDirections(Vector2 direction) {
	//private void TryBlockDirections() {
		RaycastHit2D[] results = new RaycastHit2D[5];
		//rb2d.Cast(Vector2.up, results, Mathf.Infinity);
		//rb2d.Cast(Vector2.left, results, Mathf.Infinity);
		rb2d.Cast(direction, results, Mathf.Infinity);
		//collider.Cast(direction, results, Mathf.Infinity, true);
		//collider.Cast(Vector2.up, results, Mathf.Infinity, true);
		//collider.Cast(Vector2.down, results, Mathf.Infinity, true);
		//collider.Cast(Vector2.right, results, Mathf.Infinity, true);
		//collider.Cast(Vector2.left, results, Mathf.Infinity, true);
		Debug.Log(string.Format("0:{0}, 1:{1}, 2:{2}, 3:{3}, 4:{4}", results[0].distance, 
		results[1].distance, results[2].distance, results[3].distance,
			results[4].distance));
		float distance = results[0].distance;
		if(Mathf.Abs(distance) < Mathf.Epsilon) {
			switch(facingDirection) {
				case Direction.Up:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpLeft] = true;
					isDirectionBlocked[(int)Direction.UpRight] = true;
					break;
				case Direction.Down:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.DownLeft] = true;
					isDirectionBlocked[(int)Direction.DownRight] = true;
					break;
				case Direction.Right:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpRight] = true;
					isDirectionBlocked[(int)Direction.DownRight] = true;
					break;
				case Direction.Left:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.UpLeft] = true;
					isDirectionBlocked[(int)Direction.DownLeft] = true;
					break;
				case Direction.UpRight:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
					//if(!isDirectionBlocked[(int)Direction.Up]) {
					//	isDirectionBlocked[(int)Direction.Right] = true;
					//}
					break;
				case Direction.UpLeft:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Up] = true;
					//if (!isDirectionBlocked[(int)Direction.Up]) {
						isDirectionBlocked[(int)Direction.Left] = true;
					//}
					//if (!isDirectionBlocked[(int)Direction.Left]) {
					//	isDirectionBlocked[(int)Direction.Up] = true;
					//} 
					break;
				case Direction.DownRight:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
					//isDirectionBlocked[(int)Direction.Right] = true;
					break;
				case Direction.DownLeft:
					isDirectionBlocked[(int)facingDirection] = true;
					isDirectionBlocked[(int)Direction.Down] = true;
					//isDirectionBlocked[(int)Direction.DownLeft] = true;
					break;

			}
		} else {
			for (int i = 0; i < isDirectionBlocked.Length; i++) {
				isDirectionBlocked[i] = false;
			}
		}
	}

	//private void OnCollisionStay2D(Collision2D collision) {
	//	if(collision.gameObject.CompareTag("Platform")) {
	//		Debug.Log("Touching a platform.");
	//		//int contacts = collision.contactCount;
	//		//Debug.Log(string.Format("contacts: {0}", contacts));
	//		ContactPoint2D[] contactPoints = new ContactPoint2D[10];
	//		collider.GetContacts(contactPoints);
	//		Debug.Log(string.Format("contacts: {0}", contactPoints[0].point));
	//		//transform.position.x
	//		//isBlocked = true;
	//		isDirectionBlocked[(int)facingDirection] = true;
	//		//transform.
	//		//blockedDirection = facingDirection;
	//	}
	//}

	//private void OnCollisionExit(Collision collision) {
	//	if (collision.gameObject.CompareTag("Platform")) {
	//		Debug.Log("Not touching a platform.");
	//		//isBlocked = false;
	//		//isDirectionBlocked[(int)blockedDirection] = false;
	//		//isDirectionBlocked[(int)facingDirection] = false;
	//		for(int i = 0; i < isDirectionBlocked.Length; i++) {
	//			isDirectionBlocked[i] = false;
	//		}
	//	}
	//}

	private void MovePlayer() {
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));
		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing up
			StartDirection(Direction.Up);
			if (!isDirectionBlocked[(int)Direction.Up]) { MoveInDirection(Direction.Up, overworldSpeed); }
			TryBlockDirections(Vector2.up);
		} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing down
			StartDirection(Direction.Down);
			if (!isDirectionBlocked[(int)Direction.Down]) { MoveInDirection(Direction.Down, overworldSpeed); }
			TryBlockDirections(Vector2.down);
		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing right
			StartDirection(Direction.Right);
			if (!isDirectionBlocked[(int)Direction.Right]) { MoveInDirection(Direction.Right, overworldSpeed); }
			TryBlockDirections(Vector2.right);
		} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing left
			StartDirection(Direction.Left);
			if (!isDirectionBlocked[(int)Direction.Left]) { MoveInDirection(Direction.Left, overworldSpeed); }
			TryBlockDirections(Vector2.left);
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing up-right
			StartDirection(Direction.UpRight);
			if (!isDirectionBlocked[(int)Direction.UpRight]) {
				//transform.Translate(0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpRight, overworldSpeed);
			} else if(!isDirectionBlocked[(int)Direction.Right]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else {
				MoveInDirection(Direction.Up, overworldSpeed);
			}
			//TryBlockDirections(Vector2.right);
			TryBlockDirections(new Vector2(1,1));
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing up-left
			StartDirection(Direction.UpLeft);
			if (!isDirectionBlocked[(int)Direction.UpLeft]) {
				//transform.Translate(-0.35f, 0.35f, 0);
				MoveInDirection(Direction.UpLeft, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left]) {
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
				MoveInDirection(Direction.DownLeft, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Left]) {
				MoveInDirection(Direction.Left, overworldSpeed);
			} else {
				MoveInDirection(Direction.Down, overworldSpeed);
			}
			//TryBlockDirections(Vector2.left);
			TryBlockDirections(new Vector2(-1, -1));
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing down-right
			StartDirection(Direction.DownRight);
			if (!isDirectionBlocked[(int)Direction.DownRight]) { 
				//transform.Translate(0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownRight, overworldSpeed);
			} else if (!isDirectionBlocked[(int)Direction.Right]) {
				MoveInDirection(Direction.Right, overworldSpeed);
			} else {
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
