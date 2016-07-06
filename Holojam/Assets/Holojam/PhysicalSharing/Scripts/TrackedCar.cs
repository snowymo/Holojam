using UnityEngine;
using System.Collections;

public class TrackedCar : MonoBehaviour {

	public bool isReadyToMove = false;

	// Use this for initialization
	void Start () {
	
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
	}
}
