using UnityEngine;
using System.Collections;

public class wandDoubleCtrl : wandCtrl {

	public GameObject _robot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// check if selected, and highlight it
		isCollide = isCollision ();
		if (isCollide) {
			objChosen.highlight ();
		} else {
			uncheckAll ();
		}

		// draw the ray of wand, TODO maybe activate when button B is clicked
		drawRays ();

		if (Input.GetKeyDown (KeyCode.Space)) {
			print ("click space");
			if (isCollide) {
				unlinkAll ();
				objChosen.moveWanimation ();
				_robot.GetComponent<HPDCtrl> ().setDestination (objChosen._path.getPoint(1f));
			} 
		}
	}
}
