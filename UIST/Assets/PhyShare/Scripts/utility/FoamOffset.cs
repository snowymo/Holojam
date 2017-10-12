using UnityEngine;
using System.Collections;

public class FoamOffset : MonoBehaviour {

	public bool isFoam;
	public Vector3 offset;

	// Use this for initialization
	void Start () {
		offset += GetComponent<boardOffset> ().offset;
	}
	
	// Update is called once per frame
	void Update () {
		if (isFoam) {
			GetComponent<boardOffset> ().offset = offset;

		}
	}
}
