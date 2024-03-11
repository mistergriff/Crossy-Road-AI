using UnityEngine;

public class CarController : MonoBehaviour {

	public float minSpeed = 1f;				// Minimum speed to move.
	public float maxSpeed = 10f;			// Maximum speed to move.
	public BoxCollider boxCollider;			// The box collider which detects potential collisions.
	public AudioSource honk;				// Sound to play if player is almost hit.
	public AudioClip[] honkSounds;			// All possible honks. One will be chosen at random.

	float speed = 1f;						// Current speed of the car.
	Vector3 velocity = new Vector3();       // Ref for the current velocity.
	public static CarController instance;


	void Update () {
		GetComponent<Rigidbody>().velocity = velocity;
	}

	void OnCollisionExit (Collision coll) {
		// If the car is no longer on the ground, destroy it.
		if (coll.gameObject.CompareTag ("Ground")) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter (Collision coll) {
		// Stop if the player was hit.
		if (coll.gameObject.CompareTag ("Player")) {
			if (gameObject.CompareTag("Wood"))
			{
				return;
			}
			velocity = Vector3.zero;
		}
	}

	void OnTriggerEnter (Collider other) {
		// Don't crash!
		if (other.CompareTag ("Car")) {
			velocity = (other.GetComponent<CarController>() as CarController).GetVelocity();
		}
        else if (other.CompareTag("Train"))
        {
            velocity = (other.GetComponent<CarController>() as CarController).GetVelocity();
        }
        // Honk if the player is almost hit by the car.
        else if (other.CompareTag ("Player")) {
			honk.clip = honkSounds[Random.Range (0, honkSounds.Length)];
			honk.Play();
		}
	}

	/// <summary>
	/// Move the specified direction.
	/// </summary>
	/// <param name="direction">Direction.</param>
	public void Move (float direction) {
		// Move the direction provided.
		speed = Random.Range (minSpeed, maxSpeed) * direction;
		velocity = Vector3.right * speed;

		

	}

	/// <summary>
	/// Gets the car's velocity.
	/// </summary>
	/// <returns>The velocity.</returns>
	public Vector3 GetVelocity () {
		return velocity;
	}
}
