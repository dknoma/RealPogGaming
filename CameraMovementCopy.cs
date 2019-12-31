// using UnityEngine;

// public class CameraMovementCopy : MonoBehaviour {
	
// 	private float movementSpeed = 23;
// 	private Camera cam;
// 	private TestPhysicsController player;
// 	private const float MOVEMENT_WINDOW = 4.0f;
// //	private float fallingSpeed;
// 	private float diagonalMovementSpeed;

// 	private void OnEnable() {
// 		cam = Camera.main;
// 		player = GameObject.FindWithTag("Player").GetComponent<TestPhysicsController>();    // Get the main player character
// 		movementSpeed = player.overworldSpeed;
// 		diagonalMovementSpeed = Mathf.Sqrt(movementSpeed * movementSpeed / 2); // Init diagonal movement speed
// //		fallingSpeed = player.jumpSpeed;
// 		Vector3 playerPos = player.transform.position;
// 		if (cam != null) { // Start the camera at the current players location each load
// 			transform.position = new Vector3(playerPos.x, playerPos.y, cam.transform.position.z); 
// 		}
// 	}

// 	private void FixedUpdate() {
// //        var camPos = cam.transform.position;
// //		if (player.jumping) {
// //			
// //		} else {
// 			Vector3 playerPos = player.transform.position;

// 			// Up right
// 			if (playerPos.x >= transform.position.x + MOVEMENT_WINDOW
// 			    && playerPos.y >= transform.position.y + MOVEMENT_WINDOW) {
// //				transform.Translate(diagonalMovementSpeed, diagonalMovementSpeed, 0);
// 				transform.position += new Vector3(diagonalMovementSpeed, diagonalMovementSpeed, 0) * Time.fixedDeltaTime;
// 			// Down right
// 			} else if (playerPos.x >= transform.position.x + MOVEMENT_WINDOW
// 			           && playerPos.y <= transform.position.y - MOVEMENT_WINDOW) {
// //				transform.Translate(diagonalMovementSpeed, -diagonalMovementSpeed, 0);
// 				transform.position += new Vector3(diagonalMovementSpeed, -diagonalMovementSpeed, 0) * Time.fixedDeltaTime;
// 			// Down left
// 			} else if (playerPos.x <= transform.position.x - MOVEMENT_WINDOW
// 			           && playerPos.y <= transform.position.y - MOVEMENT_WINDOW) {
// //				transform.Translate(-diagonalMovementSpeed, -diagonalMovementSpeed, 0);
// 				transform.position += new Vector3(-diagonalMovementSpeed, -diagonalMovementSpeed, 0) * Time.fixedDeltaTime;
// 			// Up left
// 			} else if (playerPos.x <= transform.position.x - MOVEMENT_WINDOW
// 			           && playerPos.y >= transform.position.y + MOVEMENT_WINDOW) {
// //				transform.Translate(-diagonalMovementSpeed, diagonalMovementSpeed, 0);
// 				transform.position += new Vector3(-diagonalMovementSpeed, diagonalMovementSpeed, 0) * Time.fixedDeltaTime;
// 			} else {
// 				if (playerPos.x >  transform.position.x + MOVEMENT_WINDOW) {
// //					transform.Translate(movementSpeed, 0, 0);
// 					transform.position += new Vector3(movementSpeed, 0, 0) * Time.fixedDeltaTime;
// 				} else if (playerPos.x  < transform.position.x - MOVEMENT_WINDOW) {
// //					transform.Translate(-movementSpeed, 0, 0);
// 					transform.position += new Vector3(-movementSpeed, 0, 0) * Time.fixedDeltaTime;
// 				}

// //				if (player.isFalling) {
// 					if (playerPos.y > transform.position.y + MOVEMENT_WINDOW) {
// 						transform.position += new Vector3(0, movementSpeed, 0) * Time.fixedDeltaTime;
// //						transform.Translate(0, movementSpeed, 0);
// 					} else if (playerPos.y < transform.position.y - MOVEMENT_WINDOW) {
// 						transform.position += new Vector3(0, -movementSpeed, 0) * Time.fixedDeltaTime;
// //						transform.Translate(0, -fallingSpeed, 0);
// 					}
// //				} else {
// //					if (playerPos.y > transform.position.y + MOVEMENT_WINDOW) {
// //						transform.position += new Vector3(0, movementSpeed, 0) * Time.fixedDeltaTime;
// ////						transform.Translate(0, movementSpeed, 0);
// //					} else if (playerPos.y < transform.position.y - MOVEMENT_WINDOW) {
// //						transform.position += new Vector3(0, -movementSpeed, 0) * Time.fixedDeltaTime;
// ////						transform.Translate(0, -movementSpeed, 0);
// //					}
// //				}
// 			}
// //		}
// 	}
// }
