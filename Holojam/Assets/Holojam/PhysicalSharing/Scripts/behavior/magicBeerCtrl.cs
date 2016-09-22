using UnityEngine;
using System.Collections;

public class magicBeerCtrl : MonoBehaviour {

	public GameObject reference;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// for test
		transform.position = reference.transform.position;
		transform.rotation = reference.transform.rotation;
	}
}
