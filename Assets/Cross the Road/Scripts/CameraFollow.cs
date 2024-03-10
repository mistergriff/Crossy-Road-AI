using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform player;					// The player object's transform.
	public float smoothTime = 0.75f;			// How long it takes the camera to move to the player's location.
	public GameControllerScript gameController;	// The game controller.
	public float offScreenOffset = 1.5f;		// How far does the player have to be from the camera to be considered off screen?
	
	Vector3 velocity = Vector3.zero;			// Reference velocity.
	Vector3 curTarget;							// What are we trying to move towards?

	void Start () {
		// Initalize the current target.
		curTarget = player.position;
	}

	// Update is called once per frame
	void Update () {
		// Follow the player.
		Follow();
		// Check if the player is off screen, behind the camera.
		CheckPlayerOffScreen();
	}

	/// <summary>
	/// Follows the player.
	/// </summary>
	void Follow () {
		// Follow the player.
		Vector3 target = player.position;
		// Don't allow the camera to move backwards.
		if (player.position.z <= transform.position.z) {
			target.z = curTarget.z;
		}
		curTarget = target;
		// Move smoothly towards the target.
		transform.position = Vector3.SmoothDamp (transform.position, target, ref velocity, smoothTime);
	}

	/// <summary>
	/// Checks if the player is off screen, and needs to be killed.
	/// </summary>
	void CheckPlayerOffScreen () {
		if (player.position.z < transform.position.z - offScreenOffset) {
			gameController.GameOver();
		}
	}
}
