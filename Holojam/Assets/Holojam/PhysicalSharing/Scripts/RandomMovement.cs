using UnityEngine;
using System.Collections;

// generate a new destination each time when the object arrived the destination

// and move it very smoothly

public class RandomMovement : MonoBehaviour {

	Vector3 destination;

	public float r;

	public float speed;

	Vector3 startPos;

	float journey;

	public GameObject finalPos;

	bool isRandomAnimating;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		generateDestination ();
		journey = Vector3.Distance (startPos, destination);
		isRandomAnimating = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			moving (finalPos.transform.position);
			isRandomAnimating = false;
		}
			
		float curDis = Vector3.Distance (this.transform.position, destination);
		if (curDis < 0.01) {
			if(isRandomAnimating)
				generateDestination ();
			startPos = transform.position;
			journey = Vector3.Distance (startPos, destination);
			curDis = Vector3.Distance (this.transform.position, destination);
		}
		if(journey != 0)
			transform.position = Vector3.Lerp(startPos, destination, 1 - curDis / journey + speed);
	}

	void generateDestination(){
		// let's assume the center is current position
		float dis = Random.Range(0,r);
		float angle_x = Random.Range (-90, 90);
		float angle_y = Random.Range (-90, 90);
		//Vector3 mark = this.transform.position;
		Vector3 translation = new Vector3 (0, 0, dis);
		Quaternion rotation = Quaternion.Euler (angle_x, angle_y, 0);
		//rotation = Quaternion.identity;
		Matrix4x4 m = Matrix4x4.TRS (translation, rotation, new Vector3 (1, 1, 1));
		destination = m.MultiplyPoint3x4 (this.transform.position);
		print ("dest:\t" + destination);
		//this.transform.position = mark;
	}

	void moving(Vector3 pos){
		destination = pos;

		startPos = transform.position;
		journey = Vector3.Distance (startPos, destination);
		float curDis = Vector3.Distance (this.transform.position, destination);
		transform.position = Vector3.Lerp(startPos, destination, 1 - curDis / journey + speed * 2);
	}
}
