using UnityEngine;
using System.Collections;

public class CoinRemover : MonoBehaviour {

	/// <summary>
	/// Once the coin isn't visible by any cameras, destroy it.
	/// The editor's camera counts as a camera, so behavior is different
	/// when run in the editor.
	/// </summary>
	void OnBecameInvisible () {
		Destroy (gameObject);
	}
}
