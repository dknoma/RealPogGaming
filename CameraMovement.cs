using UnityEngine;

public class CameraMovement : MonoBehaviour {
	
	public float movementSpeed = 0.18f;
	
	private Camera cam;
	private PlayerController player;
	private const float MOVEMENT_WINDOW = 4.0f;
	private float fallingSpeed;
	private readonly float diagonalMovementSpeed;

	public CameraMovement() {
		diagonalMovementSpeed = Mathf.Sqrt(movementSpeed * movementSpeed / 2);	// Init diagonal movement speed
	}
	
	private void OnEnable() {
		cam = Camera.main;
		player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();    // Get the main player character
		fallingSpeed = player.jumpSpeed;
		var playerPos = player.transform.position;
		if (cam != null) { // Start the camera at the current players location each load
			transform.position = new Vector3(playerPos.x, playerPos.y, cam.transform.position.z); 
		}
	}

	private void Update() {
//        var camPos = cam.transform.position;
//		if (player.jumping) {
//			
//		} else {
			var playerPos = player.transform.position;

			// Up right
			if (playerPos.x >= transform.position.x + MOVEMENT_WINDOW
			    && playerPos.y >= transform.position.y + MOVEMENT_WINDOW) {
				transform.Translate(diagonalMovementSpeed, diagonalMovementSpeed, 0);
			// Down right
			} else if (playerPos.x >= transform.position.x + MOVEMENT_WINDOW
			           && playerPos.y <= transform.position.y - MOVEMENT_WINDOW) {
				transform.Translate(diagonalMovementSpeed, -diagonalMovementSpeed, 0);
			// Down left
			} else if (playerPos.x <= transform.position.x - MOVEMENT_WINDOW
			           && playerPos.y <= transform.position.y - MOVEMENT_WINDOW) {
				transform.Translate(-diagonalMovementSpeed, -diagonalMovementSpeed, 0);
			// Up left
			} else if (playerPos.x <= transform.position.x - MOVEMENT_WINDOW
			           && playerPos.y >= transform.position.y + MOVEMENT_WINDOW) {
				transform.Translate(-diagonalMovementSpeed, diagonalMovementSpeed, 0);
			} else {
				if (playerPos.x >  transform.position.x + MOVEMENT_WINDOW) {
					transform.Translate(movementSpeed, 0, 0);
				} else if (playerPos.x  < transform.position.x - MOVEMENT_WINDOW) {
					transform.Translate(-movementSpeed, 0, 0);
				}

				if (player.isFalling) {
					if (playerPos.y > transform.position.y + MOVEMENT_WINDOW) {
						transform.Translate(0, movementSpeed, 0);
					} else if (playerPos.y < transform.position.y - MOVEMENT_WINDOW) {
						transform.Translate(0, -fallingSpeed, 0);
					}
				} else {
					if (playerPos.y > transform.position.y + MOVEMENT_WINDOW) {
						transform.Translate(0, movementSpeed, 0);
					} else if (playerPos.y < transform.position.y - MOVEMENT_WINDOW) {
						transform.Translate(0, -movementSpeed, 0);
					}
				}
			}
//		}
	}
}
