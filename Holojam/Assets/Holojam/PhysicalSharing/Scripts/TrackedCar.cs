using UnityEngine;
using System.Collections;

public class TrackedCar : MonoBehaviour {

	public bool isReadyToMove = false;

	public GameObject refObj;

	public float disError;

	RandAnimation _ra;

	// Use this for initialization
	void Start () {
		
		_ra = GameObject.Find ("RandomAnimation").GetComponent<RandAnimation> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			StepperCommunication.getInstance ().forward (1);
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			StepperCommunication.getInstance ().backward (1);
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			StepperCommunication.getInstance ().left (1);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			StepperCommunication.getInstance ().right (1);
		}

		float angle = Quaternion.Dot(transform.rotation, refObj.transform.rotation);
		float dis = Vector3.Distance (new Vector3 (transform.position.x, 0, transform.position.z), new Vector3 (refObj.transform.position.x, 0, refObj.transform.position.z));
		if (dis < disError
		//	&& (Mathf.Abs (Mathf.Abs (angle) - 1.0f) < rotateError)
		) {
			_ra.disappear(transform.position);	
		} else {
			_ra.appear (gameObject.GetComponent<CarCtrl>().isReadyToMove);
			gameObject.SetActive (false);
		}

		if (_ra.isTimeToShow == 0)
			gameObject.SetActive (true);
	}
}
