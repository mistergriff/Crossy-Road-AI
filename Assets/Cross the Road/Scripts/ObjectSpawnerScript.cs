using UnityEngine;
using System.Collections;

public class ObjectSpawnerScript : MonoBehaviour {

	public Transform objectPrefab;		// Object to spawn.
	public float minSpawnTime = 1.0f;	// Minimum time between spawns.
	public float maxSpawnTime = 3.0f;	// Maximum time between spawns.
	public bool moveLeft = true;		// Move left or right.
	public bool initialSpawn = true;	// Spawn an object instantly on creation?

	bool isWaitingForSpawn = false;
	float direction = 1.0f;

	// Use this for initialization
	void Start () {
		if (objectPrefab == null) {
			// No objectPrefab was provided. Delete this gameObject.
			Debug.LogError ("Please provide a prefab to the ObjectSpawnerScript.");
			Destroy (gameObject);
		}
		if (moveLeft) {
			// Should objects be pushed left?
			direction = -1.0f;
		}
		// Start with one obejct if desired.
		if (initialSpawn)
			SpawnObject();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isWaitingForSpawn) {
			StartCoroutine ("SpawnObjectAfter", Random.Range (minSpawnTime, maxSpawnTime));
			isWaitingForSpawn = true;
		}
	}

	/// <summary>
	/// Spawns the object after a certain amount of time.
	/// </summary>
	/// <returns>An IEnumerator for waiting.</returns>
	/// <param name="seconds">Seconds to wait before spawning the object.</param>
	IEnumerator SpawnObjectAfter (float seconds) {
		yield return new WaitForSeconds (seconds);
		SpawnObject();
		isWaitingForSpawn = false;
	}

	/// <summary>
	/// Spawns the object which corresponds to the objectPrefab.
	/// </summary>
	void SpawnObject () {
		CarController car;
		Vector3 trainOffset = new Vector3 (0, 0.1f, 0);

		if (objectPrefab.CompareTag("Train"))
		{
			trainOffset = new Vector3(0, 0.3f, 0);
		}
		car = ((Instantiate (objectPrefab, transform.position + trainOffset, Quaternion.identity) as Transform)
		       .GetComponent<CarController>() as CarController);
		// Push the object either left or right.
		if (direction == -1f){
			car.transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        }
		car.Move(direction);
	}
}
