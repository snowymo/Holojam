using UnityEngine;
using System.Collections;

public class MatchB : MonoBehaviour {

	public Vector3 offset;

	public GameObject referenceCar;

	public GameObject trackedCar;

	TrackedCar tc_tc;
	CarCtrl tc_cc;

	// Use this for initialization
	void Start () {
		tc_tc = trackedCar.GetComponent<TrackedCar> ();
	}

	// Update is called once per frame
	void Update () {
		if (!tc_tc.isReadyToMove) {
			if (Utility.getInst().checkMatchV2 (this.transform.position, trackedCar.transform.position)) {
				tc_tc.isReadyToMove = true;
				print ("match finished");
			}
		}
		// place the start ball where reference plus offset is
		this.transform.rotation = referenceCar.transform.rotation;
		this.transform.position = referenceCar.transform.position + offset;
	}


}
