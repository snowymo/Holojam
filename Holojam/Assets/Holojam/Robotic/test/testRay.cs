using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.up, out hit)) {
			print ("hit: " + hit.collider);
			GestureTarget gt = hit.transform.GetComponent<GestureTarget> ();
			if (gt != null) {
				print ("gt: " + gt);
			}
		}
		//Debug
		Debug.DrawRay (transform.position, -transform.up * 10, Color.yellow);
	}
}
