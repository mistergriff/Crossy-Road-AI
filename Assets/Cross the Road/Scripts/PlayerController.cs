using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

	public float rightBound = 5.5f;					// Player's right bound.
	public float leftBound = -4.5f;					// Player's left bound.
	public float snapToGridDistance = 0.05f;	// Once the player is almost at the location, snap to the location.
	public Animator anim;						// Used for triggering animations.
	public float smooth = 10.0f;					// Smooth player movement.
	public float speed = 1.0f;					// Player speed.
	public GameObject colorMesh;                // Mesh of the player. Used to set the color.
	public GameObject mesh;						// Mesh of the player. Used to rotate when moving.

	bool moving = false;						// Is the player currently moving?
	Vector3 jumpDestination;					// Where to jump to.
	Vector3 queuedJumpDestination;				// Where to jump to after the current jump finishes.
	Vector3 jumpStart;							// Where the player's current jump started.
	float startTime;							// When the player's current jump started.
	float journeyLength = 1.0f;					// How far to jump.
	GameControllerScript gameController;		// The gamecontroller object.
	Rigidbody playerRigidbody;

	void Start () {
		// Get starting position.
		jumpDestination = transform.position;
		jumpStart = transform.position;
		startTime = Time.time;
		// Get the GameController Object.
		gameController = GameObject.FindGameObjectWithTag ("GameController")
			.GetComponent<GameControllerScript>() as GameControllerScript;
		// Get the Rigidbody Object.
		playerRigidbody = gameObject.GetComponent<Rigidbody>();
        // Set the color of the cube.
        colorMesh.GetComponent<MeshRenderer>().material.color = gameController.GetPlayerColor();
    }

	void Update () {
		// Don't do anything if the game is paused.
		if (gameController.IsPaused()) return;
		// Make the player actually move.
		if (moving) ContinueMovement();
		// Should the player move?
		HandleMovement();
	}

	void OnCollisionEnter (Collision coll) {
		// The player has been hit by a car.
		if (coll.gameObject.CompareTag ("Car") || coll.gameObject.CompareTag("Train"))
		{
			// TODO Show some sort of death animation.
			gameController.GameOver();
		}
	}

	void OnTriggerEnter (Collider other) {
		// Pick up coins.
		if (other.CompareTag ("Coin")) {
			gameController.CollectCoin ();
			Destroy (other.gameObject);
		}

		if (other.CompareTag("Water"))
		{
            gameController.GameOver();
        }
	}

	/// <summary>
	/// Handles the movement of the player.
	/// </summary>
	void HandleMovement () {
		// Forward movement.
		if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow) ||
		    Input.GetKeyDown(KeyCode.Space)) {
			JumpForward();
		} 
		// Backward movement.
		else if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
			JumpBackward();
		}
		// Leftward movement.
		else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
			JumpLeft();
		}
		// Rightward movement.
		else if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
			JumpRight();
		}
	}

	/// <summary>
	/// Continues the movement of the player.
	/// </summary>
	void ContinueMovement () {
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp (jumpStart, jumpDestination, fracJourney);
		if (Vector3.Distance (transform.position, jumpDestination) < snapToGridDistance) {
			moving = false;
			playerRigidbody.MovePosition (jumpDestination);
			// Start moving to the next destination, if it exists.
			if (queuedJumpDestination != Vector3.zero) {
				// Jump to the next position.
				Jump (queuedJumpDestination);
				// Remove the jump from the queue.
				queuedJumpDestination = Vector3.zero;
			}
		}
	}
	
	/// <summary>
	/// Jump the specified destination, or queue the next jump if already moving.
	/// </summary>
	/// <param name="destination">Destination.</param>
	void Jump (Vector3 destination) {
		// If the player is already moving, queue up the next jump.
		if (moving) {
			queuedJumpDestination = destination;
		}
		// If the player isn't already moving, jump to the specified destination.
		else {
			moving = true;
			jumpStart = transform.position;
			jumpDestination = destination;
			startTime = Time.time;
			// Trigger the jump animation.
			anim.SetTrigger("Jump");
			// Update the score. If the score isn't higher, the gameController won't update it.
			gameController.SetScore ((int)destination.z);
		}
	}

	/// <summary>
	/// Jump forward.
	/// </summary>
	void JumpForward () {
        Vector3 newPosition = jumpDestination + Vector3.forward;
        mesh.transform.rotation = Quaternion.Euler(0, 0, 0);
        Jump(newPosition);
    }

	/// <summary>
	/// Jump backward.
	/// </summary>
	void JumpBackward () {
        Vector3 newPosition = jumpDestination - Vector3.forward;
        mesh.transform.rotation = Quaternion.Euler(0, 180, 0);
        Jump(newPosition);
    }

	/// <summary>
	/// Jump left.
	/// </summary>
	void JumpLeft(){
		// Only allow the player to jump left if they haven't reached the left bound.
		if (transform.position.x >= leftBound)
		{
			Vector3 newPosition = jumpDestination - Vector3.right;
            mesh.transform.rotation = Quaternion.Euler(0, -90, 0);
            Jump(newPosition);
		}
	}

	/// <summary>
	/// Jump right.
	/// </summary>
	void JumpRight () {
        // Only allow the player to jump right if they haven't reached the right bound.
        if (transform.position.x <= rightBound)
        {
            Vector3 newPosition = jumpDestination + Vector3.right;
            mesh.transform.rotation = Quaternion.Euler(0, 90, 0);
            Jump(newPosition);
        }
    }
}
