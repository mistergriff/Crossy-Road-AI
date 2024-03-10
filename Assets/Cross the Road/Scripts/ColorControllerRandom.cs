using UnityEngine;
using System.Collections;

public class ColorControllerRandom : MonoBehaviour {

	public Color[] colors = {Color.white};
	public GameObject mesh;

	// Use this for initialization
	void Start () {
		int rand = Random.Range (0, colors.Length);
		// Set the color of the mesh.
		mesh.GetComponent<Renderer>().material.color = colors[rand];
	}
}
