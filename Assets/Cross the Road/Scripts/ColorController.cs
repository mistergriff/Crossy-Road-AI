using UnityEngine;
using System.Collections;

public class ColorController : MonoBehaviour {

	public Color color = Color.white;
	public GameObject mesh;

	// Use this for initialization
	void Start () {
		// Set the color of the mesh.
		mesh.GetComponent<Renderer>().material.color = color;
	}
}
