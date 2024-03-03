using UnityEngine;
using System.Collections;

public class StripRemover : MonoBehaviour {

	public int deleteOffset = 5;
	
	Camera playerCamera;
	bool destroyOnNextFrame = false;

	void Start () {
		playerCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent <Camera>() as Camera;
	}

	void Update () {
		if (destroyOnNextFrame) {
			Destroy (gameObject);
		}
		// If the strip is off camera, destroy it.
		if (playerCamera.transform.position.z - deleteOffset > transform.position.z) {
			transform.position = new Vector3 (0f, -10, 0f);
			destroyOnNextFrame = true;
		}
	}
}
