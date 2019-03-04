
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
	//				if (Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)) {
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
	//				if (Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)) {
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
	//				if (Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
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
	//				if (Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
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
	//			if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Up] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Right]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Up] = false;
	//			}
	//			if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
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
	//				&& currentHeight <= resultsUp[0].transform.GetComponent<ObjectHeight>().height
	//				&& currentHeight <= resultsRight[0].transform.GetComponent<ObjectHeight>().height) {
	//				Debug.Log("\t\tMoving down...");
	//				MoveInDirection(Direction.Down, 0.2f);
	//			}
	//			break;
	//		case Direction.UpLeft:
	//			//rb2d.Cast(new Vector2(-1, 1), contactFilter, resultsUpLeft, Mathf.Infinity);
	//			upHit = targetRb2d.Cast(Vector2.up, baseContactFilter, resultsUp, Mathf.Infinity);
	//			int leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
	//			// ...
	//			if (upHit > 0 && Mathf.Abs(resultsUp[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Up] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Left]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Up] = false;
	//			}
	//			if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
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
	//				&& CheckIfBlockPlayerByHeight(resultsUp[0], currentHeight)
	//				&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
	//				Debug.Log("\t\tMoving down...");
	//				MoveInDirection(Direction.Down, 0.2f);
	//			}
	//			break;
	//		case Direction.DownRight:
	//			//rb2d.Cast(new Vector2(1, -1), contactFilter, resultsDownRight, Mathf.Infinity);
	//			int downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
	//			rightHit = targetRb2d.Cast(Vector2.right, baseContactFilter, resultsRight, Mathf.Infinity);
	//			// ...
	//			if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Down] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Right]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Down] = false;
	//			}
	//			if (rightHit > 0 && Mathf.Abs(resultsRight[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
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
	//				&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)
	//				&& CheckIfBlockPlayerByHeight(resultsRight[0], currentHeight)) {
	//				Debug.Log("\t\tMoving up...");
	//				MoveInDirection(Direction.Up, 0.2f);
	//			}
	//			break;
	//		case Direction.DownLeft:
	//			//rb2d.Cast(new Vector2(-1, -1), contactFilter, resultsDownLeft, Mathf.Infinity);
	//			downHit = targetRb2d.Cast(Vector2.down, baseContactFilter, resultsDown, Mathf.Infinity);
	//			leftHit = targetRb2d.Cast(Vector2.left, baseContactFilter, resultsLeft, Mathf.Infinity);
	//			// ...
	//			if (downHit > 0 && Mathf.Abs(resultsDown[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)) {
	//				isDirectionBlocked[(int)facingDirection] = true;
	//				isDirectionBlocked[(int)Direction.Down] = true;
	//			} else {
	//				// Only unblock the diagonal if not blocked my multiple things
	//				if (!isDirectionBlocked[(int)Direction.Left]) {
	//					isDirectionBlocked[(int)facingDirection] = false;
	//				}
	//				isDirectionBlocked[(int)Direction.Down] = false;
	//			}
	//			if (leftHit > 0 && Mathf.Abs(resultsLeft[0].distance) < boundCorrection && CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
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
	//				&& CheckIfBlockPlayerByHeight(resultsDown[0], currentHeight)
	//				&& CheckIfBlockPlayerByHeight(resultsLeft[0], currentHeight)) {
	//				Debug.Log("\t\tMoving up...");
	//				MoveInDirection(Direction.Up, 0.2f);
	//			}
	//			break;

	//	}
	//}
