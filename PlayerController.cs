using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public enum Direction { Down, DownRight, Right, TopRight, Top, TopLeft, Left, DownLeft };

	public Direction facingDirection = Direction.Down;
	public bool isWalking;

	private Camera cam;
	private Animator animator;
	private bool isFalling;
	private bool isBlocked;

	private readonly float overworldSpeed = 0.35f;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		animator.ResetTrigger("changeDirection");
		cam = Camera.main;
	}

	void Update() {
		// Let camera focus on player
		Vector3 newCamPos = new Vector3((float)Math.Round(transform.position.x, 2)+0.005f, (float)Math.Round(transform.transform.position.y, 2)+0.005f, cam.transform.position.z);
		cam.transform.position = newCamPos;
		MovePlayer();
	}

	private void MovePlayer() {
		//Debug.Log(string.Format("v: {0}, h {1}", Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")));

		if (Input.GetAxisRaw("Vertical") > 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing up
			StartDirection(Direction.Top);
			if (!isBlocked) { MoveInDirection(Direction.Top, overworldSpeed); }
		} else if (Input.GetAxisRaw("Vertical") < 0 && Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Mathf.Epsilon) {
			// Facing down
			StartDirection(Direction.Down);
			if (!isBlocked) { MoveInDirection(Direction.Down, overworldSpeed); }
		} else if (Input.GetAxisRaw("Horizontal") > 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing right
			StartDirection(Direction.Right);
			if (!isBlocked) { MoveInDirection(Direction.Right, overworldSpeed); }
		} else if (Input.GetAxisRaw("Horizontal") < 0 && Mathf.Abs(Input.GetAxisRaw("Vertical")) < Mathf.Epsilon) {
			// Facing left
			StartDirection(Direction.Left);
			if (!isBlocked) { MoveInDirection(Direction.Left, overworldSpeed); }
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing up-right
			StartDirection(Direction.TopRight);
			if (!isBlocked) {
				//transform.Translate(0.35f, 0.35f, 0);
				MoveInDirection(Direction.TopRight, overworldSpeed);
			} else {
				MoveInDirection(Direction.Right, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") > 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing up-left
			StartDirection(Direction.TopLeft);
			if (!isBlocked) {
				//transform.Translate(-0.35f, 0.35f, 0);
				MoveInDirection(Direction.TopLeft, overworldSpeed);
			} else {
				MoveInDirection(Direction.Left, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") < 0) {
			// Facing down-left
			StartDirection(Direction.DownLeft);
			if (!isBlocked) {
				//transform.Translate(-0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownLeft, overworldSpeed);
			} else {
				MoveInDirection(Direction.Left, overworldSpeed);
			}
		} else if (Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") > 0) {
			// Facing down-right
			StartDirection(Direction.DownRight);
			if (!isBlocked) { 
				//transform.Translate(0.35f, -0.35f, 0);
				MoveInDirection(Direction.DownRight, overworldSpeed);
			} else {
				MoveInDirection(Direction.Right, overworldSpeed);
			}
		} else {
			animator.SetBool("isWalking", false);
			isWalking = false;
		}
	}

	private void MoveInDirection(Direction diretction, float speed) {
		switch(diretction) {
			case Direction.Top:
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
			case Direction.TopRight:
				transform.Translate(speed, speed, 0);
				break;
			case Direction.TopLeft:
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
