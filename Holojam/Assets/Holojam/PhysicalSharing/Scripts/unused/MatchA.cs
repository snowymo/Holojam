using UnityEngine;
using System.Collections;

public class MatchA : MonoBehaviour {

	public GameObject trackedCar;

	public GameObject referenceCar;

	public Vector3 offset;

	public GameObject car;

	public GameObject frame;

	TrackedCarA tc_tc;
	//CarCtrl tc_cc;

	// Use this for initialization
	void Start () {
		tc_tc = trackedCar.GetComponent<TrackedCarA> ();
		//tc_cc = trackedCar.GetComponent<CarCtrl> ();
	}

	// Update is called once per frame
	void Update () {
		if (!tc_tc.isReadyToMove) {
			if (Utility.getInst().checkMatchV2 (this.transform.position, trackedCar.transform.position)) {
				tc_tc.isReadyToMove = true;
				//if(tc_cc!= null)
				//	tc_cc.isReadyToMove = true;
				print ("match finished");
				// for test now so that I won't hide the start ball
				//this.enabled = false;
				//this.gameObject.SetActive (false);
				//GetComponent(MeshRenderer) = false;

				car.gameObject.SetActive(false);
				frame.gameObject.SetActive(false);

			}
		}
		// place the start ball where reference plus offset is all the time
		this.transform.rotation = referenceCar.transform.rotation;
		this.transform.position = referenceCar.transform.position + offset;
	}


}
